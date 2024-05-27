// Decompiled with JetBrains decompiler
// Type: SolarWinds.Orion.Core.BusinessLayer.DAL.BlogFilter
// Assembly: SolarWinds.Orion.Core.BusinessLayer, Version=2019.4.5200.9083, Culture=neutral, PublicKeyToken=null
// MVID: E12E8C85-5CD9-4E06-8801-182E5104FADE
// Assembly location: E:\task5.dll

#nullable disable
namespace SolarWinds.Orion.Core.BusinessLayer.DAL
{
  public class BlogFilter : NotificationItemFilter
  {
    public int MaxResults { get; set; }

    public BlogFilter(bool includeAcknowledged, bool includeIgnored, int maxResults)
      : base(includeAcknowledged, includeIgnored)
    {
      this.MaxResults = maxResults;
    }

    public BlogFilter()
      : this(false, false, -1)
    {
    }
  }
}
