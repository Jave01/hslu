// Decompiled with JetBrains decompiler
// Type: SolarWinds.Orion.Core.BusinessLayer.MaintenanceExpirationHelper
// Assembly: SolarWinds.Orion.Core.BusinessLayer, Version=2019.4.5200.9083, Culture=neutral, PublicKeyToken=null
// MVID: E12E8C85-5CD9-4E06-8801-182E5104FADE
// Assembly location: E:\task5.dll

using SolarWinds.Logging;
using SolarWinds.Orion.Core.BusinessLayer.DAL;
using SolarWinds.Orion.Core.Common;
using SolarWinds.Orion.Core.Common.DALs;
using System;
using System.Collections.Generic;
using System.Linq;

#nullable disable
namespace SolarWinds.Orion.Core.BusinessLayer
{
  internal static class MaintenanceExpirationHelper
  {
    private static readonly Log log = new Log();

    internal static void CheckMaintenanceExpiration()
    {
      try
      {
        MaintenanceExpirationHelper.log.Debug((object) "Check Maintenance expiration");
        int expirationWarningDays = BusinessLayerSettings.Instance.MaintenanceExpirationWarningDays;
        Dictionary<string, MaintenanceExpirationNotificationItemDAL.ExpirationInfo> moduleExpirations = new Dictionary<string, MaintenanceExpirationNotificationItemDAL.ExpirationInfo>();
        ILicensingDAL licensing = (ILicensingDAL) new LicensingDAL();
        foreach (LicenseInfoModel licenseInfoModel in ((IEnumerable<LicenseInfoModel>) licensing.GetLicenses()).Where<LicenseInfoModel>((Func<LicenseInfoModel, bool>) (lic => !lic.IsHidden && !lic.IsEvaluation && !licensing.DefaultLicenseFilter.Contains<string>(lic.ProductName, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase))))
        {
          if (MaintenanceExpirationHelper.log.IsDebugEnabled)
            MaintenanceExpirationHelper.log.Debug((object) string.Format("Module:{0} MaintenanceTo:{1} DaysLeft:{2}", (object) licenseInfoModel.LicenseName, (object) licenseInfoModel.MaintenanceExpiration.Date, (object) licenseInfoModel.DaysRemainingCount));
          if (licenseInfoModel.DaysRemainingCount <= expirationWarningDays)
          {
            MaintenanceExpirationNotificationItemDAL.ExpirationInfo expirationInfo = new MaintenanceExpirationNotificationItemDAL.ExpirationInfo()
            {
              DaysToExpire = licenseInfoModel.DaysRemainingCount,
              ActivationKey = licenseInfoModel.LicenseKey
            };
            moduleExpirations[licenseInfoModel.LicenseName] = expirationInfo;
          }
        }
        if (moduleExpirations.Count > 0)
        {
          MaintenanceExpirationHelper.log.Debug((object) string.Format("{0} products found to be notified", (object) moduleExpirations.Count));
          MaintenanceExpirationNotificationItemDAL.Show(moduleExpirations);
        }
        else
          MaintenanceExpirationNotificationItemDAL.Hide();
      }
      catch (Exception ex)
      {
        MaintenanceExpirationHelper.log.Warn((object) "Exception while checking maintenance expiration status: ", ex);
      }
    }
  }
}
