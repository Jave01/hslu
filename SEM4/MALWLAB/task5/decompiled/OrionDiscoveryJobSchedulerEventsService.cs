// Decompiled with JetBrains decompiler
// Type: SolarWinds.Orion.Core.BusinessLayer.OrionDiscoveryJobSchedulerEventsService
// Assembly: SolarWinds.Orion.Core.BusinessLayer, Version=2019.4.5200.9083, Culture=neutral, PublicKeyToken=null
// MVID: E12E8C85-5CD9-4E06-8801-182E5104FADE
// Assembly location: E:\task5.dll

using SolarWinds.InformationService.Linq;
using SolarWinds.InformationService.Linq.Plugins;
using SolarWinds.InformationService.Linq.Plugins.Core.Orion;
using SolarWinds.JobEngine;
using SolarWinds.Logging;
using SolarWinds.Orion.Common;
using SolarWinds.Orion.Common.IO;
using SolarWinds.Orion.Core.BusinessLayer.DAL;
using SolarWinds.Orion.Core.BusinessLayer.Discovery;
using SolarWinds.Orion.Core.Common;
using SolarWinds.Orion.Core.Common.Agent;
using SolarWinds.Orion.Core.Common.i18n;
using SolarWinds.Orion.Core.Common.JobEngine;
using SolarWinds.Orion.Core.Common.Models;
using SolarWinds.Orion.Core.Common.Swis;
using SolarWinds.Orion.Core.Discovery;
using SolarWinds.Orion.Core.Discovery.DAL;
using SolarWinds.Orion.Core.Discovery.DataAccess;
using SolarWinds.Orion.Core.Models.DiscoveredObjects;
using SolarWinds.Orion.Core.Models.Discovery;
using SolarWinds.Orion.Core.Models.Enums;
using SolarWinds.Orion.Core.Models.Interfaces;
using SolarWinds.Orion.Core.Models.OldDiscoveryModels;
using SolarWinds.Orion.Core.SharedCredentials;
using SolarWinds.Orion.Core.Strings;
using SolarWinds.Orion.Discovery.Contract.DiscoveryPlugin;
using SolarWinds.Orion.Discovery.Framework.Pluggability;
using SolarWinds.Orion.Discovery.Job;
using SolarWinds.Serialization.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

#nullable disable
namespace SolarWinds.Orion.Core.BusinessLayer
{
  [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, IncludeExceptionDetailInFaults = true, AutomaticSessionShutdown = true, ConcurrencyMode = ConcurrencyMode.Multiple)]
  public class OrionDiscoveryJobSchedulerEventsService : JobSchedulerEventServicev2
  {
    protected static readonly Log resultLog = new Log("DiscoveryResultLog");
    private readonly IOneTimeAgentDiscoveryJobFactory oneTimeAgentJobFactory;
    private readonly IAgentInfoDAL agentInfoDal;
    private readonly PartialDiscoveryResultsContainer partialResultsContainer;
    private readonly TimeSpan agentPartialResultsTimeout = BusinessLayerSettings.Instance.AgentDiscoveryJobTimeout.Add(TimeSpan.FromMinutes(1.0));
    private static Dictionary<int, OrionDiscoveryJobProgressInfo> profileProgress = new Dictionary<int, OrionDiscoveryJobProgressInfo>();
    private readonly DiscoveryLogic discoveryLogic = new DiscoveryLogic();
    private readonly CultureInfo PrimaryLocale;
    internal Func<IJobSchedulerHelper> JobSchedulerHelperFactory;

    private static CultureInfo GetPrimaryLocale()
    {
      try
      {
        string primaryLocale = LocaleConfiguration.PrimaryLocale;
        JobSchedulerEventServicev2.log.Verbose((object) ("Primary locale set to " + primaryLocale));
        return new CultureInfo(primaryLocale);
      }
      catch (Exception ex)
      {
        JobSchedulerEventServicev2.log.Error((object) "Error while getting primary locale CultureInfo", ex);
      }
      return (CultureInfo) null;
    }

    public OrionDiscoveryJobSchedulerEventsService(
      CoreBusinessLayerPlugin parent,
      IOneTimeAgentDiscoveryJobFactory oneTimeAgentJobFactory)
      : this(parent, oneTimeAgentJobFactory, (IAgentInfoDAL) new AgentInfoDAL())
    {
    }

    public OrionDiscoveryJobSchedulerEventsService(
      CoreBusinessLayerPlugin parent,
      IOneTimeAgentDiscoveryJobFactory oneTimeAgentJobFactory,
      IAgentInfoDAL agentInfoDal)
      : base((IServiceStateProvider) parent)
    {
      this.oneTimeAgentJobFactory = oneTimeAgentJobFactory;
      this.agentInfoDal = agentInfoDal;
      this.resultsManager.HandleResultsOfCancelledJobs = true;
      this.PrimaryLocale = OrionDiscoveryJobSchedulerEventsService.GetPrimaryLocale();
      this.partialResultsContainer = new PartialDiscoveryResultsContainer();
      this.partialResultsContainer.DiscoveryResultsComplete += new EventHandler<DiscoveryResultsCompletedEventArgs>(this.partialResultsContainer_DiscoveryResultsComplete);
      this.partialResultsContainer.ClearStore();
      this.JobSchedulerHelperFactory = (Func<IJobSchedulerHelper>) (() => JobScheduler.GetLocalInstance());
    }

    private void partialResultsContainer_DiscoveryResultsComplete(
      object sender,
      DiscoveryResultsCompletedEventArgs e)
    {
      this.ProcessMergedPartialResults(e.CompleteResult, e.OrderedPlugins, e.ScheduledJobId, e.JobState, e.ProfileId);
    }

    protected override void ProcessJobProgress(JobProgress jobProgress)
    {
      Thread.CurrentThread.CurrentUICulture = this.PrimaryLocale ?? Thread.CurrentThread.CurrentUICulture;
      if (!string.IsNullOrEmpty(jobProgress.Progress))
      {
        try
        {
          OrionDiscoveryJobProgressInfo progress = SerializationHelper.FromXmlString<OrionDiscoveryJobProgressInfo>(jobProgress.Progress);
          progress.JobId = jobProgress.JobId;
          StringBuilder stringBuilder = new StringBuilder();
          foreach (KeyValuePair<string, int> discoveredNetObject in progress.DiscoveredNetObjects)
            stringBuilder.AppendFormat(" {0} {1};", (object) discoveredNetObject.Value, (object) discoveredNetObject.Key);
          if (progress.ProfileID.HasValue)
          {
            OrionDiscoveryJobProgressInfo discoveryJobProgressInfo = OrionDiscoveryJobSchedulerEventsService.UpdateProgress(progress);
            JobSchedulerEventServicev2.log.DebugFormat("Got Discovery progress for profile {0} Status: {1} Discovered: {2} ", (object) progress.ProfileID, (object) progress.Status.Status, (object) stringBuilder);
            if (discoveryJobProgressInfo != null)
              return;
            JobSchedulerEventServicev2.log.DebugFormat("First progress of discovery profile {0}", (object) progress.ProfileID.Value);
            DiscoveryProfileEntry profileById = DiscoveryProfileEntry.GetProfileByID(progress.ProfileID.Value);
            if (profileById == null)
              return;
            profileById.Status = new DiscoveryComplexStatus((DiscoveryStatus) 1, string.Empty);
            profileById.LastRun = DateTime.Now.ToUniversalTime();
            profileById.Update();
          }
          else
          {
            JobSchedulerEventServicev2.log.DebugFormat("Recieved progress for one shot discovery job [{0}] Discovered: {1} ", (object) jobProgress.JobId, (object) stringBuilder);
            DiscoveryResultItem discoveryResultItem;
            if (DiscoveryResultCache.Instance.TryGetResultItem(jobProgress.JobId, ref discoveryResultItem))
              discoveryResultItem.Progress = progress;
            else
              JobSchedulerEventServicev2.log.ErrorFormat("Unable to get result item {0}", (object) jobProgress.JobId);
          }
        }
        catch (Exception ex)
        {
          JobSchedulerEventServicev2.log.Error((object) "Exception occured when parsing job progress info.", ex);
        }
      }
      else
        JobSchedulerEventServicev2.log.Error((object) "Job progress not found");
    }

    protected override void ProcessJobFailure(FinishedJobInfo jobResult)
    {
      Thread.CurrentThread.CurrentUICulture = this.PrimaryLocale ?? Thread.CurrentThread.CurrentUICulture;
      if (this.partialResultsContainer.IsResultExpected(jobResult.ScheduledJobId))
      {
        JobSchedulerEventServicev2.log.WarnFormat("Partial agent discovery job {0} failed with error '{1}'. It will be removed from discovery results.", (object) jobResult.ScheduledJobId, (object) jobResult.Result.Error);
        this.partialResultsContainer.RemoveExpectedPartialResult(jobResult.ScheduledJobId);
      }
      else
      {
        int result;
        if (!int.TryParse(jobResult.State, out result))
          return;
        string str = string.Format("A Network Discovery job has failed to complete.\r\nState: {0}\r\nProfile id: {1}.\r\nThe Job Scheduler is reporting the following error:\r\n{2}", (object) jobResult.Result.State, (object) jobResult.State, (object) jobResult.Result.Error);
        JobSchedulerEventServicev2.log.Error((object) str);
        OrionDiscoveryJobSchedulerEventsService.RemoveProgressInfo(result);
        try
        {
          DiscoveryProfileEntry profile = this.GetProfile(result, jobResult.ScheduledJobId);
          if (profile == null)
            return;
          if (!profile.IsScheduled)
            profile.JobID = Guid.Empty;
          profile.RuntimeInSeconds = 0;
          profile.Status = new DiscoveryComplexStatus((DiscoveryStatus) 3, string.Format(Resources.LIBCODE_AK0_8, (object) jobResult.Result.State, (object) jobResult.State, (object) jobResult.Result.Error));
          profile.Update();
          if (!profile.IsHidden)
            return;
          this.discoveryLogic.DeleteOrionDiscoveryProfile(profile.ProfileID);
        }
        catch (Exception ex)
        {
          JobSchedulerEventServicev2.log.Error((object) string.Format("Unable to update profile {0}", (object) result), ex);
        }
      }
    }

    protected override void ProcessJobResult(FinishedJobInfo jobResult)
    {
      Thread.CurrentThread.CurrentUICulture = this.PrimaryLocale ?? Thread.CurrentThread.CurrentUICulture;
      JobSchedulerEventServicev2.log.DebugFormat("Recieved discovery results", Array.Empty<object>());
      if (jobResult.Result.State == 5)
      {
        JobSchedulerEventServicev2.log.Error((object) "Job failed");
      }
      else
      {
        if (jobResult.Result.State != 6 && jobResult.Result.State != 4)
          return;
        int? nullable = new int?();
        using (IJobSchedulerHelper scheduler = this.JobSchedulerHelperFactory())
        {
          try
          {
            Stream resultStream = this.GetResultStream(jobResult, scheduler);
            List<DiscoveryPluginInfo> discoveryPluginInfos = DiscoveryPluginFactory.GetDiscoveryPluginInfos();
            List<IDiscoveryPlugin> idiscoveryPluginList = new List<IDiscoveryPlugin>((IEnumerable<IDiscoveryPlugin>) new DiscoveryPluginFactory().GetPlugins((IList<DiscoveryPluginInfo>) discoveryPluginInfos));
            List<Type> knownTypes = new List<Type>();
            idiscoveryPluginList.ForEach((Action<IDiscoveryPlugin>) (plugin => plugin.GetKnownTypes().ForEach((Action<Type>) (t => knownTypes.Add(t)))));
            SortedDictionary<int, List<IDiscoveryPlugin>> orderedPlugins = DiscoveryPluginHelper.GetOrderedPlugins((IList<IDiscoveryPlugin>) idiscoveryPluginList, (IList<DiscoveryPluginInfo>) discoveryPluginInfos);
            List<IDiscoveryPlugin> orderedPluginList = DiscoveryPluginHelper.GetOrderedPluginList(orderedPlugins);
            if (resultStream != null && resultStream.Length > 0L)
            {
              int num = resultStream.ReadByte();
              resultStream.Position = 0L;
              OrionDiscoveryJobResult result = num == 123 || num == 91 ? SerializationHelper.Deserialize<OrionDiscoveryJobResult>(resultStream) : SerializationHelper.FromXmlStream<OrionDiscoveryJobResult>(resultStream, (IEnumerable<Type>) knownTypes);
              if (result.ProfileId.HasValue)
                nullable = new int?(result.ProfileId.Value);
              if (OrionDiscoveryJobSchedulerEventsService.resultLog.IsDebugEnabled)
              {
                if (resultStream.CanSeek)
                  resultStream.Seek(0L, SeekOrigin.Begin);
                OrionDiscoveryJobSchedulerEventsService.resultLog.DebugFormat("Discovery Job {0} Result for ProfileID = {1}:", (object) jobResult.ScheduledJobId, (object) (nullable ?? -1));
                OrionDiscoveryJobSchedulerEventsService.resultLog.Debug((object) new StreamReader(resultStream, Encoding.UTF8).ReadToEnd());
              }
              if (result.DiscoverAgentNodes)
                this.PersistResultsAndDiscoverAgentNodes(result, orderedPlugins, jobResult.ScheduledJobId, jobResult.Result.State, result.AgentsFilterQuery);
              else
                this.ProcessDiscoveryJobResult(result, orderedPlugins, jobResult.ScheduledJobId, jobResult.Result.State);
            }
            else
            {
              JobSchedulerEventServicev2.log.Error((object) "Job result is empty, job was killed before it was able to report results.");
              this.UpdateTimeoutedProfile(jobResult.ScheduledJobId, orderedPluginList);
            }
            if (nullable.HasValue)
              JobSchedulerEventServicev2.log.DebugFormat("Processing of discovery results for profile {0} completed", (object) nullable);
            else
              JobSchedulerEventServicev2.log.DebugFormat("Processing of discovery results for one time job {0} completed", (object) jobResult.ScheduledJobId);
          }
          catch (Exception ex1)
          {
            JobSchedulerEventServicev2.log.Error((object) "Exception occured when parsing job result.", ex1);
            try
            {
              if (!nullable.HasValue)
                return;
              JobSchedulerEventServicev2.log.DebugFormat("Updating discovery prfile with ID {0}", (object) nullable);
              DiscoveryProfileEntry profile = this.GetProfile(nullable.Value, jobResult.ScheduledJobId);
              if (profile == null)
                return;
              profile.Status = new DiscoveryComplexStatus((DiscoveryStatus) 3, "Parsing of discovery result failed.");
              profile.Update();
            }
            catch (Exception ex2)
            {
              JobSchedulerEventServicev2.log.Error((object) string.Format("Exception updating discovery profile {0}", (object) nullable), ex2);
            }
          }
          finally
          {
            ((IJobScheduler) scheduler).DeleteJobResult(jobResult.Result.JobId);
          }
        }
      }
    }

    private Stream GetResultStream(FinishedJobInfo jobResult, IJobSchedulerHelper scheduler)
    {
      Stream destination = (Stream) null;
      if (jobResult.Result.IsResultStreamed)
      {
        using (Stream jobResultStream = ((IJobScheduler) scheduler).GetJobResultStream(jobResult.Result.JobId, "SolarWinds.Orion.Discovery.Job.Results"))
        {
          destination = (Stream) new DynamicStream();
          jobResultStream.CopyTo(destination);
          destination.Position = 0L;
        }
      }
      else if (jobResult.Result.Output != null)
        destination = (Stream) new MemoryStream(jobResult.Result.Output);
      return destination;
    }

    private void PersistResultsAndDiscoverAgentNodes(
      OrionDiscoveryJobResult result,
      SortedDictionary<int, List<IDiscoveryPlugin>> orderedPlugins,
      Guid scheduledJobId,
      JobState state,
      string agentsFilterQuery)
    {
      JobSchedulerEventServicev2.log.DebugFormat("Received discovery job result {0} that requests agent nodes discovery. Persisting partial result and scheduling agent discovery jobs.", (object) scheduledJobId);
      this.partialResultsContainer.CreatePartialResult(scheduledJobId, result, orderedPlugins, state);
      List<AgentInfo> list = this.agentInfoDal.GetAgentsByNodesFilter(((DiscoveryResultBase) result).EngineId, result.AgentsFilterQuery).ToList<AgentInfo>();
      if (result.AgentsAddresses.Count > 0)
        list = list.Where<AgentInfo>((Func<AgentInfo, bool>) (x => result.AgentsAddresses.Contains<string>(x.AgentGuid.ToString(), (IEqualityComparer<string>) StringComparer.InvariantCultureIgnoreCase))).ToList<AgentInfo>();
      List<Task> taskList = new List<Task>(list.Count);
      foreach (AgentInfo agentInfo in list)
        taskList.Add(Task.Factory.StartNew((Action<object>) (data => this.ScheduleAgentDiscoveryJob((OrionDiscoveryJobSchedulerEventsService.AgentDiscoveryJobSchedulingData) data)), (object) new OrionDiscoveryJobSchedulerEventsService.AgentDiscoveryJobSchedulingData(scheduledJobId, ((DiscoveryResultBase) result).EngineId, result.ProfileId, agentInfo)));
      Task.WaitAll(taskList.ToArray());
      this.partialResultsContainer.AllExpectedResultsRegistered(scheduledJobId);
    }

    private void ScheduleAgentDiscoveryJob(
      OrionDiscoveryJobSchedulerEventsService.AgentDiscoveryJobSchedulingData data)
    {
      try
      {
        if (data == null)
          throw new ArgumentNullException(nameof (data));
        int? nodeId1 = data.AgentInfo.NodeID;
        if (!nodeId1.HasValue)
          throw new ArgumentException("AgentInfo does not contain valid NodeID. Discovery job will not be scheduled.");
        IOneTimeAgentDiscoveryJobFactory timeAgentJobFactory = this.oneTimeAgentJobFactory;
        nodeId1 = data.AgentInfo.NodeID;
        int nodeId2 = nodeId1.Value;
        int engineId = data.EngineId;
        int? profileId = data.ProfileId;
        List<Credential> credentials = new List<Credential>();
        Guid agentDiscoveryJob = timeAgentJobFactory.CreateOneTimeAgentDiscoveryJob(nodeId2, engineId, profileId, credentials);
        this.partialResultsContainer.ExpectPartialResult(data.MainJobId, agentDiscoveryJob, this.agentPartialResultsTimeout);
      }
      catch (Exception ex)
      {
        JobSchedulerEventServicev2.log.WarnFormat("Can't create one-time discovery job for agent {0}. Agent is probably not accessible. {1}", data != null ? (object) data.AgentInfo.AgentGuid.ToString() : (object) "unknown", (object) ex);
      }
    }

    private void ProcessMergedPartialResults(
      OrionDiscoveryJobResult result,
      SortedDictionary<int, List<IDiscoveryPlugin>> orderedPlugins,
      Guid scheduledJobId,
      JobState jobState,
      int? profileId)
    {
      JobSchedulerEventServicev2.log.DebugFormat("Received all partial results for discovery job {0}. Triggering result processing.", (object) scheduledJobId);
      try
      {
        this.ProcessDiscoveryJobResult(result, orderedPlugins, scheduledJobId, jobState);
      }
      catch (Exception ex1)
      {
        JobSchedulerEventServicev2.log.Error((object) "Exception occured when parsing job result.", ex1);
        try
        {
          if (!profileId.HasValue)
            return;
          JobSchedulerEventServicev2.log.DebugFormat("Updating discovery profile with ID {0}", (object) profileId);
          DiscoveryProfileEntry profile = this.GetProfile(profileId.Value, scheduledJobId);
          if (profile == null)
            return;
          profile.Status = new DiscoveryComplexStatus((DiscoveryStatus) 3, "Parsing of discovery result failed.");
          profile.Update();
        }
        catch (Exception ex2)
        {
          JobSchedulerEventServicev2.log.Error((object) string.Format("Exception updating discovery profile {0}", (object) profileId), ex2);
        }
      }
    }

    private bool ResultForPluginIsContained(IDiscoveryPlugin plugin, OrionDiscoveryJobResult result)
    {
      try
      {
        string pluginTypeName = plugin.GetType().FullName;
        return ((IEnumerable<DiscoveryPluginResultBase>) ((DiscoveryResultBase) result).PluginResults).Any<DiscoveryPluginResultBase>((Func<DiscoveryPluginResultBase, bool>) (pluginRes => pluginTypeName.Equals(pluginRes.PluginTypeName, StringComparison.OrdinalIgnoreCase)));
      }
      catch (Exception ex)
      {
        JobSchedulerEventServicev2.log.Error((object) ex);
        return false;
      }
    }

    private void ProcessPluginsWithInterface<T>(
      string actionName,
      OrionDiscoveryJobResult result,
      SortedDictionary<int, List<IDiscoveryPlugin>> orderedPlugins,
      Action<T> processAction)
    {
      if (result == null || ((DiscoveryResultBase) result).PluginResults == null)
      {
        JobSchedulerEventServicev2.log.Error((object) "Empty discovery result received. Nothing to process.");
      }
      else
      {
        ((DiscoveryResultBase) result).PluginResults.RemoveAll((Predicate<DiscoveryPluginResultBase>) (pluginResult =>
        {
          try
          {
            pluginResult.GetDiscoveredObjects();
            return false;
          }
          catch (Exception ex)
          {
            JobSchedulerEventServicev2.log.ErrorFormat("Failed to get discovered objects from plugin result {0} ({1}), result will be discarded: {2}", (object) pluginResult.GetType().Name, (object) pluginResult.PluginTypeName, (object) ex);
            return true;
          }
        }));
        JobSchedulerEventServicev2.log.DebugFormat("Result processing [{0}] - Start", (object) actionName);
        foreach (int key in orderedPlugins.Keys)
        {
          JobSchedulerEventServicev2.log.DebugFormat("Processing level {0} plugins", (object) key);
          foreach (IDiscoveryPlugin plugin in orderedPlugins[key])
          {
            string str = ((IEnumerable<string>) plugin.GetType().AssemblyQualifiedName.Split(',')).First<string>();
            if (plugin is T)
            {
              JobSchedulerEventServicev2.log.DebugFormat("Plugin {0} is of type {1}", (object) plugin, (object) typeof (T));
              if (this.ResultForPluginIsContained(plugin, result))
              {
                try
                {
                  JobSchedulerEventServicev2.log.DebugFormat("Processing {0}", (object) str);
                  processAction((T) plugin);
                }
                catch (Exception ex)
                {
                  JobSchedulerEventServicev2.log.Error((object) string.Format("Processing of discovery result for profile {0} failed for plugin {1}", (object) result.ProfileId, (object) plugin.GetType()), ex);
                }
              }
              else
                JobSchedulerEventServicev2.log.WarnFormat("Result for plugin {0} doesnt exist.", (object) str);
            }
            else
              JobSchedulerEventServicev2.log.DebugFormat("Plugin {0} is not of type {1}", (object) plugin, (object) typeof (T));
          }
        }
        JobSchedulerEventServicev2.log.DebugFormat("Result processing [{0}] - End", (object) actionName);
      }
    }

    internal void ProcessDiscoveryJobResult(
      OrionDiscoveryJobResult result,
      SortedDictionary<int, List<IDiscoveryPlugin>> orderedPlugins,
      Guid jobId,
      JobState jobState)
    {
      if (result == null)
        throw new ArgumentNullException(nameof (result));
      JobSchedulerEventServicev2.log.DebugFormat("Processing discovery result for profile {0}", (object) result.ProfileId);
      if (this.partialResultsContainer.IsResultExpected(jobId))
      {
        this.partialResultsContainer.AddExpectedPartialResult(jobId, result);
      }
      else
      {
        int? profileId = result.ProfileId;
        if (profileId.HasValue && result.IsFromAgent)
        {
          JobSchedulerEventServicev2.log.DebugFormat("Received job result from Agent discovery job {0} that is no longer expected. Discarding.", (object) jobId);
        }
        else
        {
          this.ProcessPluginsWithInterface<IBussinessLayerPostProcessing>("ProcessDiscoveryResult", result, orderedPlugins, (Action<IBussinessLayerPostProcessing>) (p => p.ProcessDiscoveryResult((DiscoveryResultBase) result)));
          profileId = result.ProfileId;
          if (!profileId.HasValue)
            this.ImportOneShotDiscovery(result, orderedPlugins, jobId, jobState);
          else
            this.ImportProfileResults(result, orderedPlugins, jobId, jobState);
        }
      }
    }

    private void ImportOneShotDiscovery(
      OrionDiscoveryJobResult result,
      SortedDictionary<int, List<IDiscoveryPlugin>> orderedPlugins,
      Guid jobId,
      JobState jobState)
    {
      DiscoveryResultItem resultItem;
      if (!DiscoveryResultCache.Instance.TryGetResultItem(jobId, ref resultItem))
      {
        JobSchedulerEventServicev2.log.ErrorFormat("Unable to find resultItem for job {0} in cache", (object) jobId);
      }
      else
      {
        List<IDiscoveredObjectGroup> groups = new List<IDiscoveredObjectGroup>();
        List<ISelectionType> selectionTypes = new List<ISelectionType>();
        this.ProcessPluginsWithInterface<IOneTimeJobSupport>("GetDiscoveredObjectGroups, GetSelectionTypes", result, orderedPlugins, (Action<IOneTimeJobSupport>) (p =>
        {
          groups.AddRange(p.GetDiscoveredObjectGroups());
          selectionTypes.AddRange(p.GetSelectionTypes());
        }));
        TechnologyManager instance = TechnologyManager.Instance;
        groups.AddRange(DiscoveryFilterResultByTechnology.GetDiscoveryGroups(instance));
        DiscoveredObjectTree resultTree = new DiscoveredObjectTree((DiscoveryResultBase) result, (IEnumerable<IDiscoveredObjectGroup>) groups, (IEnumerable<ISelectionType>) selectionTypes);
        foreach (DiscoveredVolume discoveredVolume in (IEnumerable<DiscoveredVolume>) resultTree.GetAllTreeObjectsOfType<DiscoveredVolume>())
        {
          if (discoveredVolume.VolumeType == 5 || discoveredVolume.VolumeType == 7 || discoveredVolume.VolumeType == 6 || discoveredVolume.VolumeType == null)
            ((DiscoveredObjectBase) discoveredVolume).IsSelected = false;
        }
        if (!resultItem.nodeId.HasValue)
        {
          this.ProcessPluginsWithInterface<IDefaultTreeState>("SetTreeDefault", result, orderedPlugins, (Action<IDefaultTreeState>) (p => p.SetTreeDefaultState(resultTree)));
          DiscoveryFilterResultByTechnology.FilterByPriority((DiscoveryResultBase) result, instance);
        }
        else
        {
          this.ProcessPluginsWithInterface<IOneTimeJobSupport>("GetDiscoveredResourcesManagedStatus", result, orderedPlugins, (Action<IOneTimeJobSupport>) (p => p.GetDiscoveredResourcesManagedStatus(resultTree, resultItem.nodeId.Value)));
          DiscoveryFilterResultByTechnology.FilterMandatoryByPriority((DiscoveryResultBase) result, instance);
        }
        resultItem.ResultTree = resultTree;
        this.UpdateOneTimeJobResultProgress(resultItem);
      }
    }

    private void UpdateOneTimeJobResultProgress(DiscoveryResultItem item)
    {
      if (item.ResultTree == null)
        return;
      if (item.Progress == null)
        item.Progress = new OrionDiscoveryJobProgressInfo()
        {
          JobId = item.JobId
        };
      item.Progress.Status = new DiscoveryComplexStatus((DiscoveryStatus) 8, "Ready for import", (DiscoveryPhase) 1);
    }

    private void ImportProfileResults(
      OrionDiscoveryJobResult result,
      SortedDictionary<int, List<IDiscoveryPlugin>> orderedPlugins,
      Guid jobId,
      JobState jobState)
    {
      this.ProcessPluginsWithInterface<IDiscoveryPlugin>("StoreDiscoveryResult", result, orderedPlugins, (Action<IDiscoveryPlugin>) (p => p.StoreDiscoveryResult((DiscoveryResultBase) result)));
      JobSchedulerEventServicev2.log.DebugFormat("Updating information about ignored items stored in profile {0}", (object) result.ProfileId.Value);
      DiscoveryIgnoredDAL.UpdateIgnoreInformationForProfile(result.ProfileId.Value);
      bool flag = true;
      DiscoveryLogs discoveryLog = new DiscoveryLogs();
      DiscoveryProfileEntry profile = (DiscoveryProfileEntry) null;
      try
      {
        JobSchedulerEventServicev2.log.DebugFormat("Updating discovery profile with ID {0}", (object) result.ProfileId.Value);
        profile = this.GetProfile(result.ProfileId.Value, jobId);
        if (profile == null)
          return;
        if (!profile.IsScheduled)
          profile.JobID = Guid.Empty;
        DateTime utcNow = DateTime.UtcNow;
        discoveryLog.FinishedTimeStamp = utcNow.AddTicks(-(utcNow.Ticks % 10000000L));
        discoveryLog.ProfileID = profile.ProfileID;
        discoveryLog.AutoImport = profile.IsAutoImport;
        discoveryLog.Result = 0;
        discoveryLog.ResultDescription = Resources2.DiscoveryLogResult_DiscoveryFinished;
        discoveryLog.BatchID = new Guid?(Guid.NewGuid());
        profile.RuntimeInSeconds = (int) (DateTime.Now - profile.LastRun.ToLocalTime()).TotalSeconds;
        if (jobState == 4)
        {
          discoveryLog.Result = 1;
          discoveryLog.ResultDescription = Resources2.DiscoveryLogResult_DiscoveryFailed;
          discoveryLog.ErrorMessage = Resources2.DiscoveryLogError_Cancelled;
          this.UpdateCanceledProfileStatus(profile);
          profile.Update();
          OrionDiscoveryJobSchedulerEventsService.RemoveProgressInfo(profile.ProfileID);
        }
        else if (profile.IsAutoImport)
        {
          flag = false;
          profile.Status = new DiscoveryComplexStatus(profile.Status.Status, profile.Status.Description, (DiscoveryPhase) 4);
          profile.Update();
          JobSchedulerEventServicev2.log.InfoFormat("Starting AutoImport of Profile:{0}", (object) profile.ProfileID);
          bool isHidden = profile.IsHidden;
          this.discoveryLogic.ImportDiscoveryResultForProfile(profile.ProfileID, isHidden, (DiscoveryImportManager.CallbackDiscoveryImportFinished) ((_result, importJobID, StartImportStatus) =>
          {
            DiscoveryAutoImportNotificationItemDAL.Show(_result, StartImportStatus);
            this.ImportResultFinished(_result, importJobID, StartImportStatus);
            DiscoveryImportManager.FillDiscoveryLogEntity(discoveryLog, _result, StartImportStatus);
            JobSchedulerEventServicev2.log.InfoFormat("AutoImport of Profile:{0} finished with result:{1}", (object) discoveryLog.ProfileID, (object) discoveryLog.Result.ToString());
            try
            {
              using (CoreSwisContext systemContext = SwisContextFactory.CreateSystemContext())
                discoveryLog.Create((SwisContext) systemContext);
              JobSchedulerEventServicev2.log.InfoFormat("DiscoveryLog created for ProfileID:{0}", (object) discoveryLog.ProfileID);
            }
            catch (Exception ex)
            {
              JobSchedulerEventServicev2.log.Error((object) "Unable to create discovery import log", ex);
            }
          }), true, new Guid?(jobId));
        }
        else
        {
          profile.Status = !profile.IsScheduled ? new DiscoveryComplexStatus((DiscoveryStatus) 2, string.Empty) : new DiscoveryComplexStatus((DiscoveryStatus) 5, string.Empty);
          OrionDiscoveryJobSchedulerEventsService.GenerateDiscoveryFinishedEvent(profile.EngineID, profile.Name);
          OrionDiscoveryJobSchedulerEventsService.RemoveProgressInfo(profile.ProfileID);
          profile.Update();
        }
      }
      catch (Exception ex)
      {
        JobSchedulerEventServicev2.log.Error((object) string.Format("Unable to update profile {0}", (object) result.ProfileId.Value), ex);
        if (profile != null)
          OrionDiscoveryJobSchedulerEventsService.GenerateDiscoveryFailedEvent(profile.EngineID, profile.Name);
        if (!flag)
          return;
        discoveryLog.Result = 1;
        discoveryLog.ResultDescription = Resources2.DiscoveryLogResult_DiscoveryFailed;
        discoveryLog.ErrorMessage = Resources2.DiscoveryLogError_SeeLog;
      }
      finally
      {
        if (profile != null)
        {
          if (profile.Status.Status == 5)
            DiscoveryNetObjectStatusManager.Instance.RequestUpdateForProfileAsync(profile.ProfileID, new Action(OrionDiscoveryJobSchedulerEventsService.FireScheduledDiscoveryNotification), TimeSpan.Zero);
          else
            DiscoveryNetObjectStatusManager.Instance.RequestUpdateForProfileAsync(profile.ProfileID, (Action) null, TimeSpan.Zero);
        }
        if (flag)
        {
          using (CoreSwisContext systemContext = SwisContextFactory.CreateSystemContext())
            discoveryLog.Create((SwisContext) systemContext);
        }
      }
    }

    private void ImportResultFinished(
      DiscoveryResultBase result,
      Guid importJobID,
      StartImportStatus status)
    {
      DiscoveryProfileEntry discoveryProfileEntry = result != null ? DiscoveryProfileEntry.GetProfileByID(result.ProfileID) : throw new ArgumentNullException(nameof (result));
      if (discoveryProfileEntry == null)
        return;
      if (status == 4)
      {
        discoveryProfileEntry.Status = new DiscoveryComplexStatus((DiscoveryStatus) 2, string.Empty);
        OrionDiscoveryJobSchedulerEventsService.GenerateImportFailedEvent(result.EngineId, discoveryProfileEntry.Name, status);
      }
      else if (status == 5)
      {
        discoveryProfileEntry.Status = new DiscoveryComplexStatus((DiscoveryStatus) 8, string.Empty);
        OrionDiscoveryJobSchedulerEventsService.GenerateImportFailedEvent(result.EngineId, discoveryProfileEntry.Name, status);
      }
      else
      {
        discoveryProfileEntry.Status = !discoveryProfileEntry.IsScheduled ? new DiscoveryComplexStatus((DiscoveryStatus) 2, string.Empty) : new DiscoveryComplexStatus((DiscoveryStatus) 5, string.Empty);
        OrionDiscoveryJobSchedulerEventsService.GenerateImportFinishedEvent(result.EngineId, discoveryProfileEntry.Name);
      }
      discoveryProfileEntry.Update();
      OrionDiscoveryJobSchedulerEventsService.RemoveProgressInfo(discoveryProfileEntry.ProfileID);
    }

    private static void GenerateImportFinishedEvent(int engineId, string profileName)
    {
      EventsDAL.InsertEvent(0, 0, string.Empty, 70, string.Format(Resources.Discovery_Succeeded_Text_Run_Import, (object) profileName), engineId);
    }

    private static void GenerateImportFailedEvent(
      int engineId,
      string profileName,
      StartImportStatus importStatus)
    {
      if (importStatus == 5)
        EventsDAL.InsertEvent(0, 0, string.Empty, 71, string.Format(Resources.Discovery_Failed_Text_Import_License, (object) profileName), engineId);
      else
        EventsDAL.InsertEvent(0, 0, string.Empty, 71, string.Format(Resources.Discovery_Failed_Text_Import, (object) profileName), engineId);
    }

    private static void GenerateDiscoveryFinishedEvent(int engineId, string profileName)
    {
      EventsDAL.InsertEvent(0, 0, string.Empty, 70, string.Format(Resources.Discovery_Succeeded_Text_Run, (object) profileName), engineId);
    }

    private static void GenerateDiscoveryFailedEvent(int engineId, string profileName)
    {
      EventsDAL.InsertEvent(0, 0, string.Empty, 71, string.Format(Resources.Discovery_Failed_Text_Run, (object) profileName), engineId);
    }

    private static void FireScheduledDiscoveryNotification()
    {
      int countOfNodes1 = DiscoveryNodeEntry.GetCountOfNodes((DiscoveryNodeStatus) 56);
      JobSchedulerEventServicev2.log.DebugFormat("SD: New nodes found: {0}", (object) countOfNodes1);
      int countOfNodes2 = DiscoveryNodeEntry.GetCountOfNodes((DiscoveryNodeStatus) 42);
      JobSchedulerEventServicev2.log.DebugFormat("SD: Changed nodes found: {0}", (object) countOfNodes2);
      string url = string.Format("/Orion/Discovery/Results/ScheduledDiscoveryResults.aspx?Status={0}", (object) 58);
      ScheduledDiscoveryNotificationItemDAL.Create(OrionDiscoveryJobSchedulerEventsService.ComposeNotificationMessage(countOfNodes1, countOfNodes2), url);
    }

    private static string ComposeNotificationMessage(int newNodes, int changedNodes)
    {
      StringBuilder stringBuilder = new StringBuilder(Resources.LIBCODE_PCC_18);
      stringBuilder.Append(" ");
      if (newNodes == 1)
        stringBuilder.Append(Resources.LIBCODE_PCC_19);
      else if (newNodes >= 0)
        stringBuilder.AppendFormat(Resources.LIBCODE_PCC_20, (object) newNodes);
      if (changedNodes > 0)
      {
        if (newNodes >= 0)
          stringBuilder.Append(" ");
        if (changedNodes == 1)
          stringBuilder.Append(Resources.LIBCODE_PCC_21);
        else
          stringBuilder.AppendFormat(Resources.LIBCODE_PCC_22, (object) changedNodes);
      }
      return stringBuilder.ToString();
    }

    public void UpdateTimeoutedProfile(Guid jobId, List<IDiscoveryPlugin> orderedPlugins)
    {
      DiscoveryProfileEntry profile = DiscoveryProfileEntry.GetAllProfiles().Where<DiscoveryProfileEntry>((Func<DiscoveryProfileEntry, bool>) (p => p.JobID == jobId)).FirstOrDefault<DiscoveryProfileEntry>();
      if (profile == null)
      {
        JobSchedulerEventServicev2.log.ErrorFormat("Unable to find profile with job id {0}", (object) jobId);
      }
      else
      {
        try
        {
          orderedPlugins.ForEach((Action<IDiscoveryPlugin>) (p =>
          {
            if (!(p is IResultManagement))
              return;
            ((IResultManagement) p).DeleteResultsForProfile(profile.ProfileID);
          }));
          if (!profile.IsScheduled)
            profile.JobID = Guid.Empty;
          this.UpdateCanceledProfileStatus(profile);
          profile.Update();
          OrionDiscoveryJobSchedulerEventsService.RemoveProgressInfo(profile.ProfileID);
        }
        catch (Exception ex)
        {
          JobSchedulerEventServicev2.log.Error((object) string.Format("Unhandled exception occured when updating profile {0}", (object) profile.ProfileID), ex);
        }
      }
    }

    public static OrionDiscoveryJobProgressInfo GetProgressInfo(int profileId)
    {
      lock (OrionDiscoveryJobSchedulerEventsService.profileProgress)
      {
        if (!OrionDiscoveryJobSchedulerEventsService.profileProgress.ContainsKey(profileId))
          return (OrionDiscoveryJobProgressInfo) null;
        OrionDiscoveryJobProgressInfo progressInfo = OrionDiscoveryJobSchedulerEventsService.profileProgress[profileId];
        progressInfo.ImportProgress = DiscoveryImportManager.GetImportProgress(progressInfo.JobId);
        if (progressInfo.ImportProgress != null)
          progressInfo.Status = new DiscoveryComplexStatus((DiscoveryStatus) 1, string.Empty, (DiscoveryPhase) 4);
        return progressInfo;
      }
    }

    public static OrionDiscoveryJobProgressInfo UpdateProgress(
      OrionDiscoveryJobProgressInfo progress)
    {
      OrionDiscoveryJobProgressInfo discoveryJobProgressInfo = (OrionDiscoveryJobProgressInfo) null;
      lock (((ICollection) OrionDiscoveryJobSchedulerEventsService.profileProgress).SyncRoot)
      {
        OrionDiscoveryJobSchedulerEventsService.profileProgress.TryGetValue(progress.ProfileID.Value, out discoveryJobProgressInfo);
        if (discoveryJobProgressInfo == null)
          OrionDiscoveryJobSchedulerEventsService.profileProgress[progress.ProfileID.Value] = progress;
        else if (!discoveryJobProgressInfo.CanceledByUser)
          discoveryJobProgressInfo.MergeWithNewProgress(progress);
      }
      return discoveryJobProgressInfo;
    }

    public static void RemoveProgressInfo(int profileID)
    {
      lock (((ICollection) OrionDiscoveryJobSchedulerEventsService.profileProgress).SyncRoot)
        OrionDiscoveryJobSchedulerEventsService.profileProgress.Remove(profileID);
    }

    private DiscoveryProfileEntry GetProfile(int profileID, Guid scheduledJobID)
    {
      JobSchedulerEventServicev2.log.DebugFormat("Loading info for profile {0}.", (object) profileID);
      DiscoveryProfileEntry profile = DiscoveryProfileEntry.GetProfileByID(profileID);
      if (profile == null)
        JobSchedulerEventServicev2.log.ErrorFormat("Profile: {0} doesn't exists. Deleting job ID: {1}", (object) profileID, (object) scheduledJobID);
      else if (profile.JobID != scheduledJobID)
      {
        JobSchedulerEventServicev2.log.ErrorFormat("Profile: {0} exists but has different JobId: {1}. Deleting job ID: {2}", (object) profileID, (object) profile.JobID, (object) scheduledJobID);
        profile = (DiscoveryProfileEntry) null;
      }
      if (profile == null)
      {
        if (!new OrionDiscoveryJobFactory().DeleteJob(scheduledJobID))
          JobSchedulerEventServicev2.log.Error((object) ("Error when deleting job: " + (object) scheduledJobID));
        JobSchedulerEventServicev2.log.ErrorFormat("Job ID: {0} for ProfileID: {1} was deleted.", (object) scheduledJobID, (object) profileID);
      }
      else
        JobSchedulerEventServicev2.log.DebugFormat("Profile info for profile {0} loaded.", (object) profileID);
      return profile;
    }

    internal static void CancelDiscoveryJob(int profileID)
    {
      DiscoveryProfileEntry profileById = DiscoveryProfileEntry.GetProfileByID(profileID);
      lock (OrionDiscoveryJobSchedulerEventsService.profileProgress)
      {
        OrionDiscoveryJobProgressInfo discoveryJobProgressInfo;
        if (!OrionDiscoveryJobSchedulerEventsService.profileProgress.TryGetValue(profileID, out discoveryJobProgressInfo))
        {
          discoveryJobProgressInfo = new OrionDiscoveryJobProgressInfo();
          discoveryJobProgressInfo.Status = profileById.Status;
          OrionDiscoveryJobSchedulerEventsService.profileProgress.Add(profileID, discoveryJobProgressInfo);
        }
        discoveryJobProgressInfo.CanceledByUser = true;
      }
    }

    private void UpdateCanceledProfileStatus(DiscoveryProfileEntry profile)
    {
      if (profile.Status.Status == 7)
        profile.Status = new DiscoveryComplexStatus((DiscoveryStatus) 6, "WEBDATA_TP0_DISCOVERY_CANCELLED_BY_USER");
      else
        profile.Status = new DiscoveryComplexStatus((DiscoveryStatus) 6, "WEBDATA_TP0_DISCOVERY_INTERRUPTED_BY_TIMEOUT");
    }

    private class AgentDiscoveryJobSchedulingData
    {
      public AgentDiscoveryJobSchedulingData(
        Guid mainJobId,
        int engineId,
        int? profileId,
        AgentInfo agentInfo)
      {
        this.MainJobId = mainJobId;
        this.EngineId = engineId;
        this.ProfileId = profileId;
        this.AgentInfo = agentInfo;
      }

      public Guid MainJobId { get; private set; }

      public int EngineId { get; private set; }

      public int? ProfileId { get; set; }

      public AgentInfo AgentInfo { get; private set; }
    }
  }
}
