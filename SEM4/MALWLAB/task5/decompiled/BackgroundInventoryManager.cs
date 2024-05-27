// Decompiled with JetBrains decompiler
// Type: SolarWinds.Orion.Core.BusinessLayer.BackgroundInventoryManager
// Assembly: SolarWinds.Orion.Core.BusinessLayer, Version=2019.4.5200.9083, Culture=neutral, PublicKeyToken=null
// MVID: E12E8C85-5CD9-4E06-8801-182E5104FADE
// Assembly location: E:\task5.dll

using SolarWinds.Logging;
using SolarWinds.Orion.Core.BusinessLayer.DAL;
using SolarWinds.Orion.Core.Common;
using SolarWinds.Orion.Core.Common.DALs;
using SolarWinds.Orion.Core.Common.Models;
using SolarWinds.Orion.Core.Models.Credentials;
using SolarWinds.Orion.Core.Pollers;
using SolarWinds.Orion.Core.Pollers.Cpu.SNMP;
using SolarWinds.Orion.Core.Pollers.Memory.SNMP;
using SolarWinds.Orion.Core.SharedCredentials;
using SolarWinds.Orion.Pollers.Framework;
using SolarWinds.Orion.Pollers.Framework.SNMP;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;

#nullable disable
namespace SolarWinds.Orion.Core.BusinessLayer
{
  internal class BackgroundInventoryManager : IDisposable
  {
    private static readonly Log log = new Log();
    private QueuedTaskScheduler<BackgroundInventoryManager.InventoryTask> scheduler;
    private SnmpGlobalSettings globals;
    private PollersDAL pollersDAL = new PollersDAL();
    private Dictionary<string, BackgroundInventoryManager.DoInventoryDelegate> inventories;
    private bool disposed;

    public bool IsRunning => this.scheduler.IsRunning;

    public int QueueSize => this.scheduler.QueueSize;

    public BackgroundInventoryManager(int parallelTasksCount)
    {
      this.scheduler = new QueuedTaskScheduler<BackgroundInventoryManager.InventoryTask>(new QueuedTaskScheduler<BackgroundInventoryManager.InventoryTask>.TaskProcessingRoutine(this.DoInventory), parallelTasksCount);
      this.scheduler.TaskProcessingFinished += new EventHandler(this.scheduler_TaskProcessingFinished);
      this.globals = new SnmpGlobalSettings();
      this.globals.MaxReplies = SettingsDAL.GetCurrentInt("SWNetPerfMon-Settings-SNMP MaxReps", 5);
      this.globals.RequestTimeout = SettingsDAL.GetCurrentInt("SWNetPerfMon-Settings-SNMP Timeout", 2500);
      this.globals.RequestRetries = SettingsDAL.GetCurrentInt("SWNetPerfMon-Settings-SNMP Retries", 2);
      this.globals.HsrpEnabled = SettingsDAL.GetCurrent<bool>("SWNetPerfMon-Settings-SNMP HSRPEnabled", true);
      this.inventories = new Dictionary<string, BackgroundInventoryManager.DoInventoryDelegate>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      this.inventories.Add("Cpu", new BackgroundInventoryManager.DoInventoryDelegate(this.DoCpuInventory));
      this.inventories.Add("Memory", new BackgroundInventoryManager.DoInventoryDelegate(this.DoMemoryInventory));
    }

    private void scheduler_TaskProcessingFinished(object sender, EventArgs e)
    {
      BackgroundInventoryManager.log.Info((object) "Background Inventorying Finished");
    }

    public void Enqueue(int nodeID, string settings)
    {
      this.scheduler.EnqueueTask(new BackgroundInventoryManager.InventoryTask(nodeID, settings));
    }

    public void Start() => this.scheduler.Start();

    public void Cancel() => this.scheduler.Cancel();

    internal void DoInventory(BackgroundInventoryManager.InventoryTask task)
    {
      Stopwatch stopwatch = Stopwatch.StartNew();
      Node node = NodeBLDAL.GetNode(task.NodeID);
      if (node == null || node.PolledStatus != 1)
      {
        BackgroundInventoryManager.log.InfoFormat("Skipping inventorying of Node {0}, status is not UP.", (object) task.NodeID);
      }
      else
      {
        SnmpSettings nodeSettings = new SnmpSettings()
        {
          AgentPort = (int) node.SNMPPort,
          ProtocolVersion = (SNMPVersion) node.SNMPVersion,
          TargetIP = IPAddress.Parse(node.IpAddress)
        };
        SnmpInventorySettings inventorySettings = new SnmpInventorySettings(node.SysObjectID);
        SnmpCredentials credentialsFromNode = CredentialHelper.ParseCredentialsFromNode(node) as SnmpCredentials;
        List<string> detectedPollers = new List<string>();
        if (BackgroundInventoryManager.log.IsInfoEnabled)
          BackgroundInventoryManager.log.InfoFormat("Starting inventorying of Node {0}, NeedsInventory = '{1}'", (object) task.NodeID, (object) task.Settings);
        string[] array = ((IEnumerable<string>) task.Settings.Split(':')).Distinct<string>().ToArray<string>();
        List<string> failedTasks = new List<string>();
        List<string> completedTasks = new List<string>();
        foreach (string key in array)
        {
          if (!this.inventories.ContainsKey(key))
          {
            failedTasks.Add(key);
            if (BackgroundInventoryManager.log.IsErrorEnabled)
              BackgroundInventoryManager.log.ErrorFormat("Unable to inventory '{0}' on Node {1}", (object) key, (object) task.NodeID);
          }
          else
          {
            if (this.scheduler.IsTaskCanceled)
            {
              if (BackgroundInventoryManager.log.IsInfoEnabled)
                BackgroundInventoryManager.log.InfoFormat("Inventorying of Node {0} was canceled. ElapsedTime = {1}", (object) task.NodeID, (object) stopwatch.ElapsedMilliseconds);
              stopwatch.Stop();
              return;
            }
            InventoryPollersResult inventoryPollersResult = this.inventories[key](nodeSettings, inventorySettings, credentialsFromNode);
            if (inventoryPollersResult == null)
            {
              failedTasks.Add(key);
              if (BackgroundInventoryManager.log.IsErrorEnabled)
                BackgroundInventoryManager.log.ErrorFormat("Inventory '{0}' on Node {1} returned null result", (object) key, (object) task.NodeID);
            }
            else if (((InventoryResultBase) inventoryPollersResult).Outcome == 1)
            {
              completedTasks.Add(key);
              detectedPollers.AddRange((IEnumerable<string>) inventoryPollersResult.PollerTypes);
            }
            else
            {
              failedTasks.Add(key);
              if (((InventoryResultBase) inventoryPollersResult).Error != null)
              {
                if (BackgroundInventoryManager.log.IsWarnEnabled)
                  BackgroundInventoryManager.log.WarnFormat("Inventory '{0}' on Node {1} failed with code {2}", (object) key, (object) task.NodeID, (object) ((InventoryResultBase) inventoryPollersResult).Error.ErrorCode);
                if (((InventoryResultBase) inventoryPollersResult).Error.ErrorCode != 31002U)
                {
                  List<string> list = ((IEnumerable<string>) array).Where<string>((Func<string, bool>) (n => !completedTasks.Contains(n) && !failedTasks.Contains(n))).ToList<string>();
                  if (list.Count > 0)
                  {
                    failedTasks.AddRange((IEnumerable<string>) list);
                    if (BackgroundInventoryManager.log.IsWarnEnabled)
                    {
                      BackgroundInventoryManager.log.WarnFormat("Skipping inventory for '{0}' on Node {1}", (object) string.Join(":", list.ToArray()), (object) task.NodeID);
                      break;
                    }
                    break;
                  }
                }
              }
              else if (BackgroundInventoryManager.log.IsWarnEnabled)
                BackgroundInventoryManager.log.WarnFormat("Inventory '{0}' on Node {1} failed on unknown error", (object) key, (object) task.NodeID);
            }
          }
        }
        string lastNodeSettings = NodeSettingsDAL.GetLastNodeSettings(task.NodeID, CoreConstants.NeedsInventoryFlag);
        if ((string.IsNullOrEmpty(lastNodeSettings) || !lastNodeSettings.Equals(task.Settings, StringComparison.OrdinalIgnoreCase)) && BackgroundInventoryManager.log.IsInfoEnabled)
        {
          BackgroundInventoryManager.log.InfoFormat("Skipping inventory result processing for Node {0}, NeedsInventory flag changed. OldValue = '{1}', NewValue = '{2}'.", (object) task.NodeID, (object) task.Settings, (object) lastNodeSettings);
        }
        else
        {
          this.InsertDetectedPollers(task, detectedPollers);
          if (failedTasks.Count == 0)
          {
            NodeSettingsDAL.DeleteSpecificSettingForNode(task.NodeID, CoreConstants.NeedsInventoryFlag);
            if (BackgroundInventoryManager.log.IsInfoEnabled)
              BackgroundInventoryManager.log.InfoFormat("Inventorying of Node {0} completed in {1}ms.", (object) task.NodeID, (object) stopwatch.ElapsedMilliseconds);
          }
          else if (failedTasks.Count < array.Length)
          {
            string str = string.Join(":", failedTasks.ToArray());
            NodeSettingsDAL.SafeInsertNodeSetting(task.NodeID, CoreConstants.NeedsInventoryFlag, (object) str);
            if (BackgroundInventoryManager.log.IsInfoEnabled)
              BackgroundInventoryManager.log.InfoFormat("Inventorying of Node {0} partially completed in {1}ms. NeedsInventory updated to '{2}'", (object) task.NodeID, (object) stopwatch.ElapsedMilliseconds, (object) str);
          }
          else if (BackgroundInventoryManager.log.IsInfoEnabled)
            BackgroundInventoryManager.log.InfoFormat("Inventorying of Node {0} failed. Elapsed time {1}ms.", (object) task.NodeID, (object) stopwatch.ElapsedMilliseconds);
          stopwatch.Stop();
        }
      }
    }

    internal void InsertDetectedPollers(
      BackgroundInventoryManager.InventoryTask task,
      List<string> detectedPollers)
    {
      if (detectedPollers.Count <= 0)
        return;
      this.pollersDAL.UpdateNetObjectPollers("N", task.NodeID, detectedPollers.ToArray());
    }

    private InventoryPollersResult DoCpuInventory(
      SnmpSettings nodeSettings,
      SnmpInventorySettings inventorySettings,
      SnmpCredentials credentials)
    {
      return ((InventoryBase) new CpuSnmpInventory()).DoInventory((GlobalSettingsBase) this.globals, (SettingsBase) nodeSettings, (InventorySettingsBase) inventorySettings, (Credential) credentials) as InventoryPollersResult;
    }

    private InventoryPollersResult DoMemoryInventory(
      SnmpSettings nodeSettings,
      SnmpInventorySettings inventorySettings,
      SnmpCredentials credentials)
    {
      return ((InventoryBase) new MemorySnmpInventory()).DoInventory((GlobalSettingsBase) this.globals, (SettingsBase) nodeSettings, (InventorySettingsBase) inventorySettings, (Credential) credentials) as InventoryPollersResult;
    }

    private void Dispose(bool disposing)
    {
      if (this.disposed)
        return;
      if (disposing && this.scheduler != null)
      {
        this.scheduler.Dispose();
        this.scheduler = (QueuedTaskScheduler<BackgroundInventoryManager.InventoryTask>) null;
      }
      this.disposed = true;
    }

    public void Dispose()
    {
      this.Dispose(true);
      GC.SuppressFinalize((object) this);
    }

    ~BackgroundInventoryManager() => this.Dispose(false);

    public delegate InventoryPollersResult DoInventoryDelegate(
      SnmpSettings nodeSettings,
      SnmpInventorySettings inventorySettings,
      SnmpCredentials credentials);

    public class InventoryTask
    {
      public int NodeID;
      public string Settings;

      public InventoryTask(int nodeID, string settings)
      {
        this.NodeID = nodeID;
        this.Settings = settings;
      }

      public override string ToString()
      {
        return string.Format("NodeID = {0}, Settings = {1}", (object) this.NodeID, (object) this.Settings);
      }
    }
  }
}
