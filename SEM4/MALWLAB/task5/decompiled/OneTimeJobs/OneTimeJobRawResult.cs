// Decompiled with JetBrains decompiler
// Type: SolarWinds.Orion.Core.BusinessLayer.OneTimeJobs.OneTimeJobRawResult
// Assembly: SolarWinds.Orion.Core.BusinessLayer, Version=2019.4.5200.9083, Culture=neutral, PublicKeyToken=null
// MVID: E12E8C85-5CD9-4E06-8801-182E5104FADE
// Assembly location: E:\task5.dll

using System;
using System.IO;

#nullable disable
namespace SolarWinds.Orion.Core.BusinessLayer.OneTimeJobs
{
  public struct OneTimeJobRawResult : IDisposable
  {
    public bool Success { get; set; }

    public string Error { get; set; }

    public Stream JobResultStream { get; set; }

    public Exception ExceptionFromJob { get; set; }

    public void Dispose() => this.JobResultStream?.Dispose();
  }
}
