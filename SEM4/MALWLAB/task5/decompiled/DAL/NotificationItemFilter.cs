// Decompiled with JetBrains decompiler
// Type: SolarWinds.Orion.Core.BusinessLayer.DAL.NotificationItemFilter
// Assembly: SolarWinds.Orion.Core.BusinessLayer, Version=2019.4.5200.9083, Culture=neutral, PublicKeyToken=null
// MVID: E12E8C85-5CD9-4E06-8801-182E5104FADE
// Assembly location: E:\task5.dll

#nullable disable
namespace SolarWinds.Orion.Core.BusinessLayer.DAL
{
  public class NotificationItemFilter
  {
    public bool IncludeAcknowledged { get; set; }

    public bool IncludeIgnored { get; set; }

    public NotificationItemFilter(bool includeAcknowledged, bool includeIgnored)
    {
      this.IncludeAcknowledged = includeAcknowledged;
      this.IncludeIgnored = includeIgnored;
    }

    public NotificationItemFilter()
      : this(false, false)
    {
    }
  }
}
