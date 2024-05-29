// Decompiled with JetBrains decompiler
// Type: SolarWinds.Orion.Core.BusinessLayer.DiscoveryResultManager
// Assembly: SolarWinds.Orion.Core.BusinessLayer, Version=2019.4.5200.9083, Culture=neutral, PublicKeyToken=null
// MVID: E12E8C85-5CD9-4E06-8801-182E5104FADE
// Assembly location: E:\task5.dll

using SolarWinds.Logging;
using SolarWinds.Orion.Core.Discovery.DataAccess;
using SolarWinds.Orion.Core.Models.Discovery;
using SolarWinds.Orion.Discovery.Contract.DiscoveryPlugin;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using System.Xml;

#nullable disable
namespace SolarWinds.Orion.Core.BusinessLayer
{
  public static class DiscoveryResultManager
  {
    private static readonly Log log = new Log();

    public static DiscoveryResultBase GetDiscoveryResult(
      int profileId,
      IList<IDiscoveryPlugin> discoveryPlugins)
    {
      if (discoveryPlugins == null)
        throw new ArgumentNullException(nameof (discoveryPlugins));
      if (profileId <= 0)
        throw new ArgumentException(string.Format("Invalid profile ID [{0}]", (object) profileId));
      DiscoveryResultBase discoveryResult = new DiscoveryResultBase();
      try
      {
        DiscoveryProfileEntry profileById = DiscoveryProfileEntry.GetProfileByID(profileId);
        discoveryResult.EngineId = profileById.EngineID;
        discoveryResult.ProfileID = profileById.ProfileID;
      }
      catch (Exception ex)
      {
        string message = string.Format("Unable to load profile {0}", (object) profileId);
        DiscoveryResultManager.log.Error((object) message, ex);
        throw new Exception(message, ex);
      }
      if (discoveryPlugins.Count == 0)
        return discoveryResult;
      int millisecondsTimeout = 300000;
      bool flag = Environment.StackTrace.Contains("ServiceModel");
      if (flag)
      {
        try
        {
          System.Configuration.Configuration configuration = ConfigurationManager.OpenExeConfiguration(Assembly.GetExecutingAssembly().Location);
          XmlDocument xmlDocument = new XmlDocument();
          xmlDocument.Load(configuration.FilePath);
          XmlNode xmlNode = xmlDocument.SelectSingleNode("/configuration/system.serviceModel/bindings/netTcpBinding/binding[@name=\"Core.NetTcpBinding\"]");
          if (xmlNode != null)
          {
            if (xmlNode.Attributes != null)
              millisecondsTimeout = (int) TimeSpan.Parse(xmlNode.Attributes["receiveTimeout"].Value).TotalMilliseconds;
          }
        }
        catch (Exception ex)
        {
          DiscoveryResultManager.log.Warn((object) "Unable to read WCF timeout from Config file.");
        }
      }
      Thread thread = new Thread(new ParameterizedThreadStart(DiscoveryResultManager.LoadResults));
      DiscoveryResultManager.LoadResultsArgs parameter = new DiscoveryResultManager.LoadResultsArgs()
      {
        discoveryPlugins = discoveryPlugins,
        profileId = profileId,
        result = discoveryResult
      };
      thread.Start((object) parameter);
      if (flag)
      {
        if (!thread.Join(millisecondsTimeout))
        {
          DiscoveryResultManager.log.Error((object) "Loading results takes more time than WCF timeout is set. Enable debug logging to see which plugin takes too long.");
          return discoveryResult;
        }
      }
      else
        thread.Join();
      DiscoveryResultBase result = parameter.result;
      DiscoveryFilterResultByTechnology.FilterByPriority(result, TechnologyManager.Instance);
      Stopwatch stopwatch = Stopwatch.StartNew();
      List<DiscoveryPluginResultBase> list = ((IEnumerable<DiscoveryPluginResultBase>) result.PluginResults).ToList<DiscoveryPluginResultBase>();
      result.PluginResults.Clear();
      foreach (DiscoveryPluginResultBase pluginResultBase in list)
        result.PluginResults.Add(pluginResultBase.GetFilteredPluginResult());
      DiscoveryResultManager.log.DebugFormat("Filtering results took {0} milliseconds.", (object) stopwatch.ElapsedMilliseconds);
      GC.Collect();
      return result;
    }

    private static void LoadResults(object args)
    {
      DiscoveryResultManager.LoadResultsArgs loadResultsArgs = (DiscoveryResultManager.LoadResultsArgs) args;
      Stopwatch stopwatch = new Stopwatch();
      foreach (IDiscoveryPlugin discoveryPlugin in (IEnumerable<IDiscoveryPlugin>) loadResultsArgs.discoveryPlugins)
      {
        stopwatch.Restart();
        DiscoveryResultManager.log.DebugFormat("Loading results from plugin {0}", (object) discoveryPlugin.GetType());
        DiscoveryPluginResultBase pluginResultBase = discoveryPlugin.LoadResults(loadResultsArgs.profileId);
        DiscoveryResultManager.log.DebugFormat("Loading results from plugin {0} took {1} milliseconds.", (object) discoveryPlugin.GetType(), (object) stopwatch.ElapsedMilliseconds);
        if (pluginResultBase == null)
          throw new Exception(string.Format("unable to get valid result for plugin {0}", (object) discoveryPlugin.GetType()));
        pluginResultBase.PluginTypeName = discoveryPlugin.GetType().FullName;
        loadResultsArgs.result.PluginResults.Add(pluginResultBase);
      }
    }

    private static string GetFilename(Type type)
    {
      return string.Format("C:\\{1}{0}.dat", (object) Guid.NewGuid(), (object) type);
    }

    private static void XmlSerializer(DiscoveryResultBase data)
    {
      Type type = typeof (DiscoveryResultBase);
      using (FileStream fileStream = new FileStream(DiscoveryResultManager.GetFilename(type), FileMode.Create))
      {
        new DataContractSerializer(type).WriteObject(XmlDictionaryWriter.CreateTextWriter((Stream) fileStream), (object) data);
        fileStream.Flush();
        fileStream.Close();
      }
    }

    private static void BinarySerializer(DiscoveryResultBase data)
    {
      FileStream serializationStream = new FileStream(DiscoveryResultManager.GetFilename(typeof (DiscoveryResultBase)), FileMode.Create);
      BinaryFormatter binaryFormatter = new BinaryFormatter();
      try
      {
        binaryFormatter.Serialize((Stream) serializationStream, (object) data);
      }
      finally
      {
        serializationStream.Close();
      }
    }

    private class LoadResultsArgs
    {
      public int profileId;
      public IList<IDiscoveryPlugin> discoveryPlugins;
      public DiscoveryResultBase result;
    }
  }
}
