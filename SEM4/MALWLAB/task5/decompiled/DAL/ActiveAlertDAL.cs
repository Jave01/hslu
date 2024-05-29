// Decompiled with JetBrains decompiler
// Type: SolarWinds.Orion.Core.BusinessLayer.DAL.ActiveAlertDAL
// Assembly: SolarWinds.Orion.Core.BusinessLayer, Version=2019.4.5200.9083, Culture=neutral, PublicKeyToken=null
// MVID: E12E8C85-5CD9-4E06-8801-182E5104FADE
// Assembly location: E:\task5.dll

using SolarWinds.InformationService.Contract2;
using SolarWinds.Logging;
using SolarWinds.Orion.Common;
using SolarWinds.Orion.Core.Alerting.DAL;
using SolarWinds.Orion.Core.Common;
using SolarWinds.Orion.Core.Common.DALs;
using SolarWinds.Orion.Core.Common.Federation;
using SolarWinds.Orion.Core.Common.Indications;
using SolarWinds.Orion.Core.Common.InformationService;
using SolarWinds.Orion.Core.Common.Models;
using SolarWinds.Orion.Core.Common.Models.Alerts;
using SolarWinds.Orion.Core.Common.Swis;
using SolarWinds.Orion.Core.Models.Alerting;
using SolarWinds.Orion.Core.Strings;
using SolarWinds.Shared;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

#nullable disable
namespace SolarWinds.Orion.Core.BusinessLayer.DAL
{
  public class ActiveAlertDAL
  {
    private static readonly Log log = new Log();
    private readonly Lazy<AlertObjectPropertiesProvider> _alertPropertiesProvider;
    private readonly IInformationServiceProxyCreator _swisProxyCreator;
    private readonly IAlertHistoryDAL _alertHistoryDAL;

    public ActiveAlertDAL()
      : this((IInformationServiceProxyCreator) SwisConnectionProxyPool.GetCreator())
    {
    }

    public ActiveAlertDAL(IInformationServiceProxyCreator swisProxyCreator)
      : this(swisProxyCreator, (IAlertHistoryDAL) new AlertHistoryDAL(swisProxyCreator))
    {
    }

    public ActiveAlertDAL(
      IInformationServiceProxyCreator swisProxyCreator,
      IAlertHistoryDAL alertHistoryDAL)
    {
      this._swisProxyCreator = swisProxyCreator;
      this._alertHistoryDAL = alertHistoryDAL;
      this._alertPropertiesProvider = new Lazy<AlertObjectPropertiesProvider>((Func<AlertObjectPropertiesProvider>) (() => new AlertObjectPropertiesProvider(swisProxyCreator)));
      StatusInfo.Init((IStatusInfoProvider) new DefaultStatusInfoProvider(), ActiveAlertDAL.log);
    }

    public int AcknowledgeActiveAlerts(
      IEnumerable<int> alertObjectIds,
      string accountId,
      string notes,
      DateTime acknowledgeDateTime)
    {
      if (!alertObjectIds.Any<int>())
        return 0;
      int num1 = 0;
      bool flag = !string.IsNullOrEmpty(notes);
      string format1 = "UPDATE AlertObjects SET AlertNote = CASE WHEN (AlertNote IS NULL) THEN @alertNote ELSE AlertNote + CHAR(13) + CHAR(10) + @alertNote END WHERE AlertObjectId IN ({0})";
      string format2 = "UPDATE AlertActive SET Acknowledged=1, AcknowledgedBy=@acknowledgedBy, AcknowledgedDateTime=@acknowledgedDateTime" + " WHERE AlertObjectID IN ({0})";
      string empty = string.Empty;
      int num2 = 0;
      using (SqlCommand textCommand = SqlHelper.GetTextCommand(format2))
      {
        foreach (int alertObjectId in alertObjectIds)
        {
          string parameterName = string.Format("@alertObjectID{0}", (object) num2++);
          if (!string.IsNullOrEmpty(empty))
            empty += ",";
          if (num2 < 1000)
          {
            textCommand.Parameters.AddWithValue(parameterName, (object) alertObjectId);
            empty += parameterName;
          }
          else
            empty += (string) (object) alertObjectId;
        }
        textCommand.Parameters.AddWithValue("@acknowledgedBy", (object) accountId);
        textCommand.Parameters.AddWithValue("@acknowledgedDateTime", (object) acknowledgeDateTime.ToUniversalTime());
        using (SqlConnection connection = DatabaseFunctions.CreateConnection())
        {
          using (SqlTransaction sqlTransaction = connection.BeginTransaction(IsolationLevel.ReadCommitted))
          {
            textCommand.CommandText = string.Format(format2, (object) empty);
            SqlHelper.ExecuteNonQuery(textCommand, connection, sqlTransaction);
            if (flag)
            {
              textCommand.Parameters.AddWithValue("@alertNote", (object) notes);
              textCommand.CommandText = string.Format(format1, (object) empty);
              SqlHelper.ExecuteNonQuery(textCommand, connection, sqlTransaction);
            }
            textCommand.CommandText = string.Format("SELECT AlertObjectID, AlertActiveID FROM AlertActive WHERE AlertObjectID IN ({0})", (object) empty);
            DataTable dataTable = SqlHelper.ExecuteDataTable(textCommand, connection, (DataTable) null);
            num1 = dataTable.Rows.Count;
            foreach (DataRow row in (InternalDataCollectionBase) dataTable.Rows)
            {
              AlertHistory alertHistory = new AlertHistory();
              alertHistory.EventType = (EventType) 2;
              alertHistory.AccountID = accountId;
              alertHistory.Message = notes;
              alertHistory.TimeStamp = acknowledgeDateTime.ToUniversalTime();
              int int32 = row["AlertObjectID"] != DBNull.Value ? Convert.ToInt32(row["AlertObjectID"]) : 0;
              long int64 = row["AlertActiveID"] != DBNull.Value ? Convert.ToInt64(row["AlertActiveID"]) : 0L;
              this._alertHistoryDAL.InsertHistoryItem(alertHistory, int64, int32, connection, sqlTransaction);
            }
            sqlTransaction.Commit();
          }
        }
      }
      return num1;
    }

    public static bool UnacknowledgeAlerts(int[] alertObjectIds, string accountId)
    {
      bool flag = true;
      foreach (int alertObjectId in alertObjectIds)
      {
        if (!ActiveAlertDAL.UnacknowledgeAlert(alertObjectId, accountId))
          flag = false;
      }
      return flag;
    }

    private static bool UnacknowledgeAlert(int alertObjectId, string accountId)
    {
      string str = "UPDATE AlertActive SET Acknowledged= null, \r\n                                     AcknowledgedBy=null, \r\n                                     AcknowledgedDateTime = null\r\n                                     WHERE [AlertObjectID] = @alertObjectId";
      AlertHistoryDAL alertHistoryDal = new AlertHistoryDAL();
      int num = -1;
      using (SqlConnection connection = DatabaseFunctions.CreateConnection())
      {
        using (SqlCommand textCommand = SqlHelper.GetTextCommand(str))
        {
          using (SqlTransaction sqlTransaction = connection.BeginTransaction(IsolationLevel.ReadCommitted))
          {
            textCommand.Parameters.AddWithValue("@alertObjectId", (object) alertObjectId);
            num = SqlHelper.ExecuteNonQuery(textCommand, connection, sqlTransaction);
            textCommand.CommandText = "SELECT AlertObjectID, AlertActiveID FROM AlertActive WHERE [AlertObjectID] = @alertObjectId";
            foreach (DataRow row in (InternalDataCollectionBase) SqlHelper.ExecuteDataTable(textCommand, connection, (DataTable) null).Rows)
            {
              int int32 = row["AlertObjectID"] != DBNull.Value ? Convert.ToInt32(row["AlertObjectID"]) : 0;
              long int64 = row["AlertActiveID"] != DBNull.Value ? Convert.ToInt64(row["AlertActiveID"]) : 0L;
              alertHistoryDal.InsertHistoryItem(new AlertHistory()
              {
                EventType = (EventType) 7,
                AccountID = accountId,
                TimeStamp = DateTime.UtcNow
              }, int64, int32, connection, sqlTransaction);
            }
            sqlTransaction.Commit();
          }
        }
      }
      return num == 1;
    }

    private string GetActiveAlertQuery(
      IEnumerable<CustomProperty> customProperties,
      bool includeNotTriggered = false)
    {
      string empty = string.Empty;
      if (customProperties != null)
      {
        StringBuilder stringBuilder = new StringBuilder();
        foreach (CustomProperty customProperty in customProperties)
          stringBuilder.AppendFormat(", AlertConfigurations.CustomProperties.[{0}]", (object) customProperty.PropertyName);
        empty = stringBuilder.ToString();
      }
      string str = " SELECT DISTINCT OrionSite.SiteID, OrionSite.Name AS SiteName,\r\n                                 Data.AlertActiveID, Data.AlertObjectID, Data.Name,\r\n                                Data.AlertMessage, Data.Severity, Data.ObjectType,\r\n                                Data.EntityUri, Data.EntityType, Data.EntityCaption, Data.EntityDetailsUrl,\r\n                                Data.RelatedNodeUri, Data.RelatedNodeDetailsUrl, Data.RelatedNodeCaption, Data.AlertID, \r\n                                Data.TriggeredDateTime, Data.LastTriggeredDateTime, Data.Message, Data.AccountID, \r\n                                Data.LastExecutedEscalationLevel, Data.AcknowledgedDateTime, Data.Acknowledged, Data.AcknowledgedBy, Data.NumberOfNotes, \r\n                                Data.TriggeredCount, Data.AcknowledgedNote, Data.Canned, Data.Category {1},\r\n                                '' AS IncidentNumber, '' AS IncidentUrl, '' AS AssignedTo\r\n                                FROM (\r\n\r\n                                SELECT AlertActive.InstanceSiteID,AlertActive.AlertActiveID, AlertObjects.AlertObjectID, AlertConfigurations.Name,\r\n                                AlertConfigurations.AlertMessage, AlertConfigurations.Severity, AlertConfigurations.ObjectType,\r\n                                AlertObjects.EntityUri, AlertObjects.EntityType, AlertObjects.EntityCaption, AlertObjects.EntityDetailsUrl,\r\n                                AlertObjects.RelatedNodeUri, AlertObjects.RelatedNodeDetailsUrl, AlertObjects.RelatedNodeCaption, AlertObjects.AlertID, \r\n                                AlertActive.TriggeredDateTime, AlertObjects.LastTriggeredDateTime, AlertActive.TriggeredMessage AS Message, AlertActive.AcknowledgedBy AS AccountID, \r\n                                AlertActive.LastExecutedEscalationLevel, AlertActive.AcknowledgedDateTime, AlertActive.Acknowledged, AlertActive.AcknowledgedBy, AlertActive.NumberOfNotes, \r\n                                AlertObjects.TriggeredCount, AlertObjects.AlertNote as AcknowledgedNote, AlertConfigurations.Canned, AlertConfigurations.Category {0}\r\n                                FROM Orion.AlertObjects AlertObjects";
      return string.Format((!includeNotTriggered ? str + " INNER JOIN Orion.AlertActive (nolock=true) AlertActive ON AlertObjects.AlertObjectID=AlertActive.AlertObjectID AND AlertObjects.InstanceSiteID=AlertActive.InstanceSiteID" : str + " LEFT JOIN Orion.AlertActive (nolock=true) AlertActive ON AlertObjects.AlertObjectID=AlertActive.AlertObjectID AND AlertObjects.InstanceSiteID=AlertActive.InstanceSiteID") + " INNER JOIN Orion.AlertConfigurations (nolock=true) AlertConfigurations ON AlertConfigurations.AlertID=AlertObjects.AlertID AND AlertConfigurations.InstanceSiteID=AlertObjects.InstanceSiteID" + ") AS Data" + " LEFT JOIN Orion.Sites AS OrionSite ON OrionSite.SiteID=Data.InstanceSiteID", (object) empty, (object) empty.Replace("AlertConfigurations.CustomProperties", "Data"));
    }

    private string GetActiveAlertTableByDateQuery()
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.Append("SELECT AlertHistory.AlertHistoryID, AlertHistory.TimeStamp, AlertObjects.AlertID, AlertObjects.EntityCaption, AlertActive.AlertObjectID, AlertActive.AlertActiveID, AlertActive.Acknowledged, AlertActive.AcknowledgedBy, AlertActive.AcknowledgedDateTime, AlertConfigurations.ObjectType,");
      stringBuilder.Append(" AlertConfigurations.Name, AlertConfigurations.AlertMessage, AlertConfigurations.AlertRefID, AlertConfigurations.Description, AlertObjects.EntityType, AlertObjects.EntityDetailsUrl, AlertActive.TriggeredDateTime, AlertObjects.EntityUri, AlertActiveObjects.EntityUri as ActiveObjectEntityUri, AlertObjects.RelatedNodeUri,");
      stringBuilder.Append(" Actions.ActionTypeID, AlertConfigurations.LastEdit, AlertConfigurations.Severity, ActionsProperties.PropertyName, ActionsProperties.PropertyValue, AlertActive.AcknowledgedNote, AlertConfigurations.Canned, AlertConfigurations.Category ");
      stringBuilder.Append(" FROM Orion.AlertObjects AlertObjects");
      stringBuilder.Append(" LEFT JOIN Orion.AlertActive (nolock=true) AlertActive ON AlertObjects.AlertObjectID=AlertActive.AlertObjectID");
      stringBuilder.Append(" INNER JOIN Orion.AlertHistory (nolock=true) AlertHistory ON AlertObjects.AlertObjectID=AlertHistory.AlertObjectID");
      stringBuilder.Append(" INNER JOIN Orion.Actions (nolock=true) Actions ON AlertHistory.ActionID = Actions.ActionID");
      stringBuilder.Append(" INNER JOIN Orion.ActionsProperties (nolock=true) ActionsProperties ON Actions.ActionID = ActionsProperties.ActionID");
      stringBuilder.Append(" INNER JOIN Orion.AlertConfigurations (nolock=true) AlertConfigurations ON AlertConfigurations.AlertID=AlertObjects.AlertID");
      stringBuilder.Append(" LEFT JOIN Orion.AlertActiveObjects (nolock=true) AlertActiveObjects ON AlertActiveObjects.AlertActiveID=AlertActive.AlertActiveID");
      stringBuilder.Append(" WHERE Actions.ActionTypeID IN ('PlaySound', 'TextToSpeech') AND ActionsProperties.PropertyName IN ('Message', 'Text') AND (AlertActive.Acknowledged IS NULL OR AlertActive.Acknowledged = false)");
      return stringBuilder.ToString();
    }

    private ActiveAlert GetActiveAlertFromDataRow(
      DataRow rActiveAlert,
      IEnumerable<CustomProperty> customProperties)
    {
      ActiveAlert activeAlert1 = new ActiveAlert();
      activeAlert1.CustomProperties = new Dictionary<string, object>();
      activeAlert1.TriggeringObjectEntityUri = rActiveAlert["EntityUri"] != DBNull.Value ? Convert.ToString(rActiveAlert["EntityUri"]) : string.Empty;
      activeAlert1.SiteID = rActiveAlert["SiteID"] != DBNull.Value ? Convert.ToInt32(rActiveAlert["SiteID"]) : -1;
      activeAlert1.SiteName = rActiveAlert["SiteName"] != DBNull.Value ? Convert.ToString(rActiveAlert["SiteName"]) : string.Empty;
      string linkPrefix = FederationUrlHelper.GetLinkPrefix(activeAlert1.SiteID);
      activeAlert1.TriggerDateTime = rActiveAlert["TriggeredDateTime"] != DBNull.Value ? DateTime.SpecifyKind(Convert.ToDateTime(rActiveAlert["TriggeredDateTime"]), DateTimeKind.Utc).ToLocalTime() : DateTime.MinValue;
      activeAlert1.ActiveTime = DateTime.Now - activeAlert1.TriggerDateTime;
      ActiveAlert activeAlert2 = activeAlert1;
      DateTime dateTime1;
      DateTime dateTime2;
      if (rActiveAlert["LastTriggeredDateTime"] == DBNull.Value)
      {
        dateTime2 = DateTime.MinValue;
      }
      else
      {
        dateTime1 = Convert.ToDateTime(rActiveAlert["LastTriggeredDateTime"]);
        dateTime2 = dateTime1.ToLocalTime();
      }
      activeAlert2.LastTriggeredDateTime = dateTime2;
      activeAlert1.ActiveTimeDisplay = this.ActiveTimeToDisplayFormat(activeAlert1.ActiveTime);
      activeAlert1.TriggeringObjectCaption = rActiveAlert["EntityCaption"] != DBNull.Value ? Convert.ToString(rActiveAlert["EntityCaption"]) : string.Empty;
      activeAlert1.TriggeringObjectDetailsUrl = linkPrefix + (rActiveAlert["EntityDetailsUrl"] != DBNull.Value ? Convert.ToString(rActiveAlert["EntityDetailsUrl"]) : string.Empty);
      activeAlert1.TriggeringObjectEntityName = rActiveAlert["EntityType"] != DBNull.Value ? Convert.ToString(rActiveAlert["EntityType"]) : string.Empty;
      activeAlert1.RelatedNodeCaption = rActiveAlert["RelatedNodeCaption"] != DBNull.Value ? Convert.ToString(rActiveAlert["RelatedNodeCaption"]) : string.Empty;
      activeAlert1.RelatedNodeDetailsUrl = linkPrefix + (rActiveAlert["RelatedNodeDetailsUrl"] != DBNull.Value ? Convert.ToString(rActiveAlert["RelatedNodeDetailsUrl"]) : string.Empty);
      activeAlert1.RelatedNodeEntityUri = rActiveAlert["RelatedNodeUri"] != DBNull.Value ? Convert.ToString(rActiveAlert["RelatedNodeUri"]) : string.Empty;
      int num = rActiveAlert["Acknowledged"] != DBNull.Value ? (Convert.ToBoolean(rActiveAlert["Acknowledged"]) ? 1 : 0) : 0;
      activeAlert1.Canned = rActiveAlert["Canned"] != DBNull.Value && Convert.ToBoolean(rActiveAlert["Canned"]);
      activeAlert1.Category = rActiveAlert["Category"] != DBNull.Value ? Convert.ToString(rActiveAlert["Category"]) : string.Empty;
      if (num != 0)
      {
        activeAlert1.AcknowledgedBy = rActiveAlert["AcknowledgedBy"] != DBNull.Value ? Convert.ToString(rActiveAlert["AcknowledgedBy"]) : string.Empty;
        activeAlert1.AcknowledgedByFullName = activeAlert1.AcknowledgedBy;
        ActiveAlert activeAlert3 = activeAlert1;
        DateTime dateTime3;
        if (rActiveAlert["AcknowledgedDateTime"] == DBNull.Value)
        {
          dateTime3 = DateTime.MinValue;
        }
        else
        {
          dateTime1 = DateTime.SpecifyKind(Convert.ToDateTime(rActiveAlert["AcknowledgedDateTime"]), DateTimeKind.Utc);
          dateTime3 = dateTime1.ToLocalTime();
        }
        activeAlert3.AcknowledgedDateTime = dateTime3;
        activeAlert1.Notes = rActiveAlert["AcknowledgedNote"] != DBNull.Value ? Convert.ToString(rActiveAlert["AcknowledgedNote"]) : string.Empty;
      }
      activeAlert1.NumberOfNotes = rActiveAlert["NumberOfNotes"] != DBNull.Value ? Convert.ToInt32(rActiveAlert["NumberOfNotes"]) : 0;
      activeAlert1.Id = rActiveAlert["AlertObjectID"] != DBNull.Value ? Convert.ToInt32(rActiveAlert["AlertObjectID"]) : 0;
      activeAlert1.AlertDefId = rActiveAlert["AlertID"] != DBNull.Value ? Convert.ToString(rActiveAlert["AlertID"]) : string.Empty;
      activeAlert1.LegacyAlert = false;
      activeAlert1.Message = rActiveAlert["Message"] != DBNull.Value ? Convert.ToString(rActiveAlert["Message"]) : string.Empty;
      activeAlert1.Name = rActiveAlert["Name"] != DBNull.Value ? Convert.ToString(rActiveAlert["Name"]) : string.Empty;
      activeAlert1.ObjectType = rActiveAlert["ObjectType"] != DBNull.Value ? Convert.ToString(rActiveAlert["ObjectType"]) : string.Empty;
      activeAlert1.Severity = rActiveAlert["Severity"] != DBNull.Value ? (ActiveAlertSeverity) Convert.ToInt32(rActiveAlert["Severity"]) : (ActiveAlertSeverity) 1;
      activeAlert1.TriggeringObjectEntityName = rActiveAlert["EntityType"] != DBNull.Value ? Convert.ToString(rActiveAlert["EntityType"]) : string.Empty;
      activeAlert1.TriggeringObjectCaption = rActiveAlert["EntityCaption"] != DBNull.Value ? Convert.ToString(rActiveAlert["EntityCaption"]) : string.Empty;
      activeAlert1.Status = rActiveAlert["AlertActiveID"] != DBNull.Value ? (ActiveAlertStatus) 1 : (ActiveAlertStatus) 0;
      if (activeAlert1.Status == null)
        activeAlert1.ActiveTimeDisplay = Resources.LIBCODE_PS0_11;
      activeAlert1.RelatedNodeEntityUri = rActiveAlert["RelatedNodeUri"] != DBNull.Value ? Convert.ToString(rActiveAlert["RelatedNodeUri"]) : string.Empty;
      activeAlert1.TriggerCount = rActiveAlert["TriggeredCount"] != DBNull.Value ? Convert.ToInt32(rActiveAlert["TriggeredCount"]) : 0;
      activeAlert1.EscalationLevel = rActiveAlert["LastExecutedEscalationLevel"] != DBNull.Value ? Convert.ToInt32(rActiveAlert["LastExecutedEscalationLevel"]) : 0;
      activeAlert1.IncidentNumber = rActiveAlert["IncidentNumber"] != DBNull.Value ? Convert.ToString(rActiveAlert["IncidentNumber"]) : string.Empty;
      activeAlert1.IncidentUrl = rActiveAlert["IncidentUrl"] != DBNull.Value ? Convert.ToString(rActiveAlert["IncidentUrl"]) : string.Empty;
      activeAlert1.AssignedTo = rActiveAlert["AssignedTo"] != DBNull.Value ? Convert.ToString(rActiveAlert["AssignedTo"]) : string.Empty;
      this.FillCustomPropertiesFromRow(rActiveAlert, customProperties, activeAlert1);
      return activeAlert1;
    }

    private void FillCustomPropertiesFromRow(
      DataRow rActiveAlert,
      IEnumerable<CustomProperty> customProperties,
      ActiveAlert activeAlert)
    {
      foreach (CustomProperty customProperty in customProperties)
      {
        object obj = (object) null;
        if (rActiveAlert[customProperty.PropertyName] != DBNull.Value)
        {
          if (customProperty.PropertyType == typeof (string))
            obj = (object) Convert.ToString(rActiveAlert[customProperty.PropertyName]);
          else if (customProperty.PropertyType == typeof (DateTime))
            obj = (object) DateTime.SpecifyKind(Convert.ToDateTime(rActiveAlert[customProperty.PropertyName]), DateTimeKind.Local);
          else if (customProperty.PropertyType == typeof (int))
            obj = (object) Convert.ToInt32(rActiveAlert[customProperty.PropertyName]);
          else if (customProperty.PropertyType == typeof (float))
            obj = (object) Convert.ToSingle(rActiveAlert[customProperty.PropertyName]);
          else if (customProperty.PropertyType == typeof (bool))
            obj = (object) Convert.ToBoolean(rActiveAlert[customProperty.PropertyName]);
        }
        activeAlert.CustomProperties.Add(customProperty.PropertyName, obj);
      }
    }

    private int GetAlertObjectIdByAlertActiveId(long alertActiveId)
    {
      int idByAlertActiveId = 0;
      string str = "SELECT TOP 1 AlertObjectID FROM Orion.AlertHistory (nolock=true) WHERE AlertActiveID=@alertActiveID";
      using (IInformationServiceProxy2 iinformationServiceProxy2 = this._swisProxyCreator.Create())
      {
        DataTable dataTable = InformationServiceProxyExtensions.QueryWithAppendedErrors((IInformationServiceProxy) iinformationServiceProxy2, str, new Dictionary<string, object>()
        {
          {
            "alertActiveID",
            (object) alertActiveId
          }
        });
        if (dataTable.Rows.Count > 0)
          idByAlertActiveId = dataTable.Rows[0]["AlertObjectID"] != DBNull.Value ? Convert.ToInt32(dataTable.Rows[0]["AlertObjectID"]) : 0;
      }
      return idByAlertActiveId;
    }

    private ActiveAlertStatus GetTriggeredStatusForActiveAlert(int alertObjectId)
    {
      return !SqlHelper.ExecuteExistsParams("SELECT AlertActiveID FROM dbo.AlertActive WITH(NOLOCK) WHERE AlertObjectID=@alertObjectId", new SqlParameter[1]
      {
        new SqlParameter("@alertObjectId", (object) alertObjectId)
      }) ? (ActiveAlertStatus) 0 : (ActiveAlertStatus) 1;
    }

    internal DataTable GetAlertResetOrUpdateIndicationPropertiesTableByAlertObjectIds(
      IEnumerable<int> alertObjectIds)
    {
      return this._alertPropertiesProvider.Value.GetAlertIndicationProperties(alertObjectIds);
    }

    private string GetQueryWithViewLimitations(string query, int viewLimitationId)
    {
      return viewLimitationId != 0 ? string.Format("{0} WITH LIMITATION {1}", (object) query, (object) viewLimitationId) : query;
    }

    private int GetOnlyViewLimitation(IEnumerable<int> limitationIds, bool federationEnabled = false)
    {
      if (limitationIds == null || !limitationIds.Any<int>())
        return 0;
      if (limitationIds.Count<int>() == 1)
        return limitationIds.ElementAt<int>(0);
      string str1 = "SELECT AccountID, LimitationID1, LimitationID2, LimitationID3 FROM Orion.Accounts (nolock=true) \r\n                                 WHERE (LimitationID1 IS NOT NULL OR LimitationID1<>0 OR LimitationID2 IS NOT NULL \r\n                                        OR LimitationID2<>0 OR LimitationID3 IS NOT NULL OR LimitationID3<>0)\r\n                                 AND AccountID=@accountID";
      string accountId = AccountContext.GetAccountID();
      using (IInformationServiceProxy2 iinformationServiceProxy2_1 = this._swisProxyCreator.Create())
      {
        IInformationServiceProxy2 iinformationServiceProxy2_2 = iinformationServiceProxy2_1;
        string str2 = str1;
        Dictionary<string, object> dictionary = new Dictionary<string, object>();
        dictionary.Add("accountID", (object) accountId);
        int num = federationEnabled ? 1 : 0;
        DataTable dataTable = InformationServiceProxyExtensions.QueryWithAppendedErrors((IInformationServiceProxy) iinformationServiceProxy2_2, str2, dictionary, num != 0);
        int limitationID1 = 0;
        int limitationID2 = 0;
        int limitationID3 = 0;
        if (dataTable.Rows.Count <= 0)
        {
          if (!limitationIds.Any<int>())
            goto label_13;
        }
        if (dataTable.Rows.Count > 0)
        {
          limitationID1 = dataTable.Rows[0]["LimitationID1"] != DBNull.Value ? Convert.ToInt32(dataTable.Rows[0]["LimitationID1"]) : 0;
          limitationID2 = dataTable.Rows[0]["LimitationID2"] != DBNull.Value ? Convert.ToInt32(dataTable.Rows[0]["LimitationID2"]) : 0;
          limitationID3 = dataTable.Rows[0]["LimitationID3"] != DBNull.Value ? Convert.ToInt32(dataTable.Rows[0]["LimitationID3"]) : 0;
        }
        return limitationIds.FirstOrDefault<int>((System.Func<int, bool>) (item => item != limitationID1 && item != limitationID2 && item != limitationID3));
      }
label_13:
      return limitationIds.ElementAt<int>(0);
    }

    private IEnumerable<string> GetUrisForGlobalAlert(int id, bool federationEnabled)
    {
      using (IInformationServiceProxy2 iinformationServiceProxy2_1 = this._swisProxyCreator.Create())
      {
        IInformationServiceProxy2 iinformationServiceProxy2_2 = iinformationServiceProxy2_1;
        Dictionary<string, object> dictionary = new Dictionary<string, object>();
        dictionary.Add("objectId", (object) id);
        int num = federationEnabled ? 1 : 0;
        return (IEnumerable<string>) InformationServiceProxyExtensions.QueryWithAppendedErrors((IInformationServiceProxy) iinformationServiceProxy2_2, "SELECT a.AlertActiveObjects.EntityUri\r\n                                              FROM Orion.AlertActive (nolock=true) AS a\r\n                                              WHERE a.AlertObjectID=@objectId", dictionary, num != 0).AsEnumerable().Where<DataRow>((System.Func<DataRow, bool>) (item => item["EntityUri"] != DBNull.Value)).Select<DataRow, string>((System.Func<DataRow, string>) (item => Convert.ToString(item["EntityUri"])));
      }
    }

    public ActiveAlert GetActiveAlert(
      int alertObjectId,
      IEnumerable<int> limitationIDs,
      bool includeNotTriggered = true)
    {
      ActiveAlert activeAlert = (ActiveAlert) null;
      IEnumerable<CustomProperty> propertiesForEntity = CustomPropertyMgr.GetCustomPropertiesForEntity("Orion.AlertConfigurationsCustomProperties");
      string str1 = this.GetActiveAlertQuery(propertiesForEntity, includeNotTriggered) + " WHERE Data.AlertObjectID=@alertObjectId";
      using (IInformationServiceProxy2 swisProxy = this._swisProxyCreator.Create())
      {
        DataTable dataTable1 = InformationServiceProxyExtensions.QueryWithAppendedErrors((IInformationServiceProxy) swisProxy, str1, new Dictionary<string, object>()
        {
          {
            nameof (alertObjectId),
            (object) alertObjectId
          }
        });
        if (dataTable1.Rows.Count > 0)
        {
          activeAlert = this.GetActiveAlertFromDataRow(dataTable1.Rows[0], propertiesForEntity);
          AlertIncidentCache.Build(swisProxy, new int?(activeAlert.Id)).FillIncidentInfo(activeAlert);
          if (!string.IsNullOrEmpty(activeAlert.RelatedNodeEntityUri))
          {
            string str2 = "SELECT NodeID, Status FROM Orion.Nodes (nolock=true) WHERE Uri=@uri";
            DataTable dataTable2 = InformationServiceProxyExtensions.QueryWithAppendedErrors((IInformationServiceProxy) swisProxy, str2, new Dictionary<string, object>()
            {
              {
                "uri",
                (object) activeAlert.RelatedNodeEntityUri
              }
            });
            if (dataTable2.Rows.Count > 0)
            {
              activeAlert.RelatedNodeID = dataTable2.Rows[0]["NodeID"] != DBNull.Value ? Convert.ToInt32(dataTable2.Rows[0]["NodeID"]) : 0;
              activeAlert.RelatedNodeStatus = dataTable2.Rows[0]["Status"] != DBNull.Value ? Convert.ToInt32(dataTable2.Rows[0]["Status"]) : 0;
              activeAlert.RelatedNodeDetailsUrl = string.Format("/Orion/View.aspx?NetObject=N:{0}", (object) activeAlert.RelatedNodeID);
            }
          }
          if (activeAlert.TriggeringObjectEntityName == "Orion.Nodes")
            activeAlert.ActiveNetObject = Convert.ToString(activeAlert.RelatedNodeID);
          if (!string.IsNullOrEmpty(activeAlert.TriggeringObjectEntityUri))
          {
            string str3 = "SELECT TME.Status, TME.Uri FROM System.ManagedEntity (nolock=true) TME WHERE TME.Uri=@uri";
            DataTable dataTable3 = InformationServiceProxyExtensions.QueryWithAppendedErrors((IInformationServiceProxy) swisProxy, str3, new Dictionary<string, object>()
            {
              {
                "uri",
                (object) activeAlert.TriggeringObjectEntityUri
              }
            });
            if (dataTable3.Rows.Count > 0)
              activeAlert.TriggeringObjectStatus = dataTable3.Rows[0]["Status"] != DBNull.Value ? Convert.ToInt32(dataTable3.Rows[0]["Status"]) : 0;
          }
          else
            activeAlert.TriggeringObjectStatus = this.GetRollupStatusForGlobalAlert(activeAlert.TriggeringObjectEntityName, alertObjectId);
          activeAlert.Status = this.GetTriggeredStatusForActiveAlert(alertObjectId);
        }
      }
      return activeAlert;
    }

    private int GetRollupStatusForGlobalAlert(
      string entity,
      int alertObjectId,
      bool federationEnabled = false)
    {
      if (!this.EntityHasStatusProperty(entity, federationEnabled))
        return 0;
      IEnumerable<string> urisForGlobalAlert = this.GetUrisForGlobalAlert(alertObjectId, federationEnabled);
      if (!urisForGlobalAlert.Any<string>())
        return 0;
      List<int> statuses = new List<int>();
      StringBuilder sbCondition = new StringBuilder();
      Action<Dictionary<string, object>> action = (Action<Dictionary<string, object>>) (swqlParameters =>
      {
        string str = string.Format("SELECT Status FROM {0} (nolock=true) WHERE {1}", (object) entity, (object) sbCondition);
        using (IInformationServiceProxy2 iinformationServiceProxy2 = this._swisProxyCreator.Create())
        {
          foreach (DataRow row in (InternalDataCollectionBase) InformationServiceProxyExtensions.QueryWithAppendedErrors((IInformationServiceProxy) iinformationServiceProxy2, str, swqlParameters, federationEnabled).Rows)
          {
            int result = 0;
            if (row["Status"] == DBNull.Value || !int.TryParse(Convert.ToString(row["Status"]), out result))
              result = 0;
            statuses.Add(result);
          }
        }
      });
      int num = 0;
      Dictionary<string, object> dictionary = new Dictionary<string, object>();
      foreach (string str in urisForGlobalAlert)
      {
        if (num > 0)
          sbCondition.Append(" OR ");
        sbCondition.AppendFormat("Uri=@uri{0}", (object) num);
        dictionary.Add(string.Format("uri{0}", (object) num), (object) str);
        ++num;
        if (num % 1000 == 0)
        {
          action(dictionary);
          dictionary = new Dictionary<string, object>();
          sbCondition.Clear();
          num = 0;
        }
      }
      if (num > 0)
        action(dictionary);
      return StatusInfo.RollupStatus((IEnumerable<int>) statuses, (EvaluationMethod) 2);
    }

    private bool EntityHasStatusProperty(string entity, bool federationEnabled = false)
    {
      using (IInformationServiceProxy2 iinformationServiceProxy2_1 = this._swisProxyCreator.Create())
      {
        IInformationServiceProxy2 iinformationServiceProxy2_2 = iinformationServiceProxy2_1;
        Dictionary<string, object> dictionary = new Dictionary<string, object>();
        dictionary.Add("entityName", (object) entity);
        int num = federationEnabled ? 1 : 0;
        return InformationServiceProxyExtensions.QueryWithAppendedErrors((IInformationServiceProxy) iinformationServiceProxy2_2, "SELECT Name FROM Metadata.Property WHERE EntityName=@entityName AND Name='Status'", dictionary, num != 0).Rows.Count > 0;
      }
    }

    public IEnumerable<ActiveAlert> GetActiveAlerts(
      IEnumerable<CustomProperty> customProperties,
      IEnumerable<int> limitationIDs,
      out List<ErrorMessage> errors,
      bool federationEnabled,
      bool includeNotTriggered = false)
    {
      List<ActiveAlert> activeAlerts = new List<ActiveAlert>();
      string activeAlertQuery = this.GetActiveAlertQuery(customProperties, includeNotTriggered);
      if (OrionConfiguration.IsDemoServer)
        activeAlertQuery += " WHERE (Data.TriggeredDateTime <= GETUTCDATE())";
      string withViewLimitations = this.GetQueryWithViewLimitations(activeAlertQuery, this.GetOnlyViewLimitation(limitationIDs, federationEnabled));
      Dictionary<string, ActiveAlert> dictionary = new Dictionary<string, ActiveAlert>();
      using (IInformationServiceProxy2 swisProxy = this._swisProxyCreator.Create())
      {
        DataTable dataTable = InformationServiceProxyExtensions.QueryWithAppendedErrors((IInformationServiceProxy) swisProxy, withViewLimitations, federationEnabled);
        errors = FederatedResultTableParser.GetErrorsFromDataTable(dataTable);
        AlertIncidentCache alertIncidentCache = AlertIncidentCache.Build(swisProxy);
        foreach (DataRow row in (InternalDataCollectionBase) dataTable.Rows)
        {
          int int32 = row["AlertID"] != DBNull.Value ? Convert.ToInt32(row["AlertID"]) : 0;
          string key = string.Format("{0}|{1}", row["EntityUri"] != DBNull.Value ? (object) Convert.ToString(row["EntityUri"]) : (object) string.Empty, (object) int32);
          if (!dictionary.ContainsKey(key))
          {
            ActiveAlert alertFromDataRow = this.GetActiveAlertFromDataRow(row, customProperties);
            alertIncidentCache.FillIncidentInfo(alertFromDataRow);
            if (string.IsNullOrEmpty(alertFromDataRow.TriggeringObjectEntityUri))
              alertFromDataRow.TriggeringObjectStatus = this.GetRollupStatusForGlobalAlert(alertFromDataRow.TriggeringObjectEntityName, alertFromDataRow.Id);
            activeAlerts.Add(alertFromDataRow);
          }
        }
      }
      return (IEnumerable<ActiveAlert>) activeAlerts;
    }

    public List<ActiveAlertDetailed> GetAlertTableByDate(
      DateTime dateTime,
      int? lastAlertHistoryId,
      List<int> limitationIDs)
    {
      List<ActiveAlert> source = new List<ActiveAlert>();
      StringBuilder stringBuilder = new StringBuilder(this.GetActiveAlertTableByDateQuery());
      object obj;
      if (lastAlertHistoryId.HasValue)
      {
        obj = (object) lastAlertHistoryId.Value;
        stringBuilder.Append("AND (AlertHistory.AlertHistoryID > @param)");
      }
      else
      {
        obj = (object) dateTime;
        stringBuilder.Append("AND (AlertHistory.TimeStamp > @param)");
      }
      string withViewLimitations = this.GetQueryWithViewLimitations(stringBuilder.ToString(), this.GetOnlyViewLimitation((IEnumerable<int>) limitationIDs));
      using (IInformationServiceProxy2 iinformationServiceProxy2_1 = this._swisProxyCreator.Create())
      {
        IInformationServiceProxy2 iinformationServiceProxy2_2 = iinformationServiceProxy2_1;
        string str = withViewLimitations;
        foreach (DataRow row in (InternalDataCollectionBase) InformationServiceProxyExtensions.QueryWithAppendedErrors((IInformationServiceProxy) iinformationServiceProxy2_2, str, new Dictionary<string, object>()
        {
          {
            "param",
            obj
          }
        }).Rows)
        {
          ActiveAlertDetailed activeAlertDetailed1 = new ActiveAlertDetailed();
          ((ActiveAlert) activeAlertDetailed1).Id = row["AlertActiveID"] != DBNull.Value ? Convert.ToInt32(row["AlertActiveID"]) : 0;
          activeAlertDetailed1.AlertHistoryId = row["AlertHistoryID"] != DBNull.Value ? Convert.ToInt32(row["AlertHistoryID"]) : 0;
          ((ActiveAlert) activeAlertDetailed1).AlertDefId = row["AlertID"] != DBNull.Value ? Convert.ToString(row["AlertID"]) : string.Empty;
          activeAlertDetailed1.AlertRefID = row["AlertRefID"] != DBNull.Value ? new Guid(Convert.ToString(row["AlertRefID"])) : Guid.Empty;
          ((ActiveAlert) activeAlertDetailed1).ActiveNetObject = row["AlertObjectID"] != DBNull.Value ? Convert.ToString(row["AlertObjectID"]) : string.Empty;
          ((ActiveAlert) activeAlertDetailed1).ObjectType = row["ObjectType"] != DBNull.Value ? Convert.ToString(row["ObjectType"]) : string.Empty;
          ((ActiveAlert) activeAlertDetailed1).Name = row["Name"] != DBNull.Value ? Convert.ToString(row["Name"]) : string.Empty;
          ((ActiveAlert) activeAlertDetailed1).TriggeringObjectDetailsUrl = row["EntityDetailsUrl"] != DBNull.Value ? Convert.ToString(row["EntityDetailsUrl"]) : string.Empty;
          DateTime dateTime1 = row["TriggeredDateTime"] != DBNull.Value ? DateTime.SpecifyKind(Convert.ToDateTime(row["TriggeredDateTime"]), DateTimeKind.Utc) : (row["TimeStamp"] != DBNull.Value ? Convert.ToDateTime(row["TimeStamp"]) : DateTime.MinValue);
          ((ActiveAlert) activeAlertDetailed1).TriggerDateTime = dateTime1.ToLocalTime();
          ((ActiveAlert) activeAlertDetailed1).TriggeringObjectEntityUri = row["EntityUri"] != DBNull.Value ? Convert.ToString(row["EntityUri"]) : (row["ActiveObjectEntityUri"] != DBNull.Value ? Convert.ToString(row["ActiveObjectEntityUri"]) : string.Empty);
          ((ActiveAlert) activeAlertDetailed1).RelatedNodeEntityUri = row["RelatedNodeUri"] != DBNull.Value ? Convert.ToString(row["RelatedNodeUri"]) : string.Empty;
          ((ActiveAlert) activeAlertDetailed1).TriggeringObjectEntityName = row["EntityType"] != DBNull.Value ? Convert.ToString(row["EntityType"]) : string.Empty;
          ((ActiveAlert) activeAlertDetailed1).Message = row["PropertyValue"] != DBNull.Value ? Convert.ToString(row["PropertyValue"]) : string.Empty;
          ((ActiveAlert) activeAlertDetailed1).TriggeringObjectCaption = row["EntityCaption"] != DBNull.Value ? Convert.ToString(row["EntityCaption"]) : string.Empty;
          activeAlertDetailed1.ActionType = row["ActionTypeID"] != DBNull.Value ? Convert.ToString(row["ActionTypeID"]) : string.Empty;
          activeAlertDetailed1.AlertDefDescription = row["Description"] != DBNull.Value ? Convert.ToString(row["Description"]) : string.Empty;
          activeAlertDetailed1.AlertDefLastEdit = row["LastEdit"] != DBNull.Value ? DateTime.SpecifyKind(Convert.ToDateTime(row["LastEdit"]), DateTimeKind.Utc) : DateTime.MinValue;
          activeAlertDetailed1.AlertDefSeverity = row["Severity"] != DBNull.Value ? Convert.ToInt32(row["Severity"]) : 2;
          ((ActiveAlert) activeAlertDetailed1).Severity = row["Severity"] != DBNull.Value ? (ActiveAlertSeverity) Convert.ToInt32(row["Severity"]) : (ActiveAlertSeverity) 2;
          activeAlertDetailed1.AlertDefMessage = row["AlertMessage"] != DBNull.Value ? Convert.ToString(row["AlertMessage"]) : string.Empty;
          ActiveAlertDetailed activeAlertDetailed2 = activeAlertDetailed1;
          if ((row["Acknowledged"] != DBNull.Value ? (Convert.ToBoolean(row["Acknowledged"]) ? 1 : 0) : 0) != 0)
          {
            ((ActiveAlert) activeAlertDetailed2).AcknowledgedBy = row["AcknowledgedBy"] != DBNull.Value ? Convert.ToString(row["AcknowledgedBy"]) : string.Empty;
            ((ActiveAlert) activeAlertDetailed2).AcknowledgedByFullName = ((ActiveAlert) activeAlertDetailed2).AcknowledgedBy;
            ActiveAlertDetailed activeAlertDetailed3 = activeAlertDetailed2;
            DateTime dateTime2;
            if (row["AcknowledgedDateTime"] == DBNull.Value)
            {
              dateTime2 = DateTime.MinValue;
            }
            else
            {
              dateTime1 = DateTime.SpecifyKind(Convert.ToDateTime(row["AcknowledgedDateTime"]), DateTimeKind.Utc);
              dateTime2 = dateTime1.ToLocalTime();
            }
            ((ActiveAlert) activeAlertDetailed3).AcknowledgedDateTime = dateTime2;
            ((ActiveAlert) activeAlertDetailed2).Notes = row["AcknowledgedNote"] != DBNull.Value ? Convert.ToString(row["AcknowledgedNote"]) : string.Empty;
          }
          source.Add((ActiveAlert) activeAlertDetailed2);
        }
      }
      return source.Cast<ActiveAlertDetailed>().ToList<ActiveAlertDetailed>();
    }

    public int ClearTriggeredActiveAlerts(IEnumerable<int> alertObjectIds, string accountId)
    {
      if (!alertObjectIds.Any<int>())
        return 0;
      string empty = string.Empty;
      using (SqlCommand textCommand = SqlHelper.GetTextCommand(string.Empty))
      {
        using (SqlConnection connection = DatabaseFunctions.CreateConnection())
        {
          using (SqlTransaction transaction = connection.BeginTransaction(IsolationLevel.ReadCommitted))
          {
            textCommand.Transaction = transaction;
            for (int index = 0; index < alertObjectIds.Count<int>(); ++index)
            {
              if (index < 1000)
              {
                string parameterName = string.Format("@alertObjectID{0}", (object) index);
                textCommand.Parameters.AddWithValue(parameterName, (object) alertObjectIds.ElementAt<int>(index));
                if (empty != string.Empty)
                  empty += ",";
                empty += parameterName;
              }
              else
              {
                if (empty != string.Empty)
                  empty += ",";
                empty += (string) (object) alertObjectIds.ElementAt<int>(index);
              }
            }
            textCommand.CommandText = string.Format("SELECT AlertObjectID, AlertActiveID FROM AlertActive WHERE [AlertObjectID] IN ({0})", (object) empty);
            foreach (DataRow row in (InternalDataCollectionBase) SqlHelper.ExecuteDataTable(textCommand, connection, (DataTable) null).Rows)
            {
              int int32 = row["AlertObjectID"] != DBNull.Value ? Convert.ToInt32(row["AlertObjectID"]) : 0;
              this._alertHistoryDAL.InsertHistoryItem(new AlertHistory()
              {
                EventType = (EventType) 8,
                AccountID = accountId,
                TimeStamp = DateTime.UtcNow
              }, row["AlertActiveID"] != DBNull.Value ? Convert.ToInt64(row["AlertActiveID"]) : 0L, int32, connection, transaction);
            }
            foreach (int alertObjectId in alertObjectIds)
              this.UpdateAlertCaptionAfterReset(alertObjectId, transaction);
            textCommand.CommandText = string.Format("DELETE FROM AlertActive WHERE AlertObjectID IN ({0})", (object) empty);
            int num = SqlHelper.ExecuteNonQuery(textCommand, connection, transaction);
            transaction.Commit();
            return num;
          }
        }
      }
    }

    public IEnumerable<AlertClearedIndicationProperties> GetAlertClearedIndicationPropertiesByAlertObjectIds(
      IEnumerable<int> alertObjectIds)
    {
      if (!alertObjectIds.Any<int>())
        return Enumerable.Empty<AlertClearedIndicationProperties>();
      List<AlertClearedIndicationProperties> byAlertObjectIds = new List<AlertClearedIndicationProperties>();
      foreach (DataRow row in (InternalDataCollectionBase) this.GetAlertResetOrUpdateIndicationPropertiesTableByAlertObjectIds(alertObjectIds).Rows)
      {
        AlertClearedIndicationProperties indicationProperties = new AlertClearedIndicationProperties();
        indicationProperties.ClearedTime = DateTime.UtcNow;
        AlertSeverity alertSeverity = row["Severity"] != DBNull.Value ? (AlertSeverity) Convert.ToInt32(row["Severity"]) : (AlertSeverity) 2;
        ((AlertIndicationProperties) indicationProperties).AlertId = row["AlertID"] != DBNull.Value ? Convert.ToInt32(row["AlertID"]) : 0;
        ((AlertIndicationProperties) indicationProperties).AlertName = row["Name"] != DBNull.Value ? Convert.ToString(row["Name"]) : string.Empty;
        ((AlertIndicationProperties) indicationProperties).AlertObjectId = row["AlertObjectID"] != DBNull.Value ? Convert.ToInt32(row["AlertObjectID"]) : 0;
        ((AlertIndicationProperties) indicationProperties).AlertDefinitionId = row["AlertRefID"] != DBNull.Value ? new Guid(Convert.ToString(row["AlertRefID"])) : Guid.Empty;
        ((AlertIndicationProperties) indicationProperties).DetailsUrl = row["EntityDetailsUrl"] != DBNull.Value ? Convert.ToString(row["EntityDetailsUrl"]) : string.Empty;
        ((AlertIndicationProperties) indicationProperties).Message = row["TriggeredMessage"] != DBNull.Value ? Convert.ToString(row["TriggeredMessage"]) : string.Empty;
        ((AlertIndicationProperties) indicationProperties).ObjectId = row["EntityUri"] != DBNull.Value ? Convert.ToString(row["EntityUri"]) : string.Empty;
        ((AlertIndicationProperties) indicationProperties).ObjectName = row["EntityType"] != DBNull.Value ? Convert.ToString(row["EntityType"]) : string.Empty;
        ((AlertIndicationProperties) indicationProperties).NetObject = row["EntityNetObjectId"] != DBNull.Value ? Convert.ToString(row["EntityNetObjectId"]) : string.Empty;
        ((AlertIndicationProperties) indicationProperties).EntityCaption = row["EntityCaption"] != DBNull.Value ? Convert.ToString(row["EntityCaption"]) : string.Empty;
        ((AlertIndicationProperties) indicationProperties).ObjectType = ((AlertIndicationProperties) indicationProperties).ObjectName;
        ((AlertIndicationProperties) indicationProperties).TriggerTimeStamp = row["TriggeredDateTime"] != DBNull.Value ? DateTime.SpecifyKind(Convert.ToDateTime(row["TriggeredDateTime"]), DateTimeKind.Utc) : DateTime.MinValue;
        indicationProperties.ObjectUris = string.IsNullOrWhiteSpace(((AlertIndicationProperties) indicationProperties).ObjectId) ? this.GetGlobalAlertRelatedUris(((AlertIndicationProperties) indicationProperties).AlertId) : (IEnumerable<string>) new List<string>();
        ((AlertIndicationProperties) indicationProperties).Severity = alertSeverity.ToString();
        byAlertObjectIds.Add(indicationProperties);
      }
      return (IEnumerable<AlertClearedIndicationProperties>) byAlertObjectIds;
    }

    private IEnumerable<string> GetGlobalAlertRelatedUris(int AlertID)
    {
      DataTable dataTable;
      using (SqlCommand textCommand = SqlHelper.GetTextCommand("SELECT ObjectId FROM [AlertConditionState] WHERE [AlertID]=@alertID AND [Type] = 0 AND [Resolved] = 1"))
      {
        textCommand.Parameters.AddWithValue("alertID", (object) AlertID);
        dataTable = SqlHelper.ExecuteDataTable(textCommand);
      }
      List<string> alertRelatedUris = new List<string>();
      foreach (DataRow row in (InternalDataCollectionBase) dataTable.Rows)
        alertRelatedUris.Add(row[0].ToString());
      return (IEnumerable<string>) alertRelatedUris;
    }

    public IEnumerable<AlertUpdatedIndication> GetAlertUpdatedIndicationPropertiesByAlertObjectIds(
      IEnumerable<int> alertObjectIds,
      string accountId,
      string notes,
      DateTime acknowledgedDateTime,
      bool acknowledge)
    {
      return !alertObjectIds.Any<int>() ? Enumerable.Empty<AlertUpdatedIndication>() : this._alertPropertiesProvider.Value.GetAlertUpdatedIndicationProperties(alertObjectIds, accountId, notes, acknowledgedDateTime, acknowledge);
    }

    public IEnumerable<AlertUpdatedIndication> GetAlertUpdatedIndicationPropertiesByAlertObjectIds(
      IEnumerable<int> alertObjectIds,
      string accountId,
      string notes,
      DateTime acknowledgedDateTime,
      bool acknowledge,
      string method)
    {
      return !alertObjectIds.Any<int>() ? Enumerable.Empty<AlertUpdatedIndication>() : this._alertPropertiesProvider.Value.GetAlertUpdatedIndicationProperties(alertObjectIds, accountId, notes, acknowledgedDateTime, acknowledge, method);
    }

    public IEnumerable<int> LimitAlertAckStateUpdateCandidates(
      IEnumerable<int> alertObjectIds,
      bool requestedAcknowledgedState)
    {
      return !alertObjectIds.Any<int>() ? Enumerable.Empty<int>() : (IEnumerable<int>) this.GetAlertResetOrUpdateIndicationPropertiesTableByAlertObjectIds(alertObjectIds).Rows.Cast<DataRow>().Where<DataRow>((System.Func<DataRow, bool>) (row => row["AlertObjectId"] != DBNull.Value)).Select(row => new
      {
        row = row,
        ackState = row["Acknowledged"] != DBNull.Value && Convert.ToBoolean(row["Acknowledged"])
      }).Where(_param1 => _param1.ackState != requestedAcknowledgedState).Select(_param1 => Convert.ToInt32(_param1.row["AlertObjectId"])).ToList<int>();
    }

    internal string ActiveTimeToDisplayFormat(TimeSpan activeTime)
    {
      string displayFormat = string.Empty;
      if (activeTime.Days > 0)
        displayFormat = displayFormat + (object) activeTime.Days + Resources.WEBCODE_PS0_30 + " ";
      if (activeTime.Hours > 0)
        displayFormat = displayFormat + (object) activeTime.Hours + Resources.WEBCODE_PS0_31 + " ";
      if (activeTime.Minutes > 0)
        displayFormat = displayFormat + (object) activeTime.Minutes + Resources.WEBCODE_PS0_32 + " ";
      return displayFormat;
    }

    public ActiveAlertObjectPage GetPageableActiveAlertObjects(
      PageableActiveAlertObjectRequest request)
    {
      ActiveAlertObjectPage activeAlertObjects = new ActiveAlertObjectPage();
      List<ActiveAlertObject> activeAlertObjectList = new List<ActiveAlertObject>();
      using (IInformationServiceProxy2 iinformationServiceProxy2_1 = this._swisProxyCreator.Create())
      {
        string withViewLimitations = this.GetQueryWithViewLimitations(string.Format("SELECT a.AlertActiveObjects.EntityUri, a.AlertActiveObjects.EntityCaption, a.AlertActiveObjects.EntityDetailsUrl,\r\n                                             a.AcknowledgedBy, a.TriggeredDateTime, MillisecondDiff(a.TriggeredDateTime, getUtcDate()) AS ActiveTime\r\n                                              FROM Orion.AlertActive (nolock=true) AS a\r\n                                              WHERE a.AlertObjectID=@objectId\r\n                                              {0}", !string.IsNullOrEmpty(request.OrderByClause) ? (object) ("ORDER BY " + request.OrderByClause) : (object) ""), this.GetOnlyViewLimitation(request.LimitationIDs));
        IInformationServiceProxy2 iinformationServiceProxy2_2 = iinformationServiceProxy2_1;
        string str1 = withViewLimitations;
        foreach (DataRow row in (InternalDataCollectionBase) InformationServiceProxyExtensions.QueryWithAppendedErrors((IInformationServiceProxy) iinformationServiceProxy2_2, str1, new Dictionary<string, object>()
        {
          {
            "objectId",
            (object) request.AlertObjectId
          }
        }).Rows)
        {
          if (row["EntityUri"] != DBNull.Value)
          {
            string str2 = Convert.ToString(row["EntityUri"]);
            string str3 = row["EntityCaption"] != DBNull.Value ? Convert.ToString(row["EntityCaption"]) : "";
            string str4 = row["EntityDetailsUrl"] != DBNull.Value ? Convert.ToString(row["EntityDetailsUrl"]) : "";
            ActiveAlertObject activeAlertObject = new ActiveAlertObject()
            {
              Caption = str3,
              Uri = str2,
              DetailsUrl = str4,
              Entity = request.TriggeringEntityName
            };
            activeAlertObjectList.Add(activeAlertObject);
          }
        }
      }
      this.FillAlertObjectStatus(request.TriggeringEntityName, activeAlertObjectList);
      activeAlertObjects.TotalRow = activeAlertObjectList.Count;
      List<ActiveAlertObject> list = activeAlertObjectList.Skip<ActiveAlertObject>(request.StartRow).Take<ActiveAlertObject>(request.PageSize).ToList<ActiveAlertObject>();
      activeAlertObjects.ActiveAlertObjects = (IEnumerable<ActiveAlertObject>) list;
      return activeAlertObjects;
    }

    private void FillAlertObjectStatus(string entity, List<ActiveAlertObject> objects)
    {
      if (!objects.Any<ActiveAlertObject>() || !this.EntityHasStatusProperty(entity))
        return;
      Dictionary<string, ActiveAlertObject> lookupUri = objects.ToDictionary<ActiveAlertObject, string, ActiveAlertObject>((System.Func<ActiveAlertObject, string>) (item => item.Uri), (System.Func<ActiveAlertObject, ActiveAlertObject>) (item => item));
      StringBuilder sbCondition = new StringBuilder();
      Action<Dictionary<string, object>> action = (Action<Dictionary<string, object>>) (swqlParameters =>
      {
        string str = string.Format("SELECT Status, Uri FROM {0} (nolock=true) WHERE {1}", (object) entity, (object) sbCondition);
        using (IInformationServiceProxy2 iinformationServiceProxy2 = this._swisProxyCreator.Create())
        {
          foreach (DataRow row in (InternalDataCollectionBase) InformationServiceProxyExtensions.QueryWithAppendedErrors((IInformationServiceProxy) iinformationServiceProxy2, str, swqlParameters).Rows)
          {
            int int32 = row["Status"] != DBNull.Value ? Convert.ToInt32(row["Status"]) : 0;
            ActiveAlertObject activeAlertObject;
            if (lookupUri.TryGetValue(row["Uri"] != DBNull.Value ? Convert.ToString(row["Uri"]) : "", out activeAlertObject))
              activeAlertObject.Status = int32;
          }
        }
      });
      int num = 0;
      Dictionary<string, object> dictionary = new Dictionary<string, object>();
      foreach (ActiveAlertObject activeAlertObject in objects)
      {
        if (num > 0)
          sbCondition.Append(" OR ");
        sbCondition.AppendFormat("Uri=@uri{0}", (object) num);
        dictionary.Add(string.Format("uri{0}", (object) num), (object) activeAlertObject.Uri);
        ++num;
        if (num % 1000 == 0)
        {
          action(dictionary);
          dictionary = new Dictionary<string, object>();
          sbCondition.Clear();
          num = 0;
        }
      }
      if (num <= 0)
        return;
      action(dictionary);
    }

    private bool UpdateAlertCaptionAfterReset(int alertObjectId, SqlTransaction transaction)
    {
      string str1;
      using (SqlCommand textCommand = SqlHelper.GetTextCommand("SELECT TOP 1 [EntityType] FROM [AlertObjects]\r\n                WHERE [AlertObjectID] = @alertObjectId\r\n                AND [EntityUri] IS NULL"))
      {
        textCommand.Parameters.AddWithValue(nameof (alertObjectId), (object) alertObjectId);
        object obj = SqlHelper.ExecuteScalar(textCommand, transaction.Connection, transaction);
        if (obj == DBNull.Value)
          return false;
        str1 = obj as string;
        if (string.IsNullOrWhiteSpace(str1))
          return false;
      }
      string str2 = "SELECT DisplayNamePlural FROM Metadata.Entity WHERE FullName = @entityName";
      string str3;
      using (IInformationServiceProxy2 iinformationServiceProxy2 = this._swisProxyCreator.Create())
      {
        DataTable dataTable = InformationServiceProxyExtensions.QueryWithAppendedErrors((IInformationServiceProxy) iinformationServiceProxy2, str2, new Dictionary<string, object>()
        {
          {
            "entityName",
            (object) str1
          }
        });
        if (dataTable.Rows.Count <= 0)
          return false;
        str3 = string.Format("{0} {1}", (object) 0, (object) (dataTable.Rows[0][0] as string));
      }
      using (SqlCommand textCommand = SqlHelper.GetTextCommand(string.Format("UPDATE [AlertObjects] SET EntityCaption = '{0}' \r\n                                                                           WHERE EntityUri IS NULL AND AlertObjectID = @alertObjectId", (object) str3)))
      {
        textCommand.Parameters.AddWithValue(nameof (alertObjectId), (object) alertObjectId);
        return SqlHelper.ExecuteNonQuery(textCommand, transaction.Connection, transaction) == 1;
      }
    }

    public string GetAlertNote(int alertObjectId)
    {
      using (SqlConnection connection = DatabaseFunctions.CreateConnection())
      {
        SqlCommand textCommand = SqlHelper.GetTextCommand("SELECT AlertNote FROM AlertObjects WHERE AlertObjectID = @alertObjectId");
        textCommand.Parameters.AddWithValue("@alertObjectId", (object) alertObjectId);
        DataTable dataTable = SqlHelper.ExecuteDataTable(textCommand, connection, (DataTable) null);
        return dataTable.Rows.Count > 0 ? dataTable.Rows[0][0] as string : string.Empty;
      }
    }

    public bool SetAlertNote(
      int alertObjectId,
      string accountId,
      string note,
      DateTime modificationDateTime)
    {
      if (alertObjectId < 0 || string.IsNullOrEmpty(accountId))
        return false;
      using (SqlConnection connection = DatabaseFunctions.CreateConnection())
      {
        using (SqlTransaction sqlTransaction = connection.BeginTransaction(IsolationLevel.ReadCommitted))
        {
          using (SqlCommand textCommand = SqlHelper.GetTextCommand("UPDATE AlertObjects SET AlertNote = @AlertNote WHERE AlertObjectId=@alertObjectId"))
          {
            textCommand.Parameters.AddWithValue("@alertObjectId", (object) alertObjectId);
            textCommand.Parameters.AddWithValue("@AlertNote", (object) note);
            SqlHelper.ExecuteNonQuery(textCommand, connection, sqlTransaction);
            textCommand.CommandText = "SELECT AlertActiveId FROM AlertActive WHERE AlertObjectId=@alertObjectId";
            object obj = SqlHelper.ExecuteScalar(textCommand, connection);
            AlertHistory alertHistory = new AlertHistory();
            int num = 0;
            if (obj != null && obj != DBNull.Value)
              num = Convert.ToInt32(obj);
            alertHistory.EventType = (EventType) 3;
            alertHistory.AccountID = accountId;
            alertHistory.Message = note;
            alertHistory.TimeStamp = modificationDateTime.ToUniversalTime();
            this._alertHistoryDAL.InsertHistoryItem(alertHistory, (long) num, alertObjectId, connection, sqlTransaction);
          }
          sqlTransaction.Commit();
          return true;
        }
      }
    }
  }
}
