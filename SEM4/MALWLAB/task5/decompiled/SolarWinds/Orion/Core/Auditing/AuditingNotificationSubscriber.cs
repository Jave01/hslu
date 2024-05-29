// Decompiled with JetBrains decompiler
// Type: SolarWinds.Orion.Core.Auditing.AuditingNotificationSubscriber
// Assembly: SolarWinds.Orion.Core.BusinessLayer, Version=2019.4.5200.9083, Culture=neutral, PublicKeyToken=null
// MVID: E12E8C85-5CD9-4E06-8801-182E5104FADE
// Assembly location: E:\task5.dll

using SolarWinds.Common.Utility;
using SolarWinds.InformationService.Contract2;
using SolarWinds.InformationService.Contract2.PubSub;
using SolarWinds.Logging;
using SolarWinds.Orion.Core.BusinessLayer;
using SolarWinds.Orion.Core.BusinessLayer.DAL;
using SolarWinds.Orion.Core.Common;
using SolarWinds.Orion.Core.Common.i18n;
using SolarWinds.Orion.Core.Common.Indications;
using SolarWinds.Orion.Core.Common.InformationService;
using SolarWinds.Orion.Core.Common.Swis;
using SolarWinds.Orion.Swis.Contract.InformationService;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Threading;

#nullable disable
namespace SolarWinds.Orion.Core.Auditing
{
  internal class AuditingNotificationSubscriber : INotificationSubscriber
  {
    private const string AuditingIndications = "AuditingIndications";
    private static readonly Log log = new Log(typeof (AuditingNotificationSubscriber));
    private IndicationPublisher indicationPublisher;
    private readonly AuditingPluginManager auditingPlugins = new AuditingPluginManager();
    private readonly AuditingDAL auditingDAL = new AuditingDAL();
    private bool checkAuditingSetting = true;
    private readonly ConcurrentDictionary<string, IEnumerable<IAuditing2>> subscriptionIdToAuditingInstances = new ConcurrentDictionary<string, IEnumerable<IAuditing2>>();

    protected bool AuditingTrailsEnabled { get; private set; }

    public void OnIndication(
      string subscriptionId,
      string indicationType,
      PropertyBag indicationProperties,
      PropertyBag sourceInstanceProperties)
    {
      if (AuditingNotificationSubscriber.log.IsDebugEnabled)
        AuditingNotificationSubscriber.log.DebugFormat("OnIndication type: {0} SubscriptionId: {1}", (object) indicationType, (object) subscriptionId);
      if (this.checkAuditingSetting)
      {
        try
        {
          object obj;
          if (IndicationHelper.GetIndicationType((IndicationType) 2) == indicationType && sourceInstanceProperties != null && sourceInstanceProperties.TryGet<string>("SettingsID") == "SWNetPerfMon-AuditingTrails" && sourceInstanceProperties.TryGet<string>("InstanceType") == "Orion.Settings" && ((Dictionary<string, object>) sourceInstanceProperties).TryGetValue("CurrentValue", out obj))
            this.AuditingTrailsEnabled = Convert.ToBoolean(obj);
          else if (!this.AuditingTrailsEnabled)
            return;
        }
        catch (Exception ex)
        {
          AuditingNotificationSubscriber.log.FatalFormat("Auditing check error - will be forciby enabled. {0}", (object) ex);
          this.AuditingTrailsEnabled = true;
          this.checkAuditingSetting = false;
        }
      }
      AuditNotificationContainer auditNotificationContainer = new AuditNotificationContainer(indicationType, indicationProperties, sourceInstanceProperties);
      foreach (IAuditing2 iauditing2 in this.subscriptionIdToAuditingInstances[subscriptionId])
      {
        try
        {
          if (AuditingNotificationSubscriber.log.IsTraceEnabled)
            AuditingNotificationSubscriber.log.TraceFormat("Trying plugin {0}", new object[1]
            {
              (object) iauditing2
            });
          IEnumerable<AuditDataContainer> source = ((IAuditing) iauditing2).ComposeDataContainers(auditNotificationContainer);
          if (source != null)
          {
            if (AuditingNotificationSubscriber.log.IsTraceEnabled)
              AuditingNotificationSubscriber.log.Trace((object) "Storing notification.");
            CultureInfo currentUiCulture = Thread.CurrentThread.CurrentUICulture;
            try
            {
              Thread.CurrentThread.CurrentUICulture = LocaleConfiguration.GetNonNeutralLocale(LocaleConfiguration.PrimaryLocale);
            }
            catch (Exception ex)
            {
              AuditingNotificationSubscriber.log.Warn((object) "Unable set CurrentUICulture to PrimaryLocale.", ex);
            }
            foreach (AuditDataContainer adc in source.Select<AuditDataContainer, AuditDataContainer>((System.Func<AuditDataContainer, AuditDataContainer>) (composedDataContainer => new AuditDataContainer(composedDataContainer, auditNotificationContainer.AccountId))))
            {
              AuditDatabaseDecoratedContainer decoratedContainer = new AuditDatabaseDecoratedContainer(adc, auditNotificationContainer, ((IAuditing) iauditing2).GetMessage(adc));
              int insertedId = this.auditingDAL.StoreNotification(decoratedContainer);
              this.PublishModificationOfAuditingEvents(decoratedContainer, insertedId);
            }
            try
            {
              Thread.CurrentThread.CurrentUICulture = currentUiCulture;
            }
            catch (Exception ex)
            {
              AuditingNotificationSubscriber.log.Warn((object) "Unable set CurrentUICulture back to original locale.", ex);
            }
          }
          else if (AuditingNotificationSubscriber.log.IsTraceEnabled)
            AuditingNotificationSubscriber.log.Trace((object) "ComposeDataContainers returned null.");
        }
        catch (Exception ex)
        {
          string seed = string.Empty;
          if (indicationProperties != null)
            seed = ((IEnumerable<KeyValuePair<string, object>>) indicationProperties).Aggregate<KeyValuePair<string, object>, string>(Environment.NewLine, (Func<string, KeyValuePair<string, object>, string>) ((current, item) => current + this.FormatPropertyData("Indication Property: ", item.Key, item.Value)));
          if (sourceInstanceProperties != null)
            seed = ((IEnumerable<KeyValuePair<string, object>>) sourceInstanceProperties).Aggregate<KeyValuePair<string, object>, string>(seed, (Func<string, KeyValuePair<string, object>, string>) ((current, item) => current + this.FormatPropertyData("SourceInstance Property: ", item.Key, item.Value)));
          AuditingNotificationSubscriber.log.ErrorFormat("Auditing translation failed. IndicationType: {0}, {1} PluginName: {2}, subscriptionId: {3} Exception: {4}", new object[5]
          {
            (object) indicationType,
            (object) seed,
            (object) ((IAuditing) iauditing2).PluginName,
            (object) subscriptionId,
            (object) ex
          });
        }
      }
    }

    private void PublishModificationOfAuditingEvents(
      AuditDatabaseDecoratedContainer auditDatabaseDecoratedContainer,
      int insertedId)
    {
      if (this.indicationPublisher == null)
        this.indicationPublisher = IndicationPublisher.CreateV3();
      Dictionary<string, object> dictionary = new Dictionary<string, object>()
      {
        {
          "ActionType",
          (object) auditDatabaseDecoratedContainer.ActionType.ToString()
        },
        {
          "AuditEventId",
          (object) insertedId
        },
        {
          "InstanceType",
          (object) "Orion.AuditingEvents"
        },
        {
          "OriginalAccountId",
          (object) auditDatabaseDecoratedContainer.AccountId
        }
      };
      this.indicationPublisher.Publish(new Indication()
      {
        IndicationProperties = IndicationHelper.GetIndicationProperties(),
        IndicationType = "System.InstanceCreated",
        SourceInstanceProperties = new PropertyBag((IDictionary<string, object>) dictionary)
      });
    }

    private string FormatPropertyData(string prefix, string key, object value)
    {
      return prefix + key + ": " + (value ?? (object) "null") + Environment.NewLine;
    }

    public void Start()
    {
      try
      {
        this.AuditingTrailsEnabled = SettingsDAL.GetCurrent<bool>("SWNetPerfMon-AuditingTrails", true);
      }
      catch (Exception ex)
      {
        AuditingNotificationSubscriber.log.FatalFormat("Auditing setting error - will be forciby enabled. {0}", (object) ex);
        this.AuditingTrailsEnabled = true;
      }
      this.checkAuditingSetting = true;
      this.auditingPlugins.Initialize();
      Scheduler.Instance.Add(new ScheduledTask("AuditingIndications", new TimerCallback(this.Subscribe), (object) null, TimeSpan.FromSeconds(1.0), TimeSpan.FromMinutes(1.0)), true);
    }

    public void Stop() => Scheduler.Instance.Remove("AuditingIndications");

    private void Subscribe(object state)
    {
      AuditingNotificationSubscriber.log.Debug((object) "Subscribing auditing indications..");
      try
      {
        AuditingNotificationSubscriber.DeleteOldSubscriptions();
      }
      catch (Exception ex)
      {
        AuditingNotificationSubscriber.log.Warn((object) "Exception deleting old subscriptions:", ex);
      }
      HashSet<string> stringSet = new HashSet<string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      foreach (IAuditing2 auditingInstance in this.auditingPlugins.AuditingInstances)
      {
        if (auditingInstance is IAuditingMultiSubscription multiSubscription)
        {
          foreach (string subscriptionQuery in multiSubscription.GetSubscriptionQueries())
            stringSet.Add(subscriptionQuery);
        }
        else
          stringSet.Add(auditingInstance.GetSubscriptionQuery());
      }
      foreach (string str in stringSet)
      {
        try
        {
          AuditingNotificationSubscriber.log.DebugFormat("Subscribing '{0}'", (object) str);
          string key = ((InformationServiceSubscriptionProviderBase) InformationServiceSubscriptionProviderShared.Instance()).Subscribe(str, (INotificationSubscriber) this, new SubscriptionOptions()
          {
            Description = "AuditingIndications"
          });
          string query1 = str;
          this.subscriptionIdToAuditingInstances.TryAdd(key, this.auditingPlugins.AuditingInstances.Where<IAuditing2>((System.Func<IAuditing2, bool>) (instance =>
          {
            try
            {
              return string.Compare(query1, instance.GetSubscriptionQuery(), StringComparison.OrdinalIgnoreCase) == 0;
            }
            catch (NotImplementedException ex)
            {
              return instance is IAuditingMultiSubscription multiSubscription2 && ((IEnumerable<string>) multiSubscription2.GetSubscriptionQueries()).Contains<string>(query1);
            }
          })));
          AuditingNotificationSubscriber.log.DebugFormat("Subscribed '{0}' with {1} number of auditing instances.", (object) str, (object) this.subscriptionIdToAuditingInstances[key].Count<IAuditing2>());
        }
        catch (Exception ex)
        {
          AuditingNotificationSubscriber.log.ErrorFormat("Unable to subscribe auditing instance with query '{0}'. {1}", (object) str, (object) ex);
        }
      }
      AuditingNotificationSubscriber.log.InfoFormat("Auditing pub/sub subscription succeeded.", Array.Empty<object>());
      Scheduler.Instance.Remove("AuditingIndications");
    }

    private static void DeleteOldSubscriptions()
    {
      using (IInformationServiceProxy2 iinformationServiceProxy2_1 = ((IInformationServiceProxyCreator) SwisConnectionProxyPool.GetSystemCreator()).Create())
      {
        string str1 = "SELECT Uri FROM System.Subscription WHERE description = @description";
        IInformationServiceProxy2 iinformationServiceProxy2_2 = iinformationServiceProxy2_1;
        string str2 = str1;
        foreach (DataRow dataRow in ((IInformationServiceProxy) iinformationServiceProxy2_2).Query(str2, (IDictionary<string, object>) new Dictionary<string, object>()
        {
          {
            "description",
            (object) "AuditingIndications"
          }
        }).Rows.Cast<DataRow>())
          ((IInformationServiceProxy) iinformationServiceProxy2_1).Delete(dataRow[0].ToString());
      }
    }
  }
}
