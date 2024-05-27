// Decompiled with JetBrains decompiler
// Type: SolarWinds.Orion.Core.BusinessLayer.NodeStatus.EnhancedNodeStatusCalculationSubscriber
// Assembly: SolarWinds.Orion.Core.BusinessLayer, Version=2019.4.5200.9083, Culture=neutral, PublicKeyToken=null
// MVID: E12E8C85-5CD9-4E06-8801-182E5104FADE
// Assembly location: E:\task5.dll

using SolarWinds.Logging;
using SolarWinds.Orion.Core.Common;
using SolarWinds.Orion.PubSub;
using System;
using System.Data.SqlClient;
using System.Threading.Tasks;

#nullable disable
namespace SolarWinds.Orion.Core.BusinessLayer.NodeStatus
{
  public class EnhancedNodeStatusCalculationSubscriber : ISubscriber, IDisposable
  {
    public static string SubscriptionUniqueName = "EnhancedNodeStatusCalculation";
    private static string SubscriptionQuery = "SUBSCRIBE CHANGES TO Orion.Settings WHEN SettingsID = 'EnhancedNodeStatusCalculation'";
    private static readonly Log log = new Log();
    private readonly ISubscriptionManager subscriptionManager;
    private readonly ISqlHelper sqlHelper;
    private ISubscription subscription;

    public EnhancedNodeStatusCalculationSubscriber(
      ISubscriptionManager subscriptionManager,
      ISqlHelper sqlHelper)
    {
      this.subscriptionManager = subscriptionManager ?? throw new ArgumentNullException(nameof (subscriptionManager));
      this.sqlHelper = sqlHelper ?? throw new ArgumentNullException(nameof (sqlHelper));
    }

    public async Task OnNotificationAsync(Notification notification)
    {
      EnhancedNodeStatusCalculationSubscriber calculationSubscriber = this;
      if (calculationSubscriber.subscription == null)
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
      string subscriptionUniqueName = EnhancedNodeStatusCalculationSubscriber.SubscriptionUniqueName;
      if (str != subscriptionUniqueName)
        return;
      if (notification.SourceInstanceProperties == null)
        EnhancedNodeStatusCalculationSubscriber.log.Error((object) "Argument SourceInstanceProperties is null.");
      else if (!notification.SourceInstanceProperties.ContainsKey("CurrentValue"))
      {
        EnhancedNodeStatusCalculationSubscriber.log.Error((object) "CurrentValue not supplied in SourceInstanceProperties.");
      }
      else
      {
        try
        {
          bool flag = Convert.ToInt32(notification.SourceInstanceProperties["CurrentValue"]) == 1;
          EnhancedNodeStatusCalculationSubscriber.log.DebugFormat("Node status calculation changed to '{0} calculation', re-calculating node status ..", flag ? (object) "Enhanced" : (object) "Classic");
          // ISSUE: reference to a compiler-generated method
          await Task.Run(new Action(calculationSubscriber.\u003COnNotificationAsync\u003Eb__7_0));
        }
        catch (Exception ex)
        {
          EnhancedNodeStatusCalculationSubscriber.log.Error((object) "Indication handling failed", ex);
        }
      }
    }

    public EnhancedNodeStatusCalculationSubscriber Start()
    {
      EnhancedNodeStatusCalculationSubscriber.log.Debug((object) "Subscribing EnhancedNodeStatusCalculation changed indications..");
      try
      {
        if (this.subscription != null)
        {
          EnhancedNodeStatusCalculationSubscriber.log.Debug((object) "Already subscribed, unsubscribing first..");
          this.Unsubscribe(this.subscription.Id);
        }
        SubscriptionId subscriptionId;
        // ISSUE: explicit constructor call
        ((SubscriptionId) ref subscriptionId).\u002Ector("Core", EnhancedNodeStatusCalculationSubscriber.SubscriptionUniqueName, (Scope) 0);
        this.subscription = this.subscriptionManager.Subscribe(subscriptionId, (ISubscriber) this, new SubscriberConfiguration()
        {
          SubscriptionQuery = EnhancedNodeStatusCalculationSubscriber.SubscriptionQuery,
          ReliableDelivery = true,
          AcknowledgeMode = (AcknowledgeMode) 0,
          MessageTimeToLive = TimeSpan.Zero
        });
        return this;
      }
      catch (Exception ex)
      {
        EnhancedNodeStatusCalculationSubscriber.log.Error((object) "Failed to subscribe.", ex);
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
        EnhancedNodeStatusCalculationSubscriber.log.Debug((object) "Unsubscribing EnhancedNodeStatusCalculation changed indications..");
        this.Unsubscribe(this.subscription.Id);
        this.subscription = (ISubscription) null;
      }
      catch (Exception ex)
      {
        EnhancedNodeStatusCalculationSubscriber.log.Error((object) "Error unsubscribing subscription.", ex);
      }
    }

    private void Unsubscribe(SubscriptionId subscriptionId)
    {
      this.subscriptionManager.Unsubscribe(subscriptionId);
    }

    private void RecalculateNodeStatus()
    {
      using (SqlCommand textCommand = this.sqlHelper.GetTextCommand("EXEC dbo.[swsp_ReflowAllNodeChildStatus]"))
        this.sqlHelper.ExecuteNonQuery(textCommand);
    }
  }
}
