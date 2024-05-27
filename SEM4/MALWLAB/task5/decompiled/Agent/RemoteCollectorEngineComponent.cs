// Decompiled with JetBrains decompiler
// Type: SolarWinds.Orion.Core.BusinessLayer.Agent.RemoteCollectorEngineComponent
// Assembly: SolarWinds.Orion.Core.BusinessLayer, Version=2019.4.5200.9083, Culture=neutral, PublicKeyToken=null
// MVID: E12E8C85-5CD9-4E06-8801-182E5104FADE
// Assembly location: E:\task5.dll

using SolarWinds.AgentManagement.Contract;
using SolarWinds.Orion.Core.BusinessLayer.Engines;
using System;
using System.Collections.Generic;
using System.Linq;

#nullable disable
namespace SolarWinds.Orion.Core.BusinessLayer.Agent
{
  internal class RemoteCollectorEngineComponent : IEngineComponent
  {
    private static readonly AgentStatus[] EngineUpStatuses;
    private readonly IRemoteCollectorAgentStatusProvider _agentStatusProvider;

    public RemoteCollectorEngineComponent(
      int engineId,
      IRemoteCollectorAgentStatusProvider agentStatusProvider)
    {
      this._agentStatusProvider = agentStatusProvider ?? throw new ArgumentNullException(nameof (agentStatusProvider));
      this.EngineId = engineId;
    }

    public int EngineId { get; }

    public EngineComponentStatus GetStatus()
    {
      return RemoteCollectorEngineComponent.ToEngineStatus(this._agentStatusProvider.GetStatus(this.EngineId));
    }

    private static EngineComponentStatus ToEngineStatus(AgentStatus agentStatus)
    {
      return !((IEnumerable<AgentStatus>) RemoteCollectorEngineComponent.EngineUpStatuses).Contains<AgentStatus>(agentStatus) ? EngineComponentStatus.Down : EngineComponentStatus.Up;
    }

    static RemoteCollectorEngineComponent()
    {
      // ISSUE: unable to decompile the method.
    }
  }
}
