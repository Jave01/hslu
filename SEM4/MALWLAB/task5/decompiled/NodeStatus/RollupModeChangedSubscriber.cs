// Decompiled with JetBrains decompiler
// Type: SolarWinds.Orion.Core.BusinessLayer.NodeStatus.RollupModeChangedSubscriber
// Assembly: SolarWinds.Orion.Core.BusinessLayer, Version=2019.4.5200.9083, Culture=neutral, PublicKeyToken=null
// MVID: E12E8C85-5CD9-4E06-8801-182E5104FADE
// Assembly location: E:\task5.dll

using SolarWinds.Logging;
using SolarWinds.Orion.Core.Common;
using SolarWinds.Orion.PubSub;
using SolarWinds.Shared;
using System;
using System.Data.SqlClient;
using System.Threading.Tasks;

#nullable disable
namespace SolarWinds.Orion.Core.BusinessLayer.NodeStatus
{
  public class RollupModeChangedSubscriber : ISubscriber, IDisposable
  {
    public static string SubscriptionUniqueName = "RollupModeChanged";
    private static string SubscriptionQuery = "SUBSCRIBE CHANGES TO Orion.Nodes WHEN [Core.StatusRollupMode] CHANGES";
    private static readonly Log log = new Log();
    private readonly ISubscriptionManager subscriptionManager;
    private readonly ISqlHelper sqlHelper;
    private ISubscription subscription;

    public RollupModeChangedSubscriber(
      ISubscriptionManager subscriptionManager,
      ISqlHelper sqlHelper)
    {
      this.subscriptionManager = subscriptionManager ?? throw new ArgumentNullException(nameof (subscriptionManager));
      this.sqlHelper = sqlHelper ?? throw new ArgumentNullException(nameof (sqlHelper));
    }

    public async Task OnNotificationAsync(Notification notification)
    {
      RollupModeChangedSubscriber changedSubscriber1 = this;
      if (changedSubscriber1.subscription == null)
        return;
      Notification notification1 = notification;
      string str;
      if (notification1 == null)
      {
        str = (string) null;
      }
      else
      {
        SubscriptionId subscriptionId = notification1.SubscriptionId;
        str = ((SubscriptionId) ref subscriptionId).UniqueName;
      }
      string subscriptionUniqueName = RollupModeChangedSubscriber.SubscriptionUniqueName;
      if (str != subscriptionUniqueName)
        return;
      if (notification.SourceInstanceProperties == null)
        RollupModeChangedSubscriber.log.Error((object) "Argument SourceInstanceProperties is null.");
      else if (!notification.SourceInstanceProperties.ContainsKey("Core.StatusRollupMode"))
      {
        RollupModeChangedSubscriber.log.Error((object) "Core.StatusRollupMode not supplied in SourceInstanceProperties.");
      }
      else
      {
        try
        {
          RollupModeChangedSubscriber changedSubscriber = changedSubscriber1;
          string instanceProperty = (string) notification.SourceInstanceProperties["Core.StatusRollupMode"];
          EvaluationMethod int32 = instanceProperty != null ? (EvaluationMethod) Convert.ToInt32(instanceProperty) : (EvaluationMethod) 0;
          int nodeId = Convert.ToInt32(notification.SourceInstanceProperties["NodeId"]);
          RollupModeChangedSubscriber.log.DebugFormat("Node with id '{0}' rollup mode changed to '{1}', re-calculating node status ..", (object) nodeId, (object) int32);
          await Task.Run((Action) (() => changedSubscriber.RecalculateNodeStatus(nodeId)));
        }
        catch (Exception ex)
        {
          RollupModeChangedSubscriber.log.Error((object) "Indication handling failed", ex);
        }
      }
    }

    public RollupModeChangedSubscriber Start()
    {
      RollupModeChangedSubscriber.log.Debug((object) "Subscribing RollupMode changed indications..");
      try
      {
        if (this.subscription != null)
        {
          RollupModeChangedSubscriber.log.Debug((object) "Already subscribed, unsubscribing first..");
          this.Unsubscribe(this.subscription.Id);
        }
        SubscriptionId subscriptionId;
        // ISSUE: explicit constructor call
        ((SubscriptionId) ref subscriptionId).\u002Ector("Core", RollupModeChangedSubscriber.SubscriptionUniqueName, (Scope) 0);
        this.subscription = this.subscriptionManager.Subscribe(subscriptionId, (ISubscriber) this, new SubscriberConfiguration()
        {
          SubscriptionQuery = RollupModeChangedSubscriber.SubscriptionQuery,
          ReliableDelivery = true,
          AcknowledgeMode = (AcknowledgeMode) 0,
          MessageTimeToLive = TimeSpan.Zero
        });
        return this;
      }
      catch (Exception ex)
      {
        RollupModeChangedSubscriber.log.Error((object) "Failed to subscribe.", ex);
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
      if (this.subscription == null)
        return;
      try
      {
        RollupModeChangedSubscriber.log.Debug((object) "Unsubscribing RollupMode changed indications..");
        this.Unsubscribe(this.subscription.Id);
        this.subscription = (ISubscription) null;
      }
      catch (Exception ex)
      {
        RollupModeChangedSubscriber.log.Error((object) "Error unsubscribing subscription.", ex);
      }
    }

    private void Unsubscribe(SubscriptionId subscriptionId)
    {
      this.subscriptionManager.Unsubscribe(subscriptionId);
    }

    private void RecalculateNodeStatus(int nodeId)
    {
      using (SqlCommand textCommand = this.sqlHelper.GetTextCommand("EXEC dbo.[swsp_ReflowNodeChildStatus] @nodeId"))
      {
        textCommand.Parameters.Add(new SqlParameter("@nodeId", (object) nodeId));
        this.sqlHelper.ExecuteNonQuery(textCommand);
      }
    }
  }
}
