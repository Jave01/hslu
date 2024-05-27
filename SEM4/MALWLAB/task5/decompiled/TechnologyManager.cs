// Decompiled with JetBrains decompiler
// Type: SolarWinds.Orion.Core.BusinessLayer.TechnologyManager
// Assembly: SolarWinds.Orion.Core.BusinessLayer, Version=2019.4.5200.9083, Culture=neutral, PublicKeyToken=null
// MVID: E12E8C85-5CD9-4E06-8801-182E5104FADE
// Assembly location: E:\task5.dll

using SolarWinds.Logging;
using SolarWinds.Orion.Core.Common.Catalogs;
using System;
using System.ComponentModel.Composition.Primitives;
using System.Threading;

#nullable disable
namespace SolarWinds.Orion.Core.BusinessLayer
{
  public class TechnologyManager
  {
    private static readonly Log log = new Log();
    public static readonly string TechonologyMEFPluginAreaID = "Technology";
    private TechnologyFactory techs;
    private TechnologyPollingFactory impls;
    private static Lazy<TechnologyManager> cachedLazyInstance = new Lazy<TechnologyManager>(LazyThreadSafetyMode.ExecutionAndPublication);

    internal TechnologyManager(ComposablePartCatalog catalog) => this.Initialize(catalog);

    public TechnologyManager()
    {
      using (ComposablePartCatalog catalogForArea = MEFPluginsLoader.Instance.GetCatalogForArea(TechnologyManager.TechonologyMEFPluginAreaID))
        this.Initialize(catalogForArea);
    }

    private void Initialize(ComposablePartCatalog catalog)
    {
      this.techs = new TechnologyFactory(catalog);
      this.impls = new TechnologyPollingFactory(catalog);
    }

    public static TechnologyManager Instance => TechnologyManager.cachedLazyInstance.Value;

    public TechnologyFactory TechnologyFactory => this.techs;

    public TechnologyPollingFactory TechnologyPollingFactory => this.impls;
  }
}
