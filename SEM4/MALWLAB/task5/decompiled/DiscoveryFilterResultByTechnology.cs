// Decompiled with JetBrains decompiler
// Type: SolarWinds.Orion.Core.BusinessLayer.DiscoveryFilterResultByTechnology
// Assembly: SolarWinds.Orion.Core.BusinessLayer, Version=2019.4.5200.9083, Culture=neutral, PublicKeyToken=null
// MVID: E12E8C85-5CD9-4E06-8801-182E5104FADE
// Assembly location: E:\task5.dll

using SolarWinds.Orion.Core.Models.DiscoveredObjects;
using SolarWinds.Orion.Core.Models.Discovery;
using SolarWinds.Orion.Core.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

#nullable disable
namespace SolarWinds.Orion.Core.BusinessLayer
{
  public class DiscoveryFilterResultByTechnology
  {
    public static IEnumerable<IDiscoveredObjectGroup> GetDiscoveryGroups(TechnologyManager mgr)
    {
      return (IEnumerable<IDiscoveredObjectGroup>) DiscoveryFilterResultByTechnology.GetDiscoveryGroupsInternal(mgr);
    }

    private static IEnumerable<TechnologyDiscoveryGroup> GetDiscoveryGroupsInternal(
      TechnologyManager mgr)
    {
      if (mgr == null)
        throw new ArgumentNullException(nameof (mgr));
      foreach (ITechnologyDiscovery itechnologyDiscovery in mgr.TechnologyFactory.Items().OfType<ITechnologyDiscovery>())
      {
        string[] array = mgr.TechnologyPollingFactory.ItemsByTechnology(((ITechnology) itechnologyDiscovery).TechnologyID).OrderByDescending<ITechnologyPolling, int>((Func<ITechnologyPolling, int>) (n => n.Priority)).Select<ITechnologyPolling, string>((Func<ITechnologyPolling, string>) (n => n.TechnologyPollingID)).ToArray<string>();
        TechnologyDiscoveryGroup technologyDiscoveryGroup = new TechnologyDiscoveryGroup(itechnologyDiscovery.ParentType, ((ITechnology) itechnologyDiscovery).DisplayName, itechnologyDiscovery.Icon, itechnologyDiscovery.TreeOrder, array);
        if (itechnologyDiscovery is IDiscoveredObjectGroupWithSelectionMode withSelectionMode)
        {
          technologyDiscoveryGroup.SelectionMode = withSelectionMode.SelectionMode;
          technologyDiscoveryGroup.SelectionDisabled = withSelectionMode.SelectionDisabled;
        }
        yield return technologyDiscoveryGroup;
      }
    }

    private static DiscoveryResultBase FilterByPriority(
      DiscoveryResultBase result,
      TechnologyManager mgr,
      bool onlyMandatory)
    {
      if (result == null)
        throw new ArgumentNullException(nameof (result));
      if (mgr == null)
        throw new ArgumentNullException(nameof (mgr));
      ILookup<string, ITechnologyPolling> lookup = mgr.TechnologyPollingFactory.Items().ToLookup<ITechnologyPolling, string>((Func<ITechnologyPolling, string>) (tp => tp.TechnologyPollingID), (IEqualityComparer<string>) StringComparer.Ordinal);
      List<IDiscoveredObject> source = new List<IDiscoveredObject>();
      foreach (DiscoveryPluginResultBase pluginResult in result.PluginResults)
      {
        IEnumerable<IDiscoveredObject> discoveredObjects = pluginResult.GetDiscoveredObjects();
        source.AddRange(discoveredObjects);
      }
      List<IDiscoveredObjectWithTechnology> list1 = source.OfType<IDiscoveredObjectWithTechnology>().ToList<IDiscoveredObjectWithTechnology>();
      foreach (TechnologyDiscoveryGroup technologyDiscoveryGroup in DiscoveryFilterResultByTechnology.GetDiscoveryGroupsInternal(mgr))
      {
        TechnologyDiscoveryGroup group = technologyDiscoveryGroup;
        if (!onlyMandatory || group.SelectionDisabled)
        {
          IEnumerable<IDiscoveredObjectWithTechnology> list2 = (IEnumerable<IDiscoveredObjectWithTechnology>) list1.Where<IDiscoveredObjectWithTechnology>((Func<IDiscoveredObjectWithTechnology, bool>) (n => group.IsMyGroupedObjectType((IDiscoveredObject) n))).ToList<IDiscoveredObjectWithTechnology>();
          List<List<IDiscoveredObjectWithTechnology>> objectWithTechnologyListList = new List<List<IDiscoveredObjectWithTechnology>>();
          foreach (IDiscoveredObject idiscoveredObject in source)
          {
            if (((DiscoveredObjectBase) group).IsChildOf(idiscoveredObject))
            {
              List<IDiscoveredObjectWithTechnology> objectWithTechnologyList = new List<IDiscoveredObjectWithTechnology>();
              foreach (IDiscoveredObjectWithTechnology objectWithTechnology in list2)
              {
                if (((IDiscoveredObject) objectWithTechnology).IsChildOf(idiscoveredObject))
                  objectWithTechnologyList.Add(objectWithTechnology);
              }
              objectWithTechnologyListList.Add(objectWithTechnologyList);
            }
          }
          foreach (List<IDiscoveredObjectWithTechnology> objectWithTechnologyList in objectWithTechnologyListList)
          {
            if (onlyMandatory)
            {
              if (objectWithTechnologyList.Any<IDiscoveredObjectWithTechnology>((Func<IDiscoveredObjectWithTechnology, bool>) (to => ((IDiscoveredObject) to).IsSelected)))
                continue;
            }
            else
              objectWithTechnologyList.ForEach((Action<IDiscoveredObjectWithTechnology>) (to => ((IDiscoveredObject) to).IsSelected = false));
            DiscoveryFilterResultByTechnology.SelectObjectWithHigherPriority((IEnumerable<IDiscoveredObjectWithTechnology>) objectWithTechnologyList, lookup);
          }
        }
      }
      return result;
    }

    public static DiscoveryResultBase FilterByPriority(
      DiscoveryResultBase result,
      TechnologyManager mgr)
    {
      return DiscoveryFilterResultByTechnology.FilterByPriority(result, mgr, false);
    }

    public static DiscoveryResultBase FilterMandatoryByPriority(
      DiscoveryResultBase result,
      TechnologyManager mgr)
    {
      return DiscoveryFilterResultByTechnology.FilterByPriority(result, mgr, true);
    }

    private static void SelectObjectWithHigherPriority(
      IEnumerable<IDiscoveredObjectWithTechnology> technologyObjects,
      ILookup<string, ITechnologyPolling> technologyPollingsById)
    {
      var data = technologyObjects.Select(n => new
      {
        Object = n,
        SelectionPriority = technologyPollingsById[n.TechnologyPollingID].Select<ITechnologyPolling, int>((Func<ITechnologyPolling, int>) (tp => tp.Priority)).DefaultIfEmpty<int>(0).First<int>()
      }).OrderByDescending(n => n.SelectionPriority).FirstOrDefault();
      if (data == null)
        return;
      ((IDiscoveredObject) data.Object).IsSelected = true;
    }
  }
}
