// Decompiled with JetBrains decompiler
// Type: SolarWinds.Orion.Core.BusinessLayer.DAL.OrionMessagesDAL
// Assembly: SolarWinds.Orion.Core.BusinessLayer, Version=2019.4.5200.9083, Culture=neutral, PublicKeyToken=null
// MVID: E12E8C85-5CD9-4E06-8801-182E5104FADE
// Assembly location: E:\task5.dll

using SolarWinds.Common.Extensions;
using SolarWinds.Orion.Core.Common;
using SolarWinds.Orion.Core.Common.InformationService;
using SolarWinds.Orion.Core.Common.Models;
using SolarWinds.Orion.Core.Common.Swis;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

#nullable disable
namespace SolarWinds.Orion.Core.BusinessLayer.DAL
{
  internal class OrionMessagesDAL
  {
    private static readonly IInformationServiceProxyCreator creator = (IInformationServiceProxyCreator) SwisConnectionProxyPool.GetCreator();
    private const string AdvancedAlertId = "AA-";

    public static DataTable GetOrionMessagesTable(OrionMessagesFilter filter)
    {
      if (!filter.IncludeAlerts && !filter.IncludeEvents && !filter.IncludeSyslogs && !filter.IncludeTraps && !filter.IncludeAudits)
        return (DataTable) null;
      string str = "SELECT MsgID, DateTime, MessageType, Icon, Message, ObjectType,\r\nObjectID, ObjectID2, NetObjectValue, IPAddress, Caption, BackColor, \r\nAcknowledged, ActiveNetObject, NetObjectPrefix, SiteId, SiteName FROM";
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.AppendFormat("SELECT TOP {0}\r\nMsgID, DateTime, MessageType, Icon, Message, ObjectType,\r\nObjectID, ObjectID2, NetObjectValue, IPAddress, Caption, BackColor, \r\nAcknowledged, ActiveNetObject, NetObjectPrefix, SiteId, SiteName FROM (", (object) filter.Count);
      string format = " {1} ( {0} )";
      bool isNodeId = false;
      bool isDeviceType = false;
      bool isVendor = false;
      bool isIp_address = false;
      bool isHostname = false;
      bool isSiteId = false;
      int num = 0;
      if (!string.IsNullOrEmpty(filter.AlertType))
      {
        string s = filter.AlertType;
        if (s.StartsWith("AA-"))
          s = s.Substring("AA-".Length);
        int result;
        if (int.TryParse(s, out result))
          num = result;
      }
      if (filter.NodeId.HasValue)
        isNodeId = true;
      else if (!string.IsNullOrEmpty(filter.DeviceType))
        isDeviceType = true;
      else if (!string.IsNullOrEmpty(filter.Vendor))
        isVendor = true;
      else if (!string.IsNullOrEmpty(filter.IpAddress))
        isIp_address = true;
      else if (!string.IsNullOrEmpty(filter.Hostname))
        isHostname = true;
      if (filter.SiteID.HasValue)
        isSiteId = true;
      if (filter.IncludeAlerts)
      {
        stringBuilder.AppendFormat(format, (object) OrionMessagesDAL.GetNewAlertsSwql(isNodeId, isDeviceType, isVendor, isIp_address, isHostname, isSiteId, num > 0, !string.IsNullOrEmpty(filter.SearchString), filter.ShowAcknowledged, filter.AlertCategoryLimitation, filter.Count), (object) str);
        format = " UNION ( {1} ( {0} ) ) ";
      }
      if (filter.IncludeAudits)
      {
        stringBuilder.AppendFormat(format, (object) OrionMessagesDAL.GetAuditSwql(isNodeId, isDeviceType, isVendor, isIp_address, isHostname, isSiteId, filter), (object) str);
        format = " UNION ( {1} ( {0} ) ) ";
      }
      if (filter.IncludeEvents)
      {
        stringBuilder.AppendFormat(format, (object) OrionMessagesDAL.GetEventsSwql(isNodeId, isDeviceType, isVendor, isIp_address, isHostname, isSiteId, !string.IsNullOrEmpty(filter.EventType), !string.IsNullOrEmpty(filter.SearchString), filter.ShowAcknowledged, filter.Count), (object) str);
        format = " UNION ( {1} ( {0} ) ) ";
      }
      if (filter.IncludeSyslogs)
      {
        stringBuilder.AppendFormat(format, (object) OrionMessagesDAL.GetSyslogSwql(isNodeId, isDeviceType, isVendor, isIp_address, isHostname, isSiteId, filter.SyslogSeverity < byte.MaxValue, filter.SyslogFacility < byte.MaxValue, !string.IsNullOrEmpty(filter.SearchString), filter.ShowAcknowledged, filter.Count), (object) str);
        format = " UNION ( {1} ( {0} ) ) ";
      }
      if (filter.IncludeTraps)
        stringBuilder.AppendFormat(format, (object) OrionMessagesDAL.GetTrapsSwql(isNodeId, isDeviceType, isVendor, isIp_address, isHostname, isSiteId, !string.IsNullOrEmpty(filter.TrapType), !string.IsNullOrEmpty(filter.TrapCommunity), !string.IsNullOrEmpty(filter.SearchString), filter.ShowAcknowledged, filter.Count), (object) str);
      stringBuilder.AppendLine(")a ORDER BY a.DateTime DESC");
      using (IInformationServiceProxy2 iinformationServiceProxy2 = OrionMessagesDAL.creator.Create())
      {
        Dictionary<string, object> dictionary = new Dictionary<string, object>();
        dictionary.Add("fromDate", (object) filter.FromDate);
        dictionary.Add("toDate", (object) filter.ToDate);
        if (isNodeId)
          dictionary.Add("nodeId", (object) filter.NodeId);
        if (isDeviceType)
          dictionary.Add("deviceType", (object) filter.DeviceType);
        if (isVendor)
          dictionary.Add("vendor", (object) filter.Vendor);
        if (isIp_address)
          dictionary.Add("ip_address", (object) string.Format("%{0}%", (object) CommonHelper.FormatFilter(IPAddressHelper.ToStringIp(filter.IpAddress))));
        if (isHostname)
          dictionary.Add("hostname", (object) string.Format("%{0}%", (object) CommonHelper.FormatFilter(filter.Hostname)));
        if (isSiteId)
          dictionary.Add("siteId", (object) filter.SiteID);
        if (!string.IsNullOrEmpty(filter.EventType))
          dictionary.Add("event_type", (object) filter.EventType);
        if (filter.SyslogSeverity < byte.MaxValue)
          dictionary.Add("syslog_severity", (object) filter.SyslogSeverity);
        if (filter.SyslogFacility < byte.MaxValue)
          dictionary.Add("syslog_facility", (object) filter.SyslogFacility);
        if (!string.IsNullOrEmpty(filter.TrapType))
          dictionary.Add("trap_type", (object) filter.TrapType);
        if (!string.IsNullOrEmpty(filter.TrapCommunity))
          dictionary.Add("trap_community", (object) filter.TrapCommunity);
        if (filter.IncludeAlerts && num > 0)
          dictionary.Add("newAlert_id", (object) num);
        if (filter.IncludeAlerts && !string.IsNullOrEmpty(filter.AlertCategoryLimitation))
          dictionary.Add("alertCategoryLimitation", (object) filter.AlertCategoryLimitation);
        if (!string.IsNullOrWhiteSpace(filter.AuditType))
          dictionary.Add("actionTypeId", (object) int.Parse(filter.AuditType));
        if (!string.IsNullOrWhiteSpace(filter.Audituser))
          dictionary.Add("accountId", (object) filter.Audituser);
        if (!string.IsNullOrEmpty(filter.SearchString))
          dictionary.Add("search_str", (object) string.Format("%{0}%", (object) filter.SearchString));
        DataTable orionMessagesTable = InformationServiceProxyExtensions.QueryWithAppendedErrors((IInformationServiceProxy) iinformationServiceProxy2, stringBuilder.ToString(), dictionary, SwisFederationInfo.IsFederationEnabled);
        orionMessagesTable.TableName = "OrionMessages";
        IEnumerableExtensions.Iterate<DataRow>(orionMessagesTable.Rows.OfType<DataRow>(), (Action<DataRow>) (r => r["DateTime"] = (object) ((DateTime) r["DateTime"]).ToLocalTime()));
        return orionMessagesTable;
      }
    }

    private static string GetEventsSwql(
      bool isNodeId,
      bool isDeviceType,
      bool isVendor,
      bool isIp_address,
      bool isHostname,
      bool isSiteId,
      bool isEventType,
      bool isSearchString,
      bool showAck,
      int maxRowCount)
    {
      StringBuilder stringBuilder = new StringBuilder("SELECT TOP {1}\r\n\tTOSTRING(e.EventID) AS MsgID,\r\n\te.EventTime AS DateTime, \r\n\t'{0}' AS MessageType,\r\n\tTOSTRING(e.EventType) AS Icon,\r\n\te.Message AS Message,\r\n\te.NetObjectType AS ObjectType, \r\n\tTOSTRING(e.NetObjectID) AS ObjectID, \r\n\t'' AS ObjectID2,\r\n    e.NetObjectValue AS NetObjectValue,\r\n\tn.IP_Address AS IPAddress,\r\n\t'' AS Caption,\r\n\tISNULL(et.BackColor, 0) AS BackColor,\r\n\te.Acknowledged AS Acknowledged,\r\n\tTOSTRING(e.NetObjectID) AS ActiveNetObject,\r\n\te.NetObjectType AS NetObjectPrefix,\r\n    e.InstanceSiteId AS SiteId,\r\n    s.Name AS SiteName\r\nFROM Orion.Events (nolock=true) AS e\r\nLEFT JOIN Orion.Nodes (nolock=true) AS n ON e.NetworkNode = n.NodeID AND e.InstanceSiteId = n.InstanceSiteId\r\nLEFT JOIN Orion.EventTypes (nolock=true) AS et ON e.EventType = et.EventType AND e.InstanceSiteId = et.InstanceSiteId\r\nLEFT JOIN Orion.Sites (nolock=true) AS s ON s.SiteID = e.InstanceSiteId\r\nWHERE e.EventTime >= @fromDate AND e.EventTime <= @toDate ");
      if (isNodeId)
        stringBuilder.Append(" AND e.NetworkNode=@nodeId ");
      if (!showAck)
        stringBuilder.Append(" AND e.Acknowledged=0 ");
      if (isDeviceType)
        stringBuilder.Append(" AND n.MachineType Like @deviceType ");
      if (isVendor)
        stringBuilder.Append(" AND n.Vendor = @vendor ");
      if (isIp_address)
        stringBuilder.Append(" AND n.IP_Address Like @ip_address ");
      if (isHostname)
        stringBuilder.Append(" AND (n.SysName Like @hostname OR n.Caption Like @hostname) ");
      if (isEventType)
        stringBuilder.Append(" AND e.EventType = @event_type ");
      if (isSearchString)
        stringBuilder.Append(" AND e.Message Like @search_str ");
      if (isSiteId)
        stringBuilder.Append(" AND e.InstanceSiteId = @siteId ");
      stringBuilder.Append(" ORDER BY e.EventTime DESC");
      return string.Format(stringBuilder.ToString(), (object) OrionMessagesHelper.GetMessageTypeString((OrionMessageType) 2), (object) maxRowCount);
    }

    private static string GetSyslogSwql(
      bool isNodeId,
      bool isDeviceType,
      bool isVendor,
      bool isIp_address,
      bool isHostname,
      bool isSiteId,
      bool isSeverityCode,
      bool isFacilityCode,
      bool isSearchString,
      bool showAck,
      int maxRowCount)
    {
      StringBuilder stringBuilder = new StringBuilder("SELECT TOP {1}\r\n\tTOSTRING(s.MessageID) AS MsgID,\r\n\ts.DateTime AS DateTime,\r\n\t'{0}' AS MessageType,\r\n\tTOSTRING(s.SysLogSeverity) AS Icon,\r\n\ts.Message AS Message,\r\n\t'N' AS ObjectType, \r\n\tTOSTRING(s.NodeID) AS ObjectID, \r\n\t'' AS ObjectID2,\r\n    '' AS NetObjectValue,\r\n\ts.IPAddress AS IPAddress,\r\n\ts.Hostname AS Caption,\r\n\ts.SysLogSeverity*1 AS BackColor,\r\n\ts.Acknowledged AS Acknowledged,\r\n\tTOSTRING(s.NodeID) AS ActiveNetObject,\r\n\t'N' AS NetObjectPrefix,\r\n    s.InstanceSiteId AS SiteId,\r\n    os.Name AS SiteName\r\nFROM Orion.Syslog (nolock=true) AS s\r\nLEFT JOIN Orion.Nodes (nolock=true) AS n ON s.NodeID = n.NodeID AND s.InstanceSiteId = n.InstanceSiteId\r\nLEFT JOIN Orion.Sites (nolock=true) AS os ON s.InstanceSiteId = os.SiteID\r\nWHERE s.DateTime >= @fromDate AND s.DateTime <= @toDate");
      if (isNodeId)
        stringBuilder.Append(" AND s.NodeID=@nodeId ");
      if (!showAck)
        stringBuilder.Append(" AND s.Acknowledged=0 ");
      if (isDeviceType)
        stringBuilder.Append(" AND n.MachineType Like @deviceType ");
      if (isVendor)
        stringBuilder.Append(" AND n.Vendor = @vendor ");
      if (isIp_address)
        stringBuilder.Append(" AND s.IPAddress Like @ip_address ");
      if (isHostname)
        stringBuilder.Append(" AND (s.Hostname Like @hostname OR n.Caption Like @hostname) ");
      if (isSeverityCode)
        stringBuilder.Append(" AND s.SysLogSeverity = @syslog_severity ");
      if (isFacilityCode)
        stringBuilder.Append(" AND s.SysLogFacility = @syslog_facility ");
      if (isSearchString)
        stringBuilder.Append(" AND s.Message Like @search_str ");
      if (isSiteId)
        stringBuilder.Append(" AND s.InstanceSiteId = @siteId ");
      stringBuilder.Append(" ORDER BY s.DateTime DESC");
      return string.Format(stringBuilder.ToString(), (object) OrionMessagesHelper.GetMessageTypeString((OrionMessageType) 3), (object) maxRowCount);
    }

    private static string GetTrapsSwql(
      bool isNodeId,
      bool isDeviceType,
      bool isVendor,
      bool isIp_address,
      bool isHostname,
      bool isSiteId,
      bool isTrapType,
      bool isCommunity,
      bool isSearchString,
      bool showAck,
      int maxRowCount)
    {
      StringBuilder stringBuilder = new StringBuilder("Select TOP {1}\r\n\tTOSTRING(t.TrapID) AS MsgID,\r\n\tt.DateTime AS DateTime,\r\n\t'{0}' AS MessageType,\r\n\t'' AS Icon,\r\n\tt.Message AS Message,\r\n\t'N' AS ObjectType, \r\n\tTOSTRING(t.NodeID) AS ObjectID, \r\n\t'' AS ObjectID2,\r\n    '' AS NetObjectValue,\r\n\tt.IPAddress AS IPAddress,\r\n\tt.Hostname AS Caption,\r\n\tt.ColorCode AS BackColor,\r\n\tt.Acknowledged AS Acknowledged,\r\n\tTOSTRING(t.NodeID) AS ActiveNetObject,\r\n\t'N' AS NetObjectPrefix,\r\n    t.InstanceSiteId AS SiteId,\r\n    s.Name AS SiteName\r\nFROM Orion.Traps (nolock=true) AS t\r\nLEFT JOIN Orion.Nodes (nolock=true) AS n ON t.NodeID = n.NodeID AND t.InstanceSiteId = n.InstanceSiteId\r\nLEFT JOIN Orion.Sites (nolock=true) AS s ON t.InstanceSiteId = s.SiteID\r\nWHERE t.DateTime >= @fromDate AND t.DateTime <= @toDate");
      if (isNodeId)
        stringBuilder.Append(" AND t.NodeID=@nodeId ");
      if (!showAck)
        stringBuilder.Append(" AND t.Acknowledged=0 ");
      if (isDeviceType)
        stringBuilder.Append(" AND n.MachineType Like @deviceType ");
      if (isVendor)
        stringBuilder.Append(" AND n.Vendor = @vendor ");
      if (isIp_address)
        stringBuilder.Append(" AND t.IPAddress Like @ip_address ");
      if (isTrapType)
        stringBuilder.Append(" AND t.TrapType Like @trap_type ");
      if (isHostname)
        stringBuilder.Append(" AND (t.Hostname Like @hostname OR n.Caption Like @hostname) ");
      if (isCommunity)
        stringBuilder.Append(" AND t.Community Like @trap_community ");
      if (isSearchString)
        stringBuilder.Append(" AND t.Message Like @search_str ");
      if (isSiteId)
        stringBuilder.Append(" AND t.InstanceSiteId = @siteId ");
      stringBuilder.Append(" ORDER BY t.DateTime DESC");
      return string.Format(stringBuilder.ToString(), (object) OrionMessagesHelper.GetMessageTypeString((OrionMessageType) 4), (object) maxRowCount);
    }

    private static string GetNewAlertsSwql(
      bool isNodeId,
      bool isDeviceType,
      bool isVendor,
      bool isIp_address,
      bool isHostname,
      bool isSiteId,
      bool isAlertID,
      bool isSearchString,
      bool showAck,
      string alertCategoryLimitation,
      int maxRowCount)
    {
      StringBuilder stringBuilder = new StringBuilder("\r\nSELECT TOP {0}\r\n\t'AA-' + TOSTRING(aa.AlertActiveID) AS MsgID,\r\n\taa.TriggeredDateTime AS DateTime,\r\n\t'Advanced Alert' AS MessageType,\r\n\t'' AS Icon,\r\n\taa.TriggeredMessage AS Message,\r\n\t'AAT' AS ObjectType,\r\n\tTOSTRING(aa.AlertObjectID) AS ObjectID,\r\n\t'' AS ObjectID2,\r\n    '' AS NetObjectValue,\r\n\tn.IP_Address AS IPAddress,\r\n\tao.EntityCaption AS Caption,\r\n\tCASE ac.Severity\r\n\t\tWHEN 1 THEN 15658734\r\n\t\tWHEN 2 THEN 16638651\r\n\t\tELSE 16300735\r\n\tEND AS BackColor,\r\n\tCASE \r\n\t\tWHEN aa.Acknowledged IS NULL THEN 0\r\n\t\tELSE 1\r\n\tEND AS Acknowledged,\r\n\t'' AS ActiveNetObject,\r\n\tao.EntityDetailsUrl AS NetObjectPrefix,\r\n    aa.InstanceSiteId AS SiteId,\r\n    s.Name AS SiteName\r\nFROM Orion.AlertActive (nolock=true) AS aa\r\nINNER JOIN Orion.AlertObjects (nolock=true) AS ao ON aa.AlertObjectID = ao.AlertObjectID AND aa.InstanceSiteId = ao.InstanceSiteId\r\nLEFT JOIN Orion.AlertConfigurations (nolock=true) AS ac ON ao.AlertID = ac.AlertID AND ao.InstanceSiteId = ac.InstanceSiteId\r\nLEFT JOIN Orion.Nodes (nolock=true) AS n ON ao.RelatedNodeId = n.NodeID AND ao.InstanceSiteId = n.InstanceSiteId\r\nLEFT JOIN Orion.Sites (nolock=true) AS s ON s.SiteID = aa.InstanceSiteId\r\nWHERE aa.TriggeredDateTime >= @fromDate AND aa.TriggeredDateTime <= @toDate");
      if (isNodeId)
        stringBuilder.Append(" AND n.NodeID=@nodeId ");
      if (isDeviceType)
        stringBuilder.Append(" AND n.MachineType Like @deviceType ");
      if (isVendor)
        stringBuilder.Append(" AND n.Vendor = @vendor ");
      if (isIp_address)
        stringBuilder.Append(" AND n.IP_Address Like @ip_address ");
      if (isHostname)
        stringBuilder.Append(" AND (n.SysName Like @hostname OR n.Caption Like @hostname) ");
      if (isAlertID)
        stringBuilder.Append(" AND ac.AlertID = @newAlert_id ");
      if (isSearchString)
        stringBuilder.Append(" AND ac.Name LIKE @search_str ");
      if (!showAck)
        stringBuilder.Append(" AND aa.Acknowledged IS NULL ");
      if (!string.IsNullOrEmpty(alertCategoryLimitation))
        stringBuilder.Append(" AND ac.Category=@alertCategoryLimitation ");
      if (isSiteId)
        stringBuilder.Append(" AND aa.InstanceSiteId = @siteId ");
      stringBuilder.Append(" ORDER BY aa.TriggeredDateTime DESC");
      return string.Format(stringBuilder.ToString(), (object) maxRowCount);
    }

    private static string GetAuditSwql(
      bool isNodeId,
      bool isDeviceType,
      bool isVendor,
      bool isIp_address,
      bool isHostname,
      bool isSiteId,
      OrionMessagesFilter filter)
    {
      StringBuilder stringBuilder1 = new StringBuilder("\r\nSELECT TOP {1}\r\n    TOSTRING(a.AuditEventID) AS MsgID,\r\n    a.TimeLoggedUtc AS DateTime,\r\n    '{0}' AS MessageType,\r\n    '' AS Icon, \r\n    a.AuditEventMessage AS Message, \r\n\ta.NetObjectType AS ObjectType, \r\n\tTOSTRING(a.NetObjectID) AS ObjectID, \r\n    '' AS ObjectID2, \r\n    '' AS NetObjectValue,\r\n    '' AS IPAddress, \r\n    '' AS Caption,\r\n    0 AS BackColor,\r\n    0 AS Acknowledged,\r\n\tTOSTRING(a.NetObjectID) AS ActiveNetObject,\r\n\ta.NetObjectType AS NetObjectPrefix,\r\n    a.InstanceSiteId AS SiteId,\r\n    s.Name AS SiteName\r\nFROM Orion.AuditingEvents (nolock=true) AS a\r\n{3}\r\nLEFT JOIN Orion.Sites (nolock=true) AS s ON a.InstanceSiteId = s.SiteID\r\nWHERE \r\n    a.TimeLoggedUtc >= @fromDate AND a.TimeLoggedUtc <= @toDate\r\n    {2} \r\n ORDER BY a.TimeLoggedUtc DESC");
      bool flag = false;
      StringBuilder stringBuilder2 = new StringBuilder();
      if (isNodeId)
        stringBuilder2.Append(" AND a.NetworkNode = @nodeId ");
      if (isDeviceType)
      {
        stringBuilder2.Append(" AND n.MachineType Like @deviceType ");
        flag = true;
      }
      if (isVendor)
      {
        stringBuilder2.Append(" AND n.Vendor = @vendor ");
        flag = true;
      }
      if (isIp_address)
      {
        stringBuilder2.Append(" AND n.IP_Address Like @ip_address ");
        flag = true;
      }
      if (isHostname)
      {
        stringBuilder2.Append(" AND (n.SysName Like @hostname OR n.Caption Like @hostname) ");
        flag = true;
      }
      if (!string.IsNullOrWhiteSpace(filter.AuditType))
        stringBuilder2.Append(" AND a.ActionTypeID = @actionTypeId ");
      if (!string.IsNullOrWhiteSpace(filter.Audituser))
        stringBuilder2.Append(" AND a.AccountID LIKE @accountId ");
      if (!string.IsNullOrWhiteSpace(filter.SearchString))
        stringBuilder2.Append(" AND a.AuditEventMessage LIKE @search_str ");
      if (isSiteId)
        stringBuilder2.Append(" AND a.InstanceSiteId = @siteId");
      return string.Format(stringBuilder1.ToString(), (object) OrionMessagesHelper.GetMessageTypeString((OrionMessageType) 5), (object) filter.Count, (object) stringBuilder2, flag ? (object) " LEFT JOIN Orion.Nodes (nolock=true) AS n ON a.NetworkNode = n.NodeID AND a.InstanceSiteId = n.InstanceSiteId " : (object) string.Empty);
    }
  }
}
