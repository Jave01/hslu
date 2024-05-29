// Decompiled with JetBrains decompiler
// Type: SolarWinds.Orion.Core.BusinessLayer.DiscoveryJobFactory
// Assembly: SolarWinds.Orion.Core.BusinessLayer, Version=2019.4.5200.9083, Culture=neutral, PublicKeyToken=null
// MVID: E12E8C85-5CD9-4E06-8801-182E5104FADE
// Assembly location: E:\task5.dll

using SolarWinds.JobEngine;
using SolarWinds.Logging;
using System;

#nullable disable
namespace SolarWinds.Orion.Core.BusinessLayer
{
  public static class DiscoveryJobFactory
  {
    private static readonly Log log = new Log();

    public static bool DeleteJob(Guid jobId)
    {
      using (IJobSchedulerHelper instance = JobScheduler.GetInstance())
      {
        try
        {
          ((IJobScheduler) instance).RemoveJob(jobId);
          return true;
        }
        catch
        {
          DiscoveryJobFactory.log.DebugFormat("Unable to delete job in Job Engine({0}", (object) jobId);
          return false;
        }
      }
    }
  }
}
