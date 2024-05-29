// Decompiled with JetBrains decompiler
// Type: SolarWinds.Orion.Core.BusinessLayer.Thresholds.ThresholdProcessingManager
// Assembly: SolarWinds.Orion.Core.BusinessLayer, Version=2019.4.5200.9083, Culture=neutral, PublicKeyToken=null
// MVID: E12E8C85-5CD9-4E06-8801-182E5104FADE
// Assembly location: E:\task5.dll

using SolarWinds.Common.Threading;
using SolarWinds.Orion.Core.Common.Catalogs;
using SolarWinds.Orion.Core.Common.Settings;
using SolarWinds.Orion.Core.Common.Thresholds;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.Linq;

#nullable disable
namespace SolarWinds.Orion.Core.BusinessLayer.Thresholds
{
  internal class ThresholdProcessingManager
  {
    private static readonly LazyWithoutExceptionCache<ThresholdProcessingManager> _instance = new LazyWithoutExceptionCache<ThresholdProcessingManager>((Func<ThresholdProcessingManager>) (() =>
    {
      using (ComposablePartCatalog catalogForArea = MEFPluginsLoader.Instance.GetCatalogForArea("Thresholds"))
        return new ThresholdProcessingManager(catalogForArea, CollectorSettings.Instance);
    }));
    [ImportMany(typeof (IThresholdDataProcessor))]
    private IEnumerable<IThresholdDataProcessor> _thresholdProcessors = Enumerable.Empty<IThresholdDataProcessor>();
    [ImportMany(typeof (ThresholdDataProvider))]
    private IEnumerable<ThresholdDataProvider> _thresholdDataProviders = Enumerable.Empty<ThresholdDataProvider>();
    private readonly ThresholdProcessingEngine _engine;

    internal ThresholdProcessingManager(ComposablePartCatalog catalog, ICollectorSettings settings)
    {
      this.ComposeParts(catalog);
      this._engine = new ThresholdProcessingEngine(this._thresholdProcessors, this._thresholdDataProviders, (IThresholdIndicator) new ThresholdIndicator(), settings)
      {
        BatchSize = BusinessLayerSettings.Instance.ThresholdsProcessingBatchSize,
        BaselineTimeFrame = BusinessLayerSettings.Instance.ThresholdsProcessingDefaultTimeFrame
      };
    }

    private void ComposeParts(ComposablePartCatalog catalog)
    {
      using (CompositionContainer container = new CompositionContainer(catalog, Array.Empty<ExportProvider>()))
        container.ComposeParts((object) this);
    }

    public static ThresholdProcessingManager Instance => ThresholdProcessingManager._instance.Value;

    internal static CompositionContainer CompositionContainer { get; set; }

    public ThresholdProcessingEngine Engine => this._engine;
  }
}
