// Decompiled with JetBrains decompiler
// Type: SolarWinds.Orion.Core.BusinessLayer.Thresholds.IThresholdIndicator
// Assembly: SolarWinds.Orion.Core.BusinessLayer, Version=2019.4.5200.9083, Culture=neutral, PublicKeyToken=null
// MVID: E12E8C85-5CD9-4E06-8801-182E5104FADE
// Assembly location: E:\task5.dll

using SolarWinds.Orion.Core.Common.Models.Thresholds;

#nullable disable
namespace SolarWinds.Orion.Core.BusinessLayer.Thresholds
{
  public interface IThresholdIndicator
  {
    void LoadPreviousThresholdData(int instanceId, string thresholdName);

    void ReportThresholdIndication(Threshold threshold);
  }
}
