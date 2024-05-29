// Decompiled with JetBrains decompiler
// Type: SolarWinds.Orion.Core.BusinessLayer.DAL.MaintenanceRenewalFilter
// Assembly: SolarWinds.Orion.Core.BusinessLayer, Version=2019.4.5200.9083, Culture=neutral, PublicKeyToken=null
// MVID: E12E8C85-5CD9-4E06-8801-182E5104FADE
// Assembly location: E:\task5.dll

#nullable disable
namespace SolarWinds.Orion.Core.BusinessLayer.DAL
{
  public class MaintenanceRenewalFilter : NotificationItemFilter
  {
    public string ProductTag { get; set; }

    public MaintenanceRenewalFilter(
      bool includeAcknowledged,
      bool includeIgnored,
      string productTag)
      : base(includeAcknowledged, includeIgnored)
    {
      this.ProductTag = productTag;
    }

    public MaintenanceRenewalFilter()
      : this(false, false, (string) null)
    {
    }
  }
}
