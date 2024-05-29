// Decompiled with JetBrains decompiler
// Type: SolarWinds.Orion.Core.BusinessLayer.DAL.IAgentInfoDAL
// Assembly: SolarWinds.Orion.Core.BusinessLayer, Version=2019.4.5200.9083, Culture=neutral, PublicKeyToken=null
// MVID: E12E8C85-5CD9-4E06-8801-182E5104FADE
// Assembly location: E:\task5.dll

using SolarWinds.Orion.Core.Common.Agent;
using System.Collections.Generic;

#nullable disable
namespace SolarWinds.Orion.Core.BusinessLayer.DAL
{
  public interface IAgentInfoDAL
  {
    IEnumerable<AgentInfo> GetAgentsInfo();

    IEnumerable<AgentInfo> GetAgentsByNodesFilter(int engineId, string nodesFilter);

    AgentInfo GetAgentInfoByNode(int nodeId);

    AgentInfo GetAgentInfo(int agentId);

    AgentInfo GetAgentInfoByIpOrHostname(string ipAddress, string hostname);

    AgentInfo GetAgentInfoByAgentAddress(string address);

    bool IsUniqueAgentName(string agentName);
  }
}
