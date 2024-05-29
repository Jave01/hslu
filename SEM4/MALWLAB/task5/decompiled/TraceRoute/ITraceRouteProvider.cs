// Decompiled with JetBrains decompiler
// Type: SolarWinds.Orion.Core.BusinessLayer.TraceRoute.ITraceRouteProvider
// Assembly: SolarWinds.Orion.Core.BusinessLayer, Version=2019.4.5200.9083, Culture=neutral, PublicKeyToken=null
// MVID: E12E8C85-5CD9-4E06-8801-182E5104FADE
// Assembly location: E:\task5.dll

using SolarWinds.Orion.Core.Common.Models;

#nullable disable
namespace SolarWinds.Orion.Core.BusinessLayer.TraceRoute
{
  public interface ITraceRouteProvider
  {
    TracerouteResult TraceRoute(string destinationHostNameOrIpAddress);

    TracerouteResult TraceRoute(
      string destinationHostNameOrIpAddress,
      long maxTimeoutInMilliseconds);
  }
}
