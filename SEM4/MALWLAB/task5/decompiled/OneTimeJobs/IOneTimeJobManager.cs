// Decompiled with JetBrains decompiler
// Type: SolarWinds.Orion.Core.BusinessLayer.OneTimeJobs.IOneTimeJobManager
// Assembly: SolarWinds.Orion.Core.BusinessLayer, Version=2019.4.5200.9083, Culture=neutral, PublicKeyToken=null
// MVID: E12E8C85-5CD9-4E06-8801-182E5104FADE
// Assembly location: E:\task5.dll

using SolarWinds.JobEngine;
using SolarWinds.JobEngine.Security;

#nullable disable
namespace SolarWinds.Orion.Core.BusinessLayer.OneTimeJobs
{
  public interface IOneTimeJobManager
  {
    OneTimeJobRawResult ExecuteJob(JobDescription jobDescription, CredentialBase jobCredential = null);

    void SetListenerUri(string listenerUri);
  }
}
