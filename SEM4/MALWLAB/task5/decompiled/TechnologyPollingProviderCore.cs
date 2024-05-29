// Decompiled with JetBrains decompiler
// Type: SolarWinds.Orion.Core.BusinessLayer.TechnologyPollingProviderCore
// Assembly: SolarWinds.Orion.Core.BusinessLayer, Version=2019.4.5200.9083, Culture=neutral, PublicKeyToken=null
// MVID: E12E8C85-5CD9-4E06-8801-182E5104FADE
// Assembly location: E:\task5.dll

using SolarWinds.Orion.Core.Models.Interfaces;
using SolarWinds.Orion.Core.Pollers.Node.ResponseTime;
using SolarWinds.Orion.Core.Strings;
using System.Collections.Generic;
using System.ComponentModel.Composition;

#nullable disable
namespace SolarWinds.Orion.Core.BusinessLayer
{
  [Export(typeof (ITechnologyPollingProvider))]
  public sealed class TechnologyPollingProviderCore : ITechnologyPollingProvider
  {
    public IEnumerable<ITechnologyPolling> Items
    {
      get
      {
        yield return (ITechnologyPolling) new TechnologyPollingByPollers("Node.CpuAndMemory", "Core.Node.CpuAndMemory", Resources.LIBCODE_MD0_01, 100, new string[2]
        {
          "N.Cpu.%",
          "N.Memory.%"
        });
        yield return (ITechnologyPolling) new TechnologyPollingByPollers("Node.NodeDetails", "Core.Node.NodeDetails", Resources.LIBCODE_GK0_1, 100, new string[1]
        {
          "N.Details.%"
        });
        yield return (ITechnologyPolling) new TechnologyPollingByPollers("Node.StatusResponseTime", "Core.Node.StatusResponseTime.Icmp", Resources.LIBCODE_ET0_04, 110, new string[2]
        {
          NodeResponseTimeIcmpPoller.PollerTypeStatus,
          NodeResponseTimeIcmpPoller.PollerTypeResponse
        });
        yield return (ITechnologyPolling) new TechnologyPollingByPollers("Node.StatusResponseTime", "Core.Node.StatusResponseTime.Snmp", Resources.LIBCODE_ET0_05, 100, new string[2]
        {
          NodeResponseTimeSnmpPoller.PollerTypeStatus,
          NodeResponseTimeSnmpPoller.PollerTypeResponse
        });
        yield return (ITechnologyPolling) new TechnologyPollingByPollers("Node.StatusResponseTime", "Core.Node.StatusResponseTime.Agent", Resources.LIBCODE_JT0_1, 120, new string[2]
        {
          NodeResponseTimeAgentPoller.PollerTypeStatus,
          NodeResponseTimeAgentPoller.PollerTypeResponse
        });
      }
    }
  }
}
