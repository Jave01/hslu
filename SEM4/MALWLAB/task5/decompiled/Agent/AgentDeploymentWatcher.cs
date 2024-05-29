// Decompiled with JetBrains decompiler
// Type: SolarWinds.Orion.Core.BusinessLayer.Agent.AgentDeploymentWatcher
// Assembly: SolarWinds.Orion.Core.BusinessLayer, Version=2019.4.5200.9083, Culture=neutral, PublicKeyToken=null
// MVID: E12E8C85-5CD9-4E06-8801-182E5104FADE
// Assembly location: E:\task5.dll

using SolarWinds.Logging;
using SolarWinds.Orion.Core.BusinessLayer.DAL;
using SolarWinds.Orion.Core.Common.Agent;
using SolarWinds.Orion.Core.Discovery;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

#nullable disable
namespace SolarWinds.Orion.Core.BusinessLayer.Agent
{
  internal class AgentDeploymentWatcher
  {
    private static readonly Log log = new Log();
    private const int MaxItemAge = 120;
    private const int CheckPeriod = 5;
    private Timer watcherTimer;
    private static readonly object syncLockItems = new object();
    private static AgentDeploymentWatcher instance;
    private static readonly object syncLockInstance = new object();

    private AgentDeploymentWatcher()
    {
    }

    private AgentDeploymentWatcher(IAgentInfoDAL agentInfoDal)
    {
      this.AgentInfoDal = agentInfoDal;
      this.Items = new Dictionary<int, AgentDeploymentWatcher.CacheItem>();
      this.NotificationSubscriber = new AgentNotificationSubscriber(new Action<int>(this.AgentNotification));
    }

    private IAgentInfoDAL AgentInfoDal { get; set; }

    private AgentNotificationSubscriber NotificationSubscriber { get; set; }

    private Dictionary<int, AgentDeploymentWatcher.CacheItem> Items { get; set; }

    public static AgentDeploymentWatcher GetInstance(IAgentInfoDAL agentInfoDal)
    {
      if (AgentDeploymentWatcher.instance != null)
        return AgentDeploymentWatcher.instance;
      lock (AgentDeploymentWatcher.syncLockInstance)
      {
        if (AgentDeploymentWatcher.instance == null)
          AgentDeploymentWatcher.instance = new AgentDeploymentWatcher(agentInfoDal);
      }
      return AgentDeploymentWatcher.instance;
    }

    public void Start()
    {
      AgentDeploymentWatcher.log.Debug((object) "AgentDeploymentWatcher.Start");
      lock (AgentDeploymentWatcher.syncLockItems)
        this.CheckWatcher();
    }

    public void AddOrUpdateDeploymentInfo(AgentDeploymentInfo deploymentInfo)
    {
      if (deploymentInfo == null)
        throw new ArgumentNullException(nameof (deploymentInfo));
      if (deploymentInfo.Agent == null)
        throw new ArgumentNullException("deploymentInfo.Agent");
      AgentDeploymentWatcher.log.DebugFormat("AddOrUpdateDeploymentInfo started, agentId:{0}, status:{1}", (object) deploymentInfo.Agent.AgentId, (object) deploymentInfo.StatusInfo.Status);
      lock (AgentDeploymentWatcher.syncLockItems)
      {
        AgentDeploymentWatcher.CacheItem cacheItem;
        if (this.Items.TryGetValue(deploymentInfo.Agent.AgentId, out cacheItem))
        {
          AgentDeploymentWatcher.log.Debug((object) "AddOrUpdateDeploymentInfo - item found in cache, updating");
          cacheItem.DeploymentInfo = deploymentInfo;
        }
        else
        {
          AgentDeploymentWatcher.log.Debug((object) "AddOrUpdateDeploymentInfo - item not found in cache, creating new item");
          cacheItem = new AgentDeploymentWatcher.CacheItem()
          {
            DeploymentInfo = deploymentInfo
          };
          cacheItem.LastChecked = DateTime.Now;
          this.Items[deploymentInfo.Agent.AgentId] = cacheItem;
        }
        cacheItem.LastUpdated = DateTime.Now;
        cacheItem.RefreshNeeded = false;
        this.CheckWatcher();
      }
    }

    public void SetOnFinishedCallback(int agentId, Action<AgentDeploymentStatus> onFinished)
    {
      AgentDeploymentWatcher.log.DebugFormat("SetOnFinishedCallback entered, agentId:{0}", (object) agentId);
      lock (AgentDeploymentWatcher.syncLockItems)
      {
        AgentDeploymentWatcher.CacheItem cacheItem;
        if (!this.Items.TryGetValue(agentId, out cacheItem))
          return;
        cacheItem.OnFinishedCallback = onFinished;
      }
    }

    public AgentDeploymentInfo GetAgentDeploymentInfo(int agentId)
    {
      AgentDeploymentWatcher.log.DebugFormat("GetAgentDeploymentInfo started, agentId:{0}", (object) agentId);
      AgentDeploymentInfo agentDeploymentInfo = (AgentDeploymentInfo) null;
      lock (AgentDeploymentWatcher.syncLockItems)
      {
        AgentDeploymentWatcher.CacheItem cacheItem;
        if (this.Items.TryGetValue(agentId, out cacheItem))
        {
          agentDeploymentInfo = cacheItem.DeploymentInfo;
          cacheItem.LastChecked = DateTime.Now;
          AgentDeploymentWatcher.log.DebugFormat("GetAgentDeploymentInfo - item found in cache, agentId:{0}, status:{1}", (object) agentId, (object) agentDeploymentInfo.StatusInfo.Status);
        }
      }
      if (agentDeploymentInfo == null)
      {
        agentDeploymentInfo = this.LoadAgentDeploymentInfo(agentId);
        AgentDeploymentWatcher.log.DebugFormat("GetAgentDeploymentInfo - item not found in cache, loading from db, agentId:{0}, status:{1}", (object) agentId, (object) agentDeploymentInfo.StatusInfo.Status);
      }
      return agentDeploymentInfo;
    }

    private void CheckWatcher()
    {
      AgentDeploymentWatcher.log.Debug((object) "Checking Watcher status");
      if (!this.NotificationSubscriber.IsSubscribed())
      {
        AgentDeploymentWatcher.log.Debug((object) "Starting NotificationSubscriber");
        this.NotificationSubscriber.Subscribe();
      }
      if (this.watcherTimer != null)
        return;
      AgentDeploymentWatcher.log.Debug((object) "Starting Watcher Timer");
      this.watcherTimer = new Timer(new TimerCallback(this.CheckItems), (object) null, TimeSpan.FromSeconds(5.0), TimeSpan.FromSeconds(5.0));
    }

    private void StopWatcher()
    {
      if (this.watcherTimer != null)
        this.watcherTimer.Dispose();
      this.watcherTimer = (Timer) null;
      if (this.NotificationSubscriber == null)
        return;
      this.NotificationSubscriber.Unsubscribe();
    }

    private void CheckItems(object state)
    {
      AgentDeploymentWatcher.log.Debug((object) "CheckItems started");
      DateTime ExpireTime = DateTime.Now.Subtract(TimeSpan.FromSeconds(120.0));
      List<Action<AgentDeploymentStatus>> actionList = new List<Action<AgentDeploymentStatus>>();
      IEnumerable<AgentDeploymentWatcher.CacheItem> array1;
      IEnumerable<int> array2;
      lock (AgentDeploymentWatcher.syncLockItems)
      {
        AgentDeploymentWatcher.log.Debug((object) "CheckItems - looking for items to remove");
        array1 = (IEnumerable<AgentDeploymentWatcher.CacheItem>) this.Items.Where<KeyValuePair<int, AgentDeploymentWatcher.CacheItem>>((Func<KeyValuePair<int, AgentDeploymentWatcher.CacheItem>, bool>) (item => item.Value.DeploymentInfo.StatusInfo.Status == 1 || item.Value.DeploymentInfo.StatusInfo.Status == 2 || item.Value.LastChecked < ExpireTime)).Select<KeyValuePair<int, AgentDeploymentWatcher.CacheItem>, AgentDeploymentWatcher.CacheItem>((Func<KeyValuePair<int, AgentDeploymentWatcher.CacheItem>, AgentDeploymentWatcher.CacheItem>) (item => item.Value)).ToArray<AgentDeploymentWatcher.CacheItem>();
        foreach (AgentDeploymentWatcher.CacheItem cacheItem in array1)
        {
          AgentDeploymentWatcher.log.DebugFormat("CheckItems - removing item, AgentId:{0}", (object) cacheItem.DeploymentInfo.Agent.AgentId);
          this.Items.Remove(cacheItem.DeploymentInfo.Agent.AgentId);
        }
        if (this.Items.Count == 0)
        {
          AgentDeploymentWatcher.log.Debug((object) "CheckItems - No remaining items in the cache, stopping Watcher");
          this.StopWatcher();
        }
        AgentDeploymentWatcher.log.Debug((object) "CheckItems - looking for items to refresh");
        array2 = (IEnumerable<int>) this.Items.Where<KeyValuePair<int, AgentDeploymentWatcher.CacheItem>>((Func<KeyValuePair<int, AgentDeploymentWatcher.CacheItem>, bool>) (item => item.Value.RefreshNeeded || item.Value.LastUpdated < ExpireTime)).Select<KeyValuePair<int, AgentDeploymentWatcher.CacheItem>, int>((Func<KeyValuePair<int, AgentDeploymentWatcher.CacheItem>, int>) (item => item.Key)).ToArray<int>();
      }
      foreach (int agentId in array2)
      {
        AgentDeploymentWatcher.log.DebugFormat("CheckItems - refreshing item AgentId:{0}", (object) agentId);
        AgentDeploymentInfo agentDeploymentInfo = this.LoadAgentDeploymentInfo(agentId);
        lock (AgentDeploymentWatcher.syncLockItems)
        {
          AgentDeploymentWatcher.CacheItem cacheItem;
          if (this.Items.TryGetValue(agentDeploymentInfo.Agent.AgentId, out cacheItem))
          {
            AgentDeploymentWatcher.log.DebugFormat("CheckItems - updating item AgentId:{0}, Status:{1}", (object) agentId, (object) agentDeploymentInfo.StatusInfo.Status);
            cacheItem.DeploymentInfo = agentDeploymentInfo;
            cacheItem.LastUpdated = DateTime.Now;
            cacheItem.RefreshNeeded = false;
          }
          else
            AgentDeploymentWatcher.log.Debug((object) "CheckItems - item not found in the cache");
        }
      }
      foreach (AgentDeploymentWatcher.CacheItem cacheItem in array1)
      {
        if (cacheItem.OnFinishedCallback != null && (cacheItem.DeploymentInfo.StatusInfo.Status == 1 || cacheItem.DeploymentInfo.StatusInfo.Status == 2))
          cacheItem.OnFinishedCallback(cacheItem.DeploymentInfo.StatusInfo.Status);
      }
    }

    private void AgentNotification(int agentId)
    {
      AgentDeploymentWatcher.log.DebugFormat("AgentNotification started, AgentId:{0}", (object) agentId);
      lock (AgentDeploymentWatcher.syncLockItems)
      {
        AgentDeploymentWatcher.CacheItem cacheItem;
        if (this.Items.TryGetValue(agentId, out cacheItem))
        {
          AgentDeploymentWatcher.log.Debug((object) "AgentNotification - item found, set refresh flag.");
          cacheItem.RefreshNeeded = true;
        }
        else
          AgentDeploymentWatcher.log.Debug((object) "AgentNotification - item not found");
      }
    }

    private AgentDeploymentInfo LoadAgentDeploymentInfo(int agentId)
    {
      AgentInfo agentInfo = new AgentManager(this.AgentInfoDal).GetAgentInfo(agentId);
      if (agentInfo != null)
      {
        string[] discoveryPluginIds = DiscoveryHelper.GetAgentDiscoveryPluginIds();
        return AgentDeploymentInfo.Calculate(agentInfo, (IEnumerable<string>) discoveryPluginIds, (string) null);
      }
      return new AgentDeploymentInfo()
      {
        Agent = new AgentInfo() { AgentId = agentId },
        StatusInfo = new AgentDeploymentStatusInfo(agentId, (AgentDeploymentStatus) 2, "Agent not found.")
      };
    }

    private class CacheItem
    {
      public AgentDeploymentInfo DeploymentInfo { get; set; }

      public DateTime LastUpdated { get; set; }

      public DateTime LastChecked { get; set; }

      public bool RefreshNeeded { get; set; }

      public Action<AgentDeploymentStatus> OnFinishedCallback { get; set; }
    }
  }
}
