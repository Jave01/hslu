// Decompiled with JetBrains decompiler
// Type: SolarWinds.Orion.Core.BusinessLayer.SettingsToRegistry
// Assembly: SolarWinds.Orion.Core.BusinessLayer, Version=2019.4.5200.9083, Culture=neutral, PublicKeyToken=null
// MVID: E12E8C85-5CD9-4E06-8801-182E5104FADE
// Assembly location: E:\task5.dll

using SolarWinds.Logging;
using SolarWinds.Orion.Common;
using System;

#nullable disable
namespace SolarWinds.Orion.Core.BusinessLayer
{
  internal class SettingsToRegistry
  {
    private static readonly Log log = new Log();
    internal Action<string, object> WriteToRegistry = (Action<string, object>) ((a, b) => OrionConfiguration.SetSetting(a, b));
    internal bool ThrowExceptions;

    public void Synchronize()
    {
      this.Synchronize((SynchronizeItem) new SettingItem("SWNetPerfMon-Settings-SNMP-SocketRecyclingInterval"), "SNMP_SocketRecyclingInterval");
      this.Synchronize((SynchronizeItem) new SettingItem("SWNetPerfMon-Settings-SNMP-SocketKeepAliveInterval"), "SNMP_SocketKeepAliveInterval");
    }

    public void Synchronize(SynchronizeItem item, string registryValueName)
    {
      try
      {
        SettingsToRegistry.log.VerboseFormat("Synchronize ... {0}", new object[1]
        {
          (object) item
        });
        object databaseValue = item.GetDatabaseValue();
        SettingsToRegistry.log.VerboseFormat("Synchronize ... {0} - value {1}", new object[2]
        {
          (object) item,
          databaseValue
        });
        if (databaseValue == null)
          return;
        this.WriteToRegistry(registryValueName, databaseValue);
      }
      catch (Exception ex)
      {
        SettingsToRegistry.log.Error((object) string.Format("Failed to synchronize {0}", (object) item), ex);
        if (!this.ThrowExceptions)
          return;
        throw;
      }
    }
  }
}
