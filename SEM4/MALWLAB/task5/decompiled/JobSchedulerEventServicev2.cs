// Decompiled with JetBrains decompiler
// Type: SolarWinds.Orion.Core.BusinessLayer.JobSchedulerEventServicev2
// Assembly: SolarWinds.Orion.Core.BusinessLayer, Version=2019.4.5200.9083, Culture=neutral, PublicKeyToken=null
// MVID: E12E8C85-5CD9-4E06-8801-182E5104FADE
// Assembly location: E:\task5.dll

using SolarWinds.JobEngine;
using SolarWinds.Logging;
using SolarWinds.Orion.Core.Common;
using SolarWinds.Orion.Core.Common.JobEngine;
using System;
using System.ServiceModel;

#nullable disable
namespace SolarWinds.Orion.Core.BusinessLayer
{
  [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, IncludeExceptionDetailInFaults = true, AutomaticSessionShutdown = true, ConcurrencyMode = ConcurrencyMode.Multiple)]
  public abstract class JobSchedulerEventServicev2 : IJobSchedulerEvents
  {
    protected static readonly Log log = new Log();
    private readonly IServiceStateProvider parentService;
    protected JobResultsManagerV2 resultsManager = new JobResultsManagerV2();

    public JobSchedulerEventServicev2()
      : this((IServiceStateProvider) null)
    {
    }

    public JobSchedulerEventServicev2(IServiceStateProvider parentService)
    {
      this.parentService = parentService;
      JobResultsManagerV2 resultsManager = this.resultsManager;
      JobResultsManagerV2.JobFailureDelegate jobFailure = resultsManager.JobFailure;
      JobSchedulerEventServicev2 schedulerEventServicev2 = this;
      // ISSUE: virtual method pointer
      JobResultsManagerV2.JobFailureDelegate b = new JobResultsManagerV2.JobFailureDelegate((object) schedulerEventServicev2, __vmethodptr(schedulerEventServicev2, ProcessJobFailure));
      resultsManager.JobFailure = (JobResultsManagerV2.JobFailureDelegate) Delegate.Combine((Delegate) jobFailure, (Delegate) b);
    }

    public void OnJobProgress(JobProgress[] jobProgressInfo)
    {
      foreach (JobProgress jobProgress in jobProgressInfo)
        this.ProcessJobProgress(jobProgress);
    }

    public void OnJobFinished(FinishedJobInfo[] jobFinishedInfo)
    {
      IServiceStateProvider parentService = this.parentService;
      if ((parentService != null ? (parentService.IsServiceDown ? 1 : 0) : 0) != 0)
      {
        JobSchedulerEventServicev2.log.InfoFormat("Parent Service Engine is in an invalid state.  Job results will be discarded.", Array.Empty<object>());
      }
      else
      {
        this.resultsManager.AddJobResults(jobFinishedInfo);
        for (FinishedJobInfo jobResult = this.resultsManager.GetJobResult(); jobResult != null; jobResult = this.resultsManager.GetJobResult())
        {
          try
          {
            this.ProcessJobResult(jobResult);
          }
          catch (Exception ex)
          {
            JobSchedulerEventServicev2.log.Error((object) "Error processing job", ex);
          }
          finally
          {
            this.resultsManager.FinishProcessingJobResult(jobResult);
          }
        }
      }
    }

    protected abstract void ProcessJobProgress(JobProgress jobProgress);

    protected abstract void ProcessJobFailure(FinishedJobInfo jobResult);

    protected abstract void ProcessJobResult(FinishedJobInfo jobResult);

    protected void RemoveJob(Guid jobId)
    {
      try
      {
        using (IJobSchedulerHelper localInstance = JobScheduler.GetLocalInstance())
        {
          JobSchedulerEventServicev2.log.DebugFormat("Removing job {0}", (object) jobId);
          ((IJobScheduler) localInstance).RemoveJob(jobId);
        }
      }
      catch (Exception ex)
      {
        JobSchedulerEventServicev2.log.ErrorFormat("Error removing job {0}.  Exception: {1}", (object) jobId, (object) ex.ToString());
      }
    }
  }
}
