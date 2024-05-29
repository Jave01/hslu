// Decompiled with JetBrains decompiler
// Type: SolarWinds.Orion.Core.BusinessLayer.DowntimeMonitoring.DowntimeMonitoringEnableSubscriber
// Assembly: SolarWinds.Orion.Core.BusinessLayer, Version=2019.4.5200.9083, Culture=neutral, PublicKeyToken=null
// MVID: E12E8C85-5CD9-4E06-8801-182E5104FADE
// Assembly location: E:\task5.dll

using SolarWinds.InformationService.Contract2;
using SolarWinds.InformationService.Contract2.PubSub;
using SolarWinds.Logging;
using SolarWinds.Orion.Common;
using SolarWinds.Orion.Core.Common.InformationService;
using SolarWinds.Orion.Swis.Contract.InformationService;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

#nullable disable
namespace SolarWinds.Orion.Core.BusinessLayer.DowntimeMonitoring
{
  public class DowntimeMonitoringEnableSubscriber : INotificationSubscriber
  {
    private static string SettingsKey = "SWNetPerfMon-Settings-EnableDowntimeMonitoring";
    private const string DowntimeMonitoringEnableIndication = "DowntimeMonitoringEnableIndication";
    protected static readonly Log Log = new Log();
    private readonly InformationServiceSubscriptionProviderBase subscriptionProvider;
    private DowntimeMonitoringNotificationSubscriber downtimeMonitoringSubscriber;
    private string subscriptionId;

    public DowntimeMonitoringEnableSubscriber(
      DowntimeMonitoringNotificationSubscriber downtimeMonitoringSubscriber)
      : this((InformationServiceSubscriptionProviderBase) InformationServiceSubscriptionProviderShared.Instance(), downtimeMonitoringSubscriber)
    {
    }

    public DowntimeMonitoringEnableSubscriber(
      InformationServiceSubscriptionProviderBase subscriptionProvider,
      DowntimeMonitoringNotificationSubscriber downtimeMonitoringSubscriber)
    {
      if (subscriptionProvider == null)
        throw new ArgumentNullException(nameof (subscriptionProvider));
      if (downtimeMonitoringSubscriber == null)
        throw new ArgumentNullException(nameof (downtimeMonitoringSubscriber));
      this.subscriptionProvider = subscriptionProvider;
      this.downtimeMonitoringSubscriber = downtimeMonitoringSubscriber;
    }

    public DowntimeMonitoringNotificationSubscriber DowntimeMonitoringSubscriber
    {
      get => this.downtimeMonitoringSubscriber;
      set
      {
        this.downtimeMonitoringSubscriber = value != null ? value : throw new ArgumentNullException(nameof (DowntimeMonitoringSubscriber));
      }
    }

    public void OnIndication(
      string subscriptionId,
      string indicationType,
      PropertyBag indicationProperties,
      PropertyBag sourceInstanceProperties)
    {
      if (sourceInstanceProperties == null)
        DowntimeMonitoringEnableSubscriber.Log.Error((object) "Argument sourceInstanceProperties is null");
      else if (!((Dictionary<string, object>) sourceInstanceProperties).ContainsKey("CurrentValue"))
      {
        DowntimeMonitoringEnableSubscriber.Log.Error((object) "CurrentValue not supplied in sourceInstanceProperties");
      }
      else
      {
        try
        {
          DowntimeMonitoringEnableSubscriber.Log.DebugFormat("Downtime monitoring changed to {0}, unsubscribing..", ((Dictionary<string, object>) sourceInstanceProperties)["CurrentValue"]);
          int num = Convert.ToBoolean(((Dictionary<string, object>) sourceInstanceProperties)["CurrentValue"]) ? 1 : 0;
          this.downtimeMonitoringSubscriber.Stop();
          if (num != 0)
          {
            DowntimeMonitoringEnableSubscriber.Log.Debug((object) "Re-subscribing..");
            this.downtimeMonitoringSubscriber.Start();
          }
          else
            this.SealIntervals();
        }
        catch (Exception ex)
        {
          DowntimeMonitoringEnableSubscriber.Log.Error((object) "Indication handling failed", ex);
        }
      }
    }

    private void SealIntervals()
    {
      using (SqlCommand textCommand = SqlHelper.GetTextCommand("UPDATE [dbo].[NetObjectDowntime] SET [DateTimeUntil] = @now WHERE [DateTimeUntil] IS NULL"))
      {
        textCommand.Parameters.AddWithValue("@now", (object) DateTime.Now.ToUniversalTime());
        SqlHelper.ExecuteNonQuery(textCommand);
      }
    }

    public void Start()
    {
      DowntimeMonitoringEnableSubscriber.Log.Debug((object) "Subscribing DowntimeMonitoringEnableSubscriber changed indications..");
      if (this.subscriptionId != null)
      {
        DowntimeMonitoringEnableSubscriber.Log.Debug((object) "Already subscribed, unsubscribing first..");
        this.Stop(false);
      }
      try
      {
        this.subscriptionId = this.subscriptionProvider.Subscribe(string.Format("SUBSCRIBE CHANGES TO Orion.Settings WHEN SettingsID = '{0}'", (object) DowntimeMonitoringEnableSubscriber.SettingsKey), (INotificationSubscriber) this, new SubscriptionOptions()
        {
          Description = "DowntimeMonitoringEnableIndication"
        });
        DowntimeMonitoringEnableSubscriber.Log.TraceFormat("Subscribed with URI '{0}'", new object[1]
        {
          (object) this.subscriptionId
        });
      }
      catch (Exception ex)
      {
        DowntimeMonitoringEnableSubscriber.Log.Error((object) "Failed to subscribe", ex);
        throw;
      }
    }

    public void Stop(bool sealInterval = true)
    {
      DowntimeMonitoringEnableSubscriber.Log.Debug((object) "Unsubscribing DowntimeMonitoringEnableSubscriber changed indications..");
      if (sealInterval)
      {
        try
        {
          this.SealIntervals();
        }
        catch (Exception ex)
        {
          DowntimeMonitoringEnableSubscriber.Log.Error((object) "Failed to seal intervals", ex);
          throw;
        }
      }
      if (this.subscriptionId == null)
      {
        DowntimeMonitoringEnableSubscriber.Log.Debug((object) "SubscriptionUri not set, no action performed");
      }
      else
      {
        try
        {
          this.subscriptionProvider.Unsubscribe(this.subscriptionId);
          this.subscriptionId = (string) null;
        }
        catch (Exception ex)
        {
          DowntimeMonitoringEnableSubscriber.Log.Error((object) "Failed to unsubscribe", ex);
          throw;
        }
      }
    }
  }
}
