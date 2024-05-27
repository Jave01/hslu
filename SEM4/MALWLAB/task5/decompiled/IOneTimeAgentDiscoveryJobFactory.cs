// Decompiled with JetBrains decompiler
// Type: SolarWinds.Orion.Core.BusinessLayer.IOneTimeAgentDiscoveryJobFactory
// Assembly: SolarWinds.Orion.Core.BusinessLayer, Version=2019.4.5200.9083, Culture=neutral, PublicKeyToken=null
// MVID: E12E8C85-5CD9-4E06-8801-182E5104FADE
// Assembly location: E:\task5.dll

using SolarWinds.Orion.Core.SharedCredentials;
using System;
using System.Collections.Generic;

#nullable disable
namespace SolarWinds.Orion.Core.BusinessLayer
{
  public interface IOneTimeAgentDiscoveryJobFactory
  {
    Guid CreateOneTimeAgentDiscoveryJob(
      int nodeId,
      int engineId,
      int? profileId,
      List<Credential> credentials);
  }
}
