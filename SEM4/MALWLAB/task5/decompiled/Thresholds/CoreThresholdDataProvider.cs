// Decompiled with JetBrains decompiler
// Type: SolarWinds.Orion.Core.BusinessLayer.Thresholds.CoreThresholdDataProvider
// Assembly: SolarWinds.Orion.Core.BusinessLayer, Version=2019.4.5200.9083, Culture=neutral, PublicKeyToken=null
// MVID: E12E8C85-5CD9-4E06-8801-182E5104FADE
// Assembly location: E:\task5.dll

using SolarWinds.Orion.Common;
using SolarWinds.Orion.Core.Common.Models.Thresholds;
using SolarWinds.Orion.Core.Common.Thresholds;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Data;
using System.Data.SqlClient;

#nullable disable
namespace SolarWinds.Orion.Core.BusinessLayer.Thresholds
{
  [Export(typeof (ThresholdDataProvider))]
  public class CoreThresholdDataProvider : ThresholdDataProvider
  {
    private const string PercentMemoryUsedName = "Nodes.Stats.PercentMemoryUsed";
    private const string ResponseTimeName = "Nodes.Stats.ResponseTime";
    private const string PercentLossName = "Nodes.Stats.PercentLoss";
    private const string CpuLoadName = "Nodes.Stats.CpuLoad";
    private const string PercentMemoryUsedChartName = "HostAvgPercentMemoryUsed";
    private const string ResponseTimeChartName = "MinMaxAvgRT";
    private const string PercentLossChartName = "PacketLossLine";
    private const string CpuLoadChartName = "CiscoMMAvgCPULoad";

    public virtual IEnumerable<string> GetKnownThresholdNames()
    {
      yield return "Nodes.Stats.PercentMemoryUsed";
      yield return "Nodes.Stats.ResponseTime";
      yield return "Nodes.Stats.PercentLoss";
      yield return "Nodes.Stats.CpuLoad";
    }

    public virtual Type GetThresholdDataProcessor() => typeof (CoreThresholdProcessor);

    public virtual StatisticalTableMetadata GetStatisticalTableMetadata(string thresholdName)
    {
      if (CoreThresholdDataProvider.IsResponseTime(thresholdName))
        return new StatisticalTableMetadata()
        {
          TableName = "ResponseTime_Statistics",
          InstanceIdColumnName = "NodeID",
          MeanColumnName = "AvgResponseTimeMean",
          StdDevColumnName = "AvgResponseTimeStDev",
          MinColumnName = "AvgResponseTimeMin",
          MaxColumnName = "AvgResponseTimeMax",
          CountColumnName = "AvgResponseTimeCount",
          MinDateTime = "MinDateTime",
          MaxDateTime = "MaxDateTime",
          Timestamp = "Timestamp"
        };
      if (CoreThresholdDataProvider.IsPercentLoss(thresholdName))
        return new StatisticalTableMetadata()
        {
          TableName = "ResponseTime_Statistics",
          InstanceIdColumnName = "NodeID",
          MeanColumnName = "PercentLossMean",
          StdDevColumnName = "PercentLossStDev",
          MinColumnName = "PercentLossMin",
          MaxColumnName = "PercentLossMax",
          CountColumnName = "PercentLossCount",
          MinDateTime = "MinDateTime",
          MaxDateTime = "MaxDateTime",
          Timestamp = "Timestamp"
        };
      if (CoreThresholdDataProvider.IsCpuLoad(thresholdName))
        return new StatisticalTableMetadata()
        {
          TableName = "CPULoad_Statistics",
          InstanceIdColumnName = "NodeID",
          MeanColumnName = "AvgLoadMean",
          StdDevColumnName = "AvgLoadStDev",
          MinColumnName = "AvgLoadMin",
          MaxColumnName = "AvgLoadMax",
          CountColumnName = "AvgLoadCount",
          MinDateTime = "MinDateTime",
          MaxDateTime = "MaxDateTime",
          Timestamp = "Timestamp"
        };
      if (!CoreThresholdDataProvider.IsPercentMemoryUsage(thresholdName))
        throw new InvalidOperationException(string.Format("Threshold name '{0}' is not supported.", (object) thresholdName));
      return new StatisticalTableMetadata()
      {
        TableName = "CPULoad_Statistics",
        InstanceIdColumnName = "NodeID",
        MeanColumnName = "AvgPercentMemoryUsedMean",
        StdDevColumnName = "AvgPercentMemoryUsedStDev",
        MinColumnName = "AvgPercentMemoryUsedMin",
        MaxColumnName = "AvgPercentMemoryUsedMax",
        CountColumnName = "AvgPercentMemoryUsedCount",
        MinDateTime = "MinDateTime",
        MaxDateTime = "MaxDateTime",
        Timestamp = "Timestamp"
      };
    }

    public virtual ThresholdMinMaxValue GetThresholdMinMaxValues(
      string thresholdName,
      int instanceId)
    {
      if (CoreThresholdDataProvider.IsResponseTime(thresholdName))
        return new ThresholdMinMaxValue()
        {
          Min = 0.0,
          Max = 100000.0,
          DataType = typeof (int)
        };
      if (CoreThresholdDataProvider.IsPercentLoss(thresholdName))
        return new ThresholdMinMaxValue()
        {
          Min = 0.0,
          Max = 100.0,
          DataType = typeof (int)
        };
      if (CoreThresholdDataProvider.IsCpuLoad(thresholdName))
        return new ThresholdMinMaxValue()
        {
          Min = 0.0,
          Max = 100.0,
          DataType = typeof (int)
        };
      if (!CoreThresholdDataProvider.IsPercentMemoryUsage(thresholdName))
        throw new InvalidOperationException(string.Format("Threshold name '{0}' is not supported.", (object) thresholdName));
      return new ThresholdMinMaxValue()
      {
        Min = 0.0,
        Max = 100.0,
        DataType = typeof (double)
      };
    }

    public virtual StatisticalData[] GetStatisticalData(
      string thresholdName,
      int instanceId,
      DateTime minDateTimeInUtc,
      DateTime maxDateTimeInUtc)
    {
      string str;
      if (CoreThresholdDataProvider.IsResponseTime(thresholdName))
        str = "SELECT AvgResponseTime, [DateTime] FROM ResponseTime_Detail WHERE NodeID = @nodeId AND ([DateTime] between @start and @end)";
      else if (CoreThresholdDataProvider.IsPercentLoss(thresholdName))
        str = "SELECT PercentLoss, [DateTime] FROM ResponseTime_Detail WHERE NodeID = @nodeId AND ([DateTime] between @start and @end)";
      else if (CoreThresholdDataProvider.IsCpuLoad(thresholdName))
      {
        str = "SELECT AvgLoad, [DateTime] FROM CPULoad_Detail WHERE NodeID = @nodeId AND ([DateTime] between @start and @end)";
      }
      else
      {
        if (!CoreThresholdDataProvider.IsPercentMemoryUsage(thresholdName))
          throw new InvalidOperationException(string.Format("Threshold name '{0}' is not supported.", (object) thresholdName));
        str = "SELECT AvgPercentMemoryUsed, [DateTime] FROM CPULoad_Detail WHERE NodeID = @nodeId AND ([DateTime] between @start and @end)";
      }
      List<StatisticalData> statisticalDataList = new List<StatisticalData>();
      using (SqlConnection connection = DatabaseFunctions.CreateConnection())
      {
        using (SqlCommand textCommand = SqlHelper.GetTextCommand(str))
        {
          textCommand.Parameters.AddWithValue("nodeId", (object) instanceId).SqlDbType = SqlDbType.Int;
          textCommand.Parameters.AddWithValue("start", (object) minDateTimeInUtc.ToLocalTime()).SqlDbType = SqlDbType.DateTime;
          textCommand.Parameters.AddWithValue("end", (object) maxDateTimeInUtc.ToLocalTime()).SqlDbType = SqlDbType.DateTime;
          using (IDataReader dataReader = SqlHelper.ExecuteReader(textCommand, connection))
          {
            while (dataReader.Read())
            {
              if (!dataReader.IsDBNull(0) && !dataReader.IsDBNull(1))
                statisticalDataList.Add(new StatisticalData()
                {
                  Value = Convert.ToDouble(dataReader[0]),
                  Date = DatabaseFunctions.GetDateTime(dataReader, 1, DateTimeKind.Local)
                });
            }
          }
        }
      }
      return statisticalDataList.ToArray();
    }

    public virtual string GetThresholdInstanceName(string thresholdName, int instanceId)
    {
      string str = "SELECT [Caption] FROM [NodesData] WHERE [NodeId] = @NodeId";
      using (SqlConnection connection = DatabaseFunctions.CreateConnection())
      {
        using (SqlCommand textCommand = SqlHelper.GetTextCommand(str))
        {
          textCommand.Connection = connection;
          textCommand.Parameters.AddWithValue("NodeId", (object) instanceId).SqlDbType = SqlDbType.Int;
          object obj = textCommand.ExecuteScalar();
          return obj != null && obj != DBNull.Value ? obj.ToString() : string.Empty;
        }
      }
    }

    public virtual string GetStatisticalDataChartName(string thresholdName)
    {
      if (CoreThresholdDataProvider.IsResponseTime(thresholdName))
        return "MinMaxAvgRT";
      if (CoreThresholdDataProvider.IsPercentLoss(thresholdName))
        return "PacketLossLine";
      if (CoreThresholdDataProvider.IsCpuLoad(thresholdName))
        return "CiscoMMAvgCPULoad";
      if (CoreThresholdDataProvider.IsPercentMemoryUsage(thresholdName))
        return "HostAvgPercentMemoryUsed";
      throw new InvalidOperationException(string.Format("Threshold name '{0}' is not supported.", (object) thresholdName));
    }

    private static bool IsResponseTime(string thresholdName)
    {
      return string.Equals(thresholdName, "Nodes.Stats.ResponseTime", StringComparison.OrdinalIgnoreCase);
    }

    private static bool IsPercentLoss(string thresholdName)
    {
      return string.Equals(thresholdName, "Nodes.Stats.PercentLoss", StringComparison.OrdinalIgnoreCase);
    }

    private static bool IsCpuLoad(string thresholdName)
    {
      return string.Equals(thresholdName, "Nodes.Stats.CpuLoad", StringComparison.OrdinalIgnoreCase);
    }

    private static bool IsPercentMemoryUsage(string thresholdName)
    {
      return string.Equals(thresholdName, "Nodes.Stats.PercentMemoryUsed", StringComparison.OrdinalIgnoreCase);
    }
  }
}
