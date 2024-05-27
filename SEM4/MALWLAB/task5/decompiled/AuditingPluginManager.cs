// Decompiled with JetBrains decompiler
// Type: SolarWinds.Orion.Core.BusinessLayer.AuditingPluginManager
// Assembly: SolarWinds.Orion.Core.BusinessLayer, Version=2019.4.5200.9083, Culture=neutral, PublicKeyToken=null
// MVID: E12E8C85-5CD9-4E06-8801-182E5104FADE
// Assembly location: E:\task5.dll

using SolarWinds.Logging;
using SolarWinds.Orion.Core.Common;
using SolarWinds.Orion.Core.Common.Indications;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;

#nullable disable
namespace SolarWinds.Orion.Core.BusinessLayer
{
  internal sealed class AuditingPluginManager
  {
    private static readonly Log log = new Log();
    private readonly List<IAuditing2> auditingInstances = new List<IAuditing2>();
    private ReadOnlyCollection<IAuditing2> auditingInstancesReadOnly;
    private bool init;
    private Dictionary<string, List<IAuditing2>> cacheTypeInstances = new Dictionary<string, List<IAuditing2>>();
    private Dictionary<string, IEnumerable<IAuditing2>> cacheTypeInstancesReadOnly = new Dictionary<string, IEnumerable<IAuditing2>>();

    static AuditingPluginManager()
    {
      AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(AssemblySatelliteResolver.AssemblyResolve);
      AppDomain.CurrentDomain.AssemblyLoad += new AssemblyLoadEventHandler(AssemblySatelliteResolver.AssemblyLoad);
    }

    public ReadOnlyCollection<IAuditing2> AuditingInstances
    {
      get
      {
        if (!this.init)
          throw new InvalidOperationException("Object has not been initialized yet. Call Start method before using.");
        return this.auditingInstancesReadOnly;
      }
    }

    public IAuditing2 GetAuditingInstancesOfActionType(AuditActionType actionType)
    {
      if (!this.init)
        throw new InvalidOperationException("Object has not been initialized yet. Call Start method before using.");
      foreach (KeyValuePair<string, IEnumerable<IAuditing2>> keyValuePair in this.cacheTypeInstancesReadOnly)
      {
        foreach (IAuditing2 instancesOfActionType in keyValuePair.Value)
        {
          if (instancesOfActionType != null && ((IAuditing) instancesOfActionType).SupportedActionTypes.Any<AuditActionType>((Func<AuditActionType, bool>) (supportedType => AuditActionType.op_Equality(supportedType, actionType))))
            return instancesOfActionType;
        }
      }
      return (IAuditing2) null;
    }

    [Obsolete("Core-Split cleanup. If you need this member please contact Core team", true)]
    public IEnumerable<IAuditing2> GetAuditingInstancesOfType(string type)
    {
      if (!this.init)
        throw new InvalidOperationException("Object has not been initialized yet. Call Start method before using.");
      if (this.cacheTypeInstancesReadOnly.ContainsKey(type))
        return this.cacheTypeInstancesReadOnly[type];
      AuditingPluginManager.log.ErrorFormat("Cache does not contain requested key {0}.", (object) type);
      return Enumerable.Empty<IAuditing2>();
    }

    public void Initialize()
    {
      this.LoadPlugins();
      this.auditingInstancesReadOnly = this.auditingInstances.AsReadOnly();
      foreach (IAuditing2 iauditing2 in this.auditingInstancesReadOnly)
      {
        string supportedIndicationType = ((IAuditing) iauditing2).SupportedIndicationType;
        if (this.cacheTypeInstances.ContainsKey(supportedIndicationType))
          this.cacheTypeInstances[supportedIndicationType].Add(iauditing2);
        else
          this.cacheTypeInstances.Add(supportedIndicationType, new List<IAuditing2>()
          {
            iauditing2
          });
      }
      foreach (KeyValuePair<string, List<IAuditing2>> cacheTypeInstance in this.cacheTypeInstances)
        this.cacheTypeInstancesReadOnly.Add(cacheTypeInstance.Key, (IEnumerable<IAuditing2>) cacheTypeInstance.Value);
      this.init = true;
    }

    private IEnumerable<Type> FindDerivedTypes(Assembly assembly)
    {
      if (AuditingPluginManager.log.IsTraceEnabled)
        AuditingPluginManager.log.Trace((object) ("Processing assembly " + assembly.FullName));
      Type[] types;
      try
      {
        if (AuditingPluginManager.log.IsTraceEnabled)
          AuditingPluginManager.log.Trace((object) "Calling GetTypes()");
        types = assembly.GetTypes();
      }
      catch (ReflectionTypeLoadException ex)
      {
        AuditingPluginManager.log.Warn((object) "Caught ReflectionTypeLoadException. Trying to get types from exception details.");
        foreach (Exception loaderException in ex.LoaderExceptions)
          AuditingPluginManager.log.Warn((object) ("LoaderException message: " + loaderException.Message + (loaderException is TypeLoadException ? " (Type=" + ((TypeLoadException) loaderException).TypeName + ")" : string.Empty)));
        types = ex.Types;
      }
      Type[] array = ((IEnumerable<Type>) types).Where<Type>(new Func<Type, bool>(this.CheckAuditType)).ToArray<Type>();
      if (AuditingPluginManager.log.IsTraceEnabled)
        AuditingPluginManager.log.Trace((object) ("Returning " + (object) array.Length + " types."));
      return (IEnumerable<Type>) array;
    }

    private bool CheckAuditType(Type c)
    {
      return c != (Type) null && typeof (IAuditing2).IsAssignableFrom(c) && !c.IsAbstract && !c.IsInterface;
    }

    private void LoadPlugins()
    {
      AuditingPluginManager.log.Trace((object) nameof (LoadPlugins));
      string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
      AuditingPluginManager.log.DebugFormat("PluginDir:'{0}'", (object) baseDirectory);
      if (Directory.Exists(baseDirectory))
      {
        string[] strArray = (string[]) null;
        try
        {
          if (AuditingPluginManager.log.IsVerboseEnabled)
            AuditingPluginManager.log.Verbose((object) "Searching files...");
          strArray = Directory.GetFiles(baseDirectory, "*Auditing.dll", SearchOption.AllDirectories);
          if (AuditingPluginManager.log.IsVerboseEnabled)
            AuditingPluginManager.log.Verbose((object) "Searching files done.");
        }
        catch (Exception ex)
        {
          AuditingPluginManager.log.ErrorFormat("GetFiles failed on '{0}'. Exception: {1}", (object) baseDirectory, (object) ex);
        }
        if (strArray == null)
          return;
        foreach (string assemblyFile in strArray)
        {
          try
          {
            AuditingPluginManager.log.DebugFormat("Loading library '{0}'.", (object) assemblyFile);
            AssemblyName assemblyRef = (AssemblyName) null;
            Assembly assembly = (Assembly) null;
            try
            {
              assemblyRef = AssemblyName.GetAssemblyName(assemblyFile);
              assembly = Assembly.Load(assemblyRef);
            }
            catch (FileLoadException ex)
            {
              AuditingPluginManager.log.WarnFormat("Unable to Load '{0}' - trying LoadFrom. {1}", (object) assemblyRef ?? (object) assemblyFile, (object) ex);
            }
            if (assembly == (Assembly) null)
              assembly = Assembly.LoadFrom(assemblyFile);
            foreach (Type derivedType in this.FindDerivedTypes(assembly))
            {
              IAuditing2 instance = (IAuditing2) assembly.CreateInstance(derivedType.FullName);
              if (instance != null)
              {
                AuditingPluginManager.log.InfoFormat("Instance of {0} created.", (object) derivedType);
                this.auditingInstances.Add(instance);
              }
              else
                AuditingPluginManager.log.ErrorFormat("Instance of {0} coudn't be created. Library: '{1}'", (object) derivedType.FullName, (object) assemblyFile);
            }
          }
          catch (Exception ex)
          {
            AuditingPluginManager.log.ErrorFormat("Unable to Load library '{0}'. Exception: {1}", (object) assemblyFile, (object) ex);
          }
        }
      }
      else
        AuditingPluginManager.log.Warn((object) string.Format("Directory '{0}' was not found.", (object) baseDirectory));
    }

    [Conditional("DEBUG")]
    private static void DebugAuditingPluginNPM(Type derivedType, IAuditing pluginInstance)
    {
      if (!AuditingPluginManager.log.IsDebugEnabled || !(derivedType.FullName == "SolarWinds.NPM.Auditing.InterfaceAdded"))
        return;
      AuditDataContainer auditDataContainer = new AuditDataContainer(pluginInstance.SupportedActionTypes.First<AuditActionType>(), new Dictionary<string, string>()
      {
        {
          "ObjectType",
          "dummy"
        }
      }, "dummy");
      string message = pluginInstance.GetMessage(auditDataContainer);
      AuditingPluginManager.log.DebugFormat("\"{0}::{1}\" Installed Successfully. Example message: \"{2}\".", (object) pluginInstance.GetType(), (object) pluginInstance.SupportedIndicationType, (object) message);
      GC.KeepAlive((object) message);
    }
  }
}
