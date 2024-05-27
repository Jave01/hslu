// Decompiled with JetBrains decompiler
// Type: SolarWinds.Orion.Core.BusinessLayer.ScheduledTaskFactory
// Assembly: SolarWinds.Orion.Core.BusinessLayer, Version=2019.4.5200.9083, Culture=neutral, PublicKeyToken=null
// MVID: E12E8C85-5CD9-4E06-8801-182E5104FADE
// Assembly location: E:\task5.dll

using SolarWinds.Common.Utility;
using SolarWinds.InformationService.Contract2.PubSub;
using SolarWinds.Logging;
using SolarWinds.Orion.Common;
using SolarWinds.Orion.Core.BusinessLayer.InformationService;
using SolarWinds.Orion.Core.Common;
using SolarWinds.Orion.Core.Common.InformationService;
using SolarWinds.Orion.Swis.Contract.InformationService;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading;

#nullable disable
namespace SolarWinds.Orion.Core.BusinessLayer
{
  internal class ScheduledTaskFactory
  {
    private static readonly Log log = new Log();

    internal static ScheduledTask CreateDatabaseMaintenanceTask(
      InformationServiceSubscriptionProviderBase subscribtionProvider)
    {
      string empty = string.Empty;
      DateTime dateTime;
      try
      {
        empty = SettingsDAL.Get("SWNetPerfMon-Settings-Archive Time");
        dateTime = DateTime.FromOADate(double.Parse(empty));
      }
      catch (Exception ex)
      {
        dateTime = DateTime.MinValue.Date.AddHours(2.0).AddMinutes(15.0);
        ScheduledTaskFactory.log.ErrorFormat("DB maintenance time setting is not set or is not correct. Setting value is {0}. \nException: {1}", (object) empty, (object) ex);
      }
      ScheduledTaskInExactTime task = new ScheduledTaskInExactTime("DatabaseMaintenance", new TimerCallback(ScheduledTaskFactory.RunDatabaseMaintenace), (object) null, dateTime);
      if (subscribtionProvider != null)
      {
        SettingsArchiveTimeSubscriber archiveTimeSubscriber = new SettingsArchiveTimeSubscriber(task);
        subscribtionProvider.Subscribe("SUBSCRIBE CHANGES TO Orion.Settings WHEN SettingsID = 'SWNetPerfMon-Settings-Archive Time'", (INotificationSubscriber) archiveTimeSubscriber, new SubscriptionOptions()
        {
          Description = "SettingsArchiveTimeSubscriber"
        });
      }
      else
        ScheduledTaskFactory.log.Error((object) "SubscribtionProvider is not initialized.");
      return (ScheduledTask) task;
    }

    private static void RunDatabaseMaintenace(object state)
    {
      ScheduledTaskFactory.log.Info((object) "Database maintenance task is starting.");
      try
      {
        Process.Start(Path.Combine(OrionConfiguration.InstallPath, "Database-Maint.exe"), "-Archive");
        ScheduledTaskFactory.log.Info((object) "Database maintenace task started.");
        SettingsDAL.Set("SWNetPerfMon-Settings-Last Archive", DateTime.UtcNow.ToOADate());
      }
      catch (Exception ex)
      {
        ScheduledTaskFactory.log.Error((object) "Error while executing Database-Maint.exe.", ex);
      }
    }
  }
}
