// Decompiled with JetBrains decompiler
// Type: SolarWinds.Orion.Core.BusinessLayer.Thresholds.ThresholdIndicator
// Assembly: SolarWinds.Orion.Core.BusinessLayer, Version=2019.4.5200.9083, Culture=neutral, PublicKeyToken=null
// MVID: E12E8C85-5CD9-4E06-8801-182E5104FADE
// Assembly location: E:\task5.dll

using SolarWinds.InformationService.Contract2;
using SolarWinds.Orion.Core.Common;
using SolarWinds.Orion.Core.Common.Indications;
using SolarWinds.Orion.Core.Common.InformationService;
using SolarWinds.Orion.Core.Common.Models.Thresholds;
using System;
using System.Collections.Generic;
using System.Data;

#nullable disable
namespace SolarWinds.Orion.Core.BusinessLayer.Thresholds
{
  internal class ThresholdIndicator : IThresholdIndicator
  {
    private readonly IInformationServiceProxyFactory _swisFactory;
    private readonly IndicationPublisher _indicationPublisher;
    private DataTable _previousThresholdValues;

    public ThresholdIndicator()
      : this((IInformationServiceProxyFactory) new InformationServiceProxyFactory(), IndicationPublisher.CreateV3())
    {
    }

    public ThresholdIndicator(
      IInformationServiceProxyFactory swisFactory,
      IndicationPublisher indicationReporter)
    {
      if (swisFactory == null)
        throw new ArgumentNullException(nameof (swisFactory));
      if (indicationReporter == null)
        throw new ArgumentNullException(nameof (indicationReporter));
      this._swisFactory = swisFactory;
      this._indicationPublisher = indicationReporter;
    }

    private ThresholdIndicator.InstanceInformation GetInstanceInformation(
      string entityType,
      int instanceId)
    {
      if (string.IsNullOrEmpty(entityType) || instanceId == 0)
        return (ThresholdIndicator.InstanceInformation) null;
      ThresholdIndicator.InstanceInformation instanceInformation = new ThresholdIndicator.InstanceInformation();
      using (IInformationServiceProxy2 connection = this._swisFactory.CreateConnection())
      {
        DataTable dataTable1 = ((IInformationServiceProxy) connection).Query("SELECT TOP 1 Prefix, KeyProperty, NameProperty FROM Orion.NetObjectTypes WHERE EntityType = @entityType", (IDictionary<string, object>) new Dictionary<string, object>()
        {
          {
            nameof (entityType),
            (object) entityType
          }
        });
        if (dataTable1 != null)
        {
          if (dataTable1.Rows.Count == 1)
          {
            string str = dataTable1.Rows[0]["Prefix"] as string;
            object obj1 = dataTable1.Rows[0]["KeyProperty"];
            object obj2 = dataTable1.Rows[0]["NameProperty"];
            instanceInformation.NetObject = string.Format("{0}:{1}", (object) str, (object) instanceId);
            if (obj1 != DBNull.Value && obj1 != DBNull.Value)
            {
              DataTable dataTable2 = ((IInformationServiceProxy) connection).Query(string.Format("SELECT {0} FROM {1} WHERE {2} = @InstanceId", obj2, (object) entityType, obj1), (IDictionary<string, object>) new Dictionary<string, object>()
              {
                {
                  "InstanceId",
                  (object) instanceId
                }
              });
              instanceInformation.InstanceName = dataTable2 == null || dataTable2.Rows.Count <= 0 ? instanceInformation.NetObject : dataTable2.Rows[0][obj2.ToString()].ToString();
            }
            else
              instanceInformation.InstanceName = instanceInformation.NetObject;
          }
        }
      }
      return instanceInformation;
    }

    public void LoadPreviousThresholdData(int instanceId, string thresholdName)
    {
      using (IInformationServiceProxy2 connection = this._swisFactory.CreateConnection())
        this._previousThresholdValues = ((IInformationServiceProxy) connection).Query("SELECT OT.ThresholdOperator,\r\n                    OT.Level1Value,\r\n                    OT.Level1Formula,\r\n                    OT.Level2Value,\r\n                    OT.Level2Formula,\r\n                    OT.WarningPolls,\r\n                    OT.WarningPollsInterval,\r\n                    OT.CriticalPolls,\r\n                    OT.CriticalPollsInterval,\r\n                    OT.WarningEnabled,\r\n                    OT.CriticalEnabled\r\n                    FROM Orion.Thresholds OT\r\n                    WHERE OT.InstanceId = @InstanceId AND OT.Name = @Name", (IDictionary<string, object>) new Dictionary<string, object>()
        {
          {
            "InstanceId",
            (object) instanceId
          },
          {
            "Name",
            (object) thresholdName
          }
        });
    }

    private string GetThresholdEntityType(string thresholdName)
    {
      using (IInformationServiceProxy2 connection = this._swisFactory.CreateConnection())
      {
        DataTable dataTable = ((IInformationServiceProxy) connection).Query("SELECT EntityType FROM Orion.Thresholds WHERE Name = @Name", (IDictionary<string, object>) new Dictionary<string, object>()
        {
          {
            "Name",
            (object) thresholdName
          }
        });
        if (dataTable != null)
        {
          if (dataTable.Rows.Count > 0)
            return dataTable.Rows[0]["EntityType"].ToString();
        }
      }
      return (string) null;
    }

    public void ReportThresholdIndication(Threshold threshold)
    {
      if (threshold == null)
        throw new ArgumentNullException(nameof (threshold));
      ThresholdIndicator.InstanceInformation instanceInformation = this.GetInstanceInformation(this.GetThresholdEntityType(threshold.ThresholdName), threshold.InstanceId);
      PropertyBag propertyBag1 = new PropertyBag();
      ((Dictionary<string, object>) propertyBag1).Add("InstanceType", (object) "Orion.Thresholds");
      ((Dictionary<string, object>) propertyBag1).Add("Name", (object) threshold.ThresholdName);
      ((Dictionary<string, object>) propertyBag1).Add("InstanceName", instanceInformation != null ? (object) instanceInformation.InstanceName : (object) threshold.InstanceId.ToString());
      ((Dictionary<string, object>) propertyBag1).Add("InstanceId", (object) threshold.InstanceId);
      ((Dictionary<string, object>) propertyBag1).Add("ThresholdType", (object) (int) threshold.ThresholdType);
      ((Dictionary<string, object>) propertyBag1).Add("ThresholdOperator", (object) (int) threshold.ThresholdOperator);
      ((Dictionary<string, object>) propertyBag1).Add("Level1Value", (object) threshold.Warning);
      ((Dictionary<string, object>) propertyBag1).Add("Level2Value", (object) threshold.Critical);
      ((Dictionary<string, object>) propertyBag1).Add("Level1Formula", (object) threshold.WarningFormula);
      ((Dictionary<string, object>) propertyBag1).Add("Level2Formula", (object) threshold.CriticalFormula);
      ((Dictionary<string, object>) propertyBag1).Add("WarningPolls", (object) threshold.WarningPolls);
      ((Dictionary<string, object>) propertyBag1).Add("WarningPollsInterval", (object) threshold.WarningPollsInterval);
      ((Dictionary<string, object>) propertyBag1).Add("CriticalPolls", (object) threshold.CriticalPolls);
      ((Dictionary<string, object>) propertyBag1).Add("CriticalPollsInterval", (object) threshold.CriticalPollsInterval);
      ((Dictionary<string, object>) propertyBag1).Add("WarningEnabled", (object) threshold.WarningEnabled);
      ((Dictionary<string, object>) propertyBag1).Add("CriticalEnabled", (object) threshold.CriticalEnabled);
      PropertyBag propertyBag2 = propertyBag1;
      if (instanceInformation != null && !string.IsNullOrEmpty(instanceInformation.NetObject))
        ((Dictionary<string, object>) propertyBag2).Add("NetObject", (object) instanceInformation.NetObject);
      if (this._previousThresholdValues != null && this._previousThresholdValues.Rows.Count > 0)
      {
        PropertyBag propertyBag3 = new PropertyBag();
        object obj1 = this._previousThresholdValues.Rows[0]["ThresholdOperator"];
        object obj2 = this._previousThresholdValues.Rows[0]["Level1Value"];
        object obj3 = this._previousThresholdValues.Rows[0]["Level2Value"];
        object obj4 = this._previousThresholdValues.Rows[0]["Level1Formula"];
        object obj5 = this._previousThresholdValues.Rows[0]["Level2Formula"];
        object obj6 = this._previousThresholdValues.Rows[0]["WarningPolls"];
        object obj7 = this._previousThresholdValues.Rows[0]["WarningPollsInterval"];
        object obj8 = this._previousThresholdValues.Rows[0]["CriticalPolls"];
        object obj9 = this._previousThresholdValues.Rows[0]["CriticalPollsInterval"];
        object obj10 = this._previousThresholdValues.Rows[0]["WarningEnabled"];
        object obj11 = this._previousThresholdValues.Rows[0]["CriticalEnabled"];
        ((Dictionary<string, object>) propertyBag3).Add("ThresholdOperator", obj1 != DBNull.Value ? obj1 : (object) null);
        ((Dictionary<string, object>) propertyBag3).Add("Level1Value", obj2 != DBNull.Value ? obj2 : (object) null);
        ((Dictionary<string, object>) propertyBag3).Add("Level2Value", obj3 != DBNull.Value ? obj3 : (object) null);
        ((Dictionary<string, object>) propertyBag3).Add("Level1Formula", obj4 != DBNull.Value ? obj4 : (object) null);
        ((Dictionary<string, object>) propertyBag3).Add("Level2Formula", obj5 != DBNull.Value ? obj5 : (object) null);
        ((Dictionary<string, object>) propertyBag3).Add("WarningPolls", obj6 != DBNull.Value ? obj6 : (object) null);
        ((Dictionary<string, object>) propertyBag3).Add("WarningPollsInterval", obj7 != DBNull.Value ? obj7 : (object) null);
        ((Dictionary<string, object>) propertyBag3).Add("CriticalPolls", obj8 != DBNull.Value ? obj8 : (object) null);
        ((Dictionary<string, object>) propertyBag3).Add("CriticalPollsInterval", obj9 != DBNull.Value ? obj9 : (object) null);
        ((Dictionary<string, object>) propertyBag3).Add("WarningEnabled", obj10 != DBNull.Value ? obj10 : (object) null);
        ((Dictionary<string, object>) propertyBag3).Add("CriticalEnabled", obj11 != DBNull.Value ? obj11 : (object) null);
        if (((Dictionary<string, object>) propertyBag3).Count > 0)
          ((Dictionary<string, object>) propertyBag2).Add("PreviousProperties", (object) propertyBag3);
        this._previousThresholdValues.Clear();
      }
      this._indicationPublisher.ReportIndication((IIndication) new ThresholdIndication((IndicationType) 2, propertyBag2));
    }

    public class InstanceInformation
    {
      public string NetObject { get; set; }

      public string InstanceName { get; set; }
    }
  }
}
