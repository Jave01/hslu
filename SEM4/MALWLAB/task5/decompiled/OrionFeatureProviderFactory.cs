// Decompiled with JetBrains decompiler
// Type: SolarWinds.Orion.Core.BusinessLayer.OrionFeatureProviderFactory
// Assembly: SolarWinds.Orion.Core.BusinessLayer, Version=2019.4.5200.9083, Culture=neutral, PublicKeyToken=null
// MVID: E12E8C85-5CD9-4E06-8801-182E5104FADE
// Assembly location: E:\task5.dll

using SolarWinds.Orion.Core.Common.Catalogs;
using SolarWinds.Orion.Core.Models.OrionFeature;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.Linq;

#nullable disable
namespace SolarWinds.Orion.Core.BusinessLayer
{
  public class OrionFeatureProviderFactory : IOrionFearureProviderFactory
  {
    [ImportMany(typeof (IOrionFeatureProvider))]
    private IEnumerable<IOrionFeatureProvider> _providers = Enumerable.Empty<IOrionFeatureProvider>();

    public static OrionFeatureProviderFactory CreateInstance()
    {
      using (ComposablePartCatalog catalogForArea = MEFPluginsLoader.Instance.GetCatalogForArea("OrionFeature"))
        return new OrionFeatureProviderFactory(catalogForArea);
    }

    public OrionFeatureProviderFactory(ComposablePartCatalog catalog)
    {
      if (catalog == null)
        throw new ArgumentNullException(nameof (catalog));
      using (CompositionContainer container = new CompositionContainer(catalog, Array.Empty<ExportProvider>()))
        container.ComposeParts((object) this);
    }

    public IEnumerable<IOrionFeatureProvider> GetProviders() => this._providers;
  }
}
