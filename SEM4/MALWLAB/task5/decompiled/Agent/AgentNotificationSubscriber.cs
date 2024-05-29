// Decompiled with JetBrains decompiler
// Type: SolarWinds.Orion.Core.BusinessLayer.Agent.AgentNotificationSubscriber
// Assembly: SolarWinds.Orion.Core.BusinessLayer, Version=2019.4.5200.9083, Culture=neutral, PublicKeyToken=null
// MVID: E12E8C85-5CD9-4E06-8801-182E5104FADE
// Assembly location: E:\task5.dll

using SolarWinds.InformationService.Contract2;
using SolarWinds.InformationService.Contract2.PubSub;
using SolarWinds.Logging;
using SolarWinds.Orion.Core.Common.InformationService;
using SolarWinds.Orion.Core.Common.Interfaces;
using System;
using System.Collections.Generic;

#nullable disable
namespace SolarWinds.Orion.Core.BusinessLayer.Agent
{
  internal class AgentNotificationSubscriber : INotificationSubscriber
  {
    private static readonly Log log = new Log();
    private IInformationServiceSubscriptionProvider subscriptionProvider;
    private Action<int> onIndication;
    private List<string> subscriptionIds = new List<string>();
    private readonly string[] subscriptionQueries = new string[4]
    {
      "SUBSCRIBE CHANGES TO Orion.AgentManagement.Agent INCLUDE AgentId WHEN AgentStatus CHANGES OR ConnectionStatus CHANGES",
      "SUBSCRIBE CHANGES TO Orion.AgentManagement.Agent INCLUDE AgentId WHEN ADDED",
      "SUBSCRIBE CHANGES TO Orion.AgentManagement.AgentPlugin INCLUDE AgentId WHEN Status CHANGES",
      "SUBSCRIBE CHANGES TO Orion.AgentManagement.AgentPlugin INCLUDE AgentId WHEN ADDED"
    };

    public AgentNotificationSubscriber(Action<int> onIndication)
      : this(onIndication, (IInformationServiceSubscriptionProvider) InformationServiceSubscriptionProviderShared.Instance())
    {
    }

    public AgentNotificationSubscriber(
      Action<int> onIndication,
      IInformationServiceSubscriptionProvider subscriptionProvider)
    {
      this.onIndication = onIndication;
      this.subscriptionProvider = subscriptionProvider;
    }

    public void Subscribe()
    {
      this.Unsubscribe();
      foreach (string subscriptionQuery in this.subscriptionQueries)
        this.subscriptionIds.Add(this.subscriptionProvider.Subscribe(subscriptionQuery, (INotificationSubscriber) this));
    }

    public void Unsubscribe()
    {
      while (this.subscriptionIds.Count > 0)
      {
        string subscriptionId = this.subscriptionIds[0];
        if (!string.IsNullOrEmpty(subscriptionId))
        {
          try
          {
            this.subscriptionProvider.Unsubscribe(subscriptionId);
          }
          catch (Exception ex)
          {
            AgentNotificationSubscriber.log.ErrorFormat("Error unsubscribing 'agent change' subscription '{0}'. {1}", (object) subscriptionId, (object) ex);
          }
        }
        this.subscriptionIds.RemoveAt(0);
      }
    }

    public bool IsSubscribed() => this.subscriptionIds.Count > 0;

    public void OnIndication(
      string subscriptionId,
      string indicationType,
      PropertyBag indicationProperties,
      PropertyBag sourceInstanceProperties)
    {
      if (!this.subscriptionIds.Contains(subscriptionId))
        return;
      try
      {
        int int32 = Convert.ToInt32(((Dictionary<string, object>) sourceInstanceProperties)["AgentId"]);
        if (this.onIndication == null || int32 == 0)
          return;
        this.onIndication(int32);
      }
      catch (Exception ex)
      {
        AgentNotificationSubscriber.log.ErrorFormat("Error processing agent notification. {0}", (object) ex);
        throw;
      }
    }
  }
}
