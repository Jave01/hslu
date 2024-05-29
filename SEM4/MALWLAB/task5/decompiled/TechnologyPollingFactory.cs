// Decompiled with JetBrains decompiler
// Type: SolarWinds.Orion.Core.BusinessLayer.TechnologyPollingFactory
// Assembly: SolarWinds.Orion.Core.BusinessLayer, Version=2019.4.5200.9083, Culture=neutral, PublicKeyToken=null
// MVID: E12E8C85-5CD9-4E06-8801-182E5104FADE
// Assembly location: E:\task5.dll

using SolarWinds.Logging;
using SolarWinds.Orion.Core.Models.Interfaces;
using SolarWinds.Orion.Core.Models.Technology;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.Linq;

#nullable disable
namespace SolarWinds.Orion.Core.BusinessLayer
{
  public class TechnologyPollingFactory
  {
    private static readonly Log log = new Log();
    private TechnologyPollingIndicator changesIndicator = new TechnologyPollingIndicator();
    internal List<ITechnologyPollingProvider> providers;

    public TechnologyPollingFactory(ComposablePartCatalog catalog)
    {
      this.providers = TechnologyPollingFactory.InitializeMEF(catalog).ToList<ITechnologyPollingProvider>();
      if (this.providers.Any<ITechnologyPollingProvider>())
        TechnologyPollingFactory.log.Info((object) ("Technology loader found technology polling providers: " + string.Join(",", this.providers.Select<ITechnologyPollingProvider, string>((Func<ITechnologyPollingProvider, string>) (t => t.GetType().FullName)).ToArray<string>())));
      else
        TechnologyPollingFactory.log.Error((object) "Technology loader found 0 technology polling providers");
    }

    protected static IEnumerable<ITechnologyPollingProvider> InitializeMEF(
      ComposablePartCatalog catalog)
    {
      using (CompositionContainer compositionContainer = new CompositionContainer(catalog, Array.Empty<ExportProvider>()))
        return (IEnumerable<ITechnologyPollingProvider>) compositionContainer.GetExports<ITechnologyPollingProvider>().Select<Lazy<ITechnologyPollingProvider>, ITechnologyPollingProvider>((Func<Lazy<ITechnologyPollingProvider>, ITechnologyPollingProvider>) (n => n.Value)).ToList<ITechnologyPollingProvider>();
    }

    public IEnumerable<ITechnologyPolling> Items()
    {
      return this.providers.SelectMany<ITechnologyPollingProvider, ITechnologyPolling>((Func<ITechnologyPollingProvider, IEnumerable<ITechnologyPolling>>) (n => n.Items));
    }

    public IEnumerable<ITechnologyPolling> ItemsByTechnology(string technologyID)
    {
      if (string.IsNullOrEmpty(technologyID))
        throw new ArgumentNullException(nameof (technologyID));
      return this.Items().Where<ITechnologyPolling>((Func<ITechnologyPolling, bool>) (n => n.TechnologyID == technologyID));
    }

    public ITechnologyPolling GetTechnologyPolling(string technologyPollingID)
    {
      if (string.IsNullOrEmpty(technologyPollingID))
        throw new ArgumentNullException(nameof (technologyPollingID));
      return this.Items().Single<ITechnologyPolling>((Func<ITechnologyPolling, bool>) (n => n.TechnologyPollingID == technologyPollingID));
    }

    public int[] EnableDisableAssignments(
      string technologyPollingID,
      bool enable,
      int[] netObjectIDs = null)
    {
      ITechnologyPolling technologyPolling = !string.IsNullOrEmpty(technologyPollingID) ? this.GetTechnologyPolling(technologyPollingID) : throw new ArgumentNullException(nameof (technologyPollingID));
      int[] numArray = (netObjectIDs == null ? technologyPolling.EnableDisableAssignment(enable) : technologyPolling.EnableDisableAssignment(enable, netObjectIDs)) ?? new int[0];
      TechnologyPollingFactory.log.DebugFormat("{0} TechnologyPolling:'{1}' of Technology:'{2}' on NetObjects:'{3}'", new object[4]
      {
        enable ? (object) "Enabled" : (object) "Disabled",
        (object) technologyPollingID,
        (object) technologyPolling.TechnologyID,
        numArray == null ? (object) "" : (object) string.Join<int>(",", (IEnumerable<int>) numArray)
      });
      if (enable)
      {
        foreach (ITechnologyPolling itechnologyPolling in this.ItemsByTechnology(technologyPolling.TechnologyID))
        {
          if (!technologyPollingID.Equals(itechnologyPolling.TechnologyPollingID, StringComparison.Ordinal))
          {
            int[] values = itechnologyPolling.EnableDisableAssignment(false, numArray) ?? new int[0];
            TechnologyPollingFactory.log.DebugFormat("{0} TechnologyPolling:'{1}' of Technology:'{2}' on NetObjects:'{3}'", new object[4]
            {
              (object) "Disabled",
              (object) itechnologyPolling.TechnologyPollingID,
              (object) itechnologyPolling.TechnologyID,
              values == null ? (object) "" : (object) string.Join<int>(",", (IEnumerable<int>) values)
            });
          }
        }
      }
      if (BusinessLayerSettings.Instance.EnableTechnologyPollingAssignmentsChangesAuditing)
        this.changesIndicator.ReportTechnologyPollingAssignmentIndication(technologyPolling, numArray, enable);
      return numArray;
    }

    public IEnumerable<TechnologyPollingAssignment> GetAssignments(string technologyPollingID)
    {
      return !string.IsNullOrEmpty(technologyPollingID) ? this.GetTechnologyPolling(technologyPollingID).GetAssignments() : throw new ArgumentNullException(nameof (technologyPollingID));
    }

    public IEnumerable<TechnologyPollingAssignment> GetAssignments(
      string technologyPollingID,
      int[] netObjectIDs)
    {
      return !string.IsNullOrEmpty(technologyPollingID) ? this.GetTechnologyPolling(technologyPollingID).GetAssignments(netObjectIDs) : throw new ArgumentNullException(nameof (technologyPollingID));
    }

    public IEnumerable<TechnologyPollingAssignment> GetAssignmentsFiltered(
      string[] technologyPollingIDsFilter,
      int[] netObjectIDsFilter,
      string[] targetEntitiesFilter,
      bool[] enabledFilter)
    {
      bool? enabledFilterValue = new bool?();
      if (enabledFilter != null)
      {
        if (enabledFilter.Length == 0)
          yield break;
        else if (((IEnumerable<bool>) enabledFilter).Distinct<bool>().Count<bool>() == 1)
          enabledFilterValue = new bool?(((IEnumerable<bool>) enabledFilter).First<bool>());
      }
      ILookup<string, ITechnologyPolling> technologyPollingsByTechnology = this.Items().Where<ITechnologyPolling>((Func<ITechnologyPolling, bool>) (tp => technologyPollingIDsFilter == null || ((IEnumerable<string>) technologyPollingIDsFilter).Contains<string>(tp.TechnologyPollingID, (IEqualityComparer<string>) StringComparer.Ordinal))).ToLookup<ITechnologyPolling, string>((Func<ITechnologyPolling, string>) (k => k.TechnologyID));
      foreach (ITechnology itechnology in TechnologyManager.Instance.TechnologyFactory.Items().Where<ITechnology>((Func<ITechnology, bool>) (t => targetEntitiesFilter == null || ((IEnumerable<string>) targetEntitiesFilter).Contains<string>(t.TargetEntity, (IEqualityComparer<string>) StringComparer.Ordinal))))
      {
        foreach (ITechnologyPolling itechnologyPolling in technologyPollingsByTechnology[itechnology.TechnologyID])
        {
          IEnumerable<TechnologyPollingAssignment> source = netObjectIDsFilter == null ? itechnologyPolling.GetAssignments() : itechnologyPolling.GetAssignments(netObjectIDsFilter);
          if (enabledFilterValue.HasValue)
            source = source.Where<TechnologyPollingAssignment>((Func<TechnologyPollingAssignment, bool>) (a => a.Enabled == enabledFilterValue.Value));
          foreach (TechnologyPollingAssignment pollingAssignment in source)
            yield return pollingAssignment;
        }
      }
    }
  }
}
