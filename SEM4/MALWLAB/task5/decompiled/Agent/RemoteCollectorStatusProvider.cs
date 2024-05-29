// Decompiled with JetBrains decompiler
// Type: SolarWinds.Orion.Core.BusinessLayer.Agent.RemoteCollectorStatusProvider
// Assembly: SolarWinds.Orion.Core.BusinessLayer, Version=2019.4.5200.9083, Culture=neutral, PublicKeyToken=null
// MVID: E12E8C85-5CD9-4E06-8801-182E5104FADE
// Assembly location: E:\task5.dll

using SolarWinds.AgentManagement.Contract;
using SolarWinds.Orion.Core.Common;
using SolarWinds.Orion.Core.Common.InformationService;
using SolarWinds.Orion.Core.Common.Internals;
using SolarWinds.Orion.Core.Common.Swis;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

#nullable disable
namespace SolarWinds.Orion.Core.BusinessLayer.Agent
{
  internal class RemoteCollectorStatusProvider : IRemoteCollectorAgentStatusProvider
  {
    private readonly CacheWithExpiration<IDictionary<int, AgentStatus>> _statusCache;

    public RemoteCollectorStatusProvider(
      ISwisConnectionProxyCreator swisProxyCreator,
      int masterEngineId,
      int cacheExpiration)
      : this(cacheExpiration, (Func<IDictionary<int, AgentStatus>>) (() => RemoteCollectorStatusProvider.GetCurrentStatuses(swisProxyCreator, masterEngineId)), (Func<DateTime>) (() => DateTime.UtcNow))
    {
      if (swisProxyCreator == null)
        throw new ArgumentNullException(nameof (swisProxyCreator));
    }

    internal RemoteCollectorStatusProvider(
      int cacheExpiration,
      Func<IDictionary<int, AgentStatus>> refreshFunc,
      Func<DateTime> currentTimeFunc)
    {
      if (currentTimeFunc == null)
        throw new ArgumentNullException(nameof (currentTimeFunc));
      this._statusCache = new CacheWithExpiration<IDictionary<int, AgentStatus>>(cacheExpiration, refreshFunc, currentTimeFunc);
    }

    public AgentStatus GetStatus(int engineId)
    {
      AgentStatus agentStatus;
      return !this._statusCache.Get().TryGetValue(engineId, out agentStatus) ? (AgentStatus) 0 : agentStatus;
    }

    public void InvalidateCache() => this._statusCache.Invalidate();

    private static IDictionary<int, AgentStatus> GetCurrentStatuses(
      ISwisConnectionProxyCreator swisProxyCreator,
      int masterEngineId)
    {
      return (IDictionary<int, AgentStatus>) RemoteCollectorStatusProvider.GetStatuses(swisProxyCreator, masterEngineId).ToDictionary<KeyValuePair<int, AgentStatus>, int, AgentStatus>((System.Func<KeyValuePair<int, AgentStatus>, int>) (i => i.Key), (System.Func<KeyValuePair<int, AgentStatus>, AgentStatus>) (i => i.Value));
    }

    internal static IEnumerable<KeyValuePair<int, AgentStatus>> GetStatuses(
      ISwisConnectionProxyCreator swisProxyCreator,
      int masterEngineId)
    {
      using (IInformationServiceProxy2 proxy = ((IInformationServiceProxyCreator) swisProxyCreator).Create())
      {
        IInformationServiceProxy2 iinformationServiceProxy2 = proxy;
        foreach (DataRow row in (InternalDataCollectionBase) ((IInformationServiceProxy) iinformationServiceProxy2).Query("SELECT e.EngineID, a.AgentStatus FROM Orion.EngineProperties (nolock=true) p\r\nINNER JOIN Orion.AgentManagement.Agent (nolock=true) a\r\nON p.PropertyName='AgentId' AND a.AgentId=p.PropertyValue\r\nINNER JOIN Orion.Engines (nolock=true) e\r\nON e.EngineID=p.EngineID AND e.MasterEngineID=@MasterEngineId", (IDictionary<string, object>) new Dictionary<string, object>()
        {
          {
            "MasterEngineId",
            (object) masterEngineId
          }
        }).Rows)
          yield return new KeyValuePair<int, AgentStatus>((int) row[0], (AgentStatus) row[1]);
      }
    }
  }
}
