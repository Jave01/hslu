// Decompiled with JetBrains decompiler
// Type: SolarWinds.Orion.Core.BusinessLayer.MaintUpdateNotifySvc.IMaintUpdateNotifySvc
// Assembly: SolarWinds.Orion.Core.BusinessLayer, Version=2019.4.5200.9083, Culture=neutral, PublicKeyToken=null
// MVID: E12E8C85-5CD9-4E06-8801-182E5104FADE
// Assembly location: E:\task5.dll

using System.CodeDom.Compiler;
using System.ServiceModel;
using System.Threading.Tasks;

#nullable disable
namespace SolarWinds.Orion.Core.BusinessLayer.MaintUpdateNotifySvc
{
  [GeneratedCode("System.ServiceModel", "4.0.0.0")]
  [ServiceContract(Namespace = "http://www.solarwinds.com/contracts/IMaintUpdateNotifySvc/2009/09", ConfigurationName = "MaintUpdateNotifySvc.IMaintUpdateNotifySvc")]
  public interface IMaintUpdateNotifySvc
  {
    [OperationContract(Action = "http://www.solarwinds.com/contracts/IMaintUpdateNotifySvc/2009/09/IMaintUpdateNotifySvc/GetData", ReplyAction = "http://www.solarwinds.com/contracts/IMaintUpdateNotifySvc/2009/09/IMaintUpdateNotifySvc/GetDataResponse")]
    [XmlSerializerFormat(SupportFaults = true)]
    UpdateResponse GetData(UpdateRequest request);

    [OperationContract(Action = "http://www.solarwinds.com/contracts/IMaintUpdateNotifySvc/2009/09/IMaintUpdateNotifySvc/GetData", ReplyAction = "http://www.solarwinds.com/contracts/IMaintUpdateNotifySvc/2009/09/IMaintUpdateNotifySvc/GetDataResponse")]
    Task<UpdateResponse> GetDataAsync(UpdateRequest request);

    [OperationContract(Action = "http://www.solarwinds.com/contracts/IMaintUpdateNotifySvc/2009/09/IMaintUpdateNotifySvc/GetLocalizedData", ReplyAction = "http://www.solarwinds.com/contracts/IMaintUpdateNotifySvc/2009/09/IMaintUpdateNotifySvc/GetLocalizedDataResponse")]
    [XmlSerializerFormat(SupportFaults = true)]
    UpdateResponse GetLocalizedData(UpdateRequest request, string locale);

    [OperationContract(Action = "http://www.solarwinds.com/contracts/IMaintUpdateNotifySvc/2009/09/IMaintUpdateNotifySvc/GetLocalizedData", ReplyAction = "http://www.solarwinds.com/contracts/IMaintUpdateNotifySvc/2009/09/IMaintUpdateNotifySvc/GetLocalizedDataResponse")]
    Task<UpdateResponse> GetLocalizedDataAsync(UpdateRequest request, string locale);
  }
}
