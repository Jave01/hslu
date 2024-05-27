// Decompiled with JetBrains decompiler
// Type: SolarWinds.Orion.Core.BusinessLayer.DAL.ScheduledDiscoveryNotificationItemDAL
// Assembly: SolarWinds.Orion.Core.BusinessLayer, Version=2019.4.5200.9083, Culture=neutral, PublicKeyToken=null
// MVID: E12E8C85-5CD9-4E06-8801-182E5104FADE
// Assembly location: E:\task5.dll

using SolarWinds.Orion.Core.Common.Models;
using System;

#nullable disable
namespace SolarWinds.Orion.Core.BusinessLayer.DAL
{
  public sealed class ScheduledDiscoveryNotificationItemDAL : GenericPopupNotificationItemDAL
  {
    public static readonly Guid ScheduledDiscoveryNotificationItemId = new Guid("3D28249D-EFE1-462e-B1A7-C55273D09AE8");

    public ScheduledDiscoveryNotificationItemDAL GetItem()
    {
      return NotificationItemDAL.GetItemById<ScheduledDiscoveryNotificationItemDAL>(ScheduledDiscoveryNotificationItemDAL.ScheduledDiscoveryNotificationItemId);
    }

    protected override Guid GetNotificationItemTypeId()
    {
      return GenericNotificationItem.ScheduledDiscoveryNotificationTypeGuid;
    }

    protected override Guid GetPopupNotificationItemId()
    {
      return ScheduledDiscoveryNotificationItemDAL.ScheduledDiscoveryNotificationItemId;
    }

    public static ScheduledDiscoveryNotificationItemDAL Create(string title, string url)
    {
      return GenericPopupNotificationItemDAL.Create<ScheduledDiscoveryNotificationItemDAL>(title, url);
    }
  }
}
