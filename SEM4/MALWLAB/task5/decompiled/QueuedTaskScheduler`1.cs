// Decompiled with JetBrains decompiler
// Type: SolarWinds.Orion.Core.BusinessLayer.QueuedTaskScheduler`1
// Assembly: SolarWinds.Orion.Core.BusinessLayer, Version=2019.4.5200.9083, Culture=neutral, PublicKeyToken=null
// MVID: E12E8C85-5CD9-4E06-8801-182E5104FADE
// Assembly location: E:\task5.dll

using Amib.Threading;
using Amib.Threading.Internal;
using SolarWinds.Logging;
using SolarWinds.Orion.Core.Common.Extensions;
using SolarWinds.Orion.Core.Common.i18n;
using SolarWinds.Orion.Core.Strings;
using System;

#nullable disable
namespace SolarWinds.Orion.Core.BusinessLayer
{
  public class QueuedTaskScheduler<TTask> : IDisposable where TTask : class
  {
    private static readonly Log log = new Log(nameof (QueuedTaskScheduler<TTask>));
    private SmartThreadPool processingThreadPool;
    private IWorkItemsGroup processingGroup;
    private STPStartInfo processingStartInfo;
    private QueuedTaskScheduler<TTask>.TaskProcessingRoutine processingRoutine;
    private volatile bool isRunning;
    private bool disposed;

    public event EventHandler TaskProcessingFinished;

    public bool IsRunning => this.isRunning;

    public int QueueSize
    {
      get
      {
        return ((WorkItemsGroupBase) this.processingThreadPool).WaitingCallbacks + this.processingGroup.WaitingCallbacks;
      }
    }

    public QueuedTaskScheduler(
      QueuedTaskScheduler<TTask>.TaskProcessingRoutine routine,
      int paralleltasksCount)
    {
      this.isRunning = false;
      this.processingRoutine = routine;
      STPStartInfo stpStartInfo = new STPStartInfo();
      stpStartInfo.MaxWorkerThreads = paralleltasksCount;
      stpStartInfo.MinWorkerThreads = 0;
      ((WIGStartInfo) stpStartInfo).StartSuspended = true;
      this.processingStartInfo = stpStartInfo;
      this.processingThreadPool = new SmartThreadPool(this.processingStartInfo);
      this.processingGroup = this.processingThreadPool.CreateWorkItemsGroup(paralleltasksCount);
      // ISSUE: method pointer
      this.processingGroup.OnIdle += new WorkItemsGroupIdleHandler((object) this, __methodptr(processingGroup_OnIdle));
    }

    public void EnqueueTask(TTask task)
    {
      this.processingGroup.QueueWorkItem(new WorkItemCallback(this.ThreadPoolCallBack), (object) task);
    }

    public void Start()
    {
      if (this.IsRunning)
        throw new InvalidOperationException(Resources.LIBCODE_JM0_30);
      if (this.QueueSize > 0)
      {
        this.isRunning = true;
        this.processingGroup.Start();
        ((WorkItemsGroupBase) this.processingThreadPool).Start();
        QueuedTaskScheduler<TTask>.log.InfoFormat("Queued tasks processing started: QueuedTasksCount = {0}, ParallelTasksCount = {1}", (object) this.QueueSize, (object) this.processingGroup.Concurrency);
      }
      else
      {
        this.isRunning = true;
        QueuedTaskScheduler<TTask>.log.InfoFormat("Queued tasks processing started: Queue is empty", Array.Empty<object>());
        if (this.TaskProcessingFinished != null)
          this.TaskProcessingFinished((object) this, new EventArgs());
        this.isRunning = false;
      }
    }

    private void processingGroup_OnIdle(IWorkItemsGroup workItemsGroup)
    {
      if (!this.isRunning)
        return;
      this.isRunning = false;
      SmartThreadPoolExtensions.Suspend(this.processingGroup);
      SmartThreadPoolExtensions.Suspend((IWorkItemsGroup) this.processingThreadPool);
      if (this.TaskProcessingFinished == null)
        return;
      this.TaskProcessingFinished((object) this, new EventArgs());
    }

    public void Cancel()
    {
      this.processingGroup.Cancel(false);
      QueuedTaskScheduler<TTask>.log.InfoFormat("Task processing recieved cancel signal, there are {0} active threads", (object) this.processingThreadPool.ActiveThreads);
      ((WorkItemsGroupBase) this.processingThreadPool).WaitForIdle();
    }

    private object ThreadPoolCallBack(object state)
    {
      if (state is TTask task)
      {
        try
        {
          if (!SmartThreadPool.IsWorkItemCanceled)
          {
            using (LocaleThreadState.EnsurePrimaryLocale())
              this.processingRoutine(task);
          }
        }
        catch (Exception ex)
        {
          QueuedTaskScheduler<TTask>.log.Error((object) "Unhandled exception in queued task processing:", ex);
        }
      }
      return (object) null;
    }

    public bool IsTaskCanceled => SmartThreadPool.IsWorkItemCanceled;

    private void Dispose(bool disposing)
    {
      if (this.disposed)
        return;
      if (disposing)
      {
        if (this.processingGroup != null)
        {
          this.processingGroup.Cancel(false);
          this.processingGroup = (IWorkItemsGroup) null;
        }
        if (this.processingThreadPool != null)
        {
          this.processingThreadPool.Dispose();
          this.processingThreadPool = (SmartThreadPool) null;
        }
      }
      this.disposed = true;
    }

    public void Dispose()
    {
      this.Dispose(true);
      GC.SuppressFinalize((object) this);
    }

    ~QueuedTaskScheduler() => this.Dispose(false);

    public delegate void TaskProcessingRoutine(TTask task) where TTask : class;
  }
}
