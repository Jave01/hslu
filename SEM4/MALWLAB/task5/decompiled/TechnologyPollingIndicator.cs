// Decompiled with JetBrains decompiler
// Type: SolarWinds.Orion.Core.BusinessLayer.TechnologyPollingIndicator
// Assembly: SolarWinds.Orion.Core.BusinessLayer, Version=2019.4.5200.9083, Culture=neutral, PublicKeyToken=null
// MVID: E12E8C85-5CD9-4E06-8801-182E5104FADE
// Assembly location: E:\task5.dll

using SolarWinds.InformationService.Contract2;
using SolarWinds.Orion.Core.BusinessLayer.DAL;
using SolarWinds.Orion.Core.Common;
using SolarWinds.Orion.Core.Common.Auditing;
using SolarWinds.Orion.Core.Common.Indications;
using SolarWinds.Orion.Core.Common.InformationService;
using SolarWinds.Orion.Core.Models.Interfaces;
using SolarWinds.Orion.Core.Models.Technology;
using System;
using System.Collections.Generic;
using System.Linq;

#nullable disable
namespace SolarWinds.Orion.Core.BusinessLayer
{
  public class TechnologyPollingIndicator
  {
    private readonly IInformationServiceProxyFactory swisFactory;
    private readonly IndicationPublisher indicationReporter;

    public static Action AuditTechnologiesChanges(
      IEnumerable<IDiscoveredObject> discoveredObjects,
      int nodeId)
    {
      if (!BusinessLayerSettings.Instance.EnableTechnologyPollingAssignmentsChangesAuditing)
        return (Action) (() => { });
      Dictionary<string, ITechnology> technologies = TechnologyManager.Instance.TechnologyFactory.Items().ToDictionary<ITechnology, string, ITechnology>((Func<ITechnology, string>) (k => k.TechnologyID), (Func<ITechnology, ITechnology>) (v => v), (IEqualityComparer<string>) StringComparer.Ordinal);
      Dictionary<string, string> dictionary1 = TechnologyManager.Instance.TechnologyPollingFactory.Items().Where<ITechnologyPolling>((Func<ITechnologyPolling, bool>) (tp => technologies.ContainsKey(tp.TechnologyID))).ToDictionary<ITechnologyPolling, string, string>((Func<ITechnologyPolling, string>) (k => k.TechnologyPollingID), (Func<ITechnologyPolling, string>) (v => technologies[v.TechnologyID].TargetEntity), (IEqualityComparer<string>) StringComparer.Ordinal);
      IEnumerable<IDiscoveredObjectWithTechnology> objectWithTechnologies = discoveredObjects.OfType<IDiscoveredObjectWithTechnology>();
      List<TechnologyPollingAssignment> changedAssignments = new List<TechnologyPollingAssignment>();
      foreach (IDiscoveredObjectWithTechnology objectWithTechnology in objectWithTechnologies)
      {
        if (dictionary1.ContainsKey(objectWithTechnology.TechnologyPollingID) && "Orion.Nodes".Equals(dictionary1[objectWithTechnology.TechnologyPollingID], StringComparison.Ordinal))
        {
          int num1 = nodeId;
          TechnologyPollingAssignment pollingAssignment = TechnologyManager.Instance.TechnologyPollingFactory.GetAssignments(objectWithTechnology.TechnologyPollingID, new int[1]
          {
            num1
          }).FirstOrDefault<TechnologyPollingAssignment>();
          int num2 = pollingAssignment == null ? 0 : (pollingAssignment.Enabled ? 1 : 0);
          bool isSelected = ((IDiscoveredObject) objectWithTechnology).IsSelected;
          int num3 = isSelected ? 1 : 0;
          if (num2 != num3)
            changedAssignments.Add(new TechnologyPollingAssignment()
            {
              TechnologyPollingID = objectWithTechnology.TechnologyPollingID,
              NetObjectID = num1,
              Enabled = isSelected
            });
        }
      }
      return (Action) (() =>
      {
        if (changedAssignments.Count == 0)
          return;
        Dictionary<string, ITechnologyPolling> dictionary2 = TechnologyManager.Instance.TechnologyPollingFactory.Items().ToDictionary<ITechnologyPolling, string, ITechnologyPolling>((Func<ITechnologyPolling, string>) (k => k.TechnologyPollingID), (Func<ITechnologyPolling, ITechnologyPolling>) (v => v), (IEqualityComparer<string>) StringComparer.Ordinal);
        TechnologyPollingIndicator pollingIndicator = new TechnologyPollingIndicator();
        foreach (TechnologyPollingAssignment pollingAssignment in changedAssignments)
          pollingIndicator.ReportTechnologyPollingAssignmentIndication(dictionary2[pollingAssignment.TechnologyPollingID], new int[1]
          {
            pollingAssignment.NetObjectID
          }, (pollingAssignment.Enabled ? 1 : 0) != 0);
      });
    }

    public TechnologyPollingIndicator()
      : this((IInformationServiceProxyFactory) new InformationServiceProxyFactory(), IndicationPublisher.CreateV3())
    {
    }

    public TechnologyPollingIndicator(
      IInformationServiceProxyFactory swisFactory,
      IndicationPublisher indicationReporter)
    {
      if (swisFactory == null)
        throw new ArgumentNullException(nameof (swisFactory));
      if (indicationReporter == null)
        throw new ArgumentNullException(nameof (indicationReporter));
      this.swisFactory = swisFactory;
      this.indicationReporter = indicationReporter;
    }

    public void ReportTechnologyPollingAssignmentIndication(
      ITechnologyPolling technologyPolling,
      int[] netObjectsInstanceIDs,
      bool enabledStateChangedTo)
    {
      if (technologyPolling == null)
        throw new ArgumentNullException(nameof (technologyPolling));
      if (netObjectsInstanceIDs == null)
        throw new ArgumentNullException(nameof (netObjectsInstanceIDs));
      if (netObjectsInstanceIDs.Length == 0)
        return;
      ITechnology technology = TechnologyManager.Instance.TechnologyFactory.GetTechnology(technologyPolling.TechnologyID);
      string netObjectPrefix = NetObjectTypesDAL.GetNetObjectPrefix(this.swisFactory, technology.TargetEntity);
      string entityName = NetObjectTypesDAL.GetEntityName(this.swisFactory, technology.TargetEntity);
      Dictionary<int, string> netObjectsCaptions = NetObjectTypesDAL.GetNetObjectsCaptions(this.swisFactory, technology.TargetEntity, netObjectsInstanceIDs);
      foreach (int objectsInstanceId in netObjectsInstanceIDs)
      {
        PropertyBag propertyBag1 = new PropertyBag();
        ((Dictionary<string, object>) propertyBag1).Add("InstanceType", (object) "Orion.TechnologyPollingAssignments");
        ((Dictionary<string, object>) propertyBag1).Add("InstanceID", (object) objectsInstanceId.ToString());
        ((Dictionary<string, object>) propertyBag1).Add("TechnologyPollingID", (object) technologyPolling.TechnologyPollingID);
        ((Dictionary<string, object>) propertyBag1).Add("Enabled", (object) enabledStateChangedTo);
        ((Dictionary<string, object>) propertyBag1).Add("TargetEntity", (object) technology.TargetEntity);
        ((Dictionary<string, object>) propertyBag1).Add("TechPollingDispName", (object) technologyPolling.DisplayName);
        ((Dictionary<string, object>) propertyBag1).Add("TechnologyDispName", (object) technology.DisplayName);
        PropertyBag propertyBag2 = propertyBag1;
        string str;
        if (netObjectsCaptions.TryGetValue(objectsInstanceId, out str))
          ((Dictionary<string, object>) propertyBag2).Add("NetObjectCaption", (object) str);
        if (netObjectPrefix != null)
        {
          ((Dictionary<string, object>) propertyBag2).Add("NetObjectPrefix", (object) netObjectPrefix);
          ((Dictionary<string, object>) propertyBag2).Add(KnownKeys.NetObject, (object) string.Format("{0}:{1}", (object) netObjectPrefix, (object) objectsInstanceId));
        }
        if (entityName != null)
          ((Dictionary<string, object>) propertyBag2).Add("NetObjectName", (object) entityName);
        this.indicationReporter.ReportIndication((IIndication) new TechnologyPollingAssignmentIndication((IndicationType) 2, propertyBag2));
      }
    }
  }
}
