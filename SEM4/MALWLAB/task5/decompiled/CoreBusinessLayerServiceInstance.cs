// Decompiled with JetBrains decompiler
// Type: SolarWinds.Orion.Core.BusinessLayer.CoreBusinessLayerServiceInstance
// Assembly: SolarWinds.Orion.Core.BusinessLayer, Version=2019.4.5200.9083, Culture=neutral, PublicKeyToken=null
// MVID: E12E8C85-5CD9-4E06-8801-182E5104FADE
// Assembly location: E:\task5.dll

using SolarWinds.JobEngine;
using SolarWinds.Orion.Core.BusinessLayer.Discovery;
using SolarWinds.Orion.Core.Common;
using SolarWinds.Orion.Core.Common.BusinessLayer;
using SolarWinds.Orion.Core.Common.Proxy.BusinessLayer;
using SolarWinds.ServiceDirectory.Client.Contract;
using System;
using System.ServiceModel;

#nullable disable
namespace SolarWinds.Orion.Core.BusinessLayer
{
  internal class CoreBusinessLayerServiceInstance : 
    BusinessLayerServiceInstanceBase<CoreBusinessLayerService>
  {
    private RescheduleDiscoveryJobsTask _discoveryJobRescheduler;
    private readonly IEngineInitiator _engineInitiator;

    public CoreBusinessLayerServiceInstance(
      int engineId,
      IEngineInitiator engineInitiator,
      CoreBusinessLayerService serviceInstance,
      ServiceHostBase serviceHost,
      IServiceDirectoryClient serviceDirectoryClient)
      : base(engineId, engineInitiator.ServerName, serviceInstance, serviceHost, serviceDirectoryClient)
    {
      this._engineInitiator = engineInitiator;
      this.Service = serviceInstance;
      this.ServiceLogicalInstanceId = CoreBusinessLayerConfiguration.GetLogicalInstanceId(this.EngineId);
    }

    public CoreBusinessLayerService Service { get; }

    protected virtual string ServiceId => "Core.BusinessLayer";

    protected virtual string ServiceLogicalInstanceId { get; }

    public void RouteJobToEngine(JobDescription jobDescription)
    {
      if (!string.IsNullOrEmpty(jobDescription.LegacyEngine))
        return;
      jobDescription.LegacyEngine = this.EngineName;
    }

    public void StopRescheduleEngineDiscoveryJobsTask()
    {
      using (this._discoveryJobRescheduler)
        this._discoveryJobRescheduler = (RescheduleDiscoveryJobsTask) null;
    }

    public void InitRescheduleEngineDiscoveryJobsTask(bool isMaster)
    {
      this._discoveryJobRescheduler = new RescheduleDiscoveryJobsTask(new Func<int, bool>(this.Service.UpdateDiscoveryJobs), this.EngineId, !isMaster, isMaster ? TimeSpan.FromSeconds(10.0) : TimeSpan.FromMinutes(10.0));
      this._discoveryJobRescheduler.StartPeriodicRescheduleTask();
    }

    public void RunRescheduleEngineDiscoveryJobsTask()
    {
      this._discoveryJobRescheduler?.QueueRescheduleAttempt();
    }

    public void InitializeEngine() => this._engineInitiator.InitializeEngine();

    public void UpdateEngine(bool updateJobEngineThrottleInfo)
    {
      this._engineInitiator.UpdateInfo(updateJobEngineThrottleInfo);
    }
  }
}
