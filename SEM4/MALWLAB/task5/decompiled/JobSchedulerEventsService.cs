// Decompiled with JetBrains decompiler
// Type: SolarWinds.Orion.Core.BusinessLayer.JobSchedulerEventsService
// Assembly: SolarWinds.Orion.Core.BusinessLayer, Version=2019.4.5200.9083, Culture=neutral, PublicKeyToken=null
// MVID: E12E8C85-5CD9-4E06-8801-182E5104FADE
// Assembly location: E:\task5.dll

using SolarWinds.JobEngine;
using SolarWinds.Logging;
using SolarWinds.Orion.Core.Common;
using SolarWinds.Orion.Core.Common.i18n;
using System;
using System.ServiceModel;

#nullable disable
namespace SolarWinds.Orion.Core.BusinessLayer
{
  [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, IncludeExceptionDetailInFaults = true, AutomaticSessionShutdown = true, ConcurrencyMode = ConcurrencyMode.Multiple)]
  internal abstract class JobSchedulerEventsService : IJobSchedulerEvents
  {
    protected static readonly Log log = new Log();
    private readonly CoreBusinessLayerPlugin parent;
    protected JobResultsManager resultsManager = new JobResultsManager();

    public JobSchedulerEventsService(CoreBusinessLayerPlugin parent)
    {
      this.parent = parent;
      JobResultsManager resultsManager = this.resultsManager;
      JobResultsManager.JobFailureDelegate jobFailure = resultsManager.JobFailure;
      JobSchedulerEventsService schedulerEventsService = this;
      // ISSUE: virtual method pointer
      JobResultsManager.JobFailureDelegate b = new JobResultsManager.JobFailureDelegate((object) schedulerEventsService, __vmethodptr(schedulerEventsService, ProcessJobFailure));
      resultsManager.JobFailure = (JobResultsManager.JobFailureDelegate) Delegate.Combine((Delegate) jobFailure, (Delegate) b);
    }

    public void OnJobProgress(JobProgress[] jobProgressInfo)
    {
      using (LocaleThreadState.EnsurePrimaryLocale())
      {
        foreach (JobProgress jobProgress in jobProgressInfo)
          this.ProcessJobProgress(jobProgress);
      }
    }

    public void OnJobFinished(FinishedJobInfo[] jobFinishedInfo)
    {
      using (LocaleThreadState.EnsurePrimaryLocale())
      {
        if (this.parent.IsServiceDown)
        {
          JobSchedulerEventsService.log.InfoFormat("Core Service Engine is in an invalid state.  Job results will be discarded.", Array.Empty<object>());
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
              JobSchedulerEventsService.log.Error((object) "Error processing job", ex);
            }
            finally
            {
              this.resultsManager.FinishProcessingJobResult(jobResult);
            }
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
        using (IJobSchedulerHelper instance = JobScheduler.GetInstance())
        {
          JobSchedulerEventsService.log.DebugFormat("Removing job {0}", (object) jobId);
          ((IJobScheduler) instance).RemoveJob(jobId);
        }
      }
      catch (Exception ex)
      {
        JobSchedulerEventsService.log.ErrorFormat("Error removing job {0}.  Exception: {1}", (object) jobId, (object) ex.ToString());
      }
    }
  }
}
