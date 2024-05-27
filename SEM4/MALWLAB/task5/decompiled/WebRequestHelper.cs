// Decompiled with JetBrains decompiler
// Type: SolarWinds.Orion.Core.BusinessLayer.WebRequestHelper
// Assembly: SolarWinds.Orion.Core.BusinessLayer, Version=2019.4.5200.9083, Culture=neutral, PublicKeyToken=null
// MVID: E12E8C85-5CD9-4E06-8801-182E5104FADE
// Assembly location: E:\task5.dll

using SolarWinds.Logging;
using SolarWinds.Orion.Core.Common.Configuration;
using System;
using System.Net;

#nullable disable
namespace SolarWinds.Orion.Core.BusinessLayer
{
  internal class WebRequestHelper
  {
    private static readonly Log _log = new Log();

    internal static HttpWebResponse SendHttpWebRequest(string query)
    {
      HttpWebRequest httpWebRequest = WebRequest.Create(query) as HttpWebRequest;
      httpWebRequest.Proxy = HttpProxySettings.Instance.AsWebProxy();
      httpWebRequest.Method = "GET";
      try
      {
        HttpWebResponse response = httpWebRequest.GetResponse() as HttpWebResponse;
        if (response.StatusCode == HttpStatusCode.OK)
          return response;
      }
      catch (Exception ex)
      {
        WebRequestHelper._log.ErrorFormat("Caught exception while trying to make http-request: {0}", (object) ex);
      }
      return (HttpWebResponse) null;
    }
  }
}
