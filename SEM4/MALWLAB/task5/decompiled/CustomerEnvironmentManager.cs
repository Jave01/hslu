// Decompiled with JetBrains decompiler
// Type: SolarWinds.Orion.Core.BusinessLayer.CustomerEnvironmentManager
// Assembly: SolarWinds.Orion.Core.BusinessLayer, Version=2019.4.5200.9083, Culture=neutral, PublicKeyToken=null
// MVID: E12E8C85-5CD9-4E06-8801-182E5104FADE
// Assembly location: E:\task5.dll

using SolarWinds.Orion.Core.BusinessLayer.DAL;
using SolarWinds.Orion.Core.BusinessLayer.MaintUpdateNotifySvc;
using SolarWinds.Orion.Core.Common;
using SolarWinds.Orion.Core.Common.DALs;
using System;

#nullable disable
namespace SolarWinds.Orion.Core.BusinessLayer
{
  public class CustomerEnvironmentManager
  {
    public static CustomerEnvironmentInfoPack GetEnvironmentInfoPack()
    {
      CustomerEnvironmentInfoPack environmentInfoPack = new CustomerEnvironmentInfoPack();
      environmentInfoPack.OSVersion = Environment.OSVersion.VersionString;
      MaintenanceRenewalsCheckStatusDAL checkStatus = MaintenanceRenewalsCheckStatusDAL.GetCheckStatus();
      DateTime minValue;
      if (checkStatus != null)
      {
        DateTime? lastUpdateCheck = checkStatus.LastUpdateCheck;
        if (lastUpdateCheck.HasValue)
        {
          lastUpdateCheck = checkStatus.LastUpdateCheck;
          minValue = lastUpdateCheck.Value;
          goto label_4;
        }
      }
      minValue = DateTime.MinValue;
label_4:
      environmentInfoPack.LastUpdateCheck = minValue;
      environmentInfoPack.OrionDBVersion = DatabaseInfoDAL.GetOrionDBVersion();
      environmentInfoPack.SQLVersion = DatabaseInfoDAL.GetSQLEngineVersion();
      environmentInfoPack.Modules = MaintUpdateNotifySvcWrapper.GetModules(ModulesCollector.GetInstalledModules());
      environmentInfoPack.CustomerUniqueId = ModulesCollector.GetCustomerUniqueId();
      return environmentInfoPack;
    }
  }
}
