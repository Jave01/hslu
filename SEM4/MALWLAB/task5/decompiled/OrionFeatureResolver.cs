// Decompiled with JetBrains decompiler
// Type: SolarWinds.Orion.Core.BusinessLayer.OrionFeatureResolver
// Assembly: SolarWinds.Orion.Core.BusinessLayer, Version=2019.4.5200.9083, Culture=neutral, PublicKeyToken=null
// MVID: E12E8C85-5CD9-4E06-8801-182E5104FADE
// Assembly location: E:\task5.dll

using SolarWinds.Logging;
using SolarWinds.Orion.Core.BusinessLayer.DAL;
using SolarWinds.Orion.Core.Models.OrionFeature;
using System;
using System.Collections.Generic;
using System.Linq;

#nullable disable
namespace SolarWinds.Orion.Core.BusinessLayer
{
  internal class OrionFeatureResolver
  {
    private static readonly Log log = new Log();
    private readonly IOrionFeaturesDAL dal;
    private readonly IOrionFearureProviderFactory providerFactory;

    public OrionFeatureResolver(IOrionFeaturesDAL dal, IOrionFearureProviderFactory providerFactory)
    {
      if (dal == null)
        throw new ArgumentNullException(nameof (dal));
      if (providerFactory == null)
        throw new ArgumentNullException(nameof (providerFactory));
      this.dal = dal;
      this.providerFactory = providerFactory;
    }

    public IEnumerable<IOrionFeatureProvider> GetProviders() => this.providerFactory.GetProviders();

    public void Resolve()
    {
      using (OrionFeatureResolver.log.Block())
        this.dal.Update(this.GetProviders().SelectMany<IOrionFeatureProvider, SolarWinds.Orion.Core.Models.OrionFeature.OrionFeature>((Func<IOrionFeatureProvider, IEnumerable<SolarWinds.Orion.Core.Models.OrionFeature.OrionFeature>>) (n => n.GetFeatures())));
    }

    internal void Resolve(string providerName)
    {
      if (string.IsNullOrEmpty(providerName))
        throw new ArgumentNullException(nameof (providerName));
      this.Resolve();
    }
  }
}
