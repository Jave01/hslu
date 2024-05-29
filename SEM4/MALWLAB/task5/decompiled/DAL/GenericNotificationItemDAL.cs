// Decompiled with JetBrains decompiler
// Type: SolarWinds.Orion.Core.BusinessLayer.DAL.GenericNotificationItemDAL
// Assembly: SolarWinds.Orion.Core.BusinessLayer, Version=2019.4.5200.9083, Culture=neutral, PublicKeyToken=null
// MVID: E12E8C85-5CD9-4E06-8801-182E5104FADE
// Assembly location: E:\task5.dll

using SolarWinds.Orion.Core.Common.Models;
using System;
using System.Collections.Generic;

#nullable disable
namespace SolarWinds.Orion.Core.BusinessLayer.DAL
{
  public class GenericNotificationItemDAL : NotificationItemDAL
  {
    protected override Guid GetNotificationItemTypeId()
    {
      return GenericNotificationItem.GenericNotificationTypeGuid;
    }

    public static GenericNotificationItemDAL GetItemById(Guid itemId)
    {
      return NotificationItemDAL.GetItemById<GenericNotificationItemDAL>(itemId);
    }

    public static GenericNotificationItemDAL GetLatestItem()
    {
      return NotificationItemDAL.GetLatestItem<GenericNotificationItemDAL>(new NotificationItemFilter(false, false));
    }

    public static ICollection<GenericNotificationItemDAL> GetItems(NotificationItemFilter filter)
    {
      return NotificationItemDAL.GetItems<GenericNotificationItemDAL>(filter);
    }

    public static int GetNotificationItemsCount()
    {
      return NotificationItemDAL.GetNotificationsCount<GenericNotificationItemDAL>(new NotificationItemFilter(false, false));
    }

    public static GenericNotificationItemDAL Insert(
      Guid notificationId,
      string title,
      string description,
      bool ignored,
      string url,
      DateTime? acknowledgedAt,
      string acknowledgedBy)
    {
      return NotificationItemDAL.Insert<GenericNotificationItemDAL>(notificationId, title, description, ignored, url, acknowledgedAt, acknowledgedBy);
    }
  }
}
