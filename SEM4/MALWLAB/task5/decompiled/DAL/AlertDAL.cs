// Decompiled with JetBrains decompiler
// Type: SolarWinds.Orion.Core.BusinessLayer.DAL.AlertDAL
// Assembly: SolarWinds.Orion.Core.BusinessLayer, Version=2019.4.5200.9083, Culture=neutral, PublicKeyToken=null
// MVID: E12E8C85-5CD9-4E06-8801-182E5104FADE
// Assembly location: E:\task5.dll

using SolarWinds.InformationService.Contract2;
using SolarWinds.Logging;
using SolarWinds.Orion.Common;
using SolarWinds.Orion.Core.Alerting.DAL;
using SolarWinds.Orion.Core.Common;
using SolarWinds.Orion.Core.Common.Alerting;
using SolarWinds.Orion.Core.Common.InformationService;
using SolarWinds.Orion.Core.Common.Models;
using SolarWinds.Orion.Core.Common.Models.Alerts;
using SolarWinds.Orion.Core.Common.Swis;
using SolarWinds.Orion.Core.Models.Alerting;
using SolarWinds.Orion.Core.Strings;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

#nullable disable
namespace SolarWinds.Orion.Core.BusinessLayer.DAL
{
  internal class AlertDAL
  {
    private const int CLRWhereClausesLimit = 100;
    private static readonly Regex _ackRegex = new Regex("AlertStatus.Acknowledged=[0-1]", RegexOptions.IgnoreCase | RegexOptions.Compiled);
    internal static Func<bool> IsInterfacesAllowed = (Func<bool>) (() => SolarWinds.Orion.Core.Common.PackageManager.PackageManager.InstanceWithCache.IsPackageInstalled("Orion.Interfaces"));
    private static readonly Log Log = new Log();
    private static bool _enableLimitationReplacement = BusinessLayerSettings.Instance.EnableLimitationReplacement;
    private static int _limitationSqlExaggeration = BusinessLayerSettings.Instance.LimitationSqlExaggeration;
    internal static IInformationServiceProxyCreator SwisCreator = (IInformationServiceProxyCreator) new SwisConnectionProxyCreator((Func<SwisConnectionProxy>) (() => new SwisConnectionProxyFactory().CreateConnection()));
    internal static IInformationServiceProxy2 SwisProxy = AlertDAL.SwisCreator.Create();

    [DllImport("ole32.dll", CharSet = CharSet.Unicode)]
    public static extern int CLSIDFromString(string sz, out Guid clsid);

    public static bool TryStrToGuid(string s, out Guid value)
    {
      if (s == null || s == "")
      {
        value = Guid.Empty;
        return false;
      }
      s = string.Format("{{{0}}}", (object) s);
      if (AlertDAL.CLSIDFromString(s, out value) >= 0)
        return true;
      value = Guid.Empty;
      return false;
    }

    public static List<KeyValuePair<string, string>> GetAlertList()
    {
      Dictionary<string, string> data = new Dictionary<string, string>();
      using (SqlCommand textCommand = SqlHelper.GetTextCommand("SELECT AlertID, Name FROM AlertConfigurations WITH(NOLOCK)"))
      {
        using (IDataReader dataReader = SqlHelper.ExecuteReader(textCommand))
        {
          while (dataReader.Read())
            data.Add("AA-" + (object) DatabaseFunctions.GetInt32(dataReader, "AlertID"), DatabaseFunctions.GetString(dataReader, "Name"));
        }
      }
      return AlertDAL.SortList(data);
    }

    [Obsolete("Old alerting will be removed. Use GetAlertList() method instead.")]
    public static List<KeyValuePair<string, string>> GetAlertList(bool includeBasic)
    {
      Dictionary<string, string> data = new Dictionary<string, string>();
      if (includeBasic)
      {
        using (SqlCommand textCommand = SqlHelper.GetTextCommand("\r\nSelect al.AlertID, a.AlertName\r\nFrom ActiveAlerts al\r\nINNER JOIN Alerts a WITH(NOLOCK) ON al.AlertID = a.AlertID\r\nGroup By al.AlertID, a.AlertName\r\nOrder By AlertName \r\n"))
        {
          using (IDataReader dataReader = SqlHelper.ExecuteReader(textCommand))
          {
            while (dataReader.Read())
              data.Add(DatabaseFunctions.GetInt32(dataReader, "AlertID").ToString(), DatabaseFunctions.GetString(dataReader, "AlertName"));
          }
        }
      }
      using (SqlCommand textCommand = SqlHelper.GetTextCommand("SELECT AlertDefID, AlertName FROM AlertDefinitions WITH(NOLOCK)\r\n                 Where AlertDefID NOT IN (SELECT AlertRefID As AlertDefID FROM AlertConfigurations)\r\n             ORDER BY AlertName"))
      {
        using (IDataReader dataReader = SqlHelper.ExecuteReader(textCommand))
        {
          while (dataReader.Read())
            data.Add(DatabaseFunctions.GetGuid(dataReader, "AlertDefID").ToString(), DatabaseFunctions.GetString(dataReader, "AlertName"));
        }
      }
      using (SqlCommand textCommand = SqlHelper.GetTextCommand("SELECT AlertID, Name FROM AlertConfigurations WITH(NOLOCK)"))
      {
        using (IDataReader dataReader = SqlHelper.ExecuteReader(textCommand))
        {
          while (dataReader.Read())
            data.Add("AA-" + (object) DatabaseFunctions.GetInt32(dataReader, "AlertID"), DatabaseFunctions.GetString(dataReader, "Name"));
        }
      }
      return AlertDAL.SortList(data);
    }

    private static List<KeyValuePair<string, string>> SortList(Dictionary<string, string> data)
    {
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>((IEnumerable<KeyValuePair<string, string>>) data);
      keyValuePairList.Sort((Comparison<KeyValuePair<string, string>>) ((first, second) => first.Value.CompareTo(second.Value)));
      return keyValuePairList;
    }

    [Obsolete("Method does not return V2 alerts.")]
    public static DataTable GetAlertTable(
      string netObject,
      string deviceType,
      string alertID,
      int maxRecords,
      bool showAcknowledged,
      List<int> limitationIDs)
    {
      return AlertDAL.GetAlertTable(netObject, deviceType, alertID, maxRecords, showAcknowledged, limitationIDs, true);
    }

    [Obsolete("Method does not return V2 alerts.")]
    public static DataTable GetAlertTable(
      string netObject,
      string deviceType,
      string alertID,
      int maxRecords,
      bool showAcknowledged,
      List<int> limitationIDs,
      bool includeBasic)
    {
      return AlertDAL.GetSortableAlertTable(netObject, deviceType, alertID, " ObjectName, AlertName ", maxRecords, showAcknowledged, limitationIDs, includeBasic);
    }

    [Obsolete("Method does not return V2 alerts.")]
    public static DataTable GetSortableAlertTable(
      string netObject,
      string deviceType,
      string alertID,
      string orderByClause,
      int maxRecords,
      bool showAcknowledged,
      List<int> limitationIDs,
      bool includeBasic)
    {
      string str1 = string.Empty;
      List<string> stringList = new List<string>();
      int result = 0;
      bool flag = false;
      string str2 = string.Empty;
      string str3 = string.Empty;
      Regex regex = new Regex("^(\\{){0,1}[0-9a-fA-F]{8}\\-[0-9a-fA-F]{4}\\-[0-9a-fA-F]{4}\\-[0-9a-fA-F]{4}\\-[0-9a-fA-F]{12}(\\}){0,1}$", RegexOptions.Compiled);
      string messageTypeString1 = OrionMessagesHelper.GetMessageTypeString((OrionMessageType) 0);
      string messageTypeString2 = OrionMessagesHelper.GetMessageTypeString((OrionMessageType) 1);
      if (alertID.Equals(messageTypeString1, StringComparison.OrdinalIgnoreCase) || alertID.Equals(messageTypeString2, StringComparison.OrdinalIgnoreCase))
      {
        str2 = alertID;
        alertID = string.Empty;
      }
      if (!string.IsNullOrEmpty(netObject))
      {
        int length = netObject.IndexOf(':', 0);
        if (length != 0)
        {
          string s = netObject.Substring(length + 1);
          if (!int.TryParse(s, out result))
          {
            flag = true;
            str3 = s;
          }
          else
            result = Convert.ToInt32(s);
          str1 = netObject.Substring(0, length);
          foreach (NetObjectType netObjectType in ModuleAlertsMap.NetObjectTypes.Items)
          {
            if (netObjectType.Prefix.ToUpper() == str1.ToUpper())
              stringList.Add(netObjectType.Name);
          }
        }
      }
      StringBuilder stringBuilder1 = new StringBuilder(" AND (AlertStatus.State=2 OR AlertStatus.State=3) ");
      StringBuilder stringBuilder2 = new StringBuilder();
      DataTable sortableAlertTable;
      using (SqlCommand textCommand = SqlHelper.GetTextCommand(""))
      {
        if (!showAcknowledged)
          stringBuilder1.Append(" AND AlertStatus.Acknowledged=0 ");
        if (!string.IsNullOrEmpty(netObject) && result != 0 | flag && !string.IsNullOrEmpty(str1) && stringList.Count > 0)
        {
          if (str1.Equals("N", StringComparison.OrdinalIgnoreCase) && result != 0)
          {
            stringBuilder1.AppendFormat(" AND Nodes.NodeID={0} ", (object) result);
          }
          else
          {
            StringBuilder stringBuilder3 = new StringBuilder();
            string str4 = string.Empty;
            foreach (string str5 in stringList)
            {
              stringBuilder3.AppendFormat(" {1} AlertStatus.ObjectType='{0}' ", (object) str5, (object) str4);
              str4 = "OR";
            }
            stringBuilder2.AppendFormat(" AND (({0}) AND AlertStatus.ActiveObject=", (object) stringBuilder3);
            if (flag)
              stringBuilder2.AppendFormat("'{0}') ", (object) str3);
            else
              stringBuilder2.AppendFormat("{0}) ", (object) result);
          }
        }
        else if (!string.IsNullOrEmpty(deviceType))
        {
          stringBuilder1.Append(" AND (Nodes.MachineType Like @machineType) ");
          textCommand.Parameters.AddWithValue("@machineType", (object) deviceType);
        }
        if (regex.IsMatch(alertID))
        {
          stringBuilder1.Append(" AND (AlertStatus.AlertDefID=@alertDefID) ");
          textCommand.Parameters.AddWithValue("@alertDefID", (object) alertID);
        }
        else if (!string.IsNullOrEmpty(alertID))
          stringBuilder1.AppendFormat(" AND (AlertStatus.AlertDefID='{0}') ", (object) Guid.Empty);
        string str6 = "IF OBJECT_ID('tempdb..#Nodes') IS NOT NULL\tDROP TABLE #Nodes\r\nSELECT Nodes.* INTO #Nodes FROM Nodes WHERE 1=1;";
        string str7 = Limitation.LimitSQL(str6, (IEnumerable<int>) limitationIDs);
        int num = !AlertDAL._enableLimitationReplacement ? 0 : (str7.Length / str6.Length > AlertDAL._limitationSqlExaggeration ? 1 : 0);
        string str8 = num != 0 ? str7 : string.Empty;
        string str9 = num != 0 ? "IF OBJECT_ID('tempdb..#Nodes') IS NOT NULL\tDROP TABLE #Nodes" : string.Empty;
        if (((str2.Equals(messageTypeString1, StringComparison.OrdinalIgnoreCase) ? 1 : (!includeBasic ? 1 : 0)) | (flag ? 1 : 0)) != 0)
          textCommand.CommandText = string.Format("{3}SELECT TOP {0} a.*, WebCommunityStrings.[GUID] AS [GUID] FROM ( {1} )a LEFT OUTER JOIN WebCommunityStrings WITH(NOLOCK) ON WebCommunityStrings.CommunityString = a.Community Order By {2} \r\n{4}", (object) maxRecords, (object) AlertDAL.GetAdvAlertSwql(stringBuilder1.ToString(), stringBuilder2.ToString(), netObject, messageTypeString1, limitationIDs, true, true), (object) orderByClause, (object) str8, (object) str9);
        else if (str2.Equals(messageTypeString2, StringComparison.OrdinalIgnoreCase))
        {
          textCommand.CommandText = string.Format("{3}SELECT TOP {0} a.*, WebCommunityStrings.[GUID] AS [GUID] FROM ( {1} )a LEFT OUTER JOIN WebCommunityStrings WITH(NOLOCK) ON WebCommunityStrings.CommunityString = a.Community Order By {2} \r\n{4}", (object) maxRecords, (object) AlertDAL.GetBasicAlertSwql(netObject, deviceType, alertID, limitationIDs, true, true), (object) orderByClause, (object) str8, (object) str9);
        }
        else
        {
          string advAlertSwql = AlertDAL.GetAdvAlertSwql(stringBuilder1.ToString(), stringBuilder2.ToString(), netObject, messageTypeString1, limitationIDs, true, true);
          string basicAlertSwql = AlertDAL.GetBasicAlertSwql(netObject, deviceType, alertID, limitationIDs, true, true);
          string empty = string.Empty;
          string str10 = !string.IsNullOrEmpty(advAlertSwql) ? string.Format("( {0}  Union ( {1} ))", (object) advAlertSwql, (object) basicAlertSwql) : string.Format("({0})", (object) basicAlertSwql);
          textCommand.CommandText = string.Format("{3}SELECT TOP {0} a.*, WebCommunityStrings.[GUID] AS [GUID] FROM ({1})a LEFT OUTER JOIN WebCommunityStrings WITH(NOLOCK) ON WebCommunityStrings.CommunityString = a.Community Order By {2} \r\n{4}", (object) maxRecords, (object) str10, (object) orderByClause, (object) str8, (object) str9);
        }
        sortableAlertTable = SqlHelper.ExecuteDataTable(textCommand);
      }
      sortableAlertTable.TableName = "History";
      return sortableAlertTable;
    }

    private static DataTable GetAvailableObjectTypes(bool federationEnabled = false)
    {
      string str = "SELECT EntityType, Name, Prefix, KeyProperty, KeyPropertyIndex FROM Orion.NetObjectTypes (nolock=true)";
      return InformationServiceProxyExtensions.QueryWithAppendedErrors((IInformationServiceProxy) AlertDAL.SwisProxy, str, federationEnabled);
    }

    private static Dictionary<string, int> GetStatusesForSwisEntities(
      string entityName,
      string entityIdName,
      IEnumerable<string> entityIds,
      bool federationEnabled = false)
    {
      Dictionary<string, int> statusesForSwisEntities = new Dictionary<string, int>((IEqualityComparer<string>) StringComparer.InvariantCultureIgnoreCase);
      if (entityIds.Count<string>() > 0)
      {
        string format = "SELECT Status, {1} AS Id FROM {0} (nolock=true) WHERE {1} IN ({2})";
        string str1 = string.Join(",", entityIds);
        string str2 = string.Format(format, (object) entityName, (object) entityIdName, (object) str1);
        foreach (DataRow row in (InternalDataCollectionBase) InformationServiceProxyExtensions.QueryWithAppendedErrors((IInformationServiceProxy) AlertDAL.SwisProxy, str2, federationEnabled).Rows)
        {
          string key = row["Id"] != DBNull.Value ? Convert.ToString(row["id"]) : string.Empty;
          int int32 = row["Status"] != DBNull.Value ? Convert.ToInt32(row["Status"]) : 0;
          if (!string.IsNullOrEmpty(key) && !statusesForSwisEntities.ContainsKey(key))
            statusesForSwisEntities.Add(key, int32);
        }
      }
      return statusesForSwisEntities;
    }

    private static string GetAlertNote(string alertDefID, string activeObject, string objectType)
    {
      string alertNote = string.Empty;
      if (new Regex("^(\\{){0,1}[0-9a-fA-F]{8}\\-[0-9a-fA-F]{4}\\-[0-9a-fA-F]{4}\\-[0-9a-fA-F]{4}\\-[0-9a-fA-F]{12}(\\}){0,1}$", RegexOptions.Compiled).IsMatch(alertDefID))
      {
        string str = "SELECT Notes FROM Orion.AlertStatus (nolock=true) WHERE AlertDefID=@alertDefID AND ActiveObject=@activeObject AND ObjectType=@objectType";
        DataTable dataTable = InformationServiceProxyExtensions.QueryWithAppendedErrors((IInformationServiceProxy) AlertDAL.SwisProxy, str, new Dictionary<string, object>()
        {
          {
            nameof (alertDefID),
            (object) alertDefID
          },
          {
            nameof (activeObject),
            (object) activeObject
          },
          {
            nameof (objectType),
            (object) objectType
          }
        });
        if (dataTable.Rows.Count > 0)
          alertNote = dataTable.Rows[0]["Notes"] != DBNull.Value ? Convert.ToString(dataTable.Rows[0]["Notes"]) : string.Empty;
      }
      else
      {
        string str = "SELECT AlertID, ObjectID AS ActiveObject, ObjectType, AlertNotes FROM Orion.ActiveAlerts (nolock=true)" + " WHERE AlertID=@alertID AND ObjectID=@objectID AND ObjectType=@objectType ";
        DataTable dataTable = InformationServiceProxyExtensions.QueryWithAppendedErrors((IInformationServiceProxy) AlertDAL.SwisProxy, str, new Dictionary<string, object>()
        {
          {
            "alertID",
            (object) alertDefID
          },
          {
            "objectID",
            (object) activeObject
          },
          {
            nameof (objectType),
            (object) objectType
          }
        });
        if (dataTable.Rows.Count > 0)
          alertNote = dataTable.Rows[0]["AlertNotes"] != DBNull.Value ? Convert.ToString(dataTable.Rows[0]["AlertNotes"]) : string.Empty;
      }
      return alertNote;
    }

    public static Dictionary<string, string> GetNotesForActiveAlerts(
      IEnumerable<ActiveAlert> activeAlerts)
    {
      Dictionary<string, string> res = new Dictionary<string, string>((IEqualityComparer<string>) StringComparer.InvariantCultureIgnoreCase);
      string strCondition = string.Empty;
      StringBuilder stringBuilder = new StringBuilder();
      bool flag1 = true;
      string sqlQuery = string.Empty;
      Action<SqlParameter[]> action1 = (Action<SqlParameter[]>) (parameters =>
      {
        if (string.IsNullOrEmpty(strCondition))
          return;
        sqlQuery = string.Format("SELECT AlertDefID, ActiveObject, ObjectType, Notes FROM AlertStatus WITH(NOLOCK) WHERE {0}", (object) strCondition);
        using (SqlCommand textCommand = SqlHelper.GetTextCommand(sqlQuery))
        {
          if (parameters != null)
            textCommand.Parameters.AddRange(parameters);
          foreach (DataRow row in (InternalDataCollectionBase) SqlHelper.ExecuteDataTable(textCommand).Rows)
          {
            string str1 = row["AlertDefID"] != DBNull.Value ? Convert.ToString(row["AlertDefID"]) : string.Empty;
            string str2 = row["ActiveObject"] != DBNull.Value ? Convert.ToString(row["ActiveObject"]) : string.Empty;
            string str3 = row["ObjectType"] != DBNull.Value ? Convert.ToString(row["ObjectType"]) : string.Empty;
            string str4 = row["Notes"] != DBNull.Value ? Convert.ToString(row["Notes"]) : string.Empty;
            string key = string.Format("{0}|{1}|{2}", (object) str1, (object) str2, (object) str3);
            if (!res.ContainsKey(key))
              res.Add(key, str4);
          }
        }
      });
      Action<SqlParameter[]> action2 = (Action<SqlParameter[]>) (parameters =>
      {
        if (string.IsNullOrEmpty(strCondition))
          return;
        sqlQuery = string.Format("SELECT AlertID, ObjectID AS ActiveObject, ObjectType, AlertNotes FROM ActiveAlerts WITH(NOLOCK) WHERE {0}", (object) strCondition);
        using (SqlCommand textCommand = SqlHelper.GetTextCommand(sqlQuery))
        {
          if (parameters != null)
            textCommand.Parameters.AddRange(parameters);
          foreach (DataRow row in (InternalDataCollectionBase) SqlHelper.ExecuteDataTable(textCommand).Rows)
          {
            string str5 = row["AlertID"] != DBNull.Value ? Convert.ToString(row["AlertID"]) : string.Empty;
            string str6 = row["ActiveObject"] != DBNull.Value ? Convert.ToString(row["ActiveObject"]) : string.Empty;
            string str7 = row["ObjectType"] != DBNull.Value ? Convert.ToString(row["ObjectType"]) : string.Empty;
            string str8 = row["AlertNotes"] != DBNull.Value ? Convert.ToString(row["AlertNotes"]) : string.Empty;
            string key = string.Format("{0}|{1}|{2}", (object) str5, (object) str6, (object) str7);
            if (!res.ContainsKey(key))
              res.Add(key, str8);
          }
        }
      });
      IEnumerable<ActiveAlert> activeAlerts1 = activeAlerts.Where<ActiveAlert>((System.Func<ActiveAlert, bool>) (item => item.AlertType == 0));
      int num1 = 0;
      List<SqlParameter> sqlParameterList = new List<SqlParameter>();
      foreach (ActiveAlert activeAlert in activeAlerts1)
      {
        if (!flag1)
          stringBuilder.Append(" OR ");
        stringBuilder.AppendFormat("(AlertDefID=@alertDefID{0} AND ActiveObject=@activeObject{0} AND ObjectType=@objectType{0})", (object) num1);
        sqlParameterList.Add(new SqlParameter(string.Format("@alertDefID{0}", (object) num1), (object) activeAlert.AlertDefId));
        sqlParameterList.Add(new SqlParameter(string.Format("@activeObject{0}", (object) num1), (object) activeAlert.ActiveNetObject));
        sqlParameterList.Add(new SqlParameter(string.Format("@objectType{0}", (object) num1), (object) activeAlert.ObjectType));
        flag1 = false;
        ++num1;
        if (num1 % 520 == 0)
        {
          strCondition = stringBuilder.ToString();
          stringBuilder.Clear();
          action1(sqlParameterList.ToArray());
          sqlParameterList.Clear();
          num1 = 0;
          strCondition = string.Empty;
          flag1 = true;
        }
      }
      strCondition = stringBuilder.ToString();
      if (!string.IsNullOrEmpty(strCondition))
        action1(sqlParameterList.ToArray());
      stringBuilder.Clear();
      int num2 = 0;
      strCondition = string.Empty;
      bool flag2 = true;
      sqlParameterList.Clear();
      foreach (ActiveAlert activeAlert in activeAlerts.Where<ActiveAlert>((System.Func<ActiveAlert, bool>) (item => item.AlertType == 1)))
      {
        if (!flag2)
          stringBuilder.Append(" OR ");
        stringBuilder.AppendFormat("(AlertID=@alertID{0} AND ObjectID=@objectID{0} AND ObjectType=@objectType{0})", (object) num2);
        sqlParameterList.Add(new SqlParameter(string.Format("@alertID{0}", (object) num2), (object) activeAlert.AlertDefId));
        sqlParameterList.Add(new SqlParameter(string.Format("@objectID{0}", (object) num2), (object) activeAlert.ActiveNetObject));
        sqlParameterList.Add(new SqlParameter(string.Format("@objectType{0}", (object) num2), (object) activeAlert.ObjectType));
        flag2 = false;
        ++num2;
        if (num2 % 520 == 0)
        {
          strCondition = stringBuilder.ToString();
          stringBuilder.Clear();
          action2(sqlParameterList.ToArray());
          sqlParameterList.Clear();
          num2 = 0;
          strCondition = string.Empty;
        }
      }
      strCondition = stringBuilder.ToString();
      action2(sqlParameterList.ToArray());
      return res;
    }

    private static Dictionary<string, int> GetTriggerCountForActiveAlerts(
      IEnumerable<ActiveAlert> activeAlerts)
    {
      Dictionary<string, int> res = new Dictionary<string, int>((IEqualityComparer<string>) StringComparer.InvariantCultureIgnoreCase);
      string strCondition = string.Empty;
      bool flag = true;
      StringBuilder stringBuilder = new StringBuilder();
      ActiveAlert[] array = activeAlerts.Where<ActiveAlert>((System.Func<ActiveAlert, bool>) (item => item.AlertType == 0)).ToArray<ActiveAlert>();
      int num = 0;
      Action<SqlParameter[]> action = (Action<SqlParameter[]>) (parameters =>
      {
        if (string.IsNullOrEmpty(strCondition))
          return;
        using (SqlCommand textCommand = SqlHelper.GetTextCommand(string.Format("SELECT AlertDefID, ActiveObject, ObjectType, TriggerCount FROM AlertStatus WITH(NOLOCK) WHERE {0}", (object) strCondition)))
        {
          if (parameters != null)
            textCommand.Parameters.AddRange(parameters);
          foreach (DataRow row in (InternalDataCollectionBase) SqlHelper.ExecuteDataTable(textCommand).Rows)
          {
            string str1 = row["AlertDefID"] != DBNull.Value ? Convert.ToString(row["AlertDefID"]) : string.Empty;
            string str2 = row["ActiveObject"] != DBNull.Value ? Convert.ToString(row["ActiveObject"]) : string.Empty;
            string str3 = row["ObjectType"] != DBNull.Value ? Convert.ToString(row["ObjectType"]) : string.Empty;
            int int32 = row["TriggerCount"] != DBNull.Value ? Convert.ToInt32(row["TriggerCount"]) : 0;
            string key = string.Format("{0}|{1}|{2}", (object) str1, (object) str2, (object) str3);
            if (!res.ContainsKey(key))
              res.Add(key, int32);
          }
        }
      });
      List<SqlParameter> sqlParameterList = new List<SqlParameter>();
      foreach (ActiveAlert activeAlert in array)
      {
        if (!flag)
          stringBuilder.Append(" OR ");
        stringBuilder.AppendFormat("(AlertDefID=@alertDefID{0} AND ActiveObject=@activeObject{0} AND ObjectType=@objectType{0})", (object) num);
        sqlParameterList.Add(new SqlParameter(string.Format("@alertDefID{0}", (object) num), (object) activeAlert.AlertDefId));
        sqlParameterList.Add(new SqlParameter(string.Format("@activeObject{0}", (object) num), (object) activeAlert.ActiveNetObject));
        sqlParameterList.Add(new SqlParameter(string.Format("@objectType{0}", (object) num), (object) activeAlert.ObjectType));
        ++num;
        flag = false;
        if (num % 520 == 0)
        {
          strCondition = stringBuilder.ToString();
          stringBuilder.Clear();
          action(sqlParameterList.ToArray());
          sqlParameterList.Clear();
          num = 0;
          strCondition = string.Empty;
          flag = true;
        }
      }
      action(sqlParameterList.ToArray());
      foreach (ActiveAlert activeAlert in activeAlerts.Where<ActiveAlert>((System.Func<ActiveAlert, bool>) (item => item.AlertType == 1)))
      {
        string key = string.Format("{0}|{1}|{2}", (object) activeAlert.AlertDefId, (object) activeAlert.ActiveNetObject, (object) activeAlert.ObjectType);
        if (!res.ContainsKey(key))
          res.Add(key, 1);
      }
      return res;
    }

    private static Dictionary<string, string> GetFullUserNames(
      IEnumerable<string> accountIDs,
      bool federationEnabled = false)
    {
      Dictionary<string, string> fullUserNames = new Dictionary<string, string>((IEqualityComparer<string>) StringComparer.InvariantCultureIgnoreCase);
      if (accountIDs.Any<string>())
      {
        List<string> list = accountIDs.Distinct<string>().ToList<string>();
        string empty = string.Empty;
        foreach (string str in list)
        {
          if (!string.IsNullOrEmpty(empty))
            empty += " OR ";
          empty += string.Format("AccountID='{0}'", (object) str);
        }
        string str1 = "SELECT AccountID, DisplayName FROM Orion.Accounts (nolock=true) WHERE " + empty;
        foreach (DataRow row in (InternalDataCollectionBase) InformationServiceProxyExtensions.QueryWithAppendedErrors((IInformationServiceProxy) AlertDAL.SwisProxy, str1, federationEnabled).Rows)
        {
          string key = row["AccountID"] != DBNull.Value ? Convert.ToString(row["AccountID"]) : string.Empty;
          string str2 = row["DisplayName"] != DBNull.Value ? Convert.ToString(row["DisplayName"]) : string.Empty;
          if (!string.IsNullOrEmpty(key) && !fullUserNames.ContainsKey(key))
            fullUserNames.Add(key, str2);
        }
      }
      return fullUserNames;
    }

    public static ActiveAlertPage GetPageableActiveAlerts(
      PageableActiveAlertRequest pageableRequest,
      ActiveAlertsRequest request = null)
    {
      IEnumerable<CustomProperty> propertiesForEntity = CustomPropertyMgr.GetCustomPropertiesForEntity("Orion.AlertConfigurationsCustomProperties");
      List<ErrorMessage> errors;
      DataTable table = AlertDAL.ConvertActiveAlertsToTable((IEnumerable<ActiveAlert>) AlertDAL.GetActiveAlerts(pageableRequest, propertiesForEntity, out errors), propertiesForEntity);
      DataTable filteredTable = AlertDAL.GetFilteredTable(pageableRequest, table, request);
      IEnumerable<DataRow> filteredAlertRows = AlertDAL.GetFilteredAlertRows(pageableRequest, filteredTable, propertiesForEntity);
      List<ActiveAlert> list = AlertDAL.ConvertRowsToActiveAlerts(AlertDAL.GetSortedAlerts(pageableRequest, filteredAlertRows), propertiesForEntity).ToList<ActiveAlert>();
      return new ActiveAlertPage()
      {
        TotalRow = list.Count,
        ActiveAlerts = (IEnumerable<ActiveAlert>) list.Skip<ActiveAlert>(pageableRequest.StartRow).Take<ActiveAlert>(pageableRequest.PageSize).ToList<ActiveAlert>(),
        ErrorsMessages = (IEnumerable<ErrorMessage>) errors
      };
    }

    private static List<ActiveAlert> GetActiveAlerts(
      PageableActiveAlertRequest request,
      IEnumerable<CustomProperty> customProperties,
      out List<ErrorMessage> errors)
    {
      return new ActiveAlertDAL().GetActiveAlerts(customProperties, request.LimitationIDs, out errors, request.FederationEnabled, request.IncludeNotTriggered).ToList<ActiveAlert>();
    }

    private static DataTable ConvertActiveAlertsToTable(
      IEnumerable<ActiveAlert> alerts,
      IEnumerable<CustomProperty> customProperties)
    {
      DataTable table = new DataTable();
      table.Locale = CultureInfo.InvariantCulture;
      table.Columns.Add("ActiveAlertID", typeof (string));
      table.Columns.Add("AlertDefID", typeof (string));
      table.Columns.Add("ActiveAlertType", typeof (ActiveAlertType));
      table.Columns.Add("AlertName", typeof (string));
      table.Columns.Add("AlertMessage", typeof (string));
      table.Columns.Add("TriggeringObject", typeof (string));
      table.Columns.Add("TriggeringObjectEntityName", typeof (string));
      table.Columns.Add("TriggeringObjectStatus", typeof (string));
      table.Columns.Add("TriggeringObjectDetailsUrl", typeof (string));
      table.Columns.Add("TriggeringObjectEntityUri", typeof (string));
      table.Columns.Add("RelatedNode", typeof (string));
      table.Columns.Add("RelatedNodeID", typeof (int));
      table.Columns.Add("RelatedNodeEntityUri", typeof (string));
      table.Columns.Add("RelatedNodeDetailsUrl", typeof (string));
      table.Columns.Add("RelatedNodeStatus", typeof (int));
      table.Columns.Add("ActiveTime", typeof (TimeSpan));
      table.Columns.Add("ActiveTimeSort", typeof (string));
      table.Columns.Add("ActiveTimeDisplay", typeof (string));
      table.Columns.Add("TriggerTime", typeof (DateTime));
      table.Columns.Add("TriggerTimeDisplay", typeof (string));
      table.Columns.Add("LastTriggeredDateTime", typeof (DateTime));
      table.Columns.Add("TriggerCount", typeof (int));
      table.Columns.Add("AcknowledgedBy", typeof (string));
      table.Columns.Add("AcknowledgedByFullName", typeof (string));
      table.Columns.Add("AcknowledgeTime", typeof (DateTime));
      table.Columns.Add("AcknowledgeTimeDisplay", typeof (string));
      table.Columns.Add("Notes", typeof (string));
      table.Columns.Add("NumberOfNotes", typeof (int));
      table.Columns.Add("Severity", typeof (ActiveAlertSeverity));
      table.Columns.Add("SeverityOrder", typeof (int));
      table.Columns.Add("SeverityText", typeof (string));
      table.Columns.Add("ActiveNetObject", typeof (string));
      table.Columns.Add("ObjectType", typeof (string));
      table.Columns.Add("LegacyAlert", typeof (bool));
      table.Columns.Add("ObjectTriggeredThisAlertDisplayName", typeof (string));
      table.Columns.Add("Canned", typeof (bool));
      table.Columns.Add("Category", typeof (string));
      table.Columns.Add("IncidentNumber", typeof (string));
      table.Columns.Add("IncidentUrl", typeof (string));
      table.Columns.Add("Assignee", typeof (string));
      table.Columns.Add("SiteID", typeof (int));
      table.Columns.Add("SiteName", typeof (string));
      foreach (CustomProperty customProperty in customProperties)
      {
        table.Columns.Add(string.Format("CP_{0}", (object) customProperty.PropertyName), customProperty.PropertyType);
        if (customProperty.PropertyType == typeof (bool) || customProperty.PropertyType == typeof (float) || customProperty.PropertyType == typeof (int) || customProperty.PropertyType == typeof (DateTime))
          table.Columns.Add(string.Format("CP_{0}_Display", (object) customProperty.PropertyName), typeof (string));
      }
      foreach (ActiveAlert alert in alerts)
      {
        DataRow row = table.NewRow();
        row["ActiveAlertID"] = (object) alert.Id;
        row["AlertDefID"] = (object) alert.AlertDefId;
        row["ActiveAlertType"] = (object) alert.AlertType;
        row["AlertName"] = (object) alert.Name;
        row["AlertMessage"] = (object) alert.Message;
        row["TriggeringObject"] = (object) alert.TriggeringObjectCaption;
        row["TriggeringObjectEntityName"] = (object) alert.TriggeringObjectEntityName;
        row["TriggeringObjectStatus"] = (object) alert.TriggeringObjectStatus;
        row["TriggeringObjectDetailsUrl"] = (object) alert.TriggeringObjectDetailsUrl;
        row["TriggeringObjectEntityUri"] = (object) alert.TriggeringObjectEntityUri;
        row["RelatedNode"] = (object) alert.RelatedNodeCaption;
        row["RelatedNodeID"] = (object) alert.RelatedNodeID;
        row["RelatedNodeEntityUri"] = (object) alert.RelatedNodeEntityUri;
        row["RelatedNodeDetailsUrl"] = (object) alert.RelatedNodeDetailsUrl;
        row["RelatedNodeStatus"] = (object) alert.RelatedNodeStatus;
        row["ActiveTime"] = (object) alert.ActiveTime;
        row["ActiveTimeSort"] = (object) alert.ActiveTime.ToString("d\\.hh\\:mm\\:ss", (IFormatProvider) CultureInfo.InvariantCulture);
        row["ActiveTimeDisplay"] = (object) alert.ActiveTimeDisplay;
        row["TriggerTime"] = (object) DateTime.SpecifyKind(alert.TriggerDateTime, DateTimeKind.Utc);
        row["TriggerTimeDisplay"] = (object) DateTimeHelper.ConvertToDisplayDate(alert.TriggerDateTime);
        row["LastTriggeredDateTime"] = (object) alert.LastTriggeredDateTime;
        row["TriggerCount"] = (object) alert.TriggerCount;
        row["AcknowledgedBy"] = (object) alert.AcknowledgedBy;
        row["AcknowledgedByFullName"] = (object) alert.AcknowledgedByFullName;
        row["AcknowledgeTime"] = (object) DateTime.SpecifyKind(alert.AcknowledgedDateTime, DateTimeKind.Utc);
        row["AcknowledgeTimeDisplay"] = !(alert.AcknowledgedDateTime != DateTime.MinValue) || alert.AcknowledgedDateTime.Year == 1899 ? (object) Resources.WEBJS_PS0_59 : (object) DateTimeHelper.ConvertToDisplayDate(alert.AcknowledgedDateTime);
        row["Notes"] = (object) alert.Notes;
        row["NumberOfNotes"] = (object) alert.NumberOfNotes;
        row["Severity"] = (object) alert.Severity;
        row["SeverityText"] = (object) alert.Severityi18nMessage;
        row["SeverityOrder"] = (object) alert.SeverityOrder;
        row["ActiveNetObject"] = (object) alert.ActiveNetObject;
        row["ObjectType"] = (object) alert.ObjectType;
        row["LegacyAlert"] = (object) alert.LegacyAlert;
        row["ObjectTriggeredThisAlertDisplayName"] = (object) alert.ObjectTriggeredThisAlertDisplayName;
        row["Canned"] = (object) alert.Canned;
        row["Category"] = (object) alert.Category;
        row["IncidentNumber"] = (object) alert.IncidentNumber;
        row["IncidentUrl"] = (object) alert.IncidentUrl;
        row["Assignee"] = (object) alert.AssignedTo;
        row["SiteID"] = (object) alert.SiteID;
        row["SiteName"] = (object) alert.SiteName;
        if (alert.CustomProperties != null)
        {
          foreach (CustomProperty customProperty in customProperties)
          {
            if (alert.CustomProperties.ContainsKey(customProperty.PropertyName))
            {
              row[string.Format("CP_{0}", (object) customProperty.PropertyName)] = alert.CustomProperties[customProperty.PropertyName] != null ? alert.CustomProperties[customProperty.PropertyName] : (object) DBNull.Value;
              if (customProperty.PropertyType == typeof (bool) || customProperty.PropertyType == typeof (float) || customProperty.PropertyType == typeof (int))
                row[string.Format("CP_{0}_Display", (object) customProperty.PropertyName)] = alert.CustomProperties[customProperty.PropertyName] != null ? (object) Convert.ToString(alert.CustomProperties[customProperty.PropertyName]) : (object) string.Empty;
              else if (customProperty.PropertyType == typeof (DateTime))
              {
                if (alert.CustomProperties[customProperty.PropertyName] != null)
                  row[string.Format("CP_{0}_Display", (object) customProperty.PropertyName)] = (object) DateTimeHelper.ConvertToDisplayDate((DateTime) alert.CustomProperties[customProperty.PropertyName]);
                else
                  row[string.Format("CP_{0}_Display", (object) customProperty.PropertyName)] = (object) DBNull.Value;
              }
            }
          }
        }
        table.Rows.Add(row);
      }
      return table;
    }

    private static IEnumerable<ActiveAlert> ConvertRowsToActiveAlerts(
      IEnumerable<DataRow> rows,
      IEnumerable<CustomProperty> customProperties)
    {
      List<ActiveAlert> activeAlerts = new List<ActiveAlert>();
      foreach (DataRow row in rows)
      {
        ActiveAlert activeAlert = new ActiveAlert();
        activeAlert.Id = Convert.ToInt32(row["ActiveAlertID"]);
        activeAlert.AlertDefId = Convert.ToString(row["AlertDefID"]);
        activeAlert.AlertType = (ActiveAlertType) row["ActiveAlertType"];
        activeAlert.Name = Convert.ToString(row["AlertName"]);
        activeAlert.Message = Convert.ToString(row["AlertMessage"]);
        activeAlert.TriggeringObjectCaption = Convert.ToString(row["TriggeringObject"]);
        activeAlert.TriggeringObjectEntityName = Convert.ToString(row["TriggeringObjectEntityName"]);
        activeAlert.TriggeringObjectStatus = Convert.ToInt32(row["TriggeringObjectStatus"]);
        activeAlert.TriggeringObjectDetailsUrl = Convert.ToString(row["TriggeringObjectDetailsUrl"]);
        activeAlert.TriggeringObjectEntityUri = Convert.ToString(row["TriggeringObjectEntityUri"]);
        activeAlert.RelatedNodeCaption = Convert.ToString(row["RelatedNode"]);
        activeAlert.RelatedNodeID = Convert.ToInt32(row["RelatedNodeID"]);
        activeAlert.RelatedNodeEntityUri = Convert.ToString(row["RelatedNodeEntityUri"]);
        activeAlert.RelatedNodeDetailsUrl = Convert.ToString(row["RelatedNodeDetailsUrl"]);
        activeAlert.RelatedNodeStatus = Convert.ToInt32(row["RelatedNodeStatus"]);
        activeAlert.ActiveTime = (TimeSpan) row["ActiveTime"];
        activeAlert.ActiveTimeDisplay = Convert.ToString(row["ActiveTimeDisplay"]);
        activeAlert.TriggerDateTime = Convert.ToDateTime(row["TriggerTime"]);
        activeAlert.LastTriggeredDateTime = Convert.ToDateTime(row["LastTriggeredDateTime"]);
        activeAlert.TriggerCount = Convert.ToInt32(row["TriggerCount"]);
        activeAlert.AcknowledgedBy = Convert.ToString(row["AcknowledgedBy"]);
        activeAlert.AcknowledgedByFullName = Convert.ToString(row["AcknowledgedByFullName"]);
        activeAlert.AcknowledgedDateTime = Convert.ToDateTime(row["AcknowledgeTime"]);
        activeAlert.Notes = Convert.ToString(row["Notes"]);
        activeAlert.NumberOfNotes = Convert.ToInt32(row["NumberOfNotes"]);
        activeAlert.Severity = (ActiveAlertSeverity) row["Severity"];
        activeAlert.ActiveNetObject = Convert.ToString(row["ActiveNetObject"]);
        activeAlert.ObjectType = Convert.ToString(row["ObjectType"]);
        activeAlert.LegacyAlert = Convert.ToBoolean(row["LegacyAlert"]);
        activeAlert.Canned = Convert.ToBoolean(row["Canned"]);
        activeAlert.Category = Convert.ToString(row["Category"]);
        activeAlert.IncidentNumber = Convert.ToString(row["IncidentNumber"]);
        activeAlert.IncidentUrl = Convert.ToString(row["IncidentUrl"]);
        activeAlert.AssignedTo = Convert.ToString(row["Assignee"]);
        activeAlert.SiteID = Convert.ToInt32(row["SiteID"]);
        activeAlert.SiteName = Convert.ToString(row["SiteName"]);
        if (!activeAlert.LegacyAlert)
        {
          activeAlert.CustomProperties = new Dictionary<string, object>();
          foreach (CustomProperty customProperty in customProperties)
          {
            string columnName = string.Format("CP_{0}", (object) customProperty.PropertyName);
            if (row[columnName] != DBNull.Value)
              activeAlert.CustomProperties.Add(customProperty.PropertyName, row[columnName]);
            else
              activeAlert.CustomProperties.Add(customProperty.PropertyName, (object) null);
          }
        }
        activeAlerts.Add(activeAlert);
      }
      return (IEnumerable<ActiveAlert>) activeAlerts;
    }

    internal static DataTable GetFilteredTable(
      PageableActiveAlertRequest pRequest,
      DataTable alertTable,
      ActiveAlertsRequest request = null)
    {
      string alertsFilterCondition = AlertDAL.GetActiveAlertsFilterCondition(pRequest);
      bool flag = ((IEnumerable<string>) alertsFilterCondition.Split(new string[2]
      {
        "OR",
        "AND"
      }, StringSplitOptions.None)).Count<string>() > 100;
      if (!string.IsNullOrEmpty(alertsFilterCondition) && request == null || !flag)
      {
        alertTable.CaseSensitive = true;
        DataRow[] source = alertTable.Select(alertsFilterCondition);
        if (source != null && source.Length != 0)
          alertTable = ((IEnumerable<DataRow>) source).CopyToDataTable<DataRow>();
        else
          alertTable.Rows.Clear();
      }
      else if (request != null)
      {
        alertTable.CaseSensitive = true;
        DataRow[] array = alertTable.AsEnumerable().Where<DataRow>(AlertDAL.GenerateLambdaFilter(request).Compile()).ToArray<DataRow>();
        if (array != null && array.Length != 0)
          alertTable = ((IEnumerable<DataRow>) array).CopyToDataTable<DataRow>();
        else
          alertTable.Rows.Clear();
      }
      return alertTable;
    }

    private static Expression<System.Func<DataRow, bool>> GenerateLambdaFilter(
      ActiveAlertsRequest request)
    {
      Expression<System.Func<DataRow, bool>> filterPredicate = (Expression<System.Func<DataRow, bool>>) null;
      if (request.TriggeringObjectEntityUris != null && ((IEnumerable<string>) request.TriggeringObjectEntityUris).Any<string>())
      {
        foreach (string triggeringObjectEntityUri in request.TriggeringObjectEntityUris)
        {
          // ISSUE: object of a compiler-generated type is created
          // ISSUE: variable of a compiler-generated type
          AlertDAL.\u003C\u003Ec__DisplayClass27_0 cDisplayClass270 = new AlertDAL.\u003C\u003Ec__DisplayClass27_0();
          // ISSUE: reference to a compiler-generated field
          cDisplayClass270.template = triggeringObjectEntityUri;
          ParameterExpression parameterExpression;
          // ISSUE: method reference
          // ISSUE: method reference
          // ISSUE: field reference
          Expression<System.Func<DataRow, bool>> testExpression = Expression.Lambda<System.Func<DataRow, bool>>((Expression) Expression.Call((Expression) Expression.Call(p["TriggeringObjectEntityUri"], (MethodInfo) MethodBase.GetMethodFromHandle(__methodref (object.ToString)), Array.Empty<Expression>()), (MethodInfo) MethodBase.GetMethodFromHandle(__methodref (string.Equals)), (Expression) Expression.Field((Expression) Expression.Constant((object) cDisplayClass270, typeof (AlertDAL.\u003C\u003Ec__DisplayClass27_0)), FieldInfo.GetFieldFromHandle(__fieldref (AlertDAL.\u003C\u003Ec__DisplayClass27_0.template)))), parameterExpression);
          filterPredicate = AlertDAL.GetFilterPredicate(filterPredicate, testExpression);
        }
        foreach (string triggeringObjectEntityUri in request.TriggeringObjectEntityUris)
        {
          // ISSUE: object of a compiler-generated type is created
          // ISSUE: variable of a compiler-generated type
          AlertDAL.\u003C\u003Ec__DisplayClass27_1 cDisplayClass271 = new AlertDAL.\u003C\u003Ec__DisplayClass27_1();
          // ISSUE: reference to a compiler-generated field
          cDisplayClass271.template = string.Format("{0}/", (object) triggeringObjectEntityUri);
          ParameterExpression parameterExpression;
          // ISSUE: method reference
          // ISSUE: method reference
          // ISSUE: field reference
          Expression<System.Func<DataRow, bool>> testExpression = Expression.Lambda<System.Func<DataRow, bool>>((Expression) Expression.Call((Expression) Expression.Call(p["TriggeringObjectEntityUri"], (MethodInfo) MethodBase.GetMethodFromHandle(__methodref (object.ToString)), Array.Empty<Expression>()), (MethodInfo) MethodBase.GetMethodFromHandle(__methodref (string.Contains)), (Expression) Expression.Field((Expression) Expression.Constant((object) cDisplayClass271, typeof (AlertDAL.\u003C\u003Ec__DisplayClass27_1)), FieldInfo.GetFieldFromHandle(__fieldref (AlertDAL.\u003C\u003Ec__DisplayClass27_1.template)))), parameterExpression);
          filterPredicate = AlertDAL.GetFilterPredicate(filterPredicate, testExpression);
        }
        if (((IEnumerable<string>) request.TriggeringObjectEntityNames).Any<string>() && string.Compare(request.TriggeringObjectEntityNames[0], "Orion.Groups", StringComparison.OrdinalIgnoreCase) == 0)
        {
          foreach (int num in request.AlertActiveIdsGlobal)
          {
            // ISSUE: object of a compiler-generated type is created
            // ISSUE: variable of a compiler-generated type
            AlertDAL.\u003C\u003Ec__DisplayClass27_2 cDisplayClass272 = new AlertDAL.\u003C\u003Ec__DisplayClass27_2();
            // ISSUE: reference to a compiler-generated field
            cDisplayClass272.template = num.ToString();
            ParameterExpression parameterExpression;
            // ISSUE: method reference
            // ISSUE: method reference
            // ISSUE: field reference
            Expression<System.Func<DataRow, bool>> testExpression = Expression.Lambda<System.Func<DataRow, bool>>((Expression) Expression.Call((Expression) Expression.Call(p["ActiveAlertID"], (MethodInfo) MethodBase.GetMethodFromHandle(__methodref (object.ToString)), Array.Empty<Expression>()), (MethodInfo) MethodBase.GetMethodFromHandle(__methodref (string.Equals)), (Expression) Expression.Field((Expression) Expression.Constant((object) cDisplayClass272, typeof (AlertDAL.\u003C\u003Ec__DisplayClass27_2)), FieldInfo.GetFieldFromHandle(__fieldref (AlertDAL.\u003C\u003Ec__DisplayClass27_2.template)))), parameterExpression);
            filterPredicate = AlertDAL.GetFilterPredicate(filterPredicate, testExpression);
          }
        }
      }
      else if (request.TriggeringObjectEntityNames != null && ((IEnumerable<string>) request.TriggeringObjectEntityNames).Any<string>())
      {
        foreach (string objectEntityName in request.TriggeringObjectEntityNames)
        {
          // ISSUE: object of a compiler-generated type is created
          // ISSUE: variable of a compiler-generated type
          AlertDAL.\u003C\u003Ec__DisplayClass27_3 cDisplayClass273 = new AlertDAL.\u003C\u003Ec__DisplayClass27_3();
          // ISSUE: reference to a compiler-generated field
          cDisplayClass273.template = objectEntityName;
          ParameterExpression parameterExpression;
          // ISSUE: method reference
          // ISSUE: method reference
          // ISSUE: field reference
          Expression<System.Func<DataRow, bool>> testExpression = Expression.Lambda<System.Func<DataRow, bool>>((Expression) Expression.Call((Expression) Expression.Call(p["TriggeringObjectEntityName"], (MethodInfo) MethodBase.GetMethodFromHandle(__methodref (object.ToString)), Array.Empty<Expression>()), (MethodInfo) MethodBase.GetMethodFromHandle(__methodref (string.Equals)), (Expression) Expression.Field((Expression) Expression.Constant((object) cDisplayClass273, typeof (AlertDAL.\u003C\u003Ec__DisplayClass27_3)), FieldInfo.GetFieldFromHandle(__fieldref (AlertDAL.\u003C\u003Ec__DisplayClass27_3.template)))), parameterExpression);
          filterPredicate = AlertDAL.GetFilterPredicate(filterPredicate, testExpression);
        }
      }
      else if (request.RelatedNodeId > 0 || !string.IsNullOrEmpty(request.RelatedNodeEntityUri))
      {
        if (request.RelatedNodeId > 0)
        {
          // ISSUE: object of a compiler-generated type is created
          // ISSUE: variable of a compiler-generated type
          AlertDAL.\u003C\u003Ec__DisplayClass27_4 cDisplayClass274 = new AlertDAL.\u003C\u003Ec__DisplayClass27_4();
          // ISSUE: reference to a compiler-generated field
          cDisplayClass274.template = request.RelatedNodeId.ToString();
          ParameterExpression parameterExpression;
          // ISSUE: method reference
          // ISSUE: method reference
          // ISSUE: field reference
          Expression<System.Func<DataRow, bool>> testExpression = Expression.Lambda<System.Func<DataRow, bool>>((Expression) Expression.Call((Expression) Expression.Call(p["RelatedNodeID"], (MethodInfo) MethodBase.GetMethodFromHandle(__methodref (object.ToString)), Array.Empty<Expression>()), (MethodInfo) MethodBase.GetMethodFromHandle(__methodref (string.Equals)), (Expression) Expression.Field((Expression) Expression.Constant((object) cDisplayClass274, typeof (AlertDAL.\u003C\u003Ec__DisplayClass27_4)), FieldInfo.GetFieldFromHandle(__fieldref (AlertDAL.\u003C\u003Ec__DisplayClass27_4.template)))), parameterExpression);
          filterPredicate = AlertDAL.GetFilterPredicate(filterPredicate, testExpression);
        }
        IEnumerable<int> alertActiveIdsGlobal = request.AlertActiveIdsGlobal;
        if (!string.IsNullOrEmpty(request.RelatedNodeEntityUri))
        {
          // ISSUE: object of a compiler-generated type is created
          // ISSUE: variable of a compiler-generated type
          AlertDAL.\u003C\u003Ec__DisplayClass27_5 cDisplayClass275 = new AlertDAL.\u003C\u003Ec__DisplayClass27_5();
          foreach (int num in alertActiveIdsGlobal)
          {
            // ISSUE: object of a compiler-generated type is created
            // ISSUE: variable of a compiler-generated type
            AlertDAL.\u003C\u003Ec__DisplayClass27_6 cDisplayClass276 = new AlertDAL.\u003C\u003Ec__DisplayClass27_6();
            // ISSUE: reference to a compiler-generated field
            cDisplayClass276.template = num.ToString();
            ParameterExpression parameterExpression;
            // ISSUE: method reference
            // ISSUE: method reference
            // ISSUE: field reference
            Expression<System.Func<DataRow, bool>> testExpression = Expression.Lambda<System.Func<DataRow, bool>>((Expression) Expression.Call((Expression) Expression.Call(p["ActiveAlertID"], (MethodInfo) MethodBase.GetMethodFromHandle(__methodref (object.ToString)), Array.Empty<Expression>()), (MethodInfo) MethodBase.GetMethodFromHandle(__methodref (string.Equals)), (Expression) Expression.Field((Expression) Expression.Constant((object) cDisplayClass276, typeof (AlertDAL.\u003C\u003Ec__DisplayClass27_6)), FieldInfo.GetFieldFromHandle(__fieldref (AlertDAL.\u003C\u003Ec__DisplayClass27_6.template)))), parameterExpression);
            filterPredicate = AlertDAL.GetFilterPredicate(filterPredicate, testExpression);
          }
          // ISSUE: reference to a compiler-generated field
          cDisplayClass275.relatedNodeEntityUri = request.RelatedNodeEntityUri;
          ParameterExpression parameterExpression1;
          // ISSUE: method reference
          // ISSUE: method reference
          // ISSUE: field reference
          Expression<System.Func<DataRow, bool>> testExpression1 = Expression.Lambda<System.Func<DataRow, bool>>((Expression) Expression.Call((Expression) Expression.Call(p["TriggeringObjectEntityUri"], (MethodInfo) MethodBase.GetMethodFromHandle(__methodref (object.ToString)), Array.Empty<Expression>()), (MethodInfo) MethodBase.GetMethodFromHandle(__methodref (string.Equals)), (Expression) Expression.Field((Expression) Expression.Constant((object) cDisplayClass275, typeof (AlertDAL.\u003C\u003Ec__DisplayClass27_5)), FieldInfo.GetFieldFromHandle(__fieldref (AlertDAL.\u003C\u003Ec__DisplayClass27_5.relatedNodeEntityUri)))), parameterExpression1);
          ParameterExpression parameterExpression2;
          // ISSUE: method reference
          // ISSUE: method reference
          // ISSUE: field reference
          filterPredicate = AlertDAL.GetFilterPredicate(AlertDAL.GetFilterPredicate(filterPredicate, testExpression1), Expression.Lambda<System.Func<DataRow, bool>>((Expression) Expression.Call((Expression) Expression.Call(p["RelatedNodeEntityUri"], (MethodInfo) MethodBase.GetMethodFromHandle(__methodref (object.ToString)), Array.Empty<Expression>()), (MethodInfo) MethodBase.GetMethodFromHandle(__methodref (string.Equals)), (Expression) Expression.Field((Expression) Expression.Constant((object) cDisplayClass275, typeof (AlertDAL.\u003C\u003Ec__DisplayClass27_5)), FieldInfo.GetFieldFromHandle(__fieldref (AlertDAL.\u003C\u003Ec__DisplayClass27_5.relatedNodeEntityUri)))), parameterExpression2));
        }
        foreach (string objectEntityName in request.TriggeringObjectEntityNames)
        {
          // ISSUE: object of a compiler-generated type is created
          // ISSUE: variable of a compiler-generated type
          AlertDAL.\u003C\u003Ec__DisplayClass27_7 cDisplayClass277 = new AlertDAL.\u003C\u003Ec__DisplayClass27_7();
          // ISSUE: reference to a compiler-generated field
          cDisplayClass277.template = objectEntityName;
          ParameterExpression parameterExpression;
          // ISSUE: method reference
          // ISSUE: method reference
          // ISSUE: field reference
          Expression<System.Func<DataRow, bool>> testExpression = Expression.Lambda<System.Func<DataRow, bool>>((Expression) Expression.Call((Expression) Expression.Call(p["TriggeringObjectEntityName"], (MethodInfo) MethodBase.GetMethodFromHandle(__methodref (object.ToString)), Array.Empty<Expression>()), (MethodInfo) MethodBase.GetMethodFromHandle(__methodref (string.Equals)), (Expression) Expression.Field((Expression) Expression.Constant((object) cDisplayClass277, typeof (AlertDAL.\u003C\u003Ec__DisplayClass27_7)), FieldInfo.GetFieldFromHandle(__fieldref (AlertDAL.\u003C\u003Ec__DisplayClass27_7.template)))), parameterExpression);
          filterPredicate = AlertDAL.GetFilterPredicate(filterPredicate, testExpression);
        }
      }
      if (!request.ShowAcknowledgedAlerts)
      {
        ParameterExpression instance;
        // ISSUE: method reference
        // ISSUE: method reference
        // ISSUE: method reference
        // ISSUE: field reference
        InvocationExpression right = Expression.Invoke((Expression) Expression.Lambda<System.Func<DataRow, bool>>((Expression) Expression.OrElse((Expression) Expression.Call((Expression) Expression.Call(p["AcknowledgedBy"], (MethodInfo) MethodBase.GetMethodFromHandle(__methodref (object.ToString)), Array.Empty<Expression>()), (MethodInfo) MethodBase.GetMethodFromHandle(__methodref (string.Equals)), (Expression) Expression.Constant((object) "", typeof (string))), (Expression) Expression.Equal((Expression) Expression.Call((Expression) instance, (MethodInfo) MethodBase.GetMethodFromHandle(__methodref (DataRow.get_Item)), (Expression) Expression.Constant((object) "AcknowledgedBy", typeof (string))), (Expression) Expression.Field((Expression) null, FieldInfo.GetFieldFromHandle(__fieldref (DBNull.Value))))), instance), filterPredicate.Parameters.Cast<Expression>());
        filterPredicate = Expression.Lambda<System.Func<DataRow, bool>>((Expression) Expression.And(filterPredicate.Body, (Expression) right), (IEnumerable<ParameterExpression>) filterPredicate.Parameters);
      }
      return filterPredicate;
    }

    private static Expression<System.Func<DataRow, bool>> GetFilterPredicate(
      Expression<System.Func<DataRow, bool>> filterPredicate,
      Expression<System.Func<DataRow, bool>> testExpression)
    {
      if (filterPredicate == null)
        filterPredicate = testExpression;
      InvocationExpression right = Expression.Invoke((Expression) testExpression, filterPredicate.Parameters.Cast<Expression>());
      filterPredicate = Expression.Lambda<System.Func<DataRow, bool>>((Expression) Expression.Or(filterPredicate.Body, (Expression) right), (IEnumerable<ParameterExpression>) filterPredicate.Parameters);
      return filterPredicate;
    }

    private static string GetActiveAlertsFilterCondition(PageableActiveAlertRequest request)
    {
      string empty = string.Empty;
      return string.IsNullOrEmpty(request.FilterStatement) ? AlertDAL.GetFilterCondition(request.FilterByPropertyName, request.FilterByPropertyValue, request.FilterByPropertyType) : request.FilterStatement;
    }

    private static string GetFilterCondition(
      string FilterByPropertyName,
      string FilterByPropertyValue,
      string FilterByPropertyType)
    {
      string empty = string.Empty;
      if (string.IsNullOrEmpty(FilterByPropertyName) || FilterByPropertyValue.Equals("[All]", StringComparison.OrdinalIgnoreCase))
        return empty;
      string str1 = "(" + FilterByPropertyName;
      string str2;
      if (string.IsNullOrEmpty(FilterByPropertyValue) && FilterByPropertyType == "System.String")
        str2 = str1 + " IS NULL OR " + FilterByPropertyName + " = ''";
      else if (string.IsNullOrEmpty(FilterByPropertyValue))
      {
        str2 = str1 + " IS NULL";
      }
      else
      {
        switch (FilterByPropertyType)
        {
          case "System.String":
            str2 = str1 + "='" + FilterByPropertyValue.Replace("'", "''") + "'";
            break;
          case "System.DateTime":
            DateTime result = DateTime.MinValue;
            str2 = !DateTime.TryParse(FilterByPropertyValue, (IFormatProvider) Thread.CurrentThread.CurrentUICulture, DateTimeStyles.None, out result) ? (!DateTime.TryParseExact(FilterByPropertyValue, "MM/dd/yyyy h:mm tt", (IFormatProvider) CultureInfo.InvariantCulture, DateTimeStyles.None, out result) ? str1 + string.Format("='{0}'", (object) DateTime.MinValue.ToString("yyyy-MM-ddTHH:mm:ss")) : str1 + string.Format("='{0}'", (object) result.ToString("yyyy-MM-ddTHH:mm:ss"))) : str1 + string.Format("='{0}'", (object) result.ToString("yyyy-MM-ddTHH:mm:ss"));
            break;
          case "System.Single":
            str2 = str1 + "=" + FilterByPropertyValue.Replace(",", ".");
            break;
          default:
            str2 = str1 + string.Format("={0}", (object) FilterByPropertyValue);
            break;
        }
      }
      return str2 + ")";
    }

    private static string GetActiveAlertsSearchCondition(
      string filterValue,
      IEnumerable<CustomProperty> customProperties)
    {
      string str = AlertDAL.EscapeLikeValue(filterValue);
      string alertsSearchCondition = string.Empty + "AlertName LIKE '" + str + "'" + " OR AlertMessage LIKE '" + str + "'" + " OR ObjectTriggeredThisAlertDisplayName LIKE '" + str + "'" + " OR ActiveTimeDisplay LIKE '" + str + "'" + " OR TriggerTimeDisplay LIKE '" + str + "'" + " OR AcknowledgedBy LIKE '" + str + "'" + " OR SeverityText LIKE '" + str + "'" + " OR AcknowledgeTimeDisplay LIKE '" + str + "'";
      foreach (CustomProperty customProperty in customProperties)
        alertsSearchCondition = !(customProperty.PropertyType != typeof (bool)) || !(customProperty.PropertyType != typeof (float)) || !(customProperty.PropertyType != typeof (int)) || !(customProperty.PropertyType != typeof (DateTime)) ? alertsSearchCondition + string.Format(" OR CP_{0}_Display LIKE '{1}'", (object) customProperty.PropertyName, (object) str) : alertsSearchCondition + string.Format(" OR CP_{0} LIKE '{1}'", (object) customProperty.PropertyName, (object) str);
      return alertsSearchCondition;
    }

    private static IEnumerable<DataRow> GetFilteredAlertRows(
      PageableActiveAlertRequest request,
      DataTable alertTable,
      IEnumerable<CustomProperty> customProperties)
    {
      string strCondition = !string.IsNullOrWhiteSpace(request.SearchValue) ? AlertDAL.GetActiveAlertsSearchCondition(request.SearchValue, customProperties) : string.Empty;
      string filterCondition = AlertDAL.GetFilterCondition(request.SecondaryFilters, strCondition, request.SecondaryFilterOperator);
      return (IEnumerable<DataRow>) alertTable.Select(filterCondition);
    }

    private static IEnumerable<DataRow> GetSortedAlerts(
      PageableActiveAlertRequest request,
      IEnumerable<DataRow> alertRows)
    {
      DataRow[] sortedAlerts = new DataRow[0];
      if (!alertRows.Any<DataRow>())
        return (IEnumerable<DataRow>) sortedAlerts;
      string sortColumnName = string.Empty;
      SortOrder sortOrder = SortOrder.Ascending;
      if (request.OrderByClause.EndsWith("ASC", StringComparison.OrdinalIgnoreCase))
        sortColumnName = request.OrderByClause.Substring(0, request.OrderByClause.Length - 3).Trim().TrimStart('[').TrimEnd(']');
      else if (request.OrderByClause.EndsWith("DESC", StringComparison.OrdinalIgnoreCase))
      {
        sortColumnName = request.OrderByClause.Substring(0, request.OrderByClause.Length - 4).Trim().TrimStart('[').TrimEnd(']');
        sortOrder = SortOrder.Descending;
      }
      else if (!string.IsNullOrEmpty(request.OrderByClause))
        sortColumnName = request.OrderByClause.TrimStart('[').TrimEnd(']');
      return string.IsNullOrEmpty(request.OrderByClause) ? (IEnumerable<DataRow>) alertRows.ToArray<DataRow>() : (IEnumerable<DataRow>) AlertDAL.GetSortedAlerts(alertRows, sortColumnName, sortOrder).ToArray<DataRow>();
    }

    private static IEnumerable<DataRow> GetSortedAlerts(
      IEnumerable<DataRow> rows,
      string sortColumnName,
      SortOrder sortOrder)
    {
      if (!rows.Any<DataRow>())
        return (IEnumerable<DataRow>) new DataRow[0];
      if (!rows.ElementAt<DataRow>(0).Table.Columns.Contains(sortColumnName))
      {
        AlertDAL.Log.WarnFormat("Unable to sort by column '{0}', because column doesn't belong to the table. Alert grid will not be sorted. If it is custom property column please make sure, that wasn't deleted.", (object) sortColumnName);
        return rows;
      }
      Type type = rows.First<DataRow>()[sortColumnName].GetType();
      if (type == typeof (DateTime))
      {
        System.Func<DataRow, DateTime> keySelector = (System.Func<DataRow, DateTime>) (row =>
        {
          if (row[sortColumnName] == DBNull.Value)
            return new DateTime();
          DateTime? nullable = row[sortColumnName] as DateTime?;
          return !nullable.HasValue ? new DateTime() : nullable.Value;
        });
        return sortOrder != SortOrder.Ascending ? (IEnumerable<DataRow>) rows.OrderByDescending<DataRow, DateTime>(keySelector).ToArray<DataRow>() : (IEnumerable<DataRow>) rows.OrderBy<DataRow, DateTime>(keySelector).ToArray<DataRow>();
      }
      if (type == typeof (TimeSpan))
      {
        System.Func<DataRow, TimeSpan> keySelector = (System.Func<DataRow, TimeSpan>) (row =>
        {
          if (row[sortColumnName] == DBNull.Value)
            return TimeSpan.FromSeconds(0.0);
          TimeSpan? nullable = row[sortColumnName] as TimeSpan?;
          return !nullable.HasValue ? TimeSpan.FromSeconds(0.0) : nullable.Value;
        });
        return sortOrder != SortOrder.Ascending ? (IEnumerable<DataRow>) rows.OrderByDescending<DataRow, TimeSpan>(keySelector).ToArray<DataRow>() : (IEnumerable<DataRow>) rows.OrderBy<DataRow, TimeSpan>(keySelector).ToArray<DataRow>();
      }
      return sortOrder != SortOrder.Ascending ? (IEnumerable<DataRow>) rows.OrderByDescending<DataRow, string>((System.Func<DataRow, string>) (row => row[sortColumnName] == DBNull.Value ? string.Empty : Convert.ToString(row[sortColumnName])), (IComparer<string>) new NaturalStringComparer()).ToArray<DataRow>() : (IEnumerable<DataRow>) rows.OrderBy<DataRow, string>((System.Func<DataRow, string>) (row => row[sortColumnName] == DBNull.Value ? string.Empty : Convert.ToString(row[sortColumnName])), (IComparer<string>) new NaturalStringComparer()).ToArray<DataRow>();
    }

    private static ActiveAlert SortableAlertDataRowToActiveAlertObject(
      DataRow rAlert,
      Func<string, ActiveAlertType, string> getSwisEntityName,
      DateTime currentDateTime,
      List<string> nodeStatusIds,
      List<string> interfaceStatusIds,
      List<string> containerStatusIds,
      List<string> acknowledgedBy)
    {
      ActiveAlert activeAlertObject = new ActiveAlert()
      {
        AlertDefId = rAlert["AlertID"] != DBNull.Value ? Convert.ToString(rAlert["AlertID"]) : string.Empty
      };
      activeAlertObject.AlertType = !AlertDAL.TryStrToGuid(activeAlertObject.AlertDefId, out Guid _) ? (ActiveAlertType) 1 : (ActiveAlertType) 0;
      activeAlertObject.Name = rAlert["AlertName"] != DBNull.Value ? Convert.ToString(rAlert["AlertName"]) : string.Empty;
      activeAlertObject.TriggerDateTime = rAlert["AlertTime"] != DBNull.Value ? Convert.ToDateTime(rAlert["AlertTime"]) : DateTime.MinValue;
      activeAlertObject.Message = rAlert["EventMessage"] != DBNull.Value ? Convert.ToString(rAlert["EventMessage"]) : string.Empty;
      activeAlertObject.TriggeringObjectCaption = rAlert["ObjectName"] != DBNull.Value ? Convert.ToString(rAlert["ObjectName"]) : string.Empty;
      activeAlertObject.ActiveNetObject = rAlert["ActiveNetObject"] != DBNull.Value ? Convert.ToString(rAlert["ActiveNetObject"]) : "0";
      string str1 = rAlert["ObjectType"] != DBNull.Value ? Convert.ToString(rAlert["ObjectType"]) : string.Empty;
      string strA = getSwisEntityName(str1, activeAlertObject.AlertType);
      activeAlertObject.TriggeringObjectEntityName = strA;
      activeAlertObject.RelatedNodeCaption = rAlert["Sysname"] != DBNull.Value ? Convert.ToString(rAlert["Sysname"]) : string.Empty;
      activeAlertObject.RelatedNodeID = rAlert["NodeID"] != DBNull.Value ? Convert.ToInt32(rAlert["NodeID"]) : 0;
      if (activeAlertObject.RelatedNodeID > 0 && !nodeStatusIds.Contains(activeAlertObject.RelatedNodeID.ToString()))
        nodeStatusIds.Add(activeAlertObject.RelatedNodeID.ToString());
      if (string.Compare(strA, "Orion.Nodes", StringComparison.OrdinalIgnoreCase) == 0)
      {
        if (!nodeStatusIds.Contains(activeAlertObject.ActiveNetObject))
          nodeStatusIds.Add(activeAlertObject.ActiveNetObject);
        int result = 0;
        if (int.TryParse(activeAlertObject.ActiveNetObject, out result))
        {
          activeAlertObject.RelatedNodeCaption = activeAlertObject.TriggeringObjectCaption;
          activeAlertObject.RelatedNodeID = result;
        }
      }
      else if (string.Compare(strA, "Orion.NPM.Interfaces", StringComparison.OrdinalIgnoreCase) == 0)
        interfaceStatusIds.Add(activeAlertObject.ActiveNetObject);
      else if (string.Compare(strA, "Orion.Groups", StringComparison.OrdinalIgnoreCase) == 0)
        containerStatusIds.Add(activeAlertObject.ActiveNetObject);
      string str2 = rAlert["NetObjectPrefix"] != DBNull.Value ? Convert.ToString(rAlert["NetObjectPrefix"]) : string.Empty;
      activeAlertObject.TriggeringObjectDetailsUrl = string.IsNullOrEmpty(str2) ? string.Empty : string.Format("/Orion/View.aspx?NetObject={0}:{1}", (object) str2, (object) activeAlertObject.ActiveNetObject);
      activeAlertObject.ActiveTime = currentDateTime - activeAlertObject.TriggerDateTime;
      activeAlertObject.ActiveTimeDisplay = new ActiveAlertDAL().ActiveTimeToDisplayFormat(activeAlertObject.ActiveTime);
      activeAlertObject.RelatedNodeDetailsUrl = string.Format("/Orion/View.aspx?NetObject=N:{0}", (object) activeAlertObject.RelatedNodeID);
      activeAlertObject.AcknowledgedBy = rAlert["AcknowledgedBy"] != DBNull.Value ? Convert.ToString(rAlert["AcknowledgedBy"]) : string.Empty;
      acknowledgedBy.Add(activeAlertObject.AcknowledgedBy);
      activeAlertObject.AcknowledgedDateTime = rAlert["AcknowledgedTime"] != DBNull.Value ? DateTime.SpecifyKind(Convert.ToDateTime(rAlert["AcknowledgedTime"]), DateTimeKind.Local) : DateTime.MinValue;
      string str3 = string.Format("{0} - ", (object) activeAlertObject.RelatedNodeCaption);
      if (activeAlertObject.TriggeringObjectCaption.StartsWith(str3))
        activeAlertObject.TriggeringObjectCaption = activeAlertObject.TriggeringObjectCaption.Substring(str3.Length, activeAlertObject.TriggeringObjectCaption.Length - str3.Length);
      activeAlertObject.ObjectType = str1;
      activeAlertObject.Severity = (ActiveAlertSeverity) 1;
      activeAlertObject.LegacyAlert = true;
      return activeAlertObject;
    }

    internal static string EscapeLikeValue(string value)
    {
      if (!value.StartsWith("%") || !value.EndsWith("%") || value.Length < 2)
        return AlertDAL.EscapeLikeValueInternal(value);
      value = value.Substring(1, value.Length - 2);
      return string.Format("%{0}%", (object) AlertDAL.EscapeLikeValueInternal(value));
    }

    private static string EscapeLikeValueInternal(string value)
    {
      StringBuilder stringBuilder = new StringBuilder(value.Length);
      for (int index = 0; index < value.Length; ++index)
      {
        char ch = value[index];
        switch (ch)
        {
          case '%':
          case '*':
          case '[':
          case ']':
            stringBuilder.Append("[").Append(ch).Append("]");
            break;
          case '\'':
            stringBuilder.Append("''");
            break;
          default:
            stringBuilder.Append(ch);
            break;
        }
      }
      return stringBuilder.ToString();
    }

    [Obsolete("Method does not return V2 alerts.")]
    public static ActiveAlert GetActiveAlert(
      string activeAlertDefId,
      string activeNetObject,
      string objectType,
      IEnumerable<int> limitationIDs)
    {
      DataRow[] dataRowArray1 = AlertDAL.GetSortableAlertTable(string.Empty, string.Empty, activeAlertDefId, "AlertTime DESC", int.MaxValue, true, limitationIDs.ToList<int>(), true).Select(string.Format("ActiveNetObject='{0}' AND ObjectType='{1}'", (object) activeNetObject, (object) objectType));
      DataTable tblNetObjectTypes = AlertDAL.GetAvailableObjectTypes();
      Func<string, ActiveAlertType, string> getSwisEntityName = (Func<string, ActiveAlertType, string>) ((objectType2, alertType) =>
      {
        DataRow[] dataRowArray2 = alertType != null ? tblNetObjectTypes.Select("Prefix='" + objectType2 + "'") : tblNetObjectTypes.Select("Name='" + objectType2 + "'");
        return dataRowArray2.Length != 0 && dataRowArray2[0]["EntityType"] != DBNull.Value ? Convert.ToString(dataRowArray2[0]["EntityType"]) : string.Empty;
      });
      if (dataRowArray1.Length != 0)
      {
        List<string> stringList1 = new List<string>();
        List<string> stringList2 = new List<string>();
        List<string> stringList3 = new List<string>();
        List<string> stringList4 = new List<string>();
        ActiveAlert activeAlertObject = AlertDAL.SortableAlertDataRowToActiveAlertObject(dataRowArray1[0], getSwisEntityName, DateTime.Now, stringList1, stringList2, stringList3, stringList4);
        Dictionary<string, int> statusesForSwisEntities1 = AlertDAL.GetStatusesForSwisEntities("Orion.Nodes", "NodeID", (IEnumerable<string>) stringList1);
        Dictionary<string, int> statusesForSwisEntities2 = AlertDAL.GetStatusesForSwisEntities("Orion.NPM.Interfaces", "InterfaceID", (IEnumerable<string>) stringList2);
        Dictionary<string, int> statusesForSwisEntities3 = AlertDAL.GetStatusesForSwisEntities("Orion.Groups", "ContainerID", (IEnumerable<string>) stringList3);
        Dictionary<string, string> fullUserNames = AlertDAL.GetFullUserNames((IEnumerable<string>) stringList4);
        string strA = getSwisEntityName(activeAlertObject.ObjectType, activeAlertObject.AlertType);
        if (string.Compare(strA, "Orion.Nodes", StringComparison.OrdinalIgnoreCase) == 0 && statusesForSwisEntities1.ContainsKey(activeAlertObject.ActiveNetObject))
          activeAlertObject.TriggeringObjectStatus = statusesForSwisEntities1[activeAlertObject.ActiveNetObject];
        else if (string.Compare(strA, "Orion.NPM.Interfaces", StringComparison.OrdinalIgnoreCase) == 0 && statusesForSwisEntities2.ContainsKey(activeAlertObject.ActiveNetObject))
          activeAlertObject.TriggeringObjectStatus = statusesForSwisEntities2[activeAlertObject.ActiveNetObject];
        else if (string.Compare(strA, "Orion.Groups", StringComparison.OrdinalIgnoreCase) == 0 && statusesForSwisEntities3.ContainsKey(activeAlertObject.ActiveNetObject))
          activeAlertObject.TriggeringObjectStatus = statusesForSwisEntities3[activeAlertObject.ActiveNetObject];
        Dictionary<string, int> dictionary1 = statusesForSwisEntities1;
        int relatedNodeId = activeAlertObject.RelatedNodeID;
        string key1 = relatedNodeId.ToString();
        if (dictionary1.ContainsKey(key1))
        {
          ActiveAlert activeAlert = activeAlertObject;
          Dictionary<string, int> dictionary2 = statusesForSwisEntities1;
          relatedNodeId = activeAlertObject.RelatedNodeID;
          string key2 = relatedNodeId.ToString();
          int num = dictionary2[key2];
          activeAlert.RelatedNodeStatus = num;
        }
        if (fullUserNames.ContainsKey(activeAlertObject.AcknowledgedBy))
          activeAlertObject.AcknowledgedByFullName = fullUserNames[activeAlertObject.AcknowledgedBy];
        activeAlertObject.Notes = AlertDAL.GetAlertNote(activeAlertObject.AlertDefId, activeAlertObject.ActiveNetObject, activeAlertObject.ObjectType);
        activeAlertObject.Status = (ActiveAlertStatus) 1;
        activeAlertObject.EscalationLevel = 1;
        activeAlertObject.LegacyAlert = true;
        return activeAlertObject;
      }
      string str = "SELECT Name FROM Orion.AlertDefinitions WHERE AlertDefID=@alertDefId AND ObjectType=@objectType";
      DataTable dataTable = InformationServiceProxyExtensions.QueryWithAppendedErrors((IInformationServiceProxy) AlertDAL.SwisProxy, str, new Dictionary<string, object>()
      {
        {
          "alertDefId",
          (object) activeAlertDefId
        },
        {
          nameof (objectType),
          (object) objectType
        }
      });
      if (dataTable.Rows.Count <= 0)
        return (ActiveAlert) null;
      ActiveAlert activeAlert1 = new ActiveAlert()
      {
        LegacyAlert = false,
        Status = (ActiveAlertStatus) 0,
        Name = dataTable.Rows[0]["Name"] != DBNull.Value ? Convert.ToString(dataTable.Rows[0]["Name"]) : string.Empty,
        Severity = (ActiveAlertSeverity) 1,
        ObjectType = objectType
      };
      activeAlert1.TriggeringObjectEntityName = getSwisEntityName(objectType, activeAlert1.AlertType);
      activeAlert1.ActiveNetObject = activeNetObject;
      string strA1 = getSwisEntityName(activeAlert1.ObjectType, activeAlert1.AlertType);
      if (string.Compare(strA1, "Orion.Nodes", StringComparison.OrdinalIgnoreCase) == 0)
      {
        Dictionary<string, int> statusesForSwisEntities = AlertDAL.GetStatusesForSwisEntities("Orion.Nodes", "NodeID", (IEnumerable<string>) new List<string>()
        {
          activeNetObject
        });
        if (statusesForSwisEntities.ContainsKey(activeAlert1.ActiveNetObject))
          activeAlert1.TriggeringObjectStatus = statusesForSwisEntities[activeAlert1.ActiveNetObject];
      }
      else if (string.Compare(strA1, "Orion.NPM.Interfaces", StringComparison.OrdinalIgnoreCase) == 0)
      {
        Dictionary<string, int> statusesForSwisEntities = AlertDAL.GetStatusesForSwisEntities("Orion.NPM.Interfaces", "InterfaceID", (IEnumerable<string>) new List<string>()
        {
          activeNetObject
        });
        if (statusesForSwisEntities.ContainsKey(activeAlert1.ActiveNetObject))
          activeAlert1.TriggeringObjectStatus = statusesForSwisEntities[activeAlert1.ActiveNetObject];
      }
      else if (string.Compare(strA1, "Orion.Groups", StringComparison.OrdinalIgnoreCase) == 0)
      {
        Dictionary<string, int> statusesForSwisEntities = AlertDAL.GetStatusesForSwisEntities("Orion.Groups", "ContainerID", (IEnumerable<string>) new List<string>()
        {
          activeNetObject
        });
        if (statusesForSwisEntities.ContainsKey(activeAlert1.ActiveNetObject))
          activeAlert1.TriggeringObjectStatus = statusesForSwisEntities[activeAlert1.ActiveNetObject];
      }
      return activeAlert1;
    }

    private static string GetFilterCondition(
      IEnumerable<ActiveAlertFilter> filters,
      string strCondition,
      FilterOperator filterOperator)
    {
      if (filters.Any<ActiveAlertFilter>())
      {
        string str1 = "AND";
        if (filterOperator == 1)
          str1 = "OR";
        if (!string.IsNullOrEmpty(strCondition))
          strCondition = string.Format("({0}) {1} ", (object) strCondition, (object) str1);
        strCondition = filterOperator != null ? strCondition + " ( 1<>1" : strCondition + " ( 1=1 ";
        foreach (ActiveAlertFilter filter in filters)
        {
          if (string.Compare(filter.FieldDataType, "datetime", true) == 0)
            strCondition = AlertDAL.GetDateTimeFilterCondition(strCondition, filter);
          else if (string.Compare(filter.FieldDataType, "string", true) == 0)
          {
            if (!string.IsNullOrEmpty(filter.Value))
            {
              filter.Value = AlertDAL.EscapeLikeValue(filter.Value);
              strCondition = filter.Comparison != null ? (filter.Comparison != 4 ? strCondition + string.Format(" {2} {0} <> '{1}'", (object) filter.FieldName, (object) filter.Value, (object) str1) : strCondition + string.Format(" {2} {0} LIKE '%{1}%'", (object) filter.FieldName, (object) filter.Value, (object) str1)) : strCondition + string.Format(" {2} {0} = '{1}'", (object) filter.FieldName, (object) filter.Value, (object) str1);
            }
            else
              strCondition = filter.Comparison == 3 ? strCondition + string.Format(" {1} ({0} <> '' OR {0} IS NOT NULL)", (object) filter.FieldName, (object) str1) : strCondition + string.Format(" {1} ({0} = '' OR {0} IS NULL)", (object) filter.FieldName, (object) str1);
          }
          else if (string.Compare(filter.FieldDataType, "list$system.string", true) == 0)
          {
            if (!string.IsNullOrEmpty(filter.Value))
            {
              if (filter.Comparison != 3)
              {
                StringBuilder stringBuilder = new StringBuilder();
                string str2 = filter.Value;
                string[] separator = new string[1]{ "$#" };
                foreach (string str3 in str2.Split(separator, StringSplitOptions.None))
                  stringBuilder.Append("'" + str3 + "',");
                if (stringBuilder.Length > 0)
                  stringBuilder = stringBuilder.Remove(stringBuilder.Length - 1, 1);
                strCondition += string.Format(" {2} {0} in ({1})", (object) filter.FieldName, (object) stringBuilder, (object) str1);
              }
              else
                strCondition += string.Format(" {2} {0} <> '{1}'", (object) filter.FieldName, (object) filter.Value, (object) str1);
            }
            else
              strCondition = filter.Comparison == 3 ? strCondition + string.Format(" {1} ({0} <> '' OR {0} IS NOT NULL)", (object) filter.FieldName, (object) str1) : strCondition + string.Format(" {1} ({0} = '' OR {0} IS NULL)", (object) filter.FieldName, (object) str1);
          }
          else if (filter.FieldDataType.StartsWith("bool"))
            strCondition = filter.Comparison != null ? strCondition + string.Format(" {2} {0} <> {1}", (object) filter.FieldName, (object) Convert.ToInt32(filter.Value), (object) str1) : strCondition + string.Format(" {2} {0} = {1}", (object) filter.FieldName, (object) Convert.ToInt32(filter.Value), (object) str1);
          else if (string.Compare(filter.FieldDataType, "numeric", true) == 0)
          {
            if (filter.Comparison == 1)
              strCondition += string.Format(" {2} {0} < {1}", (object) filter.FieldName, (object) Convert.ToDouble(filter.Value), (object) str1);
            else if (filter.Comparison == 2)
              strCondition += string.Format(" {2} {0} > {1}", (object) filter.FieldName, (object) Convert.ToDouble(filter.Value), (object) str1);
            else if (filter.Comparison == null)
              strCondition = !string.IsNullOrEmpty(filter.Value) ? strCondition + string.Format(" {2} {0} = {1}", (object) filter.FieldName, (object) Convert.ToDouble(filter.Value), (object) str1) : strCondition + string.Format(" {1} {0} IS Null", (object) filter.FieldName, (object) str1);
          }
        }
        strCondition += ")";
      }
      return strCondition;
    }

    private static string GetDateTimeFilterCondition(string strCondition, ActiveAlertFilter filter)
    {
      string str1 = DateTime.MinValue.ToString("yyyy-MM-ddTHH:mm:ss");
      string str2 = new DateTime(1899, 12, 30).ToString("yyyy-MM-ddTHH:mm:ss");
      if (string.IsNullOrEmpty(filter.Value))
      {
        strCondition += string.Format(" AND ({0} IS NULL OR {0} = '{1}' OR {0} = '{2}') ", (object) filter.FieldName, (object) str1, (object) str2);
      }
      else
      {
        DateTime result;
        if (DateTime.TryParseExact(filter.Value, "MM/dd/yyyy h:mm tt", (IFormatProvider) CultureInfo.InvariantCulture, DateTimeStyles.None, out result))
        {
          string str3 = result.ToString("yyyy-MM-ddTHH:mm:ss");
          if (filter.Comparison == null)
            strCondition += string.Format(" AND {0} = '{1}'", (object) filter.FieldName, (object) str3);
          else if (filter.Comparison == 1)
            strCondition += string.Format(" AND {0} < '{1}' AND {0} > '{2}' AND {0} > '{3}'", (object) filter.FieldName, (object) str3, (object) str1, (object) str2);
          else if (filter.Comparison == 2)
            strCondition += string.Format(" AND {0} > '{1}'", (object) filter.FieldName, (object) str3);
          else if (filter.Comparison == 3)
            strCondition += string.Format(" AND {0} <> '{1}'", (object) filter.FieldName, (object) str3);
        }
      }
      return strCondition;
    }

    [Obsolete("Method does not return V2 alerts.")]
    public static string GetAdvAlertSwql(List<int> limitationIDs)
    {
      return AlertDAL.GetAdvAlertSwql(string.Empty, string.Empty, limitationIDs, false, false);
    }

    [Obsolete("Method does not return V2 alerts.")]
    public static string GetAdvAlertSwql(
      string whereClause,
      string advAlertsLabel,
      List<int> limitationIDs,
      bool includeDefaultFields,
      bool includeToolsetFields)
    {
      return AlertDAL.GetAdvAlertSwql(whereClause, string.Empty, string.Empty, advAlertsLabel, limitationIDs, includeDefaultFields, includeToolsetFields);
    }

    [Obsolete("Method does not return V2 alerts.")]
    public static string GetAdvAlertSwql(
      string whereClause,
      string netObjectWhereClause,
      string netObject,
      string advAlertsLabel,
      List<int> limitationIDs,
      bool includeDefaultFields,
      bool includeToolsetFields)
    {
      StringBuilder stringBuilder = new StringBuilder();
      string str1 = string.Empty;
      List<string> activeNetObjectTypes = AlertDAL.GetActiveNetObjectTypes(whereClause);
      List<NetObjectType> netObjectTypeList;
      if (activeNetObjectTypes != null && activeNetObjectTypes.Count != 0)
      {
        netObjectTypeList = ModuleAlertsMap.NetObjectTypes.Items;
      }
      else
      {
        netObjectTypeList = new List<NetObjectType>();
        netObjectTypeList.Add(ModuleAlertsMap.NetObjectTypes.Items[0]);
      }
      string format = "{0}";
      foreach (NetObjectType netObjectType in netObjectTypeList)
      {
        NetObjectType noType = netObjectType;
        if (activeNetObjectTypes.Count <= 0 || activeNetObjectTypes.Any<string>((System.Func<string, bool>) (netObj => string.Equals(netObj, noType.Name, StringComparison.OrdinalIgnoreCase))))
        {
          string triggeredAlertsQuery = ModuleAlertsMap.GetTriggeredAlertsQuery(noType.Name, includeDefaultFields, includeToolsetFields);
          AlertDAL.Log.DebugFormat("Query Template for {0}  : {1} ", (object) noType.Name, (object) triggeredAlertsQuery);
          if (!string.IsNullOrEmpty(triggeredAlertsQuery))
          {
            string netObjectCondition = ModuleAlertsMap.GetParentNetObjectCondition(noType.Name, netObject);
            string str2 = string.Format("{0} {1}", (object) whereClause, string.IsNullOrEmpty(netObjectCondition) ? (object) netObjectWhereClause : (object) string.Format(" AND {0}", (object) netObjectCondition));
            stringBuilder.AppendLine(str1);
            string str3 = string.Format(triggeredAlertsQuery, (object) str2, (object) advAlertsLabel);
            stringBuilder.AppendFormat(format, (object) str3);
            format = "({0})";
            str1 = " UNION ";
          }
        }
      }
      return stringBuilder.ToString();
    }

    [Obsolete("Old alerting will be removed")]
    private static List<string> GetActiveNetObjectTypes(string whereClause)
    {
      List<string> activeNetObjectTypes = new List<string>();
      string str = string.Empty;
      Match match = AlertDAL._ackRegex.Match(whereClause);
      if (match.Success)
        str = string.Format("AND {0}", (object) match.Value);
      using (SqlCommand textCommand = SqlHelper.GetTextCommand(string.Format("Select DISTINCT AlertStatus.ObjectType from AlertDefinitions WITH(NOLOCK)\r\nINNER JOIN AlertStatus WITH(NOLOCK) ON AlertStatus.AlertDefID = AlertDefinitions.AlertDefID Where (AlertStatus.State=2 OR AlertStatus.State=3) {0}", (object) str)))
      {
        using (IDataReader dataReader = SqlHelper.ExecuteReader(textCommand))
        {
          while (dataReader.Read())
            activeNetObjectTypes.Add(DatabaseFunctions.GetString(dataReader, "ObjectType"));
        }
      }
      return activeNetObjectTypes;
    }

    [Obsolete("Method does not return V2 alerts.")]
    public static DataTable GetPageableAlerts(
      List<int> limitationIDs,
      string period,
      int fromRow,
      int toRow,
      string type,
      string alertId,
      bool showAcknAlerts)
    {
      DataTable pageableAlerts = (DataTable) null;
      List<SqlParameter> sqlParams = new List<SqlParameter>();
      string[] strArray = period.Split('~');
      DateTime localTime1 = DateTime.Parse(strArray[0]).ToLocalTime();
      DateTime localTime2 = DateTime.Parse(strArray[1]).ToLocalTime();
      List<SqlParameter> sqlParameterList1 = sqlParams;
      SqlParameter sqlParameter1 = new SqlParameter("@StartDate", SqlDbType.DateTime);
      sqlParameter1.Value = (object) localTime1;
      sqlParameterList1.Add(sqlParameter1);
      List<SqlParameter> sqlParameterList2 = sqlParams;
      SqlParameter sqlParameter2 = new SqlParameter("@EndDate", SqlDbType.DateTime);
      sqlParameter2.Value = (object) localTime2;
      sqlParameterList2.Add(sqlParameter2);
      string str1 = "IF OBJECT_ID('tempdb..#Nodes') IS NOT NULL\tDROP TABLE #Nodes\r\nSELECT * INTO #Nodes FROM Nodes WHERE 1=1;";
      string str2 = Limitation.LimitSQL(str1, (IEnumerable<int>) limitationIDs);
      int num = !AlertDAL._enableLimitationReplacement ? 0 : (str2.Length / str1.Length > AlertDAL._limitationSqlExaggeration ? 1 : 0);
      string str3 = num != 0 ? str2 : string.Empty;
      string str4 = num != 0 ? str3 : string.Empty;
      string str5 = num != 0 ? "IF OBJECT_ID('tempdb..#Nodes') IS NOT NULL\tDROP TABLE #Nodes" : string.Empty;
      string str6;
      switch (type.ToUpper())
      {
        case "ADVANCED":
          str6 = string.Format("{3}Select * from (\r\nSELECT a.AlertTime, a.AlertName, 'Advanced' AS AlertType, a.ObjectName, a.Acknowledged, a.AlertID, 0 as BAlertID, a.ObjectType, \r\na.ObjectID as ActiveObject, ROW_NUMBER() OVER (ORDER BY a.ObjectName, a.AlertName) AS Row \r\nFROM ( {0} )a Where a.AlertTime >= @StartDate And a.AlertTime <= @EndDate\r\n) t \r\nWHERE Row BETWEEN {1} AND {2} Order By t.ObjectName, t.AlertName\r\n{4}", (object) AlertDAL.GetAdvAlertSwql(AlertDAL.GetWhereClause(alertId, showAcknAlerts, sqlParams), string.Empty, string.Empty, OrionMessagesHelper.GetMessageTypeString((OrionMessageType) 0), limitationIDs, true, true), (object) fromRow, (object) toRow, (object) str4, (object) str5);
          break;
        case "BASIC":
          str6 = string.Format("Select * from (\r\nSELECT a.AlertTime, a.AlertName, 'Basic' AS AlertType, a.ObjectType + '::' + a.ObjectName As ObjectName, \r\na.Acknowledged, a.AlertID, a.AlertID as BAlertID, a.ObjectType, a.ObjectID as ActiveObject, ROW_NUMBER() OVER (ORDER BY a.ObjectName, a.AlertName) AS Row \r\nFROM ( {0} ) a Where a.AlertTime >= @StartDate And a.AlertTime <= @EndDate\r\n) t \r\nWHERE Row BETWEEN {1} AND {2} Order By t.ObjectName, t.AlertName", (object) AlertDAL.GetBasicAlertSwql(string.Empty, string.Empty, alertId, limitationIDs, true, true), (object) fromRow, (object) toRow);
          break;
        default:
          str6 = string.Format("{4}Select * from (\r\nSELECT a.AlertTime, a.AlertName, \r\na.AlertType, Case a.AlertType When 'Advanced' Then a.ObjectName Else a.ObjectType + '::' + a.ObjectName End As ObjectName, a.Acknowledged, a.AlertID, \r\na.BAlertID, a.ObjectType, a.ObjectID as ActiveObject, ROW_NUMBER() OVER (ORDER BY a.ObjectName, a.AlertName) AS Row \r\nFROM ( {0} Union ( {1} ))a Where a.AlertTime >= @StartDate And a.AlertTime <= @EndDate\r\n) t \r\nWHERE Row BETWEEN {2} AND {3} Order By t.ObjectName, t.AlertName\r\n{5}", (object) string.Format("Select *, 0 as BAlertID, 'Advanced' AS AlertType From ({0}) AAT", (object) AlertDAL.GetAdvAlertSwql(AlertDAL.GetWhereClause(alertId, showAcknAlerts, sqlParams), string.Empty, string.Empty, OrionMessagesHelper.GetMessageTypeString((OrionMessageType) 0), limitationIDs, true, true)), (object) string.Format("Select *, BAT.AlertID as BAlertID, 'Basic' AS AlertType From ({0}) BAT", (object) AlertDAL.GetBasicAlertSwql(string.Empty, string.Empty, alertId, limitationIDs, true, true)), (object) fromRow, (object) toRow, (object) str4, (object) str5);
          break;
      }
      using (SqlCommand textCommand = SqlHelper.GetTextCommand(str6))
      {
        textCommand.Parameters.AddRange(sqlParams.ToArray());
        pageableAlerts = SqlHelper.ExecuteDataTable(textCommand);
      }
      pageableAlerts.TableName = "Alerts";
      return pageableAlerts;
    }

    private static string GetWhereClause(
      string alertId,
      bool showAcknAlerts,
      List<SqlParameter> sqlParams)
    {
      StringBuilder stringBuilder = new StringBuilder(" AND (AlertStatus.State=@Triggered) ");
      List<SqlParameter> sqlParameterList1 = sqlParams;
      SqlParameter sqlParameter1 = new SqlParameter("@Triggered", SqlDbType.TinyInt);
      sqlParameter1.Value = (object) 2;
      sqlParameterList1.Add(sqlParameter1);
      if (!showAcknAlerts)
      {
        stringBuilder.Append(" AND AlertStatus.Acknowledged=@Acknowledged ");
        List<SqlParameter> sqlParameterList2 = sqlParams;
        SqlParameter sqlParameter2 = new SqlParameter("@Acknowledged", SqlDbType.TinyInt);
        sqlParameter2.Value = (object) 0;
        sqlParameterList2.Add(sqlParameter2);
      }
      Guid result;
      if (Guid.TryParse(alertId, out result))
      {
        stringBuilder.Append(" AND (AlertStatus.AlertDefID=@AlertDefID) ");
        List<SqlParameter> sqlParameterList3 = sqlParams;
        SqlParameter sqlParameter3 = new SqlParameter("@AlertDefID", SqlDbType.UniqueIdentifier);
        sqlParameter3.Value = (object) result;
        sqlParameterList3.Add(sqlParameter3);
      }
      return stringBuilder.ToString();
    }

    [Obsolete("Don't use this method anymore. Old alerts will be removed.")]
    public static string GetBasicAlertSwql(List<int> limitationIDs)
    {
      return AlertDAL.GetBasicAlertSwql(string.Empty, string.Empty, string.Empty, limitationIDs, false, false);
    }

    [Obsolete("Don't use this method anymore. Old alerts will be removed.")]
    public static string GetBasicAlertSwql(
      string netObject,
      string deviceType,
      string alertId,
      List<int> limitationIDs,
      bool includeDefaultFields,
      bool includeToolsetFields)
    {
      string empty = string.Empty;
      int num = 0;
      if (!string.IsNullOrEmpty(netObject))
      {
        string[] strArray = netObject.Split(':');
        if (strArray.Length == 2)
        {
          empty = strArray[0];
          num = Convert.ToInt32(strArray[1]);
        }
      }
      StringBuilder stringBuilder = new StringBuilder();
      if (num != 0)
      {
        if (empty == "N")
          stringBuilder.AppendFormat(" AND (ActiveAlerts.NodeID={0}) ", (object) num);
        else
          stringBuilder.AppendFormat(" AND (ObjectType='{0}' AND ObjectID={1}) ", (object) empty, (object) num);
      }
      else if (!string.IsNullOrEmpty(deviceType))
        stringBuilder.AppendFormat(" AND (MachineType Like '{0}') ", (object) deviceType);
      int result;
      if (int.TryParse(alertId, out result))
        stringBuilder.AppendFormat(" AND (ActiveAlerts.AlertID={0}) ", (object) result);
      else if (!string.IsNullOrEmpty(alertId))
        stringBuilder.Append(" AND (ActiveAlerts.AlertID=0) ");
      return AlertDAL.GetBasicAlertSwql(stringBuilder.ToString(), string.Empty, limitationIDs, includeDefaultFields, includeToolsetFields);
    }

    [Obsolete("Don't use this method anymore. Old alerts will be removed.")]
    public static string GetBasicAlertSwql(
      string whereClause,
      string basicAlertsLabel,
      List<int> limitationIDs,
      bool includeDefaultFields,
      bool includeToolsetFields)
    {
      bool flag = AlertDAL.IsInterfacesAllowed();
      StringBuilder stringBuilder = new StringBuilder("SELECT ");
      if (includeDefaultFields)
        stringBuilder.AppendFormat("\r\nCAST(ActiveAlerts.AlertID AS NVARCHAR(38)) AS AlertID,\r\nAlerts.AlertName AS AlertName,\r\nActiveAlerts.AlertTime AS AlertTime, \r\nCAST(ActiveAlerts.ObjectID AS NVARCHAR(38)) AS ObjectID, \r\nCASE WHEN ActiveAlerts.ObjectType = 'N' THEN ActiveAlerts.ObjectName ELSE ActiveAlerts.NodeName + '-' + ActiveAlerts.ObjectName END AS ObjectName,\r\nActiveAlerts.ObjectType AS ObjectType,\r\n'0' AS Acknowledged,\r\n'' AS AcknowledgedBy, \r\n'18991230' AS AcknowledgedTime, \r\nCAST(ActiveAlerts.EventMessage AS NVARCHAR(1024)) AS EventMessage,\r\n{0} AS MonitoredProperty, \r\n", !string.IsNullOrEmpty(basicAlertsLabel) ? (object) string.Format("'{0}'", (object) basicAlertsLabel) : (object) "ActiveAlerts.MonitoredProperty");
      if (includeToolsetFields)
      {
        stringBuilder.Append("\r\nNodes.IP_Address AS IP_Address, \r\nNodes.DNS AS DNS, \r\nNodes.[SysName] AS [Sysname], \r\nNodes.[Community] AS [Community], \r\n");
        if (flag)
          stringBuilder.Append("\r\nInterfaces.InterfaceName AS InterfaceName, \r\nInterfaces.InterfaceIndex AS InterfaceIndex,\r\n");
        else
          stringBuilder.Append("\r\nNULL AS InterfaceName, \r\nNULL AS InterfaceIndex,\r\n");
      }
      stringBuilder.Append("\r\nActiveAlerts.CurrentValue AS CurrentValue, \r\nCAST(ActiveAlerts.ObjectID AS NVARCHAR(38)) AS ActiveNetObject, \r\nActiveAlerts.ObjectType AS NetObjectPrefix, \r\nNodes.NodeID AS NodeID\r\nFROM ActiveAlerts\r\nINNER JOIN Nodes WITH(NOLOCK) ON ActiveAlerts.NodeID = Nodes.NodeID\r\nINNER JOIN Alerts WITH(NOLOCK) ON ActiveAlerts.AlertID = Alerts.AlertID ");
      if (includeToolsetFields && flag)
        stringBuilder.Append(" LEFT OUTER JOIN Interfaces WITH(NOLOCK) ON ActiveAlerts.ObjectID = Interfaces.InterfaceID AND ActiveAlerts.ObjectType = 'I' ");
      stringBuilder.AppendLine(" WHERE 1=1 ");
      stringBuilder.Append(whereClause);
      return Limitation.LimitSQL(stringBuilder.ToString(), (IEnumerable<int>) limitationIDs);
    }

    [Obsolete("Don't use this method anymore. Old alerts will be removed.")]
    public static List<SolarWinds.Orion.Core.Common.Models.Node> GetAlertNetObjects(
      List<int> limitationIDs)
    {
      List<SolarWinds.Orion.Core.Common.Models.Node> alertNetObjects = new List<SolarWinds.Orion.Core.Common.Models.Node>();
      string str = string.Format("Select * FROM Nodes WITH(NOLOCK)  \r\n                                    WHERE Nodes.NodeID IN (\r\n\t\t\t\t\t\t\t\t\tSelect DISTINCT NodeID FROM({0} UNION {1}) as t1\r\n\t\t\t\t\t\t\t\t\t) Order By Caption", (object) AlertDAL.GetAdvAlertSwql(limitationIDs), (object) AlertDAL.GetBasicAlertSwql(limitationIDs));
      bool[] flagArray = new bool[2];
      using (SqlCommand textCommand = SqlHelper.GetTextCommand(str))
      {
        using (IDataReader dataReader = SqlHelper.ExecuteReader(textCommand))
        {
          while (dataReader.Read())
            alertNetObjects.Add(NodeDAL.CreateNode(dataReader, flagArray));
        }
      }
      return alertNetObjects;
    }

    [Obsolete("Don't use this method anymore. Old alerts will be removed.")]
    public static Dictionary<int, string> GetNodeData(List<int> limitationIDs)
    {
      return AlertDAL.GetNodeData(limitationIDs, true);
    }

    [Obsolete("Don't use this method anymore. Old alerts will be removed.")]
    public static Dictionary<int, string> GetNodeData(List<int> limitationIDs, bool includeBasic)
    {
      Dictionary<int, string> nodeData = new Dictionary<int, string>();
      string str1 = Limitation.LimitSQL("Select Top 1 NodeID from Nodes", (IEnumerable<int>) limitationIDs);
      int num = !AlertDAL._enableLimitationReplacement ? 0 : (str1.Length / "Select Top 1 NodeID from Nodes".Length > AlertDAL._limitationSqlExaggeration ? 1 : 0);
      string str2 = num != 0 ? "IF OBJECT_ID('tempdb..#Nodes') IS NOT NULL\tDROP TABLE #Nodes" : string.Empty;
      string str3 = num != 0 ? "#Nodes" : "Nodes";
      string str4 = num != 0 ? "IF OBJECT_ID('tempdb..#Nodes') IS NOT NULL\tDROP TABLE #Nodes\r\nSELECT * INTO #Nodes FROM Nodes WHERE 1=1;" : string.Empty;
      using (SqlCommand textCommand = SqlHelper.GetTextCommand(string.Format("{4}\r\nSelect {3}.NodeID, {3}.Caption\r\nFROM {3} \r\nWhere {3}.NodeID IN\r\n(Select DISTINCT NodeID FROM({0}{1}) as t1) \r\nOrder By Caption \r\n{2}", (object) AlertDAL.GetAdvAlertSwql(limitationIDs), includeBasic ? (object) (" UNION " + AlertDAL.GetBasicAlertSwql(limitationIDs)) : (object) string.Empty, (object) str2, (object) str3, (object) str4)))
      {
        using (IDataReader dataReader = SqlHelper.ExecuteReader(textCommand))
        {
          while (dataReader.Read())
            nodeData.Add(DatabaseFunctions.GetInt32(dataReader, "NodeID"), DatabaseFunctions.GetString(dataReader, "Caption"));
        }
      }
      return nodeData;
    }

    [Obsolete("Old alerting will be removed")]
    public static void AcknowledgeAlertsAction(
      List<string> alertKeys,
      string accountID,
      bool fromEmail,
      string acknowledgeNotes)
    {
      AlertDAL.AcknowledgeAlertsAction(alertKeys, accountID, fromEmail ? (AlertAcknowledgeType) 1 : (AlertAcknowledgeType) 0, acknowledgeNotes);
    }

    [Obsolete("Old alerting will be removed")]
    public static void AcknowledgeAlertsAction(
      List<string> alertKeys,
      string accountId,
      AlertAcknowledgeType acknowledgeType,
      string acknowledgeNotes)
    {
      AlertDAL.AcknowledgeAlertsAction(alertKeys, accountId, acknowledgeNotes, AlertsHelper.GetAcknowledgeMethodDisplayString(acknowledgeType));
    }

    [Obsolete("Old alerting will be removed")]
    public static int AcknowledgeAlertsAction(
      List<string> alertKeys,
      string accountID,
      string acknowledgeNotes,
      string method)
    {
      int num = 0;
      foreach (string alertKey in alertKeys)
      {
        string alertId;
        string netObjectId;
        string objectType;
        if (AlertsHelper.TryParseAlertKey(alertKey, ref alertId, ref netObjectId, ref objectType) && AlertDAL.AcknowledgeAlert(alertId, netObjectId, objectType, accountID, acknowledgeNotes, method) == null)
          ++num;
      }
      return num;
    }

    [Obsolete("Old alerting will be removed")]
    public static AlertAcknowledgeResult AcknowledgeAlert(
      string alertId,
      string netObjectId,
      string objectType,
      string accountId,
      string acknowledgeNotes,
      string method)
    {
      string str1 = string.Empty;
      if (!string.IsNullOrEmpty(acknowledgeNotes))
        str1 = ", Notes = CAST(ISNULL(Notes, '') AS NVARCHAR(MAX)) + @Notes";
      using (SqlCommand textCommand = SqlHelper.GetTextCommand(string.Format("\r\nBEGIN TRAN\r\n\r\nDECLARE @acknowleged smallint;\r\nSET @acknowleged = -1;\r\n\r\nSELECT @acknowleged = Acknowledged  FROM [AlertStatus] \r\nWHERE AlertDefID =  @AlertDefID AND ActiveObject = @ActiveObject AND ObjectType LIKE @ObjectType\r\n\r\nIF(@acknowleged = 0)\r\nBEGIN\r\n\tUPDATE AlertStatus SET \r\n                                    Acknowledged = 1, \r\n                                    AcknowledgedBy = @AcknowledgedBy,\r\n                                    AcknowledgedTime = GETDATE(),\r\n          LastUpdate = GETDATE() {0}\r\n          WHERE AlertDefID = @AlertDefID AND ActiveObject = @ActiveObject AND ObjectType LIKE @ObjectType AND Acknowledged = 0\r\nEND\r\n\r\nSELECT @acknowleged\r\n\r\nCOMMIT", (object) str1)))
      {
        string acknowledgeUsername = AlertsHelper.GetAcknowledgeUsername(accountId, method);
        textCommand.Parameters.AddWithValue("@AcknowledgedBy", (object) acknowledgeUsername);
        textCommand.Parameters.AddWithValue("@AlertDefID", (object) alertId);
        textCommand.Parameters.AddWithValue("@ActiveObject", (object) netObjectId);
        textCommand.Parameters.AddWithValue("@ObjectType", (object) objectType);
        string str2 = string.Format(Resources.COREBUSINESSLAYERDAL_CODE_YK0_7, (object) Environment.NewLine, (object) acknowledgeNotes);
        textCommand.Parameters.AddWithValue("@Notes", (object) str2);
        return (AlertAcknowledgeResult) Convert.ToInt32(SqlHelper.ExecuteScalar(textCommand));
      }
    }

    [Obsolete("Old alerting will be removed")]
    public static void UnacknowledgeAlertsAction(
      List<string> alertKeys,
      string accountID,
      AlertAcknowledgeType acknowledgeType)
    {
      foreach (string alertKey in alertKeys)
      {
        string str1;
        string str2;
        string str3;
        if (AlertsHelper.TryParseAlertKey(alertKey, ref str1, ref str2, ref str3))
        {
          using (SqlCommand textCommand = SqlHelper.GetTextCommand("UPDATE AlertStatus SET \r\n                                    Acknowledged = 0, \r\n                                    AcknowledgedBy = @AcknowledgedBy,\r\n                                    AcknowledgedTime = GETDATE(),\r\n                                    LastUpdate = GETDATE()\r\n                                   WHERE AlertDefID = @AlertDefID\r\n                                     AND ActiveObject = @ActiveObject\r\n                                     AND ObjectType LIKE @ObjectType\r\n                                     AND Acknowledged = 1"))
          {
            textCommand.Parameters.AddWithValue("@AcknowledgedBy", (object) AlertsHelper.GetAcknowledgeUsername(accountID, acknowledgeType));
            textCommand.Parameters.AddWithValue("@AlertDefID", (object) str1);
            textCommand.Parameters.AddWithValue("@ActiveObject", (object) str2);
            textCommand.Parameters.AddWithValue("@ObjectType", (object) str3);
            SqlHelper.ExecuteNonQuery(textCommand);
          }
        }
      }
    }

    [Obsolete("Old alerting will be removed")]
    public static void AcknowledgeAlertsAction(List<string> alertKeys, string accountID)
    {
      AlertDAL.AcknowledgeAlertsAction(alertKeys, accountID, false);
    }

    [Obsolete("Old alerting will be removed")]
    public static void AcknowledgeAlertsAction(
      List<string> alertKeys,
      string accountID,
      bool fromEmail)
    {
      AlertDAL.AcknowledgeAlertsAction(alertKeys, accountID, fromEmail, (string) null);
    }

    [Obsolete("Old alerting will be removed")]
    public static void ClearTriggeredAlert(List<string> alertKeys)
    {
      Regex regex = new Regex("^(\\{){0,1}[0-9a-fA-F]{8}\\-[0-9a-fA-F]{4}\\-[0-9a-fA-F]{4}\\-[0-9a-fA-F]{4}\\-[0-9a-fA-F]{12}(\\}){0,1}$", RegexOptions.Compiled);
      foreach (string alertKey in alertKeys)
      {
        string input;
        string str1;
        string str2;
        if (AlertsHelper.TryParseAlertKey(alertKey, ref input, ref str1, ref str2))
        {
          string empty = string.Empty;
          if (regex.IsMatch(input))
          {
            using (SqlCommand textCommand = SqlHelper.GetTextCommand("DELETE FROM AlertStatus WHERE AlertDefID = @AlertDefID \r\n                                    AND ActiveObject = @ActiveObject AND ObjectType LIKE @ObjectType"))
            {
              textCommand.Parameters.AddWithValue("@AlertDefID", (object) input);
              textCommand.Parameters.AddWithValue("@ActiveObject", (object) str1);
              textCommand.Parameters.AddWithValue("@ObjectType", (object) str2);
              SqlHelper.ExecuteNonQuery(textCommand);
            }
          }
          else
          {
            using (SqlCommand textCommand = SqlHelper.GetTextCommand("DELETE FROM ActiveAlerts WHERE AlertID=@alertID AND ObjectID=@activeObject AND ObjectType LIKE @objectType"))
            {
              textCommand.Parameters.AddWithValue("@alertID", (object) input);
              textCommand.Parameters.AddWithValue("@activeObject", (object) str1);
              textCommand.Parameters.AddWithValue("@objectType", (object) str2);
              SqlHelper.ExecuteNonQuery(textCommand);
            }
          }
        }
      }
    }

    [Obsolete("Old alerting will be removed")]
    public static int EnableAdvancedAlert(Guid alertDefID, bool enabled)
    {
      using (SqlCommand textCommand = SqlHelper.GetTextCommand("\r\nSET NOCOUNT OFF;\r\nUPDATE [AlertDefinitions]\r\n SET [Enabled]=@enabled\r\n WHERE AlertDefID = @AlertDefID"))
      {
        textCommand.Parameters.Add("@AlertDefID", SqlDbType.UniqueIdentifier).Value = (object) alertDefID;
        textCommand.Parameters.Add("@enabled", SqlDbType.Bit).Value = (object) enabled;
        return SqlHelper.ExecuteNonQuery(textCommand);
      }
    }

    [Obsolete("Old alerting will be removed")]
    public static int EnableAdvancedAlerts(List<string> alertDefIDs, bool enabled, bool enableAll)
    {
      if (alertDefIDs.Count == 0)
        return 0;
      string str1 = string.Empty;
      string str2 = string.Empty;
      if (!enableAll)
      {
        foreach (string alertDefId in alertDefIDs)
        {
          str1 = string.Format("{0}{1}'{2}'", (object) str1, (object) str2, (object) alertDefId);
          str2 = ", ";
        }
      }
      using (SqlCommand textCommand = SqlHelper.GetTextCommand(string.Format("\r\nSET NOCOUNT OFF;\r\nUPDATE [AlertDefinitions]\r\n SET [Enabled]=@enabled\r\n{0}", enableAll ? (object) string.Empty : (object) string.Format("WHERE AlertDefID in ({0})", (object) str1))))
      {
        textCommand.Parameters.Add("@enabled", SqlDbType.Bit).Value = (object) enabled;
        return SqlHelper.ExecuteNonQuery(textCommand);
      }
    }

    [Obsolete("Old alerting will be removed")]
    public static int RemoveAdvancedAlert(Guid alertDefID)
    {
      using (SqlCommand textCommand = SqlHelper.GetTextCommand("\r\nSET NOCOUNT OFF;\r\nDelete FROM [AlertDefinitions]\r\n WHERE AlertDefID = @AlertDefID"))
      {
        textCommand.Parameters.Add("@AlertDefID", SqlDbType.UniqueIdentifier).Value = (object) alertDefID;
        return SqlHelper.ExecuteNonQuery(textCommand);
      }
    }

    public static int RemoveAdvancedAlerts(List<string> alertDefIDs, bool deleteAll)
    {
      if (alertDefIDs.Count == 0)
        return 0;
      string str1 = string.Empty;
      string str2 = string.Empty;
      if (!deleteAll)
      {
        foreach (string alertDefId in alertDefIDs)
        {
          str1 = string.Format("{0}{1}'{2}'", (object) str1, (object) str2, (object) alertDefId);
          str2 = ", ";
        }
      }
      using (SqlCommand textCommand = SqlHelper.GetTextCommand(string.Format("\r\nSET NOCOUNT OFF;\r\nDelete FROM [AlertDefinitions]\r\n{0}", deleteAll ? (object) string.Empty : (object) string.Format("WHERE AlertDefID in ({0})", (object) str1))))
        return SqlHelper.ExecuteNonQuery(textCommand);
    }

    [Obsolete("Old alerting will be removed")]
    public static int AdvAlertsCount()
    {
      using (SqlCommand textCommand = SqlHelper.GetTextCommand("SELECT COUNT(AlertDefID) AS TotalCount FROM [AlertDefinitions]"))
        return Convert.ToInt32(SqlHelper.ExecuteScalar(textCommand));
    }

    [Obsolete("Old alerting will be removed")]
    public static DataTable GetAdvancedAlerts()
    {
      DataTable advancedAlerts;
      using (SqlCommand textCommand = SqlHelper.GetTextCommand("Select * from [AlertDefinitions]"))
        advancedAlerts = SqlHelper.ExecuteDataTable(textCommand);
      if (advancedAlerts != null)
        advancedAlerts.TableName = "AlertsDefinition";
      return advancedAlerts;
    }

    [Obsolete("Old alerting will be removed")]
    public static List<AlertAction> GetAdvancedAlertActions(Guid? alertDefID = null)
    {
      return OldAlertsDAL.GetAdvancedAlertActions(alertDefID);
    }

    [Obsolete("Old alerting will be removed")]
    public static AlertDefinitionOld GetAdvancedAlertDefinition(Guid alertDefID)
    {
      return OldAlertsDAL.GetAdvancedAlertDefinition(alertDefID, true);
    }

    [Obsolete("Old alerting will be removed")]
    public static List<AlertDefinitionOld> GetAdvancedAlertDefinitions()
    {
      return OldAlertsDAL.GetAdvancedAlertDefinitions(true);
    }

    [Obsolete("Old alerting will be removed")]
    public static DataTable GetAdvancedAlert(Guid alertDefID)
    {
      DataTable advancedAlert;
      using (SqlCommand textCommand = SqlHelper.GetTextCommand("SELECT Ald.AlertDefID, Ald.AlertName, Ald.AlertDescription, Ald.StartTime, Ald.EndTime, Ald.DOW, Ald.Enabled, Ald.TriggerSustained, Ald.ExecuteInterval, Ald.IgnoreTimeout,\r\n\t\t\t\t\t\t\tAld.TriggerQuery, Ald.ResetQuery, Ald.SuppressionQuery, \r\n\t\t\t\t\t\t\tAcd.TriggerAction, Acd.SortOrder, ActionType, Title, Target, Parameter1, Parameter2, Parameter3, Parameter4\r\n\t\t\t\tFROM [AlertDefinitions] Ald\r\n\t\t\t\tLeft Join [ActionDefinitions] Acd ON Ald.AlertDefID = Acd.AlertDefID\r\nWHERE Ald.AlertDefID=@AlertDefID\r\nOrder by Ald.AlertDefID, Acd.TriggerAction, Acd.SortOrder\r\n"))
      {
        textCommand.Parameters.Add("@AlertDefID", SqlDbType.UniqueIdentifier).Value = (object) alertDefID;
        advancedAlert = SqlHelper.ExecuteDataTable(textCommand);
      }
      if (advancedAlert != null)
        advancedAlert.TableName = "AlertsDefinition";
      return advancedAlert;
    }

    [Obsolete("Old alerting will be removed")]
    public static int UpdateAlertDef(Guid alertDefID, bool enabled)
    {
      return OldAlertsDAL.UpdateAlertDef(alertDefID, enabled);
    }

    [Obsolete("Old alerting will be removed")]
    public static int UpdateAlertDef(
      Guid alertDefID,
      string alertName,
      string alertDescr,
      bool enabled,
      int evInterval,
      string dow,
      DateTime startTime,
      DateTime endTime,
      bool ignoreTimeout)
    {
      using (SqlCommand textCommand = SqlHelper.GetTextCommand("SET NOCOUNT OFF;\r\nUpdate [AlertDefinitions]\r\n SET [AlertName] = @alertName\r\n      ,[AlertDescription] = @alertDescr\r\n      ,[Enabled] = @enabled\r\n      ,[DOW] = @dow\r\n      ,[ExecuteInterval] = @evInterval\r\n\t\t,[StartTime] = @startTime\r\n\t\t,[EndTime] = @endTime\r\n\t,[IgnoreTimeout] = @ignoreTimeout\r\n WHERE AlertDefID = @AlertDefID"))
      {
        textCommand.Parameters.Add("@AlertDefID", SqlDbType.UniqueIdentifier).Value = (object) alertDefID;
        textCommand.Parameters.Add("@alertName", SqlDbType.NVarChar).Value = (object) alertName;
        textCommand.Parameters.Add("@alertDescr", SqlDbType.NVarChar).Value = (object) alertDescr;
        textCommand.Parameters.Add("@enabled", SqlDbType.Bit).Value = (object) enabled;
        textCommand.Parameters.Add("@dow", SqlDbType.NVarChar, 16).Value = (object) dow;
        textCommand.Parameters.Add("@evInterval", SqlDbType.BigInt).Value = (object) evInterval;
        textCommand.Parameters.Add("@startTime", SqlDbType.DateTime).Value = (object) startTime;
        textCommand.Parameters.Add("@endTime", SqlDbType.DateTime).Value = (object) endTime;
        textCommand.Parameters.Add("@ignoreTimeout", SqlDbType.Bit).Value = (object) ignoreTimeout;
        return SqlHelper.ExecuteNonQuery(textCommand);
      }
    }

    [Obsolete("Old alerting will be removed")]
    public static DataTable GetPagebleAdvancedAlerts(
      string column,
      string direction,
      int number,
      int size)
    {
      size = Math.Max(size, 25);
      DataTable pagebleAdvancedAlerts;
      using (SqlCommand textCommand = SqlHelper.GetTextCommand(string.Format("\r\nSelect *\r\nfrom (SELECT *, ROW_NUMBER() OVER (ORDER BY {0} {1}) RowNr from [AlertDefinitions]) t\r\nWHERE RowNr BETWEEN {2} AND {3}\r\nORDER BY {0} {1}", string.IsNullOrEmpty(column) ? (object) "AlertName" : (object) column, string.IsNullOrEmpty(direction) ? (object) "ASC" : (object) direction, (object) (number + 1), (object) (number + size))))
        pagebleAdvancedAlerts = SqlHelper.ExecuteDataTable(textCommand);
      if (pagebleAdvancedAlerts != null)
        pagebleAdvancedAlerts.TableName = "AlertsDefinition";
      return pagebleAdvancedAlerts;
    }

    public static int UpdateAdvancedAlertNote(
      string alerfDefID,
      string activeObject,
      string objectType,
      string notes)
    {
      using (SqlCommand textCommand = SqlHelper.GetTextCommand("UPDATE AlertStatus SET Notes=@Notes Where AlertDefID=@AlertDefID AND ActiveObject=@ActiveObject AND ObjectType=@ObjectType"))
      {
        textCommand.Parameters.Add("@Notes", SqlDbType.NVarChar).Value = (object) notes;
        textCommand.Parameters.AddWithValue("@AlertDefID", (object) alerfDefID);
        textCommand.Parameters.Add("@ActiveObject", SqlDbType.VarChar).Value = (object) activeObject;
        textCommand.Parameters.Add("@ObjectType", SqlDbType.NVarChar).Value = (object) objectType;
        return SqlHelper.ExecuteNonQuery(textCommand);
      }
    }

    public static int AppendNoteToAlert(
      string alertId,
      string activeObject,
      string objectType,
      string note)
    {
      using (SqlCommand textCommand = SqlHelper.GetTextCommand("UPDATE AlertStatus SET Notes =(\r\nCASE\r\nWHEN (Notes IS NULL)\r\nTHEN\r\n @Notes\r\nELSE\r\n CAST(Notes AS NVARCHAR(MAX)) + CHAR(13) + CHAR(10) + '---' + CHAR(13) + CHAR(10) + @Notes\r\nEND\r\n) Where AlertDefID=@AlertDefID AND ActiveObject=@ActiveObject AND ObjectType=@ObjectType"))
      {
        textCommand.Parameters.AddWithValue("@AlertDefID", (object) alertId);
        textCommand.Parameters.Add("@ActiveObject", SqlDbType.VarChar).Value = (object) activeObject;
        textCommand.Parameters.Add("@ObjectType", SqlDbType.NVarChar).Value = (object) objectType;
        textCommand.Parameters.AddWithValue("@Notes", (object) note);
        return SqlHelper.ExecuteNonQuery(textCommand);
      }
    }

    [Obsolete("Old alerting will be removed")]
    public static AlertNotificationSettings GetAlertNotificationSettings(
      string alertDefID,
      string netObjectType,
      string alertName)
    {
      try
      {
        IAlertNotificationSettingsProvider settingsProvider = (IAlertNotificationSettingsProvider) new AlertNotificationSettingsProvider();
        AlertNotificationSettings notificationSettings = (AlertNotificationSettings) null;
        using (SqlCommand textCommand = SqlHelper.GetTextCommand("SELECT AlertName\r\n                            ,ObjectType\r\n                            ,NotifyEnabled\r\n                            ,NotificationSettings\r\n                    FROM [AlertDefinitions]\r\n                    WHERE AlertDefID = @AlertDefinitionId"))
        {
          textCommand.Parameters.AddWithValue("@AlertDefinitionId", (object) (string.IsNullOrWhiteSpace(alertDefID) ? Guid.Empty : new Guid(alertDefID)));
          DataTable dataTable = SqlHelper.ExecuteDataTable(textCommand);
          if (dataTable.Rows.Count != 0)
          {
            string str1 = (string) dataTable.Rows[0]["AlertName"];
            string str2 = (string) dataTable.Rows[0]["ObjectType"];
            bool flag = (bool) dataTable.Rows[0]["NotifyEnabled"];
            string str3 = dataTable.Rows[0]["NotificationSettings"] is DBNull ? (string) null : (string) dataTable.Rows[0]["NotificationSettings"];
            if (str2.Equals(netObjectType, StringComparison.OrdinalIgnoreCase))
            {
              notificationSettings = settingsProvider.GetAlertNotificationSettings(str2, str1, str3);
              notificationSettings.Enabled = flag;
            }
          }
          if (notificationSettings == null)
          {
            notificationSettings = settingsProvider.GetDefaultAlertNotificationSettings(netObjectType, alertName);
            notificationSettings.Enabled = true;
          }
        }
        return notificationSettings;
      }
      catch (Exception ex)
      {
        AlertDAL.Log.Error((object) string.Format("Error getting alert notification settings for alert {0}", (object) alertDefID), ex);
        throw;
      }
    }

    [Obsolete("Old alerting will be removed")]
    public static void SetAlertNotificationSettings(
      string alertDefID,
      AlertNotificationSettings settings)
    {
      try
      {
        if (alertDefID == null)
          throw new ArgumentNullException(nameof (alertDefID));
        string xml = ((IAlertNotificationSettingsConverter) new AlertNotificationSettingsConverter()).ToXml(settings);
        using (SqlCommand textCommand = SqlHelper.GetTextCommand("UPDATE [AlertDefinitions] SET\r\n                            NotifyEnabled = @NotifyEnabled,\r\n                            NotificationSettings = @NotificationSettings\r\n                      WHERE AlertDefID = @AlertDefinitionId"))
        {
          textCommand.Parameters.AddWithValue("@AlertDefinitionId", (object) alertDefID);
          textCommand.Parameters.AddWithValue("@NotifyEnabled", (object) settings.Enabled);
          textCommand.Parameters.AddWithValue("@NotificationSettings", (object) xml);
          SqlHelper.ExecuteNonQuery(textCommand);
        }
      }
      catch (Exception ex)
      {
        AlertDAL.Log.Error((object) string.Format("Error setting alert notification settings for alert {0}", (object) alertDefID), ex);
        throw;
      }
    }

    [Obsolete("Old alerting will be removed")]
    public static AlertNotificationDetails GetAlertDetailsForNotification(
      string alertDefID,
      string activeObject,
      string objectType)
    {
      try
      {
        AlertNotificationDetails detailsForNotification = new AlertNotificationDetails();
        using (SqlCommand textCommand = SqlHelper.GetTextCommand("SELECT s.ActiveObject\r\n                          ,s.ObjectType\r\n                          ,s.ObjectName\r\n                          ,s.TriggerTimeStamp\r\n                          ,s.ResetTimeStamp\r\n                          ,s.Acknowledged\r\n                          ,s.AcknowledgedBy\r\n                          ,s.AcknowledgedTime\r\n                          ,s.AlertNotes\r\n                          ,s.Notes\r\n                          ,s.AlertMessage\r\n                          ,s.TriggerCount\r\n                         ,ad.AlertName\r\n                         ,ad.NotifyEnabled\r\n                         ,ad.NotificationSettings\r\n                    FROM AlertStatus s JOIN AlertDefinitions ad\r\n                      ON s.AlertDefID = ad.AlertDefID\r\n                    WHERE ad.NotifyEnabled = 1\r\n                      AND s.AlertDefID = @AlertDefinitionId\r\n                      AND s.ActiveObject=@ActiveObject \r\n                      AND s.ObjectType LIKE @ObjectType"))
        {
          textCommand.Parameters.AddWithValue("@AlertDefinitionId", (object) alertDefID);
          textCommand.Parameters.AddWithValue("@ActiveObject", (object) activeObject);
          textCommand.Parameters.AddWithValue("@ObjectType", (object) objectType);
          DataTable dataTable = SqlHelper.ExecuteDataTable(textCommand);
          if (dataTable.Rows.Count == 0)
            return detailsForNotification;
          string str1;
          string str2;
          AlertsHelper.GetOriginalUsernameFromAcknowledgeUsername((string) dataTable.Rows[0]["AcknowledgedBy"], ref str1, ref str2);
          detailsForNotification.Acknowledged = (byte) dataTable.Rows[0]["Acknowledged"] > (byte) 0;
          detailsForNotification.AcknowledgedBy = str1;
          detailsForNotification.AcknowledgedMethod = str2;
          detailsForNotification.AcknowledgedTime = ((DateTime) dataTable.Rows[0]["AcknowledgedTime"]).ToUniversalTime();
          detailsForNotification.AlertDefinitionId = alertDefID;
          detailsForNotification.ActiveObject = (string) dataTable.Rows[0]["ActiveObject"];
          detailsForNotification.ObjectType = (string) dataTable.Rows[0]["ObjectType"];
          detailsForNotification.AlertName = (string) dataTable.Rows[0]["AlertName"];
          detailsForNotification.ObjectName = (string) dataTable.Rows[0]["ObjectName"];
          detailsForNotification.AlertNotes = dataTable.Rows[0]["AlertNotes"] is DBNull ? string.Empty : (string) dataTable.Rows[0]["AlertNotes"];
          detailsForNotification.Notes = dataTable.Rows[0]["Notes"] is DBNull ? string.Empty : (string) dataTable.Rows[0]["Notes"];
          detailsForNotification.TriggerCount = (int) dataTable.Rows[0]["TriggerCount"];
          detailsForNotification.AlertMessage = dataTable.Rows[0]["AlertMessage"] is DBNull ? string.Empty : (string) dataTable.Rows[0]["AlertMessage"];
          AlertNotificationDetails notificationDetails1 = detailsForNotification;
          DateTime dateTime = (DateTime) dataTable.Rows[0]["TriggerTimeStamp"];
          DateTime universalTime1 = dateTime.ToUniversalTime();
          notificationDetails1.TriggerTimeStamp = universalTime1;
          AlertNotificationDetails notificationDetails2 = detailsForNotification;
          dateTime = (DateTime) dataTable.Rows[0]["ResetTimeStamp"];
          DateTime universalTime2 = dateTime.ToUniversalTime();
          notificationDetails2.ResetTimeStamp = universalTime2;
          IAlertNotificationSettingsProvider settingsProvider = (IAlertNotificationSettingsProvider) new AlertNotificationSettingsProvider();
          detailsForNotification.NotificationSettings = settingsProvider.GetAlertNotificationSettings(detailsForNotification.ObjectType, detailsForNotification.AlertName, dataTable.Rows[0]["NotificationSettings"] is DBNull ? (string) null : (string) dataTable.Rows[0]["NotificationSettings"]);
          detailsForNotification.NotificationSettings.Enabled = (bool) dataTable.Rows[0]["NotifyEnabled"];
        }
        return detailsForNotification;
      }
      catch (Exception ex)
      {
        AlertDAL.Log.Error((object) string.Format("Error getting alert details for notification for alert {0}", (object) alertDefID), ex);
        throw;
      }
    }

    [Obsolete("Old alerting will be removed")]
    public static AlertNotificationSettings GetBasicAlertNotificationSettings(
      int alertID,
      string netObjectType,
      int propertyID,
      string alertName)
    {
      try
      {
        IAlertNotificationSettingsProvider settingsProvider = (IAlertNotificationSettingsProvider) new AlertNotificationSettingsProvider();
        AlertNotificationSettings notificationSettings = (AlertNotificationSettings) null;
        if (netObjectType.Equals("NetworkNode", StringComparison.OrdinalIgnoreCase))
          netObjectType = "Node";
        using (SqlCommand textCommand = SqlHelper.GetTextCommand("SELECT AlertName\r\n                            ,PropertyID\r\n                            ,NotifyEnabled\r\n                            ,NotificationSettings\r\n                    FROM [Alerts]\r\n                    WHERE AlertID = @AlertDefinitionId"))
        {
          textCommand.Parameters.AddWithValue("@AlertDefinitionId", (object) alertID);
          DataTable dataTable = SqlHelper.ExecuteDataTable(textCommand);
          if (dataTable.Rows.Count != 0)
          {
            string str1 = (string) dataTable.Rows[0]["AlertName"];
            int num1 = (int) dataTable.Rows[0]["PropertyID"];
            bool flag = (bool) dataTable.Rows[0]["NotifyEnabled"];
            string str2 = dataTable.Rows[0]["NotificationSettings"] is DBNull ? (string) null : (string) dataTable.Rows[0]["NotificationSettings"];
            int num2 = propertyID;
            if (num1 == num2)
            {
              notificationSettings = settingsProvider.GetAlertNotificationSettings(netObjectType, str1.Trim(), str2);
              notificationSettings.Enabled = flag;
            }
          }
          if (notificationSettings == null)
          {
            notificationSettings = settingsProvider.GetDefaultAlertNotificationSettings(netObjectType, alertName);
            notificationSettings.Enabled = true;
          }
        }
        return notificationSettings;
      }
      catch (Exception ex)
      {
        AlertDAL.Log.Error((object) string.Format("Error getting basic alert notification settings for alert {0}", (object) alertID), ex);
        throw;
      }
    }

    public static void SetBasicAlertNotificationSettings(
      int alertID,
      AlertNotificationSettings settings)
    {
      try
      {
        string xml = ((IAlertNotificationSettingsConverter) new AlertNotificationSettingsConverter()).ToXml(settings);
        using (SqlCommand textCommand = SqlHelper.GetTextCommand("UPDATE [Alerts] SET\r\n                            NotifyEnabled = @NotifyEnabled,\r\n                            NotificationSettings = @NotificationSettings\r\n                      WHERE AlertID = @AlertDefinitionId"))
        {
          textCommand.Parameters.AddWithValue("@AlertDefinitionId", (object) alertID);
          textCommand.Parameters.AddWithValue("@NotifyEnabled", (object) settings.Enabled);
          textCommand.Parameters.AddWithValue("@NotificationSettings", (object) xml);
          SqlHelper.ExecuteNonQuery(textCommand);
        }
      }
      catch (Exception ex)
      {
        AlertDAL.Log.Error((object) string.Format("Error setting basic alert notification settings for alert {0}", (object) alertID), ex);
        throw;
      }
    }

    [Obsolete("Old alerting will be removed")]
    public static bool RevertMigratedAlert(Guid alertRefId, bool enableInOldAlerting)
    {
      string str = "Update Alerts Set Reverted=@Reverted, Enabled=@Enabled WHERE AlertDefID=@AlertDefID";
      int num;
      using (SqlCommand textCommand = SqlHelper.GetTextCommand("Update AlertDefinitions Set Reverted=@Reverted, Enabled=@Enabled WHERE AlertDefID=@AlertDefID"))
      {
        textCommand.Parameters.AddRange(new SqlParameter[3]
        {
          new SqlParameter("Reverted", (object) true),
          new SqlParameter("Enabled", (object) enableInOldAlerting),
          new SqlParameter("AlertDefID", (object) alertRefId)
        });
        num = SqlHelper.ExecuteNonQuery(textCommand);
      }
      if (num < 1)
      {
        using (SqlCommand textCommand = SqlHelper.GetTextCommand(str))
        {
          textCommand.Parameters.AddRange(new SqlParameter[3]
          {
            new SqlParameter("Reverted", (object) true),
            new SqlParameter("Enabled", (object) enableInOldAlerting),
            new SqlParameter("AlertDefID", (object) alertRefId)
          });
          num = SqlHelper.ExecuteNonQuery(textCommand);
        }
      }
      return num > 0;
    }

    public static int GetAlertObjectId(string alertKey)
    {
      string[] strArray = !string.IsNullOrWhiteSpace(alertKey) ? alertKey.Split(':') : throw new ArgumentException("Parameter is null or empty", nameof (alertKey));
      if (strArray.Length != 3)
      {
        string message = string.Format("Error getting alert key parts. Original key: '{0}'", (object) alertKey);
        AlertDAL.Log.Error((object) message);
        throw new ArgumentException(message);
      }
      Guid result1;
      if (!Guid.TryParse(strArray[0], out result1))
      {
        string message = string.Format("Error getting AlertDefId as GUID. Original key: '{0}'", (object) strArray[0]);
        AlertDAL.Log.Error((object) message);
        throw new ArgumentException(message);
      }
      using (SqlCommand textCommand = SqlHelper.GetTextCommand("SELECT TOP 1 AlertObjectID FROM AlertStatusView WHERE AlertDefID=@alertDefID AND ActiveObject=@activeObject"))
      {
        SqlParameterCollection parameters1 = textCommand.Parameters;
        SqlParameter sqlParameter1 = new SqlParameter("alertDefID", SqlDbType.UniqueIdentifier);
        sqlParameter1.Value = (object) result1;
        parameters1.Add(sqlParameter1);
        SqlParameterCollection parameters2 = textCommand.Parameters;
        SqlParameter sqlParameter2 = new SqlParameter("activeObject", SqlDbType.NVarChar);
        sqlParameter2.Value = (object) strArray[1];
        parameters2.Add(sqlParameter2);
        object obj = SqlHelper.ExecuteScalar(textCommand);
        int result2;
        if (obj != DBNull.Value && obj != null && int.TryParse(obj.ToString(), out result2))
          return result2;
        AlertDAL.Log.InfoFormat("AlertObjectID for alertKey: '{0}' was not found.", (object) alertKey);
        return 0;
      }
    }
  }
}
