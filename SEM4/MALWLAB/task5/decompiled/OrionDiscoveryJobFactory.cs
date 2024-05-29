// Decompiled with JetBrains decompiler
// Type: SolarWinds.Orion.Core.BusinessLayer.OrionDiscoveryJobFactory
// Assembly: SolarWinds.Orion.Core.BusinessLayer, Version=2019.4.5200.9083, Culture=neutral, PublicKeyToken=null
// MVID: E12E8C85-5CD9-4E06-8801-182E5104FADE
// Assembly location: E:\task5.dll

using SolarWinds.JobEngine;
using SolarWinds.Logging;
using SolarWinds.Orion.Common;
using SolarWinds.Orion.Core.Common;
using SolarWinds.Orion.Core.Common.DALs;
using SolarWinds.Orion.Core.Common.JobEngine;
using SolarWinds.Orion.Core.Common.Models;
using SolarWinds.Orion.Core.Models.Discovery;
using SolarWinds.Orion.Discovery.Contract.DiscoveryPlugin;
using SolarWinds.Orion.Discovery.Framework.Interfaces;
using SolarWinds.Orion.Discovery.Framework.Pluggability;
using SolarWinds.Orion.Discovery.Job;
using SolarWinds.Serialization.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Xml;

#nullable disable
namespace SolarWinds.Orion.Core.BusinessLayer
{
  public class OrionDiscoveryJobFactory : IJobFactory
  {
    private static readonly Log log = new Log();
    private const int DefaultJobTimeout = 60;
    private const string ListenerUri = "net.pipe://localhost/orion/core/scheduleddiscoveryjobsevents2";
    private readonly IEngineDAL engineDAL;

    public OrionDiscoveryJobFactory()
      : this((IEngineDAL) new EngineDAL())
    {
    }

    internal OrionDiscoveryJobFactory(IEngineDAL engineDal)
    {
      this.engineDAL = engineDal != null ? engineDal : throw new ArgumentNullException(nameof (engineDal));
    }

    public string GetOrionDiscoveryJobDescriptionString(
      OrionDiscoveryJobDescription discoveryJobDescription,
      List<DiscoveryPluginInfo> pluginInfos,
      bool jsonFormat = false)
    {
      if (jsonFormat)
        return SerializationHelper.ToJson((object) discoveryJobDescription);
      DiscoveryPluginInfoCollection pluginInfoCollection = new DiscoveryPluginInfoCollection()
      {
        PluginInfos = pluginInfos
      };
      List<Type> typeList = new List<Type>();
      foreach (DiscoveryPluginJobDescriptionBase pluginJobDescription in discoveryJobDescription.DiscoveryPluginJobDescriptions)
      {
        if (!typeList.Contains(pluginJobDescription.GetType()))
          typeList.Add(pluginJobDescription.GetType());
      }
      return SerializationHelper.XmlWrap(new List<string>()
      {
        SerializationHelper.ToXmlString((object) pluginInfoCollection),
        SerializationHelper.ToXmlString((object) discoveryJobDescription, (IEnumerable<Type>) typeList)
      });
    }

    public void GetOrionDiscoveryJobDescriptionXml(
      OrionDiscoveryJobDescription discoveryJobDescription,
      List<DiscoveryPluginInfo> pluginInfos,
      XmlWriter xmlWriter)
    {
      IEnumerable<Type> types = discoveryJobDescription.DiscoveryPluginJobDescriptions.Select<DiscoveryPluginJobDescriptionBase, Type>((Func<DiscoveryPluginJobDescriptionBase, Type>) (pjd => pjd.GetType())).Distinct<Type>();
      SerializationHelper.XmlWrap((IEnumerable<XmlReader>) new XmlReader[2]
      {
        SerializationHelper.ToXmlReader((object) new DiscoveryPluginInfoCollection()
        {
          PluginInfos = pluginInfos
        }),
        SerializationHelper.ToXmlReader((object) discoveryJobDescription, types)
      }, xmlWriter);
    }

    public ScheduledJob CreateDiscoveryJob(DiscoveryConfiguration configuration)
    {
      return this.CreateDiscoveryJob(configuration, (IDiscoveryPluginFactory) new DiscoveryPluginFactory());
    }

    internal static DiscoveryPollingEngineType? GetDiscoveryPollingEngineType(
      int engineId,
      IEngineDAL engineDal = null)
    {
      engineDal = (IEngineDAL) ((object) engineDal ?? (object) new EngineDAL());
      Engine engine = engineDal.GetEngine(engineId);
      if (engine.ServerType.Equals("BranchOffice"))
        engine.ServerType = "RemoteCollector";
      DiscoveryPollingEngineType result;
      if (Enum.TryParse<DiscoveryPollingEngineType>(engine.ServerType, true, out result))
        return new DiscoveryPollingEngineType?(result);
      if (OrionDiscoveryJobFactory.log.IsErrorEnabled)
        OrionDiscoveryJobFactory.log.Error((object) ("Unable to determine DiscoveryPollingEngineType value for engine server type '" + engine.ServerType + "'"));
      return new DiscoveryPollingEngineType?();
    }

    internal static bool IsDiscoveryPluginSupportedForDiscoveryPollingEngineType(
      IDiscoveryPlugin plugin,
      DiscoveryPollingEngineType discovryPollingEngineType,
      IDictionary<IDiscoveryPlugin, DiscoveryPluginInfo> pluginInfoPairs)
    {
      return pluginInfoPairs.ContainsKey(plugin) && ((IEnumerable<DiscoveryPollingEngineType>) pluginInfoPairs[plugin].SupportedPollingEngineTypes).Contains<DiscoveryPollingEngineType>(discovryPollingEngineType);
    }

    public ScheduledJob CreateDiscoveryJob(
      DiscoveryConfiguration configuration,
      IDiscoveryPluginFactory pluginFactory)
    {
      if (configuration == null)
        throw new ArgumentNullException(nameof (configuration));
      Engine engine = this.engineDAL.GetEngine(((DiscoveryConfigurationBase) configuration).EngineId);
      DiscoveryPollingEngineType? pollingEngineType = OrionDiscoveryJobFactory.GetDiscoveryPollingEngineType(configuration.EngineID, this.engineDAL);
      int result;
      if (!int.TryParse(SettingsDAL.Get("SWNetPerfMon-Settings-SNMP MaxReps"), out result))
        result = 5;
      OrionDiscoveryJobDescription discoveryJobDescription = new OrionDiscoveryJobDescription()
      {
        ProfileId = ((DiscoveryConfigurationBase) configuration).ProfileId,
        EngineId = ((DiscoveryConfigurationBase) configuration).EngineId,
        HopCount = configuration.HopCount,
        IcmpTimeout = configuration.SearchTimeout,
        SnmpConfiguration = new DiscoveryCommonSnmpConfiguration()
        {
          MaxSnmpReplies = result,
          SnmpRetries = configuration.SnmpRetries,
          SnmpTimeout = configuration.SnmpTimeout,
          SnmpPort = configuration.SnmpPort,
          PreferredSnmpVersion = configuration.PreferredSnmpVersion
        },
        DisableICMP = configuration.DisableICMP,
        PreferredPollingMethod = ((DiscoveryConfigurationBase) configuration).GetDiscoveryPluginConfiguration<CoreDiscoveryPluginConfiguration>().PreferredPollingMethod,
        VulnerabilityCheckDisabled = SettingsDAL.GetCurrentInt("SWNetPerfMon-Settings-VulnerabilityCheckDisabled", 0) == 1,
        MaxThreadsInDetectionPhase = SettingsDAL.GetCurrentInt("Discovery-MaxThreadsInDetectionPhase", 5),
        MaxThreadsInInventoryPhase = SettingsDAL.GetCurrentInt("Discovery-MaxThreadsInInventoryPhase", 5),
        PreferredDnsAddressFamily = SettingsDAL.GetCurrentInt("SWNetPerfMon-Settings-Default Preferred AddressFamily DHCP", 4),
        TagFilter = configuration.TagFilter,
        DefaultProbes = configuration.DefaultProbes
      };
      List<DiscoveryPluginInfo> discoveryPluginInfos = DiscoveryPluginFactory.GetDiscoveryPluginInfos();
      IList<IDiscoveryPlugin> plugins = pluginFactory.GetPlugins((IList<DiscoveryPluginInfo>) discoveryPluginInfos);
      List<DiscoveryPluginInfo> pluginInfos = new List<DiscoveryPluginInfo>();
      IDictionary<IDiscoveryPlugin, DiscoveryPluginInfo> pairsPluginAndInfo = DiscoveryPluginHelper.CreatePairsPluginAndInfo((IEnumerable<IDiscoveryPlugin>) plugins, (IEnumerable<DiscoveryPluginInfo>) discoveryPluginInfos);
      bool flag1 = RegistrySettings.IsFreePoller();
      foreach (IDiscoveryPlugin idiscoveryPlugin in (IEnumerable<IDiscoveryPlugin>) plugins)
      {
        if (flag1 && !(idiscoveryPlugin is ISupportFreeEngine))
          OrionDiscoveryJobFactory.log.DebugFormat("Discovery plugin {0} is not supported on FPE machine", (object) idiscoveryPlugin);
        else if (!((DiscoveryConfigurationBase) configuration).ProfileId.HasValue && !(idiscoveryPlugin is IOneTimeJobSupport))
        {
          OrionDiscoveryJobFactory.log.DebugFormat("Plugin {0} is not supporting one time job and it's description woun't be added.", (object) idiscoveryPlugin.GetType().FullName);
        }
        else
        {
          if (configuration.TagFilter != null && configuration.TagFilter.Any<string>())
          {
            if (!(idiscoveryPlugin is IDiscoveryPluginTags idiscoveryPluginTags))
            {
              OrionDiscoveryJobFactory.log.DebugFormat("Discovery job for tags requested, however plugin {0} doesn't support tags, skipping.", (object) idiscoveryPlugin);
              continue;
            }
            if (!configuration.TagFilter.Intersect<string>(idiscoveryPluginTags.Tags ?? Enumerable.Empty<string>(), (IEqualityComparer<string>) StringComparer.InvariantCultureIgnoreCase).Any<string>())
            {
              OrionDiscoveryJobFactory.log.DebugFormat("Discovery job for tags [{0}], however plugin {1} doesn't support any of the tags requested, skipping.", (object) string.Join(",", (IEnumerable<string>) configuration.TagFilter), (object) idiscoveryPlugin);
              continue;
            }
          }
          if (configuration.IsAgentJob && (!(idiscoveryPlugin is IAgentPluginJobSupport pluginJobSupport) || !((IEnumerable<string>) configuration.AgentPlugins).Contains<string>(pluginJobSupport.PluginId)))
            OrionDiscoveryJobFactory.log.DebugFormat("Plugin {0} is not contained in supported agent plugins and will not be used.", (object) idiscoveryPlugin.GetType().FullName);
          else if (pollingEngineType.HasValue && !OrionDiscoveryJobFactory.IsDiscoveryPluginSupportedForDiscoveryPollingEngineType(idiscoveryPlugin, pollingEngineType.Value, pairsPluginAndInfo))
          {
            if (OrionDiscoveryJobFactory.log.IsDebugEnabled)
              OrionDiscoveryJobFactory.log.DebugFormat(string.Format("Plugin {0} is not supported for polling engine {1}", (object) idiscoveryPlugin.GetType().FullName, (object) configuration.EngineID), Array.Empty<object>());
          }
          else
          {
            Exception exception = (Exception) null;
            DiscoveryPluginJobDescriptionBase jobDescriptionBase;
            try
            {
              jobDescriptionBase = idiscoveryPlugin.GetJobDescription((DiscoveryConfigurationBase) configuration);
            }
            catch (Exception ex)
            {
              jobDescriptionBase = (DiscoveryPluginJobDescriptionBase) null;
              exception = ex;
            }
            if (jobDescriptionBase == null)
            {
              string str = "Plugin " + idiscoveryPlugin.GetType().FullName + " was not able found valid job description.";
              if (exception != null)
                OrionDiscoveryJobFactory.log.Warn((object) str, exception);
              else
                OrionDiscoveryJobFactory.log.Warn((object) str);
            }
            else
            {
              discoveryJobDescription.DiscoveryPluginJobDescriptions.Add(jobDescriptionBase);
              DiscoveryPluginInfo discoveryPluginInfo = pairsPluginAndInfo[idiscoveryPlugin];
              pluginInfos.Add(discoveryPluginInfo);
            }
          }
        }
      }
      JobDescription jobDescription = new JobDescription()
      {
        TypeName = typeof (OrionDiscoveryJob).AssemblyQualifiedName,
        JobDetailConfiguration = this.GetOrionDiscoveryJobDescriptionString(discoveryJobDescription, pluginInfos, configuration.UseJsonFormat),
        JobNamespace = "orion",
        ResultTTL = TimeSpan.FromMinutes(10.0),
        TargetNode = new HostAddress(IPAddressHelper.ToStringIp(engine.IP), (AddressType) 4),
        LegacyEngine = engine.ServerName.ToLowerInvariant(),
        EndpointAddress = configuration.IsAgentJob ? configuration.AgentAddress : (string) null,
        SupportedRoles = (PackageType) 7
      };
      jobDescription.Timeout = OrionDiscoveryJobFactory.GetDiscoveryJobTimeout(configuration);
      ScheduledJob discoveryJob;
      if (configuration.CronSchedule != null)
      {
        bool flag2 = false;
        string str = configuration.CronSchedule.CronExpression;
        DateTime dateTime;
        if (string.IsNullOrWhiteSpace(str))
        {
          dateTime = configuration.CronSchedule.StartTime;
          DateTime localTime = dateTime.ToLocalTime();
          if (localTime < DateTime.Now)
          {
            OrionDiscoveryJobFactory.log.InfoFormat("Profile (ID={0}) with past Once(Cron) schedule. We should not create job for it.", (object) configuration.ProfileID);
            return (ScheduledJob) null;
          }
          str = string.Format("{0} {1} {2} {3} *", (object) localTime.Minute, (object) localTime.Hour, (object) localTime.Day, (object) localTime.Month);
          flag2 = true;
        }
        discoveryJob = new ScheduledJob(jobDescription, str, "net.pipe://localhost/orion/core/scheduleddiscoveryjobsevents2", configuration.ProfileID.ToString());
        discoveryJob.RunOnce = flag2;
        discoveryJob.TimeZoneInfo = TimeZoneInfo.Local;
        if (!flag2)
        {
          ScheduledJob scheduledJob1 = discoveryJob;
          dateTime = configuration.CronSchedule.StartTime;
          DateTime universalTime1 = dateTime.ToUniversalTime();
          scheduledJob1.Start = universalTime1;
          DateTime? endTime1 = configuration.CronSchedule.EndTime;
          dateTime = DateTime.MaxValue;
          if ((endTime1.HasValue ? (endTime1.HasValue ? (endTime1.GetValueOrDefault() != dateTime ? 1 : 0) : 0) : 1) != 0)
          {
            DateTime? endTime2 = configuration.CronSchedule.EndTime;
            if (endTime2.HasValue)
            {
              ScheduledJob scheduledJob2 = discoveryJob;
              endTime2 = configuration.CronSchedule.EndTime;
              dateTime = endTime2.Value;
              DateTime universalTime2 = dateTime.ToUniversalTime();
              scheduledJob2.End = universalTime2;
            }
          }
        }
      }
      else
        discoveryJob = configuration.ScheduleRunAtTime.Equals(DateTime.MinValue) ? new ScheduledJob(jobDescription, configuration.ScheduleRunFrequency, "net.pipe://localhost/orion/core/scheduleddiscoveryjobsevents2", configuration.ProfileID.ToString()) : new ScheduledJob(jobDescription, configuration.ScheduleRunAtTime, "net.pipe://localhost/orion/core/scheduleddiscoveryjobsevents2", configuration.ProfileID.ToString());
      return discoveryJob;
    }

    private static TimeSpan GetDiscoveryJobTimeout(DiscoveryConfiguration configuration)
    {
      if (configuration.IsAgentJob)
        return BusinessLayerSettings.Instance.AgentDiscoveryJobTimeout;
      return configuration.JobTimeout == TimeSpan.Zero || configuration.JobTimeout == TimeSpan.MinValue ? TimeSpan.FromMinutes(60.0) : configuration.JobTimeout;
    }

    private Guid SubmitScheduledJobToScheduler(
      Guid jobId,
      ScheduledJob job,
      bool executeImmediately,
      bool useLocal)
    {
      using (IJobSchedulerHelper ijobSchedulerHelper = useLocal ? JobScheduler.GetLocalInstance() : JobScheduler.GetMainInstance())
      {
        if (jobId == Guid.Empty)
        {
          OrionDiscoveryJobFactory.log.Debug((object) "Adding new job to Job Engine");
          return ((IJobScheduler) ijobSchedulerHelper).AddJob(job);
        }
        try
        {
          OrionDiscoveryJobFactory.log.DebugFormat("Updating job definition in Job Engine ({0})", (object) jobId);
          ((IJobScheduler) ijobSchedulerHelper).UpdateJob(jobId, job, executeImmediately);
          return jobId;
        }
        catch (FaultException<JobEngineConnectionFault> ex)
        {
          OrionDiscoveryJobFactory.log.DebugFormat("Unable to update job definition in Job Engine({0}", (object) jobId);
          throw;
        }
        catch (Exception ex)
        {
          OrionDiscoveryJobFactory.log.DebugFormat("Unable to update job definition in Job Engine({0}", (object) jobId);
        }
        OrionDiscoveryJobFactory.log.Debug((object) "Adding new job to Job Engine");
        return ((IJobScheduler) ijobSchedulerHelper).AddJob(job);
      }
    }

    public Guid SubmitScheduledJob(Guid jobId, ScheduledJob job, bool executeImmediately)
    {
      return this.SubmitScheduledJobToScheduler(jobId, job, executeImmediately, false);
    }

    public Guid SubmitScheduledJobToLocalEngine(
      Guid jobId,
      ScheduledJob job,
      bool executeImmediately)
    {
      return this.SubmitScheduledJobToScheduler(jobId, job, executeImmediately, true);
    }

    public bool DeleteJob(Guid jobId)
    {
      using (IJobSchedulerHelper localInstance = JobScheduler.GetLocalInstance())
      {
        try
        {
          ((IJobScheduler) localInstance).RemoveJob(jobId);
          return true;
        }
        catch
        {
          OrionDiscoveryJobFactory.log.DebugFormat("Unable to delete job in Job Engine({0}", (object) jobId);
          return false;
        }
      }
    }
  }
}
