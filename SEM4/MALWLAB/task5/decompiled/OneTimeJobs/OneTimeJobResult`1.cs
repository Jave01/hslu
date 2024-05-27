// Decompiled with JetBrains decompiler
// Type: SolarWinds.Orion.Core.BusinessLayer.OneTimeJobs.OneTimeJobResult`1
// Assembly: SolarWinds.Orion.Core.BusinessLayer, Version=2019.4.5200.9083, Culture=neutral, PublicKeyToken=null
// MVID: E12E8C85-5CD9-4E06-8801-182E5104FADE
// Assembly location: E:\task5.dll

#nullable disable
namespace SolarWinds.Orion.Core.BusinessLayer.OneTimeJobs
{
  public class OneTimeJobResult<T>
  {
    public bool Success { get; set; }

    public string Message { get; set; }

    public T Value { get; set; }
  }
}
