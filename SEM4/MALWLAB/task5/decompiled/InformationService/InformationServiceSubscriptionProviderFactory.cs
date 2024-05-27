// Decompiled with JetBrains decompiler
// Type: SolarWinds.Orion.Core.BusinessLayer.InformationService.InformationServiceSubscriptionProviderFactory
// Assembly: SolarWinds.Orion.Core.BusinessLayer, Version=2019.4.5200.9083, Culture=neutral, PublicKeyToken=null
// MVID: E12E8C85-5CD9-4E06-8801-182E5104FADE
// Assembly location: E:\task5.dll

using SolarWinds.Orion.Core.Common.Interfaces;

#nullable disable
namespace SolarWinds.Orion.Core.BusinessLayer.InformationService
{
  public class InformationServiceSubscriptionProviderFactory
  {
    public static IInformationServiceSubscriptionProvider GetInformationServiceSubscriptionProviderFactory(
      string netObjectOperationEndpoint)
    {
      return (IInformationServiceSubscriptionProvider) new InformationServiceSubscriptionProvider(netObjectOperationEndpoint);
    }

    public static IInformationServiceSubscriptionProvider GetInformationServiceSubscriptionProviderFactoryV3(
      string netObjectOperationEndpoint)
    {
      return (IInformationServiceSubscriptionProvider) InformationServiceSubscriptionProvider.CreateV3(netObjectOperationEndpoint);
    }
  }
}
