// Decompiled with JetBrains decompiler
// Type: SolarWinds.Orion.Core.BusinessLayer.Thresholds.MinMaxCalculator
// Assembly: SolarWinds.Orion.Core.BusinessLayer, Version=2019.4.5200.9083, Culture=neutral, PublicKeyToken=null
// MVID: E12E8C85-5CD9-4E06-8801-182E5104FADE
// Assembly location: E:\task5.dll

using SolarWinds.Orion.Core.Common.Models.Thresholds;
using System;

#nullable disable
namespace SolarWinds.Orion.Core.BusinessLayer.Thresholds
{
  internal class MinMaxCalculator
  {
    public ThresholdMinMaxValue Calculate(StatisticalData[] values)
    {
      if (values.Length == 0)
        return new ThresholdMinMaxValue()
        {
          Max = 0.0,
          Min = 0.0
        };
      double val1_1 = values[0].Value;
      double val1_2 = values[0].Value;
      for (int index = 1; index < values.Length; ++index)
      {
        val1_1 = Math.Min(val1_1, values[index].Value);
        val1_2 = Math.Max(val1_2, values[index].Value);
      }
      return new ThresholdMinMaxValue()
      {
        Min = val1_1,
        Max = val1_2
      };
    }
  }
}
