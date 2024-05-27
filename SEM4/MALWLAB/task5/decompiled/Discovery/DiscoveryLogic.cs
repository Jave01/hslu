// Decompiled with JetBrains decompiler
// Type: SolarWinds.Orion.Core.BusinessLayer.Discovery.DiscoveryLogic
// Assembly: SolarWinds.Orion.Core.BusinessLayer, Version=2019.4.5200.9083, Culture=neutral, PublicKeyToken=null
// MVID: E12E8C85-5CD9-4E06-8801-182E5104FADE
// Assembly location: E:\task5.dll

using SolarWinds.Logging;
using SolarWinds.Orion.Core.Common;
using SolarWinds.Orion.Core.Common.Models;
using SolarWinds.Orion.Core.Discovery;
using SolarWinds.Orion.Core.Discovery.DataAccess;
using SolarWinds.Orion.Core.Models.DiscoveredObjects;
using SolarWinds.Orion.Core.Models.Discovery;
using SolarWinds.Orion.Core.Models.Interfaces;
using SolarWinds.Orion.Discovery.Contract.DiscoveryPlugin;
using SolarWinds.Orion.Discovery.Framework.Pluggability;
using System;
using System.Collections.Generic;
using System.Linq;

#nullable disable
namespace SolarWinds.Orion.Core.BusinessLayer.Discovery
{
  internal class DiscoveryLogic
  {
    private static Log log = new Log();
    private IJobFactory _jobFactory;

    internal IJobFactory JobFactory
    {
      get => this._jobFactory ?? (this._jobFactory = (IJobFactory) new OrionDiscoveryJobFactory());
      set => this._jobFactory = value;
    }

    public DiscoveryResultBase FilterIgnoredItems(DiscoveryResultBase discoveryResult)
    {
      DiscoveryResultBase discoveryResult1 = this.FilterItems(discoveryResult, (Func<IDiscoveryPlugin, DiscoveryResultBase, DiscoveryPluginResultBase>) ((plugin, result) => plugin.FilterOutItemsFromIgnoreList(result)));
      DiscoveryConfiguration config = DiscoveryDatabase.GetDiscoveryConfiguration(((DiscoveryPluginResultBase) discoveryResult.GetPluginResultOfType<CoreDiscoveryPluginResult>()).ProfileId ?? 0);
      if (config != null && config.IsAutoImport)
        discoveryResult1 = this.FilterItems(discoveryResult1, (Func<IDiscoveryPlugin, DiscoveryResultBase, DiscoveryPluginResultBase>) ((plugin, result) => !(plugin is ISupportAutoImport) ? (DiscoveryPluginResultBase) null : ((ISupportAutoImport) plugin).FilterAutoImportItems(result, (DiscoveryConfigurationBase) config)));
      return discoveryResult1;
    }

    private DiscoveryResultBase FilterItems(
      DiscoveryResultBase discoveryResult,
      Func<IDiscoveryPlugin, DiscoveryResultBase, DiscoveryPluginResultBase> filterFunction)
    {
      foreach (IDiscoveryPlugin orderedDiscoveryPlugin in (IEnumerable<IDiscoveryPlugin>) DiscoveryHelper.GetOrderedDiscoveryPlugins())
      {
        DiscoveryPluginResultBase pluginResultBase1 = filterFunction(orderedDiscoveryPlugin, discoveryResult);
        if (pluginResultBase1 != null)
        {
          pluginResultBase1.PluginTypeName = orderedDiscoveryPlugin.GetType().FullName;
          Type returnedType = pluginResultBase1.GetType();
          List<DiscoveryPluginResultBase> list = ((IEnumerable<DiscoveryPluginResultBase>) discoveryResult.PluginResults).Where<DiscoveryPluginResultBase>((Func<DiscoveryPluginResultBase, bool>) (item => item.GetType() != returnedType)).ToList<DiscoveryPluginResultBase>();
          discoveryResult.PluginResults.Clear();
          discoveryResult.PluginResults.Add(pluginResultBase1);
          foreach (DiscoveryPluginResultBase pluginResultBase2 in list)
            discoveryResult.PluginResults.Add(pluginResultBase2);
        }
      }
      return discoveryResult;
    }

    public void DeleteOrionDiscoveryProfile(int profileID)
    {
      DiscoveryLogic.log.DebugFormat("Deleting profile {0}", (object) profileID);
      this.DeleteDiscoveryProfileInternal(DiscoveryProfileEntry.GetProfileByID(profileID) ?? throw new ArgumentNullException(string.Format("Profile {0} not found.", (object) profileID)));
    }

    public void DeleteHiddenOrionDiscoveryProfilesByName(string profileName)
    {
      DiscoveryLogic.log.DebugFormat("Deleting hidden profile '{0}'", (object) profileName);
      foreach (DiscoveryProfileEntry profile in DiscoveryProfileEntry.GetProfilesByName(profileName).Where<DiscoveryProfileEntry>((Func<DiscoveryProfileEntry, bool>) (x => x.IsHidden)))
        this.DeleteDiscoveryProfileInternal(profile);
    }

    private void DeleteDiscoveryProfileInternal(DiscoveryProfileEntry profile)
    {
      if (profile.JobID != Guid.Empty)
      {
        DiscoveryLogic.log.DebugFormat("Deleting job for profile {0}", (object) profile.ProfileID);
        try
        {
          if (this.JobFactory.DeleteJob(profile.JobID))
            DiscoveryLogic.log.ErrorFormat("Error when deleting job {0}.", (object) profile.ProfileID);
          DiscoveryLogic.log.DebugFormat("Job for profile {0} deleted.", (object) profile.ProfileID);
        }
        catch (Exception ex)
        {
          DiscoveryLogic.log.ErrorFormat("Exception when deleting job {0}. Exception: {1}", (object) profile.ProfileID, (object) ex);
        }
      }
      DiscoveryLogic.log.DebugFormat("Removing profile {0} from database.", (object) profile.ProfileID);
      DiscoveryDatabase.DeleteProfile(profile);
      DiscoveryLogic.log.DebugFormat("Profile {0} removed from database.", (object) profile.ProfileID);
    }

    public void ImportDiscoveryResultForProfile(
      int profileID,
      bool deleteProfileAfterImport,
      DiscoveryImportManager.CallbackDiscoveryImportFinished callback = null,
      bool checkLicenseLimits = false,
      Guid? importID = null)
    {
      IList<IDiscoveryPlugin> discoveryPlugins = DiscoveryHelper.GetOrderedDiscoveryPlugins();
      SortedDictionary<int, List<IDiscoveryPlugin>> orderedPlugins = DiscoveryPluginHelper.GetOrderedPlugins(discoveryPlugins, (IList<DiscoveryPluginInfo>) DiscoveryHelper.GetDiscoveryPluginInfos());
      DiscoveryResultBase result1 = this.FilterIgnoredItems(DiscoveryResultManager.GetDiscoveryResult(profileID, discoveryPlugins));
      Guid importId = Guid.NewGuid();
      if (importID.HasValue)
        importId = importID.Value;
      DiscoveryImportManager.CallbackDiscoveryImportFinished callbackAfterImport = callback;
      if (deleteProfileAfterImport)
        callbackAfterImport = (DiscoveryImportManager.CallbackDiscoveryImportFinished) ((result, id, status) =>
        {
          this.DeleteOrionDiscoveryProfile(result.ProfileID);
          if (callback == null)
            return;
          callback(result, id, status);
        });
      DiscoveryImportManager.StartImport(importId, result1, orderedPlugins, checkLicenseLimits, callbackAfterImport);
    }

    public IEnumerable<DiscoveryPluginConfigurationBase> DeserializePluginConfigurationItems(
      List<string> discoveryPluginConfigurationBaseItems)
    {
      List<DiscoveryPluginConfigurationBase> configurationBaseList = new List<DiscoveryPluginConfigurationBase>();
      foreach (string configurationBaseItem in discoveryPluginConfigurationBaseItems)
      {
        DiscoveryPluginItems<DiscoveryPluginConfigurationBase> collection = new DiscoveryPluginItems<DiscoveryPluginConfigurationBase>(configurationBaseItem);
        configurationBaseList.AddRange((IEnumerable<DiscoveryPluginConfigurationBase>) collection);
      }
      return (IEnumerable<DiscoveryPluginConfigurationBase>) configurationBaseList;
    }

    public void ImportDiscoveryResultsForConfiguration(
      DiscoveryImportConfiguration importCfg,
      Guid importID)
    {
      DiscoveryLogic.log.DebugFormat("Loading discovery results.", Array.Empty<object>());
      if (DiscoveryProfileEntry.GetProfileByID(importCfg.ProfileID) == null)
        throw new Exception(string.Format("Requested profile {0} not found.", (object) importCfg.ProfileID));
      DiscoveryImportManager.UpdateProgress(importID, "ImportDiscoveryResults Started", "Loading Plugins", false);
      IList<IDiscoveryPlugin> discoveryPlugins = DiscoveryHelper.GetOrderedDiscoveryPlugins();
      SortedDictionary<int, List<IDiscoveryPlugin>> orderedPlugins = DiscoveryPluginHelper.GetOrderedPlugins(discoveryPlugins, (IList<DiscoveryPluginInfo>) DiscoveryHelper.GetDiscoveryPluginInfos());
      DiscoveryResultBase discoveryResult = DiscoveryResultManager.GetDiscoveryResult(importCfg.ProfileID, discoveryPlugins);
      DiscoveryResultBase result1;
      if (importCfg.NodeIDs.Count > 0)
      {
        DiscoveryLogic.log.DebugFormat("Nodes to be imported : {0}", (object) importCfg.NodeIDs.Count);
        foreach (DiscoveredNode discoveredNode in discoveryResult.GetPluginResultOfType<CoreDiscoveryPluginResult>().DiscoveredNodes)
        {
          if (importCfg.NodeIDs.Contains(discoveredNode.NodeID))
            ((DiscoveredObjectBase) discoveredNode).IsSelected = true;
          else
            ((DiscoveredObjectBase) discoveredNode).IsSelected = false;
        }
        foreach (DiscoveryPluginResultBase pluginResultBase1 in this.Linearize((IEnumerable<DiscoveryPluginResultBase>) discoveryResult.PluginResults))
        {
          DiscoveryPluginResultBase pluginResultBase2 = !(pluginResultBase1 is IDiscoveryPluginResultContextFiltering contextFiltering) ? pluginResultBase1.GetFilteredPluginResult() : contextFiltering.GetFilteredPluginResultFromContext(discoveryResult);
          discoveryResult.PluginResults.Remove(pluginResultBase1);
          discoveryResult.PluginResults.Add(pluginResultBase2);
          DiscoveryLogic.log.DebugFormat("Applying filters for pluggin - {0}.", (object) pluginResultBase1.PluginTypeName);
        }
        result1 = this.FilterIgnoredItems(discoveryResult);
      }
      else
        result1 = discoveryResult;
      result1.ProfileID = importCfg.ProfileID;
      DiscoveryLogic.log.DebugFormat("Importing started.", Array.Empty<object>());
      if (importCfg.DeleteProfileAfterImport)
        DiscoveryImportManager.StartImport(importID, result1, orderedPlugins, false, (DiscoveryImportManager.CallbackDiscoveryImportFinished) ((result, importId, importStatus) => this.DeleteOrionDiscoveryProfile(result.ProfileID)));
      else
        DiscoveryImportManager.StartImport(importID, result1, orderedPlugins);
    }

    private List<DiscoveryPluginResultBase> Linearize(IEnumerable<DiscoveryPluginResultBase> input)
    {
      List<DiscoveryPluginResultBase> source = Linearizer.Linearize<DiscoveryPluginResultBase>((IEnumerable<Linearizer.Input<DiscoveryPluginResultBase>>) input.Select<DiscoveryPluginResultBase, Linearizer.Input<DiscoveryPluginResultBase>>((Func<DiscoveryPluginResultBase, Linearizer.Input<DiscoveryPluginResultBase>>) (item => Linearizer.CreateInputItem<DiscoveryPluginResultBase>(item, item.GetPrerequisites(input)))).ToArray<Linearizer.Input<DiscoveryPluginResultBase>>(), true, true);
      IEnumerable<DiscoveryPluginResultBase> collection = source.Where<DiscoveryPluginResultBase>((Func<DiscoveryPluginResultBase, bool>) (item => item is CoreDiscoveryPluginResult && item.PluginTypeName == "SolarWinds.Orion.Core.DiscoveryPlugin.CoreDiscoveryPlugin"));
      List<DiscoveryPluginResultBase> pluginResultBaseList = new List<DiscoveryPluginResultBase>();
      pluginResultBaseList.AddRange(collection);
      for (int index = 0; index < source.Count; ++index)
      {
        DiscoveryPluginResultBase pluginResultBase = source[index];
        if (!(pluginResultBase is CoreDiscoveryPluginResult) || !(pluginResultBase.PluginTypeName == "SolarWinds.Orion.Core.DiscoveryPlugin.CoreDiscoveryPlugin"))
          pluginResultBaseList.Add(pluginResultBase);
      }
      return pluginResultBaseList;
    }
  }
}
