// Decompiled with JetBrains decompiler
// Type: SolarWinds.Orion.Core.BusinessLayer.DiscoveryJobSchedulerEventsService
// Assembly: SolarWinds.Orion.Core.BusinessLayer, Version=2019.4.5200.9083, Culture=neutral, PublicKeyToken=null
// MVID: E12E8C85-5CD9-4E06-8801-182E5104FADE
// Assembly location: E:\task5.dll

using SolarWinds.JobEngine;
using SolarWinds.Orion.Core.Strings;
using System;
using System.ServiceModel;
using System.Text;

#nullable disable
namespace SolarWinds.Orion.Core.BusinessLayer
{
  [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, IncludeExceptionDetailInFaults = true, AutomaticSessionShutdown = true, ConcurrencyMode = ConcurrencyMode.Multiple)]
  internal class DiscoveryJobSchedulerEventsService : JobSchedulerEventsService
  {
    public DiscoveryJobSchedulerEventsService(CoreBusinessLayerPlugin parent)
      : base(parent)
    {
      this.resultsManager.HandleResultsOfCancelledJobs = true;
    }

    protected override void ProcessJobProgress(JobProgress jobProgress)
    {
      this.RemoveOldDiscoveryJob(jobProgress.JobId);
    }

    protected override void ProcessJobResult(FinishedJobInfo jobInfo)
    {
      this.RemoveOldDiscoveryJob(jobInfo.ScheduledJobId);
    }

    protected override void ProcessJobFailure(FinishedJobInfo jobInfo)
    {
      this.RemoveOldDiscoveryJob(jobInfo.ScheduledJobId);
    }

    private string ComposeNotificationMessage(int newNodes, int changedNodes)
    {
      StringBuilder stringBuilder = new StringBuilder(Resources.LIBCODE_PCC_18);
      stringBuilder.Append(" ");
      if (newNodes == 1)
        stringBuilder.Append(Resources.LIBCODE_PCC_19);
      else if (newNodes > 1)
        stringBuilder.AppendFormat(Resources.LIBCODE_PCC_20, (object) newNodes);
      if (changedNodes > 0)
      {
        if (newNodes >= 0)
          stringBuilder.Append(" ");
        if (changedNodes == 1)
          stringBuilder.Append(Resources.LIBCODE_PCC_21);
        else
          stringBuilder.AppendFormat(Resources.LIBCODE_PCC_22, (object) changedNodes);
      }
      return stringBuilder.ToString();
    }

    private void RemoveOldDiscoveryJob(Guid jobId)
    {
      if (jobId == Guid.Empty)
      {
        JobSchedulerEventsService.log.ErrorFormat("Unable to identify id of old discovery job to delete.", Array.Empty<object>());
      }
      else
      {
        try
        {
          JobSchedulerEventsService.log.InfoFormat("Deleting old discovery job [{0}]", (object) jobId);
          if (DiscoveryJobFactory.DeleteJob(jobId))
            return;
          JobSchedulerEventsService.log.ErrorFormat("Error when deleting old discovery job [{0}]", (object) jobId);
        }
        catch (Exception ex)
        {
          JobSchedulerEventsService.log.Error((object) string.Format("Exception occured when deleting old discovery job [{0}]", (object) jobId), ex);
        }
      }
    }
  }
}
