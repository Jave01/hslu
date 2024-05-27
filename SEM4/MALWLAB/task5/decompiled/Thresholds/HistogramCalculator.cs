// Decompiled with JetBrains decompiler
// Type: SolarWinds.Orion.Core.BusinessLayer.Thresholds.HistogramCalculator
// Assembly: SolarWinds.Orion.Core.BusinessLayer, Version=2019.4.5200.9083, Culture=neutral, PublicKeyToken=null
// MVID: E12E8C85-5CD9-4E06-8801-182E5104FADE
// Assembly location: E:\task5.dll

using SolarWinds.Orion.Core.Common.Models;
using SolarWinds.Orion.Core.Common.Models.Thresholds;
using System;

#nullable disable
namespace SolarWinds.Orion.Core.BusinessLayer.Thresholds
{
  internal class HistogramCalculator
  {
    public StatisticalDataHistogram[] CreateHistogram(
      StatisticalData[] data,
      TimeFrame[] timeFrames,
      int intervalsCount)
    {
      ThresholdMinMaxValue minMax = new MinMaxCalculator().Calculate(data);
      return HistogramCalculator.CreateBucketsAndHistogram(data, timeFrames, intervalsCount, minMax);
    }

    public StatisticalDataHistogram[] CreateHistogramWithScaledInterval(
      StatisticalData[] data,
      TimeFrame[] timeFrames,
      int intervalsCount,
      Type dataType)
    {
      ThresholdMinMaxValue minMax = new MinMaxCalculator().Calculate(data);
      if (dataType != (Type) null && dataType == typeof (int))
      {
        int val1 = (int) (minMax.Max - minMax.Min);
        if (val1 == 0)
          val1 = 1;
        intervalsCount = Math.Min(val1, intervalsCount);
      }
      return HistogramCalculator.CreateBucketsAndHistogram(data, timeFrames, intervalsCount, minMax);
    }

    private static StatisticalDataHistogram[] CreateBucketsAndHistogram(
      StatisticalData[] data,
      TimeFrame[] timeFrames,
      int intervalsCount,
      ThresholdMinMaxValue minMax)
    {
      Bucket[] buckets = new Bucketizer().CreateBuckets(intervalsCount, minMax);
      StatisticalDataHistogram[] pointsFromBuckets = HistogramCalculator.CreateHistogramsPointsFromBuckets(HistogramCalculator.CreateHistogramForTimeFrames(timeFrames, buckets.Length), buckets);
      return HistogramCalculator.CalculatePointsFrequencyFromStatistics(data, pointsFromBuckets);
    }

    private static StatisticalDataHistogram[] CalculatePointsFrequencyFromStatistics(
      StatisticalData[] values,
      StatisticalDataHistogram[] histograms)
    {
      for (int index1 = 0; index1 < values.Length; ++index1)
      {
        StatisticalData statisticalData = values[index1];
        for (int index2 = 0; index2 < histograms.Length; ++index2)
        {
          if (histograms[index2].TimeFrame.IsInTimeFrame(statisticalData.Date))
            HistogramCalculator.IncrementPointFrequency(histograms[index2], statisticalData.Value);
        }
      }
      return histograms;
    }

    private static void IncrementPointFrequency(StatisticalDataHistogram histograms, double value)
    {
      for (int index = 0; index < histograms.DataPoints.Length; ++index)
      {
        if (histograms.DataPoints[index].EndValue >= value)
        {
          ++histograms.DataPoints[index].Frequency;
          break;
        }
      }
    }

    private static StatisticalDataHistogram[] CreateHistogramsPointsFromBuckets(
      StatisticalDataHistogram[] histograms,
      Bucket[] buckets)
    {
      for (int index1 = 0; index1 < buckets.Length; ++index1)
      {
        for (int index2 = 0; index2 < histograms.Length; ++index2)
          histograms[index2].DataPoints[index1] = new HistogramDataPoint(buckets[index1].MinValue, buckets[index1].MaxValue);
      }
      return histograms;
    }

    private static StatisticalDataHistogram[] CreateHistogramForTimeFrames(
      TimeFrame[] timeFrames,
      int bucketsCount)
    {
      StatisticalDataHistogram[] histogramForTimeFrames = new StatisticalDataHistogram[timeFrames.Length];
      for (int index = 0; index < timeFrames.Length; ++index)
        histogramForTimeFrames[index] = new StatisticalDataHistogram(timeFrames[index], bucketsCount);
      return histogramForTimeFrames;
    }
  }
}
