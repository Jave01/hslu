// Decompiled with JetBrains decompiler
// Type: SolarWinds.Orion.Core.BusinessLayer.CoreBusinessLayerService
// Assembly: SolarWinds.Orion.Core.BusinessLayer, Version=2019.4.5200.9083, Culture=neutral, PublicKeyToken=null
// MVID: E12E8C85-5CD9-4E06-8801-182E5104FADE
// Assembly location: E:\task5.dll

using SolarWinds.Collector.Contract;
using SolarWinds.Common.Net;
using SolarWinds.Common.Snmp;
using SolarWinds.Common.Utility;
using SolarWinds.InformationService.Contract2;
using SolarWinds.InformationService.Linq;
using SolarWinds.InformationService.Linq.Plugins;
using SolarWinds.InformationService.Linq.Plugins.Core.Orion;
using SolarWinds.JobEngine;
using SolarWinds.JobEngine.Security;
using SolarWinds.Licensing.Framework;
using SolarWinds.Licensing.Framework.Interfaces;
using SolarWinds.Logging;
using SolarWinds.Net.SNMP;
using SolarWinds.Orion.Common;
using SolarWinds.Orion.Core.Actions;
using SolarWinds.Orion.Core.Actions.DAL;
using SolarWinds.Orion.Core.Actions.Runners;
using SolarWinds.Orion.Core.Alerting;
using SolarWinds.Orion.Core.Alerting.DAL;
using SolarWinds.Orion.Core.Alerting.Migration;
using SolarWinds.Orion.Core.Alerting.Migration.Plugins;
using SolarWinds.Orion.Core.Alerting.Models;
using SolarWinds.Orion.Core.Alerting.Plugins.Conditions.Sql;
using SolarWinds.Orion.Core.BusinessLayer.Agent;
using SolarWinds.Orion.Core.BusinessLayer.BL;
using SolarWinds.Orion.Core.BusinessLayer.CentralizedSettings;
using SolarWinds.Orion.Core.BusinessLayer.DAL;
using SolarWinds.Orion.Core.BusinessLayer.Discovery;
using SolarWinds.Orion.Core.BusinessLayer.Discovery.DiscoveryCache;
using SolarWinds.Orion.Core.BusinessLayer.OneTimeJobs;
using SolarWinds.Orion.Core.BusinessLayer.Thresholds;
using SolarWinds.Orion.Core.BusinessLayer.TraceRoute;
using SolarWinds.Orion.Core.CertificateUpdate;
using SolarWinds.Orion.Core.Common;
using SolarWinds.Orion.Core.Common.Agent;
using SolarWinds.Orion.Core.Common.Alerting;
using SolarWinds.Orion.Core.Common.BusinessLayer;
using SolarWinds.Orion.Core.Common.Catalogs;
using SolarWinds.Orion.Core.Common.CentralizedSettings;
using SolarWinds.Orion.Core.Common.DALs;
using SolarWinds.Orion.Core.Common.Enums;
using SolarWinds.Orion.Core.Common.ExpressionEvaluator;
using SolarWinds.Orion.Core.Common.Extensions;
using SolarWinds.Orion.Core.Common.i18n;
using SolarWinds.Orion.Core.Common.Indications;
using SolarWinds.Orion.Core.Common.InformationService;
using SolarWinds.Orion.Core.Common.JobEngine;
using SolarWinds.Orion.Core.Common.Licensing;
using SolarWinds.Orion.Core.Common.MacroParsing;
using SolarWinds.Orion.Core.Common.Models;
using SolarWinds.Orion.Core.Common.Models.Alerts;
using SolarWinds.Orion.Core.Common.Models.Mib;
using SolarWinds.Orion.Core.Common.Models.Technology;
using SolarWinds.Orion.Core.Common.Models.Thresholds;
using SolarWinds.Orion.Core.Common.PackageManager;
using SolarWinds.Orion.Core.Common.Proxy.Audit;
using SolarWinds.Orion.Core.Common.Proxy.BusinessLayer;
using SolarWinds.Orion.Core.Common.Settings;
using SolarWinds.Orion.Core.Common.Swis;
using SolarWinds.Orion.Core.Discovery;
using SolarWinds.Orion.Core.Discovery.BL;
using SolarWinds.Orion.Core.Discovery.DAL;
using SolarWinds.Orion.Core.Discovery.DataAccess;
using SolarWinds.Orion.Core.Jobs2;
using SolarWinds.Orion.Core.Models;
using SolarWinds.Orion.Core.Models.Actions;
using SolarWinds.Orion.Core.Models.Actions.Contexts;
using SolarWinds.Orion.Core.Models.Actions.Contracts;
using SolarWinds.Orion.Core.Models.Alerting;
using SolarWinds.Orion.Core.Models.Credentials;
using SolarWinds.Orion.Core.Models.DiscoveredObjects;
using SolarWinds.Orion.Core.Models.Discovery;
using SolarWinds.Orion.Core.Models.Enums;
using SolarWinds.Orion.Core.Models.Events;
using SolarWinds.Orion.Core.Models.Interfaces;
using SolarWinds.Orion.Core.Models.MacroParsing;
using SolarWinds.Orion.Core.Models.OldDiscoveryModels;
using SolarWinds.Orion.Core.Models.Technology;
using SolarWinds.Orion.Core.Models.WebIntegration;
using SolarWinds.Orion.Core.Pollers.Node.ResponseTime;
using SolarWinds.Orion.Core.Pollers.Node.WMI;
using SolarWinds.Orion.Core.SharedCredentials;
using SolarWinds.Orion.Core.SharedCredentials.Credentials;
using SolarWinds.Orion.Core.Strings;
using SolarWinds.Orion.Discovery.Contract.DiscoveryPlugin;
using SolarWinds.Orion.Discovery.Contract.Models;
using SolarWinds.Orion.Discovery.Framework;
using SolarWinds.Orion.Discovery.Framework.Pluggability;
using SolarWinds.Orion.MacroProcessor;
using SolarWinds.Orion.Pollers.Framework.SNMP;
using SolarWinds.Orion.ServiceDirectory.Wcf;
using SolarWinds.Orion.Web.Integration;
using SolarWinds.Orion.Web.Integration.Common.Models;
using SolarWinds.Orion.Web.Integration.Maintenance;
using SolarWinds.Orion.Web.Integration.SupportCases;
using SolarWinds.Serialization.Json;
using SolarWinds.ServiceDirectory.Client.Contract;
using SolarWinds.Settings;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel.Composition.Primitives;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

#nullable disable
namespace SolarWinds.Orion.Core.BusinessLayer
{
  [ServiceBehavior(Name = "CoreServiceEngine", InstanceContextMode = InstanceContextMode.Single, IncludeExceptionDetailInFaults = false, AutomaticSessionShutdown = true, ConcurrencyMode = ConcurrencyMode.Multiple)]
  [ErrorBehavior(typeof (CoreErrorHandler))]
  [SolarWinds.Orion.ServiceDirectory.Wcf.ServiceDirectory("Core.BusinessLayer")]
  public class CoreBusinessLayerService : 
    ICoreBusinessLayer,
    IDisposable,
    IOneTimeAgentDiscoveryJobFactory,
    IServiceDirectoryIntegration
  {
    private readonly Guid ItemTypeCertificateUpdateRequired = new Guid("{4E9EB71A-3A11-468E-A672-1E3E440E4F89}");
    private CertificateMaintenance certificateMaintenance = CertificateMaintenance.GetForFullMaintenance(BusinessLayerSettings.Instance.SafeCertificateMaintenanceTrialPeriod, BusinessLayerSettings.Instance.CertificateMaintenanceAgentPollFrequency);
    private Lazy<IActionRunner> actionRunner = new Lazy<IActionRunner>(new Func<IActionRunner>(CoreBusinessLayerService.CreateActionRunner));
    private readonly ActionMethodInvoker _actionMethodInvoker = new ActionMethodInvoker();
    private AuditingDAL auditingDal = new AuditingDAL();
    private IJobFactory _jobFactory;
    private IPersistentDiscoveryCache _persistentDiscoveryCache;
    private DiscoveryLogic discoveryLogic = new DiscoveryLogic();
    private static readonly Dictionary<string, Action<PollingEngineStatus, object>> statusParsers = new Dictionary<string, Action<PollingEngineStatus, object>>()
    {
      {
        "NetPerfMon Engine:Network Node Elements",
        (Action<PollingEngineStatus, object>) ((s, o) => s.NetworkNodeElements = Convert.ToInt32(o))
      },
      {
        "NetPerfMon Engine:Interface Elements",
        (Action<PollingEngineStatus, object>) ((s, o) => s.InterfaceElements = Convert.ToInt32(o))
      },
      {
        "NetPerfMon Engine:Volume Elements",
        (Action<PollingEngineStatus, object>) ((s, o) => s.VolumeElements = Convert.ToInt32(o))
      },
      {
        "NetPerfMon Engine:Date Time",
        (Action<PollingEngineStatus, object>) ((s, o) => s.DateTime = Convert.ToDateTime(o))
      },
      {
        "NetPerfMon Engine:Paused",
        (Action<PollingEngineStatus, object>) ((s, o) => s.Paused = Convert.ToBoolean(o))
      },
      {
        "Max Outstanding Polls",
        (Action<PollingEngineStatus, object>) ((s, o) => s.MaxOutstandingPolls = Convert.ToInt32(o))
      },
      {
        "Status Pollers:ICMP Status Polling Index",
        (Action<PollingEngineStatus, object>) ((s, o) => s.ICMPStatusPollingIndex = o.ToString())
      },
      {
        "Status Pollers:SNMP Status Polling Index",
        (Action<PollingEngineStatus, object>) ((s, o) => s.SNMPStatusPollingIndex = o.ToString())
      },
      {
        "Status Pollers:ICMP Polls per second",
        (Action<PollingEngineStatus, object>) ((s, o) => s.ICMPStatusPollsPerSecond = Convert.ToDouble(o))
      },
      {
        "Status Pollers:SNMP Polls per second",
        (Action<PollingEngineStatus, object>) ((s, o) => s.SNMPStatusPollsPerSecond = Convert.ToDouble(o))
      },
      {
        "Packet Queues:DNS Outstanding",
        (Action<PollingEngineStatus, object>) ((s, o) => s.DNSOutstanding = Convert.ToInt32(o))
      },
      {
        "Packet Queues:ICMP Outstanding",
        (Action<PollingEngineStatus, object>) ((s, o) => s.ICMPOutstanding = Convert.ToInt32(o))
      },
      {
        "Packet Queues:SNMP Outstanding",
        (Action<PollingEngineStatus, object>) ((s, o) => s.SNMPOutstanding = Convert.ToInt32(o))
      },
      {
        "Statistics Pollers:ICMP Statistic Polling Index",
        (Action<PollingEngineStatus, object>) ((s, o) => s.ICMPStatisticPollingIndex = o.ToString())
      },
      {
        "Statistics Pollers:SNMP Statistic Polling Index",
        (Action<PollingEngineStatus, object>) ((s, o) => s.SNMPStatisticPollingIndex = o.ToString())
      },
      {
        "Statistics Pollers:ICMP Polls per second",
        (Action<PollingEngineStatus, object>) ((s, o) => s.ICMPStatisticPollsPerSecond = Convert.ToDouble(o))
      },
      {
        "Statistics Pollers:SNMP Polls per second",
        (Action<PollingEngineStatus, object>) ((s, o) => s.SNMPStatisticPollsPerSecond = Convert.ToDouble(o))
      },
      {
        "Status Pollers:Max Status Polls Per Second",
        (Action<PollingEngineStatus, object>) ((s, o) => s.MaxStatusPollsPerSecond = Convert.ToInt32(o))
      },
      {
        "Statistics Pollers:Max Statistic Polls Per Second",
        (Action<PollingEngineStatus, object>) ((s, o) => s.MaxStatisticPollsPerSecond = Convert.ToInt32(o))
      }
    };
    private readonly object _syncRoot = new object();
    private readonly Dictionary<string, string> _elementCountQueries = new Dictionary<string, string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
    private readonly MibDAL mibDAL = new MibDAL();
    private static readonly List<NodeSubType> WmiCompatibleNodeSubTypes = new List<NodeSubType>()
    {
      (NodeSubType) 4,
      (NodeSubType) 3
    };
    private NodeAssignmentHelper nodeHelper = new NodeAssignmentHelper();
    private const string SourceInstanceUriProperty = "SourceInstanceUri";
    private const string UriProperty = "Uri";
    private ExpirableCache<string, IEnumerable<MaintenanceStatus>> _maintenanceInfoCache;
    private ExpirableCache<string, IEnumerable<SupportCase>> _supportCasesCache;
    private ExpirableCache<string, LicenseAndManagementInfo> _LAMInfoCache;
    public static readonly Version CoreBusinessLayerServiceVersion = typeof (CoreBusinessLayerService).Assembly.GetName().Version;
    private static readonly Log log = new Log();
    private readonly ManualResetEvent shutdownEvent = new ManualResetEvent(false);
    private readonly CoreBusinessLayerPlugin parent;
    private readonly bool _areInterfacesSupported;
    private readonly AuditingPluginManager _auditPluginManager = new AuditingPluginManager();
    private readonly IAgentInfoDAL _agentInfoDal;
    private readonly INodeBLDAL _nodeBlDal;
    private readonly ISettingsDAL _settingsDal;
    private readonly IOneTimeJobManager _oneTimeJobManager;
    private readonly IEngineDAL _engineDal;
    private readonly IEngineIdentityProvider _engineIdentityProvider;
    private readonly string _serviceLogicalInstanceId;

    private CertificateMaintenance Maintenance
    {
      get
      {
        if (this.certificateMaintenance == null)
          this.certificateMaintenance = RegistrySettings.IsFullOrion() ? CertificateMaintenance.GetForFullMaintenance(BusinessLayerSettings.Instance.SafeCertificateMaintenanceTrialPeriod, BusinessLayerSettings.Instance.CertificateMaintenanceAgentPollFrequency) : CertificateMaintenance.GetForDbSyncOnly();
        return this.certificateMaintenance;
      }
    }

    [Obsolete("Replacing MD5 certificates is not supported anymore - PRO-1041")]
    public void StartCertificateMaintenance() => this.Maintenance.StartCertificateMaintenance();

    [Obsolete("Replacing MD5 certificates is not supported anymore - PRO-1041")]
    public void ApproveBreakingCertificateMaintenance()
    {
      this.Maintenance.ApproveBreakingCertificateMaintenance();
    }

    [Obsolete("Replacing MD5 certificates is not supported anymore - PRO-1041")]
    public void RetryCertificateMaintenance()
    {
      this.RemoveCertificateMaintenanceNotification();
      this.Maintenance.RetryCertificateMaintenance();
    }

    [Obsolete("Replacing MD5 certificates is not supported anymore - PRO-1041")]
    public CertificateUpdateBlockingInfo GetCertificateUpdateBlockingInfo()
    {
      return this.Maintenance.GetCertificateUpdateBlockingInfo();
    }

    [Obsolete("Replacing MD5 certificates is not supported anymore - PRO-1041")]
    public CertificateMaintenanceResult GetCertificateMaintenanceStatus()
    {
      CertificateMaintenanceResult maintenanceStatus = this.Maintenance.GetCertificateMaintenanceStatus();
      if (maintenanceStatus == null || maintenanceStatus == 1)
        this.RemoveCertificateMaintenanceNotification();
      return maintenanceStatus;
    }

    [Obsolete("Replacing MD5 certificates is not supported anymore - PRO-1041")]
    public void RequestUserApprovalForCertificateMaintenance()
    {
      NotificationItem notificationItemById = this.GetNotificationItemById(this.ItemTypeCertificateUpdateRequired);
      if (notificationItemById != null)
      {
        if (CoreBusinessLayerService.ShouldNotificationBeRestored(notificationItemById))
        {
          CoreBusinessLayerService.log.Info((object) "Notification for certificate maintenance is acknowledged for long time. Unacknowledging it to remind user.");
          notificationItemById.SetNotAcknowledged();
          this.UpdateNotificationItem(notificationItemById);
        }
        else
          CoreBusinessLayerService.log.Info((object) "Notification for certificate maintenance already exists.");
      }
      else
      {
        CoreBusinessLayerService.log.Info((object) "Creating notification to confirm certificate maintenance.");
        this.InsertNotificationItem(new NotificationItem(this.ItemTypeCertificateUpdateRequired, Resources2.LIBCODE_OM0_6, Resources2.LIBCODE_OM0_7, DateTime.UtcNow, false, this.ItemTypeCertificateUpdateRequired, "/Orion/CertificateMaintenanceConfirmation.aspx", new DateTime?(), (string) null));
      }
    }

    private void RemoveCertificateMaintenanceNotification()
    {
      this.DeleteNotificationItemById(this.ItemTypeCertificateUpdateRequired);
    }

    private static bool ShouldNotificationBeRestored(NotificationItem notificationItem)
    {
      TimeSpan notificationReappearPeriod = BusinessLayerSettings.Instance.CertificateMaintenanceNotificationReappearPeriod;
      if (!notificationItem.IsAcknowledged)
        return false;
      DateTime utcNow = DateTime.UtcNow;
      DateTime? acknowledgedAt = notificationItem.AcknowledgedAt;
      TimeSpan? nullable = acknowledgedAt.HasValue ? new TimeSpan?(utcNow - acknowledgedAt.GetValueOrDefault()) : new TimeSpan?();
      TimeSpan timeSpan = notificationReappearPeriod;
      return nullable.HasValue && nullable.GetValueOrDefault() > timeSpan;
    }

    private static IActionRunner CreateActionRunner()
    {
      return (IActionRunner) new ActionRunner((IActionPluginsProvider) new MEFActionPluginsProvider(false), CoreBusinessLayerService.CreateProxy());
    }

    public ActionResult ExecuteAction(ActionDefinition actionDefinition, ActionContextBase context)
    {
      return this.actionRunner.Value.Execute(actionDefinition, context);
    }

    public string InvokeActionMethod(string actionTypeID, string methodName, string args)
    {
      return this._actionMethodInvoker.InvokeActionMethod(actionTypeID, methodName, args);
    }

    private static IInformationServiceProxyCreator CreateProxy()
    {
      return (IInformationServiceProxyCreator) new SwisConnectionProxyCreator((Func<SwisConnectionProxy>) (() => new SwisConnectionProxyFactory(true).CreateConnection()));
    }

    public string SimulateAction(ActionDefinition actionDefinition, ActionContextBase context)
    {
      return this.actionRunner.Value.Simulate(actionDefinition, context);
    }

    public int DeployAgent(AgentDeploymentSettings settings)
    {
      try
      {
        CoreBusinessLayerService.log.InfoFormat("DeployAgent on {0}-{1} called", (object) settings.IpAddress, (object) settings.Hostname);
        return new AgentDeployer().StartDeployingAgent(settings);
      }
      catch (Exception ex)
      {
        throw MessageUtilities.NewFaultException<CoreFaultContract>(ex);
      }
    }

    public void DeployAgentPlugins(int agentId, IEnumerable<string> requiredPlugins)
    {
      this.DeployAgentPlugins(agentId, requiredPlugins, (Action<AgentDeploymentStatus>) null);
    }

    protected void DeployAgentPlugins(
      int agentId,
      IEnumerable<string> requiredPlugins,
      Action<AgentDeploymentStatus> onFinishedCallback)
    {
      CoreBusinessLayerService.log.InfoFormat("DeployAgentPlugins called, agentId:{0}, requiredPlugins:{1}", (object) agentId, (object) string.Join(",", requiredPlugins));
      new AgentDeployer().StartDeployingPlugins(agentId, requiredPlugins, onFinishedCallback);
    }

    public string[] GetRequiredAgentDiscoveryPlugins()
    {
      return DiscoveryHelper.GetAgentDiscoveryPluginIds();
    }

    public void DeployAgentDiscoveryPlugins(int agentId)
    {
      this.DeployAgentDiscoveryPluginsAsync(agentId);
    }

    public Task<AgentDeploymentStatus> DeployAgentDiscoveryPluginsAsync(int agentId)
    {
      TaskCompletionSource<AgentDeploymentStatus> taskSource = new TaskCompletionSource<AgentDeploymentStatus>();
      string[] discoveryPlugins = this.GetRequiredAgentDiscoveryPlugins();
      this.DeployAgentPlugins(agentId, (IEnumerable<string>) discoveryPlugins, (Action<AgentDeploymentStatus>) (status => taskSource.TrySetResult(status)));
      return taskSource.Task;
    }

    public AgentInfo GetAgentInfo(int agentId)
    {
      return new AgentManager(this._agentInfoDal).GetAgentInfo(agentId);
    }

    public AgentInfo GetAgentInfoByNodeId(int nodeId)
    {
      return new AgentManager(this._agentInfoDal).GetAgentInfoByNodeId(nodeId);
    }

    public AgentInfo DetectAgent(string ipAddress, string hostname)
    {
      return new AgentManager(this._agentInfoDal).DetectAgent(ipAddress, hostname);
    }

    public AgentDeploymentInfo GetAgentDeploymentInfo(int agentId)
    {
      return AgentDeploymentWatcher.GetInstance(this._agentInfoDal).GetAgentDeploymentInfo(agentId);
    }

    public void UpdateAgentNodeId(int agentId, int nodeId)
    {
      new AgentManager(this._agentInfoDal).UpdateAgentNodeId(agentId, nodeId);
    }

    public void ResetAgentNodeId(int nodeId)
    {
      new AgentManager(this._agentInfoDal).ResetAgentNodeId(nodeId);
    }

    private void UpdateNotification()
    {
      CoreBusinessLayerService.log.Debug((object) "Agent deployed, update notification item");
    }

    public List<KeyValuePair<string, string>> GetAlertList() => AlertDAL.GetAlertList();

    [Obsolete("Old alerting will be removed. Use GetAlertList() method instead.")]
    public List<KeyValuePair<string, string>> GetAlertNames(bool includeBasic)
    {
      return AlertDAL.GetAlertList(includeBasic);
    }

    public List<NetObjectType> GetAlertNetObjectTypes() => ModuleAlertsMap.NetObjectTypes.Items;

    [Obsolete("Method does not return V2 alerts.")]
    public DataTable GetSortableAlertTable(
      string netObject,
      string deviceType,
      string alertID,
      string orderByClause,
      int maxRecords,
      bool showAcknowledged,
      List<int> limitationIDs,
      bool includeBasic)
    {
      return AlertDAL.GetSortableAlertTable(netObject, deviceType, alertID, orderByClause, maxRecords, showAcknowledged, limitationIDs, includeBasic);
    }

    public List<ActiveAlertDetailed> GetAlertTableByDate(
      DateTime date,
      int? lastAlertHistoryId,
      List<int> limitationIDs,
      bool showAcknowledged)
    {
      return new ActiveAlertDAL().GetAlertTableByDate(date.ToLocalTime(), lastAlertHistoryId, limitationIDs);
    }

    public int GetLastAlertHistoryId() => new AlertHistoryDAL().GetLastHystoryId();

    [Obsolete("Method does not return V2 alerts.")]
    public DataTable GetPageableAlerts(
      List<int> limitationIDs,
      string period,
      int fromRow,
      int toRow,
      string type,
      string alertId,
      bool showAcknAlerts)
    {
      return AlertDAL.GetPageableAlerts(limitationIDs, period, fromRow, toRow, type, alertId, showAcknAlerts);
    }

    [Obsolete("Method does not return V2 alerts.")]
    public DataTable GetAlertTable(
      string netObject,
      string deviceType,
      string alertID,
      int maxRecords,
      bool showAcknowledged,
      List<int> limitationIDs)
    {
      return AlertDAL.GetAlertTable(netObject, deviceType, alertID, maxRecords, showAcknowledged, limitationIDs);
    }

    [Obsolete("Method does not return V2 alerts.")]
    public DataTable GetAlerts(
      string netObject,
      string deviceType,
      string alertID,
      int maxRecords,
      bool showAcknowledged,
      List<int> limitationIDs,
      bool includeBasic)
    {
      return AlertDAL.GetAlertTable(netObject, deviceType, alertID, maxRecords, showAcknowledged, limitationIDs, includeBasic);
    }

    [Obsolete("Old alerting will be removed")]
    public void AcknowledgeAlertsAction(List<string> alertKeys, string accountID)
    {
      AlertDAL.AcknowledgeAlertsAction(alertKeys, accountID);
      this.FireUpdateNotification((IEnumerable<string>) alertKeys, (AlertUpdatedIndicationType) 1, accountID);
    }

    [Obsolete("Old alerting will be removed")]
    public void AcknowledgeAlertsFromAlertManager(List<string> alertKeys, string accountID)
    {
      AlertDAL.AcknowledgeAlertsAction(alertKeys, accountID, (AlertAcknowledgeType) 2, (string) null);
      this.FireUpdateNotification((IEnumerable<string>) alertKeys, (AlertUpdatedIndicationType) 1, accountID);
    }

    [Obsolete("Old alerting will be removed")]
    public void UnacknowledgeAlertsFromAlertManager(List<string> alertKeys, string accountID)
    {
      AlertDAL.UnacknowledgeAlertsAction(alertKeys, accountID, (AlertAcknowledgeType) 2);
      this.FireUpdateNotification((IEnumerable<string>) alertKeys, (AlertUpdatedIndicationType) 1, accountID);
    }

    [Obsolete("Old alerting will be removed")]
    public void AcknowledgeAlerts(List<string> alertKeys, string accountID, bool viaEmail)
    {
      AlertDAL.AcknowledgeAlertsAction(alertKeys, accountID, viaEmail);
      this.FireUpdateNotification((IEnumerable<string>) alertKeys, (AlertUpdatedIndicationType) 1, accountID);
    }

    [Obsolete("Old alerting will be removed")]
    public void AcknowledgeAlertsWithNotes(List<string> alertKeys, string accountID, string notes)
    {
      AlertDAL.AcknowledgeAlertsAction(alertKeys, accountID, false, notes);
      this.FireUpdateNotification((IEnumerable<string>) alertKeys, (AlertUpdatedIndicationType) 3, accountID);
    }

    public int GetAlertObjectId(string alertkey) => AlertDAL.GetAlertObjectId(alertkey);

    public int AcknowledgeAlertsWithMethod(
      List<string> alertKeys,
      string accountId,
      string notes,
      string method)
    {
      List<string> stringList = new List<string>();
      List<int> intList = new List<int>();
      foreach (string alertKey in alertKeys)
      {
        string empty1 = string.Empty;
        string activeObject = string.Empty;
        string empty2 = string.Empty;
        if (AlertsHelper.TryParseAlertKey(alertKey.Replace("swis://", "swis//"), ref empty1, ref activeObject, ref empty2))
        {
          activeObject = activeObject.Replace("swis//", "swis://");
          if (!activeObject.StartsWith("swis://"))
          {
            CoreBusinessLayerService.log.WarnFormat("Unable to acknowledge alert {0} for net object {1}. Old alerts aren't supported.", (object) alertKey, (object) activeObject);
          }
          else
          {
            int alertObjectId = this.GetAlertObjectId(empty1, activeObject, empty2);
            if (alertObjectId > 0)
              intList.Add(alertObjectId);
          }
        }
      }
      int num = 0;
      if (intList.Any<int>())
        num += this.AcknowledgeAlertsWithMethodV2((IEnumerable<int>) intList, accountId, notes, DateTime.UtcNow, method);
      return num;
    }

    public int AcknowledgeAlertsV2(
      IEnumerable<int> alertObjectIds,
      string accountId,
      string notes,
      DateTime acknowledgeDateTime)
    {
      return this.AcknowledgeAlertsWithMethodV2(alertObjectIds, accountId, notes, acknowledgeDateTime, (string) null);
    }

    public int AcknowledgeAlertsWithMethodV2(
      IEnumerable<int> alertObjectIds,
      string accountId,
      string notes,
      DateTime acknowledgeDateTime,
      string method)
    {
      ActiveAlertDAL activeAlertDal = new ActiveAlertDAL();
      IEnumerable<int> alertObjectIds1 = activeAlertDal.LimitAlertAckStateUpdateCandidates(alertObjectIds, true);
      List<IIndication> iindicationList1 = new List<IIndication>();
      iindicationList1.AddRange((IEnumerable<IIndication>) activeAlertDal.GetAlertUpdatedIndicationPropertiesByAlertObjectIds(alertObjectIds1, accountId, notes, acknowledgeDateTime, true, method));
      DataTable byAlertObjectIds = activeAlertDal.GetAlertResetOrUpdateIndicationPropertiesTableByAlertObjectIds(alertObjectIds);
      foreach (int alertObjectId in alertObjectIds)
      {
        DataRow[] dataRowArray = byAlertObjectIds.Select("AlertObjectID=" + (object) alertObjectId);
        PropertyBag propertyBag1 = new PropertyBag();
        if (dataRowArray.Length != 0)
        {
          ((Dictionary<string, object>) propertyBag1).Add("Acknowledged", dataRowArray[0]["Acknowledged"] != DBNull.Value ? (object) Convert.ToString(dataRowArray[0]["Acknowledged"]) : (object) "False");
          ((Dictionary<string, object>) propertyBag1).Add("AcknowledgedBy", dataRowArray[0]["AcknowledgedBy"] != DBNull.Value ? (object) Convert.ToString(dataRowArray[0]["AcknowledgedBy"]) : (object) string.Empty);
          ((Dictionary<string, object>) propertyBag1).Add("AcknowledgedDateTime", dataRowArray[0]["AcknowledgedDateTime"] != DBNull.Value ? (object) Convert.ToString(dataRowArray[0]["AcknowledgedDateTime"]) : (object) string.Empty);
          ((Dictionary<string, object>) propertyBag1).Add("AlertNote", dataRowArray[0]["AlertNote"] != DBNull.Value ? (object) Convert.ToString(dataRowArray[0]["AlertNote"]) : (object) string.Empty);
        }
        List<IIndication> iindicationList2 = iindicationList1;
        PropertyBag propertyBag2 = new PropertyBag();
        ((Dictionary<string, object>) propertyBag2).Add("AlertObjectID", (object) alertObjectId);
        ((Dictionary<string, object>) propertyBag2).Add("Acknowledged", (object) "True");
        ((Dictionary<string, object>) propertyBag2).Add("AcknowledgedBy", (object) accountId);
        ((Dictionary<string, object>) propertyBag2).Add("AcknowledgedDateTime", (object) acknowledgeDateTime);
        ((Dictionary<string, object>) propertyBag2).Add("AlertNote", (object) notes);
        ((Dictionary<string, object>) propertyBag2).Add("PreviousProperties", (object) propertyBag1);
        ((Dictionary<string, object>) propertyBag2).Add("InstanceType", (object) "Orion.AlertActive");
        CommonIndication commonIndication = new CommonIndication((IndicationType) 2, propertyBag2);
        iindicationList2.Add((IIndication) commonIndication);
      }
      int num = activeAlertDal.AcknowledgeActiveAlerts(alertObjectIds1, accountId, notes, acknowledgeDateTime);
      if (num <= 0)
        return num;
      IndicationPublisher.CreateV3().ReportIndications((IEnumerable<IIndication>) iindicationList1);
      return num;
    }

    public AlertAcknowledgeResult AcknowledgeAlert(
      string alertId,
      string netObjectId,
      string objectType,
      string accountId,
      string notes,
      string method)
    {
      AlertAcknowledgeResult acknowledgeResult = (AlertAcknowledgeResult) 0;
      if (!netObjectId.StartsWith("swis://"))
      {
        CoreBusinessLayerService.log.WarnFormat("Unable to acknowledge alert {0} for net object {1}. Old alerts aren't supported.", (object) alertId, (object) netObjectId);
      }
      else
      {
        int alertObjectId = this.GetAlertObjectId(alertId, netObjectId, objectType);
        if (alertObjectId > 0)
          acknowledgeResult = this.AcknowledgeAlertsV2((IEnumerable<int>) new List<int>()
          {
            alertObjectId
          }, accountId, notes, DateTime.UtcNow) == 1 ? (AlertAcknowledgeResult) 0 : (AlertAcknowledgeResult) -1;
      }
      return acknowledgeResult;
    }

    [Obsolete("Old alerting will be removed", true)]
    public void ClearTriggeredAlerts(List<string> alertKeys)
    {
      this.FireResetNotification((IEnumerable<string>) alertKeys, IndicationConstants.SystemAccountId);
      AlertDAL.ClearTriggeredAlert(alertKeys);
    }

    public void ClearTriggeredAlertsV2(IEnumerable<int> alertObjectIds, string accountId)
    {
      ActiveAlertDAL activeAlertDal = new ActiveAlertDAL();
      IEnumerable<AlertClearedIndicationProperties> byAlertObjectIds = activeAlertDal.GetAlertClearedIndicationPropertiesByAlertObjectIds(alertObjectIds);
      List<IIndication> iindicationList = new List<IIndication>();
      foreach (AlertClearedIndicationProperties indicationProperties in byAlertObjectIds)
      {
        AlertClearedIndication clearedIndication = new AlertClearedIndication(!string.IsNullOrEmpty(accountId) ? accountId : IndicationConstants.SystemAccountId, (AlertIndicationProperties) indicationProperties);
        iindicationList.Add((IIndication) clearedIndication);
      }
      foreach (int alertObjectId in alertObjectIds)
      {
        CommonIndication commonIndication = new CommonIndication((IndicationType) 1, new AlertDeletedIndicationProperties()
        {
          AlertObjectId = alertObjectId
        }.CreatePropertyBag());
        iindicationList.Add((IIndication) commonIndication);
      }
      activeAlertDal.ClearTriggeredActiveAlerts(alertObjectIds, accountId);
      IndicationPublisher.CreateV3().ReportIndications((IEnumerable<IIndication>) iindicationList);
    }

    [Obsolete("Old alerting will be removed")]
    public int EnableAdvancedAlert(Guid alertDefID, bool enable)
    {
      return AlertDAL.EnableAdvancedAlert(alertDefID, enable);
    }

    [Obsolete("Old alerting will be removed")]
    public int EnableAdvancedAlerts(List<string> alertDefIDs, bool enable, bool enableAll)
    {
      return AlertDAL.EnableAdvancedAlerts(alertDefIDs, enable, enableAll);
    }

    [Obsolete("Old alerting will be removed")]
    public int RemoveAdvancedAlert(Guid alertDefID) => AlertDAL.RemoveAdvancedAlert(alertDefID);

    public int RemoveAdvancedAlerts(List<string> alertDefIDs, bool deleteAll)
    {
      return AlertDAL.RemoveAdvancedAlerts(alertDefIDs, deleteAll);
    }

    [Obsolete("Old alerting will be removed")]
    public int UpdateAlertDef(
      Guid alertDefID,
      string alertName,
      string alertDescr,
      bool enabled,
      int evInterval,
      string dow,
      DateTime startTime,
      DateTime endTime,
      bool ignoreTimeout)
    {
      return AlertDAL.UpdateAlertDef(alertDefID, alertName, alertDescr, enabled, evInterval, dow, startTime, endTime, ignoreTimeout);
    }

    [Obsolete("Old alerting will be removed")]
    public DataTable GetAdvancedAlerts() => AlertDAL.GetAdvancedAlerts();

    [Obsolete("Old alerting will be removed")]
    public DataTable GetPagebleAdvancedAlerts(
      string sortColumn,
      string sortDirection,
      int startRowNumber,
      int pageSize)
    {
      return AlertDAL.GetPagebleAdvancedAlerts(sortColumn, sortDirection, startRowNumber, pageSize);
    }

    public ActiveAlertPage GetPageableActiveAlerts(
      PageableActiveAlertRequest pageableRequest,
      ActiveAlertsRequest activeAlertsRequest = null)
    {
      return AlertDAL.GetPageableActiveAlerts(pageableRequest, activeAlertsRequest);
    }

    public ActiveAlertObjectPage GetPageableActiveAlertObjects(
      PageableActiveAlertObjectRequest request)
    {
      return new ActiveAlertDAL().GetPageableActiveAlertObjects(request);
    }

    public ActiveAlert GetActiveAlert(
      ActiveAlertUniqueidentifier activeAlertUniqIdentifier,
      IEnumerable<int> limitationIDs)
    {
      return new ActiveAlertDAL().GetActiveAlert(activeAlertUniqIdentifier.AlertObjectID, limitationIDs);
    }

    public AlertHistoryPage GetActiveAlertHistory(
      int alertObjectId,
      PageableActiveAlertRequest request)
    {
      return new AlertHistoryDAL().GetActiveAlertHistory(alertObjectId, request);
    }

    [Obsolete("Old alerting will be removed")]
    public int AdvAlertsCount() => AlertDAL.AdvAlertsCount();

    [Obsolete("Old alerting will be removed")]
    public DataTable GetAdvancedAlert(Guid alertDefID) => AlertDAL.GetAdvancedAlert(alertDefID);

    private int GetAlertObjectId(string alertDefId, string activeObject, string objectType)
    {
      int objectId = 0;
      string note = string.Empty;
      this.GetAlertObjectIdAndAlertNote(alertDefId, activeObject, objectType, out objectId, out note);
      return objectId;
    }

    private void GetAlertObjectIdAndAlertNote(
      string alertDefId,
      string activeObject,
      string objectType,
      out int objectId,
      out string note)
    {
      objectId = 0;
      note = string.Empty;
      string str = "SELECT AO.AlertObjectID, AO.AlertNote FROM Orion.AlertObjects AO \r\n                                    INNER JOIN Orion.AlertConfigurations AC ON AO.AlertID=AC.AlertID\r\n                                    WHERE EntityUri=@entityUri AND EntityType=@objectType AND AC.AlertRefID=@alertDefId";
      using (IInformationServiceProxy2 iinformationServiceProxy2 = ((IInformationServiceProxyCreator) SwisConnectionProxyPool.GetCreator()).Create())
      {
        DataTable dataTable = ((IInformationServiceProxy) iinformationServiceProxy2).Query(str, (IDictionary<string, object>) new Dictionary<string, object>()
        {
          {
            "entityUri",
            (object) activeObject
          },
          {
            nameof (objectType),
            (object) objectType
          },
          {
            nameof (alertDefId),
            (object) alertDefId
          }
        });
        if (dataTable.Rows.Count <= 0)
          return;
        objectId = dataTable.Rows[0]["AlertObjectID"] != DBNull.Value ? Convert.ToInt32(dataTable.Rows[0]["AlertObjectID"]) : 0;
        note = dataTable.Rows[0]["AlertNote"] != DBNull.Value ? Convert.ToString(dataTable.Rows[0]["AlertNote"]) : string.Empty;
      }
    }

    public int AppendNoteToAlert(
      string alertDefId,
      string activeObject,
      string objectType,
      string note)
    {
      string accountId = AuditMessageInspector.UserContext ?? IndicationConstants.SystemAccountId;
      int alert = 0;
      if (!activeObject.StartsWith("swis://"))
      {
        CoreBusinessLayerService.log.WarnFormat("Unable to append Note to alert {0}. Old alerts aren't supported.", (object) activeObject);
      }
      else
      {
        int objectId = 0;
        string note1 = string.Empty;
        this.GetAlertObjectIdAndAlertNote(alertDefId, activeObject, objectType, out objectId, out note1);
        if (objectId > 0)
          alert = this.SetAlertNote(objectId, accountId, note, DateTime.UtcNow, note1) ? 1 : 0;
      }
      return alert;
    }

    [Obsolete("Old alerting will be removed")]
    public int UpdateAdvancedAlertNote(
      string alerfDefID,
      string activeObject,
      string objectType,
      string notes)
    {
      int num = AlertDAL.UpdateAdvancedAlertNote(alerfDefID, activeObject, objectType, notes);
      this.FireUpdateNotification((IEnumerable<string>) new string[1]
      {
        AlertsHelper.GetAlertKey(alerfDefID, activeObject, objectType)
      }, (AlertUpdatedIndicationType) 2, IndicationConstants.SystemAccountId);
      return num;
    }

    [Obsolete("Old alerting will be removed")]
    public AlertNotificationSettings GetAlertNotificationSettings(
      string alertDefID,
      string netObjectType,
      string alertName)
    {
      return AlertDAL.GetAlertNotificationSettings(alertDefID, netObjectType, alertName);
    }

    [Obsolete("Old alerting will be removed")]
    public void SetAlertNotificationSettings(string alertDefID, AlertNotificationSettings settings)
    {
      AlertDAL.SetAlertNotificationSettings(alertDefID, settings);
    }

    [Obsolete("Old alerting will be removed")]
    public AlertNotificationSettings GetBasicAlertNotificationSettings(
      int alertID,
      string netObjectType,
      int propertyID,
      string alertName)
    {
      return AlertDAL.GetBasicAlertNotificationSettings(alertID, netObjectType, propertyID, alertName);
    }

    public void SetBasicAlertNotificationSettings(int alertID, AlertNotificationSettings settings)
    {
      AlertDAL.SetBasicAlertNotificationSettings(alertID, settings);
    }

    [Obsolete("Old alerting will be removed", true)]
    private void FireUpdateNotification(
      IEnumerable<string> alertKeys,
      AlertUpdatedIndicationType type,
      string accountId)
    {
      this.FireNotification<AlertUpdatedIndicationProperties, AlertUpdatedIndication>(alertKeys, accountId, "Alert Update", (Action<AlertNotificationDetails, AlertUpdatedIndicationProperties>) ((notificationDetails, indicationProperties) =>
      {
        indicationProperties.Type = type;
        indicationProperties.Acknowledged = notificationDetails.Acknowledged;
        indicationProperties.AcknowledgedBy = notificationDetails.AcknowledgedBy;
        indicationProperties.AcknowledgedMethod = notificationDetails.AcknowledgedMethod;
        indicationProperties.Notes = notificationDetails.Notes;
      }));
    }

    [Obsolete("Old alerting will be removed", true)]
    private void FireResetNotification(IEnumerable<string> alertKeys, string accountId)
    {
      this.FireNotification<AlertResetIndicationProperties, AlertResetIndication>(alertKeys, accountId, "Alert Reset", (Action<AlertNotificationDetails, AlertResetIndicationProperties>) ((notificationDetails, indicationProperties) => indicationProperties.ResetTime = DateTime.UtcNow));
    }

    [Obsolete("Old alerting will be removed")]
    private void FireNotification<TProperties, TIndication>(
      IEnumerable<string> alertKeys,
      string accountId,
      string name,
      Action<AlertNotificationDetails, TProperties> customIndicationPropertiesHandler)
      where TProperties : AlertIndicationProperties, new()
      where TIndication : AlertIndication
    {
      CoreBusinessLayerService.log.DebugFormat("Firing {0} notifications", (object) name);
      MacroParser macroParser = new MacroParser(new Action<string, int>(BusinessLayerOrionEvent.WriteEvent));
      using (macroParser.MyDBConnection = DatabaseFunctions.CreateConnection(false))
      {
        foreach (string alertKey in alertKeys)
        {
          try
          {
            string alertDefID;
            string activeObject;
            string objectType;
            if (!AlertsHelper.TryParseAlertKey(alertKey, ref alertDefID, ref activeObject, ref objectType))
            {
              CoreBusinessLayerService.log.WarnFormat("Error firing notification for {0} because of invalid alert key {1}", (object) name, (object) alertKey);
            }
            else
            {
              AlertNotificationDetails detailsForNotification = AlertDAL.GetAlertDetailsForNotification(alertDefID, activeObject, objectType);
              if (detailsForNotification != null)
              {
                if (detailsForNotification.NotificationSettings.Enabled)
                {
                  macroParser.ObjectType = detailsForNotification.ObjectType;
                  macroParser.ActiveObject = detailsForNotification.ActiveObject;
                  macroParser.ObjectName = detailsForNotification.ObjectName;
                  macroParser.AlertID = new Guid(detailsForNotification.AlertDefinitionId);
                  macroParser.AlertName = detailsForNotification.AlertName;
                  macroParser.AlertMessage = detailsForNotification.AlertMessage;
                  macroParser.AlertTriggerTime = detailsForNotification.TriggerTimeStamp.ToLocalTime();
                  macroParser.AlertTriggerCount = detailsForNotification.TriggerCount;
                  macroParser.Acknowledged = detailsForNotification.Acknowledged;
                  macroParser.AcknowledgedBy = detailsForNotification.AcknowledgedBy;
                  macroParser.AcknowledgedTime = detailsForNotification.AcknowledgedTime.ToLocalTime();
                  TProperties indicationProperties = new AlertNotificationSettingsProvider().GetAlertIndicationProperties<TProperties>((System.Func<string, string>) (s => macroParser.ParseMacros(s, false)), detailsForNotification.ActiveObject, detailsForNotification.ObjectType, detailsForNotification.ObjectName, new Guid(detailsForNotification.AlertDefinitionId), detailsForNotification.AlertName, detailsForNotification.TriggerTimeStamp, detailsForNotification.NotificationSettings);
                  customIndicationPropertiesHandler(detailsForNotification, indicationProperties);
                  IndicationPublisher.CreateV3().ReportIndication((IIndication) Activator.CreateInstance(typeof (TIndication), (object) accountId, (object) indicationProperties));
                }
              }
            }
          }
          catch (Exception ex)
          {
            CoreBusinessLayerService.log.Error((object) string.Format("Error firing {0} notification", (object) name), ex);
          }
        }
      }
    }

    public IEnumerable<AlertScopeItem> GetObjectsInAlertScope(int[] alertIds)
    {
      List<AlertScopeItem> objectsInAlertScope = new List<AlertScopeItem>();
      ISwisConnectionProxyCreator creator = SwisConnectionProxyPool.GetCreator();
      IAlertDefinitionsDAL ialertDefinitionsDal = AlertDefinitionsDAL.Create((IConditionTypeProvider) ConditionTypeProvider.Create((IInformationServiceProxyCreator) creator), (IInformationServiceProxyCreator) creator);
      foreach (int alertId in alertIds)
      {
        if (!ialertDefinitionsDal.Exist(alertId))
        {
          if (CoreBusinessLayerService.log.IsDebugEnabled)
            CoreBusinessLayerService.log.DebugFormat("There is no AlertDefinition with AlertId={0}", (object) alertId);
        }
        else
        {
          AlertDefinition alertDefinition = ialertDefinitionsDal.Get(alertId);
          if (alertDefinition.Trigger.Conditions[0].Type is IConditionEntityScope type)
          {
            IEnumerable<EntityInstance> list = (IEnumerable<EntityInstance>) type.GetScope(alertDefinition.Trigger.Conditions[0].Condition, alertDefinition.Trigger.Conditions[0].ObjectType).ToList<EntityInstance>();
            if (list.Any<EntityInstance>())
            {
              string str = (string) null;
              Entity entityByObjectType = alertDefinition.Trigger.Conditions[0].Type.EntityProvider.GetEntityByObjectType(alertDefinition.Trigger.Conditions[0].ObjectType);
              if (entityByObjectType != null)
                str = entityByObjectType.FullName;
              foreach (EntityInstance entityInstance in list)
                objectsInAlertScope.Add(new AlertScopeItem()
                {
                  InstanceName = entityInstance.DisplayName,
                  ObjectId = entityInstance.Uri,
                  AlertId = alertId,
                  EntityType = str
                });
            }
          }
        }
      }
      return (IEnumerable<AlertScopeItem>) objectsInAlertScope;
    }

    public IEnumerable<AlertScopeItem> GetAllAlertsInObjectScopeWithParams(
      string entityType,
      string[] objectIds,
      bool loadAction,
      bool loadSchedules)
    {
      List<AlertScopeItem> resultItems = new List<AlertScopeItem>();
      ISwisConnectionProxyCreator creator = SwisConnectionProxyPool.GetCreator();
      IEnumerable<AlertScopeItem> whichCanAffectObject = this.GetAlertsWhichCanAffectObject(entityType, (IInformationServiceProxyCreator) creator, loadAction, loadSchedules);
      List<AlertScopeItem> alertScopeItems = new List<AlertScopeItem>();
      int num = 0;
      foreach (AlertScopeItem alertScopeItem in whichCanAffectObject)
      {
        bool flag = false;
        if (alertScopeItem.ScopeQuery == null || alertScopeItem.ScopeQuery.Params == null || alertScopeItem.ScopeQuery.Params.Count == 0)
        {
          alertScopeItems.Add(alertScopeItem);
          flag = true;
        }
        else if (alertScopeItem.ScopeQuery.Params.Count + num < 2000)
        {
          alertScopeItems.Add(alertScopeItem);
          num += alertScopeItem.ScopeQuery.Params.Count;
          flag = true;
        }
        if (!flag)
        {
          this.GetAlertsForTheBulk(alertScopeItems, entityType, objectIds, creator, resultItems);
          alertScopeItems.Clear();
          alertScopeItems.Add(alertScopeItem);
          num = alertScopeItem.ScopeQuery.Params.Count;
        }
      }
      this.GetAlertsForTheBulk(alertScopeItems, entityType, objectIds, creator, resultItems);
      return (IEnumerable<AlertScopeItem>) resultItems;
    }

    public IEnumerable<AlertScopeItem> GetAllAlertsInObjectScope(
      string entityType,
      string[] objectIds)
    {
      return this.GetAllAlertsInObjectScopeWithParams(entityType, objectIds, true, true);
    }

    private void GetAlertsForTheBulk(
      List<AlertScopeItem> alertScopeItems,
      string entityType,
      string[] objectIds,
      ISwisConnectionProxyCreator swisCreator,
      List<AlertScopeItem> resultItems)
    {
      if (alertScopeItems == null)
        return;
      if (alertScopeItems.Count == 0)
        return;
      try
      {
        List<AlertScopeItem> alertsForQueries = this.GetAlertsForQueries((IEnumerable<AlertScopeItem>) alertScopeItems, entityType, objectIds, swisCreator);
        if (alertsForQueries == null || alertsForQueries.Count <= 0)
          return;
        foreach (AlertScopeItem alertScopeItem in alertsForQueries)
          resultItems.Add(alertScopeItem);
      }
      catch (Exception ex)
      {
        CoreBusinessLayerService.log.ErrorFormat("Error occurred during validating alert scope queries for {0} type {1} objects and {2} AlertScope elements", (object) entityType, (object) objectIds.Length, (object) alertScopeItems.Count);
      }
    }

    private List<AlertScopeItem> GetAlertsForQueries(
      IEnumerable<AlertScopeItem> alertScopeItems,
      string entityType,
      string[] objectIds,
      ISwisConnectionProxyCreator swisCreator)
    {
      List<AlertScopeItem> alertsForQueries = new List<AlertScopeItem>();
      Tuple<string, IDictionary<string, object>> tuple = this.PrepareQueryForAlerts(entityType, alertScopeItems, objectIds);
      if (!string.IsNullOrEmpty(tuple.Item1))
      {
        using (IInformationServiceProxy2 iinformationServiceProxy2 = ((IInformationServiceProxyCreator) swisCreator).Create())
        {
          DataTable dataTable = ((IInformationServiceProxy) iinformationServiceProxy2).Query(tuple.Item1, tuple.Item2);
          if (dataTable != null)
          {
            if (dataTable.Rows.Count > 0)
            {
              Dictionary<int, string> dictionary = alertScopeItems.ToDictionary<AlertScopeItem, int, string>((System.Func<AlertScopeItem, int>) (p => p.AlertId), (System.Func<AlertScopeItem, string>) (q => q.InstanceName));
              foreach (DataRow row in (InternalDataCollectionBase) dataTable.Rows)
              {
                int int32 = Convert.ToInt32(row["AlertId"]);
                string str1 = dictionary[int32];
                int num = int32;
                string str2 = row["ObjectId"].ToString();
                alertsForQueries.Add(new AlertScopeItem()
                {
                  AlertId = num,
                  EntityType = entityType,
                  InstanceName = str1,
                  ObjectId = str2
                });
              }
            }
          }
        }
      }
      return alertsForQueries;
    }

    private Tuple<string, IDictionary<string, object>> PrepareQueryForAlerts(
      string entityType,
      IEnumerable<AlertScopeItem> alertScopeItems,
      string[] objectIds)
    {
      string str1 = string.Join(",", ((IEnumerable<string>) objectIds).Select<string, string>((System.Func<string, string>) (p => "'" + p + "'")));
      if (string.IsNullOrEmpty("Uri"))
        throw new InvalidInputException(string.Format("Orion.Alert can't recognize {0} entity, check Orion.NetObjectTypesExt if exists", (object) entityType));
      int num1 = 1;
      IDictionary<string, object> dictionary = (IDictionary<string, object>) new Dictionary<string, object>();
      StringBuilder stringBuilder = new StringBuilder();
      foreach (AlertScopeItem alertScopeItem in alertScopeItems)
      {
        string str2 = string.Format("SELECT {0} AS AlertId, '{1}' AS ObjectId FROM {2} AS E0 WHERE E0.{3} IN ({4})", (object) alertScopeItem.AlertId, (object) objectIds[0], (object) entityType, (object) "Uri", (object) str1);
        if (string.IsNullOrEmpty(alertScopeItem.ScopeQuery.Query))
        {
          CoreBusinessLayerService.log.Warn((object) "Object scope can be evaluated because ScopeQuery (query for evaluation) is not provided (null)");
        }
        else
        {
          int num2 = alertScopeItem.ScopeQuery.Query.IndexOf("WHERE", StringComparison.OrdinalIgnoreCase);
          if (num2 < 0)
          {
            int num3 = stringBuilder.Length == 0 ? 1 : 0;
            if (num3 == 0)
              stringBuilder.AppendLine("UNION (");
            stringBuilder.AppendLine(str2);
            if (num3 == 0)
              stringBuilder.AppendLine(")");
          }
          else
          {
            string str3 = Regex.Replace(alertScopeItem.ScopeQuery.Query.Substring(num2 + 5).Trim(), "\\s+", " ") + " ";
            foreach (string key in alertScopeItem.ScopeQuery.Params.Keys)
            {
              string oldValue = string.Format("@{0} ", (object) key);
              string newValue = string.Format("@{0}no{1} ", (object) key, (object) num1);
              str3 = str3.Replace(oldValue, newValue);
              dictionary.Add(key + "no" + (object) num1, alertScopeItem.ScopeQuery.Params[key]);
            }
            int num4 = stringBuilder.Length == 0 ? 1 : 0;
            if (num4 == 0)
              stringBuilder.AppendLine("UNION (");
            stringBuilder.AppendLine(string.Format("{0} AND {1}", (object) str2, (object) str3));
            ++num1;
            if (num4 == 0)
              stringBuilder.AppendLine(")");
          }
        }
      }
      string str4 = stringBuilder.ToString();
      if (CoreBusinessLayerService.log.IsDebugEnabled)
        CoreBusinessLayerService.log.DebugFormat("Provided query to evaluation of alert scope: {0}", (object) str4);
      return Tuple.Create<string, IDictionary<string, object>>(str4, dictionary);
    }

    private IEnumerable<AlertScopeItem> GetAlertsWhichCanAffectObject(
      string entityType,
      IInformationServiceProxyCreator swisCreator,
      bool loadActions,
      bool loadSchedules)
    {
      List<AlertScopeItem> results = new List<AlertScopeItem>();
      ConditionTypeProvider conditionTypeProvider = ConditionTypeProvider.Create(swisCreator);
      IAlertDefinitionsDAL ialertDefinitionsDal = AlertDefinitionsDAL.Create((IConditionTypeProvider) conditionTypeProvider, swisCreator);
      string objectType = ((ConditionTypeBase) conditionTypeProvider.Get("Core.Dynamic")).EntityProvider.GetObjectTypeByEntityFullName(entityType);
      IEnumerable<AlertDefinition> objectTypeWithParams = ialertDefinitionsDal.GetValidItemsOfObjectTypeWithParams(objectType, loadActions, loadSchedules);
      if (objectTypeWithParams != null)
      {
        IEnumerable<AlertDefinition> source = objectTypeWithParams.Where<AlertDefinition>((System.Func<AlertDefinition, bool>) (p => p.Enabled));
        object obj = new object();
        Parallel.ForEach<AlertDefinition>(source, (Action<AlertDefinition>) (definition =>
        {
          if (definition.Trigger.Conditions.Count <= 0 || !(definition.Trigger.Conditions[0].Type is IConditionEntityScope type2))
            return;
          QueryResult scopeQuery = type2.GetScopeQuery(definition.Trigger.Conditions[0].Condition, objectType);
          AlertScopeItem alertScopeItem = new AlertScopeItem()
          {
            AlertId = Convert.ToInt32((object) definition.AlertID),
            InstanceName = definition.Name,
            ScopeQuery = scopeQuery,
            EntityType = entityType
          };
          lock (obj)
            results.Add(alertScopeItem);
        }));
      }
      return (IEnumerable<AlertScopeItem>) results;
    }

    public AlertScopeItemDetails GetAlertScopeDetails(int alertId)
    {
      DataTable dataTable1;
      DataTable dataTable2;
      using (IInformationServiceProxy2 iinformationServiceProxy2 = ((IInformationServiceProxyCreator) SwisConnectionProxyPool.GetCreator()).Create())
      {
        dataTable1 = ((IInformationServiceProxy) iinformationServiceProxy2).Query("SELECT Field \r\n\t                                                  FROM Orion.CustomProperty \r\n\t                                                  WHERE TargetEntity = 'Orion.AlertConfigurationsCustomProperties'\r\n\t                                                  AND Table = 'AlertConfigurationsCustomProperties'");
        StringBuilder stringBuilder = new StringBuilder();
        if (dataTable1.Rows.Count > 0)
        {
          foreach (DataRow row in (InternalDataCollectionBase) dataTable1.Rows)
            stringBuilder.Append(", [CP]." + row[0].ToString());
        }
        string str = "SELECT AC.AlertID, AC.Name, AC.Description, AC.Severity {columnNames} \r\n\t                                     FROM Orion.AlertConfigurations AS [AC] \r\n\t                                     INNER JOIN Orion.AlertConfigurationsCustomProperties AS [CP] \r\n\t                                     ON AC.AlertID = CP.AlertID WHERE AC.AlertID = @alertID".Replace("{columnNames}", stringBuilder.ToString());
        dataTable2 = ((IInformationServiceProxy) iinformationServiceProxy2).Query(str, (IDictionary<string, object>) new Dictionary<string, object>()
        {
          {
            "alertID",
            (object) alertId
          }
        });
      }
      AlertScopeItemDetails alertScopeDetails = (AlertScopeItemDetails) null;
      if (dataTable2 != null && dataTable2.Rows.Count == 1)
      {
        alertScopeDetails = new AlertScopeItemDetails();
        alertScopeDetails.AlertId = alertId;
        alertScopeDetails.AlertName = dataTable2.Rows[0]["Name"].ToString();
        alertScopeDetails.Description = dataTable2.Rows[0]["Description"].ToString();
        alertScopeDetails.Severity = (AlertSeverity) int.Parse(dataTable2.Rows[0]["Severity"].ToString());
        alertScopeDetails.CustomProperties = new Dictionary<string, string>();
        foreach (DataRow row in (InternalDataCollectionBase) dataTable1.Rows)
        {
          string str = row[0].ToString();
          alertScopeDetails.CustomProperties.Add(str, dataTable2.Rows[0][str].ToString());
        }
      }
      return alertScopeDetails;
    }

    public bool UnacknowledgeAlerts(int[] alertObjectIds, string accountId)
    {
      ActiveAlertDAL activeAlertDal = new ActiveAlertDAL();
      IEnumerable<int> source = activeAlertDal.LimitAlertAckStateUpdateCandidates((IEnumerable<int>) alertObjectIds, false);
      List<IIndication> iindicationList1 = new List<IIndication>();
      iindicationList1.AddRange((IEnumerable<IIndication>) activeAlertDal.GetAlertUpdatedIndicationPropertiesByAlertObjectIds((IEnumerable<int>) alertObjectIds, accountId, string.Empty, DateTime.UtcNow, false));
      DataTable byAlertObjectIds = activeAlertDal.GetAlertResetOrUpdateIndicationPropertiesTableByAlertObjectIds((IEnumerable<int>) alertObjectIds);
      foreach (int alertObjectId in alertObjectIds)
      {
        DataRow[] dataRowArray = byAlertObjectIds.Select("AlertObjectID=" + (object) alertObjectId);
        PropertyBag propertyBag1 = new PropertyBag();
        if (dataRowArray.Length != 0)
        {
          ((Dictionary<string, object>) propertyBag1).Add("Acknowledged", dataRowArray[0]["Acknowledged"] != DBNull.Value ? (object) Convert.ToString(dataRowArray[0]["Acknowledged"]) : (object) "False");
          ((Dictionary<string, object>) propertyBag1).Add("AcknowledgedBy", dataRowArray[0]["AcknowledgedBy"] != DBNull.Value ? (object) Convert.ToString(dataRowArray[0]["AcknowledgedBy"]) : (object) string.Empty);
          ((Dictionary<string, object>) propertyBag1).Add("AcknowledgedDateTime", dataRowArray[0]["AcknowledgedDateTime"] != DBNull.Value ? (object) Convert.ToString(dataRowArray[0]["AcknowledgedDateTime"]) : (object) string.Empty);
        }
        List<IIndication> iindicationList2 = iindicationList1;
        PropertyBag propertyBag2 = new PropertyBag();
        ((Dictionary<string, object>) propertyBag2).Add("AlertObjectID", (object) alertObjectId);
        ((Dictionary<string, object>) propertyBag2).Add("Acknowledged", (object) "False");
        ((Dictionary<string, object>) propertyBag2).Add("AcknowledgedBy", (object) string.Empty);
        ((Dictionary<string, object>) propertyBag2).Add("AcknowledgedDateTime", (object) string.Empty);
        ((Dictionary<string, object>) propertyBag2).Add("PreviousProperties", (object) propertyBag1);
        ((Dictionary<string, object>) propertyBag2).Add("InstanceType", (object) "Orion.AlertActive");
        CommonIndication commonIndication = new CommonIndication((IndicationType) 2, propertyBag2);
        iindicationList2.Add((IIndication) commonIndication);
      }
      int num = ActiveAlertDAL.UnacknowledgeAlerts(source.ToArray<int>(), accountId) ? 1 : 0;
      if (num == 0)
        return num != 0;
      IndicationPublisher.CreateV3().ReportIndications((IEnumerable<IIndication>) iindicationList1);
      return num != 0;
    }

    public string GetAlertNote(int alertObjectId)
    {
      return new ActiveAlertDAL().GetAlertNote(alertObjectId);
    }

    public bool SetAlertNote(
      int alertObjectId,
      string accountId,
      string note,
      DateTime modificationDateTime,
      string previousNote)
    {
      ActiveAlertDAL activeAlertDal1 = new ActiveAlertDAL();
      int num = activeAlertDal1.SetAlertNote(alertObjectId, accountId, note, modificationDateTime) ? 1 : 0;
      if (num == 0)
        return num != 0;
      List<IIndication> iindicationList1 = new List<IIndication>();
      List<IIndication> iindicationList2 = iindicationList1;
      PropertyBag propertyBag1 = new PropertyBag();
      ((Dictionary<string, object>) propertyBag1).Add("AlertObjectID", (object) alertObjectId);
      ((Dictionary<string, object>) propertyBag1).Add("AlertNote", (object) note);
      PropertyBag propertyBag2 = new PropertyBag();
      ((Dictionary<string, object>) propertyBag2).Add("AlertNote", (object) previousNote);
      ((Dictionary<string, object>) propertyBag1).Add("PreviousProperties", (object) propertyBag2);
      ((Dictionary<string, object>) propertyBag1).Add("InstanceType", (object) "Orion.AlertObjects");
      CommonIndication commonIndication = new CommonIndication((IndicationType) 2, propertyBag1);
      iindicationList2.Add((IIndication) commonIndication);
      ActiveAlertDAL activeAlertDal2 = activeAlertDal1;
      List<int> alertObjectIds = new List<int>();
      alertObjectIds.Add(alertObjectId);
      string accountId1 = accountId;
      string notes = note;
      DateTime utcNow = DateTime.UtcNow;
      IEnumerable<AlertUpdatedIndication> byAlertObjectIds = activeAlertDal2.GetAlertUpdatedIndicationPropertiesByAlertObjectIds((IEnumerable<int>) alertObjectIds, accountId1, notes, utcNow, false);
      if (byAlertObjectIds.Any<AlertUpdatedIndication>())
      {
        PropertyBag instanceProperties = ((Indication) byAlertObjectIds.ElementAt<AlertUpdatedIndication>(0)).GetSourceInstanceProperties();
        if (((Dictionary<string, object>) instanceProperties).ContainsKey("Acknowledged"))
          ((Dictionary<string, object>) instanceProperties).Remove("Acknowledged");
        if (((Dictionary<string, object>) instanceProperties).ContainsKey("AcknowledgedBy"))
          ((Dictionary<string, object>) instanceProperties).Remove("AcknowledgedBy");
        if (((Dictionary<string, object>) instanceProperties).ContainsKey("AcknowledgedMethod"))
          ((Dictionary<string, object>) instanceProperties).Remove("AcknowledgedMethod");
        if (!((Dictionary<string, object>) instanceProperties).ContainsKey("Notes"))
          ((Dictionary<string, object>) instanceProperties).Add("Notes", (object) note);
        IIndication iindication = (IIndication) new CommonIndication((IndicationType) 10, instanceProperties);
        iindicationList1.Add(iindication);
      }
      IndicationPublisher.CreateV3().ReportIndications((IEnumerable<IIndication>) iindicationList1);
      return num != 0;
    }

    public bool SetAlertNote(
      int alertObjectId,
      string accountId,
      string note,
      DateTime modificationDateTime)
    {
      return this.SetAlertNote(alertObjectId, accountId, note, modificationDateTime, string.Empty);
    }

    public bool AppendAlertNote(
      int alertObjectId,
      string accountId,
      string note,
      DateTime modificationDateTime)
    {
      string alertNote = this.GetAlertNote(alertObjectId);
      note = string.IsNullOrWhiteSpace(alertNote) ? note : alertNote + Environment.NewLine + note;
      return this.SetAlertNote(alertObjectId, accountId, note, modificationDateTime, alertNote);
    }

    public AlertImportResult ImportAlert(
      string fileContent,
      string userName,
      bool generateNewGuid,
      bool importIfExists,
      bool importSmtpServer)
    {
      return this.ImportAlertConfiguration(fileContent, userName, generateNewGuid, importIfExists, importSmtpServer, false, string.Empty);
    }

    public AlertImportResult ImportAlertConfiguration(
      string fileContent,
      string user,
      bool generateNewGuid,
      bool importIfExists,
      bool importSmtpServer,
      bool stripSensitiveData,
      string protectionPassword)
    {
      try
      {
        ISwisConnectionProxyCreator creator = SwisConnectionProxyPool.GetCreator();
        AlertDefinition alertDefinition = new AlertImporter((IAlertMigrationPluginProvider) AlertMigrationPluginProvider.Create((ComposablePartCatalog) AppDomainCatalogSingleton.Instance), (IInformationServiceProxyCreator) creator, (INetObjectTypeSource) new CoreNetObjectTypeSource((ICoreBusinessLayer) this), generateNewGuid, importIfExists, false, false).ImportAlert(XElement.Parse(fileContent), (string) null, false, true, (ICoreBusinessLayer) this, (List<CannedAlertImportResult>) null, stripSensitiveData, protectionPassword);
        return new AlertImportResult()
        {
          AlertId = new int?(alertDefinition.AlertID.Value),
          Name = alertDefinition.Name,
          MigrationMessage = "Alert imported successfully"
        };
      }
      catch (CryptographicException ex)
      {
        return new AlertImportResult()
        {
          MigrationMessage = string.Format("Alert import failed with error: {0}", (object) ex.Message),
          IncorrectPasswordForDecryptSensitiveData = true
        };
      }
      catch (Exception ex)
      {
        return new AlertImportResult()
        {
          MigrationMessage = string.Format("Alert import failed with error: {0}", (object) ex.Message)
        };
      }
    }

    [Obsolete("Old alerting will be removed")]
    public bool RevertMigratedAlert(Guid alertRefId, bool enableInOldAlerting)
    {
      return AlertDAL.RevertMigratedAlert(alertRefId, enableInOldAlerting);
    }

    public string ExportAlert(int alertId) => new AlertExporter().ExportAlert(alertId);

    public string ExportAlertConfiguration(
      int alertId,
      bool stripSensitiveData,
      string protectionPassword)
    {
      return new AlertExporter().ExportAlert(alertId, stripSensitiveData, protectionPassword);
    }

    public AlertMigrationResult MigrateAdvancedAlertFromDB(string definitionIdGuid)
    {
      using (AlertsMigrator alertsMigrator = new AlertsMigrator())
        return alertsMigrator.MigrateAdvancedAlertFromDB(definitionIdGuid);
    }

    public AlertMigrationResult[] MigrateAllAdvancedAlertsFromDB()
    {
      using (AlertsMigrator alertsMigrator = new AlertsMigrator())
        return alertsMigrator.MigrateAllAdvancedAlertsFromDB(false);
    }

    public AlertMigrationResult[] MigrateAdvancedAlertFromXML(string xmlOldAlertDefinition)
    {
      using (AlertsMigrator alertsMigrator = new AlertsMigrator())
        return alertsMigrator.MigrateAdvancedAlertFromXML(xmlOldAlertDefinition);
    }

    public AlertMigrationResult MigrateBasicAlertFromDB(int alertId)
    {
      using (AlertsMigrator alertsMigrator = new AlertsMigrator())
        return alertsMigrator.MigrateBasicAlertFromDB(alertId);
    }

    public AlertMigrationResult[] MigrateAllBasicAlertsFromDB()
    {
      using (AlertsMigrator alertsMigrator = new AlertsMigrator())
        return alertsMigrator.MigrateAllBasicAlertsFromDB(false);
    }

    public AlertMigrationResult[] GetAlertMigrationResults(string migrationId)
    {
      return new AlertMigrationLogDAL().GetAlertMigrationResults(migrationId).ToArray<AlertMigrationResult>();
    }

    public CannedAlertImportResult[] GetCannedAlertImportResults(string importId)
    {
      return new ImportedCannedAlertDAL().GetCannedAlertImportResults(importId).ToArray<CannedAlertImportResult>();
    }

    public List<AuditActionTypeInfo> GetAuditingActionTypes()
    {
      AuditingDAL auditingDal = new AuditingDAL();
      auditingDal.LoadKeys();
      return auditingDal.GetAuditingActionTypes();
    }

    public DataTable GetAuditingTable(
      int maxRecords,
      int netObjectId,
      string netObjectType,
      int nodeId,
      string actionTypeIds,
      DateTime startTime,
      DateTime endTime)
    {
      return this.GetAuditingTableWithHtmlMessages(AuditingDAL.GetAuditingTable(maxRecords, netObjectId, netObjectType, nodeId, actionTypeIds, startTime, endTime));
    }

    private DataTable GetAuditingTableWithHtmlMessages(DataTable originTable)
    {
      DataTable withHtmlMessages = new DataTable()
      {
        TableName = "AuditingTableWithHtmlMessages"
      };
      withHtmlMessages.Columns.Add("DateTime", typeof (DateTime));
      withHtmlMessages.Columns.Add("AccountID", typeof (string));
      withHtmlMessages.Columns.Add("Message", typeof (string));
      if (originTable == null || originTable.Rows.Count == 0)
        return withHtmlMessages;
      foreach (IGrouping<int, DataRow> source in originTable.Rows.Cast<DataRow>().GroupBy<DataRow, int>((System.Func<DataRow, int>) (it => it.Field<int>("AuditEventID"))))
      {
        int actionTypeId = source.First<DataRow>().Field<int>("ActionTypeID");
        string account = source.First<DataRow>().Field<string>("AccountID");
        DateTime dateTime = source.First<DataRow>().Field<DateTime>("DateTime");
        Dictionary<string, string> dictionary = source.Select(it => new
        {
          Key = it.Field<string>("ArgsKey"),
          Value = it.Field<string>("ArgsValue")
        }).Where(it => it.Key != null).ToDictionary(it => it.Key, it => it.Value);
        string storedMessage = source.First<DataRow>().Field<string>("Message");
        withHtmlMessages.Rows.Add((object) dateTime, (object) account, (object) this.RetrieveHtmlMessage(actionTypeId, account, dictionary, storedMessage));
      }
      return withHtmlMessages;
    }

    private string RetrieveHtmlMessage(
      int actionTypeId,
      string account,
      Dictionary<string, string> args,
      string storedMessage)
    {
      AuditActionType typeFromActionId = this.auditingDal.GetActionTypeFromActionId(actionTypeId);
      IAuditing2 instancesOfActionType = this._auditPluginManager.GetAuditingInstancesOfActionType(typeFromActionId);
      return instancesOfActionType != null ? ((IAuditing) instancesOfActionType).GetHTMLMessage(new AuditDataContainer(typeFromActionId, args, account)) : storedMessage;
    }

    public DataTable GetAuditingTypesTable() => AuditingDAL.GetAuditingTypesTable();

    private static BlogItem DalToWfc(BlogItemDAL dal)
    {
      return dal == null ? (BlogItem) null : new BlogItem(dal.Id, dal.Title, dal.Description, dal.CreatedAt, dal.Ignored, dal.Url, dal.AcknowledgedAt, dal.AcknowledgedBy, dal.PostGuid, dal.PostId, dal.Owner, dal.PublicationDate, dal.CommentsUrl, dal.CommentsCount);
    }

    public BlogItem GetBlogNotificationItem(Guid blogId)
    {
      CoreBusinessLayerService.log.Debug((object) "Sending request for BlogItemDAL.GetBlogById.");
      try
      {
        return CoreBusinessLayerService.DalToWfc(BlogItemDAL.GetItemById(blogId));
      }
      catch (Exception ex)
      {
        CoreBusinessLayerService.log.Error((object) ("Error obtaining blog notification item: " + ex.ToString()));
        throw new Exception(string.Format(Resources.LIBCODE_JM0_25, (object) blogId));
      }
    }

    public List<BlogItem> GetBlogNotificationItems(int maxResultsCount, bool includeIgnored)
    {
      CoreBusinessLayerService.log.Debug((object) "Sending request for BlogItemDAL.GetItems.");
      try
      {
        List<BlogItem> notificationItems = new List<BlogItem>();
        foreach (BlogItemDAL dal in (IEnumerable<BlogItemDAL>) BlogItemDAL.GetItems(new BlogFilter(true, includeIgnored, maxResultsCount)))
          notificationItems.Add(CoreBusinessLayerService.DalToWfc(dal));
        return notificationItems;
      }
      catch (Exception ex)
      {
        CoreBusinessLayerService.log.Error((object) ("Error when obtaining blog notification items: " + ex.ToString()));
        throw new Exception(Resources.LIBCODE_JM0_26);
      }
    }

    public void ForceBlogUpdatesCheck()
    {
      CoreBusinessLayerService.log.Debug((object) "Sending request for CoreHelper.CheckOrionProductTeamBlog.");
      try
      {
        CoreHelper.CheckOrionProductTeamBlog();
      }
      catch (Exception ex)
      {
        CoreBusinessLayerService.log.Error((object) ("Error forcing blog notification items update: " + ex.ToString()));
        throw new Exception(Resources.LIBCODE_JM0_27);
      }
    }

    public BlogItem GetBlogNotificationItemForPost(Guid postGuid, long postId)
    {
      CoreBusinessLayerService.log.Debug((object) "Sending request for BlogItemDAL.GetBlogItemForPos.");
      try
      {
        return CoreBusinessLayerService.DalToWfc(BlogItemDAL.GetBlogItemForPost(postGuid, postId));
      }
      catch (Exception ex)
      {
        CoreBusinessLayerService.log.Error((object) ("Error obtaining blog notification item for post: " + ex.ToString()));
        throw new Exception(string.Format(Resources.LIBCODE_JM0_28, (object) postGuid, (object) postId));
      }
    }

    [Obsolete("removed", true)]
    public List<ServiceURI> GetAllVMwareServiceURIs()
    {
      throw new NotSupportedException(nameof (GetAllVMwareServiceURIs));
    }

    [Obsolete("removed", true)]
    public VMCredential GetVMwareCredential(long vmwareCredentialsID)
    {
      throw new NotSupportedException(nameof (GetVMwareCredential));
    }

    [Obsolete("removed", true)]
    public void InsertUpdateVMHostNode(VMHostNode node)
    {
      throw new NotSupportedException(nameof (InsertUpdateVMHostNode));
    }

    [Obsolete("removed", true)]
    public VMHostNode GetVMHostNode(int nodeId)
    {
      throw new NotSupportedException(nameof (GetVMHostNode));
    }

    [Obsolete("Core-Split cleanup. If you need this member please contact Core team", true)]
    public Dictionary<int, bool> RunNowGeolocation()
    {
      CoreBusinessLayerService.log.DebugFormat("Running job(s)", Array.Empty<object>());
      Dictionary<int, bool> dictionary = new Dictionary<int, bool>();
      string[] availableForGeolocation = WorldMapPointsDAL.GetEntitiesAvailableForGeolocation();
      int key = 1;
      foreach (string str1 in availableForGeolocation)
      {
        string str2;
        if (!WebSettingsDAL.TryGet(string.Format("{0}_GeolocationField", (object) str1), ref str2) || string.IsNullOrWhiteSpace(str2))
          return dictionary;
        ActionDefinition actionDefinition = new ActionDefinition();
        actionDefinition.ActionTypeID = "Geolocation";
        actionDefinition.Enabled = true;
        ActionProperties actionProperties = new ActionProperties();
        actionProperties.Add("StreetAddress", "Location");
        actionProperties.Add("Entity", str1);
        actionProperties.Add("MapQuestApiKey", WorldMapPointsDAL.GetMapQuestKey());
        actionDefinition.Properties = actionProperties;
        bool flag = this.ExecuteAction(actionDefinition, (ActionContextBase) new GeolocationActionContext()).Status == 1;
        if (!dictionary.Keys.Contains<int>(key))
          dictionary.Add(key, flag);
        else
          dictionary[key] = flag;
      }
      return dictionary;
    }

    public List<MacroPickerDefinition> GetMacroPickerDefinition(MacroContext context)
    {
      return new MacroParser().GetMacroPickerDefinition(context).ToList<MacroPickerDefinition>();
    }

    public string FormatMacroValue(string formatter, string value, string dataType)
    {
      return new MacroParser().FormatValue(formatter, value, dataType);
    }

    public NetObjectTypes GetNetObjectTypes() => NetObjectTypeMgr.GetNetObjectTypes();

    public Dictionary<string, string> GetNetObjectData() => NetObjectTypeMgr.GetNetObjectData();

    public TestJobResult TestSnmpCredentialsOnAgent(
      int nodeId,
      uint snmpAgentPort,
      SnmpCredentialsV2 credentials)
    {
      AgentInfo agentInfoByNode = this._agentInfoDal.GetAgentInfoByNode(nodeId);
      if (agentInfoByNode == null || agentInfoByNode.ConnectionStatus != 1)
      {
        CoreBusinessLayerService.log.WarnFormat("SNMP credential test could not start because agent for node {0} is not connected", (object) nodeId);
        return new TestJobResult()
        {
          Success = false,
          Message = Resources.TestErrorAgentNotConnected
        };
      }
      TimeSpan testJobTimeout = BusinessLayerSettings.Instance.TestJobTimeout;
      SnmpSettings snmpSettings = new SnmpSettings()
      {
        AgentPort = (int) snmpAgentPort,
        TargetIP = IPAddress.Loopback
      };
      string errorMessage;
      TestJobResult result = this.ExecuteJobAndGetResult<TestJobResult>(new JobDescription()
      {
        TypeName = "SolarWinds.Orion.Core.TestSnmpCredentialsJob",
        JobDetailConfiguration = SerializationHelper.ToJson((object) snmpSettings),
        JobNamespace = "orion",
        ResultTTL = testJobTimeout,
        Timeout = testJobTimeout,
        TargetNode = new HostAddress(IPAddress.Loopback.ToString(), (AddressType) 4),
        EndpointAddress = agentInfoByNode.AgentGuid.ToString(),
        SupportedRoles = (PackageType) 7
      }, (CredentialBase) credentials, JobResultDataFormatType.Json, "SNMP", out errorMessage);
      if (result.Success)
      {
        CoreBusinessLayerService.log.InfoFormat("SNMP credential test finished. Success: {0}, Message: {1}", (object) result.Success, (object) result.Message);
        return result;
      }
      return new TestJobResult()
      {
        Success = false,
        Message = errorMessage
      };
    }

    internal T ExecuteJobAndGetResult<T>(
      JobDescription jobDescription,
      CredentialBase jobCredential,
      JobResultDataFormatType resultDataFormat,
      string jobType,
      out string errorMessage)
      where T : TestJobResult, new()
    {
      this.GetCurrentServiceInstance().RouteJobToEngine(jobDescription);
      using (OneTimeJobRawResult timeJobRawResult = this._oneTimeJobManager.ExecuteJob(jobDescription, jobCredential))
      {
        errorMessage = timeJobRawResult.Error;
        if (!timeJobRawResult.Success)
        {
          CoreBusinessLayerService.log.WarnFormat(jobType + " credential test failed: " + timeJobRawResult.Error, Array.Empty<object>());
          string messageFromException = this.GetLocalizedErrorMessageFromException(timeJobRawResult.ExceptionFromJob);
          T result = new T();
          result.Success = false;
          result.Message = string.IsNullOrEmpty(messageFromException) ? errorMessage : messageFromException;
          return result;
        }
        try
        {
          if (resultDataFormat != JobResultDataFormatType.Xml)
            return SerializationHelper.Deserialize<T>(timeJobRawResult.JobResultStream);
          using (XmlTextReader xmlTextReader = new XmlTextReader(timeJobRawResult.JobResultStream))
          {
            xmlTextReader.Namespaces = false;
            return (T) new XmlSerializer(typeof (T)).Deserialize((XmlReader) xmlTextReader);
          }
        }
        catch (Exception ex)
        {
          CoreBusinessLayerService.log.Error((object) string.Format("Failed to deserialize {0} credential test job result: {1}", (object) jobType, (object) ex));
          T result = new T();
          result.Success = false;
          result.Message = this.GetLocalizedErrorMessageFromException(timeJobRawResult.ExceptionFromJob);
          return result;
        }
      }
    }

    private string GetLocalizedErrorMessageFromException(Exception exception)
    {
      return exception != null && exception is FaultException<JobEngineConnectionFault> ? Resources.LIBCODE_PS0_20 : string.Empty;
    }

    public void UpdateOrionFeatures()
    {
      this.ServiceContainer.GetService<OrionFeatureResolver>().Resolve();
    }

    public void UpdateOrionFeaturesForProvider(string provider)
    {
      this.ServiceContainer.GetService<OrionFeatureResolver>().Resolve(provider);
    }

    public void DeleteOrionServerByEngineId(int engineId)
    {
      new OrionServerDAL().DeleteOrionServerByEngineId(engineId);
    }

    public IEnumerable<SolarWinds.Orion.Core.Common.Models.Technology.Technology> GetTechnologyList()
    {
      return (IEnumerable<SolarWinds.Orion.Core.Common.Models.Technology.Technology>) TechnologyManager.Instance.TechnologyFactory.Items().Select<ITechnology, SolarWinds.Orion.Core.Common.Models.Technology.Technology>((System.Func<ITechnology, SolarWinds.Orion.Core.Common.Models.Technology.Technology>) (t => new SolarWinds.Orion.Core.Common.Models.Technology.Technology()
      {
        DisplayName = t.DisplayName,
        TargetEntity = t.TargetEntity,
        TechnologyID = t.TechnologyID
      })).ToList<SolarWinds.Orion.Core.Common.Models.Technology.Technology>();
    }

    public IEnumerable<TechnologyPolling> GetTechnologyPollingList()
    {
      return (IEnumerable<TechnologyPolling>) TechnologyManager.Instance.TechnologyPollingFactory.Items().Select<ITechnologyPolling, TechnologyPolling>((System.Func<ITechnologyPolling, TechnologyPolling>) (t => new TechnologyPolling()
      {
        DisplayName = t.DisplayName,
        TechnologyID = t.TechnologyID,
        Priority = t.Priority,
        TechnologyPollingID = t.TechnologyPollingID
      })).ToList<TechnologyPolling>();
    }

    public int[] EnableDisableTechnologyPollingAssignmentOnNetObjects(
      string technologyPollingID,
      bool enable,
      int[] netObjectIDs)
    {
      return TechnologyManager.Instance.TechnologyPollingFactory.EnableDisableAssignments(technologyPollingID, enable, netObjectIDs);
    }

    public int[] EnableDisableTechnologyPollingAssignment(string technologyPollingID, bool enable)
    {
      return TechnologyManager.Instance.TechnologyPollingFactory.EnableDisableAssignments(technologyPollingID, enable);
    }

    public IEnumerable<TechnologyPollingAssignment> GetTechnologyPollingAssignmentsOnNetObjects(
      string technologyPollingID,
      int[] netObjectIDs)
    {
      return (IEnumerable<TechnologyPollingAssignment>) TechnologyManager.Instance.TechnologyPollingFactory.GetAssignments(technologyPollingID, netObjectIDs).ToList<TechnologyPollingAssignment>();
    }

    public IEnumerable<TechnologyPollingAssignment> GetTechnologyPollingAssignments(
      string technologyPollingID)
    {
      return (IEnumerable<TechnologyPollingAssignment>) TechnologyManager.Instance.TechnologyPollingFactory.GetAssignments(technologyPollingID).ToList<TechnologyPollingAssignment>();
    }

    public ICollection<TechnologyPollingAssignment> GetTechnologyPollingAssignmentsFiltered(
      string[] technologyPollingIDsFilter,
      int[] netObjectIDsFilter,
      string[] targetEntitiesFilter,
      bool[] enabledFilter)
    {
      return (ICollection<TechnologyPollingAssignment>) TechnologyManager.Instance.TechnologyPollingFactory.GetAssignmentsFiltered(technologyPollingIDsFilter, netObjectIDsFilter, targetEntitiesFilter, enabledFilter).ToList<TechnologyPollingAssignment>();
    }

    public List<TimeFrame> GetAllTimeFrames(string timeFrameName = null)
    {
      return TimeFramesDAL.GetAllTimeFrames(timeFrameName);
    }

    public List<TimeFrame> GetCoreTimeFrames(string timeFrameName = null)
    {
      return TimeFramesDAL.GetCoreTimeFrames(timeFrameName);
    }

    public IList<PredefinedCustomProperty> GetPredefinedCustomProperties(
      string targetEntity,
      bool includeAdvanced)
    {
      return CustomPropertyDAL.GetPredefinedPropertiesForTable(targetEntity, includeAdvanced);
    }

    public bool IsSystemProperty(string tableName, string propName)
    {
      return CustomPropertyDAL.IsSystemProperty(tableName, propName);
    }

    public bool IsReservedWord(string propName) => CustomPropertyDAL.IsReservedWord(propName);

    public bool IsColumnExists(string table, string name)
    {
      return CustomPropertyDAL.IsColumnExists(table, name);
    }

    public List<string> GetSystemPropertyNamesFromDb(string table)
    {
      return CustomPropertyDAL.GetSystemPropertyNamesFromDb(table);
    }

    [Obsolete("Use IDependencyProxy class", true)]
    public int DeleteDependencies(List<int> listIds)
    {
      CoreBusinessLayerService.log.Error((object) "Unexpected call to deprecated method DeleteDependencies.");
      throw new InvalidOperationException("Use IDependencyProxy class");
    }

    [Obsolete("Use IDependencyProxy class", true)]
    public Dependency GetDependency(int id)
    {
      CoreBusinessLayerService.log.Error((object) "Unexpected call to deprecated method GetDependency");
      throw new InvalidOperationException("Use IDependencyProxy class");
    }

    [Obsolete("Use IDependencyProxy class", true)]
    public void UpdateDependency(Dependency dependency)
    {
      CoreBusinessLayerService.log.Error((object) "Unexpected call to deprecated method UpdateDependency");
      throw new InvalidOperationException("Use IDependencyProxy class");
    }

    [Obsolete("Use DeleteOrionDiscoveryProfile", true)]
    public void DeleteDiscoveryProfile(int profileID)
    {
    }

    public DiscoveryConfiguration GetDiscoveryConfigurationByProfile(int profileID)
    {
      return DiscoveryDatabase.GetDiscoveryConfiguration(profileID);
    }

    public bool TryConnectionWithJobScheduler(out string errorMessage)
    {
      try
      {
        using (IJobSchedulerHelper instance = JobScheduler.GetInstance())
        {
          ((IJobScheduler) instance).PolicyExists("Nothing");
          errorMessage = string.Empty;
          return true;
        }
      }
      catch (Exception ex)
      {
        errorMessage = string.Format("{0}: {1}", (object) ex.GetType().Name, (object) ex.Message);
        return false;
      }
    }

    public IJobFactory JobFactory
    {
      get => this._jobFactory ?? (this._jobFactory = (IJobFactory) new OrionDiscoveryJobFactory());
      set => this._jobFactory = value;
    }

    public IPersistentDiscoveryCache PersistentDiscoveryCache
    {
      get
      {
        return this._persistentDiscoveryCache ?? (this._persistentDiscoveryCache = (IPersistentDiscoveryCache) new SolarWinds.Orion.Core.BusinessLayer.Discovery.DiscoveryCache.PersistentDiscoveryCache());
      }
      set => this._persistentDiscoveryCache = value;
    }

    public Guid CreateOneTimeAgentDiscoveryJob(
      int nodeId,
      int engineId,
      int? profileId,
      List<Credential> credentials)
    {
      return this.CreateOneTimeDiscoveryJobWithCache(new OneTimeDiscoveryJobConfiguration()
      {
        NodeId = new int?(nodeId),
        IpAddress = IPAddress.Loopback,
        EngineId = engineId,
        Credentials = credentials,
        ProfileId = profileId
      }, (DiscoveryCacheConfiguration) 2);
    }

    public Guid CreateOneTimeDiscoveryJob(
      int? nodeId,
      uint? snmpPort,
      SNMPVersion? preferredSnmpVersion,
      IPAddress ip,
      int engineId,
      List<Credential> credentials)
    {
      return this.CreateOneTimeDiscoveryJobWithCache(new OneTimeDiscoveryJobConfiguration()
      {
        NodeId = nodeId,
        SnmpPort = snmpPort,
        PreferredSnmpVersion = preferredSnmpVersion,
        IpAddress = ip,
        EngineId = engineId,
        Credentials = credentials
      }, (DiscoveryCacheConfiguration) 2);
    }

    public Guid CreateOneTimeDiscoveryJobWithCache(
      int? nodeId,
      uint? snmpPort,
      SNMPVersion? preferredSnmpVersion,
      IPAddress ip,
      int engineId,
      List<Credential> credentials,
      DiscoveryCacheConfiguration cacheConfiguration)
    {
      return this.CreateOneTimeDiscoveryJobWithCache(new OneTimeDiscoveryJobConfiguration()
      {
        NodeId = nodeId,
        SnmpPort = snmpPort,
        PreferredSnmpVersion = preferredSnmpVersion,
        IpAddress = ip,
        EngineId = engineId,
        Credentials = credentials
      }, cacheConfiguration);
    }

    public Guid CreateOneTimeDiscoveryJobWithCache(
      OneTimeDiscoveryJobConfiguration jobConfiguration,
      DiscoveryCacheConfiguration cacheConfiguration)
    {
      CoreBusinessLayerService.log.DebugFormat("Creating one shot discovery job. Caching policy is {0}", (object) cacheConfiguration);
      if (jobConfiguration.NodeId.HasValue && cacheConfiguration == 2)
      {
        CoreBusinessLayerService.log.DebugFormat("Scanning cache for discovery for Node {0}", (object) jobConfiguration.NodeId);
        DiscoveryResultItem resultForNode = this.PersistentDiscoveryCache.GetResultForNode(jobConfiguration.NodeId.Value);
        if (resultForNode != null)
        {
          DiscoveryResultCache.Instance.AddOrReplaceResult(resultForNode);
          return resultForNode.JobId;
        }
      }
      else
        CoreBusinessLayerService.log.DebugFormat("Bypassing discovery cache. ", Array.Empty<object>());
      DiscoveryConfiguration discoveryConfiguration = new DiscoveryConfiguration();
      ((DiscoveryConfigurationBase) discoveryConfiguration).ProfileId = jobConfiguration.ProfileId;
      ((DiscoveryConfigurationBase) discoveryConfiguration).EngineId = jobConfiguration.EngineId;
      discoveryConfiguration.HopCount = 0;
      discoveryConfiguration.SearchTimeout = DiscoverySettings.DefaultSearchTimeout;
      discoveryConfiguration.SnmpTimeout = TimeSpan.FromMilliseconds((double) this._settingsDal.GetCurrentInt("SWNetPerfMon-Settings-SNMP Timeout", 2500));
      discoveryConfiguration.SnmpRetries = this._settingsDal.GetCurrentInt("SWNetPerfMon-Settings-SNMP Retries", 2);
      discoveryConfiguration.DefaultProbes = jobConfiguration.DefaultProbes;
      discoveryConfiguration.TagFilter = jobConfiguration.TagFilter;
      discoveryConfiguration.DisableICMP = jobConfiguration.DisableIcmp;
      DiscoveryConfiguration configuration = discoveryConfiguration;
      DiscoveryPollingEngineType? pollingEngineType = OrionDiscoveryJobFactory.GetDiscoveryPollingEngineType(jobConfiguration.EngineId, this._engineDal);
      SolarWinds.Orion.Core.Common.Models.Node node = (SolarWinds.Orion.Core.Common.Models.Node) null;
      if (jobConfiguration.NodeId.HasValue)
      {
        node = this._nodeBlDal.GetNode(jobConfiguration.NodeId.Value);
        if (node == null)
          CoreBusinessLayerService.log.ErrorFormat("Unable to get node {0}", (object) jobConfiguration.NodeId.Value);
      }
      int? nodeId;
      if (jobConfiguration.SnmpPort.HasValue)
        configuration.SnmpPort = jobConfiguration.SnmpPort.Value;
      else if (node != null)
      {
        configuration.SnmpPort = node.SNMPPort;
      }
      else
      {
        configuration.SnmpPort = 161U;
        Log log = CoreBusinessLayerService.log;
        nodeId = jobConfiguration.NodeId;
        // ISSUE: variable of a boxed type
        __Boxed<int> local = (System.ValueType) (nodeId ?? -1);
        IPAddress ipAddress = jobConfiguration.IpAddress;
        log.InfoFormat("Unable to determine SNMP port node {0} IP {1}, using default 161", (object) local, (object) ipAddress);
      }
      if (jobConfiguration.PreferredSnmpVersion.HasValue)
        configuration.PreferredSnmpVersion = jobConfiguration.PreferredSnmpVersion.Value;
      else if (node != null)
      {
        configuration.PreferredSnmpVersion = (SNMPVersion) node.SNMPVersion;
      }
      else
      {
        configuration.PreferredSnmpVersion = (SNMPVersion) 2;
        Log log = CoreBusinessLayerService.log;
        nodeId = jobConfiguration.NodeId;
        // ISSUE: variable of a boxed type
        __Boxed<int> local = (System.ValueType) (nodeId ?? -1);
        IPAddress ipAddress = jobConfiguration.IpAddress;
        log.InfoFormat("Unable to determine preffered SNMP version node {0} IP {1}, using default v2c", (object) local, (object) ipAddress);
      }
      List<Credential> credentials = jobConfiguration.Credentials ?? new List<Credential>();
      AgentInfo updateConfiguration = this.TryGetAgentInfoAndUpdateConfiguration(node, credentials, configuration);
      List<string> agentPlugins = new List<string>();
      bool flag = RegistrySettings.IsFreePoller();
      List<DiscoveryPluginInfo> discoveryPluginInfos = DiscoveryPluginFactory.GetDiscoveryPluginInfos();
      IList<IDiscoveryPlugin> discoveryPlugins = DiscoveryHelper.GetOrderedDiscoveryPlugins();
      IDictionary<IDiscoveryPlugin, DiscoveryPluginInfo> pairsPluginAndInfo = DiscoveryPluginHelper.CreatePairsPluginAndInfo((IEnumerable<IDiscoveryPlugin>) discoveryPlugins, (IEnumerable<DiscoveryPluginInfo>) discoveryPluginInfos);
      foreach (IDiscoveryPlugin plugin in (IEnumerable<IDiscoveryPlugin>) discoveryPlugins)
      {
        if (flag && !(plugin is ISupportFreeEngine))
          CoreBusinessLayerService.log.DebugFormat("Discovery plugin {0} is not supported on FPE machine", (object) plugin.GetType().FullName);
        else if (!(plugin is IOneTimeJobSupport ioneTimeJobSupport))
        {
          CoreBusinessLayerService.log.DebugFormat("N/A one time job for {0}", (object) plugin);
        }
        else
        {
          if (jobConfiguration.TagFilter != null && jobConfiguration.TagFilter.Any<string>())
          {
            if (!(plugin is IDiscoveryPluginTags idiscoveryPluginTags))
            {
              CoreBusinessLayerService.log.DebugFormat("Discovery job for tags requested, however plugin {0} doesn't support tags, skipping.", (object) plugin);
              continue;
            }
            if (!jobConfiguration.TagFilter.Intersect<string>(idiscoveryPluginTags.Tags ?? Enumerable.Empty<string>(), (IEqualityComparer<string>) StringComparer.InvariantCultureIgnoreCase).Any<string>())
            {
              CoreBusinessLayerService.log.DebugFormat("Discovery job for tags [{0}], however plugin {1} doesn't support any of the tags requested, skipping.", (object) string.Join(",", (IEnumerable<string>) jobConfiguration.TagFilter), (object) plugin);
              continue;
            }
          }
          if (updateConfiguration == null || this.DoesPluginSupportAgent(plugin, updateConfiguration, agentPlugins))
          {
            if (pollingEngineType.HasValue && !OrionDiscoveryJobFactory.IsDiscoveryPluginSupportedForDiscoveryPollingEngineType(plugin, pollingEngineType.Value, pairsPluginAndInfo))
            {
              if (CoreBusinessLayerService.log.IsDebugEnabled)
                CoreBusinessLayerService.log.DebugFormat(string.Format("Plugin {0} is not supported for polling engine {1} of type {2}", (object) plugin.GetType().FullName, (object) configuration.EngineID, (object) pollingEngineType.Value), Array.Empty<object>());
            }
            else
            {
              DiscoveryPluginConfigurationBase jobConfiguration1 = ioneTimeJobSupport.GetOneTimeJobConfiguration(jobConfiguration.NodeId, jobConfiguration.IpAddress, credentials);
              ((DiscoveryConfigurationBase) configuration).AddDiscoveryPluginConfiguration(jobConfiguration1);
              CoreBusinessLayerService.log.DebugFormat("added one time job for {0}", (object) plugin);
            }
          }
        }
      }
      configuration.AgentPlugins = agentPlugins.ToArray();
      ScheduledJob discoveryJob = this.JobFactory.CreateDiscoveryJob(configuration);
      if (discoveryJob == null)
      {
        CoreBusinessLayerService.log.WarnFormat("Cannot create Discovery Job for NodeID {0}", (object) jobConfiguration.NodeId);
        return Guid.Empty;
      }
      Guid localEngine = this.JobFactory.SubmitScheduledJobToLocalEngine(Guid.Empty, discoveryJob, true);
      CoreBusinessLayerService.log.DebugFormat("Adding one time job with ID {0} into result cache", (object) localEngine);
      DiscoveryResultCache.Instance.AddOrReplaceResult(new DiscoveryResultItem(localEngine, jobConfiguration.NodeId, cacheConfiguration));
      return localEngine;
    }

    private AgentInfo TryGetAgentInfoAndUpdateConfiguration(
      SolarWinds.Orion.Core.Common.Models.Node node,
      List<Credential> credentials,
      DiscoveryConfiguration configuration)
    {
      AgentInfo nodeOrCredentials = this.TryGetAgentInfoFromNodeOrCredentials(node, credentials);
      if (nodeOrCredentials != null)
      {
        this.EnsureDiscoveryPluginsOnAgent(node, credentials, ref nodeOrCredentials);
        configuration.AgentAddress = nodeOrCredentials.AgentGuid.ToString();
        configuration.IsAgentJob = true;
        configuration.UseJsonFormat = nodeOrCredentials.UseJsonFormat;
      }
      return nodeOrCredentials;
    }

    private void EnsureDiscoveryPluginsOnAgent(
      SolarWinds.Orion.Core.Common.Models.Node node,
      List<Credential> credentials,
      ref AgentInfo agentInfo)
    {
      try
      {
        foreach (IAgentPluginJobSupport pluginJobSupport in DiscoveryHelper.GetOrderedDiscoveryPlugins().OfType<IOneTimeJobSupport>().OfType<IAgentPluginJobSupport>())
        {
          IAgentPluginJobSupport plugin = pluginJobSupport;
          AgentPluginInfo agentPluginInfo = agentInfo.Plugins.SingleOrDefault<AgentPluginInfo>((System.Func<AgentPluginInfo, bool>) (ap => ap.PluginId == plugin.PluginId));
          if (agentPluginInfo == null || !AgentInfo.PluginDeploymentFinishedStatuses.Contains(agentPluginInfo.Status))
          {
            CoreBusinessLayerService.log.DebugFormat("Found plugin '{0}' that is required for agent discovery but is missing on agent {1} ({2}) NodeId {3}", new object[4]
            {
              (object) plugin.PluginId,
              (object) agentInfo.HostName,
              (object) agentInfo.IPAddress,
              (object) agentInfo.NodeID
            });
            Task<AgentDeploymentStatus> task = this.DeployAgentDiscoveryPluginsAsync(agentInfo.AgentId);
            TimeSpan deploymentTimeLimit = BusinessLayerSettings.Instance.AgentDiscoveryPluginsDeploymentTimeLimit;
            if (!task.Wait(deploymentTimeLimit))
              CoreBusinessLayerService.log.WarnFormat("Plugin deployment on agent {0} ({1}) NodeId {2} hasn't finished in {3}.", new object[4]
              {
                (object) agentInfo.HostName,
                (object) agentInfo.IPAddress,
                (object) agentInfo.NodeID,
                (object) deploymentTimeLimit
              });
            else if (task.Result == 1)
              CoreBusinessLayerService.log.DebugFormat("Plugin deployment on agent {0} ({1}) NodeId {2} finished successfuly.", (object) agentInfo.HostName, (object) agentInfo.IPAddress, (object) agentInfo.NodeID);
            else
              CoreBusinessLayerService.log.WarnFormat("Plugin deployment on agent {0} ({1}) NodeId {2} finished with status {3}.", new object[4]
              {
                (object) agentInfo.HostName,
                (object) agentInfo.IPAddress,
                (object) agentInfo.NodeID,
                (object) task.Result
              });
            agentInfo = this.TryGetAgentInfoFromNodeOrCredentials(node, credentials);
            break;
          }
        }
      }
      catch (Exception ex)
      {
        CoreBusinessLayerService.log.ErrorFormat("Error during EnsureDiscoveryPluginsOnAgent for agent {0} ({1}) NodeId {2}. {3}", new object[4]
        {
          (object) agentInfo.HostName,
          (object) agentInfo.IPAddress,
          (object) agentInfo.NodeID,
          (object) ex
        });
      }
    }

    private AgentInfo TryGetAgentInfoFromNodeOrCredentials(SolarWinds.Orion.Core.Common.Models.Node node, List<Credential> credentials)
    {
      if (credentials == null)
        throw new ArgumentNullException(nameof (credentials));
      AgentInfo nodeOrCredentials = (AgentInfo) null;
      AgentManagementCredential managementCredential1 = credentials.OfType<AgentManagementCredential>().SingleOrDefault<AgentManagementCredential>();
      if (managementCredential1 != null)
      {
        nodeOrCredentials = this._agentInfoDal.GetAgentInfo(managementCredential1.AgentId);
        if (nodeOrCredentials == null)
          throw new InvalidOperationException(string.Format("No AgentManagement record found for AgentID {0}", (object) managementCredential1.AgentId));
      }
      if (nodeOrCredentials == null && node != null && node.NodeSubType == 4)
      {
        nodeOrCredentials = this._agentInfoDal.GetAgentInfoByNode(node.ID);
        if (nodeOrCredentials == null)
          throw new InvalidOperationException(string.Format("No AgentManagement record found for NodeID {0}", (object) node.ID));
        AgentManagementCredential managementCredential2 = new AgentManagementCredential()
        {
          AgentId = nodeOrCredentials.AgentId,
          AgentGuid = nodeOrCredentials.AgentGuid,
          Plugins = nodeOrCredentials.Plugins.Select<AgentPluginInfo, string>((System.Func<AgentPluginInfo, string>) (p => p.PluginId)).ToArray<string>()
        };
        credentials.Add((Credential) managementCredential2);
      }
      return nodeOrCredentials;
    }

    private bool DoesPluginSupportAgent(
      IDiscoveryPlugin plugin,
      AgentInfo agentInfo,
      List<string> agentPlugins)
    {
      IAgentPluginJobSupport agentSupport = plugin as IAgentPluginJobSupport;
      if (agentSupport == null)
      {
        CoreBusinessLayerService.log.DebugFormat("Agent discovery plugin jobs not supported for {0} on agent {1} ({2}) NodeId {3}", new object[4]
        {
          (object) plugin,
          (object) agentInfo.HostName,
          (object) agentInfo.IPAddress,
          (object) agentInfo.NodeID
        });
        return false;
      }
      if (agentPlugins.Contains(agentSupport.PluginId))
        return true;
      AgentPluginInfo agentPluginInfo = agentInfo.Plugins.SingleOrDefault<AgentPluginInfo>((System.Func<AgentPluginInfo, bool>) (ap => ap.PluginId == agentSupport.PluginId));
      if (agentPluginInfo == null || agentPluginInfo.Status != 1)
      {
        CoreBusinessLayerService.log.WarnFormat("Agent plugin {0} on agent {1} ({2}) NodeId {3} not deployed for discovery. Plugin status: {4}. ", new object[5]
        {
          (object) agentSupport.PluginId,
          (object) agentInfo.HostName,
          (object) agentInfo.IPAddress,
          (object) agentInfo.NodeID,
          (object) (agentPluginInfo != null ? agentPluginInfo.Status : 0)
        });
        return false;
      }
      agentPlugins.Add(agentPluginInfo.PluginId);
      return true;
    }

    public CreateDiscoveryJobResult CreateOrionDiscoveryJob(int profileID, bool executeImmediately)
    {
      CoreBusinessLayerService.log.DebugFormat("Creating discovery job for profile {0} where executeImmediately is {1}.", (object) profileID, (object) executeImmediately);
      DiscoveryConfiguration discoveryConfiguration = this.ServiceContainer.GetService<IDiscoveryDAL>().GetDiscoveryConfiguration(profileID);
      if (discoveryConfiguration == null)
        throw new ArgumentNullException("configuration");
      DiscoveryProfileEntry profileById = DiscoveryProfileEntry.GetProfileByID(discoveryConfiguration.ProfileID.Value);
      if (profileById == null)
        throw new ArgumentNullException("profile");
      if (discoveryConfiguration.Status.Status == 1 && discoveryConfiguration.JobID != Guid.Empty)
        return (CreateDiscoveryJobResult) 2;
      if (profileById.JobID != Guid.Empty)
      {
        CoreBusinessLayerService.log.DebugFormat("Deleting old job for profile {0}.", (object) profileID);
        if (this.JobFactory.DeleteJob(profileById.JobID))
          profileById.JobID = Guid.Empty;
        else
          throw new CoreBusinessLayerService.DicoveryDeletingJobError(Resources.DiscoveryBL_DicoveryDeletingJobError, new object[1]
          {
            (object) discoveryConfiguration.JobID
          });
      }
      ScheduledJob discoveryJob = this.JobFactory.CreateDiscoveryJob(discoveryConfiguration);
      if (discoveryJob == null)
        return (CreateDiscoveryJobResult) 1;
      if (!executeImmediately)
      {
        if (discoveryConfiguration.CronSchedule != null)
          profileById.Status = new DiscoveryComplexStatus((DiscoveryStatus) 5, string.Empty);
        else if (discoveryConfiguration.ScheduleRunAtTime != DateTime.MinValue || discoveryConfiguration.ScheduleRunFrequency != TimeSpan.Zero)
        {
          DateTime dateTime = DateTime.Now;
          dateTime = dateTime.ToUniversalTime();
          int int32_1 = Convert.ToInt32(dateTime.TimeOfDay.TotalMinutes);
          int minutes = 0;
          DateTime scheduleRunAtTime = profileById.ScheduleRunAtTime;
          TimeSpan timeSpan;
          if (!scheduleRunAtTime.Equals(DateTime.MinValue))
          {
            scheduleRunAtTime = profileById.ScheduleRunAtTime;
            timeSpan = scheduleRunAtTime.TimeOfDay;
            int int32_2 = Convert.ToInt32(timeSpan.TotalMinutes);
            minutes = int32_1 >= int32_2 ? 1440 - (int32_1 - int32_2) : int32_2 - int32_1;
          }
          timeSpan = discoveryConfiguration.ScheduleRunFrequency;
          if (!timeSpan.Equals(TimeSpan.Zero))
            minutes = profileById.ScheduleRunFrequency;
          discoveryJob.InitialWait = new TimeSpan(0, minutes, 0);
          profileById.Status = new DiscoveryComplexStatus((DiscoveryStatus) 5, string.Empty);
        }
        else
          profileById.Status = new DiscoveryComplexStatus((DiscoveryStatus) 4, string.Empty);
      }
      else
        profileById.Status = new DiscoveryComplexStatus((DiscoveryStatus) 1, string.Empty);
      if (profileById.Status.Status != 4)
      {
        CoreBusinessLayerService.log.DebugFormat("Submiting job for profile {0}.", (object) profileID);
        Guid localEngine;
        try
        {
          localEngine = this.JobFactory.SubmitScheduledJobToLocalEngine(Guid.Empty, discoveryJob, executeImmediately);
        }
        catch (FaultException ex)
        {
          CoreBusinessLayerService.log.Error((object) string.Format("Failed to create job for scheduled discovery profile {0}, rescheduler will keep trying", (object) profileID), (Exception) ex);
          this.parent.RunRescheduleEngineDiscoveryJobsTask(profileById.EngineID);
          throw;
        }
        profileById.JobID = localEngine;
      }
      else
      {
        CoreBusinessLayerService.log.DebugFormat("No job for profile {0} will be created.", (object) profileID);
        profileById.JobID = Guid.Empty;
      }
      CoreBusinessLayerService.log.DebugFormat("Updating profile {0}.", (object) profileID);
      profileById.Update();
      CoreBusinessLayerService.log.DebugFormat("Job for profile {0} created.", (object) profileID);
      return (CreateDiscoveryJobResult) 1;
    }

    public OrionDiscoveryJobProgressInfo GetOrionDiscoveryJobProgress(int profileID)
    {
      OrionDiscoveryJobProgressInfo discoveryJobProgress = OrionDiscoveryJobSchedulerEventsService.GetProgressInfo(profileID);
      if (discoveryJobProgress == null)
      {
        discoveryJobProgress = new OrionDiscoveryJobProgressInfo();
        DiscoveryProfileEntry profileById = DiscoveryProfileEntry.GetProfileByID(profileID);
        discoveryJobProgress.Status = profileById.Status;
        discoveryJobProgress.Starting = true;
        discoveryJobProgress.IsAutoImport = profileById.IsAutoImport;
      }
      if (discoveryJobProgress.Status.Status == 3)
      {
        CoreBusinessLayerService.log.WarnFormat("GetOrionDiscoveryJobProgress(): Error status on profile Id {0}", (object) profileID);
        throw new Exception("Error state received from discovery: " + discoveryJobProgress.ToString());
      }
      return discoveryJobProgress;
    }

    [Obsolete("Method from old discovery", true)]
    public void CancelDiscovery(int profileID)
    {
    }

    public List<DiscoveryResult> GetDiscoveryResultsList(
      DiscoveryNodeStatus status,
      DiscoveryResultsFilterType filterType,
      object filterValue,
      bool selectOnlyTopX,
      out bool thereIsMoreNodes,
      bool loadInterfacesAndVolumes)
    {
      CoreBusinessLayerService.log.DebugFormat("Sending request for results to DAL for status: {0}, filter type: {1}, filter: {2}.", (object) status, (object) filterType, filterValue == null ? (object) "null" : filterValue);
      List<DiscoveryResult> discoveryResultsList1 = DiscoveryDatabase.GetDiscoveryResultsList(status, filterType, filterValue, selectOnlyTopX, ref thereIsMoreNodes, loadInterfacesAndVolumes);
      CoreBusinessLayerService.log.DebugFormat("Results recieved from DAL for status: {0}, filter type: {1}, filter: {2}.", (object) status, (object) filterType, filterValue == null ? (object) "null" : filterValue);
      if (filterType == 2)
      {
        int profileId = Convert.ToInt32(filterValue);
        if (!discoveryResultsList1.Any<DiscoveryResult>((System.Func<DiscoveryResult, bool>) (item => ((DiscoveryResultBase) item).ProfileID == profileId)))
        {
          DiscoveryResult discoveryResult = new DiscoveryResult(profileId);
          discoveryResultsList1.Add(discoveryResult);
        }
      }
      CoreBusinessLayerService.log.DebugFormat("Converting old discovery result to new one.", Array.Empty<object>());
      List<DiscoveryResult> discoveryResultsList2 = this.ConvertScheduledDiscoveryResults(discoveryResultsList1);
      CoreBusinessLayerService.log.DebugFormat("Converting old discovery result to new one finished.", Array.Empty<object>());
      CoreBusinessLayerService.log.DebugFormat("Sending list of results back for status: {0}, filter type: {1}, filter: {2}.", (object) status, (object) filterType, filterValue == null ? (object) "null" : filterValue);
      return discoveryResultsList2;
    }

    public DiscoveryNode GetVolumesAndInterfacesForDiscoveryNode(DiscoveryNode discoveryNode)
    {
      CoreBusinessLayerService.log.DebugFormat("Sending request for load interfaces and volumes to BL for nodeID: {0}", (object) discoveryNode.NodeID);
      IEnumerable<IScheduledDiscoveryImport> ischeduledDiscoveryImports = DiscoveryHelper.GetOrderedDiscoveryPlugins().OfType<IScheduledDiscoveryImport>();
      DiscoveryDatabase.LoadInterfacesAndVolumesForNode(discoveryNode, ischeduledDiscoveryImports);
      CoreBusinessLayerService.log.DebugFormat("Request received for load interfaces and volumes to BL for nodeID: {0}", (object) discoveryNode.NodeID);
      return discoveryNode;
    }

    public int GetCountOfDiscoveryResults(DiscoveryNodeStatus status)
    {
      return DiscoveryNodeEntry.GetCountOfNodes(status);
    }

    public List<DateTime> GetDiscoveryResultListOfDates(DiscoveryNodeStatus status)
    {
      return DiscoveryNodeEntry.GetListOfDatesByStatus(status);
    }

    public List<int> GetDiscoveryResultListOfProfiles(DiscoveryNodeStatus status)
    {
      return DiscoveryNodeEntry.GetListOfProfilesByStatus(status);
    }

    public List<string> GetDiscoveryResultListOfMachineTypes(DiscoveryNodeStatus status)
    {
      return DiscoveryNodeEntry.GetListOfMachineTypesByStatus(status);
    }

    public void DeleteDiscoveryResultsByProfile(int profileID)
    {
      DiscoveryDatabase.DeleteResultsByProfile(profileID);
    }

    public DiscoveredObjectTreeWcfWrapper GetOneTimeJobResult(Guid jobId)
    {
      DiscoveryResultItem result = (DiscoveryResultItem) null;
      if (!DiscoveryResultCache.Instance.TryGetResultItem(jobId, ref result) || result == null || result.ResultTree == null)
        return (DiscoveredObjectTreeWcfWrapper) null;
      CoreBusinessLayerService.log.DebugFormat("Recieved one time job {0} result from cache", (object) jobId);
      if (result.CacheConfiguration != null && result.nodeId.HasValue)
      {
        CoreBusinessLayerService.log.DebugFormat("Storing the result into cache", Array.Empty<object>());
        this.PersistentDiscoveryCache.StoreResultForNode(result.nodeId.Value, result);
      }
      if (result.nodeId.HasValue && result.isCached)
      {
        foreach (IOneTimeJobSupport ioneTimeJobSupport in DiscoveryHelper.GetOrderedDiscoveryPlugins().OfType<IOneTimeJobSupport>())
        {
          try
          {
            ioneTimeJobSupport.GetDiscoveredResourcesManagedStatus(result.ResultTree, result.nodeId.Value);
          }
          catch (Exception ex)
          {
            CoreBusinessLayerService.log.WarnFormat("Error occurred while updating selections in Resource tree with plugin {0}. Ex: {1}", (object) ioneTimeJobSupport.GetType(), (object) ex);
          }
        }
      }
      DiscoveryResultCache.Instance.RemoveResult(jobId);
      return new DiscoveredObjectTreeWcfWrapper(result.ResultTree, result.timeOfCreation, result.isCached);
    }

    public OrionDiscoveryJobProgressInfo GetOneTimeJobProgress(Guid jobId)
    {
      DiscoveryResultItem discoveryResultItem = (DiscoveryResultItem) null;
      if (!DiscoveryResultCache.Instance.TryGetResultItem(jobId, ref discoveryResultItem) || discoveryResultItem == null || discoveryResultItem.Progress == null)
        return (OrionDiscoveryJobProgressInfo) null;
      CoreBusinessLayerService.log.DebugFormat("Recieved one time job {0} progress from cache", (object) jobId);
      return discoveryResultItem.Progress;
    }

    public Dictionary<string, int> ImportOneTimeJobResult(
      DiscoveredObjectTreeWcfWrapper treeOfSelection,
      int nodeId)
    {
      if (treeOfSelection == null)
        throw new ArgumentNullException(nameof (treeOfSelection));
      if (treeOfSelection.Tree == null)
        throw new NullReferenceException("treeOfSelection::Tree");
      CoreBusinessLayerService.log.DebugFormat("Importing List of Discovered Resources for node with id '{0}'", (object) nodeId);
      DateTime now = DateTime.Now;
      Dictionary<string, int> dictionary1 = new Dictionary<string, int>();
      Action action = TechnologyPollingIndicator.AuditTechnologiesChanges((IEnumerable<IDiscoveredObject>) treeOfSelection.Tree.GetAllTreeObjectsOfType<IDiscoveredObjectWithTechnology>(), nodeId);
      foreach (IOneTimeJobSupport ioneTimeJobSupport in DiscoveryHelper.GetOrderedDiscoveryPlugins().OfType<IOneTimeJobSupport>())
      {
        try
        {
          if (CoreBusinessLayerService.log.IsDebugEnabled)
            CoreBusinessLayerService.log.DebugFormat("Updating List of Discovered Resources in plugin '{0}' for node with id '{1}'", (object) ioneTimeJobSupport.GetType(), (object) nodeId);
          Dictionary<string, int> dictionary2 = ioneTimeJobSupport.UpdateDiscoveredResourcesManagedStatus(treeOfSelection.Tree, nodeId);
          if (dictionary2 != null)
          {
            if (dictionary2.Count > 0)
            {
              foreach (KeyValuePair<string, int> keyValuePair in dictionary2)
                dictionary1.Add(keyValuePair.Key, keyValuePair.Value);
            }
          }
        }
        catch (Exception ex)
        {
          CoreBusinessLayerService.log.Error((object) string.Format("Unhandled exception occured when importing one time job result with plugin {0}", (object) ioneTimeJobSupport.GetType()), ex);
        }
      }
      action();
      CoreBusinessLayerService.log.DebugFormat("Completed updating of Discovered Resources for node with id '{0}'. Total execution time: {1} ms", (object) nodeId, (object) (DateTime.Now - now).Milliseconds);
      return dictionary1;
    }

    public List<DiscoveryItemGroupDefinition> GetDiscoveryScheduledImportGroupDefinitions()
    {
      return DiscoveryHelper.GetOrderedDiscoveryPlugins().OfType<IScheduledDiscoveryImport>().SelectMany<IScheduledDiscoveryImport, IDiscoveredObjectGroupScheduledImport>((System.Func<IScheduledDiscoveryImport, IEnumerable<IDiscoveredObjectGroupScheduledImport>>) (n => n.GetScheduledDiscoveryObjectGroups())).Select<IDiscoveredObjectGroupScheduledImport, DiscoveryItemGroupDefinition>((System.Func<IDiscoveredObjectGroupScheduledImport, DiscoveryItemGroupDefinition>) (n =>
      {
        DiscoveryItemGroupDefinition groupDefinitions = new DiscoveryItemGroupDefinition();
        ((DiscoveredObjectGroupWcf<IDiscoveredObjectGroupScheduledImport>) groupDefinitions).Group = n;
        return groupDefinitions;
      })).ToList<DiscoveryItemGroupDefinition>();
    }

    public List<DiscoveryIgnoredNode> GetDiscoveryIgnoredNodes()
    {
      List<DiscoveryIgnoredNode> discoveryIgnoredNodes = new List<DiscoveryIgnoredNode>();
      bool flag = SolarWinds.Orion.Core.Common.ModuleManager.ModuleManager.InstanceWithCache.IsThereModule("NPM");
      IDictionary<int, ICollection<DiscoveryIgnoredInterfaceEntry>> ignoredInterfacesDict = DiscoveryIgnoredInterfaceEntry.GetIgnoredInterfacesDict();
      IDictionary<int, ICollection<DiscoveryIgnoredVolumeEntry>> ignoredVolumesDict = DiscoveryIgnoredVolumeEntry.GetIgnoredVolumesDict();
      IEnumerable<IScheduledDiscoveryIgnore> list = (IEnumerable<IScheduledDiscoveryIgnore>) DiscoveryHelper.GetOrderedDiscoveryPlugins().OfType<IScheduledDiscoveryIgnore>().ToList<IScheduledDiscoveryIgnore>();
      foreach (DiscoveryIgnoredNodeEntry ignoredNodes in (IEnumerable<DiscoveryIgnoredNodeEntry>) DiscoveryIgnoredNodeEntry.GetIgnoredNodesList())
      {
        DiscoveryIgnoredNode discoveryIgnoredNode = new DiscoveryIgnoredNode(ignoredNodes.ID, ignoredNodes.EngineID, ignoredNodes.IPAddress, ignoredNodes.Caption, ignoredNodes.IsIgnored, ignoredNodes.DateAdded);
        if (ignoredInterfacesDict.ContainsKey(ignoredNodes.ID))
        {
          foreach (DiscoveryIgnoredInterfaceEntry ignoredInterfaceEntry in (IEnumerable<DiscoveryIgnoredInterfaceEntry>) ignoredInterfacesDict[ignoredNodes.ID])
            discoveryIgnoredNode.IgnoredInterfaces.Add(new DiscoveryIgnoredInterface(ignoredInterfaceEntry.ID, ignoredInterfaceEntry.IgnoredNodeID, ignoredInterfaceEntry.PhysicalAddress, ignoredInterfaceEntry.Description, ignoredInterfaceEntry.Caption, ignoredInterfaceEntry.Type, ignoredInterfaceEntry.IfxName, ignoredInterfaceEntry.DateAdded));
        }
        if (ignoredVolumesDict.ContainsKey(ignoredNodes.ID))
        {
          foreach (DiscoveryIgnoredVolumeEntry ignoredVolumeEntry in (IEnumerable<DiscoveryIgnoredVolumeEntry>) ignoredVolumesDict[ignoredNodes.ID])
            discoveryIgnoredNode.IgnoredVolumes.Add(new DiscoveryIgnoredVolume(ignoredVolumeEntry.ID, ignoredVolumeEntry.IgnoredNodeID, ignoredVolumeEntry.Description, (VolumeType) ignoredVolumeEntry.Type, ignoredVolumeEntry.DateAdded));
        }
        foreach (IScheduledDiscoveryIgnore ischeduledDiscoveryIgnore in list)
        {
          DiscoveryPluginResultBase pluginResultBase = ischeduledDiscoveryIgnore.LoadIgnoredResults(ignoredNodes.ID);
          discoveryIgnoredNode.NodeResult.PluginResults.Add(pluginResultBase);
        }
        if ((flag || ignoredNodes.IsIgnored || discoveryIgnoredNode.IgnoredVolumes.Count != 0 ? 0 : (!((IEnumerable<DiscoveryPluginResultBase>) discoveryIgnoredNode.NodeResult.PluginResults).Any<DiscoveryPluginResultBase>((System.Func<DiscoveryPluginResultBase, bool>) (n => n.GetDiscoveredObjects().Any<IDiscoveredObject>())) ? 1 : 0)) == 0)
          discoveryIgnoredNodes.Add(discoveryIgnoredNode);
      }
      return discoveryIgnoredNodes;
    }

    public string AddDiscoveryIgnoredNode(DiscoveryNode discoveryNode)
    {
      int num = this.AddDiscoveryIgnoredNodeOnly(discoveryNode);
      if (num == -1)
      {
        CoreBusinessLayerService.log.ErrorFormat("Discovery Node(NodeID:{0},ProfileID:{1}) could not be ignored", (object) discoveryNode.NodeID, (object) discoveryNode.ProfileID);
        return string.Format(Resources.WEBCODE_ET_01, (object) discoveryNode.Name);
      }
      if (!discoveryNode.IsSelected)
      {
        foreach (DiscoveryInterface discoveryInterface in discoveryNode.Interfaces.Where<DiscoveryInterface>((System.Func<DiscoveryInterface, bool>) (n => n.IsSelected)))
        {
          if (!this.AddDiscoveryIgnoredInterface(discoveryNode, discoveryInterface))
            CoreBusinessLayerService.log.WarnFormat("Discovery Interface(InterfaceID:{0}) could not be ignored, because it is already ignored", (object) discoveryInterface.InterfaceID);
        }
        foreach (DiscoveryVolume discoveryVolume in discoveryNode.Volumes.Where<DiscoveryVolume>((System.Func<DiscoveryVolume, bool>) (n => n.IsSelected)))
        {
          if (!this.AddDiscoveryIgnoredVolume(discoveryNode, discoveryVolume))
            CoreBusinessLayerService.log.WarnFormat("Discovery Volume(VolumeID:{0}) could not be ignored, because it is already ignored", (object) discoveryVolume.VolumeID);
        }
        discoveryNode.NodeResult.PluginResults.Add((DiscoveryPluginResultBase) new CoreDiscoveryPluginResult()
        {
          DiscoveredNodes = new List<DiscoveredNode>()
          {
            new DiscoveredNode()
            {
              IgnoredNodeId = new int?(num),
              NodeID = discoveryNode.NodeID,
              OrionNodeId = new int?(discoveryNode.ManagedNodeId)
            }
          }
        });
        foreach (IScheduledDiscoveryIgnore ischeduledDiscoveryIgnore in DiscoveryHelper.GetOrderedDiscoveryPlugins().OfType<IScheduledDiscoveryIgnore>())
          ischeduledDiscoveryIgnore.StoreResultsToIgnoreList(discoveryNode.NodeResult);
      }
      return string.Empty;
    }

    private int AddDiscoveryIgnoredNodeOnly(DiscoveryNode discoveryNode)
    {
      CoreBusinessLayerService.log.Debug((object) "Sending request for insert ignored node to DAL.");
      try
      {
        return DiscoveryIgnoredNodeEntry.Insert(discoveryNode.EngineID, discoveryNode.IPAddress, discoveryNode.Name, discoveryNode.IsSelected, discoveryNode.ProfileID);
      }
      catch (Exception ex)
      {
        CoreBusinessLayerService.log.Error((object) ("Error when inserting ignored node: " + ex.ToString()));
        throw new CoreBusinessLayerService.DiscoveryInsertingIgnoredNodeError(Resources.DiscoveryBL_DiscoveryInsertingIgnoredNodeError, new object[1]
        {
          (object) discoveryNode.IPAddress
        }, ex);
      }
    }

    public bool AddDiscoveryIgnoredNodesNewDiscovery(IEnumerable<DiscoveredNode> discoveredNodes)
    {
      IEnumerable<IScheduledDiscoveryIgnore> list = (IEnumerable<IScheduledDiscoveryIgnore>) DiscoveryHelper.GetOrderedDiscoveryPlugins().OfType<IScheduledDiscoveryIgnore>().ToList<IScheduledDiscoveryIgnore>();
      IDictionary<int, int> dictionary = (IDictionary<int, int>) new Dictionary<int, int>();
      bool flag = SolarWinds.Orion.Core.Common.ModuleManager.ModuleManager.InstanceWithCache.IsThereModule("NPM");
      foreach (DiscoveredNode discoveredNode in discoveredNodes)
      {
        CoreBusinessLayerService.log.Debug((object) "Sending request for insert ignored node to DAL.");
        try
        {
          int engineId;
          if (dictionary.ContainsKey(discoveredNode.ProfileID))
          {
            engineId = dictionary[discoveredNode.ProfileID];
          }
          else
          {
            engineId = DiscoveryProfileEntry.GetProfileByID(discoveredNode.ProfileID).EngineID;
            dictionary[discoveredNode.ProfileID] = engineId;
          }
          string stringIp = IPAddressHelper.ToStringIp(discoveredNode.IP);
          string displayName = ((DiscoveredObjectBase) discoveredNode).DisplayName;
          int num = DiscoveryIgnoredNodeEntry.Insert(engineId, stringIp, displayName, true, discoveredNode.ProfileID);
          foreach (DiscoveryIgnoredDAL.VolumeInfo volumeInfo in DiscoveryIgnoredDAL.GetDiscoveredVolumesForNode(discoveredNode))
            DiscoveryIgnoredVolumeEntry.Insert(engineId, stringIp, displayName, volumeInfo.VolumeDescription, volumeInfo.VolumeType, discoveredNode.NodeID, discoveredNode.ProfileID, num);
          if (flag)
          {
            foreach (DiscoveryIgnoredDAL.InterfaceInfo interfaceInfo in DiscoveryIgnoredDAL.GetDiscoveredInterfacesForNode(discoveredNode))
              DiscoveryIgnoredInterfaceEntry.Insert(engineId, stringIp, displayName, interfaceInfo.PhysicalAddress, interfaceInfo.InterfaceName, interfaceInfo.InterfaceName, interfaceInfo.InterfaceType, interfaceInfo.IfName, discoveredNode.NodeID, discoveredNode.ProfileID, num);
          }
          DiscoveryResultBase discoveryResultBase = new DiscoveryResultBase();
          CoreDiscoveryPluginResult discoveryPluginResult = new CoreDiscoveryPluginResult();
          discoveryPluginResult.DiscoveredNodes = new List<DiscoveredNode>()
          {
            discoveredNode
          };
          discoveredNode.IgnoredNodeId = new int?(num);
          discoveryResultBase.PluginResults.Add((DiscoveryPluginResultBase) discoveryPluginResult);
          foreach (IScheduledDiscoveryIgnore ischeduledDiscoveryIgnore in list)
          {
            DiscoveryPluginResultBase pluginResultBase = ((IScheduledDiscoveryImport) ischeduledDiscoveryIgnore).LoadResults(discoveredNode.ProfileID, discoveredNode.NodeID);
            discoveryResultBase.PluginResults.Add(pluginResultBase);
            ischeduledDiscoveryIgnore.StoreResultsToIgnoreList(discoveryResultBase);
          }
        }
        catch (Exception ex)
        {
          CoreBusinessLayerService.log.Error((object) ("Error when inserting ignored node: " + ex.ToString()));
          throw new CoreBusinessLayerService.DiscoveryInsertingIgnoredNodeError(Resources.DiscoveryBL_DiscoveryInsertingIgnoredNodeError, new object[1]
          {
            (object) IPAddressHelper.ToStringIp(discoveredNode.IP)
          }, ex);
        }
      }
      return true;
    }

    public bool AddDiscoveryIgnoredNodeNewDiscovery(DiscoveredNode discoveredNode)
    {
      CoreBusinessLayerService.log.Debug((object) "Sending request for insert ignored node to DAL.");
      try
      {
        int engineId = DiscoveryProfileEntry.GetProfileByID(discoveredNode.ProfileID).EngineID;
        string stringIp = IPAddressHelper.ToStringIp(discoveredNode.IP);
        string displayName = ((DiscoveredObjectBase) discoveredNode).DisplayName;
        int num = DiscoveryIgnoredNodeEntry.Insert(engineId, stringIp, displayName, true, discoveredNode.ProfileID);
        bool flag = num != -1;
        foreach (DiscoveryIgnoredDAL.VolumeInfo volumeInfo in DiscoveryIgnoredDAL.GetDiscoveredVolumesForNode(discoveredNode))
          DiscoveryIgnoredVolumeEntry.Insert(engineId, stringIp, displayName, volumeInfo.VolumeDescription, volumeInfo.VolumeType, discoveredNode.NodeID, discoveredNode.ProfileID, num);
        if (SolarWinds.Orion.Core.Common.ModuleManager.ModuleManager.InstanceWithCache.IsThereModule("NPM"))
        {
          foreach (DiscoveryIgnoredDAL.InterfaceInfo interfaceInfo in DiscoveryIgnoredDAL.GetDiscoveredInterfacesForNode(discoveredNode))
            DiscoveryIgnoredInterfaceEntry.Insert(engineId, stringIp, displayName, interfaceInfo.PhysicalAddress, interfaceInfo.InterfaceName, interfaceInfo.InterfaceName, interfaceInfo.InterfaceType, interfaceInfo.IfName, discoveredNode.NodeID, discoveredNode.ProfileID, num);
        }
        DiscoveryResultBase discoveryResultBase = new DiscoveryResultBase();
        CoreDiscoveryPluginResult discoveryPluginResult = new CoreDiscoveryPluginResult();
        discoveryPluginResult.DiscoveredNodes = new List<DiscoveredNode>()
        {
          discoveredNode
        };
        discoveredNode.IgnoredNodeId = new int?(num);
        discoveryResultBase.PluginResults.Add((DiscoveryPluginResultBase) discoveryPluginResult);
        foreach (IScheduledDiscoveryIgnore ischeduledDiscoveryIgnore in DiscoveryHelper.GetOrderedDiscoveryPlugins().OfType<IScheduledDiscoveryIgnore>())
        {
          DiscoveryPluginResultBase pluginResultBase = ((IScheduledDiscoveryImport) ischeduledDiscoveryIgnore).LoadResults(discoveredNode.ProfileID, discoveredNode.NodeID);
          discoveryResultBase.PluginResults.Add(pluginResultBase);
          ischeduledDiscoveryIgnore.StoreResultsToIgnoreList(discoveryResultBase);
        }
        return flag;
      }
      catch (Exception ex)
      {
        CoreBusinessLayerService.log.Error((object) ("Error when inserting ignored node: " + ex.ToString()));
        throw new CoreBusinessLayerService.DiscoveryInsertingIgnoredNodeError(Resources.DiscoveryBL_DiscoveryInsertingIgnoredNodeError, new object[1]
        {
          (object) IPAddressHelper.ToStringIp(discoveredNode.IP)
        }, ex);
      }
    }

    public void DeleteDiscoveryIgnoredNodes(IEnumerable<DiscoveryIgnoredNode> nodes)
    {
      if (nodes == null)
        throw new ArgumentNullException(nameof (nodes));
      IEnumerable<IScheduledDiscoveryIgnore> list = (IEnumerable<IScheduledDiscoveryIgnore>) DiscoveryHelper.GetOrderedDiscoveryPlugins().OfType<IScheduledDiscoveryIgnore>().ToList<IScheduledDiscoveryIgnore>();
      List<int> intList1 = new List<int>();
      List<int> intList2 = new List<int>();
      foreach (DiscoveryIgnoredNode node in nodes)
      {
        if (node != null)
        {
          foreach (DiscoveryIgnoredVolume discoveryIgnoredVolume in node.IgnoredVolumes.Where<DiscoveryIgnoredVolume>((System.Func<DiscoveryIgnoredVolume, bool>) (n => n.IsSelected)))
            this.DeleteDiscoveryIgnoredVolume(discoveryIgnoredVolume);
          foreach (DiscoveryIgnoredInterface discoveryIgnoredInterface in node.IgnoredInterfaces.Where<DiscoveryIgnoredInterface>((System.Func<DiscoveryIgnoredInterface, bool>) (n => n.IsSelected)))
            this.DeleteDiscoveryIgnoredInterface(discoveryIgnoredInterface);
          bool flag = true;
          foreach (IScheduledDiscoveryIgnore ischeduledDiscoveryIgnore in list)
          {
            ischeduledDiscoveryIgnore.RemoveResultsFromIgnoreList(node.NodeResult);
            if (flag && ischeduledDiscoveryIgnore.LoadIgnoredResults(node.ID).GetDiscoveredObjects().Any<IDiscoveredObject>())
              flag = false;
          }
          if (flag)
            intList2.Add(node.ID);
          else
            intList1.Add(node.ID);
        }
      }
      if (intList2.Count > 0)
        DiscoveryIgnoredNodeEntry.DeleteByListID(intList2);
      if (intList1.Count <= 0)
        return;
      DiscoveryIgnoredNodeEntry.DisableIsIgnoredList(intList1);
    }

    public void DeleteDiscoveryIgnoredNode(DiscoveryIgnoredNode node)
    {
      if (node == null)
        throw new ArgumentNullException(nameof (node));
      foreach (DiscoveryIgnoredVolume discoveryIgnoredVolume in node.IgnoredVolumes.Where<DiscoveryIgnoredVolume>((System.Func<DiscoveryIgnoredVolume, bool>) (n => n.IsSelected)))
        this.DeleteDiscoveryIgnoredVolume(discoveryIgnoredVolume);
      foreach (DiscoveryIgnoredInterface discoveryIgnoredInterface in node.IgnoredInterfaces.Where<DiscoveryIgnoredInterface>((System.Func<DiscoveryIgnoredInterface, bool>) (n => n.IsSelected)))
        this.DeleteDiscoveryIgnoredInterface(discoveryIgnoredInterface);
      bool flag = true;
      foreach (IScheduledDiscoveryIgnore ischeduledDiscoveryIgnore in DiscoveryHelper.GetOrderedDiscoveryPlugins().OfType<IScheduledDiscoveryIgnore>())
      {
        ischeduledDiscoveryIgnore.RemoveResultsFromIgnoreList(node.NodeResult);
        if (flag && ischeduledDiscoveryIgnore.LoadIgnoredResults(node.ID).GetDiscoveredObjects().Any<IDiscoveredObject>())
          flag = false;
      }
      if (flag)
      {
        DiscoveryIgnoredNodeEntry.DeleteByID(node.ID);
      }
      else
      {
        if (!node.IsSelected || !node.IsIgnored)
          return;
        DiscoveryIgnoredNodeEntry.DisableIsIgnored(node.ID);
      }
    }

    [Obsolete("Core-Split cleanup. If you need this member please contact Core team", true)]
    public void DeleteDiscoveryIgnoredNode(int ignoredNodeID)
    {
      CoreBusinessLayerService.log.Debug((object) "Sending request for delete to DAL.");
      try
      {
        DiscoveryIgnoredNodeEntry.DeleteByID(ignoredNodeID);
      }
      catch (Exception ex)
      {
        CoreBusinessLayerService.log.Error((object) ("Error when deleting ignored node: " + ex.ToString()));
        throw new CoreBusinessLayerService.DiscoveryDeletingIgnoredNodeError(Resources.DiscoveryBL_DiscoveryDeletingIgnoredNodeError, new object[1]
        {
          (object) ignoredNodeID
        }, ex);
      }
    }

    public void RemoveDiscoveryNodeFromIgnored(DiscoveryNode discoveryNode)
    {
      CoreBusinessLayerService.log.Debug((object) "Sending request for delete to DAL.");
      try
      {
        DiscoveryIgnoredNodeEntry.DeleteByKeyColums(discoveryNode.EngineID, discoveryNode.IPAddress, discoveryNode.Name);
      }
      catch (Exception ex)
      {
        CoreBusinessLayerService.log.Error((object) ("Error when deleting ignored node: " + ex.ToString()));
        throw new CoreBusinessLayerService.DiscoveryDeletingIgnoredNodeError(Resources.DiscoveryBL_DiscoveryDeletingIgnoredNodeError_Engine, new object[2]
        {
          (object) discoveryNode.EngineID,
          (object) discoveryNode.IPAddress
        }, ex);
      }
    }

    public bool AddDiscoveryIgnoredInterface(
      DiscoveryNode discoveryNode,
      DiscoveryInterface discoveryInterface)
    {
      CoreBusinessLayerService.log.Debug((object) "Sending request for insert ignored interface to DAL.");
      try
      {
        return DiscoveryIgnoredInterfaceEntry.Insert(discoveryNode.EngineID, discoveryNode.IPAddress, discoveryNode.Name, discoveryInterface.PhysicalAddress, discoveryInterface.InterfaceDescription, discoveryInterface.InterfaceCaption, discoveryInterface.InterfaceType, discoveryInterface.IfxName, discoveryNode.NodeID, discoveryNode.ProfileID, 0);
      }
      catch (Exception ex)
      {
        CoreBusinessLayerService.log.Error((object) ("Error when inserting ignored interface: " + ex.ToString()));
        throw new CoreBusinessLayerService.DiscoveryInsertingIgnoredInterfaceError(Resources.DiscoveryBL_DiscoveryInsertingIgnoredInterfaceError, new object[1]
        {
          (object) discoveryNode.IPAddress
        }, ex);
      }
    }

    public bool DeleteDiscoveryIgnoredInterface(
      DiscoveryIgnoredInterface discoveryIgnoredInterface)
    {
      CoreBusinessLayerService.log.Debug((object) "Sending request for delete ignored interface to DAL.");
      try
      {
        return DiscoveryIgnoredInterfaceEntry.DeleteByID(discoveryIgnoredInterface.ID);
      }
      catch (Exception ex)
      {
        CoreBusinessLayerService.log.Error((object) ("Error when deleting ignored interface: " + ex.ToString()));
        throw new CoreBusinessLayerService.DiscoveryDeletingIgnoredInterfaceError(Resources.DiscoveryBL_DiscoveryDeletingIgnoredInterfaceError, new object[1]
        {
          (object) discoveryIgnoredInterface.Description
        }, ex);
      }
    }

    public bool AddDiscoveryIgnoredVolume(
      DiscoveryNode discoveryNode,
      DiscoveryVolume discoveryVolume)
    {
      CoreBusinessLayerService.log.Debug((object) "Sending request for insert ignored volume to DAL.");
      try
      {
        return DiscoveryIgnoredVolumeEntry.Insert(discoveryNode.EngineID, discoveryNode.IPAddress, discoveryNode.Name, discoveryVolume.VolumeDescription, (int) discoveryVolume.VolumeType, discoveryNode.NodeID, discoveryNode.ProfileID, 0);
      }
      catch (Exception ex)
      {
        CoreBusinessLayerService.log.Error((object) ("Error when inserting ignored volume: " + ex.ToString()));
        throw new CoreBusinessLayerService.DiscoveryInsertingIgnoredVolumeError(Resources.DiscoveryBL_DiscoveryInsertingIgnoredVolumeError, new object[1]
        {
          (object) discoveryNode.IPAddress
        }, ex);
      }
    }

    public bool DeleteDiscoveryIgnoredVolume(DiscoveryIgnoredVolume discoveryIgnoredVolume)
    {
      CoreBusinessLayerService.log.Debug((object) "Sending request for delete ignored volume to DAL.");
      try
      {
        return DiscoveryIgnoredVolumeEntry.DeleteByID(discoveryIgnoredVolume.ID);
      }
      catch (Exception ex)
      {
        CoreBusinessLayerService.log.Error((object) ("Error when deleting ignored volume: " + ex.ToString()));
        throw new CoreBusinessLayerService.DiscoveryDeletingIgnoredVolumeError(Resources.DiscoveryBL_DiscoveryDeletingIgnoredVolumeError, new object[1]
        {
          (object) discoveryIgnoredVolume.Description
        }, ex);
      }
    }

    public string ValidateBulkList(IEnumerable<string> bulkList)
    {
      StringBuilder errors = new StringBuilder();
      foreach (string normalizeHostName in HostListNormalizer.NormalizeHostNames(bulkList))
        this.ValidateHostAddress(normalizeHostName, errors);
      return errors.ToString();
    }

    public List<Subnet> FindRouterSubnets(
      string router,
      List<SnmpEntry> credentials,
      int engineId,
      out string errorMessage)
    {
      StringBuilder errors = new StringBuilder();
      if (!this.ValidateHostAddress(router, errors))
      {
        errorMessage = errors.ToString();
        return (List<Subnet>) null;
      }
      errorMessage = (string) null;
      string str = ((IEnumerable<IPAddress>) Dns.GetHostAddresses(router)).FirstOrDefault<IPAddress>((System.Func<IPAddress, bool>) (ipaddress => ipaddress.AddressFamily == AddressFamily.InterNetwork)).ToString();
      if (str == null)
      {
        CoreBusinessLayerService.log.Error((object) string.Format("IP address for host {0} is missing", (object) router));
        throw new CoreBusinessLayerService.DiscoveryHostAddressMissingError(Resources.DiscoveryBL_DiscoveryHostAddressMissingError, new object[1]
        {
          (object) router
        });
      }
      Dictionary<string, string> dictionary;
      SNMPHelper.SNMPQueryForIp(str, "1.3.6.1.2.1.4.21.1.11", credentials, "getsubtree", ref dictionary);
      List<Subnet> routerSubnets = new List<Subnet>();
      foreach (KeyValuePair<string, string> keyValuePair in dictionary)
      {
        uint num;
        if (!(keyValuePair.Value == "255.255.255.255") && !(keyValuePair.Value == "0.0.0.0") && !(keyValuePair.Key == "127.0.0.0") && HostHelper.IsIpAddress(keyValuePair.Key) && HostHelper.IsIpAddress(keyValuePair.Value) && Subnet.GetSubnetClass(keyValuePair.Key, ref num) != null)
        {
          Subnet subnet = new Subnet(keyValuePair.Key, keyValuePair.Value);
          routerSubnets.Add(subnet);
        }
      }
      if (routerSubnets.Count != 0)
        return routerSubnets;
      errorMessage = Resources.CODE_VB0_1;
      return (List<Subnet>) null;
    }

    [Obsolete("There is no longer a sigle license count value.  Use FeatureManager to get the license limits for each element type.", true)]
    public int GetLicenseCount() => 0;

    public void ValidateProfilesTimeout()
    {
      new List<DiscoveryProfileEntry>((IEnumerable<DiscoveryProfileEntry>) DiscoveryProfileEntry.GetAllProfiles()).ForEach((Action<DiscoveryProfileEntry>) (profile =>
      {
        if (!(DateTime.MinValue != profile.LastRun) || (DateTime.Now - profile.LastRun.ToLocalTime()).TotalMinutes <= (double) profile.JobTimeout || profile.Status.Status != 1)
          return;
        CoreBusinessLayerService.log.Warn((object) string.Format("Discovery profile {0} end during timeout {1}", (object) profile.ProfileID, (object) profile.JobTimeout));
        profile.Status = new DiscoveryComplexStatus((DiscoveryStatus) 3, "LIBCODE_TM0_25");
        profile.Update();
      }));
    }

    public Intervals GetSettingsPollingIntervals() => DiscoveryDAL.GetSettingsPollingIntervals();

    public List<SnmpEntry> GetAllCredentials() => DiscoveryDAL.GetAllCredentials();

    internal bool UpdateDiscoveryJobs(int engineId)
    {
      return this.UpdateSelectedDiscoveryJobs((List<int>) null, engineId);
    }

    public void UpdateSelectedDiscoveryJobs(List<int> profileIdsFilter)
    {
      int operationEngineId = this.GetCurrentOperationEngineId();
      this.UpdateSelectedDiscoveryJobs(profileIdsFilter, operationEngineId);
    }

    private bool UpdateSelectedDiscoveryJobs(List<int> profileIdsFilter, int engineId)
    {
      try
      {
        CoreBusinessLayerService.log.Debug((object) "Updating scheduled discovery jobs.");
        if (profileIdsFilter != null && profileIdsFilter.Count == 0)
          return true;
        string errorMessage;
        if (this.TryConnectionWithJobSchedulerV2(out errorMessage))
        {
          ICollection<DiscoveryProfileEntry> allProfiles = DiscoveryProfileEntry.GetAllProfiles();
          CoreBusinessLayerService.log.Debug((object) "Filtering old profiles");
          List<DiscoveryProfileEntry> list = allProfiles.Where<DiscoveryProfileEntry>((System.Func<DiscoveryProfileEntry, bool>) (p => p.SIPPort == 0)).Select<DiscoveryProfileEntry, DiscoveryProfileEntry>((System.Func<DiscoveryProfileEntry, DiscoveryProfileEntry>) (p => p)).ToList<DiscoveryProfileEntry>();
          if (profileIdsFilter != null)
            list = list.Where<DiscoveryProfileEntry>((System.Func<DiscoveryProfileEntry, bool>) (p => profileIdsFilter.Contains(p.ProfileID))).ToList<DiscoveryProfileEntry>();
          foreach (DiscoveryProfileEntry profile in list)
          {
            DiscoveryConfiguration discoveryConfiguration = this.ServiceContainer.GetService<IDiscoveryDAL>().GetDiscoveryConfiguration(profile.ProfileID);
            if (((DiscoveryConfigurationBase) discoveryConfiguration).EngineId == engineId && profile.IsScheduled)
              this.UpdateDiscoveryJob(profile, discoveryConfiguration);
          }
          return true;
        }
        CoreBusinessLayerService.log.WarnFormat("Can't update scheduled jobs, JobScheduler is not running. - {0}", (object) errorMessage);
        return false;
      }
      catch (Exception ex)
      {
        CoreBusinessLayerService.log.Error((object) "Unhandled exception occured when rescheduling discovery jobs", ex);
        return false;
      }
    }

    private void UpdateDiscoveryJob(
      DiscoveryProfileEntry profile,
      DiscoveryConfiguration configuration)
    {
      CoreBusinessLayerService.log.DebugFormat("Updating discovery job. ProfileId={0}.", (object) profile.ProfileID);
      if (configuration == null)
      {
        CoreBusinessLayerService.log.ErrorFormat("Discovery Configuration wasn't found. ProfileId={0}", (object) profile.ProfileID);
      }
      else
      {
        ScheduledJob discoveryJob = this.JobFactory.CreateDiscoveryJob(configuration);
        if (discoveryJob == null)
          return;
        discoveryJob.InitialWait = CoreBusinessLayerService.CalculateJobInitialWait(profile);
        CoreBusinessLayerService.log.DebugFormat("Submiting job for profile {0}. CronExpression={1}, Frequency={2}, InitialWait={3}", new object[4]
        {
          (object) profile.ProfileID,
          (object) discoveryJob.CronExpression,
          (object) discoveryJob.Frequency,
          (object) discoveryJob.InitialWait
        });
        Guid localEngine = this.JobFactory.SubmitScheduledJobToLocalEngine(profile.JobID, discoveryJob, false);
        if (localEngine != profile.JobID)
        {
          CoreBusinessLayerService.log.DebugFormat("Updating profile, ProfileId={0}.", (object) profile.ProfileID);
          profile.JobID = localEngine;
          profile.Status = new DiscoveryComplexStatus((DiscoveryStatus) 5, string.Empty);
          profile.Update();
        }
        CoreBusinessLayerService.log.DebugFormat("Discovery job was updated successfully, ProfileId={0}.", (object) profile.ProfileID);
      }
    }

    private static TimeSpan CalculateJobInitialWait(DiscoveryProfileEntry profile)
    {
      DateTime dateTime = DateTime.Now;
      dateTime = dateTime.ToUniversalTime();
      int int32_1 = Convert.ToInt32(dateTime.TimeOfDay.TotalMinutes);
      int minutes = 0;
      if (!profile.ScheduleRunAtTime.Equals(DateTime.MinValue))
      {
        int int32_2 = Convert.ToInt32(profile.ScheduleRunAtTime.TimeOfDay.TotalMinutes);
        minutes = int32_1 >= int32_2 ? 1440 - (int32_1 - int32_2) : int32_2 - int32_1;
      }
      if (profile.ScheduleRunFrequency != 0)
        minutes = profile.ScheduleRunFrequency;
      return new TimeSpan(0, minutes, 0);
    }

    private bool ValidateHostAddress(string hostNameOrAddress, StringBuilder errors)
    {
      try
      {
        Dns.GetHostAddresses(hostNameOrAddress);
        return true;
      }
      catch (ArgumentOutOfRangeException ex)
      {
        errors.AppendFormat(Resources.LIBCODE_TM0_12 + "<br />", (object) hostNameOrAddress);
      }
      catch (ArgumentException ex)
      {
        errors.AppendFormat(Resources.LIBCODE_TM0_13 + "<br />", (object) hostNameOrAddress);
      }
      catch (Exception ex)
      {
        errors.AppendFormat(Resources.LIBCODE_TM0_14 + "<br/>", (object) hostNameOrAddress);
      }
      return false;
    }

    internal void ForceDiscoveryPluginsToLoadTypes()
    {
      CoreBusinessLayerService.log.Debug((object) "Start loading plugins known types");
      IList<IDiscoveryPlugin> discoveryPlugins = DiscoveryHelper.GetOrderedDiscoveryPlugins();
      CoreBusinessLayerService.log.DebugFormat("Number of found plugins:", (object) discoveryPlugins.Count);
      System.Type[] array = discoveryPlugins.SelectMany<IDiscoveryPlugin, System.Type>((System.Func<IDiscoveryPlugin, IEnumerable<System.Type>>) (plugin => (IEnumerable<System.Type>) plugin.GetKnownTypes())).ToArray<System.Type>();
      CoreBusinessLayerService.log.DebugFormat("Number of found known types:", (object) array.Length);
      CoreBusinessLayerService.log.Debug((object) "Finish loading plugins known types");
    }

    [Obsolete("Core-Split cleanup. If you need this member please contact Core team", true)]
    public void StartDiscoveryImport(int profileId)
    {
    }

    [Obsolete("Core-Split cleanup. If you need this member please contact Core team", true)]
    public void StoreDiscoveryConfiguration(DiscoveryConfiguration configuration)
    {
      throw new NotImplementedException();
    }

    [Obsolete("Core-Split cleanup. If you need this member please contact Core team", true)]
    public DiscoveryConfiguration LoadDiscoveryConfiguration(int profileID)
    {
      throw new NotImplementedException();
    }

    public DiscoveryResultBase GetDiscoveryResult(int profileId)
    {
      return DiscoveryResultManager.GetDiscoveryResult(profileId, DiscoveryHelper.GetOrderedDiscoveryPlugins());
    }

    public DiscoveryImportProgressInfo GetOrionDiscoveryImportProgress(Guid importID)
    {
      return DiscoveryImportManager.GetImportProgress(importID);
    }

    public StartImportStatus ImportOrionDiscoveryResults(Guid importId, DiscoveryResultBase result)
    {
      SortedDictionary<int, List<IDiscoveryPlugin>> orderedPlugins = DiscoveryPluginHelper.GetOrderedPlugins(DiscoveryHelper.GetOrderedDiscoveryPlugins(), (IList<DiscoveryPluginInfo>) DiscoveryHelper.GetDiscoveryPluginInfos());
      return DiscoveryImportManager.StartImport(importId, result, orderedPlugins, false, (DiscoveryImportManager.CallbackDiscoveryImportFinished) ((_result, importJobID, StartImportStatus) =>
      {
        try
        {
          DiscoveryLogs discoveryLog = new DiscoveryLogs();
          DiscoveryImportManager.FillDiscoveryLogEntity(discoveryLog, _result, StartImportStatus);
          discoveryLog.AutoImport = false;
          using (CoreSwisContext systemContext = SwisContextFactory.CreateSystemContext())
            discoveryLog.Create((SwisContext) systemContext);
          CoreBusinessLayerService.log.InfoFormat("DiscoveryLog created for ProfileID:{0}", (object) discoveryLog.ProfileID);
        }
        catch (Exception ex)
        {
          CoreBusinessLayerService.log.Error((object) "Unable to create discovery import log", ex);
        }
      }));
    }

    public int CreateOrionDiscoveryProfileFromConfigurationStrings(
      DiscoveryConfiguration configurationWithoutPluginInformation,
      List<string> discoveryPluginConfigurationBaseItems)
    {
      foreach (DiscoveryPluginConfigurationBase configurationItem in this.discoveryLogic.DeserializePluginConfigurationItems(discoveryPluginConfigurationBaseItems))
        ((DiscoveryConfigurationBase) configurationWithoutPluginInformation).PluginConfigurations.Add(configurationItem);
      return this.CreateOrionDiscoveryProfile(configurationWithoutPluginInformation, true);
    }

    protected int CreateOrionDiscoveryProfile(
      DiscoveryConfiguration configuration,
      bool refreshCredentialsFromDb)
    {
      CoreBusinessLayerService.log.Debug((object) "Creating new discovery profile.");
      List<int> usedCredentials;
      int newConfiguration = this.ServiceContainer.GetService<IDiscoveryDAL>().CreateNewConfiguration(configuration, refreshCredentialsFromDb, ref usedCredentials);
      if (usedCredentials.Count > 0)
        this.RescheduleJobsWithUsingCredentials(usedCredentials, newConfiguration);
      CoreBusinessLayerService.log.DebugFormat("Discovery profile {0} was successfully created.", (object) newConfiguration);
      return newConfiguration;
    }

    public int CreateOrionDiscoveryProfile(DiscoveryConfiguration configuration)
    {
      return this.CreateOrionDiscoveryProfile(configuration, false);
    }

    public void UpdateOrionDiscoveryProfile(DiscoveryConfiguration configuration)
    {
      if (configuration.ProfileID.HasValue)
        CoreBusinessLayerService.log.DebugFormat("Updating configuration for profile {0}", (object) configuration.ProfileID);
      else
        CoreBusinessLayerService.log.Warn((object) "Trying to update configuration for profile with no ID");
      List<int> usedCredentials = this.ServiceContainer.GetService<IDiscoveryDAL>().StoreDiscoveryConfiguration(configuration);
      if (usedCredentials.Count > 0)
        this.RescheduleJobsWithUsingCredentials(usedCredentials, ((DiscoveryConfigurationBase) configuration).ProfileId.Value);
      CoreBusinessLayerService.log.DebugFormat("Configuration for profile {0} updated.", (object) configuration.ProfileID);
    }

    private void RescheduleJobsWithUsingCredentials(
      List<int> usedCredentials,
      int excludedProfileId)
    {
      List<int> list = this.GetProfileIDsUsingCredentials(usedCredentials).Where<int>((System.Func<int, bool>) (id => id != excludedProfileId)).ToList<int>();
      CoreBusinessLayerService.log.InfoFormat("Rescheduling discovery profiles [{0}] because credential it is refefencing were changed.", (object) string.Join(", ", list.Select<int, string>((System.Func<int, string>) (id => id.ToString())).ToArray<string>()));
      new DiscoveryJobRescheduler().RescheduleJobsForProfiles(list);
    }

    public List<int> GetProfileIDsUsingCredentials(List<int> credentialIdList)
    {
      if (credentialIdList == null)
        throw new ArgumentNullException(nameof (credentialIdList));
      List<int> result = new List<int>();
      if (credentialIdList.Count == 0)
        return result;
      IDiscoveryDAL service = this.ServiceContainer.GetService<IDiscoveryDAL>();
      List<int> allProfileIds = service.GetAllProfileIDs();
      List<DiscoveryConfiguration> discoveryConfigurationList = new List<DiscoveryConfiguration>();
      foreach (int num in allProfileIds)
        discoveryConfigurationList.Add(service.GetDiscoveryConfiguration(num));
      foreach (DiscoveryConfiguration discoveryConfiguration in discoveryConfigurationList)
      {
        DiscoveryConfiguration configuration = discoveryConfiguration;
        foreach (ICredentialStorage icredentialStorage in ((IEnumerable) ((DiscoveryConfigurationBase) configuration).PluginConfigurations).OfType<ICredentialStorage>())
          icredentialStorage.GetCredentialList().ToList<int>().ForEach((Action<int>) (c =>
          {
            if (!credentialIdList.Contains(c))
              return;
            List<int> intList1 = result;
            int? profileId = ((DiscoveryConfigurationBase) configuration).ProfileId;
            int num1 = profileId.Value;
            if (intList1.Contains(num1))
              return;
            List<int> intList2 = result;
            profileId = ((DiscoveryConfigurationBase) configuration).ProfileId;
            int num2 = profileId.Value;
            intList2.Add(num2);
          }));
      }
      return result;
    }

    public void DeleteOrionDiscoveryProfile(int profileID)
    {
      this.discoveryLogic.DeleteOrionDiscoveryProfile(profileID);
    }

    public void DeleteHiddenOrionDiscoveryProfilesByName(string profileName)
    {
      this.discoveryLogic.DeleteHiddenOrionDiscoveryProfilesByName(profileName);
    }

    public DiscoveryConfiguration GetOrionDiscoveryConfigurationByProfile(int profileID)
    {
      return this.ServiceContainer.GetService<IDiscoveryDAL>().GetDiscoveryConfiguration(profileID);
    }

    public void CancelOrionDiscovery(int profileID)
    {
      DiscoveryProfileEntry profileById = DiscoveryProfileEntry.GetProfileByID(profileID);
      if (profileById.Status.Status != 1 || !(profileById.JobID != Guid.Empty))
        return;
      using (IJobSchedulerHelper localInstance = JobScheduler.GetLocalInstance())
      {
        CoreBusinessLayerService.log.DebugFormat("Checking if job {0} exists in is really running", (object) profileById.JobID);
        OrionDiscoveryJobProgressInfo progressInfo = OrionDiscoveryJobSchedulerEventsService.GetProgressInfo(profileID);
        if (progressInfo == null)
        {
          string str = "An error has occurred during Network Discovery cancellation: there are no Network Discovery jobs to cancel.\r\nThis error may be due to either a lost database connection or a business layer fault condition.";
          profileById.Status = new DiscoveryComplexStatus((DiscoveryStatus) 3, "WEBDATA_TP0_ERROR_DURING_DISCOVERY");
          profileById.Update();
          CoreBusinessLayerService.log.ErrorFormat("Job {0}: {1}", (object) profileById.JobID, (object) str);
          throw new CoreBusinessLayerService.DiscoveryJobCancellationError(Resources.DiscoveryBL_DiscoveryJobCancellationError, new object[1]
          {
            (object) profileById.JobID
          });
        }
        CoreBusinessLayerService.log.DebugFormat("Cancelling job {0}", (object) profileById.JobID);
        try
        {
          OrionDiscoveryJobSchedulerEventsService.CancelDiscoveryJob(profileById.ProfileID);
          ((IJobScheduler) localInstance).CancelJob(profileById.JobID);
          profileById.Status = new DiscoveryComplexStatus((DiscoveryStatus) 7, "WEBDATA_TP0_DISCOVERY_CANCELLED_BY_USER");
          profileById.Update();
        }
        catch (Exception ex)
        {
          string str = "An error has occurred during Network Discovery cancellation: " + ex.Message;
          profileById.Status = new DiscoveryComplexStatus((DiscoveryStatus) 3, "WEBDATA_TP0_ERROR_DURING_DISCOVER_NO_JOB");
          profileById.Update();
          if (progressInfo != null)
            progressInfo.Status = new DiscoveryComplexStatus((DiscoveryStatus) 3, "WEBDATA_TP0_ERROR_DURING_DISCOVER_NO_JOB");
          CoreBusinessLayerService.log.ErrorFormat("Job {0}: {1}", (object) profileById.JobID, (object) str);
          throw;
        }
      }
    }

    public bool TryConnectionWithJobSchedulerV2(out string errorMessage)
    {
      try
      {
        using (IJobSchedulerHelper localInstance = JobScheduler.GetLocalInstance())
        {
          ((IJobScheduler) localInstance).PolicyExists("Nothing");
          errorMessage = string.Empty;
          return true;
        }
      }
      catch (Exception ex)
      {
        errorMessage = string.Format("{0}: {1}", (object) ex.GetType().Name, (object) ex.Message);
        return false;
      }
    }

    public List<SnmpCredentialsV2> GetSharedSnmpV2Credentials(string owner)
    {
      List<SnmpCredentialsV2> snmpV2Credentials = new List<SnmpCredentialsV2>();
      try
      {
        snmpV2Credentials = new CredentialManager().GetCredentials<SnmpCredentialsV2>(owner).ToList<SnmpCredentialsV2>();
      }
      catch (Exception ex)
      {
        CoreBusinessLayerService.log.Error((object) "Unhandled exception occured when loading shared credentials.", ex);
      }
      return snmpV2Credentials;
    }

    public List<SnmpCredentialsV3> GetSharedSnmpV3Credentials(string owner)
    {
      List<SnmpCredentialsV3> snmpV3Credentials = new List<SnmpCredentialsV3>();
      try
      {
        snmpV3Credentials = new CredentialManager().GetCredentials<SnmpCredentialsV3>(owner).ToList<SnmpCredentialsV3>();
      }
      catch (Exception ex)
      {
        CoreBusinessLayerService.log.Error((object) "Unhandled exception occured when loading shared credentials.", ex);
      }
      return snmpV3Credentials;
    }

    public List<UsernamePasswordCredential> GetSharedWmiCredentials(string owner)
    {
      List<UsernamePasswordCredential> sharedWmiCredentials = new List<UsernamePasswordCredential>();
      try
      {
        sharedWmiCredentials = new CredentialManager().GetCredentials<UsernamePasswordCredential>(owner).ToList<UsernamePasswordCredential>();
      }
      catch (Exception ex)
      {
        CoreBusinessLayerService.log.Error((object) "Unhandled exception occured when loading shared credentials.", ex);
      }
      return sharedWmiCredentials;
    }

    public Dictionary<string, int> GetElementsManagedCount()
    {
      return LicenseSaturationLogic.GetElementsManagedCount();
    }

    public Dictionary<string, int> GetAlreadyManagedElementCount(
      List<DiscoveryResultBase> discoveryResults,
      IList<IDiscoveryPlugin> plugins)
    {
      Dictionary<string, int> source = new Dictionary<string, int>();
      foreach (IDiscoveryPlugin plugin in (IEnumerable<IDiscoveryPlugin>) plugins)
      {
        if (plugin is IGetManagedElements)
        {
          Dictionary<string, int> pluginAlreadyManaged = ((IGetManagedElements) plugin).GetAlreadyManagedElements(discoveryResults);
          source = pluginAlreadyManaged.Concat<KeyValuePair<string, int>>(source.Where<KeyValuePair<string, int>>((System.Func<KeyValuePair<string, int>, bool>) (kvp => !pluginAlreadyManaged.ContainsKey(kvp.Key)))).ToDictionary<KeyValuePair<string, int>, string, int>((System.Func<KeyValuePair<string, int>, string>) (v => v.Key.ToLower()), (System.Func<KeyValuePair<string, int>, int>) (v => v.Value));
        }
      }
      return source;
    }

    public Dictionary<string, int> GetAlreadyManagedElementCount(
      List<DiscoveryResultBase> discoveryResults)
    {
      IList<IDiscoveryPlugin> discoveryPlugins = DiscoveryHelper.GetOrderedDiscoveryPlugins();
      return this.GetAlreadyManagedElementCount(discoveryResults, discoveryPlugins);
    }

    public DiscoveryResultBase FilterIgnoredItems(DiscoveryResultBase discoveryResult)
    {
      return this.discoveryLogic.FilterIgnoredItems(discoveryResult);
    }

    public List<DiscoveryResult> ConvertScheduledDiscoveryResults(
      List<DiscoveryResult> scheduledResults)
    {
      List<DiscoveryResult> discoveryResultList = new ScheduledDiscoveryResultConvertor().ConvertScheduledDiscoveryResults(scheduledResults);
      foreach (DiscoveryResult result in discoveryResultList)
      {
        DiscoveryFilterResultByTechnology.FilterByPriority((DiscoveryResultBase) result, TechnologyManager.Instance);
        List<DiscoveryPluginResultBase> list = ((IEnumerable<DiscoveryPluginResultBase>) ((DiscoveryResultBase) result).PluginResults).ToList<DiscoveryPluginResultBase>();
        ((DiscoveryResultBase) result).PluginResults.Clear();
        foreach (DiscoveryPluginResultBase pluginResultBase in list)
          ((DiscoveryResultBase) result).PluginResults.Add(pluginResultBase.GetFilteredPluginResult());
      }
      return discoveryResultList;
    }

    public void RequestScheduledDiscoveryNetObjectStatusUpdateAsync()
    {
      DiscoveryNetObjectStatusManager.Instance.RequestUpdateAsync((Action) null, TimeSpan.Zero);
    }

    public void ImportDiscoveryResultForProfile(int profileID, bool deleteProfileAfterImport)
    {
      this.discoveryLogic.ImportDiscoveryResultForProfile(profileID, deleteProfileAfterImport);
    }

    public Guid ImportDiscoveryResultsForConfiguration(DiscoveryImportConfiguration importCfg)
    {
      Guid importID = Guid.NewGuid();
      ThreadPool.QueueUserWorkItem((WaitCallback) (callback =>
      {
        try
        {
          this.discoveryLogic.ImportDiscoveryResultsForConfiguration(importCfg, importID);
        }
        catch (Exception ex)
        {
          CoreBusinessLayerService.log.Error((object) "Error in ImportDiscoveryResultsForConfiguration", ex);
        }
      }));
      return importID;
    }

    public ValidationResult ValidateActiveDirectoryAccess(ActiveDirectoryAccess access)
    {
      if (access == null)
        throw new ArgumentNullException(nameof (access));
      if (access.Credential == null)
      {
        UsernamePasswordCredential credential = new CredentialManager().GetCredential<UsernamePasswordCredential>(access.CredentialID);
        access.Credential = credential;
      }
      return new ActiveDirectoryDiscovery(access.HostName, access.Credential).ValidateConnection();
    }

    public List<OrganizationalUnit> GetActiveDirectoryOrganizationUnits(ActiveDirectoryAccess access)
    {
      return this.GetActiveDirectoryDiscovery(access).GetAllOrganizationalUnits().ToList<OrganizationalUnit>();
    }

    public List<OrganizationalUnitCountOfComputers> GetCountOfComputers(ActiveDirectoryAccess access)
    {
      return this.GetActiveDirectoryDiscovery(access).GetCountOfStationsInAD();
    }

    private ActiveDirectoryDiscovery GetActiveDirectoryDiscovery(ActiveDirectoryAccess access)
    {
      if (access == null)
        throw new ArgumentNullException(nameof (access));
      if (access.Credential == null)
      {
        UsernamePasswordCredential credential = new CredentialManager().GetCredential<UsernamePasswordCredential>(access.CredentialID);
        access.Credential = credential;
      }
      return new ActiveDirectoryDiscovery(access.HostName, access.Credential);
    }

    public IPAddress GetHostAddress(string hostName, AddressFamily preferredAddressFamily)
    {
      GetHostAddressJobResult result = this.ExecuteJobAndGetResult<GetHostAddressJobResult>(GetHostAddressJob.CreateJobDescription(hostName, BusinessLayerSettings.Instance.TestJobTimeout), (CredentialBase) null, JobResultDataFormatType.Xml, "GetHostAddressJob", out string _);
      if (!((TestJobResult) result).Success)
        throw new ResolveHostAddressException("Can not resolve IP address for host " + hostName + ".");
      IPAddress hostAddress = CommonHelper.GetHostAddress(result.IpAddresses.Select<string, IPAddress>((System.Func<string, IPAddress>) (item => IPAddress.Parse(item))), preferredAddressFamily, (IPAddress) null);
      CoreBusinessLayerService.log.InfoFormat(string.Format("IPAddress for host {0} is {1}", (object) hostName, (object) hostAddress), Array.Empty<object>());
      return hostAddress;
    }

    public void CleanOneTimeJobResults()
    {
      foreach (IOneTimeJobCleanup ioneTimeJobCleanup in DiscoveryHelper.GetOrderedDiscoveryPlugins().OfType<IOneTimeJobCleanup>())
      {
        try
        {
          CoreBusinessLayerService.log.DebugFormat("Cleaning one time job results within plugin '{0}'", (object) ioneTimeJobCleanup.GetType());
          ioneTimeJobCleanup.CleanOneTimeJobResults();
        }
        catch (Exception ex)
        {
          CoreBusinessLayerService.log.Error((object) string.Format("Exception occured when cleaning one time job results within plugin {0}", (object) ioneTimeJobCleanup.GetType()), ex);
        }
      }
    }

    public Dictionary<string, Dictionary<int, string>> GetEngines() => EngineDAL.GetEngines();

    public void DeleteEngine(int engineID) => EngineDAL.DeleteEngine(engineID);

    [Obsolete("Core-Split cleanup. If you need this member please contact Core team", true)]
    public string GetCurrentEngineIPAddress()
    {
      return EngineDAL.GetEngineIpAddressByServerName(Environment.MachineName);
    }

    public DataTable GetEventTypesTable() => EventsWebDAL.GetEventTypesTable();

    public DataTable GetEventsTable(GetEventsParameter param) => EventsWebDAL.GetEventsTable(param);

    public DataTable GetEvents(GetEventsParameter param) => EventsWebDAL.GetEvents(param);

    public void AcknowledgeEvents(List<int> events) => EventsWebDAL.AcknowledgeEvents(events);

    public DataTable GetEventSummaryTable(
      int netObjectID,
      string netObjectType,
      DateTime fromDate,
      DateTime toDate,
      List<int> limitationIDs)
    {
      return EventsWebDAL.GetEventSummaryTable(netObjectID, netObjectType, fromDate, toDate, limitationIDs);
    }

    public bool Blow(bool generateException, string exceptionType, string message)
    {
      if (!generateException)
        return true;
      Exception exception;
      if (CoreBusinessLayerService.TryGetException(exceptionType, message, out exception))
        throw exception;
      return false;
    }

    private static bool TryGetException(
      string exceptionType,
      string message,
      out Exception exception)
    {
      try
      {
        if (string.Equals(exceptionType, "SolarWinds.Orion.Core.Common.CoreFaultContract", StringComparison.OrdinalIgnoreCase))
        {
          exception = (Exception) MessageUtilities.NewFaultException<CoreFaultContract>((Exception) new ApplicationException(message));
        }
        else
        {
          System.Type type = System.Type.GetType(exceptionType);
          exception = (Exception) Activator.CreateInstance(type, (object) message);
        }
        return true;
      }
      catch (Exception ex)
      {
        CoreBusinessLayerService.log.Error((object) "Failed to fake exception. Returning false.", ex);
        exception = (Exception) null;
        return false;
      }
    }

    public List<JobEngineInfo> EnumerateJobEngines() => JobEngineDAL.EnumerateJobEngine();

    public JobEngineInfo GetEngine(int engineId) => JobEngineDAL.GetEngine(engineId);

    [Obsolete("Use GetEngine method. Obsolete since Core 2018.2 Pacman.")]
    public JobEngineInfo GetEngineWithPollingSettings(int engineId)
    {
      return JobEngineDAL.GetEngineWithPollingSettings(engineId);
    }

    public int GetEngineIdForNetObject(string netObject)
    {
      return JobEngineDAL.GetEngineIdForNetObject(netObject);
    }

    internal IIndication RemoveNetObjectInternal(string netobject)
    {
      if (string.IsNullOrEmpty(netobject))
        return (IIndication) null;
      string[] strArray = netobject.Split(':');
      if (strArray.Length != 2)
        return (IIndication) null;
      try
      {
        int result;
        if (int.TryParse(strArray[1], out result))
        {
          if (strArray[0].Equals("N", StringComparison.OrdinalIgnoreCase))
          {
            foreach (Volume nodeVolume in (Collection<int, Volume>) VolumeDAL.GetNodeVolumes(result))
              this.DeleteVolume(nodeVolume);
            this.DeleteNode(result);
          }
          else if (strArray[0].Equals("V", StringComparison.OrdinalIgnoreCase))
          {
            Volume volume = VolumeDAL.GetVolume(result);
            if (volume != null)
              this.DeleteVolume(volume);
          }
          else if (strArray[0].Equals("I", StringComparison.OrdinalIgnoreCase))
          {
            if (this.AreInterfacesSupported)
              return (IIndication) new InterfaceIndication((IndicationType) 1, new Interface()
              {
                InterfaceID = result
              });
          }
        }
      }
      catch (Exception ex)
      {
        CoreBusinessLayerService.log.Error((object) ex);
        throw;
      }
      return (IIndication) null;
    }

    public void RemoveNetObjects(List<string> netObjectIds)
    {
      List<IIndication> indications = new List<IIndication>(netObjectIds.Count);
      netObjectIds.ForEach((Action<string>) (nodeId =>
      {
        IIndication iindication = this.RemoveNetObjectInternal(nodeId);
        if (iindication == null)
          return;
        indications.Add(iindication);
      }));
      if (indications.Count <= 0)
        return;
      IndicationPublisher.CreateV3().ReportIndications((IEnumerable<IIndication>) indications);
    }

    public void RemoveNetObject(string netObjectId)
    {
      IIndication iindication = this.RemoveNetObjectInternal(netObjectId);
      if (iindication == null)
        return;
      IndicationPublisher.CreateV3().ReportIndication(iindication);
    }

    private static void BizLayerErrorHandler(Exception ex)
    {
      CoreBusinessLayerService.log.Error((object) "Exception occurred when communication with VIM Business Layer");
    }

    public void PollNow(string netObjectId)
    {
      using (IPollingControllerServiceHelper instance = PollingController.GetInstance())
        ((IPollingControllerService) instance).PollNow(netObjectId);
    }

    public void JobNowByJobKey(string netObjectId, string jobKey)
    {
      JobExecutionCondition executionCondition = new JobExecutionCondition()
      {
        EntityIdentifier = netObjectId,
        JobKey = jobKey
      };
      using (IPollingControllerServiceHelper instance = PollingController.GetInstance())
        ((IPollingControllerService) instance).JobNow(executionCondition);
    }

    public string PollNodeNow(string netObjectId)
    {
      string empty = string.Empty;
      using (IPollingControllerServiceHelper instance = PollingController.GetInstance())
        ((IPollingControllerService) instance).PollNow(netObjectId);
      return empty;
    }

    public void CancelNow(string netObjectId)
    {
      using (IPollingControllerServiceHelper instance = PollingController.GetInstance())
        ((IPollingControllerService) instance).CancelJob(new JobExecutionCondition()
        {
          JobType = (PollerJobType) 7,
          EntityIdentifier = netObjectId
        });
    }

    public void Rediscover(string netObjectId)
    {
      using (IPollingControllerServiceHelper instance = PollingController.GetInstance())
        ((IPollingControllerService) instance).RediscoverNow(netObjectId);
    }

    public void RefreshSettingsFromDatabase() => new SettingsToRegistry().Synchronize();

    public void ApplyPollingIntervals(
      int nodePollInterval,
      int interfacePollInterval,
      int volumePollInterval,
      int rediscoveryInterval)
    {
      CoreBusinessLayerService.log.ErrorFormat("NotImplemented ApplyPollingIntervals", Array.Empty<object>());
    }

    public void ApplyStatPollingIntervals(
      int nodePollInterval,
      int interfacePollInterval,
      int volumePollInterval)
    {
      CoreBusinessLayerService.log.ErrorFormat("NotImplemented ApplyStatPollingIntervals", Array.Empty<object>());
    }

    public int UpdateNodesPollingEngine(int engineId, int[] nodeIds)
    {
      return JobEngineDAL.UpdateNodesPollingEngine(engineId, nodeIds);
    }

    public string GetLicenseSWID()
    {
      IProductLicense[] activeLicenses = ((ILicenseManager) LicenseManager.GetInstance()).GetActiveLicenses(true);
      return (((IEnumerable<IProductLicense>) activeLicenses).FirstOrDefault<IProductLicense>((System.Func<IProductLicense, bool>) (l => l.LicenseType != 1 && l.LicenseType > 0)) ?? ((IEnumerable<IProductLicense>) activeLicenses).FirstOrDefault<IProductLicense>()).CustomerId;
    }

    public bool ActivateOfflineLicense(string fileNamePath)
    {
      if (string.IsNullOrEmpty(fileNamePath))
        throw new ArgumentNullException(nameof (fileNamePath));
      if (!System.IO.File.Exists(fileNamePath))
      {
        CoreBusinessLayerService.log.DebugFormat("File {0} doesn't exists.", (object) fileNamePath);
        return false;
      }
      string str = System.IO.File.ReadAllText(fileNamePath);
      using (IInformationServiceProxy2 iinformationServiceProxy2 = ((IInformationServiceProxyCreator) SwisConnectionProxyPool.GetSystemCreator()).Create())
      {
        PropertyBag propertyBag = iinformationServiceProxy2.Invoke<PropertyBag>("Orion.Licensing.Licenses", "ActivateOffline", new object[1]
        {
          (object) str
        });
        return ((Dictionary<string, object>) propertyBag).ContainsKey("Success") && Convert.ToBoolean(((Dictionary<string, object>) propertyBag)["Success"]);
      }
    }

    public Oid GetOid(string oidValue) => this.mibDAL.GetOid(oidValue);

    public bool IsMibDatabaseAvailable() => this.mibDAL.IsMibDatabaseAvailable();

    public Oids GetChildOids(string parentOid) => this.mibDAL.GetChildOids(parentOid);

    public MemoryStream GetIcon(string oid) => this.mibDAL.GetIcon(oid);

    public Dictionary<string, MemoryStream> GetIcons() => this.mibDAL.GetIcons();

    public Oids GetSearchingOidsByDescription(string searchCriteria, string searchMIBsCriteria)
    {
      return this.mibDAL.GetSearchingOidsByDescription(searchCriteria, searchMIBsCriteria);
    }

    public Oids GetSearchingOidsByName(string searchCriteria)
    {
      return this.mibDAL.GetSearchingOidsByName(searchCriteria);
    }

    public void CancelRunningCommand() => this.mibDAL.CancelRunningCommand();

    public bool IsModuleInstalled(string moduleTag)
    {
      return ModulesCollector.IsModuleInstalled(moduleTag);
    }

    public bool IsModuleInstalledbyTabName(string moduleTabName)
    {
      return ModulesCollector.IsModuleInstalledbyTabName(moduleTabName);
    }

    public List<ModuleInfo> GetInstalledModules() => ModulesCollector.GetInstalledModules();

    public List<ModuleLicenseInfo> GetModuleLicenseInformation()
    {
      return ModuleLicenseInfoProvider.GetModuleLicenseInformation();
    }

    public Version GetModuleVersion(string moduleTag)
    {
      return ModulesCollector.GetModuleVersion(moduleTag);
    }

    public List<ModuleLicenseSaturationInfo> GetModuleSaturationInformation()
    {
      return LicenseSaturationLogic.GetModulesSaturationInfo(new int?(SolarWinds.Orion.Core.BusinessLayer.Settings.LicenseSaturationPercentage));
    }

    public List<SolarWinds.Orion.Core.Common.Models.Node> GetNetworkDevices(
      CorePageType pageType,
      List<int> limitationIDs)
    {
      return NetworkDeviceDAL.Instance.GetNetworkDevices(pageType, limitationIDs);
    }

    public Dictionary<int, string> GetNetworkDeviceNamesForPage(
      CorePageType pageType,
      List<int> limitationIDs)
    {
      return NetworkDeviceDAL.Instance.GetNetworkDeviceNamesForPage(pageType, limitationIDs);
    }

    public Dictionary<int, string> GetDeviceNamesForPage(
      CorePageType pageType,
      List<int> limitationIDs,
      bool includeBasic)
    {
      return NetworkDeviceDAL.Instance.GetNetworkDeviceNamesForPage(pageType, limitationIDs, includeBasic);
    }

    public Dictionary<string, string> GetNetworkDeviceTypes(List<int> limitationIDs)
    {
      return NetworkDeviceDAL.Instance.GetNetworkDeviceTypes(limitationIDs);
    }

    public List<string> GetAllVendors(List<int> limitationIDs)
    {
      return NetworkDeviceDAL.Instance.GetAllVendors(limitationIDs);
    }

    private static MaintenanceRenewalItem DalToWfc(MaintenanceRenewalItemDAL dal)
    {
      return dal == null ? (MaintenanceRenewalItem) null : new MaintenanceRenewalItem(dal.Id, dal.Title, dal.Description, dal.CreatedAt, dal.Ignored, dal.Url, dal.AcknowledgedAt, dal.AcknowledgedBy, dal.ProductTag, dal.DateReleased, dal.NewVersion);
    }

    public MaintenanceRenewalItem GetMaintenanceRenewalNotificationItem(Guid renewalId)
    {
      CoreBusinessLayerService.log.Debug((object) "Sending request for MaintenanceRenewalItemDAL.GetItemById.");
      try
      {
        return CoreBusinessLayerService.DalToWfc(MaintenanceRenewalItemDAL.GetItemById(renewalId));
      }
      catch (Exception ex)
      {
        CoreBusinessLayerService.log.Error((object) ("Error obtaining maintenance renewal notification item: " + ex.ToString()));
        throw new Exception(string.Format(Resources.LIBCODE_JM0_22, (object) renewalId));
      }
    }

    public List<MaintenanceRenewalItem> GetMaintenanceRenewalNotificationItems(bool includeIgnored)
    {
      CoreBusinessLayerService.log.Debug((object) "Sending request for MaintenanceRenewalItemDAL.GetItems.");
      try
      {
        List<MaintenanceRenewalItem> notificationItems = new List<MaintenanceRenewalItem>();
        foreach (MaintenanceRenewalItemDAL dal in (IEnumerable<MaintenanceRenewalItemDAL>) MaintenanceRenewalItemDAL.GetItems(new MaintenanceRenewalFilter(true, includeIgnored, (string) null)))
          notificationItems.Add(CoreBusinessLayerService.DalToWfc(dal));
        return notificationItems;
      }
      catch (Exception ex)
      {
        CoreBusinessLayerService.log.Error((object) ("Error when obtaining maintenance renewals notification items: " + ex.ToString()));
        throw new Exception(Resources.LIBCODE_JM0_23);
      }
    }

    public MaintenanceRenewalItem GetLatestMaintenanceRenewalItem()
    {
      CoreBusinessLayerService.log.Debug((object) "Sending request for MaintenanceRenewalItemDAL.GetLatestItem.");
      try
      {
        return CoreBusinessLayerService.DalToWfc(MaintenanceRenewalItemDAL.GetLatestItem((NotificationItemFilter) new MaintenanceRenewalFilter(true, false, (string) null)));
      }
      catch (Exception ex)
      {
        CoreBusinessLayerService.log.Error((object) ("Error when obtaining maintenance renewals notification item: " + ex.ToString()));
        throw new Exception(Resources.LIBCODE_JM0_23);
      }
    }

    public MaintenanceRenewalsCheckStatus GetMaintenanceRenewalsCheckStatus()
    {
      CoreBusinessLayerService.log.Debug((object) "Sending request for MaintenanceRenewalsCheckStatusDAL.GetCheckStatus.");
      try
      {
        MaintenanceRenewalsCheckStatusDAL checkStatus = MaintenanceRenewalsCheckStatusDAL.GetCheckStatus();
        return checkStatus == null ? (MaintenanceRenewalsCheckStatus) null : new MaintenanceRenewalsCheckStatus(checkStatus.LastUpdateCheck, checkStatus.NextUpdateCheck);
      }
      catch (Exception ex)
      {
        CoreBusinessLayerService.log.Error((object) ("Error obtaining maintenance renewals status: " + ex.ToString()));
        throw new Exception(Resources.LIBCODE_JM0_20);
      }
    }

    public void ForceMaintenanceRenewalsCheck()
    {
      CoreBusinessLayerService.log.Debug((object) "Sending request for CoreHelper.CheckMaintenanceRenewals.");
      try
      {
        CoreHelper.CheckMaintenanceRenewals(false);
      }
      catch (Exception ex)
      {
        CoreBusinessLayerService.log.Error((object) ("Error while checking maintenance renewals: " + ex.ToString()));
        throw new Exception(Resources.LIBCODE_JM0_21);
      }
    }

    public Nodes GetAllNodes() => NodeBLDAL.GetNodes();

    public List<int> GetSortedNodeIDs() => NodeBLDAL.GetSortedNodeIDs();

    public List<string> GetNodeFields() => NodeBLDAL.GetFields();

    public List<string> GetCustomPropertyFields(bool includeMemo)
    {
      return new List<string>((IEnumerable<string>) CustomPropertyMgr.GetPropNamesForTable("NodesCustomProperties", includeMemo));
    }

    public Dictionary<string, string> GetVendors() => NodeBLDAL.GetVendors();

    public void DeleteNode(int nodeId)
    {
      SolarWinds.Orion.Core.Common.Models.Node node = this.GetNode(nodeId);
      if (node == null)
        return;
      Dictionary<string, object> nodeInfo = CoreBusinessLayerService.GetNodeInfo(node);
      NodeBLDAL.DeleteNode(node);
      NodeSettingsDAL.DeleteNodeSettings(nodeId);
      NodeNotesDAL.DeleteNodeNotes(nodeId);
      NodeIndication nodeIndication = new NodeIndication((IndicationType) 1, new SolarWinds.Orion.Core.Common.Models.Node()
      {
        Id = nodeId,
        Caption = nodeInfo["DisplayName"].ToString(),
        IpAddress = node.IpAddress,
        Status = node.Status
      });
      foreach (KeyValuePair<string, object> keyValuePair in nodeInfo)
        ((Dictionary<string, object>) nodeIndication.NodeProperties)[keyValuePair.Key] = keyValuePair.Value;
      IndicationPublisher.CreateV3().ReportIndication((IIndication) nodeIndication);
    }

    private static Dictionary<string, object> GetNodeInfo(SolarWinds.Orion.Core.Common.Models.Node node)
    {
      return new SwisEntityHelper(CoreBusinessLayerService.CreateProxy()).GetProperties("Orion.Nodes", node.ID, new string[2]
      {
        "DisplayName",
        "Uri"
      });
    }

    [Obsolete("This is a temporary solution. Don't use this method in modules")]
    public SolarWinds.Orion.Core.Common.Models.Node InsertNodeWithFaultContract(
      SolarWinds.Orion.Core.Common.Models.Node node,
      bool allowDuplicates,
      bool reportIndication)
    {
      try
      {
        return this.InsertNode(node, allowDuplicates, reportIndication);
      }
      catch (LicenseException ex)
      {
        throw MessageUtilities.NewFaultException<CoreFaultContract>((Exception) ex);
      }
    }

    [Obsolete("This is a temporary solution. Don't use this method in modules")]
    public SolarWinds.Orion.Core.Common.Models.Node InsertNodeWithFaultContract(
      SolarWinds.Orion.Core.Common.Models.Node node,
      bool allowDuplicates)
    {
      return this.InsertNodeWithFaultContract(node, allowDuplicates, true);
    }

    [Obsolete("This is a temporary solution. Don't use this method in modules")]
    public SolarWinds.Orion.Core.Common.Models.Node InsertNodeWithFaultContract(SolarWinds.Orion.Core.Common.Models.Node node)
    {
      return this.InsertNode(node, false);
    }

    public SolarWinds.Orion.Core.Common.Models.Node InsertNode(
      SolarWinds.Orion.Core.Common.Models.Node node,
      bool allowDuplicates,
      bool reportIndication)
    {
      int maxElementCount = new FeatureManager().GetMaxElementCount(WellKnownElementTypes.Nodes);
      if (NodeBLDAL.GetNodeCount() >= maxElementCount)
        throw LicenseException.FromElementsExceeded(maxElementCount);
      node = NodeBLDAL.InsertNode(node, allowDuplicates);
      NodeBLDAL.PopulateWebCommunityStrings();
      if (reportIndication)
        IndicationPublisher.CreateV3().ReportIndication((IIndication) new NodeIndication((IndicationType) 0, node));
      return node;
    }

    public SolarWinds.Orion.Core.Common.Models.Node InsertNode(SolarWinds.Orion.Core.Common.Models.Node node, bool allowDuplicates)
    {
      return this.InsertNode(node, allowDuplicates, true);
    }

    public SolarWinds.Orion.Core.Common.Models.Node InsertNode(SolarWinds.Orion.Core.Common.Models.Node node)
    {
      return this.InsertNode(node, false);
    }

    public SolarWinds.Orion.Core.Common.Models.Node InsertNodeAndGenericPollers(SolarWinds.Orion.Core.Common.Models.Node node)
    {
      return this.nodeHelper.InsertNodeAndGenericPollers(node);
    }

    public void AddPollersForNode(int nodeId, string[] newPollers)
    {
      SolarWinds.Orion.Core.Common.Models.Node node = nodeId > 0 ? NodeDAL.GetNode(nodeId) : throw new ArgumentException("Argument nodeId cannot be less then or equal zero");
      if (node == null)
        throw new ArgumentException("Node doesn't exist");
      if (newPollers == null || newPollers.Length == 0)
        return;
      this.nodeHelper.AddPollersForNode(node, newPollers);
    }

    public void AddBasicPollersForNode(int nodeId, NodeSubType nodeSubtType)
    {
      this.nodeHelper.AddBasicPollersForNode((nodeId > 0 ? NodeDAL.GetNode(nodeId) : throw new ArgumentException(nameof (nodeId))) ?? throw new ArgumentException("Node doesn't exist"));
    }

    public void RemoveBasicPollersForNode(int nodeId, NodeSubType nodeSubType)
    {
      CoreBusinessLayerService.log.DebugFormat("Removing basic pollers for NodeID = {0}, SubType = {1} ....", (object) nodeId, (object) nodeSubType);
      PollersDAL pollersDal = new PollersDAL();
      List<string> stringList = new List<string>();
      stringList.AddRange((IEnumerable<string>) NodeResponseTimeIcmpPoller.SubPollerTypes);
      if (nodeSubType != 2)
      {
        if (nodeSubType == 1)
        {
          stringList.Add("N.Details.SNMP.Generic");
          stringList.Add("N.Uptime.SNMP.Generic");
        }
        else if (nodeSubType == 3)
        {
          stringList.Add(NodeDetailsPollerGeneric.PollerType);
          stringList.Add(NodeUptimePollerGeneric.PollerType);
        }
      }
      int num = nodeId;
      string[] array = stringList.ToArray();
      pollersDal.Delete("N", num, array);
      CoreBusinessLayerService.log.DebugFormat("Basic pollers count = {0}, removed for NodeID = {1}, SubType = {2}", (object) stringList.Count, (object) nodeId, (object) nodeSubType);
    }

    public void UpdateNode(SolarWinds.Orion.Core.Common.Models.Node node)
    {
      DateTime utcNow = DateTime.UtcNow;
      if (utcNow > node.UnManageFrom && utcNow < node.UnManageUntil)
      {
        node.UnManaged = true;
        node.Status = "9";
        node.PolledStatus = 9;
        node.GroupStatus = "Unmanaged.gif";
        node.StatusDescription = "Node status is Unmanaged.";
      }
      else if (node.Status.Trim() == "9")
      {
        node.UnManaged = false;
        node.Status = "0";
        node.PolledStatus = 0;
        node.GroupStatus = "Unknown.gif";
        node.StatusDescription = "Node status is Unknown.";
      }
      if (node.UnPluggable)
      {
        node.Status = "11";
        node.PolledStatus = 11;
        node.GroupStatus = "External.gif";
        node.StatusDescription = "Node status is External.";
      }
      else if (node.Status.Trim() == "11")
      {
        node.Status = "0";
        node.PolledStatus = 0;
        node.GroupStatus = "Unknown.gif";
        node.StatusDescription = "Node status is Unknown.";
      }
      SolarWinds.Orion.Core.Common.Models.Node node1 = NodeBLDAL.GetNode(node.Id);
      NodeBLDAL.UpdateNode(node);
      NodeBLDAL.PopulateWebCommunityStrings();
      NodeIndication nodeIndication = new NodeIndication((IndicationType) 2, node, node1);
      if (((Dictionary<string, object>) nodeIndication.ChangedProperties).ContainsKey("ObjectSubType"))
      {
        bool flag = CoreBusinessLayerService.WmiCompatibleNodeSubTypes.Contains(node.NodeSubType) && CoreBusinessLayerService.WmiCompatibleNodeSubTypes.Contains(node1.NodeSubType);
        if (!flag)
        {
          foreach (Volume nodeVolume in (Collection<int, Volume>) VolumeDAL.GetNodeVolumes(node.Id))
            VolumeDAL.DeleteVolume(nodeVolume);
        }
        PollersDAL pollersDal = new PollersDAL();
        pollersDal.Delete(new PollerAssignment("N", node.ID, "%SNMP%"));
        if (flag)
        {
          pollersDal.Delete(new PollerAssignment("N", node.ID, "N.Uptime.%"));
          pollersDal.Delete(new PollerAssignment("N", node.ID, "N.Details.%"));
        }
        else
          pollersDal.Delete(new PollerAssignment("N", node.ID, "%WMI%"));
        pollersDal.Delete(new PollerAssignment("N", node.ID, "%Agent%"));
        if (node.NodeSubType == 4)
          pollersDal.Delete(new PollerAssignment("N", node.ID, "%ICMP%"));
        if (!(node.EntityType ?? "").Contains(".VIM."))
          NodeBLDAL.UpdateNodeProperty((IDictionary<string, object>) new Dictionary<string, object>()
          {
            {
              "CPULoad",
              (object) -2
            },
            {
              "TotalMemory",
              (object) -2
            },
            {
              "MemoryUsed",
              (object) -2
            },
            {
              "PercentMemoryUsed",
              (object) -2
            },
            {
              "BufferNoMemThisHour",
              (object) -2
            },
            {
              "BufferNoMemToday",
              (object) -2
            },
            {
              "BufferSmMissThisHour",
              (object) -2
            },
            {
              "BufferSmMissToday",
              (object) -2
            },
            {
              "BufferMdMissThisHour",
              (object) -2
            },
            {
              "BufferMdMissToday",
              (object) -2
            },
            {
              "BufferBgMissThisHour",
              (object) -2
            },
            {
              "BufferBgMissToday",
              (object) -2
            },
            {
              "BufferLgMissThisHour",
              (object) -2
            },
            {
              "BufferLgMissToday",
              (object) -2
            },
            {
              "BufferHgMissThisHour",
              (object) -2
            },
            {
              "BufferHgMissToday",
              (object) -2
            }
          }, node.ID);
        NodeBLDAL.UpdateNodeProperty((IDictionary<string, object>) new Dictionary<string, object>()
        {
          {
            "LastBoot",
            (object) null
          },
          {
            "SystemUpTime",
            (object) null
          },
          {
            "LastSystemUpTimePollUtc",
            (object) null
          }
        }, node.ID);
      }
      if (!nodeIndication.AnyChange)
        return;
      IndicationPublisher.CreateV3().ReportIndication((IIndication) nodeIndication);
    }

    public SolarWinds.Orion.Core.Common.Models.Node GetNode(int nodeId)
    {
      return NodeBLDAL.GetNode(nodeId);
    }

    public bool IsNodeWireless(int nodeId) => NodeBLDAL.IsNodeWireless(nodeId);

    public bool IsNodeEnergyWise(int nodeId) => NodeBLDAL.IsNodeEnergyWise(nodeId);

    public SolarWinds.Orion.Core.Common.Models.Node GetNodeWithOptions(
      int nodeId,
      bool getInterfaces,
      bool getVolumes)
    {
      return NodeBLDAL.GetNodeWithOptions(nodeId, getInterfaces, getVolumes);
    }

    public Resources ListResources(SolarWinds.Orion.Core.Common.Models.Node node)
    {
      return ResourceLister.ListResources(node);
    }

    public Guid BeginListResources(SolarWinds.Orion.Core.Common.Models.Node node)
    {
      return ResourceLister.BeginListResources(node);
    }

    public Guid BeginCoreListResources(SolarWinds.Orion.Core.Common.Models.Node node, bool includeInterfaces)
    {
      return ResourceLister.BeginListResources(node, includeInterfaces);
    }

    public ListResourcesStatus GetListResourcesStatus(Guid listResourcesOperationId)
    {
      return ResourceLister.GetListResourcesStatus(listResourcesOperationId);
    }

    public float GetAvailability(int nodeID, DateTime startDate, DateTime endDate)
    {
      return NodeBLDAL.GetAvailability(nodeID, startDate, endDate);
    }

    public Dictionary<string, int> GetValuesAndCountsForNodePropertyFiltered(
      string property,
      string accountId,
      Dictionary<string, object> filters)
    {
      return NodeBLDAL.GetValuesAndCountsForPropertyFiltered(property, accountId, filters);
    }

    public Dictionary<string, int> GetValuesAndCountsForNodeProperty(
      string property,
      string accountId)
    {
      return NodeBLDAL.GetValuesAndCountsForProperty(property, accountId);
    }

    public Dictionary<string, int> GetCultureSpecificValuesAndCountsForNodeProperty(
      string property,
      string accountId,
      CultureInfo culture)
    {
      return NodeBLDAL.GetValuesAndCountsForProperty(property, accountId, culture);
    }

    public Nodes GetNodesFiltered(
      Dictionary<string, object> filterValues,
      bool includeInterfaces,
      bool includeVolumes)
    {
      return NodeBLDAL.GetNodesFiltered(filterValues, includeInterfaces, includeVolumes);
    }

    public Nodes GetNodesByIds(int[] nodeIds) => NodeBLDAL.GetNodesByIds(nodeIds);

    public Dictionary<string, string> GetVendorIconFileNames()
    {
      return NodeBLDAL.GetVendorIconFileNames();
    }

    public List<string> GetNodeDistinctValuesForField(string fieldName)
    {
      return NodeBLDAL.GetNodeDistinctValuesForField(fieldName);
    }

    public NodeHardwareType GetNodeHardwareType(int nodeId)
    {
      return NodeBLDAL.GetNodeHardwareType(nodeId);
    }

    public void BulkUpdateNodePollingInterval(int pollInterval, int engineId)
    {
      NodeBLDAL.BulkUpdateNodePollingInterval(pollInterval, engineId);
    }

    public Dictionary<string, object> GetNodeCustomProperties(
      int nodeId,
      ICollection<string> properties)
    {
      return NodeBLDAL.GetNodeCustomProperties(nodeId, properties);
    }

    public DataTable GetPagebleNodes(
      string property,
      string type,
      string val,
      string column,
      string direction,
      int number,
      int size,
      string searchText)
    {
      return NodeBLDAL.GetPagebleNodes(property, type, val, column, direction, number, size, searchText);
    }

    public int GetNodesCount(string property, string type, string val, string searchText)
    {
      return NodeBLDAL.GetNodesCount(property, type, val, searchText);
    }

    public DataTable GetGroupsByNodeProperty(string property, string propertyType)
    {
      return NodeBLDAL.GetGroupsByNodeProperty(property, propertyType);
    }

    public void ReflowAllNodeChildStatus()
    {
      NodeChildStatusParticipationDAL.ReflowAllNodeChildStatus();
    }

    public string ResolveNodeNameByIP(string ipAddress)
    {
      return this.ExecuteJobAndGetResult<ResolveHostNameByIpJobResult>(ResolveHostNameByIpJob.CreateJobDescription(ipAddress, BusinessLayerSettings.Instance.TestJobTimeout), (CredentialBase) null, JobResultDataFormatType.Xml, "ResolveHostNameByIpJob", out string _).HostName;
    }

    public DataTable GetNodeCPUsByPercentLoad(int nodeId, int pageNumber, int pageSize)
    {
      return NodeBLDAL.GetNodeCPUsByPercentLoad(nodeId, pageNumber, pageSize);
    }

    public DataTable GetNodesCpuIndexCounts(List<string> nodeIds)
    {
      return NodeBLDAL.GetNodesCpuIndexCounts(nodeIds);
    }

    public bool AddNodeNote(
      int nodeId,
      string accountId,
      string note,
      DateTime modificationDateTime)
    {
      return NodeBLDAL.AddNodeNote(nodeId, accountId, note, modificationDateTime);
    }

    public NodeNotesPage GetNodeNotes(PageableNodeNoteRequest request)
    {
      return new NodeNotesDAL().GetNodeNotes(request);
    }

    public void UpdateSpecificSettingForAllNodes(
      string settingName,
      string settingValue,
      string whereClause)
    {
      NodeSettingsDAL.UpdateSpecificSettingForAllNodes(settingName, settingValue, whereClause);
    }

    public int GetAvailableNotificationItemsCountByType(Guid typeId)
    {
      CoreBusinessLayerService.log.Debug((object) "Sending request for NotificationItemDAL.GetNotificationsCountByType.");
      try
      {
        return NotificationItemDAL.GetNotificationsCountByType(typeId, new NotificationItemFilter(false, false));
      }
      catch (Exception ex)
      {
        CoreBusinessLayerService.log.Error((object) ("Can't get notification items count by type: " + ex.ToString()));
        throw new Exception(string.Format(Resources.LIBCODE_JM0_8, (object) typeId));
      }
    }

    public Dictionary<Guid, int> GetAvailableNotificationItemsCounts()
    {
      CoreBusinessLayerService.log.Debug((object) "Sending request for NotificationItemDAL.GetNotificationsCounts.");
      try
      {
        return NotificationItemDAL.GetNotificationsCounts();
      }
      catch (Exception ex)
      {
        CoreBusinessLayerService.log.Error((object) ("Can't get notification items count for all types: " + (object) ex));
        throw new Exception(Resources.LIBCODE_JM0_9);
      }
    }

    public void IgnoreNotificationItem(Guid notificationId)
    {
      CoreBusinessLayerService.log.Debug((object) "Sending request for NotificationItemDAL.IgnoreItem.");
      try
      {
        NotificationItemDAL.IgnoreItem(notificationId);
      }
      catch (Exception ex)
      {
        CoreBusinessLayerService.log.Error((object) ("Can't ignore notification item: " + ex.ToString()));
        throw new Exception(string.Format(Resources.LIBCODE_JM0_10, (object) notificationId));
      }
    }

    public void IgnoreNotificationItems(List<Guid> notificationIds)
    {
      CoreBusinessLayerService.log.Debug((object) "Sending request for NotificationItemDAL.IgnoreItems.");
      try
      {
        NotificationItemDAL.IgnoreItems((ICollection<Guid>) notificationIds);
      }
      catch (Exception ex)
      {
        CoreBusinessLayerService.log.Error((object) ("Can't ignore multiple notification items: " + ex.ToString()));
        throw new Exception(Resources.LIBCODE_JM0_11);
      }
    }

    public void AcknowledgeNotificationItem(
      Guid notificationId,
      string byAccountId,
      DateTime createdBefore)
    {
      CoreBusinessLayerService.log.Debug((object) "Sending request for .NotificationItemDAL.AcknowledgeItem.");
      try
      {
        NotificationItemDAL.AcknowledgeItem(notificationId, byAccountId, createdBefore);
      }
      catch (Exception ex)
      {
        CoreBusinessLayerService.log.Error((object) ("Can't acknowledge notification item: " + ex.ToString()));
        throw new Exception(string.Format(Resources.LIBCODE_JM0_12, (object) notificationId, (object) byAccountId));
      }
    }

    public void AcknowledgeNotificationItemsByType(
      Guid typeId,
      string byAccountId,
      DateTime createdBefore)
    {
      CoreBusinessLayerService.log.Debug((object) "Sending request for .NotificationItemDAL.AcknowledgeItemsByType.");
      try
      {
        NotificationItemDAL.AcknowledgeItemsByType(typeId, byAccountId, createdBefore);
      }
      catch (Exception ex)
      {
        CoreBusinessLayerService.log.Error((object) ("Can't acknowledge notification items by type: " + ex.ToString()));
        throw new Exception(string.Format(Resources.LIBCODE_JM0_13, (object) typeId, (object) byAccountId));
      }
    }

    public void AcknowledgeAllNotificationItems(string byAccountId, DateTime createdBefore)
    {
      CoreBusinessLayerService.log.Debug((object) "Sending request for NotificationItemDAL.AcknowledgeAllItems.");
      try
      {
        NotificationItemDAL.AcknowledgeAllItems(byAccountId, createdBefore);
      }
      catch (Exception ex)
      {
        CoreBusinessLayerService.log.Error((object) ("Can't acknowledge all notification items: " + ex.ToString()));
        throw new Exception(string.Format(Resources.LIBCODE_JM0_14, (object) byAccountId));
      }
    }

    public List<HeaderNotificationItem> GetLatestNotificationItemsWithCount()
    {
      CoreBusinessLayerService.log.Debug((object) "Sending request for NotificationItemDAL.GetLatestItemByType.");
      try
      {
        List<HeaderNotificationItem> ret = new List<HeaderNotificationItem>();
        NotificationItemDAL.GetLatestItemsWithCount(new NotificationItemFilter(false, false), (Action<NotificationItemDAL, int>) ((item, count) => ret.Add(new HeaderNotificationItem(item.Id, item.Title, item.Description, item.CreatedAt, item.Ignored, item.TypeId, item.Url, item.AcknowledgedAt, item.AcknowledgedBy, count))));
        return ret;
      }
      catch (Exception ex)
      {
        CoreBusinessLayerService.log.Error((object) ("Can't obtain latest notification items: " + ex.ToString()));
        throw new Exception(Resources.LIBCODE_LK0_1);
      }
    }

    public List<NotificationItem> GetNotificationItemsByType(Guid typeId, bool includeIgnored)
    {
      CoreBusinessLayerService.log.Debug((object) "Sending request for NotificationItemDAL.GetItemsByTypeId.");
      try
      {
        ICollection<NotificationItemDAL> itemsByTypeId = NotificationItemDAL.GetItemsByTypeId(typeId, new NotificationItemFilter(true, includeIgnored));
        List<NotificationItem> notificationItemsByType = new List<NotificationItem>();
        foreach (NotificationItemDAL notificationItemDal in (IEnumerable<NotificationItemDAL>) itemsByTypeId)
        {
          NotificationItem notificationItem = new NotificationItem(notificationItemDal.Id, notificationItemDal.Title, notificationItemDal.Description, notificationItemDal.CreatedAt, notificationItemDal.Ignored, notificationItemDal.TypeId, notificationItemDal.Url, notificationItemDal.AcknowledgedAt, notificationItemDal.AcknowledgedBy);
          notificationItemsByType.Add(notificationItem);
        }
        return notificationItemsByType;
      }
      catch (Exception ex)
      {
        CoreBusinessLayerService.log.Error((object) ("Can't obtain latest notification item: " + ex.ToString()));
        throw new Exception(string.Format(Resources.LIBCODE_JM0_15, (object) typeId));
      }
    }

    public void InsertNotificationItem(NotificationItem item)
    {
      CoreBusinessLayerService.log.Debug((object) "Sending request for NotificationItemDAL.InsertNotificationItem.");
      try
      {
        NotificationItemDAL.Insert(item.Id, item.TypeId, item.Title, item.Description, item.Ignored, item.Url, item.AcknowledgedAt, item.AcknowledgedBy);
      }
      catch (Exception ex)
      {
        CoreBusinessLayerService.log.Error((object) ("Unable to insert notification item: " + ex.ToString()));
        throw new Exception(string.Format(Resources.LIBCODE_JM0_17, (object) item.Id, (object) item.TypeId));
      }
    }

    public void UpdateNotificationItem(NotificationItem item)
    {
      CoreBusinessLayerService.log.Debug((object) "Sending request for NotificationItemDAL.UpdateNotificationItem.");
      try
      {
        NotificationItemDAL.Update(item.Id, item.TypeId, item.Title, item.Description, item.Ignored, item.Url, item.CreatedAt, item.AcknowledgedAt, item.AcknowledgedBy);
      }
      catch (Exception ex)
      {
        CoreBusinessLayerService.log.Error((object) ("Unable to update notification item: " + ex.ToString()));
        throw new Exception(string.Format(Resources.LIBCODE_JM0_16, (object) item.Id, (object) item.TypeId));
      }
    }

    public bool DeleteNotificationItemById(Guid itemId)
    {
      CoreBusinessLayerService.log.Debug((object) "Sending request for NotificationItemDAL.Delete.");
      try
      {
        return NotificationItemDAL.Delete(itemId);
      }
      catch (Exception ex)
      {
        CoreBusinessLayerService.log.Error((object) ("Unable to delete notification item: " + ex.ToString()));
        throw new Exception(string.Format(Resources.LIBCODE_JM0_18, (object) itemId));
      }
    }

    public NotificationItem GetNotificationItemById(Guid itemId)
    {
      CoreBusinessLayerService.log.Debug((object) "Sending request for NotificationItemDAL.GetItemById.");
      try
      {
        NotificationItemDAL itemById = NotificationItemDAL.GetItemById<NotificationItemDAL>(itemId);
        return itemById != null ? new NotificationItem(itemById.Id, itemById.Title, itemById.Description, itemById.CreatedAt, itemById.Ignored, itemById.TypeId, itemById.Url, itemById.AcknowledgedAt, itemById.AcknowledgedBy) : (NotificationItem) null;
      }
      catch (Exception ex)
      {
        CoreBusinessLayerService.log.Error((object) ("Can't obtain notification item: " + ex.ToString()));
        throw new Exception(string.Format(Resources.LIBCODE_JM0_19, (object) itemId));
      }
    }

    public DataTable GetOrionMessagesTable(OrionMessagesFilter filter)
    {
      return OrionMessagesDAL.GetOrionMessagesTable(filter);
    }

    public PollerAssignment GetPoller(int pollerID) => PollerDAL.GetPoller(pollerID);

    public void DeletePoller(int pollerID) => PollerDAL.DeletePoller(pollerID);

    public int InsertPoller(PollerAssignment poller) => PollerDAL.InsertPoller(poller);

    public PollerAssignments GetPollersForNode(int nodeId) => PollerDAL.GetPollersForNode(nodeId);

    public PollerAssignments GetAllPollersForNode(int nodeId)
    {
      return PollerDAL.GetAllPollersForNode(nodeId, this.AreInterfacesSupported);
    }

    public int AddReportJob(ReportJobConfiguration configuration)
    {
      List<int> intList1 = new List<int>();
      ISwisConnectionProxyCreator creator = SwisConnectionProxyPool.GetCreator();
      List<int> intList2 = new FrequenciesDAL((IInformationServiceProxyCreator) creator).SaveFrequencies(new List<ISchedule>((IEnumerable<ISchedule>) configuration.Schedules));
      int num1 = ReportJobDAL.AddReportJob(configuration);
      int num2 = num1;
      ReportSchedulesDAL.ScheduleReportJobWithFrequencies(intList2, num2);
      new ActionsDAL((IInformationServiceProxyCreator) creator).SaveActionsForAssignments(num1, ((ActionEnviromentType) 1).ToString(), (IEnumerable<ActionDefinition>) configuration.Actions);
      configuration.ReportJobID = num1;
      this.AddBLScheduler(configuration);
      return configuration.ReportJobID;
    }

    public void ChangeReportJobStatus(int jobId, bool enable)
    {
      ReportJobDAL.ChangeReportJobStatus(jobId, enable);
      ReportJobConfiguration reportJob = ReportJobDAL.GetReportJob(jobId);
      this.RemoveBLScheduler(reportJob);
      this.AddBLScheduler(reportJob);
    }

    public void AssignJobsToReport(int reportId, List<int> schedulesIds)
    {
      CoreBusinessLayerService.log.DebugFormat("Assigning jobs for report", Array.Empty<object>());
      List<int> jobsIdsWithReport = ReportJobDAL.GetJobsIdsWithReport(reportId);
      jobsIdsWithReport.AddRange((IEnumerable<int>) schedulesIds);
      ReportJobDAL.AssignJobsToReport(reportId, schedulesIds);
      if (jobsIdsWithReport.Count == 0)
        return;
      foreach (ReportJobConfiguration jobsById in ReportJobDAL.GetJobsByIds(jobsIdsWithReport))
      {
        this.RemoveBLScheduler(jobsById);
        this.AddBLScheduler(jobsById);
      }
    }

    public void AssignJobsToReports(List<int> reportIds, List<int> schedulesIds)
    {
      CoreBusinessLayerService.log.DebugFormat("Assigning jobs for report", Array.Empty<object>());
      List<int> intList = new List<int>();
      if (reportIds.Count > 0)
        intList = ReportJobDAL.GetJobsIdsWithReports(reportIds);
      intList.AddRange((IEnumerable<int>) schedulesIds);
      ReportJobDAL.AssignJobsToReports(reportIds, schedulesIds);
      foreach (ReportJobConfiguration jobsById in ReportJobDAL.GetJobsByIds(intList))
      {
        this.RemoveBLScheduler(jobsById);
        this.AddBLScheduler(jobsById);
      }
    }

    public void UpdateReportJob(ReportJobConfiguration configuration, int[] allowedReportIds)
    {
      CoreBusinessLayerService.log.DebugFormat("Updating report job", Array.Empty<object>());
      ReportJobConfiguration reportJob = ReportJobDAL.GetReportJob(configuration.ReportJobID);
      this.RemoveBLScheduler(reportJob);
      bool flag = true;
      if (reportJob.Schedules != null && configuration.Schedules != null && reportJob.Schedules.Count == configuration.Schedules.Count)
      {
        foreach (ReportSchedule schedule in reportJob.Schedules)
        {
          ReportSchedule reportSchedule1 = schedule;
          DateTime startTime = schedule.StartTime;
          DateTime localTime = startTime.ToLocalTime();
          reportSchedule1.StartTime = localTime;
          ReportSchedule reportSchedule2 = schedule;
          DateTime? endTime = schedule.EndTime;
          DateTime? nullable;
          if (endTime.HasValue)
          {
            endTime = schedule.EndTime;
            startTime = endTime.Value;
            nullable = new DateTime?(startTime.ToLocalTime());
          }
          else
            nullable = schedule.EndTime;
          reportSchedule2.EndTime = nullable;
        }
        flag = !reportJob.Schedules.SequenceEqual<ReportSchedule>((IEnumerable<ReportSchedule>) configuration.Schedules);
      }
      List<int> second = new List<int>();
      ISwisConnectionProxyCreator creator = SwisConnectionProxyPool.GetCreator();
      if (flag)
      {
        FrequenciesDAL frequenciesDal = new FrequenciesDAL((IInformationServiceProxyCreator) creator);
        second = frequenciesDal.SaveFrequencies(new List<ISchedule>((IEnumerable<ISchedule>) configuration.Schedules));
        List<int> list = this.GetReportJob(configuration.ReportJobID).Schedules.Select<ReportSchedule, int>((System.Func<ReportSchedule, int>) (x => x.FrequencyId)).Except<int>((IEnumerable<int>) second).ToList<int>();
        if (list.Count > 0)
          frequenciesDal.DeleteFrequencies(list);
      }
      ReportJobDAL.UpdateReportJob(configuration, allowedReportIds, flag);
      ReportSchedulesDAL.ScheduleReportJobWithFrequencies(second, configuration.ReportJobID);
      List<ActionDefinition> actionDefinitionList = new List<ActionDefinition>();
      foreach (ActionDefinition action in reportJob.Actions)
      {
        ActionDefinition actionDefinition = action;
        if (!configuration.Actions.Any<ActionDefinition>((System.Func<ActionDefinition, bool>) (a =>
        {
          int? id1 = a.ID;
          int? id2 = actionDefinition.ID;
          return id1.GetValueOrDefault() == id2.GetValueOrDefault() & id1.HasValue == id2.HasValue;
        })) && !actionDefinition.IsShared)
          actionDefinitionList.Add(actionDefinition);
      }
      ActionsDAL actionsDal = new ActionsDAL((IInformationServiceProxyCreator) creator);
      actionsDal.SaveActionsForAssignments(configuration.ReportJobID, ((ActionEnviromentType) 1).ToString(), (IEnumerable<ActionDefinition>) configuration.Actions);
      foreach (ActionDefinition actionDefinition in actionDefinitionList)
        actionsDal.DeleteAction(Convert.ToInt32((object) actionDefinition.ID));
      this.AddBLScheduler(configuration);
    }

    public void DeleteReportJobs(List<int> reportJobIds)
    {
      CoreBusinessLayerService.log.DebugFormat("Deleting report job", Array.Empty<object>());
      List<ReportJobConfiguration> jobsByIds = ReportJobDAL.GetJobsByIds(reportJobIds);
      ISwisConnectionProxyCreator creator = SwisConnectionProxyPool.GetCreator();
      ActionsDAL actionsDal = new ActionsDAL((IInformationServiceProxyCreator) creator);
      foreach (ReportJobConfiguration configuration in jobsByIds)
      {
        this.RemoveBLScheduler(configuration);
        actionsDal.SaveActionsForAssignments(configuration.ReportJobID, ((ActionEnviromentType) 1).ToString(), (IEnumerable<ActionDefinition>) new List<ActionDefinition>());
      }
      List<int> intList = new List<int>();
      foreach (ReportJobConfiguration jobConfiguration in jobsByIds)
      {
        if (jobConfiguration.Schedules != null)
          intList.AddRange(jobConfiguration.Schedules.Select<ReportSchedule, int>((System.Func<ReportSchedule, int>) (x => x.FrequencyId)));
      }
      new FrequenciesDAL((IInformationServiceProxyCreator) creator).DeleteFrequencies(intList);
      ReportJobDAL.DeleteReportJobs(reportJobIds);
    }

    private void RemoveBLScheduler(ReportJobConfiguration configuration)
    {
      if (configuration.Schedules == null)
        return;
      for (int index = 0; index < configuration.Schedules.Count; ++index)
        Scheduler.Instance.Remove(string.Format("ReportJob-{0}_{1}", (object) configuration.ReportJobID.ToString(), (object) index));
    }

    private void AddBLScheduler(ReportJobConfiguration configuration)
    {
      ReportJobInitializer.AddActionsToScheduler(configuration, this);
    }

    public List<ReportJobConfiguration> GetSchedulesWithReport(int reportId)
    {
      return ReportJobDAL.GetJobsWithReport(reportId);
    }

    public int DublicateReportJob(int reportJobId, string jobName, int[] allowedReportIds)
    {
      CoreBusinessLayerService.log.DebugFormat("Dublicate report job", Array.Empty<object>());
      int num = ReportJobDAL.DublicateReportJob(reportJobId, jobName, allowedReportIds);
      ReportJobConfiguration reportJob = this.GetReportJob(reportJobId);
      reportJob.ReportJobID = num;
      ISwisConnectionProxyCreator creator = SwisConnectionProxyPool.GetCreator();
      ActionsDAL actionsDal = new ActionsDAL((IInformationServiceProxyCreator) creator);
      foreach (ReportSchedule schedule in reportJob.Schedules)
        schedule.FrequencyId = 0;
      ReportSchedulesDAL.ScheduleReportJobWithFrequencies(new FrequenciesDAL((IInformationServiceProxyCreator) creator).SaveFrequencies(new List<ISchedule>((IEnumerable<ISchedule>) reportJob.Schedules)), num);
      foreach (ActionDefinition action in reportJob.Actions)
      {
        action.Title = actionsDal.GetUniqueNameForAction(action.Title);
        action.IsShared = false;
      }
      actionsDal.SaveActionsForAssignments(num, ((ActionEnviromentType) 1).ToString(), (IEnumerable<ActionDefinition>) reportJob.Actions);
      this.AddBLScheduler(reportJob);
      return num;
    }

    public ReportJobConfiguration GetReportJob(int jobId)
    {
      CoreBusinessLayerService.log.DebugFormat("Extracting report job by ID", Array.Empty<object>());
      return ReportJobDAL.GetReportJob(jobId);
    }

    public void UnAssignReportsFromJob(int jobId, List<int> reportIds)
    {
      ReportJobDAL.UnAssignReportsFromJob(jobId, reportIds);
      ReportJobConfiguration reportJob = ReportJobDAL.GetReportJob(jobId);
      this.RemoveBLScheduler(reportJob);
      this.AddBLScheduler(reportJob);
    }

    public Dictionary<int, bool> RunNow(List<int> schedulesIds)
    {
      CoreBusinessLayerService.log.DebugFormat("Running job(s)", Array.Empty<object>());
      Dictionary<int, bool> dictionary = new Dictionary<int, bool>();
      List<ReportJobConfiguration> jobsByIds = ReportJobDAL.GetJobsByIds(schedulesIds);
      for (int index = 0; index < jobsByIds.Count; ++index)
      {
        foreach (ActionDefinition action in jobsByIds[index].Actions)
        {
          ReportingActionContext context = new ReportingActionContext()
          {
            AccountID = jobsByIds[index].AccountID,
            UrlsGroupedByLeftPart = ReportJobInitializer.GroupUrls(jobsByIds[index]),
            WebsiteID = jobsByIds[index].WebsiteID
          };
          ((ActionContextBase) context).MacroContext.Add((ContextBase) new ReportingContext()
          {
            AccountID = jobsByIds[index].AccountID,
            ScheduleName = jobsByIds[index].Name,
            ScheduleDescription = jobsByIds[index].Description,
            LastRun = jobsByIds[index].LastRun,
            WebsiteID = jobsByIds[index].WebsiteID
          });
          ((ActionContextBase) context).MacroContext.Add((ContextBase) new GenericContext());
          if (!dictionary.Keys.Contains<int>(jobsByIds[index].ReportJobID))
          {
            dictionary.Add(jobsByIds[index].ReportJobID, this.ExecuteAction(action, (ActionContextBase) context).Status == 1);
          }
          else
          {
            ActionResult actionResult = this.ExecuteAction(action, (ActionContextBase) context);
            dictionary[jobsByIds[index].ReportJobID] = actionResult.Status == 1 && dictionary[jobsByIds[index].ReportJobID];
          }
        }
        jobsByIds[index].LastRun = new DateTime?(DateTime.Now.ToUniversalTime());
        ReportJobDAL.UpdateLastRun(jobsByIds[index].ReportJobID, jobsByIds[index].LastRun);
      }
      return dictionary;
    }

    [Obsolete("Core-Split cleanup. If you need this member please contact Core team", true)]
    public int[] GetListOfAllowedReports()
    {
      try
      {
        using (IInformationServiceProxy2 iinformationServiceProxy2 = ((IInformationServiceProxyCreator) SwisConnectionProxyPool.GetCreator()).Create())
        {
          DataTable dataTable = ((IInformationServiceProxy) iinformationServiceProxy2).Query("SELECT ReportID FROM Orion.Report");
          List<int> intList = new List<int>();
          if (dataTable != null)
            intList.AddRange(dataTable.Rows.Cast<DataRow>().Select<DataRow, int>((System.Func<DataRow, int>) (row => (int) row["ReportID"])));
          return intList.ToArray();
        }
      }
      catch (Exception ex)
      {
        CoreBusinessLayerService.log.Error((object) "GetLimitedReportIds failed.", ex);
        throw;
      }
    }

    public List<SmtpServer> GetAvailableSmtpServers() => SmtpServerDAL.GetAvailableSmtpServers();

    public bool DefaultSmtpServerExists() => SmtpServerDAL.DefaultSmtpServerExists();

    public int InsertSmtpServer(SmtpServer server) => SmtpServerDAL.InsertSmtpServer(server);

    public SmtpServer GetSmtpServer(int id) => SmtpServerDAL.GetSmtpServer(id);

    public SmtpServer GetSmtpServerByAddress(string address)
    {
      return SmtpServerDAL.GetSmtpServerByAddress(address);
    }

    public bool UpdateSmtpServer(SmtpServer server) => SmtpServerDAL.UpdateSmtpServer(server);

    public bool DeleteSmtpServer(int id) => SmtpServerDAL.DeleteSmtpServer(id);

    public void SetSmtpServerAsDefault(int id) => SmtpServerDAL.SetSmtpServerAsDefault(id);

    public bool SNMPQuery(
      int nodeId,
      string oid,
      string snmpGetType,
      out Dictionary<string, string> response)
    {
      return SNMPHelper.SNMPQuery(nodeId, snmpGetType, oid, ref response);
    }

    private static bool AreRelatedOids(string queryOid, string returnedOid)
    {
      if (queryOid.Equals(returnedOid))
        return true;
      string[] strArray1 = queryOid.Split('.');
      string[] strArray2 = returnedOid.Split('.');
      if (strArray1.Length > strArray2.Length)
        return false;
      for (int index = 0; index < strArray1.Length; ++index)
      {
        if (strArray1[index].CompareTo(strArray2[index]) != 0)
          return false;
      }
      return true;
    }

    private static string GetOidValueFromXmlNodes(XmlNode[] xmlNodes)
    {
      string empty = string.Empty;
      XmlText xmlText = xmlNodes != null ? (XmlText) ((IEnumerable<XmlNode>) xmlNodes).FirstOrDefault<XmlNode>((System.Func<XmlNode, bool>) (item => item is XmlText)) : (XmlText) null;
      if (xmlText != null)
        empty = xmlText.Value;
      return empty;
    }

    private static bool NodeSnmpQueryProcessGetOrGetNextResult(
      string oid,
      Dictionary<string, string> response,
      SnmpJobResults jobResult)
    {
      string valueFromXmlNodes = CoreBusinessLayerService.GetOidValueFromXmlNodes(((List<SnmpOID>) jobResult.Results[0].OIDs)[0].Value as XmlNode[]);
      if (string.IsNullOrEmpty(valueFromXmlNodes))
      {
        response["swerror"] = Resources.LIBCODE_PS0_18;
        return false;
      }
      if (CoreBusinessLayerService.AreRelatedOids(oid, ((List<SnmpOID>) jobResult.Results[0].OIDs)[0].OID))
      {
        response["0"] = valueFromXmlNodes;
        return true;
      }
      response["swerror"] = Resources.LIBCODE_PS0_19;
      return false;
    }

    private static bool NodeSnmpQueryProcessSubtreeResult(
      string oid,
      Dictionary<string, string> response,
      SnmpJobResults jobResult)
    {
      if (((List<SnmpOID>) jobResult.Results[0].OIDs).Count == 0)
        return false;
      string str1 = oid;
      string str2 = oid;
      bool flag = true;
      foreach (SnmpOID oiD in (List<SnmpOID>) jobResult.Results[0].OIDs)
      {
        if (oiD.OID.StartsWith(str1 + ".") && str2 != oiD.OID)
        {
          str2 = oiD.OID;
          string key = "0";
          if (oiD.OID.Length > str1.Length + 1)
            key = oiD.OID.Substring(str1.Length + 1);
          string valueFromXmlNodes = CoreBusinessLayerService.GetOidValueFromXmlNodes(oiD.Value as XmlNode[]);
          if (string.IsNullOrEmpty(valueFromXmlNodes))
          {
            response[key] = Resources.LIBCODE_PS0_19;
            flag = false;
          }
          else
            response[key] = valueFromXmlNodes;
        }
      }
      return flag;
    }

    public bool NodeSNMPQuery(
      SolarWinds.Orion.Core.Common.Models.Node node,
      string oid,
      string snmpGetType,
      out Dictionary<string, string> response)
    {
      SnmpRequestType result1;
      if (!System.Enum.TryParse<SnmpRequestType>(snmpGetType, true, out result1))
        result1 = (SnmpRequestType) 0;
      List<SnmpRequest> snmpRequestList = new List<SnmpRequest>();
      snmpRequestList.Add(new SnmpRequest()
      {
        OID = oid,
        IsTransform = false,
        RequestType = result1
      });
      int num = SettingsDAL.GetCurrentInt("SWNetPerfMon-Settings-SNMP Timeout", 2500) / 1000;
      JobDescription jobDescription = SnmpJob.CreateJobDescription(node.IpAddress, node.SNMPPort, num, node.SNMPVersion, snmpRequestList, BusinessLayerSettings.Instance.TestJobTimeout);
      SnmpCredentialsV2 jobCredential = new SnmpCredentialsV2(node.ReadOnlyCredentials);
      response = new Dictionary<string, string>();
      SnmpJobResults result2 = this.ExecuteJobAndGetResult<SnmpJobResults>(jobDescription, (CredentialBase) jobCredential, JobResultDataFormatType.Xml, "SNMP", out string _);
      if (!((TestJobResult) result2).Success || result2.Results.Count == 0)
        return false;
      if (result2.Results[0].ResultType != null)
      {
        response["swerror"] = result2.Results[0].ErrorMessage;
        return false;
      }
      return result1 == null || result1 == 1 ? CoreBusinessLayerService.NodeSnmpQueryProcessGetOrGetNextResult(oid, response, result2) : CoreBusinessLayerService.NodeSnmpQueryProcessSubtreeResult(oid, response, result2);
    }

    [Obsolete("Core-Split cleanup. If you need this member please contact Core team", true)]
    public void SNMPQueryForIp(
      string ip,
      string oid,
      List<SnmpEntry> credentials,
      string snmpGetType,
      out Dictionary<string, string> response)
    {
      SNMPHelper.SNMPQueryForIp(ip, oid, credentials, snmpGetType, ref response);
    }

    public Dictionary<string, Dictionary<string, string>> GetColumns(string tableOID, int nodeId)
    {
      return SNMPHelper.GetColumns(tableOID, nodeId);
    }

    public bool ValidateSNMP(
      SNMPVersion snmpVersion,
      string ip,
      uint snmpPort,
      string community,
      string authKey,
      bool authKeyIsPwd,
      SNMPv3AuthType authType,
      SNMPv3PrivacyType privacyType,
      string privacyPassword,
      bool privKeyIsPwd,
      string context,
      string username)
    {
      return this.ValidateSNMPWithErrorMessage(snmpVersion, ip, snmpPort, community, authKey, authKeyIsPwd, authType, privacyType, privacyPassword, privKeyIsPwd, context, username, out string _);
    }

    public bool ValidateSNMPWithErrorMessage(
      SNMPVersion snmpVersion,
      string ip,
      uint snmpPort,
      string community,
      string authKey,
      bool authKeyIsPwd,
      SNMPv3AuthType authType,
      SNMPv3PrivacyType privacyType,
      string privacyPassword,
      bool privKeyIsPwd,
      string context,
      string username,
      out string localizedErrorMessage)
    {
      List<SnmpRequest> snmpRequestList = new List<SnmpRequest>();
      snmpRequestList.Add(new SnmpRequest()
      {
        OID = "1.3.6.1.2.1.1.2.0",
        IsTransform = false,
        OIDLabel = "sysObjectID",
        RequestType = (SnmpRequestType) 0
      });
      localizedErrorMessage = string.Empty;
      int num = SettingsDAL.GetCurrentInt("SWNetPerfMon-Settings-SNMP Timeout", 2500) / 1000;
      SnmpJobResults result = this.ExecuteJobAndGetResult<SnmpJobResults>(SnmpJob.CreateJobDescription(ip, snmpPort, num, snmpVersion, snmpRequestList, BusinessLayerSettings.Instance.TestJobTimeout), (CredentialBase) new SnmpCredentialsV2()
      {
        CommunityString = community,
        CredentialName = "",
        SNMPV3AuthKeyIsPwd = authKeyIsPwd,
        SNMPv3AuthType = authType,
        SNMPv3AuthPassword = authKey,
        SNMPv3PrivacyType = privacyType,
        SNMPv3PrivacyPassword = privacyPassword,
        SNMPV3PrivKeyIsPwd = privKeyIsPwd,
        SnmpV3Context = context,
        SNMPv3UserName = username
      }, JobResultDataFormatType.Xml, "SNMP", out string _);
      if (!((TestJobResult) result).Success)
      {
        localizedErrorMessage = ((TestJobResult) result).Message;
        return false;
      }
      bool flag = result.Results.Count > 0 && result.Results[0].ResultType == 0;
      CoreBusinessLayerService.log.InfoFormat("SNMP credential test finished. Success: {0}", (object) flag);
      return flag;
    }

    public bool ValidateReadWriteSNMP(
      SNMPVersion snmpVersion,
      string ip,
      uint snmpPort,
      string community,
      string authKey,
      bool authKeyIsPwd,
      SNMPv3AuthType authType,
      SNMPv3PrivacyType privacyType,
      string privacyPassword,
      bool privKeyIsPwd,
      string context,
      string username)
    {
      int num = SettingsDAL.GetCurrentInt("SWNetPerfMon-Settings-SNMP Timeout", 2500) / 1000;
      ValidateJobResult result = this.ExecuteJobAndGetResult<ValidateJobResult>(SnmpReadWriteCredentialValidateJob.CreateJobDescription(ip, snmpPort, num, snmpVersion, BusinessLayerSettings.Instance.TestJobTimeout), (CredentialBase) new SnmpCredentialsV2()
      {
        CommunityString = community,
        CredentialName = "",
        SNMPV3AuthKeyIsPwd = authKeyIsPwd,
        SNMPv3AuthType = authType,
        SNMPv3AuthPassword = authKey,
        SNMPv3PrivacyType = privacyType,
        SNMPv3PrivacyPassword = privacyPassword,
        SNMPV3PrivKeyIsPwd = privKeyIsPwd,
        SnmpV3Context = context,
        SNMPv3UserName = username
      }, JobResultDataFormatType.Xml, "SNMP", out string _);
      CoreBusinessLayerService.log.InfoFormat(string.Format("SNMP read/write credential test finished. Success: {0}.", (object) result.IsValid), Array.Empty<object>());
      return result.IsValid;
    }

    private void SnmpEncodingSettingsChanged(object sender, SettingsChangedEventArgs e)
    {
      try
      {
        if (SnmpSettings.Instance.Encoding == SNMPEncodingExtension.GetWebName((SNMPEncoding) 0))
        {
          string autoEncoding = SNMPHelper.GetAutoEncoding();
          CoreBusinessLayerService.log.InfoFormat("Set encoding {0} for primary locale {1}", (object) autoEncoding, (object) LocaleConfiguration.PrimaryLocale);
          SNMPEncodingSettings.Instance.ChangeEncoding(Encoding.GetEncoding(autoEncoding));
        }
        else
          SNMPEncodingSettings.Instance.ChangeEncoding(Encoding.GetEncoding(SnmpSettings.Instance.Encoding));
      }
      catch (Exception ex)
      {
        CoreBusinessLayerService.log.Error((object) ("Unable to save text encoding setting for snmp. Encoding will be system default: " + Encoding.Default.WebName), ex);
      }
    }

    [Obsolete("Use GetSnmpV3CredentialsSet method")]
    public List<string> GetCredentialsSet()
    {
      return this.GetSnmpV3CredentialsSet().Values.ToList<string>();
    }

    public IDictionary<int, string> GetSnmpV3CredentialsSet()
    {
      try
      {
        return new CredentialManager().GetCredentialNames<SnmpCredentialsV3>("Orion");
      }
      catch (Exception ex)
      {
        CoreBusinessLayerService.log.Error((object) "Error getting Snmp v3 credentials", ex);
        throw;
      }
    }

    [Obsolete("Use InsertSnmpV3Credentials method")]
    public void InsertCredentials(SnmpCredentials crendentials)
    {
      this.InsertSnmpV3Credentials((SnmpCredentialsV3) CredentialHelper.ParseCredentials(crendentials));
    }

    public int? InsertSnmpV3Credentials(SnmpCredentialsV3 credentials)
    {
      try
      {
        new CredentialManager().AddCredential<SnmpCredentialsV3>("Orion", credentials);
        return ((Credential) credentials).ID;
      }
      catch (Exception ex)
      {
        CoreBusinessLayerService.log.Error((object) "Error inserting Snmp v3 credentials", ex);
        throw;
      }
    }

    [Obsolete("Use DeleteSnmpV3Credentials method")]
    public void DeleteCredentials(string CredentialName)
    {
      this.DeleteSnmpV3Credentials(CredentialName);
    }

    public void DeleteSnmpV3Credentials(string CredentialName)
    {
      try
      {
        CredentialManager credentialManager = new CredentialManager();
        credentialManager.DeleteCredential<SnmpCredentialsV3>("Orion", credentialManager.GetCredentials<SnmpCredentialsV3>("Orion", CredentialName).FirstOrDefault<SnmpCredentialsV3>());
      }
      catch (Exception ex)
      {
        CoreBusinessLayerService.log.Error((object) string.Format("Error deleting Snmp v3 credentials by name {0}", (object) CredentialName), ex);
        throw;
      }
    }

    public void DeleteSnmpV3CredentialsByID(int CredentialID)
    {
      try
      {
        new CredentialManager().DeleteCredential("Orion", CredentialID);
      }
      catch (Exception ex)
      {
        CoreBusinessLayerService.log.Error((object) string.Format("Error deleting Snmp v3 credentials by id {0}", (object) CredentialID), ex);
        throw;
      }
    }

    [Obsolete("Use GetSnmpV3Credentials method")]
    public SnmpCredentials GetCredentials(string CredentialName)
    {
      try
      {
        return SnmpCredentials.CreateSnmpCredentials(CredentialHelper.GetSnmpEntry((SnmpCredentials) new CredentialManager().GetCredentials<SnmpCredentialsV3>("Orion", CredentialName).FirstOrDefault<SnmpCredentialsV3>()));
      }
      catch (Exception ex)
      {
        CoreBusinessLayerService.log.Error((object) string.Format("Error getting Snmp v3 credentials by name {0}", (object) CredentialName), ex);
        throw;
      }
    }

    public SnmpCredentialsV3 GetSnmpV3Credentials(string CredentialName)
    {
      try
      {
        return new CredentialManager().GetCredentials<SnmpCredentialsV3>("Orion", CredentialName).FirstOrDefault<SnmpCredentialsV3>();
      }
      catch (Exception ex)
      {
        CoreBusinessLayerService.log.Error((object) string.Format("Error getting Snmp v3 credentials by name {0}", (object) CredentialName), ex);
        throw;
      }
    }

    public SnmpCredentialsV3 GetSnmpV3CredentialsByID(int CredentialID)
    {
      try
      {
        return new CredentialManager().GetCredentials<SnmpCredentialsV3>((IEnumerable<int>) new List<int>()
        {
          CredentialID
        }).FirstOrDefault<SnmpCredentialsV3>();
      }
      catch (Exception ex)
      {
        CoreBusinessLayerService.log.Error((object) string.Format("Error getting Snmp v3 credentials by id {0}", (object) CredentialID), ex);
        throw;
      }
    }

    [Obsolete("Use UpdateSnmpV3Credentials method")]
    public void UpdateCredentials(SnmpCredentials credentials)
    {
      try
      {
        new CredentialManager().UpdateCredential<SnmpCredentialsV3>("Orion", (SnmpCredentialsV3) CredentialHelper.ParseCredentials(credentials));
      }
      catch (Exception ex)
      {
        CoreBusinessLayerService.log.Error((object) "Error updating Snmp v3 credentials", ex);
        throw;
      }
    }

    public void UpdateSnmpV3Credentials(SnmpCredentialsV3 credentials)
    {
      try
      {
        new CredentialManager().UpdateCredential<SnmpCredentialsV3>("Orion", credentials);
      }
      catch (Exception ex)
      {
        CoreBusinessLayerService.log.Error((object) "Error updating Snmp v3 credentials", ex);
        throw;
      }
    }

    public StringDictionary GetSeverities() => SysLogDAL.GetSeverities();

    public StringDictionary GetFacilities() => SysLogDAL.GetFacilities();

    public BaselineValues GetBaselineValues(string thresholdName, int instanceId)
    {
      return ThresholdProcessingManager.Instance.Engine.GetBaselineValues(thresholdName, instanceId);
    }

    public List<BaselineValues> GetBaselineValuesForAllTimeFrames(
      string thresholdName,
      int instanceId)
    {
      return ThresholdProcessingManager.Instance.Engine.GetBaselineValuesForAllTimeFrames(thresholdName, instanceId);
    }

    public ThresholdComputationResult ComputeThresholds(
      string thresholdName,
      int instanceId,
      string warningFormula,
      string criticalFormula,
      BaselineValues baselineValues,
      ThresholdOperatorEnum thresholdOperator)
    {
      return ThresholdProcessingManager.Instance.Engine.ComputeThresholds(thresholdName, instanceId, warningFormula, criticalFormula, baselineValues, thresholdOperator);
    }

    public ThresholdComputationResult ProcessThresholds(
      double warningThreshold,
      double criticalThreshold,
      ThresholdOperatorEnum oper,
      ThresholdMinMaxValue minMaxValues)
    {
      return ThresholdProcessingManager.Instance.Engine.ProcessThresholds(warningThreshold, criticalThreshold, oper, minMaxValues);
    }

    public ThresholdComputationResult ProcessThresholds(
      bool warningEnabled,
      double warningThreshold,
      bool criticalEnabled,
      double criticalThreshold,
      ThresholdOperatorEnum oper,
      ThresholdMinMaxValue minMaxValues)
    {
      return ThresholdProcessingManager.Instance.Engine.ProcessThresholds(warningEnabled, warningThreshold, criticalEnabled, criticalThreshold, oper, minMaxValues);
    }

    public ValidationResult IsFormulaValid(
      string thresholdName,
      string formula,
      ThresholdLevel level,
      ThresholdOperatorEnum thresholdOperator)
    {
      return ThresholdProcessingManager.Instance.Engine.IsFormulaValid(thresholdName, formula, level, thresholdOperator);
    }

    public ThresholdMinMaxValue GetThresholdMinMaxValues(string thresholdName, int instanceId)
    {
      return ThresholdProcessingManager.Instance.Engine.GetThresholdMinMaxValues(thresholdName, instanceId);
    }

    public int SetThreshold(Threshold threshold)
    {
      return ThresholdProcessingManager.Instance.Engine.SetThreshold(threshold);
    }

    public StatisticalDataHistogram[] GetHistogramForStatisticalData(
      string thresholdName,
      int instanceId)
    {
      return ThresholdProcessingManager.Instance.Engine.GetHistogramForStatisticalData(thresholdName, instanceId);
    }

    public string GetStatisticalDataChartName(string thresholdName)
    {
      return ThresholdProcessingManager.Instance.Engine.GetStatisticalDataChartName(thresholdName);
    }

    public string GetThresholdInstanceName(string thresholdName, int instanceId)
    {
      return ThresholdProcessingManager.Instance.Engine.GetThresholdInstanceName(thresholdName, instanceId);
    }

    public TracerouteResult TraceRoute(string destinationHostNameOrIpAddress)
    {
      return CoreBusinessLayerService.CreateTraceRouteProvider().TraceRoute(destinationHostNameOrIpAddress);
    }

    private static ITraceRouteProvider CreateTraceRouteProvider()
    {
      return (ITraceRouteProvider) new TraceRouteProviderSync();
    }

    public Views GetSummaryDetailsViews() => ViewsDAL.GetSummaryDetailsViews();

    public void DeleteVolume(Volume volume)
    {
      Dictionary<string, object> volumeBaseInfo = CoreBusinessLayerService.GetVolumeBaseInfo(volume);
      VolumeDAL.DeleteVolume(volume);
      VolumeIndication volumeIndication = new VolumeIndication((IndicationType) 1, volume);
      foreach (KeyValuePair<string, object> keyValuePair in volumeBaseInfo)
        volumeIndication.AddSourceInstanceProperty(keyValuePair.Key, keyValuePair.Value);
      volumeIndication.AddIndicationProperty("SourceInstanceUri", volumeBaseInfo["Uri"]);
      IndicationPublisher.CreateV3().ReportIndication((IIndication) volumeIndication);
    }

    public int InsertVolume(Volume volume)
    {
      int maxElementCount = new FeatureManager().GetMaxElementCount(WellKnownElementTypes.Volumes);
      if (VolumeDAL.GetVolumeCount() >= maxElementCount)
        throw LicenseException.FromElementsExceeded(maxElementCount);
      int num = VolumeDAL.InsertVolume(volume);
      CoreBusinessLayerService.FireVolumeIndication((IndicationType) 0, volume);
      return num;
    }

    public void UpdateVolume(Volume volume)
    {
      PropertyBag changedProperties = VolumeDAL.UpdateVolume(volume);
      CoreBusinessLayerService.FireVolumeIndication((IndicationType) 2, volume, changedProperties);
    }

    public Volume GetVolume(int volumeID) => VolumeDAL.GetVolume(volumeID);

    public void BulkUpdateVolumePollingInterval(int pollingInterval, int engineId)
    {
      VolumeDAL.BulkUpdateVolumePollingInterval(pollingInterval, engineId);
    }

    public Dictionary<string, object> GetVolumeCustomProperties(
      int volumeId,
      ICollection<string> properties)
    {
      return VolumeDAL.GetVolumeCustomProperties(volumeId, properties);
    }

    public Volumes GetVolumesByIds(int[] volumeIds) => VolumeDAL.GetVolumesByIds(volumeIds);

    private static void FireVolumeIndication(
      IndicationType indicationType,
      Volume volume,
      PropertyBag changedProperties = null)
    {
      try
      {
        Dictionary<string, object> volumeBaseInfo = CoreBusinessLayerService.GetVolumeBaseInfo(volume);
        VolumeIndication volumeIndication = new VolumeIndication(indicationType, volume);
        if (indicationType <= 2)
          volumeIndication.AddIndicationProperty("SourceInstanceUri", volumeBaseInfo["Uri"]);
        foreach (KeyValuePair<string, object> keyValuePair in volumeBaseInfo)
          volumeIndication.AddSourceInstanceProperty(keyValuePair.Key, keyValuePair.Value);
        if (changedProperties != null)
        {
          foreach (KeyValuePair<string, object> changedProperty in (Dictionary<string, object>) changedProperties)
            volumeIndication.AddSourceInstanceProperty(changedProperty.Key, changedProperty.Value);
        }
        IndicationPublisher.CreateV3().ReportIndication((IIndication) volumeIndication);
      }
      catch (Exception ex)
      {
        string str = string.Format("Error delivering indication {0} for Volume '{1}' with id {2}.", (object) indicationType, (object) volume.ID, (object) volume.Caption);
        CoreBusinessLayerService.log.Error((object) str, ex);
      }
    }

    private static Dictionary<string, object> GetVolumeBaseInfo(Volume volume)
    {
      return new SwisEntityHelper(CoreBusinessLayerService.CreateProxy()).GetProperties("Orion.Volumes", volume.VolumeId, new string[2]
      {
        "DisplayName",
        "Uri"
      });
    }

    public ExternalWebsites GetExternalWebsites() => ExternalWebsitesDAL.GetAll();

    public ExternalWebsite GetExternalWebsite(int id) => ExternalWebsitesDAL.Get(id);

    public int CreateExternalWebsite(ExternalWebsite site) => ExternalWebsitesDAL.Insert(site);

    public void UpdateExternalWebsite(ExternalWebsite site) => ExternalWebsitesDAL.Update(site);

    public void DeleteExternalWebsite(int id) => ExternalWebsitesDAL.Delete(id);

    public void AddNewWebMenuItemToMenubar(WebMenuItem item, string menubarName)
    {
      int itemId = WebMenubarDAL.InsertItem(item);
      WebMenubarDAL.AppendItemToMenu(menubarName, itemId);
    }

    public void DeleteWebMenuItemByLink(string link) => WebMenubarDAL.DeleteItemByLink(link);

    public void RenameWebMenuItemByLink(
      string newName,
      string newDescription,
      string newMenuBar,
      string link)
    {
      WebMenubarDAL.RenameItemByLink(newName, newDescription, newMenuBar, link);
    }

    public bool MenuItemExists(string link) => WebMenubarDAL.MenuItemExists(link);

    public RemoteAccessToken GetUserWebIntegrationToken(string username)
    {
      return new RemoteAuthManager().GetUserToken(username);
    }

    public bool IsUserWebIntegrationAvailable(string username)
    {
      return new RemoteAuthManager().IsUserAvailable(username);
    }

    public void DisableUserWebIntegration(string username)
    {
      new RemoteAuthManager().DisableUser(username);
    }

    public RemoteAccessToken ConfigureUserWebIntegration(
      string username,
      string clientId,
      string clientPassword)
    {
      RemoteAuthManager remoteAuthManager = new RemoteAuthManager();
      try
      {
        return remoteAuthManager.ConfigureUser(username, clientId, clientPassword);
      }
      catch (Exception ex)
      {
        throw MessageUtilities.NewFaultException<CoreFaultContract>(ex);
      }
    }

    public IEnumerable<MaintenanceStatus> GetMaintenanceInfoFromCustomerPortal(string username)
    {
      try
      {
        return this.MaintenanceInfoCache[username];
      }
      catch (Exception ex)
      {
        CoreBusinessLayerService.log.Error((object) ex);
        throw;
      }
    }

    public LicenseAndManagementInfo GetLicenseAndMaintenanceSummary(string username)
    {
      return this.LAMInfoCache[username];
    }

    public IEnumerable<SupportCase> GetSupportCases(string username)
    {
      try
      {
        return this.SupportCasesCache[username];
      }
      catch (Exception ex)
      {
        CoreBusinessLayerService.log.Error((object) ex);
        throw;
      }
    }

    private IEnumerable<MaintenanceStatus> GetMaintenanceInfoFromCustomerPortalInternal(
      string username)
    {
      return (IEnumerable<MaintenanceStatus>) new RemoteMaintenanceClient(username).GetMaintenanceInfo().Select<WebMaintenanceStatus, MaintenanceStatus>((System.Func<WebMaintenanceStatus, MaintenanceStatus>) (i => i.ToMaintenanceStatus())).ToList<MaintenanceStatus>();
    }

    private LicenseAndManagementInfo GetLicenseAndMaintenanceSummaryInternal(string username)
    {
      int num1;
      int num2;
      int num3;
      if (this.IsUserWebIntegrationAvailable(username))
      {
        IEnumerable<MaintenanceStatus> fromCustomerPortal = this.GetMaintenanceInfoFromCustomerPortal(username);
        IEnumerable<MaintenanceStatus> source = fromCustomerPortal == null ? (IEnumerable<MaintenanceStatus>) new List<MaintenanceStatus>() : (IEnumerable<MaintenanceStatus>) fromCustomerPortal.ToList<MaintenanceStatus>();
        num1 = source.Count<MaintenanceStatus>((System.Func<MaintenanceStatus, bool>) (m => (int) (m.ExpirationDate - DateTime.UtcNow.Date).TotalDays >= 90));
        num2 = source.Count<MaintenanceStatus>((System.Func<MaintenanceStatus, bool>) (m => (int) (m.ExpirationDate - DateTime.UtcNow.Date).TotalDays < 90 && (int) (m.ExpirationDate - DateTime.UtcNow.Date).TotalDays > 0));
        num3 = source.Count<MaintenanceStatus>((System.Func<MaintenanceStatus, bool>) (m => (int) (m.ExpirationDate - DateTime.UtcNow.Date).TotalDays <= 0));
      }
      else
      {
        List<ModuleLicenseInfo> list = this.GetModuleLicenseInformation().Where<ModuleLicenseInfo>((System.Func<ModuleLicenseInfo, bool>) (m => !string.Equals("DPI", m.ModuleName, StringComparison.OrdinalIgnoreCase) && !m.IsEval)).ToList<ModuleLicenseInfo>();
        num1 = list.Count<ModuleLicenseInfo>((System.Func<ModuleLicenseInfo, bool>) (m =>
        {
          DateTime dateTime = m.MaintenanceExpiration;
          DateTime date1 = dateTime.Date;
          dateTime = DateTime.UtcNow;
          DateTime date2 = dateTime.Date;
          return (int) (date1 - date2).TotalDays >= 90;
        }));
        num2 = list.Count<ModuleLicenseInfo>((System.Func<ModuleLicenseInfo, bool>) (m =>
        {
          DateTime dateTime1 = m.MaintenanceExpiration;
          DateTime date3 = dateTime1.Date;
          dateTime1 = DateTime.UtcNow;
          DateTime date4 = dateTime1.Date;
          if ((int) (date3 - date4).TotalDays >= 90)
            return false;
          DateTime dateTime2 = m.MaintenanceExpiration;
          DateTime date5 = dateTime2.Date;
          dateTime2 = DateTime.UtcNow;
          DateTime date6 = dateTime2.Date;
          return (int) (date5 - date6).TotalDays > 0;
        }));
        num3 = list.Count<ModuleLicenseInfo>((System.Func<ModuleLicenseInfo, bool>) (m =>
        {
          DateTime dateTime = m.MaintenanceExpiration;
          DateTime date7 = dateTime.Date;
          dateTime = DateTime.UtcNow;
          DateTime date8 = dateTime.Date;
          return (int) (date7 - date8).TotalDays <= 0;
        }));
      }
      int num4 = this.GetModuleLicenseInformation().Count<ModuleLicenseInfo>((System.Func<ModuleLicenseInfo, bool>) (m => !string.Equals("DPI", m.ModuleName, StringComparison.OrdinalIgnoreCase) && m.IsEval && !m.IsRC));
      int count1 = this.GetModuleSaturationInformation().Count;
      int count2 = this.GetMaintenanceRenewalNotificationItems(false).Count;
      return new LicenseAndManagementInfo()
      {
        UpdatesAvailableCount = count2,
        LicenseLimitReachedCount = count1,
        EvaluationExpiringCount = num4,
        MaintenanceExpiringCount = num2,
        MaintenanceActiveCount = num1,
        MaintenanceExpiredCount = num3
      };
    }

    private IEnumerable<SupportCase> GetSupportCasesInternal(string username)
    {
      return (IEnumerable<SupportCase>) new RemoteSupportCasesClient(username).GetSupportCases().Select<WebSupportCase, SupportCase>((System.Func<WebSupportCase, SupportCase>) (c => c.ToSupportCase())).ToList<SupportCase>();
    }

    private ExpirableCache<string, IEnumerable<MaintenanceStatus>> MaintenanceInfoCache
    {
      get
      {
        return this._maintenanceInfoCache ?? (this._maintenanceInfoCache = new ExpirableCache<string, IEnumerable<MaintenanceStatus>>(TimeSpan.FromMinutes(5.0), new System.Func<string, IEnumerable<MaintenanceStatus>>(this.GetMaintenanceInfoFromCustomerPortalInternal)));
      }
    }

    private ExpirableCache<string, LicenseAndManagementInfo> LAMInfoCache
    {
      get
      {
        return this._LAMInfoCache ?? (this._LAMInfoCache = new ExpirableCache<string, LicenseAndManagementInfo>(TimeSpan.FromMinutes(1.0), new System.Func<string, LicenseAndManagementInfo>(this.GetLicenseAndMaintenanceSummaryInternal)));
      }
    }

    private ExpirableCache<string, IEnumerable<SupportCase>> SupportCasesCache
    {
      get
      {
        return this._supportCasesCache ?? (this._supportCasesCache = new ExpirableCache<string, IEnumerable<SupportCase>>(TimeSpan.FromMinutes(5.0), new System.Func<string, IEnumerable<SupportCase>>(this.GetSupportCasesInternal)));
      }
    }

    public WebResources GetSpecificResources(int viewID, string queryFilterString)
    {
      return WebResourcesDAL.GetSpecificResources(viewID, queryFilterString);
    }

    public void DeleteResource(int resourceId) => WebResourcesDAL.DeleteResource(resourceId);

    public void DeleteResourceProperties(int resourceId)
    {
      WebResourcesDAL.DeleteResourceProperties(resourceId);
    }

    public int InsertNewResource(WebResource resource, int viewID)
    {
      return WebResourcesDAL.InsertNewResource(resource, viewID);
    }

    public void InsertNewResourceProperty(
      int resourceID,
      string propertyName,
      string propertyValue)
    {
      WebResourcesDAL.InsertNewResourceProperty(resourceID, propertyName, propertyValue);
    }

    public string GetSpecificResourceProperty(int resourceID, string queryFilterString)
    {
      return WebResourcesDAL.GetSpecificResourceProperty(resourceID, queryFilterString);
    }

    public void UpdateResourceProperty(int resourceID, string propertyName, string propertyValue)
    {
      WebResourcesDAL.UpdateResourceProperty(resourceID, propertyName, propertyValue);
    }

    public void UpdateWebsiteInfo(string serverName, string ipAddress, int port)
    {
      WebsitesDAL.UpdateWebsiteInfo(serverName, ipAddress, port);
    }

    public string GetSiteAddress() => WebsitesDAL.GetSiteAddress();

    public bool IsHttpsUsed() => WebsitesDAL.IsHttpsUsed();

    private static NotificationItemType DalToWfc(NotificationItemTypeDAL dal)
    {
      return dal == null ? (NotificationItemType) null : new NotificationItemType(dal.Id, dal.TypeName, dal.Module, dal.Caption, dal.DetailsUrl, dal.DetailsCaption, dal.Icon, dal.Description, dal.DisplayAs, dal.RequiredRoles.ToArray(), dal.CustomDismissButtonText, dal.HideDismissButton);
    }

    public NotificationItemType GetNotificationItemTypeById(Guid typeId)
    {
      CoreBusinessLayerService.log.Debug((object) "Sending request for NotificationItemTypeDAL.GetTypeById.");
      try
      {
        return CoreBusinessLayerService.DalToWfc(NotificationItemTypeDAL.GetTypeById(typeId));
      }
      catch (Exception ex)
      {
        CoreBusinessLayerService.log.Error((object) ("Error obtaining notification item type: " + ex.ToString()));
        throw new Exception(string.Format("Error obtaining notification item type: ID={0}.", (object) typeId));
      }
    }

    public List<NotificationItemType> GetNotificationItemTypes()
    {
      CoreBusinessLayerService.log.Debug((object) "Sending request for NotificationItemTypeDAL.GetTypes.");
      try
      {
        List<NotificationItemType> notificationItemTypes = new List<NotificationItemType>();
        foreach (NotificationItemTypeDAL type in (IEnumerable<NotificationItemTypeDAL>) NotificationItemTypeDAL.GetTypes())
          notificationItemTypes.Add(CoreBusinessLayerService.DalToWfc(type));
        return notificationItemTypes;
      }
      catch (Exception ex)
      {
        CoreBusinessLayerService.log.Error((object) ("Error obtaining notification item types collection: " + ex.ToString()));
        throw new Exception("Error obtaining notification item types collection.");
      }
    }

    public Dictionary<string, string> GetServicesDisplayNames(List<string> servicesNames)
    {
      return ServiceManager.Instance.GetServicesDisplayNames(servicesNames);
    }

    public Dictionary<string, WindowsServiceRestartState> GetServicesStates(
      List<string> servicesNames)
    {
      return ServiceManager.Instance.GetServicesStates(servicesNames);
    }

    public void RestartServices(List<string> servicesNames)
    {
      ServiceManager.Instance.RestartServices(servicesNames);
    }

    public bool ValidateWMI(string ip, string userName, string password)
    {
      return this.ValidateWMIWithErrorMessage(ip, userName, password, out string _);
    }

    public bool ValidateWMIWithErrorMessage(
      string ip,
      string userName,
      string password,
      out string localizedErrorMessage)
    {
      JobDescription jobDescription = WmiJob<WmiValidateCredentialJobResults>.CreateJobDescription<WmiValidateCredentialJob>(ip, SettingsDAL.GetCurrentInt("SWNetPerfMon-Settings-WMI Retries", 0), Convert.ToBoolean(SettingsDAL.GetCurrentInt("SWNetPerfMon-Settings-WMI Auto Correct Reverse DNS", 0)), SettingsDAL.GetCurrentInt("SWNetPerfMon-Settings-WMI Default Root Namespace Override Index", 0), SettingsDAL.GetCurrentInt("SWNetPerfMon-Settings-WMI Retry Interval", 0), BusinessLayerSettings.Instance.TestJobTimeout);
      WmiCredentials jobCredential = new WmiCredentials()
      {
        Password = password,
        UserName = userName
      };
      localizedErrorMessage = string.Empty;
      WmiValidateCredentialJobResults result = this.ExecuteJobAndGetResult<WmiValidateCredentialJobResults>(jobDescription, (CredentialBase) jobCredential, JobResultDataFormatType.Xml, "WMI", out string _);
      if (!((TestJobResult) result).Success)
      {
        localizedErrorMessage = ((TestJobResult) result).Message;
        return false;
      }
      CoreBusinessLayerService.log.InfoFormat("WMI credential test finished. Success: {0}", (object) result.CredentialsValid);
      return result.CredentialsValid;
    }

    public int? InsertWmiCredential(UsernamePasswordCredential credential, string owner)
    {
      new CredentialManager().AddCredential<UsernamePasswordCredential>(owner, credential);
      return ((Credential) credential).ID;
    }

    public UsernamePasswordCredential GetWmiCredential(int credentialID)
    {
      return new CredentialManager().GetCredential<UsernamePasswordCredential>(credentialID);
    }

    public string GetWmiSysName(string ip, string userName, string password)
    {
      GetSysNameJobResult result = this.ExecuteJobAndGetResult<GetSysNameJobResult>(WmiJob<GetSysNameJobResult>.CreateJobDescription<WmiGetSysNameJob>(ip, SettingsDAL.GetCurrentInt("SWNetPerfMon-Settings-WMI Retries", 0), Convert.ToBoolean(SettingsDAL.GetCurrentInt("SWNetPerfMon-Settings-WMI Auto Correct Reverse DNS", 0)), SettingsDAL.GetCurrentInt("SWNetPerfMon-Settings-WMI Default Root Namespace Override Index", 0), SettingsDAL.GetCurrentInt("SWNetPerfMon-Settings-WMI Retry Interval", 0), BusinessLayerSettings.Instance.TestJobTimeout), (CredentialBase) new WmiCredentials()
      {
        Password = password,
        UserName = userName
      }, JobResultDataFormatType.Xml, "WMI", out string _);
      CoreBusinessLayerService.log.InfoFormat("Wmi GetSysName job finished. SysName: {0}", (object) result.SysName);
      return result.SysName;
    }

    private IServiceProvider ServiceContainer => this.parent.ServiceContainer;

    public CoreBusinessLayerService(
      CoreBusinessLayerPlugin pluginParent,
      IOneTimeJobManager oneTimeJobManager,
      int engineId)
      : this(pluginParent, SolarWinds.Orion.Core.Common.PackageManager.PackageManager.InstanceWithCache, (INodeBLDAL) new NodeBLDAL(), (IAgentInfoDAL) new AgentInfoDAL(), (ISettingsDAL) new SettingsDAL(), oneTimeJobManager, (IEngineDAL) new EngineDAL(), (IEngineIdentityProvider) new EngineIdentityProvider(), engineId)
    {
    }

    internal CoreBusinessLayerService(
      CoreBusinessLayerPlugin pluginParent,
      IPackageManager packageManager,
      INodeBLDAL nodeBlDal,
      IAgentInfoDAL agentInfoDal,
      ISettingsDAL settingsDal,
      IOneTimeJobManager oneTimeJobManager,
      IEngineDAL engineDal,
      IEngineIdentityProvider engineIdentityProvider,
      int engineId)
    {
      if (nodeBlDal == null)
        throw new ArgumentNullException(nameof (nodeBlDal));
      if (agentInfoDal == null)
        throw new ArgumentNullException(nameof (agentInfoDal));
      if (settingsDal == null)
        throw new ArgumentNullException(nameof (settingsDal));
      if (oneTimeJobManager == null)
        throw new ArgumentNullException(nameof (oneTimeJobManager));
      if (engineDal == null)
        throw new ArgumentNullException(nameof (engineDal));
      if (engineIdentityProvider == null)
        throw new ArgumentNullException(nameof (engineIdentityProvider));
      this.parent = pluginParent;
      this._nodeBlDal = nodeBlDal;
      this._agentInfoDal = agentInfoDal;
      this._settingsDal = settingsDal;
      this._auditPluginManager.Initialize();
      this._areInterfacesSupported = packageManager.IsPackageInstalled("Orion.Interfaces");
      this._oneTimeJobManager = oneTimeJobManager;
      this._engineDal = engineDal;
      this._engineIdentityProvider = engineIdentityProvider;
      this._serviceLogicalInstanceId = CoreBusinessLayerConfiguration.GetLogicalInstanceId(engineId);
      SnmpSettings.Instance.Changed += new EventHandler<SettingsChangedEventArgs>(this.SnmpEncodingSettingsChanged);
    }

    public EventWaitHandle ShutdownWaitHandle => (EventWaitHandle) this.shutdownEvent;

    public void Shutdown()
    {
      if (!(JobScheduler.GetInstance() is IChannel instance))
        return;
      MessageUtilities.ShutdownCommunicationObject((ICommunicationObject) instance);
    }

    internal bool AreInterfacesSupported => this._areInterfacesSupported;

    public void CheckBLConnection()
    {
    }

    public void Dispose()
    {
    }

    public IEnumerable<ServiceEndpointDescriptor> GetServiceEndpointDescriptors(
      ServiceDescription serviceDescription)
    {
      ConnectionDescriptorToServiceEndpointMapper endpointMapper = new ConnectionDescriptorToServiceEndpointMapper();
      yield return new ServiceEndpointDescriptor()
      {
        ServiceEndpointProperties = (IReadOnlyDictionary<string, object>) new Dictionary<string, object>()
        {
          {
            "isLocal",
            (object) true
          },
          {
            "isStreamed",
            (object) false
          }
        },
        ConnectionDescriptor = ConnectionDescriptorToServiceEndpointMapperExtensions.Map((IConnectionDescriptorToServiceEndpointMapper) endpointMapper, LegacyServicesSettings.Instance.NetPipeWcfEndpointEnabled ? "CoreBlNamedPipe" : "CoreBlNetTcp", serviceDescription)
      };
      yield return new ServiceEndpointDescriptor()
      {
        ServiceEndpointProperties = (IReadOnlyDictionary<string, object>) new Dictionary<string, object>()
        {
          {
            "isLocal",
            (object) false
          },
          {
            "isStreamed",
            (object) false
          }
        },
        ConnectionDescriptor = ConnectionDescriptorToServiceEndpointMapperExtensions.Map((IConnectionDescriptorToServiceEndpointMapper) endpointMapper, "CoreBlNetTcp", serviceDescription)
      };
      yield return new ServiceEndpointDescriptor()
      {
        ServiceEndpointProperties = (IReadOnlyDictionary<string, object>) new Dictionary<string, object>()
        {
          {
            "isLocal",
            (object) false
          },
          {
            "isStreamed",
            (object) true
          }
        },
        ConnectionDescriptor = ConnectionDescriptorToServiceEndpointMapperExtensions.Map((IConnectionDescriptorToServiceEndpointMapper) endpointMapper, "CoreBlNetTcpStreamed", serviceDescription)
      };
    }

    private CoreBusinessLayerServiceInstance GetCurrentServiceInstance()
    {
      return this.parent.GetServiceInstance(this.GetCurrentOperationEngineId());
    }

    private int GetCurrentOperationEngineId()
    {
      IEngineIdentity iengineIdentity;
      return this._engineIdentityProvider.TryGetCurrent(ref iengineIdentity) ? iengineIdentity.EngineId : throw new InvalidOperationException("Failed to retrieve current EngineId from the operation context.");
    }

    public string ServiceLogicalInstanceId
    {
      get
      {
        CoreBusinessLayerService.log.Info((object) ("Registering to service directory, ServiceId: Core.BusinessLayer, ServiceLogicalInstanceId: " + this._serviceLogicalInstanceId));
        return this._serviceLogicalInstanceId;
      }
    }

    public Version ServiceInstanceVersion
    {
      get => CoreBusinessLayerService.CoreBusinessLayerServiceVersion;
    }

    public class DiscoveryBusinessLayerError : Exception
    {
      internal DiscoveryBusinessLayerError(string format, object[] args, Exception inner)
        : base(string.Format((IFormatProvider) CultureInfo.CurrentUICulture, format ?? string.Empty, args ?? new object[0]), inner)
      {
      }

      internal DiscoveryBusinessLayerError(string format, params object[] args)
        : base(string.Format((IFormatProvider) CultureInfo.CurrentUICulture, format ?? string.Empty, args ?? new object[0]))
      {
      }

      internal DiscoveryBusinessLayerError(string message, Exception inner)
        : base(message, inner)
      {
      }

      internal DiscoveryBusinessLayerError(string message)
        : base(message)
      {
      }
    }

    public class DicoveryDeletingJobError : CoreBusinessLayerService.DiscoveryBusinessLayerError
    {
      internal DicoveryDeletingJobError(string format, params object[] args)
        : base(format, args)
      {
      }
    }

    [Obsolete("Core-Split cleanup. If you need this member please contact Core team", true)]
    public class DicoveryStateError : CoreBusinessLayerService.DiscoveryBusinessLayerError
    {
      internal DicoveryStateError(string format, params object[] args)
        : base(format, args)
      {
      }
    }

    public class DiscoveryInsertingIgnoredNodeError : 
      CoreBusinessLayerService.DiscoveryBusinessLayerError
    {
      internal DiscoveryInsertingIgnoredNodeError(string format, object[] args, Exception inner)
        : base(format, args, inner)
      {
      }
    }

    public class DiscoveryDeletingIgnoredNodeError : 
      CoreBusinessLayerService.DiscoveryBusinessLayerError
    {
      internal DiscoveryDeletingIgnoredNodeError(string format, object[] args, Exception inner)
        : base(format, args, inner)
      {
      }
    }

    public class DiscoveryInsertingIgnoredInterfaceError : 
      CoreBusinessLayerService.DiscoveryBusinessLayerError
    {
      internal DiscoveryInsertingIgnoredInterfaceError(
        string format,
        object[] args,
        Exception inner)
        : base(format, args, inner)
      {
      }
    }

    public class DiscoveryDeletingIgnoredInterfaceError : 
      CoreBusinessLayerService.DiscoveryBusinessLayerError
    {
      internal DiscoveryDeletingIgnoredInterfaceError(
        string format,
        object[] args,
        Exception inner)
        : base(format, args, inner)
      {
      }
    }

    public class DiscoveryInsertingIgnoredVolumeError : 
      CoreBusinessLayerService.DiscoveryBusinessLayerError
    {
      internal DiscoveryInsertingIgnoredVolumeError(string format, object[] args, Exception inner)
        : base(format, args, inner)
      {
      }
    }

    public class DiscoveryDeletingIgnoredVolumeError : 
      CoreBusinessLayerService.DiscoveryBusinessLayerError
    {
      internal DiscoveryDeletingIgnoredVolumeError(string format, object[] args, Exception inner)
        : base(format, args, inner)
      {
      }
    }

    public class DiscoveryHostAddressMissingError : 
      CoreBusinessLayerService.DiscoveryBusinessLayerError
    {
      internal DiscoveryHostAddressMissingError(string format, params object[] args)
        : base(format, args)
      {
      }
    }

    public class DiscoveryJobCancellationError : CoreBusinessLayerService.DiscoveryBusinessLayerError
    {
      internal DiscoveryJobCancellationError(string format, params object[] args)
        : base(format, args)
      {
      }
    }
  }
}
