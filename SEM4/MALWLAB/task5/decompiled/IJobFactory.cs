﻿// Decompiled with JetBrains decompiler
// Type: SolarWinds.Orion.Core.BusinessLayer.IJobFactory
// Assembly: SolarWinds.Orion.Core.BusinessLayer, Version=2019.4.5200.9083, Culture=neutral, PublicKeyToken=null
// MVID: E12E8C85-5CD9-4E06-8801-182E5104FADE
// Assembly location: E:\task5.dll

using SolarWinds.JobEngine;
using SolarWinds.Orion.Core.Common.Models;
using System;

#nullable disable
namespace SolarWinds.Orion.Core.BusinessLayer
{
  public interface IJobFactory
  {
    Guid SubmitScheduledJob(Guid jobId, ScheduledJob job, bool executeImmediately);

    ScheduledJob CreateDiscoveryJob(DiscoveryConfiguration configuration);

    bool DeleteJob(Guid jobId);

    Guid SubmitScheduledJobToLocalEngine(Guid jobId, ScheduledJob job, bool executeImmediately);
  }
}
