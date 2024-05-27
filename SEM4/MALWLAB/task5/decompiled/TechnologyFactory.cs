// Decompiled with JetBrains decompiler
// Type: SolarWinds.Orion.Core.BusinessLayer.TechnologyFactory
// Assembly: SolarWinds.Orion.Core.BusinessLayer, Version=2019.4.5200.9083, Culture=neutral, PublicKeyToken=null
// MVID: E12E8C85-5CD9-4E06-8801-182E5104FADE
// Assembly location: E:\task5.dll

using SolarWinds.Logging;
using SolarWinds.Orion.Core.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.Linq;

#nullable disable
namespace SolarWinds.Orion.Core.BusinessLayer
{
  public class TechnologyFactory
  {
    private static readonly Log log = new Log();
    private Dictionary<string, ITechnology> items;

    public TechnologyFactory(ComposablePartCatalog catalog)
    {
      this.items = this.InitializeMEF(catalog).ToDictionary<ITechnology, string>((Func<ITechnology, string>) (n => n.TechnologyID));
      if (this.items.Any<KeyValuePair<string, ITechnology>>())
        TechnologyFactory.log.Info((object) ("Technology loader found technologies: " + string.Join(",", this.items.Values.Select<ITechnology, string>((Func<ITechnology, string>) (t => t.TechnologyID)).ToArray<string>())));
      else
        TechnologyFactory.log.Error((object) "Technology loader found 0 technologies");
    }

    protected IEnumerable<ITechnology> InitializeMEF(ComposablePartCatalog catalog)
    {
      using (CompositionContainer compositionContainer = new CompositionContainer(catalog, Array.Empty<ExportProvider>()))
        return (IEnumerable<ITechnology>) compositionContainer.GetExports<ITechnology>().Select<Lazy<ITechnology>, ITechnology>((Func<Lazy<ITechnology>, ITechnology>) (n => n.Value)).ToList<ITechnology>();
    }

    public IEnumerable<ITechnology> Items() => (IEnumerable<ITechnology>) this.items.Values;

    public ITechnology GetTechnology(string technologyID)
    {
      if (string.IsNullOrEmpty(technologyID))
        throw new ArgumentNullException(nameof (technologyID));
      ITechnology technology = (ITechnology) null;
      if (!this.items.TryGetValue(technologyID, out technology))
        throw new KeyNotFoundException(technologyID);
      return technology;
    }
  }
}
