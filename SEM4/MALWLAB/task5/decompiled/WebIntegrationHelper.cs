// Decompiled with JetBrains decompiler
// Type: SolarWinds.Orion.Core.BusinessLayer.WebIntegrationHelper
// Assembly: SolarWinds.Orion.Core.BusinessLayer, Version=2019.4.5200.9083, Culture=neutral, PublicKeyToken=null
// MVID: E12E8C85-5CD9-4E06-8801-182E5104FADE
// Assembly location: E:\task5.dll

using SolarWinds.Orion.Core.Models.WebIntegration;
using SolarWinds.Orion.Web.Integration.Common.Models;

#nullable disable
namespace SolarWinds.Orion.Core.BusinessLayer
{
  internal static class WebIntegrationHelper
  {
    public static SupportCase ToSupportCase(this WebSupportCase webSupportCase)
    {
      return new SupportCase()
      {
        CaseNumber = webSupportCase.CaseNumber,
        CaseURL = webSupportCase.CaseURL,
        LastUpdated = webSupportCase.LastUpdated,
        Status = (CaseStatus) webSupportCase.Status,
        Title = webSupportCase.Title
      };
    }

    public static MaintenanceStatus ToMaintenanceStatus(
      this WebMaintenanceStatus webMaintenanceStatus)
    {
      return new MaintenanceStatus()
      {
        ExpirationDate = webMaintenanceStatus.ExpirationDate,
        ProductName = webMaintenanceStatus.ProductName,
        ShortName = webMaintenanceStatus.ShortName
      };
    }
  }
}
