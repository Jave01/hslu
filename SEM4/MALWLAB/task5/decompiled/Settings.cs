// Decompiled with JetBrains decompiler
// Type: SolarWinds.Orion.Core.BusinessLayer.Settings
// Assembly: SolarWinds.Orion.Core.BusinessLayer, Version=2019.4.5200.9083, Culture=neutral, PublicKeyToken=null
// MVID: E12E8C85-5CD9-4E06-8801-182E5104FADE
// Assembly location: E:\task5.dll

using SolarWinds.Orion.Core.Common;
using System;

#nullable disable
namespace SolarWinds.Orion.Core.BusinessLayer
{
  internal static class Settings
  {
    [Obsolete("Core-Split cleanup. If you need this member please contact Core team", true)]
    internal static TimeSpan DiscoverESXNodesTimer => TimeSpan.FromMinutes(5.0);

    [Obsolete("Core-Split cleanup. If you need this member please contact Core team", true)]
    internal static TimeSpan UpdateESXNotificationsTimer => TimeSpan.FromMinutes(2.0);

    [Obsolete("Core-Split cleanup. If you need this member please contact Core team", true)]
    internal static TimeSpan VMwareESXJobTimeout => TimeSpan.FromMinutes(10.0);

    internal static TimeSpan CheckMaintenanceRenewalsTimer => TimeSpan.FromDays(7.0);

    internal static TimeSpan CheckOrionProductTeamBlogTimer => TimeSpan.FromDays(7.0);

    internal static bool IsProductsBlogDisabled
    {
      get => SettingsDAL.Get("ProductsBlog-Disable").Equals("1");
    }

    internal static bool IsMaintenanceRenewalsDisabled
    {
      get => SettingsDAL.Get("MaintenanceRenewals-Disable").Equals("1");
    }

    internal static bool IsLicenseSaturationDisabled
    {
      get => SettingsDAL.Get("LicenseSaturation-Disable").Equals("1");
    }

    internal static bool IsAutomaticGeolocationEnabled
    {
      get => SettingsDAL.Get("AutomaticGeolocation-Enable").Equals("1");
    }

    internal static TimeSpan AutomaticGeolocationCheckInterval
    {
      get
      {
        string s;
        TimeSpan result;
        return WebSettingsDAL.TryGet(nameof (AutomaticGeolocationCheckInterval), ref s) && TimeSpan.TryParse(s, out result) ? result : TimeSpan.FromHours(1.0);
      }
    }

    internal static int LicenseSaturationPercentage
    {
      get
      {
        int result;
        return int.TryParse(SettingsDAL.Get("LicenseSaturation-WarningPercentage"), out result) ? result : 80;
      }
    }

    internal static int PollerLimitWarningScaleFactor
    {
      get
      {
        int result;
        return int.TryParse(SettingsDAL.Get(nameof (PollerLimitWarningScaleFactor)), out result) ? result : 85;
      }
    }

    internal static int PollerLimitreachedScaleFactor
    {
      get
      {
        int result;
        return int.TryParse(SettingsDAL.Get("PollerLimitReachedScaleFactor"), out result) ? result : 100;
      }
    }
  }
}
