// Decompiled with JetBrains decompiler
// Type: SolarWinds.Orion.Core.BusinessLayer.Agent.RemoteCollectorEngineInitiator
// Assembly: SolarWinds.Orion.Core.BusinessLayer, Version=2019.4.5200.9083, Culture=neutral, PublicKeyToken=null
// MVID: E12E8C85-5CD9-4E06-8801-182E5104FADE
// Assembly location: E:\task5.dll

using SolarWinds.Orion.Core.BusinessLayer.Engines;
using SolarWinds.Orion.Core.Common;
using SolarWinds.Orion.Core.Common.DALs;
using SolarWinds.Orion.Core.Common.JobEngine;
using System;
using System.Collections.Generic;

#nullable disable
namespace SolarWinds.Orion.Core.BusinessLayer.Agent
{
  internal class RemoteCollectorEngineInitiator : IEngineInitiator
  {
    private readonly bool _interfaceAvailable;
    private readonly IEngineDAL _engineDal;
    private readonly IThrottlingStatusProvider _throttlingStatusProvider;
    private readonly IEngineComponent _engineComponent;
    private const float DefaultPollingCompletion = 0.0f;
    private static readonly Dictionary<string, object> DefaultValues = new Dictionary<string, object>()
    {
      {
        "EngineVersion",
        (object) RegistrySettings.GetVersionDisplayString()
      }
    };

    public RemoteCollectorEngineInitiator(
      int engineId,
      string engineName,
      bool interfaceAvailable,
      IEngineDAL engineDal,
      IThrottlingStatusProvider throttlingStatusProvider,
      IEngineComponent engineComponent)
    {
      if (engineName == null)
        throw new ArgumentNullException(nameof (engineName));
      this._engineDal = engineDal ?? throw new ArgumentNullException(nameof (engineDal));
      this._throttlingStatusProvider = throttlingStatusProvider ?? throw new ArgumentNullException(nameof (throttlingStatusProvider));
      this._engineComponent = engineComponent ?? throw new ArgumentNullException(nameof (engineComponent));
      this.EngineId = engineId;
      this.ServerName = engineName.ToUpperInvariant();
      this._interfaceAvailable = interfaceAvailable;
    }

    public int EngineId { get; }

    public string ServerName { get; }

    public EngineComponentStatus ComponentStatus => this._engineComponent.GetStatus();

    public bool AllowKeepAlive => this.ComponentStatus == EngineComponentStatus.Up;

    public bool AllowPollingCompletion => this.ComponentStatus == EngineComponentStatus.Up;

    public void InitializeEngine()
    {
      this._engineDal.UpdateEngineInfo(this.EngineId, RemoteCollectorEngineInitiator.DefaultValues, false, this._interfaceAvailable, this.AllowKeepAlive);
    }

    public void UpdateInfo(bool updateJobEngineThrottleInfo)
    {
      this._engineDal.UpdateEngineInfo(this.EngineId, new Dictionary<string, object>()
      {
        {
          "PollingCompletion",
          (object) (this.AllowPollingCompletion & updateJobEngineThrottleInfo ? this._throttlingStatusProvider.GetPollingCompletion() : 0.0f)
        }
      }, true, this._interfaceAvailable, this.AllowKeepAlive);
    }
  }
}
