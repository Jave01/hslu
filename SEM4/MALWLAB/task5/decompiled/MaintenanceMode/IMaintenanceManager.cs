// Decompiled with JetBrains decompiler
// Type: SolarWinds.Orion.Core.BusinessLayer.MaintenanceMode.IMaintenanceManager
// Assembly: SolarWinds.Orion.Core.BusinessLayer, Version=2019.4.5200.9083, Culture=neutral, PublicKeyToken=null
// MVID: E12E8C85-5CD9-4E06-8801-182E5104FADE
// Assembly location: E:\task5.dll

using SolarWinds.Orion.Core.Models.MaintenanceMode;

#nullable disable
namespace SolarWinds.Orion.Core.BusinessLayer.MaintenanceMode
{
  public interface IMaintenanceManager
  {
    void Unmanage(MaintenancePlanAssignment assignment);

    void Remanage(MaintenancePlanAssignment assignment);
  }
}
