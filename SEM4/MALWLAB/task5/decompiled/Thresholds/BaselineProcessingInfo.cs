// Decompiled with JetBrains decompiler
// Type: SolarWinds.Orion.Core.BusinessLayer.Thresholds.BaselineProcessingInfo
// Assembly: SolarWinds.Orion.Core.BusinessLayer, Version=2019.4.5200.9083, Culture=neutral, PublicKeyToken=null
// MVID: E12E8C85-5CD9-4E06-8801-182E5104FADE
// Assembly location: E:\task5.dll

using SolarWinds.Orion.Core.Common.Models.Thresholds;
using System;

#nullable disable
namespace SolarWinds.Orion.Core.BusinessLayer.Thresholds
{
  internal class BaselineProcessingInfo
  {
    private readonly Threshold _threshold;
    private readonly BaselineValues _baselineValues;

    public BaselineProcessingInfo(Threshold threshold, BaselineValues baselineValues)
    {
      if (threshold == null)
        throw new ArgumentNullException(nameof (threshold));
      if (baselineValues == null)
        throw new ArgumentNullException(nameof (baselineValues));
      this._threshold = threshold;
      this._baselineValues = baselineValues;
    }

    public Threshold Threshold => this._threshold;

    public BaselineValues BaselineValues => this._baselineValues;
  }
}
