// Decompiled with JetBrains decompiler
// Type: SolarWinds.Orion.Core.BusinessLayer.ConfigurationSettings.WindowsServiceSettings
// Assembly: SolarWinds.Orion.Core.BusinessLayer, Version=2019.4.5200.9083, Culture=neutral, PublicKeyToken=null
// MVID: E12E8C85-5CD9-4E06-8801-182E5104FADE
// Assembly location: E:\task5.dll

using SolarWinds.Settings;

#nullable disable
namespace SolarWinds.Orion.Core.BusinessLayer.ConfigurationSettings
{
  internal class WindowsServiceSettings : SettingsBase
  {
    public static readonly WindowsServiceSettings Instance = new WindowsServiceSettings();
    [Setting(Default = 20000, AllowServerOverride = true, ServiceRestartDependencies = new string[] {"OrionModuleEngine"})]
    public int ServiceTimeout;

    private WindowsServiceSettings()
    {
    }
  }
}
