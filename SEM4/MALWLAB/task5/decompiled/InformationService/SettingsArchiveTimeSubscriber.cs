// Decompiled with JetBrains decompiler
// Type: SolarWinds.Orion.Core.BusinessLayer.InformationService.SettingsArchiveTimeSubscriber
// Assembly: SolarWinds.Orion.Core.BusinessLayer, Version=2019.4.5200.9083, Culture=neutral, PublicKeyToken=null
// MVID: E12E8C85-5CD9-4E06-8801-182E5104FADE
// Assembly location: E:\task5.dll

using SolarWinds.Common.Utility;
using SolarWinds.InformationService.Contract2;
using SolarWinds.InformationService.Contract2.PubSub;
using SolarWinds.Logging;
using System;
using System.Collections.Generic;

#nullable disable
namespace SolarWinds.Orion.Core.BusinessLayer.InformationService
{
  internal class SettingsArchiveTimeSubscriber : INotificationSubscriber
  {
    private readonly Log log = new Log();
    private readonly ScheduledTaskInExactTime task;

    public SettingsArchiveTimeSubscriber(ScheduledTaskInExactTime task)
    {
      this.task = task != null ? task : throw new ArgumentNullException(nameof (task));
    }

    public void OnIndication(
      string subscriptionId,
      string indicationType,
      PropertyBag indicationProperties,
      PropertyBag sourceInstanceProperties)
    {
      try
      {
        this.task.ExactRunTime = DateTime.FromOADate(double.Parse(((Dictionary<string, object>) sourceInstanceProperties)["CurrentValue"].ToString()));
      }
      catch (Exception ex)
      {
        this.log.Error((object) "Error when getting Archive time from SWIS.", ex);
      }
    }
  }
}
