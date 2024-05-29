// Decompiled with JetBrains decompiler
// Type: SolarWinds.Orion.Core.BusinessLayer.Thresholds.Bucketizer
// Assembly: SolarWinds.Orion.Core.BusinessLayer, Version=2019.4.5200.9083, Culture=neutral, PublicKeyToken=null
// MVID: E12E8C85-5CD9-4E06-8801-182E5104FADE
// Assembly location: E:\task5.dll

using SolarWinds.Orion.Core.Common.Models.Thresholds;
using SolarWinds.Orion.Core.Common.Thresholds;
using System;
using System.Collections.Generic;
using System.Linq;

#nullable disable
namespace SolarWinds.Orion.Core.BusinessLayer.Thresholds
{
  internal class Bucketizer
  {
    public Bucket[] CreateBuckets(int bucketsCount, ThresholdMinMaxValue minMax)
    {
      if (bucketsCount == 0 || minMax.Max < minMax.Min)
        return new Bucket[0];
      Bucket[] source = new Bucket[bucketsCount];
      double num = (minMax.Max - minMax.Min) / (double) bucketsCount;
      double maxValue = ThresholdsHelper.RoundThresholdsValue(minMax.Min);
      for (int index = 0; index < bucketsCount; ++index)
      {
        double minValue = maxValue;
        maxValue = ThresholdsHelper.RoundThresholdsValue(maxValue + num);
        source[index] = new Bucket(minValue, maxValue);
      }
      source[source.Length - 1].MaxValue = minMax.Max;
      double referalMinValue = ThresholdsHelper.RoundThresholdsValue(minMax.Min);
      if (!((IEnumerable<Bucket>) source).All<Bucket>((Func<Bucket, bool>) (bucket => bucket.MinValue.Equals(referalMinValue))))
        return source;
      return new Bucket[1]{ source[source.Length - 1] };
    }
  }
}
