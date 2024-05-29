// Decompiled with JetBrains decompiler
// Type: SolarWinds.Orion.Core.BusinessLayer.OneTimeJobs.OneTimeJobManager
// Assembly: SolarWinds.Orion.Core.BusinessLayer, Version=2019.4.5200.9083, Culture=neutral, PublicKeyToken=null
// MVID: E12E8C85-5CD9-4E06-8801-182E5104FADE
// Assembly location: E:\task5.dll

using SolarWinds.Common.IO;
using SolarWinds.JobEngine;
using SolarWinds.JobEngine.Security;
using SolarWinds.Logging;
using SolarWinds.Orion.Core.Common.JobEngine;
using SolarWinds.Orion.Core.Strings;
using System;
using System.Collections.Concurrent;
using System.IO;
using System.Security.Cryptography;
using System.ServiceModel;
using System.Threading;

#nullable disable
namespace SolarWinds.Orion.Core.BusinessLayer.OneTimeJobs
{
  [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, IncludeExceptionDetailInFaults = true, AutomaticSessionShutdown = true, ConcurrencyMode = ConcurrencyMode.Multiple)]
  public class OneTimeJobManager : JobSchedulerEventServicev2, IOneTimeJobManager
  {
    private static readonly Log Logger = new Log();
    private string schedulerPublicKey = string.Empty;
    private string listenerUri = string.Empty;
    private ConcurrentDictionary<Guid, OneTimeJobManager.PendingJobItem> pendingJobs = new ConcurrentDictionary<Guid, OneTimeJobManager.PendingJobItem>();
    private readonly Func<IJobSchedulerHelper> jobSchedulerHelperFactory;
    private readonly TimeSpan jobTimeoutTolerance = TimeSpan.FromSeconds(10.0);

    public event EventHandler<EventArgs> JobStarted;

    public OneTimeJobManager()
      : this((IServiceStateProvider) null)
    {
    }

    public OneTimeJobManager(IServiceStateProvider parent)
      : this(parent, (Func<IJobSchedulerHelper>) (() => JobScheduler.GetLocalInstance()), TimeSpan.FromSeconds(10.0))
    {
    }

    internal OneTimeJobManager(
      IServiceStateProvider parent,
      Func<IJobSchedulerHelper> jobSchedulerHelperFactory,
      TimeSpan jobTimeoutTolerance)
      : base(parent)
    {
      this.jobSchedulerHelperFactory = jobSchedulerHelperFactory;
      this.jobTimeoutTolerance = jobTimeoutTolerance;
    }

    public void SetListenerUri(string listenerUri) => this.listenerUri = listenerUri;

    private RSACryptoServiceProvider CreateCrypoService()
    {
      if (string.IsNullOrEmpty(this.schedulerPublicKey))
      {
        using (IJobSchedulerHelper ijobSchedulerHelper = this.jobSchedulerHelperFactory())
          this.schedulerPublicKey = ((IJobScheduler) ijobSchedulerHelper).GetPublicKey();
      }
      RSACryptoServiceProvider crypoService = new RSACryptoServiceProvider();
      crypoService.FromXmlString(this.schedulerPublicKey);
      return crypoService;
    }

    private Credential EncryptCredentials(CredentialBase creds)
    {
      if (creds == null)
        return Credential.Empty;
      using (RSACryptoServiceProvider crypoService = this.CreateCrypoService())
        return new Credential(creds, (RSA) crypoService);
    }

    private Guid SubmitScheduledJobToScheduler(ScheduledJob job)
    {
      using (IJobSchedulerHelper ijobSchedulerHelper = this.jobSchedulerHelperFactory())
      {
        OneTimeJobManager.Logger.Debug((object) "Adding new job to Job Engine");
        return ((IJobScheduler) ijobSchedulerHelper).AddJob(job);
      }
    }

    public OneTimeJobRawResult ExecuteJob(
      JobDescription jobDescription,
      CredentialBase jobCredential = null)
    {
      if (this.listenerUri == string.Empty)
      {
        JobSchedulerEventServicev2.log.Error((object) "ListenerUri remains uninitialized");
        return new OneTimeJobRawResult()
        {
          Success = false,
          Error = Resources.TestErrorJobFailed
        };
      }
      if (jobCredential != null)
        jobDescription.Credential = this.EncryptCredentials(jobCredential);
      if (jobDescription.SupportedRoles == null)
        jobDescription.SupportedRoles = (PackageType) 7;
      ScheduledJob job = new ScheduledJob()
      {
        NotificationAddress = this.listenerUri,
        State = "CoreOneTimeJob",
        RunOnce = true,
        IsOneShot = true,
        Job = jobDescription
      };
      Guid scheduler;
      try
      {
        scheduler = this.SubmitScheduledJobToScheduler(job);
        OneTimeJobManager.Logger.DebugFormat("Job {0} scheduled", (object) scheduler);
      }
      catch (Exception ex)
      {
        OneTimeJobManager.Logger.ErrorFormat("Failed to submit job: {0}", (object) ex);
        OneTimeJobRawResult timeJobRawResult = new OneTimeJobRawResult();
        timeJobRawResult.Success = false;
        timeJobRawResult.Error = Resources.TestErrorJobFailed;
        timeJobRawResult.ExceptionFromJob = ex;
        timeJobRawResult = timeJobRawResult;
        return timeJobRawResult;
      }
      TimeSpan timeout = jobDescription.Timeout.Add(this.jobTimeoutTolerance);
      OneTimeJobManager.PendingJobItem pendingJobItem = new OneTimeJobManager.PendingJobItem();
      this.pendingJobs[scheduler] = pendingJobItem;
      if (this.JobStarted != null)
        this.JobStarted((object) this, (EventArgs) new OneTimeJobManager.JobStartedEventArgs(scheduler));
      OneTimeJobRawResult timeJobRawResult1;
      if (pendingJobItem.WaitHandle.WaitOne(timeout))
      {
        timeJobRawResult1 = pendingJobItem.RawResult;
      }
      else
      {
        OneTimeJobManager.Logger.ErrorFormat("No result from job {0} received before timeout ({1})", (object) scheduler, (object) timeout);
        timeJobRawResult1 = new OneTimeJobRawResult()
        {
          Success = false,
          Error = Resources.TestErrorTimeout
        };
      }
      this.pendingJobs.TryRemove(scheduler, out pendingJobItem);
      return timeJobRawResult1;
    }

    protected override void ProcessJobProgress(JobProgress jobProgress)
    {
      OneTimeJobManager.Logger.InfoFormat("Progress from job {0}: {1}", (object) jobProgress.JobId, (object) jobProgress.Progress);
    }

    protected override void ProcessJobFailure(FinishedJobInfo jobResult)
    {
      Guid scheduledJobId = jobResult.ScheduledJobId;
      OneTimeJobManager.PendingJobItem pendingJobItem;
      if (this.pendingJobs.TryGetValue(scheduledJobId, out pendingJobItem))
      {
        OneTimeJobRawResult result = new OneTimeJobRawResult()
        {
          Success = false,
          Error = Resources.TestErrorJobFailed
        };
        OneTimeJobManager.Logger.WarnFormat("Job {0} failed with error: {1}", (object) scheduledJobId, (object) jobResult.Result.Error);
        pendingJobItem.Done(result);
      }
      else
        OneTimeJobManager.Logger.ErrorFormat("Failure of unknown job {0} received", (object) scheduledJobId);
    }

    protected override void ProcessJobResult(FinishedJobInfo jobResult)
    {
      Guid scheduledJobId = jobResult.ScheduledJobId;
      OneTimeJobManager.PendingJobItem pendingJobItem;
      if (this.pendingJobs.TryGetValue(scheduledJobId, out pendingJobItem))
      {
        OneTimeJobRawResult result = new OneTimeJobRawResult();
        try
        {
          result.Success = jobResult.Result.State == 6 && string.IsNullOrEmpty(jobResult.Result.Error);
          if (jobResult.Result.IsResultStreamed)
          {
            using (IJobSchedulerHelper ijobSchedulerHelper = this.jobSchedulerHelperFactory())
            {
              using (Stream jobResultStream = ((IJobScheduler) ijobSchedulerHelper).GetJobResultStream(jobResult.Result.JobId, "JobResult"))
              {
                result.JobResultStream = (Stream) new DynamicStream();
                jobResultStream.CopyTo(result.JobResultStream);
                result.JobResultStream.Position = 0L;
              }
              ((IJobScheduler) ijobSchedulerHelper).DeleteJobResult(jobResult.Result.JobId);
            }
          }
          else if (jobResult.Result.Output != null && jobResult.Result.Output.Length != 0)
            result.JobResultStream = (Stream) new MemoryStream(jobResult.Result.Output);
          result.Error = jobResult.Result.Error;
          OneTimeJobManager.Logger.InfoFormat("Result of one time job {0} received", (object) scheduledJobId);
        }
        catch (Exception ex)
        {
          result.Success = false;
          result.Error = Resources.TestErrorInvalidResult;
          OneTimeJobManager.Logger.ErrorFormat("Failed to process result of one time job {0}: {1}", (object) scheduledJobId, (object) ex);
        }
        pendingJobItem.Done(result);
      }
      else
      {
        OneTimeJobManager.Logger.ErrorFormat("Result of unknown job {0} received", (object) scheduledJobId);
        if (jobResult.Result == null || !jobResult.Result.IsResultStreamed)
          return;
        using (IJobSchedulerHelper ijobSchedulerHelper = this.jobSchedulerHelperFactory())
          ((IJobScheduler) ijobSchedulerHelper).DeleteJobResult(jobResult.Result.JobId);
      }
    }

    public class JobStartedEventArgs : EventArgs
    {
      public Guid JobId { get; private set; }

      public JobStartedEventArgs(Guid jobId) => this.JobId = jobId;
    }

    private class PendingJobItem
    {
      private ManualResetEvent waitHandle = new ManualResetEvent(false);

      public ManualResetEvent WaitHandle => this.waitHandle;

      public OneTimeJobRawResult RawResult { get; private set; }

      public void Done(OneTimeJobRawResult result)
      {
        this.RawResult = result;
        this.waitHandle.Set();
      }
    }
  }
}
