// Decompiled with JetBrains decompiler
// Type: SolarWinds.Orion.Core.BusinessLayer.MaintUpdateNotifySvcWrapper
// Assembly: SolarWinds.Orion.Core.BusinessLayer, Version=2019.4.5200.9083, Culture=neutral, PublicKeyToken=null
// MVID: E12E8C85-5CD9-4E06-8801-182E5104FADE
// Assembly location: E:\task5.dll

using SolarWinds.Orion.Core.BusinessLayer.DAL;
using SolarWinds.Orion.Core.BusinessLayer.MaintUpdateNotifySvc;
using SolarWinds.Orion.Core.Common.Models;
using System.Collections.Generic;

#nullable disable
namespace SolarWinds.Orion.Core.BusinessLayer
{
  public class MaintUpdateNotifySvcWrapper
  {
    public static MaintenanceRenewalItemDAL GetNotificationItem(VersionInfo versionInfo)
    {
      MaintenanceRenewalItemDAL renewal = new MaintenanceRenewalItemDAL();
      MaintUpdateNotifySvcWrapper.UpdateNotificationItem(renewal, versionInfo);
      return renewal;
    }

    public static void UpdateNotificationItem(
      MaintenanceRenewalItemDAL renewal,
      VersionInfo versionInfo)
    {
      if (string.IsNullOrEmpty(versionInfo.Hotfix))
        renewal.Title = versionInfo.Message.MaintenanceMessage;
      else
        renewal.Title = string.Format("{0} {1}", (object) versionInfo.Message.MaintenanceMessage, (object) versionInfo.Hotfix);
      renewal.Description = versionInfo.ReleaseNotes;
      if (renewal.DateReleased < versionInfo.DateReleased)
        renewal.Ignored = false;
      renewal.Url = versionInfo.Link;
      renewal.SetNotAcknowledged();
      renewal.ProductTag = versionInfo.ProductTag;
      renewal.DateReleased = versionInfo.DateReleased;
      renewal.NewVersion = versionInfo.Version;
    }

    public static ModuleInfo[] GetModules(List<ModuleInfo> listModules)
    {
      ModuleInfo[] modules = new ModuleInfo[listModules.Count];
      for (int index = 0; index < listModules.Count; ++index)
        modules[index] = new ModuleInfo()
        {
          ProductDisplayName = listModules[index].ProductDisplayName,
          HotfixVersion = listModules[index].HotfixVersion,
          Version = listModules[index].Version,
          ProductTag = listModules[index].ProductTag,
          LicenseInfo = listModules[index].LicenseInfo
        };
      return modules;
    }
  }
}
