// Decompiled with JetBrains decompiler
// Type: SolarWinds.Orion.Core.BusinessLayer.LicenseSaturationHelper
// Assembly: SolarWinds.Orion.Core.BusinessLayer, Version=2019.4.5200.9083, Culture=neutral, PublicKeyToken=null
// MVID: E12E8C85-5CD9-4E06-8801-182E5104FADE
// Assembly location: E:\task5.dll

using SolarWinds.Logging;
using SolarWinds.Orion.Core.BusinessLayer.DAL;
using SolarWinds.Orion.Core.Common.Licensing;
using SolarWinds.Orion.Core.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;

#nullable disable
namespace SolarWinds.Orion.Core.BusinessLayer
{
  internal static class LicenseSaturationHelper
  {
    private static readonly Log Log = new Log();
    private static readonly int SaturationLimit = Settings.LicenseSaturationPercentage;

    internal static void CheckLicenseSaturation()
    {
      try
      {
        LicenseSaturationHelper.Log.Debug((object) "Checking license saturation");
        List<ModuleLicenseSaturationInfo> modulesSaturationInfo = LicenseSaturationLogic.GetModulesSaturationInfo(new int?(LicenseSaturationHelper.SaturationLimit));
        if (modulesSaturationInfo.Count == 0)
        {
          LicenseSaturationHelper.Log.DebugFormat("All modules below {0}% of their license", (object) LicenseSaturationHelper.SaturationLimit);
          LicenseSaturationNotificationItemDAL.Hide();
          LicensePreSaturationNotificationItemDAL.Hide();
        }
        else
        {
          List<ModuleLicenseSaturationInfo> list1 = modulesSaturationInfo.Where<ModuleLicenseSaturationInfo>((Func<ModuleLicenseSaturationInfo, bool>) (q => q.ElementList.Any<ElementLicenseSaturationInfo>((Func<ElementLicenseSaturationInfo, bool>) (l => l.Saturation > 99.0)))).ToList<ModuleLicenseSaturationInfo>();
          List<ModuleLicenseSaturationInfo> list2 = modulesSaturationInfo.Where<ModuleLicenseSaturationInfo>((Func<ModuleLicenseSaturationInfo, bool>) (q => q.ElementList.Any<ElementLicenseSaturationInfo>((Func<ElementLicenseSaturationInfo, bool>) (l => l.Saturation > (double) LicenseSaturationHelper.SaturationLimit && l.Saturation < 100.0)))).ToList<ModuleLicenseSaturationInfo>();
          List<ElementLicenseSaturationInfo> overUsedElements = new List<ElementLicenseSaturationInfo>();
          list1.ForEach((Action<ModuleLicenseSaturationInfo>) (l => overUsedElements.AddRange((IEnumerable<ElementLicenseSaturationInfo>) l.ElementList.ToArray())));
          if (LicenseSaturationHelper.Log.IsInfoEnabled)
            LicenseSaturationHelper.Log.InfoFormat("These elements are at 100% of their license: {0}", (object) string.Join(";", overUsedElements.Select<ElementLicenseSaturationInfo, string>((Func<ElementLicenseSaturationInfo, string>) (q => q.ElementType))));
          LicenseSaturationNotificationItemDAL.Show(overUsedElements.Select<ElementLicenseSaturationInfo, string>((Func<ElementLicenseSaturationInfo, string>) (q => q.ElementType)));
          List<ElementLicenseSaturationInfo> warningElements = new List<ElementLicenseSaturationInfo>();
          Action<ModuleLicenseSaturationInfo> action = (Action<ModuleLicenseSaturationInfo>) (l => warningElements.AddRange((IEnumerable<ElementLicenseSaturationInfo>) l.ElementList.ToArray()));
          list2.ForEach(action);
          if (LicenseSaturationHelper.Log.IsInfoEnabled)
            LicenseSaturationHelper.Log.InfoFormat("These elements are above {0}% of their license: {1}", (object) LicenseSaturationHelper.SaturationLimit, (object) string.Join(";", warningElements.Select<ElementLicenseSaturationInfo, string>((Func<ElementLicenseSaturationInfo, string>) (q => q.ElementType))));
          LicensePreSaturationNotificationItemDAL.Show();
        }
      }
      catch (Exception ex)
      {
        LicenseSaturationHelper.Log.Error((object) "Exception running CheckLicenseSaturation:", ex);
      }
    }

    internal static void SaveElementsUsageInfo()
    {
      try
      {
        LicenseSaturationHelper.Log.Debug((object) "Collecting elements usage information to store in history");
        List<ModuleLicenseSaturationInfo> modulesSaturationInfo = LicenseSaturationLogic.GetModulesSaturationInfo(new int?());
        if (modulesSaturationInfo.Count != 0)
        {
          List<ElementLicenseSaturationInfo> elements = new List<ElementLicenseSaturationInfo>();
          modulesSaturationInfo.ForEach((Action<ModuleLicenseSaturationInfo>) (m => elements.AddRange((IEnumerable<ElementLicenseSaturationInfo>) m.ElementList.ToArray())));
          ElementsUsageDAL.Save((IEnumerable<ElementLicenseSaturationInfo>) elements);
        }
        else
          LicenseSaturationHelper.Log.DebugFormat("There is no elements usage information to store in history", Array.Empty<object>());
      }
      catch (Exception ex)
      {
        LicenseSaturationHelper.Log.Error((object) "Exception running SaveElementsUsageInfo:", ex);
      }
    }
  }
}
