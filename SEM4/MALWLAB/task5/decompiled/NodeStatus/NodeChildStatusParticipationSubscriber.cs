// Decompiled with JetBrains decompiler
// Type: SolarWinds.Orion.Core.BusinessLayer.NodeStatus.NodeChildStatusParticipationSubscriber
// Assembly: SolarWinds.Orion.Core.BusinessLayer, Version=2019.4.5200.9083, Culture=neutral, PublicKeyToken=null
// MVID: E12E8C85-5CD9-4E06-8801-182E5104FADE
// Assembly location: E:\task5.dll

using SolarWinds.Logging;
using SolarWinds.Orion.Core.Common;
using SolarWinds.Orion.PubSub;
using System;
using System.Data.SqlClient;
using System.Threading;
using System.Threading.Tasks;

#nullable disable
namespace SolarWinds.Orion.Core.BusinessLayer.NodeStatus
{
  public class NodeChildStatusParticipationSubscriber : ISubscriber, IDisposable
  {
    public static string SubscriptionUniqueName = "NodeChildStatusParticipationChanged";
    private static string SubscriptionQuery = "SUBSCRIBE CHANGES TO Orion.NodeChildStatusParticipation WHEN [Enabled] CHANGES";
    private readonly int delay;
    private static readonly Log log = new Log();
    private readonly ISubscriptionManager subscriptionManager;
    private readonly ISqlHelper sqlHelper;
    private ISubscription subscription;
    private Timer reflowScheduler;

    public NodeChildStatusParticipationSubscriber(
      ISubscriptionManager subscriptionManager,
      ISqlHelper sqlHelper)
      : this(subscriptionManager, sqlHelper, 5000)
    {
    }

    public NodeChildStatusParticipationSubscriber(
      ISubscriptionManager subscriptionManager,
      ISqlHelper sqlHelper,
      int delay)
    {
      this.subscriptionManager = subscriptionManager ?? throw new ArgumentNullException(nameof (subscriptionManager));
      this.sqlHelper = sqlHelper ?? throw new ArgumentNullException(nameof (sqlHelper));
      this.delay = delay >= 0 ? delay : throw new ArgumentOutOfRangeException(nameof (delay), "cannot be negative");
    }

    public Task OnNotificationAsync(Notification notification)
    {
      Task<bool> task = Task.FromResult<bool>(false);
      if (this.subscription != null)
      {
        string str1;
        if (notification == null)
        {
          str1 = (string) null;
        }
        else
        {
          SubscriptionId subscriptionId = notification.SubscriptionId;
          str1 = ((SubscriptionId) ref subscriptionId).UniqueName;
        }
        string subscriptionUniqueName = NodeChildStatusParticipationSubscriber.SubscriptionUniqueName;
        if (!(str1 != subscriptionUniqueName))
        {
          if (notification.SourceInstanceProperties == null)
          {
            NodeChildStatusParticipationSubscriber.log.Error((object) "Argument SourceInstanceProperties is null.");
            return (Task) task;
          }
          if (notification.SourceInstanceProperties.ContainsKey("EntityType"))
          {
            if (notification.SourceInstanceProperties.ContainsKey("Enabled"))
            {
              try
              {
                string str2 = Convert.ToString(notification.SourceInstanceProperties["EntityType"]);
                string str3 = Convert.ToBoolean(notification.SourceInstanceProperties["Enabled"]) ? "enabled" : "disabled";
                NodeChildStatusParticipationSubscriber.log.DebugFormat("Node child status participation for '" + str2 + "' is " + str3 + ", re-calculating node status ..", Array.Empty<object>());
                this.reflowScheduler?.Change(-1, -1);
                this.reflowScheduler = this.SetupReflowScheduler();
                return (Task) Task.FromResult<bool>(this.reflowScheduler.Change(this.delay, -1));
              }
              catch (Exception ex)
              {
                NodeChildStatusParticipationSubscriber.log.Error((object) "Indication handling failed", ex);
              }
              return (Task) task;
            }
          }
          NodeChildStatusParticipationSubscriber.log.Error((object) "The EntityType or Enabled not supplied in SourceInstanceProperties.");
          return (Task) task;
        }
      }
      return (Task) task;
    }

    public NodeChildStatusParticipationSubscriber Start()
    {
      NodeChildStatusParticipationSubscriber.log.Debug((object) "Subscribing NodeChildStatusParticipation changed indications..");
      try
      {
        if (this.subscription != null)
        {
          NodeChildStatusParticipationSubscriber.log.Debug((object) "Already subscribed, unsubscribing first..");
          this.Unsubscribe(this.subscription.Id);
        }
        SubscriptionId subscriptionId;
        // ISSUE: explicit constructor call
        ((SubscriptionId) ref subscriptionId).\u002Ector("Core", NodeChildStatusParticipationSubscriber.SubscriptionUniqueName, (Scope) 0);
        this.subscription = this.subscriptionManager.Subscribe(subscriptionId, (ISubscriber) this, new SubscriberConfiguration()
        {
          SubscriptionQuery = NodeChildStatusParticipationSubscriber.SubscriptionQuery,
          ReliableDelivery = true,
          AcknowledgeMode = (AcknowledgeMode) 0,
          MessageTimeToLive = TimeSpan.Zero
        });
        return this;
      }
      catch (Exception ex)
      {
        NodeChildStatusParticipationSubscriber.log.Error((object) "Failed to subscribe.", ex);
        throw;
      }
    }

    public void Dispose()
    {
      this.Dispose(true);
      GC.SuppressFinalize((object) this);
    }

    protected void Dispose(bool disposing)
    {
      if (this.subscription != null)
      {
        try
        {
          NodeChildStatusParticipationSubscriber.log.Debug((object) "Unsubscribing NodeChildStatusParticipation changed indications..");
          this.Unsubscribe(this.subscription.Id);
          this.subscription = (ISubscription) null;
        }
        catch (Exception ex)
        {
          NodeChildStatusParticipationSubscriber.log.Error((object) "Error unsubscribing subscription.", ex);
        }
      }
      if (!(this.reflowScheduler != null & disposing))
        return;
      this.reflowScheduler.Dispose();
    }

    private void Unsubscribe(SubscriptionId subscriptionId)
    {
      this.subscriptionManager.Unsubscribe(subscriptionId);
    }

    private Timer SetupReflowScheduler()
    {
      return new Timer(new TimerCallback(this.OnReflowAllNodeChildStatus), (object) null, -1, -1);
    }

    public event EventHandler ReflowAllNodeChildStatus;

    private void OnReflowAllNodeChildStatus(object state)
    {
      try
      {
        using (NodeChildStatusParticipationSubscriber.log.Block("NodeChildStatusParticipation.OnReflowAllNodeChildStatus"))
        {
          NodeChildStatusParticipationSubscriber.log.Debug((object) "swsp_ReflowAllNodeChildStatus");
          using (SqlCommand textCommand = this.sqlHelper.GetTextCommand("EXEC dbo.[swsp_ReflowAllNodeChildStatus]"))
            this.sqlHelper.ExecuteNonQuery(textCommand);
          NodeChildStatusParticipationSubscriber.log.Debug((object) "Invoke Notification Event");
          EventHandler allNodeChildStatus = this.ReflowAllNodeChildStatus;
          if (allNodeChildStatus == null)
            return;
          allNodeChildStatus((object) this, EventArgs.Empty);
        }
      }
      catch (Exception ex)
      {
        NodeChildStatusParticipationSubscriber.log.Error((object) ex);
      }
    }
  }
}
