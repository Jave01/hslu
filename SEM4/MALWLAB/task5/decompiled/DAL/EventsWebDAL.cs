// Decompiled with JetBrains decompiler
// Type: SolarWinds.Orion.Core.BusinessLayer.DAL.EventsWebDAL
// Assembly: SolarWinds.Orion.Core.BusinessLayer, Version=2019.4.5200.9083, Culture=neutral, PublicKeyToken=null
// MVID: E12E8C85-5CD9-4E06-8801-182E5104FADE
// Assembly location: E:\task5.dll

using SolarWinds.Orion.Common;
using SolarWinds.Orion.Core.Common;
using SolarWinds.Orion.Core.Models.Events;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;

#nullable disable
namespace SolarWinds.Orion.Core.BusinessLayer.DAL
{
  public class EventsWebDAL
  {
    public static DataTable GetEventTypesTable()
    {
      DataTable eventTypesTable = (DataTable) null;
      using (SqlCommand textCommand = SqlHelper.GetTextCommand(Limitation.LimitSQL("SELECT DISTINCT Name, EventType FROM EventTypes ORDER BY Name", Array.Empty<Limitation>())))
        eventTypesTable = SqlHelper.ExecuteDataTable(textCommand);
      eventTypesTable.TableName = "EventTypesTable";
      return eventTypesTable;
    }

    public static DataTable GetEvents(GetEventsParameter param)
    {
      if (param == null)
        throw new ArgumentNullException(nameof (param));
      DataTable events = (DataTable) null;
      StringBuilder stringBuilder = new StringBuilder();
      using (SqlCommand textCommand = SqlHelper.GetTextCommand(string.Empty))
      {
        stringBuilder.Append("SELECT TOP (@maxRecords) NetObjectType, NetObjectID, NetObjectID2, NetObjectValue, EventID, Acknowledged, EventTime, \r\n                             Events.EventType as EventType, Message, ISNULL(EventTypes.BackColor, 0) as BackColor, NodesData.NodeID as LinkNodeID\r\n                             FROM Events WITH(NOLOCK)\r\n                             LEFT JOIN NodesData WITH(NOLOCK) ON Events.NetworkNode = NodesData.NodeID\r\n                             LEFT JOIN NodesCustomProperties WITH(NOLOCK) ON Events.NetworkNode = NodesCustomProperties.NodeID\r\n                             LEFT JOIN EventTypes ON Events.EventType = EventTypes.EventType");
        textCommand.Parameters.Add(new SqlParameter("@maxRecords", (object) param.MaxRecords));
        List<string> stringList = new List<string>();
        if (!param.IncludeAcknowledged)
          stringList.Add("Acknowledged='false'");
        if (param.NodeId >= 0)
        {
          stringList.Add(" Events.NetworkNode=@nodeID");
          textCommand.Parameters.Add(new SqlParameter("@nodeID", (object) param.NodeId));
        }
        if (param.NetObjectId >= 0)
        {
          stringList.Add(" Events.NetObjectID=@netObjectID");
          textCommand.Parameters.Add(new SqlParameter("@netObjectID", (object) param.NetObjectId));
        }
        if (!string.IsNullOrEmpty(param.NetObjectType))
        {
          stringList.Add(" Events.NetObjectType Like @netObjectType");
          SqlParameterCollection parameters = textCommand.Parameters;
          SqlParameter sqlParameter = new SqlParameter("@netObjectType", SqlDbType.VarChar, 10);
          sqlParameter.Value = (object) param.NetObjectType;
          parameters.Add(sqlParameter);
        }
        if (!string.IsNullOrEmpty(param.DeviceType))
        {
          stringList.Add(" (NodesData.MachineType Like @deviceType)");
          textCommand.Parameters.Add(new SqlParameter("@deviceType", (object) param.DeviceType));
        }
        if (param.EventType > 0)
        {
          stringList.Add(" Events.EventType=@eventType");
          textCommand.Parameters.Add(new SqlParameter("@eventType", (object) param.EventType.ToString()));
        }
        if (param.FromDate.HasValue && param.ToDate.HasValue)
        {
          stringList.Add(" EventTime >= @fromDate AND EventTime <= @toDate");
          textCommand.Parameters.Add(new SqlParameter("@fromDate", (object) param.FromDate));
          textCommand.Parameters.Add(new SqlParameter("@toDate", (object) param.ToDate));
        }
        if (stringList.Count > 0)
        {
          stringBuilder.Append(" WHERE");
          if (stringList.Count == 1)
          {
            stringBuilder.AppendFormat(" {0}", (object) stringList[0]);
          }
          else
          {
            stringBuilder.AppendFormat(" {0}", (object) stringList[0]);
            for (int index = 1; index < stringList.Count; ++index)
              stringBuilder.AppendFormat(" AND {0}", (object) stringList[index]);
          }
        }
        stringBuilder.Append(" ORDER BY EventID DESC");
        textCommand.CommandText = Limitation.LimitSQL(stringBuilder.ToString(), (IEnumerable<int>) param.LimitationIDs);
        events = SqlHelper.ExecuteDataTable(textCommand);
      }
      events.TableName = "EventsTable";
      return events;
    }

    public static DataTable GetEventsTable(GetEventsParameter param)
    {
      if (param.NetObjectType.Equals("N", StringComparison.OrdinalIgnoreCase))
      {
        param.NodeId = param.NetObjectId;
        param.NetObjectId = -1;
        param.NetObjectType = string.Empty;
        return EventsWebDAL.GetEvents(param);
      }
      param.NodeId = -1;
      return EventsWebDAL.GetEvents(param);
    }

    public static void AcknowledgeEvents(List<int> events)
    {
      if (events == null)
        return;
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.Append("UPDATE Events SET Acknowledged='true' WHERE EventID IN (");
      for (int index = 0; index < events.Count - 1; ++index)
        stringBuilder.AppendFormat("{0}, ", (object) events[index]);
      stringBuilder.AppendFormat("{0} )", (object) events[events.Count - 1]);
      using (SqlCommand textCommand = SqlHelper.GetTextCommand(stringBuilder.ToString()))
        SqlHelper.ExecuteNonQuery(textCommand);
    }

    public static DataTable GetEventSummaryTable(
      int netObjectID,
      string netObjectType,
      DateTime fromDate,
      DateTime toDate,
      List<int> limitationIDs)
    {
      DataTable eventSummaryTable = (DataTable) null;
      using (SqlCommand textCommand = SqlHelper.GetTextCommand(string.Empty))
      {
        StringBuilder stringBuilder = new StringBuilder("SELECT Events.EventType as EventType, EventTypes.Name as Name, EventTypes.BackColor as BackColor, Count(Events.EventType) as Total  \r\n                                                        FROM EventTypes \r\n                                                        INNER JOIN Events WITH(NOLOCK) ON EventTypes.EventType = Events.EventType\r\n                                                        LEFT JOIN NodesData WITH(NOLOCK) ON Events.NetworkNode = NodesData.NodeID \r\n                                                        LEFT JOIN NodesCustomProperties WITH(NOLOCK) ON Events.NetworkNode = NodesCustomProperties.NodeID \r\n                                                        WHERE Events.Acknowledged='false' \r\n                                                        AND Events.EventTime >= @fromDate AND Events.EventTime <= @toDate ");
        textCommand.Parameters.Add(new SqlParameter("@fromDate", (object) fromDate));
        textCommand.Parameters.Add(new SqlParameter("@toDate", (object) toDate));
        if (netObjectID >= 0 && !string.IsNullOrEmpty(netObjectType) && netObjectType.Equals("N", StringComparison.OrdinalIgnoreCase))
        {
          stringBuilder.Append(" AND ((Events.NetObjectID=@netObjectID AND Events.NetObjectType LIKE @netObjectType) OR Events.NetworkNode=@netObjectID)");
          textCommand.Parameters.Add(new SqlParameter("@netObjectID", (object) netObjectID));
          textCommand.Parameters.Add(new SqlParameter("@netObjectType", (object) netObjectType));
        }
        else
        {
          if (netObjectID >= 0)
          {
            stringBuilder.Append(" AND Events.NetObjectID=@netObjectID");
            textCommand.Parameters.Add(new SqlParameter("@netObjectID", (object) netObjectID));
          }
          if (!string.IsNullOrEmpty(netObjectType))
          {
            stringBuilder.Append(" AND Events.NetObjectType LIKE @netObjectType");
            SqlParameterCollection parameters = textCommand.Parameters;
            SqlParameter sqlParameter = new SqlParameter("@netObjectType", SqlDbType.VarChar, 10);
            sqlParameter.Value = (object) netObjectType;
            parameters.Add(sqlParameter);
          }
        }
        stringBuilder.Append(" GROUP BY Events.EventType, EventTypes.Name, EventTypes.BackColor ORDER BY Events.EventType");
        textCommand.CommandText = Limitation.LimitSQL(stringBuilder.ToString(), (IEnumerable<int>) limitationIDs);
        eventSummaryTable = SqlHelper.ExecuteDataTable(textCommand);
      }
      eventSummaryTable.TableName = "EventSummaryTable";
      return eventSummaryTable;
    }
  }
}
