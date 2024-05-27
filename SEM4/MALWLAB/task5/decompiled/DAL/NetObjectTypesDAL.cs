// Decompiled with JetBrains decompiler
// Type: SolarWinds.Orion.Core.BusinessLayer.DAL.NetObjectTypesDAL
// Assembly: SolarWinds.Orion.Core.BusinessLayer, Version=2019.4.5200.9083, Culture=neutral, PublicKeyToken=null
// MVID: E12E8C85-5CD9-4E06-8801-182E5104FADE
// Assembly location: E:\task5.dll

using SolarWinds.Orion.Core.Common;
using SolarWinds.Orion.Core.Common.InformationService;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

#nullable disable
namespace SolarWinds.Orion.Core.BusinessLayer.DAL
{
  public class NetObjectTypesDAL
  {
    public static Dictionary<int, string> GetNetObjectsCaptions(
      IInformationServiceProxyFactory swisFactory,
      string entityType,
      int[] instanceIds)
    {
      if (swisFactory == null)
        throw new ArgumentNullException(nameof (swisFactory));
      if (string.IsNullOrEmpty(entityType))
        throw new ArgumentException(nameof (entityType));
      Dictionary<int, string> netObjectsCaptions = new Dictionary<int, string>();
      string str1 = (string) null;
      using (IInformationServiceProxy2 connection = swisFactory.CreateConnection())
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
          if (dataTable1.Rows.Count > 0)
          {
            str1 = dataTable1.Rows[0]["Prefix"].ToString();
            string name1 = dataTable1.Rows[0]["KeyProperty"].ToString();
            string name2 = dataTable1.Rows[0]["NameProperty"].ToString();
            if (!string.IsNullOrEmpty(name1))
            {
              if (!string.IsNullOrEmpty(name2))
              {
                DataTable dataTable2 = ((IInformationServiceProxy) connection).Query(string.Format("SELECT {2},{0} FROM {1} WHERE {2} in ({3})", (object) name2, (object) entityType, (object) name1, (object) string.Join<int>(",", (IEnumerable<int>) instanceIds)));
                if (dataTable2 != null)
                {
                  int ordinal1 = dataTable2.Columns[name1].Ordinal;
                  int ordinal2 = dataTable2.Columns[name2].Ordinal;
                  foreach (DataRow dataRow in dataTable2.Rows.Cast<DataRow>())
                  {
                    string s = dataRow[ordinal1].ToString();
                    int result;
                    if (!string.IsNullOrEmpty(s) && int.TryParse(s, out result))
                      netObjectsCaptions[result] = dataRow[ordinal2].ToString();
                  }
                }
              }
            }
          }
        }
      }
      if (string.IsNullOrEmpty(str1))
        str1 = entityType;
      for (int index = 0; index < instanceIds.Length; ++index)
      {
        if (!netObjectsCaptions.ContainsKey(instanceIds[index]))
        {
          string str2 = string.Format("{0}:{1}", (object) str1, (object) instanceIds[index]);
          netObjectsCaptions.Add(instanceIds[index], str2);
        }
      }
      return netObjectsCaptions;
    }

    public static string GetNetObjectPrefix(
      IInformationServiceProxyFactory swisFactory,
      string entityType)
    {
      if (swisFactory == null)
        throw new ArgumentNullException(nameof (swisFactory));
      if (string.IsNullOrEmpty(entityType))
        throw new ArgumentException(nameof (entityType));
      using (IInformationServiceProxy2 connection = swisFactory.CreateConnection())
      {
        DataTable dataTable = ((IInformationServiceProxy) connection).Query("SELECT TOP 1 Prefix FROM Orion.NetObjectTypes WHERE EntityType = @entityType", (IDictionary<string, object>) new Dictionary<string, object>()
        {
          {
            nameof (entityType),
            (object) entityType
          }
        });
        return dataTable == null || dataTable.Rows.Count == 0 ? (string) null : dataTable.Rows[0]["Prefix"].ToString();
      }
    }

    public static string GetEntityName(
      IInformationServiceProxyFactory swisFactory,
      string entityType)
    {
      if (swisFactory == null)
        throw new ArgumentNullException(nameof (swisFactory));
      if (string.IsNullOrEmpty(entityType))
        throw new ArgumentException(nameof (entityType));
      using (IInformationServiceProxy2 connection = swisFactory.CreateConnection())
      {
        DataTable dataTable = ((IInformationServiceProxy) connection).Query("SELECT DisplayName, Name FROM Metadata.Entity WHERE Type = @entityType", (IDictionary<string, object>) new Dictionary<string, object>()
        {
          {
            nameof (entityType),
            (object) entityType
          }
        });
        if (dataTable == null || dataTable.Rows.Count == 0)
          return (string) null;
        string entityName = dataTable.Rows[0]["DisplayName"].ToString();
        if (string.IsNullOrEmpty(entityName))
          entityName = dataTable.Rows[0]["Name"].ToString();
        return entityName;
      }
    }

    public static string GetNetObjectName(
      IInformationServiceProxyFactory swisFactory,
      string entityType)
    {
      if (swisFactory == null)
        throw new ArgumentNullException(nameof (swisFactory));
      if (string.IsNullOrEmpty(entityType))
        throw new ArgumentException(nameof (entityType));
      using (IInformationServiceProxy2 connection = swisFactory.CreateConnection())
      {
        DataTable dataTable = ((IInformationServiceProxy) connection).Query("SELECT Name FROM Orion.NetObjectTypes WHERE EntityType = @entityType", (IDictionary<string, object>) new Dictionary<string, object>()
        {
          {
            nameof (entityType),
            (object) entityType
          }
        });
        return dataTable == null || dataTable.Rows.Count == 0 ? (string) null : dataTable.Rows[0]["Name"].ToString();
      }
    }
  }
}
