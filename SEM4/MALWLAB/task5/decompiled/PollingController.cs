// Decompiled with JetBrains decompiler
// Type: SolarWinds.Orion.Core.BusinessLayer.PollingController
// Assembly: SolarWinds.Orion.Core.BusinessLayer, Version=2019.4.5200.9083, Culture=neutral, PublicKeyToken=null
// MVID: E12E8C85-5CD9-4E06-8801-182E5104FADE
// Assembly location: E:\task5.dll

using SolarWinds.Collector.Contract;
using SolarWinds.Logging;
using SolarWinds.Orion.Core.Common;
using System;
using System.ServiceModel;

#nullable disable
namespace SolarWinds.Orion.Core.BusinessLayer
{
  internal class PollingController : 
    IPollingControllerServiceHelper,
    IPollingControllerService,
    IDisposable
  {
    private static readonly Log log = new Log();
    private IChannelProxy<IPollingControllerService> channel;

    public static IPollingControllerServiceHelper GetInstance()
    {
      return (IPollingControllerServiceHelper) new PollingController((IChannelProxy<IPollingControllerService>) new ChannelProxy<IPollingControllerService>(new ChannelFactory<IPollingControllerService>("ToCollector")));
    }

    public PollingController(IChannelProxy<IPollingControllerService> channel)
    {
      this.channel = channel != null ? channel : throw new ArgumentNullException(nameof (channel));
    }

    public void PollNow(string entityIdentifier)
    {
      this.channel.Invoke((Action<IPollingControllerService>) (n => n.PollNow(entityIdentifier)));
    }

    public void RediscoverNow(string entityIdentifier)
    {
      this.channel.Invoke((Action<IPollingControllerService>) (n => n.RediscoverNow(entityIdentifier)));
    }

    public void JobNow(JobExecutionCondition condition)
    {
      this.channel.Invoke((Action<IPollingControllerService>) (n => n.JobNow(condition)));
    }

    public void CancelJob(JobExecutionCondition condition)
    {
      this.channel.Invoke((Action<IPollingControllerService>) (n => n.CancelJob(condition)));
    }

    ~PollingController() => this.Dispose(false);

    public void Dispose()
    {
      this.Dispose(true);
      GC.SuppressFinalize((object) this);
    }

    protected void Dispose(bool disposing)
    {
      if (this.channel == null)
        return;
      ((IDisposable) this.channel).Dispose();
    }
  }
}
