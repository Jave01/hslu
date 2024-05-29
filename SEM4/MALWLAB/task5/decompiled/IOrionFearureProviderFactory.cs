// Decompiled with JetBrains decompiler
// Type: SolarWinds.Orion.Core.BusinessLayer.IOrionFearureProviderFactory
// Assembly: SolarWinds.Orion.Core.BusinessLayer, Version=2019.4.5200.9083, Culture=neutral, PublicKeyToken=null
// MVID: E12E8C85-5CD9-4E06-8801-182E5104FADE
// Assembly location: E:\task5.dll

using SolarWinds.Orion.Core.Models.OrionFeature;
using System.Collections.Generic;

#nullable disable
namespace SolarWinds.Orion.Core.BusinessLayer
{
  public interface IOrionFearureProviderFactory
  {
    IEnumerable<IOrionFeatureProvider> GetProviders();
  }
}
