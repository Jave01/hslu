﻿// Decompiled with JetBrains decompiler
// Type: SolarWinds.Orion.Core.BusinessLayer.OneTimeJobs.OneTimeJobService
// Assembly: SolarWinds.Orion.Core.BusinessLayer, Version=2019.4.5200.9083, Culture=neutral, PublicKeyToken=null
// MVID: E12E8C85-5CD9-4E06-8801-182E5104FADE
// Assembly location: E:\task5.dll

using SolarWinds.JobEngine;
using SolarWinds.JobEngine.Security;
using SolarWinds.Logging;
using SolarWinds.Orion.Core.BusinessLayer.BL;
using SolarWinds.Orion.Core.Common;
using SolarWinds.Orion.Core.Common.DALs;
using SolarWinds.Orion.Core.Strings;
using SolarWinds.Serialization.Json;
using System;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.Xml;
using System.Xml.Serialization;

#nullable disable
namespace SolarWinds.Orion.Core.BusinessLayer.OneTimeJobs
{
  public class OneTimeJobService : IOneTimeJobService, IDisposable
  {
    private static readonly Log log = new Log();
    private readonly IOneTimeJobManager oneTimeJobManager;
    private ServiceHost oneTimeJobsCallbackHost;
    private IEngineDAL engineDal;

    public OneTimeJobService()
      : this((IOneTimeJobManager) new OneTimeJobManager(), (IEngineDAL) new EngineDAL())
    {
    }

    public OneTimeJobService(IOneTimeJobManager oneTimeJobManager, IEngineDAL engineDal)
    {
      this.oneTimeJobManager = oneTimeJobManager ?? throw new ArgumentNullException(nameof (oneTimeJobManager));
      this.engineDal = engineDal ?? throw new ArgumentNullException(nameof (engineDal));
    }

    public void Start()
    {
      this.oneTimeJobsCallbackHost = new ServiceHost((object) this.oneTimeJobManager, Array.Empty<Uri>());
      this.oneTimeJobManager.SetListenerUri(this.oneTimeJobsCallbackHost.Description.Endpoints.First<ServiceEndpoint>().ListenUri.AbsoluteUri);
      this.oneTimeJobsCallbackHost.Open();
    }

    public void Start(string endpointAddress)
    {
      this.oneTimeJobsCallbackHost = new ServiceHost((object) this.oneTimeJobManager, Array.Empty<Uri>());
      NetNamedPipeBinding namedPipeBinding = new NetNamedPipeBinding();
      namedPipeBinding.SendTimeout = TimeSpan.FromMinutes(3.0);
      namedPipeBinding.TransferMode = TransferMode.StreamedResponse;
      namedPipeBinding.MaxBufferSize = int.MaxValue;
      namedPipeBinding.MaxReceivedMessageSize = (long) int.MaxValue;
      namedPipeBinding.ReaderQuotas = new XmlDictionaryReaderQuotas()
      {
        MaxStringContentLength = int.MaxValue,
        MaxArrayLength = int.MaxValue
      };
      this.oneTimeJobsCallbackHost.AddServiceEndpoint(typeof (IJobSchedulerEvents), (Binding) namedPipeBinding, endpointAddress);
      this.oneTimeJobManager.SetListenerUri(endpointAddress);
      this.oneTimeJobsCallbackHost.Open();
    }

    public OneTimeJobResult<T> ExecuteJobAndGetResult<T>(
      int engineId,
      JobDescription jobDescription,
      CredentialBase jobCredential,
      JobResultDataFormatType resultDataFormat,
      string jobType)
      where T : class, new()
    {
      return this.ExecuteJobAndGetResult<T>(this.engineDal.GetEngine(engineId).ServerName, jobDescription, jobCredential, resultDataFormat, jobType);
    }

    public OneTimeJobResult<T> ExecuteJobAndGetResult<T>(
      string engineName,
      JobDescription jobDescription,
      CredentialBase jobCredential,
      JobResultDataFormatType resultDataFormat,
      string jobType)
      where T : class, new()
    {
      this.RouteJobToEngine(jobDescription, engineName);
      using (OneTimeJobRawResult timeJobRawResult = this.oneTimeJobManager.ExecuteJob(jobDescription, jobCredential))
      {
        string error = timeJobRawResult.Error;
        if (!timeJobRawResult.Success)
        {
          OneTimeJobService.log.WarnFormat(jobType + " credential test failed: " + timeJobRawResult.Error, Array.Empty<object>());
          string messageFromException = this.GetLocalizedErrorMessageFromException(timeJobRawResult.ExceptionFromJob);
          return new OneTimeJobResult<T>()
          {
            Success = false,
            Message = string.IsNullOrEmpty(messageFromException) ? error : messageFromException
          };
        }
        try
        {
          T obj;
          if (resultDataFormat == JobResultDataFormatType.Xml)
          {
            using (XmlTextReader xmlTextReader = new XmlTextReader(timeJobRawResult.JobResultStream))
            {
              xmlTextReader.Namespaces = false;
              obj = (T) new XmlSerializer(typeof (T)).Deserialize((XmlReader) xmlTextReader);
            }
          }
          else
            obj = SerializationHelper.Deserialize<T>(timeJobRawResult.JobResultStream);
          return new OneTimeJobResult<T>()
          {
            Success = true,
            Value = obj
          };
        }
        catch (Exception ex)
        {
          OneTimeJobService.log.Error((object) string.Format("Failed to deserialize {0} credential test job result: {1}", (object) jobType, (object) ex));
          return new OneTimeJobResult<T>()
          {
            Success = false,
            Message = this.GetLocalizedErrorMessageFromException(timeJobRawResult.ExceptionFromJob)
          };
        }
      }
    }

    private void RouteJobToEngine(JobDescription jobDescription, string engineName)
    {
      if (!string.IsNullOrEmpty(jobDescription.LegacyEngine))
        return;
      jobDescription.LegacyEngine = engineName;
    }

    private string GetLocalizedErrorMessageFromException(Exception exception)
    {
      return exception is FaultException<JobEngineConnectionFault> ? Resources.LIBCODE_PS0_20 : string.Empty;
    }

    public void Dispose()
    {
      MessageUtilities.ShutdownCommunicationObject((ICommunicationObject) this.oneTimeJobsCallbackHost);
    }
  }
}
