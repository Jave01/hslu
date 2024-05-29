// Decompiled with JetBrains decompiler
// Type: SolarWinds.Orion.Core.BusinessLayer.MaintenanceMode.MaintenanceIndicationSubscriber
// Assembly: SolarWinds.Orion.Core.BusinessLayer, Version=2019.4.5200.9083, Culture=neutral, PublicKeyToken=null
// MVID: E12E8C85-5CD9-4E06-8801-182E5104FADE
// Assembly location: E:\task5.dll

using SolarWinds.InformationService.Contract2;
using SolarWinds.InformationService.Contract2.PubSub;
using SolarWinds.Logging;
using SolarWinds.Orion.Core.BusinessLayer.DAL;
using SolarWinds.Orion.Core.Common.Indications;
using SolarWinds.Orion.Core.Common.InformationService;
using SolarWinds.Orion.Core.Models.MaintenanceMode;
using System;
using System.Collections.Generic;
using System.Linq;

#nullable disable
namespace SolarWinds.Orion.Core.BusinessLayer.MaintenanceMode
{
  internal class MaintenanceIndicationSubscriber : INotificationSubscriber, IDisposable
  {
    private static readonly Log log = new Log();
    private readonly IMaintenanceManager manager;
    private readonly InformationServiceSubscriptionProviderBase subscriptionProvider;
    private string subscriptionId;
    private const string SubscriptionQuery = "SUBSCRIBE CHANGES TO Orion.MaintenancePlanAssignment";

    public MaintenanceIndicationSubscriber()
      : this((IMaintenanceManager) new MaintenanceManager(InformationServiceProxyPoolCreatorFactory.GetSystemCreator(), (IMaintenanceModePlanDAL) new MaintenanceModePlanDAL()), (InformationServiceSubscriptionProviderBase) InformationServiceSubscriptionProviderShared.Instance())
    {
    }

    public MaintenanceIndicationSubscriber(
      IMaintenanceManager manager,
      InformationServiceSubscriptionProviderBase subscriptionProvider)
    {
      this.manager = manager;
      this.subscriptionProvider = subscriptionProvider;
    }

    public void Start()
    {
      try
      {
        this.subscriptionId = this.Subscribe();
      }
      catch (Exception ex)
      {
        MaintenanceIndicationSubscriber.log.ErrorFormat("Unable to start maintenance mode service. Unmanage functionality may be affected. {0}", (object) ex);
        throw;
      }
    }

    public void OnIndication(
      string subscriptionId,
      string indicationType,
      PropertyBag indicationProperties,
      PropertyBag sourceInstanceProperties)
    {
      if (this.subscriptionId != subscriptionId)
        return;
      MaintenanceIndicationSubscriber.log.DebugFormat("Received maintenance mode indication '{0}'.", (object) indicationType);
      MaintenanceIndicationSubscriber.log.DebugFormat("Indication Properties: {0}", (object) indicationProperties);
      MaintenanceIndicationSubscriber.log.DebugFormat("Source Instance Properties: {0}", (object) sourceInstanceProperties);
      try
      {
        MaintenancePlanAssignment assignment = this.CreateAssignment(sourceInstanceProperties);
        if (IndicationHelper.GetIndicationType((IndicationType) 0).Equals(indicationType))
          this.manager.Unmanage(assignment);
        else if (IndicationHelper.GetIndicationType((IndicationType) 1).Equals(indicationType))
          this.manager.Remanage(assignment);
        else
          IndicationHelper.GetIndicationType((IndicationType) 2).Equals(indicationType);
      }
      catch (Exception ex)
      {
        MaintenanceIndicationSubscriber.log.ErrorFormat("Unable to process maintenance mode indication. {0}", (object) ex);
        throw;
      }
    }

    internal MaintenancePlanAssignment CreateAssignment(PropertyBag sourceInstanceProperties)
    {
      if (sourceInstanceProperties == null)
        throw new ArgumentNullException(nameof (sourceInstanceProperties));
      return ((Dictionary<string, object>) sourceInstanceProperties).Keys.Any<string>() ? new MaintenancePlanAssignment()
      {
        ID = Convert.ToInt32(((Dictionary<string, object>) sourceInstanceProperties)["ID"]),
        EntityType = Convert.ToString(((Dictionary<string, object>) sourceInstanceProperties)["EntityType"]),
        EntityID = Convert.ToInt32(((Dictionary<string, object>) sourceInstanceProperties)["EntityID"]),
        MaintenancePlanID = Convert.ToInt32(((Dictionary<string, object>) sourceInstanceProperties)["MaintenancePlanID"])
      } : throw new ArgumentException(nameof (sourceInstanceProperties));
    }

    public void Dispose()
    {
      this.Dispose(true);
      GC.SuppressFinalize((object) this);
    }

    protected void Dispose(bool disposing)
    {
      if (string.IsNullOrEmpty(this.subscriptionId))
        return;
      try
      {
        this.Unsubscribe(this.subscriptionId);
        this.subscriptionId = (string) null;
      }
      catch (Exception ex)
      {
        MaintenanceIndicationSubscriber.log.ErrorFormat("Error unsubscribing subscription '{0}'. {1}", (object) this.subscriptionId, (object) ex);
      }
    }

    private string Subscribe()
    {
      return this.subscriptionProvider.Subscribe("SUBSCRIBE CHANGES TO Orion.MaintenancePlanAssignment", (INotificationSubscriber) this);
    }

    private void Unsubscribe(string subscriptionId)
    {
      this.subscriptionProvider.Unsubscribe(subscriptionId);
    }

    ~MaintenanceIndicationSubscriber() => this.Dispose(false);
  }
}
