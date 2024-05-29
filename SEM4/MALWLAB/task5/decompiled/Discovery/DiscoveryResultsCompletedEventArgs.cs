// Decompiled with JetBrains decompiler
// Type: SolarWinds.Orion.Core.BusinessLayer.Discovery.DiscoveryResultsCompletedEventArgs
// Assembly: SolarWinds.Orion.Core.BusinessLayer, Version=2019.4.5200.9083, Culture=neutral, PublicKeyToken=null
// MVID: E12E8C85-5CD9-4E06-8801-182E5104FADE
// Assembly location: E:\task5.dll

using SolarWinds.JobEngine;
using SolarWinds.Orion.Discovery.Contract.DiscoveryPlugin;
using SolarWinds.Orion.Discovery.Job;
using System;
using System.Collections.Generic;

#nullable disable
namespace SolarWinds.Orion.Core.BusinessLayer.Discovery
{
  public class DiscoveryResultsCompletedEventArgs : EventArgs
  {
    public DiscoveryResultsCompletedEventArgs(
      OrionDiscoveryJobResult completeResult,
      SortedDictionary<int, List<IDiscoveryPlugin>> orderedPlugins,
      Guid scheduledJobId,
      JobState jobState,
      int? profileId)
    {
      this.CompleteResult = completeResult;
      this.OrderedPlugins = orderedPlugins;
      this.ScheduledJobId = scheduledJobId;
      this.JobState = jobState;
      this.ProfileId = profileId;
    }

    public OrionDiscoveryJobResult CompleteResult { get; private set; }

    public SortedDictionary<int, List<IDiscoveryPlugin>> OrderedPlugins { get; private set; }

    public Guid ScheduledJobId { get; private set; }

    public JobState JobState { get; private set; }

    public int? ProfileId { get; private set; }
  }
}
