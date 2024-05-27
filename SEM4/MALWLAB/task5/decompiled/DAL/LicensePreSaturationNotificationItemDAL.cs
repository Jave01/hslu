// Decompiled with JetBrains decompiler
// Type: SolarWinds.Orion.Core.BusinessLayer.DAL.LicensePreSaturationNotificationItemDAL
// Assembly: SolarWinds.Orion.Core.BusinessLayer, Version=2019.4.5200.9083, Culture=neutral, PublicKeyToken=null
// MVID: E12E8C85-5CD9-4E06-8801-182E5104FADE
// Assembly location: E:\task5.dll

using SolarWinds.Orion.Core.Common.Models;
using SolarWinds.Orion.Core.Strings;
using System;

#nullable disable
namespace SolarWinds.Orion.Core.BusinessLayer.DAL
{
  public sealed class LicensePreSaturationNotificationItemDAL : NotificationItemDAL
  {
    public static readonly Guid LicensePreSaturationNotificationItemId = new Guid("{C95EC3BD-9CBB-D82A-824C-482d6B138550}");
    private const string popupCallFunction = "javascript:SW.Core.SalesTrigger.ShowLicensePopupAsync();";

    private static string NotificationMessage
    {
      get
      {
        return string.Format(Resources.Notification_LicensePreSaturation, (object) Settings.LicenseSaturationPercentage);
      }
    }

    protected override Guid GetNotificationItemTypeId()
    {
      return GenericNotificationItem.LicensePreSaturationNotificationTypeGuid;
    }

    public static LicensePreSaturationNotificationItemDAL GetItem()
    {
      return NotificationItemDAL.GetItemById<LicensePreSaturationNotificationItemDAL>(LicensePreSaturationNotificationItemDAL.LicensePreSaturationNotificationItemId);
    }

    public static void Show()
    {
      LicensePreSaturationNotificationItemDAL notificationItemDal = LicensePreSaturationNotificationItemDAL.GetItem();
      if (notificationItemDal == null)
      {
        NotificationItemDAL.Insert<LicensePreSaturationNotificationItemDAL>(LicensePreSaturationNotificationItemDAL.LicensePreSaturationNotificationItemId, LicensePreSaturationNotificationItemDAL.NotificationMessage, string.Empty, false, "javascript:SW.Core.SalesTrigger.ShowLicensePopupAsync();", new DateTime?(), (string) null);
      }
      else
      {
        notificationItemDal.SetNotAcknowledged();
        notificationItemDal.Update();
      }
    }

    public static void Hide()
    {
      NotificationItemDAL.Delete(LicensePreSaturationNotificationItemDAL.LicensePreSaturationNotificationItemId);
    }
  }
}
