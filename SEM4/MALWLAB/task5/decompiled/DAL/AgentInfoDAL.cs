// Decompiled with JetBrains decompiler
// Type: SolarWinds.Orion.Core.BusinessLayer.DAL.AgentInfoDAL
// Assembly: SolarWinds.Orion.Core.BusinessLayer, Version=2019.4.5200.9083, Culture=neutral, PublicKeyToken=null
// MVID: E12E8C85-5CD9-4E06-8801-182E5104FADE
// Assembly location: E:\task5.dll

using SolarWinds.AgentManagement.Contract.Models;
using SolarWinds.InformationService.InformationServiceClient;
using SolarWinds.Orion.Common;
using SolarWinds.Orion.Core.Common;
using SolarWinds.Orion.Core.Common.Agent;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;

#nullable disable
namespace SolarWinds.Orion.Core.BusinessLayer.DAL
{
  public class AgentInfoDAL : IAgentInfoDAL
  {
    public IEnumerable<AgentInfo> GetAgentsInfo()
    {
      return this.GetAgentsInfo((string) null, (IDictionary<string, object>) null);
    }

    public AgentInfo GetAgentInfoByNode(int nodeId)
    {
      return this.GetAgentsInfo("a.NodeID = @NodeID", (IDictionary<string, object>) new Dictionary<string, object>()
      {
        {
          "NodeID",
          (object) nodeId
        }
      }).FirstOrDefault<AgentInfo>();
    }

    public AgentInfo GetAgentInfo(int agentId)
    {
      return this.GetAgentsInfo("a.AgentId = @AgentId", (IDictionary<string, object>) new Dictionary<string, object>()
      {
        {
          "AgentId",
          (object) agentId
        }
      }).FirstOrDefault<AgentInfo>();
    }

    public AgentInfo GetAgentInfoByAgentAddress(string address)
    {
      Guid result;
      if (!Guid.TryParse(address, out result))
        return (AgentInfo) null;
      return this.GetAgentsInfo("a.AgentGuid = @AgentGuid", (IDictionary<string, object>) new Dictionary<string, object>()
      {
        {
          "AgentGuid",
          (object) result
        }
      }).FirstOrDefault<AgentInfo>();
    }

    public AgentInfo GetAgentInfoByIpOrHostname(string ipAddress, string hostname)
    {
      if (string.IsNullOrWhiteSpace(ipAddress) && string.IsNullOrWhiteSpace(hostname))
        throw new ArgumentException("ipAddress or hostname must be specified");
      List<string> values = new List<string>();
      Dictionary<string, object> parameters = new Dictionary<string, object>();
      if (!string.IsNullOrWhiteSpace(ipAddress))
      {
        values.Add("a.IP = @ipAddress");
        parameters.Add(nameof (ipAddress), (object) ipAddress);
      }
      if (!string.IsNullOrWhiteSpace(hostname))
      {
        values.Add("a.Hostname = @hostname");
        parameters.Add(nameof (hostname), (object) hostname);
      }
      return this.GetAgentsInfo(string.Format("({0}) AND (n.ObjectSubType IS NULL OR n.ObjectSubType <> 'Agent')", (object) string.Join(" OR ", (IEnumerable<string>) values)), (IDictionary<string, object>) parameters).FirstOrDefault<AgentInfo>();
    }

    public IEnumerable<AgentInfo> GetAgentsByNodesFilter(int engineId, string nodesWhereClause)
    {
      List<AgentInfo> source = new List<AgentInfo>();
      using (InformationServiceConnection systemConnectionV3 = InformationServiceConnectionProvider.CreateSystemConnectionV3())
      {
        using (InformationServiceCommand command = systemConnectionV3.CreateCommand())
        {
          ((DbCommand) command).CommandText = string.Format("SELECT agents.AgentId, \r\n                            agents.AgentGuid, \r\n                            agents.NodeId, \r\n                            nodes.ObjectSubType,\r\n                            agents.Uri,\r\n                            agents.Name, \r\n                            agents.Hostname, \r\n                            agents.IP, \r\n                            agents.PollingEngineId,\r\n                            agents.AgentStatus, \r\n                            agents.AgentStatusMessage, \r\n                            agents.ConnectionStatus, \r\n                            agents.ConnectionStatusMessage,\r\n                            agents.OSType,\r\n                            agents.OSDistro,\r\n                            p.PluginId, \r\n                            p.Status, \r\n                            p.StatusMessage\r\n                    FROM Orion.AgentManagement.Agent agents\r\n                    INNER JOIN Orion.Nodes Nodes ON Nodes.NodeID = agents.NodeID\r\n                    LEFT JOIN Orion.AgentManagement.AgentPlugin p ON agents.AgentId = p.AgentId \r\n                    WHERE agents.PollingEngineId=@engineId AND agents.ConnectionStatus = @connectionStatus {0}\r\n                    ORDER BY agents.AgentId", !string.IsNullOrWhiteSpace(nodesWhereClause) ? (object) (" AND " + nodesWhereClause) : (object) string.Empty);
          command.Parameters.AddWithValue("connectionStatus", (object) 1);
          command.Parameters.AddWithValue(nameof (engineId), (object) engineId);
          using (IDataReader reader = (IDataReader) command.ExecuteReader())
            source = this.GetAgentsFromReader(reader);
        }
      }
      return source.Where<AgentInfo>((System.Func<AgentInfo, bool>) (x => x.NodeID.HasValue && x.NodeSubType == "Agent"));
    }

    private IEnumerable<AgentInfo> GetAgentsInfo(
      string whereClause,
      IDictionary<string, object> parameters)
    {
      List<AgentInfo> agentInfoList = new List<AgentInfo>();
      using (InformationServiceConnection systemConnectionV3 = InformationServiceConnectionProvider.CreateSystemConnectionV3())
      {
        using (InformationServiceCommand command = systemConnectionV3.CreateCommand())
        {
          ((DbCommand) command).CommandText = string.Format("SELECT a.AgentId, \r\n                            a.AgentGuid, \r\n                            a.NodeId, \r\n                            n.ObjectSubType,\r\n                            a.Uri,\r\n                            a.Name, \r\n                            a.Hostname, \r\n                            a.IP, \r\n                            a.PollingEngineId,\r\n                            a.AgentStatus, \r\n                            a.AgentStatusMessage, \r\n                            a.ConnectionStatus, \r\n                            a.ConnectionStatusMessage,\r\n                            p.PluginId, \r\n                            p.Status, \r\n                            p.StatusMessage,\r\n                            a.OSType,\r\n                            a.OSDistro\r\n                    FROM Orion.AgentManagement.Agent a\r\n                    LEFT JOIN Orion.Nodes n ON n.NodeID = a.NodeID\r\n                    LEFT JOIN Orion.AgentManagement.AgentPlugin p ON a.AgentId = p.AgentId \r\n                    {0}\r\n                    ORDER BY a.AgentId", !string.IsNullOrEmpty(whereClause) ? (object) ("WHERE " + whereClause) : (object) string.Empty);
          if (parameters != null)
          {
            foreach (KeyValuePair<string, object> parameter in (IEnumerable<KeyValuePair<string, object>>) parameters)
              command.Parameters.AddWithValue(parameter.Key, parameter.Value);
          }
          using (IDataReader reader = (IDataReader) command.ExecuteReader())
            return (IEnumerable<AgentInfo>) this.GetAgentsFromReader(reader);
        }
      }
    }

    private List<AgentInfo> GetAgentsFromReader(IDataReader reader)
    {
      List<AgentInfo> agentsFromReader = new List<AgentInfo>();
      AgentInfo agentInfo1 = (AgentInfo) null;
      while (reader.Read())
      {
        OsType result;
        if (!Enum.TryParse<OsType>(DatabaseFunctions.GetString(reader, "OSType"), true, out result))
          result = (OsType) 0;
        AgentInfo agentInfo2 = new AgentInfo()
        {
          AgentId = DatabaseFunctions.GetInt32(reader, "AgentId"),
          AgentGuid = DatabaseFunctions.GetGuid(reader, "AgentGuid"),
          NodeID = DatabaseFunctions.GetNullableInt32(reader, "NodeId"),
          NodeSubType = DatabaseFunctions.GetString(reader, "ObjectSubType", (string) null),
          Uri = DatabaseFunctions.GetString(reader, "Uri"),
          PollingEngineId = DatabaseFunctions.GetInt32(reader, "PollingEngineId"),
          AgentStatus = DatabaseFunctions.GetInt32(reader, "AgentStatus"),
          AgentStatusMessage = DatabaseFunctions.GetString(reader, "AgentStatusMessage"),
          ConnectionStatus = DatabaseFunctions.GetInt32(reader, "ConnectionStatus"),
          ConnectionStatusMessage = DatabaseFunctions.GetString(reader, "ConnectionStatusMessage"),
          OsType = result,
          OsDistro = DatabaseFunctions.GetString(reader, "OSDistro")
        };
        agentInfo2.Name = DatabaseFunctions.GetString(reader, "Name");
        agentInfo2.HostName = DatabaseFunctions.GetString(reader, "HostName");
        agentInfo2.IPAddress = DatabaseFunctions.GetString(reader, "IP");
        if (agentInfo1 == null || agentInfo1.AgentId != agentInfo2.AgentId)
        {
          agentsFromReader.Add(agentInfo2);
          agentInfo1 = agentInfo2;
        }
        AgentPluginInfo agentPluginInfo = new AgentPluginInfo()
        {
          PluginId = DatabaseFunctions.GetString(reader, "PluginId", (string) null)
        };
        if (agentPluginInfo.PluginId != null)
        {
          agentPluginInfo.Status = DatabaseFunctions.GetInt32(reader, "Status");
          agentPluginInfo.StatusMessage = DatabaseFunctions.GetString(reader, "StatusMessage");
          agentInfo1.AddPlugin(agentPluginInfo);
        }
      }
      return agentsFromReader;
    }

    public bool IsUniqueAgentName(string agentName)
    {
      using (InformationServiceConnection systemConnectionV3 = InformationServiceConnectionProvider.CreateSystemConnectionV3())
      {
        using (InformationServiceCommand command = systemConnectionV3.CreateCommand())
        {
          ((DbCommand) command).CommandText = "SELECT COUNT(AgentId) AS Cnt FROM Orion.AgentManagement.Agent WHERE Name = @Name";
          command.Parameters.AddWithValue("Name", (object) agentName);
          using (InformationServiceDataReader serviceDataReader = command.ExecuteReader())
            return !((DbDataReader) serviceDataReader).Read() || (int) ((DbDataReader) serviceDataReader)[0] == 0;
        }
      }
    }
  }
}
