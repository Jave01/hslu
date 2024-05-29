// Decompiled with JetBrains decompiler
// Type: SolarWinds.Orion.Core.BusinessLayer.OneTimeJobs.IOneTimeJobService
// Assembly: SolarWinds.Orion.Core.BusinessLayer, Version=2019.4.5200.9083, Culture=neutral, PublicKeyToken=null
// MVID: E12E8C85-5CD9-4E06-8801-182E5104FADE
// Assembly location: E:\task5.dll

using SolarWinds.JobEngine;
using SolarWinds.JobEngine.Security;
using SolarWinds.Orion.Core.BusinessLayer.BL;
using System;

#nullable disable
namespace SolarWinds.Orion.Core.BusinessLayer.OneTimeJobs
{
  public interface IOneTimeJobService : IDisposable
  {
    void Start(string listenerUri);

    OneTimeJobResult<T> ExecuteJobAndGetResult<T>(
      int engineId,
      JobDescription jobDescription,
      CredentialBase jobCredential,
      JobResultDataFormatType resultDataFormat,
      string jobType)
      where T : class, new();

    OneTimeJobResult<T> ExecuteJobAndGetResult<T>(
      string engineName,
      JobDescription jobDescription,
      CredentialBase jobCredential,
      JobResultDataFormatType resultDataFormat,
      string jobType)
      where T : class, new();
  }
}
