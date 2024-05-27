// Decompiled with JetBrains decompiler
// Type: SolarWinds.Orion.Core.BusinessLayer.DowntimeMonitoring.DowntimeMonitoringNotificationSubscriber
// Assembly: SolarWinds.Orion.Core.BusinessLayer, Version=2019.4.5200.9083, Culture=neutral, PublicKeyToken=null
// MVID: E12E8C85-5CD9-4E06-8801-182E5104FADE
// Assembly location: E:\task5.dll

using SolarWinds.Common.Utility;
using SolarWinds.InformationService.Contract2;
using SolarWinds.InformationService.Contract2.PubSub;
using SolarWinds.Logging;
using SolarWinds.Orion.Core.Common;
using SolarWinds.Orion.Core.Common.DALs;
using SolarWinds.Orion.Core.Common.Indications;
using SolarWinds.Orion.Core.Common.InformationService;
using SolarWinds.Orion.Core.Common.Models;
using SolarWinds.Orion.Core.Common.Swis;
using SolarWinds.Orion.Swis.Contract.InformationService;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;

#nullable disable
namespace SolarWinds.Orion.Core.BusinessLayer.DowntimeMonitoring
{
  public class DowntimeMonitoringNotificationSubscriber : INotificationSubscriber
  {
    private const string NetObjectDowntimeIndication = "NetObjectDowntimeIndication";
    private const string NetObjectDowntimeInitializator = "NetObjectDowntimeInitializator";
    private string _nodeNetObjectIdColumn;
    protected static readonly Log log = new Log();
    private readonly INetObjectDowntimeDAL _netObjectDowntimeDal;
    private readonly IInformationServiceProxyCreator _swisServiceProxyCreator;
    private readonly ISwisUriParser _swisUriParser;
    private Lazy<ILookup<string, NetObjectTypeEx>> _netObjectTypes;
    private static readonly Regex NodeIdRegex = new Regex("(N:\\d+)", RegexOptions.Compiled);
    private readonly List<string> subscriptionUris = new List<string>();

    internal Lazy<ILookup<string, NetObjectTypeEx>> NetObjectTypes
    {
      set => this._netObjectTypes = value;
    }

    private static bool IsEnabled
    {
      get => SettingsDAL.GetCurrent<bool>("SWNetPerfMon-Settings-EnableDowntimeMonitoring", true);
    }

    public DowntimeMonitoringNotificationSubscriber(INetObjectDowntimeDAL netObjectDowntimeDal)
      : this(netObjectDowntimeDal, (IInformationServiceProxyCreator) SwisConnectionProxyPool.GetSystemCreator(), (ISwisUriParser) new SwisUriParser())
    {
    }

    internal DowntimeMonitoringNotificationSubscriber(
      INetObjectDowntimeDAL netObjectDowntimeDal,
      IInformationServiceProxyCreator serviceProxyCreator,
      ISwisUriParser swisUriParser)
    {
      this._netObjectDowntimeDal = netObjectDowntimeDal ?? throw new ArgumentNullException(nameof (netObjectDowntimeDal));
      this._swisServiceProxyCreator = serviceProxyCreator ?? throw new ArgumentNullException(nameof (serviceProxyCreator));
      this._swisUriParser = swisUriParser ?? throw new ArgumentNullException(nameof (swisUriParser));
      this._nodeNetObjectIdColumn = (string) null;
      this._netObjectTypes = new Lazy<ILookup<string, NetObjectTypeEx>>(new Func<ILookup<string, NetObjectTypeEx>>(this.LoadNetObjectTypesExtSwisInfo), LazyThreadSafetyMode.PublicationOnly);
    }

    public void OnIndication(
      string subscriptionId,
      string indicationType,
      PropertyBag indicationProperties,
      PropertyBag sourceInstanceProperties)
    {
      Stopwatch stopwatch = new Stopwatch();
      try
      {
        stopwatch.Start();
        if (sourceInstanceProperties == null)
          throw new ArgumentNullException(nameof (sourceInstanceProperties));
        if (DowntimeMonitoringNotificationSubscriber.log.IsDebugEnabled)
          DowntimeMonitoringNotificationSubscriber.log.Debug((object) this.DetailInfo(subscriptionId, indicationType, indicationProperties, sourceInstanceProperties));
        string instanceType = sourceInstanceProperties.TryGet<string>("InstanceType") ?? sourceInstanceProperties.TryGet<string>("SourceInstanceType");
        if (instanceType == null)
        {
          DowntimeMonitoringNotificationSubscriber.log.Error((object) "Wrong PropertyBag data. InstanceType or SourceInstanceType are null");
        }
        else
        {
          string columnForSwisEntity = this.GetNetObjectIdColumnForSwisEntity(instanceType);
          if (columnForSwisEntity == null)
          {
            DowntimeMonitoringNotificationSubscriber.log.DebugFormat("Not a supported instance type: {0}", (object) instanceType);
          }
          else
          {
            object obj;
            if (!((Dictionary<string, object>) sourceInstanceProperties).TryGetValue(columnForSwisEntity, out obj))
              DowntimeMonitoringNotificationSubscriber.log.DebugFormat("Unable to get Entity ID. InstanceType : {0}, ID Field: {1}", (object) instanceType, (object) columnForSwisEntity);
            else if (indicationType == IndicationHelper.GetIndicationType((IndicationType) 2) || indicationType == IndicationHelper.GetIndicationType((IndicationType) 0))
            {
              object statusObject1;
              ((Dictionary<string, object>) sourceInstanceProperties).TryGetValue("Status", out statusObject1);
              if (statusObject1 == null)
              {
                DowntimeMonitoringNotificationSubscriber.log.DebugFormat("No Status reported for InstanceType : {0}", (object) instanceType);
              }
              else
              {
                if (this._nodeNetObjectIdColumn == null)
                  this._nodeNetObjectIdColumn = this.GetNetObjectIdColumnForSwisEntity("Orion.Nodes");
                object statusObject2;
                ((Dictionary<string, object>) sourceInstanceProperties).TryGetValue(this._nodeNetObjectIdColumn, out statusObject2);
                if (statusObject2 == null)
                  DowntimeMonitoringNotificationSubscriber.log.DebugFormat("SourceBag must include NodeId. InstanceType : {0}", (object) instanceType);
                else
                  this._netObjectDowntimeDal.Insert(new NetObjectDowntime()
                  {
                    EntityID = obj.ToString(),
                    NodeID = this.ExtractStatusID(statusObject2),
                    EntityType = instanceType,
                    DateTimeFrom = (DateTime) ((Dictionary<string, object>) indicationProperties)[IndicationConstants.IndicationTime],
                    StatusID = this.ExtractStatusID(statusObject1)
                  });
              }
            }
            else
            {
              if (!(indicationType == IndicationHelper.GetIndicationType((IndicationType) 1)))
                return;
              this._netObjectDowntimeDal.DeleteDowntimeObjects(obj.ToString(), instanceType);
            }
          }
        }
      }
      catch (Exception ex)
      {
        DowntimeMonitoringNotificationSubscriber.log.Error((object) string.Format("Exception occured when processing incoming indication of type \"{0}\"", (object) indicationType), ex);
      }
      finally
      {
        stopwatch.Stop();
        DowntimeMonitoringNotificationSubscriber.log.DebugFormat("Downtime notification has been processed in {0} miliseconds.", (object) stopwatch.ElapsedMilliseconds);
      }
    }

    internal int ExtractStatusID(object statusObject) => Convert.ToInt32(statusObject);

    public virtual void Start()
    {
      if (!DowntimeMonitoringNotificationSubscriber.IsEnabled)
      {
        DowntimeMonitoringNotificationSubscriber.log.Info((object) "Subscription of Downtime Monitoring cancelled (disabled by Settings option)");
        this.Stop();
      }
      else
      {
        Scheduler.Instance.Add(new ScheduledTask("NetObjectDowntimeInitializator", new TimerCallback(this.Initialize), (object) null, TimeSpan.FromSeconds(1.0), TimeSpan.FromMinutes(1.0)));
        Scheduler.Instance.Add(new ScheduledTask("NetObjectDowntimeIndication", new TimerCallback(this.Subscribe), (object) null, TimeSpan.FromSeconds(1.0), TimeSpan.FromMinutes(1.0)));
      }
    }

    private void Initialize(object state)
    {
      List<NetObjectDowntime> netObjectDowntimeList = new List<NetObjectDowntime>();
      try
      {
        using (IInformationServiceProxy2 iinformationServiceProxy2 = this._swisServiceProxyCreator.Create())
        {
          DateTime utcNow = DateTime.UtcNow;
          foreach (DataRow row in (InternalDataCollectionBase) ((IInformationServiceProxy) iinformationServiceProxy2).Query("SELECT Uri, Status, InstanceType, AncestorDetailsUrls\r\n                                            FROM System.ManagedEntity\r\n                                            WHERE UnManaged = false").Rows)
          {
            try
            {
              if (this.IsValid(row))
                netObjectDowntimeList.Add(new NetObjectDowntime()
                {
                  DateTimeFrom = utcNow,
                  EntityID = this._swisUriParser.GetEntityId(row["Uri"].ToString()),
                  NodeID = this.ExtractStatusID((object) this.GetNodeIDFromUrl((string[]) row["AncestorDetailsUrls"])),
                  EntityType = row["InstanceType"].ToString(),
                  StatusID = (int) row["Status"]
                });
            }
            catch (Exception ex)
            {
              DowntimeMonitoringNotificationSubscriber.log.Error((object) string.Format("Unable to create NetObjectDowntime instance from ManagedEntity with Uri '{0}', {1}", row["Uri"], (object) ex));
            }
          }
        }
      }
      catch (Exception ex)
      {
        DowntimeMonitoringNotificationSubscriber.log.ErrorFormat("Exception while initializing NetObjectDowntime table with ManagedEntities. {0}", (object) ex);
      }
      this._netObjectDowntimeDal.Insert((IEnumerable<NetObjectDowntime>) netObjectDowntimeList);
      Scheduler.Instance.Remove("NetObjectDowntimeInitializator");
    }

    private bool IsValid(DataRow row)
    {
      try
      {
        this.GetNodeIDFromUrl((string[]) row["AncestorDetailsUrls"]);
        return !row.IsNull("Uri");
      }
      catch (Exception ex)
      {
        return false;
      }
    }

    public virtual void Stop()
    {
      this.Unsubscribe();
      Scheduler.Instance.Remove("NetObjectDowntimeIndication");
    }

    private void Unsubscribe()
    {
      if (this.subscriptionUris.Count == 0)
        return;
      try
      {
        foreach (string subscriptionUri in this.subscriptionUris)
          ((InformationServiceSubscriptionProviderBase) InformationServiceSubscriptionProviderShared.Instance()).Unsubscribe(subscriptionUri);
        this.subscriptionUris.Clear();
      }
      catch (Exception ex)
      {
        DowntimeMonitoringNotificationSubscriber.log.ErrorFormat("Unsubscribe failed: '{0}'", (object) ex);
        throw;
      }
    }

    private void Subscribe(object state)
    {
      DowntimeMonitoringNotificationSubscriber.log.Debug((object) "Subscribing Managed Entity changed indications..");
      try
      {
        try
        {
          this.DeleteOldSubscriptions();
        }
        catch (Exception ex)
        {
          DowntimeMonitoringNotificationSubscriber.log.Warn((object) "Exception deleting old subscriptions:", ex);
        }
        if (this.subscriptionUris.Count > 0)
          this.Unsubscribe();
        string[] strArray = new string[3]
        {
          "SUBSCRIBE System.InstanceDeleted WHEN InstanceType ISA 'System.ManagedEntity' OR SourceInstanceType ISA 'System.ManagedEntity'",
          "SUBSCRIBE System.InstanceCreated WHEN InstanceType ISA 'System.ManagedEntity' OR SourceInstanceType ISA 'System.ManagedEntity'",
          "SUBSCRIBE CHANGES TO System.ManagedEntity WHEN Status CHANGES"
        };
        foreach (string str1 in strArray)
        {
          string str2 = ((InformationServiceSubscriptionProviderBase) InformationServiceSubscriptionProviderShared.Instance()).Subscribe(str1, (INotificationSubscriber) this, new SubscriptionOptions()
          {
            Description = "NetObjectDowntimeIndication"
          });
          this.subscriptionUris.Add(str2);
          DowntimeMonitoringNotificationSubscriber.log.InfoFormat("Pub/sub subscription succeeded. uri:'{0}'", (object) str2);
        }
        Scheduler.Instance.Remove("NetObjectDowntimeIndication");
      }
      catch (Exception ex)
      {
        DowntimeMonitoringNotificationSubscriber.log.ErrorFormat("{0} in Subscribe: {1}\r\n{2}", (object) ex.GetType(), (object) ex.Message, (object) ex.StackTrace);
      }
    }

    private string DetailInfo(
      string subscriptionId,
      string indicationType,
      PropertyBag indicationProperties,
      PropertyBag sourceInstanceProperties)
    {
      return string.Format("Pub/Sub Notification: ID: {0}, Type: {1}, IndicationProperties: {2}, InstanceProperties: {3}", (object) subscriptionId, (object) indicationType, (object) string.Join(", ", ((IEnumerable<KeyValuePair<string, object>>) indicationProperties).Select<KeyValuePair<string, object>, string>((System.Func<KeyValuePair<string, object>, string>) (kvp => string.Format("{0} = {1}", (object) kvp.Key, kvp.Value)))), ((Dictionary<string, object>) sourceInstanceProperties).Count > 0 ? (object) string.Join(", ", ((IEnumerable<KeyValuePair<string, object>>) sourceInstanceProperties).Select<KeyValuePair<string, object>, string>((System.Func<KeyValuePair<string, object>, string>) (kvp => string.Format("{0} = {1}", (object) kvp.Key, kvp.Value)))) : (object) string.Empty);
    }

    internal string GetNodeIDFromUrl(string[] urls)
    {
      foreach (string url in urls)
      {
        Match match = DowntimeMonitoringNotificationSubscriber.NodeIdRegex.Match(url);
        if (match.Success)
          return NetObjectHelper.GetObjectID(match.Value);
      }
      throw new ArgumentException(string.Format("Cannot parse NodeId from AncestorUrl. Urls: {0}.", (object) string.Join(",", urls)), nameof (urls));
    }

    internal string GetNetObjectIdColumnForSwisEntity(string instanceType)
    {
      string columnForSwisEntity = (string) null;
      if (this._netObjectTypes == null || this._netObjectTypes.Value == null)
        return (string) null;
      NetObjectTypeEx netObjectTypeEx = this._netObjectTypes.Value[instanceType].FirstOrDefault<NetObjectTypeEx>((System.Func<NetObjectTypeEx, bool>) (n => n.KeyPropertyIndex == 0));
      if (netObjectTypeEx != null)
        columnForSwisEntity = netObjectTypeEx.KeyProperty;
      return columnForSwisEntity;
    }

    private ILookup<string, NetObjectTypeEx> LoadNetObjectTypesExtSwisInfo()
    {
      using (IInformationServiceProxy2 iinformationServiceProxy2 = this._swisServiceProxyCreator.Create())
        return ((IInformationServiceProxy) iinformationServiceProxy2).Query("SELECT EntityType, Name, Prefix, KeyProperty, NameProperty, KeyPropertyIndex, CanConvert FROM Orion.NetObjectTypesExt").Rows.Cast<DataRow>().Select<DataRow, NetObjectTypeEx>((System.Func<DataRow, NetObjectTypeEx>) (row => new NetObjectTypeEx(row.Field<string>("EntityType"), row.Field<string>("Name"), row.Field<string>("Prefix"), row.Field<string>("KeyProperty"), row.Field<string>("NameProperty"), (int) row.Field<short>("CanConvert"), row.Field<int>("KeyPropertyIndex")))).ToLookup<NetObjectTypeEx, string>((System.Func<NetObjectTypeEx, string>) (k => k.EntityType));
    }

    private void DeleteOldSubscriptions()
    {
      using (IInformationServiceProxy2 iinformationServiceProxy2_1 = this._swisServiceProxyCreator.Create())
      {
        string str1 = "SELECT Uri FROM System.Subscription WHERE description = @description";
        IInformationServiceProxy2 iinformationServiceProxy2_2 = iinformationServiceProxy2_1;
        string str2 = str1;
        foreach (DataRow dataRow in ((IInformationServiceProxy) iinformationServiceProxy2_2).Query(str2, (IDictionary<string, object>) new Dictionary<string, object>()
        {
          {
            "description",
            (object) "NetObjectDowntimeIndication"
          }
        }).Rows.Cast<DataRow>())
          ((IInformationServiceProxy) iinformationServiceProxy2_1).Delete(dataRow[0].ToString());
      }
    }
  }
}
