// Decompiled with JetBrains decompiler
// Type: SolarWinds.Orion.Core.BusinessLayer.DAL.GenericPopupNotificationItemDAL
// Assembly: SolarWinds.Orion.Core.BusinessLayer, Version=2019.4.5200.9083, Culture=neutral, PublicKeyToken=null
// MVID: E12E8C85-5CD9-4E06-8801-182E5104FADE
// Assembly location: E:\task5.dll

using System;

#nullable disable
namespace SolarWinds.Orion.Core.BusinessLayer.DAL
{
  public class GenericPopupNotificationItemDAL : NotificationItemDAL
  {
    protected GenericPopupNotificationItemDAL()
    {
    }

    protected virtual Guid GetPopupNotificationItemId() => Guid.Empty;

    protected static TNotificationItem Create<TNotificationItem>(string title, string url) where TNotificationItem : GenericPopupNotificationItemDAL, new()
    {
      Guid notificationItemId = new TNotificationItem().GetPopupNotificationItemId();
      TNotificationItem notificationItem = !(notificationItemId == Guid.Empty) ? NotificationItemDAL.GetItemById<TNotificationItem>(notificationItemId) : throw new ArgumentException("Can't obtain Popup Notification Item GUID", nameof (TNotificationItem));
      if ((object) notificationItem == null)
        return NotificationItemDAL.Insert<TNotificationItem>(notificationItemId, title, (string) null, false, url, new DateTime?(), (string) null);
      notificationItem.Title = title;
      notificationItem.Description = (string) null;
      notificationItem.Url = url;
      notificationItem.CreatedAt = DateTime.UtcNow;
      notificationItem.SetNotAcknowledged();
      notificationItem.Ignored = false;
      return !notificationItem.Update() ? default (TNotificationItem) : notificationItem;
    }
  }
}
