// Decompiled with JetBrains decompiler
// Type: SolarWinds.Orion.Core.BusinessLayer.DiscoveryImportManager
// Assembly: SolarWinds.Orion.Core.BusinessLayer, Version=2019.4.5200.9083, Culture=neutral, PublicKeyToken=null
// MVID: E12E8C85-5CD9-4E06-8801-182E5104FADE
// Assembly location: E:\task5.dll

using SolarWinds.InformationService.Linq.Plugins.Core.Orion;
using SolarWinds.Logging;
using SolarWinds.Orion.Common;
using SolarWinds.Orion.Core.Common;
using SolarWinds.Orion.Core.Common.Data;
using SolarWinds.Orion.Core.Common.i18n;
using SolarWinds.Orion.Core.Common.Licensing;
using SolarWinds.Orion.Core.Common.Models;
using SolarWinds.Orion.Core.Discovery.DataAccess;
using SolarWinds.Orion.Core.Models.Discovery;
using SolarWinds.Orion.Core.Strings;
using SolarWinds.Orion.Discovery.Contract.DiscoveryPlugin;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

#nullable disable
namespace SolarWinds.Orion.Core.BusinessLayer
{
  public class DiscoveryImportManager
  {
    private const int MAX_TEXT_LENGTH = 131072;
    private static readonly Log log = new Log();
    private static readonly Dictionary<Guid, DiscoveryImportProgressInfo> imports = new Dictionary<Guid, DiscoveryImportProgressInfo>();

    public static DiscoveryImportProgressInfo GetImportProgress(Guid importId)
    {
      lock (((ICollection) DiscoveryImportManager.imports).SyncRoot)
      {
        if (!DiscoveryImportManager.imports.ContainsKey(importId))
          return (DiscoveryImportProgressInfo) null;
        DiscoveryImportProgressInfo import = DiscoveryImportManager.imports[importId];
        if (import.LogBuilder.Length > 131072)
        {
          StringBuilder stringBuilder1 = new StringBuilder();
          StringBuilder stringBuilder2 = new StringBuilder();
          using (StringReader stringReader = new StringReader(import.NewLogText))
          {
            bool flag = false;
            string str;
            while ((str = stringReader.ReadLine()) != null)
            {
              if ((stringBuilder1.Length + str.Length <= 131072 || stringBuilder1.Length == 0) && !flag)
              {
                stringBuilder1.AppendLine(str);
              }
              else
              {
                flag = true;
                stringBuilder2.AppendLine(str);
              }
            }
          }
          import.NewLogText = stringBuilder2.ToString();
          return new DiscoveryImportProgressInfo(import)
          {
            NewLogText = stringBuilder1.ToString(),
            Finished = false
          };
        }
        DiscoveryImportManager.imports.Remove(importId);
        return import;
      }
    }

    private static void UpdateProgress(
      Guid importId,
      string text,
      double progress,
      double phaseProgress,
      string phaseName,
      bool finished)
    {
      lock (((ICollection) DiscoveryImportManager.imports).SyncRoot)
      {
        if (!DiscoveryImportManager.imports.ContainsKey(importId))
          DiscoveryImportManager.imports[importId] = new DiscoveryImportProgressInfo();
        DiscoveryImportProgressInfo import = DiscoveryImportManager.imports[importId];
        import.LogBuilder.AppendLine(text);
        import.Finished = finished;
        import.OverallProgress = progress;
        import.PhaseProgress = phaseProgress;
        import.PhaseName = phaseName;
        if (!DiscoveryImportManager.log.IsInfoEnabled)
          return;
        DiscoveryImportManager.log.InfoFormat("{0} {1}/{2}: {3}", new object[4]
        {
          (object) phaseName,
          (object) progress,
          (object) phaseProgress,
          (object) text
        });
      }
    }

    public static void UpdateProgress(Guid importId, string text, string phaseName, bool finished)
    {
      DiscoveryImportManager.UpdateProgress(importId, text, 0.0, 0.0, phaseName, finished);
    }

    public static StartImportStatus StartImport(
      Guid importId,
      DiscoveryResultBase result,
      SortedDictionary<int, List<IDiscoveryPlugin>> importingPlugins)
    {
      return DiscoveryImportManager.StartImport(importId, result, importingPlugins, false, (DiscoveryImportManager.CallbackDiscoveryImportFinished) null);
    }

    internal static StartImportStatus StartImport(
      Guid importId,
      DiscoveryResultBase result,
      SortedDictionary<int, List<IDiscoveryPlugin>> importingPlugins,
      bool checkLicenseLimits,
      DiscoveryImportManager.CallbackDiscoveryImportFinished callbackAfterImport)
    {
      if (result == null)
        throw new ArgumentNullException(nameof (result));
      ThreadPool.QueueUserWorkItem((WaitCallback) (state =>
      {
        try
        {
          DiscoveryImportManager.StartImportInternal(importId, result, importingPlugins, checkLicenseLimits, callbackAfterImport);
        }
        catch (Exception ex)
        {
          DiscoveryImportManager.log.Error((object) "Error in StartImport", ex);
        }
      }));
      return (StartImportStatus) 1;
    }

    private static void StartImportInternal(
      Guid importId,
      DiscoveryResultBase result,
      SortedDictionary<int, List<IDiscoveryPlugin>> importingPlugins,
      bool checkLicenseLimits,
      DiscoveryImportManager.CallbackDiscoveryImportFinished callbackAfterImport)
    {
      string webjsPs017 = Resources.WEBJS_PS0_17;
      StartImportStatus status = (StartImportStatus) 4;
      List<DiscoveryLogItem> items = new List<DiscoveryLogItem>();
      try
      {
        DiscoveryConfiguration discoveryConfiguration = DiscoveryDatabase.GetDiscoveryConfiguration(result.ProfileID);
        if (discoveryConfiguration != null)
        {
          string name = discoveryConfiguration.Name;
        }
      }
      catch (Exception ex)
      {
        DiscoveryImportManager.log.Warn((object) "Unable to load profile name", ex);
      }
      using (LocaleThreadState.EnsurePrimaryLocale())
      {
        try
        {
          DiscoveryNetObjectStatusManager.Instance.BeginOrionDatabaseChanges();
          if (checkLicenseLimits && DiscoveryImportManager.GetLicensedStatus(result).Any<ElementLicenseInfo>((System.Func<ElementLicenseInfo, bool>) (n => n.ExceededBy != 0)))
          {
            DiscoveryImportManager.log.Debug((object) "Can't import discovery result, because license was exceeded");
            status = (StartImportStatus) 5;
          }
          else
          {
            double progress = 0.0;
            double num = (double) (100 / importingPlugins.Keys.Count);
            foreach (int key in importingPlugins.Keys)
            {
              foreach (IDiscoveryPlugin idiscoveryPlugin in importingPlugins[key])
              {
                IDiscoveryPlugin plugin = idiscoveryPlugin;
                if (plugin is ISupportImportLog isupportImportLog)
                  isupportImportLog.SetImportLogCallback(new Action<DiscoveryLogItem>(items.Add));
                plugin.ImportResults(result, (Action<string, double>) ((message, phaseProgress) => DiscoveryImportManager.UpdateProgress(importId, message, progress + phaseProgress / (double) importingPlugins.Keys.Count, phaseProgress, plugin.GetImportPhaseName(), false)));
              }
              progress += num;
            }
            DiscoveryImportManager.UpdateProgress(importId, Resources.LIBCODE_VB0_28, 100.0, 100.0, string.Empty, true);
            status = (StartImportStatus) 6;
          }
        }
        catch (Exception ex)
        {
          status = (StartImportStatus) 4;
          DiscoveryImportManager.log.Error((object) "Exception occurred during discovery import", ex);
          DiscoveryImportManager.UpdateProgress(importId, Resources.LIBCODE_TM0_30, 100.0, 100.0, string.Empty, true);
        }
        finally
        {
          DiscoveryNetObjectStatusManager.Instance.EndOrionDatabaseChanges();
          result.BatchID = Guid.NewGuid();
          try
          {
            DiscoveryImportManager.InsertDiscoveryLogItems(items, result.BatchID);
          }
          catch (Exception ex)
          {
            DiscoveryImportManager.log.Error((object) "Unable to store discovery import items", ex);
          }
          if (callbackAfterImport != null)
          {
            try
            {
              callbackAfterImport(result, importId, status);
            }
            catch (Exception ex)
            {
              DiscoveryImportManager.log.Error((object) "Error while calling callback after import.", ex);
            }
          }
          DiscoveryNetObjectStatusManager.Instance.RequestUpdateAsync((Action) null, BusinessLayerSettings.Instance.DiscoveryUpdateNetObjectStatusWaitForChangesDelay);
        }
      }
    }

    public static List<ElementLicenseInfo> GetLicensedStatus(DiscoveryResultBase discoveryResult)
    {
      if (discoveryResult == null)
        throw new ArgumentNullException(nameof (discoveryResult));
      List<ElementLicenseInfo> licensedStatus = new List<ElementLicenseInfo>();
      IFeatureManager ifeatureManager = (IFeatureManager) new FeatureManager();
      Dictionary<string, int> dictionary = new Dictionary<string, int>();
      Dictionary<string, int> elementsManagedCount = LicenseSaturationLogic.GetElementsManagedCount();
      foreach (string key in elementsManagedCount.Keys)
        dictionary[key] = ifeatureManager.GetMaxElementCount(key);
      foreach (DiscoveryPluginResultBase pluginResult in discoveryResult.PluginResults)
      {
        List<ElementLicenseInfo> collection;
        if (!pluginResult.CheckLicensingStatusForImport(elementsManagedCount, dictionary, ref collection))
          licensedStatus.AddRange((IEnumerable<ElementLicenseInfo>) collection);
      }
      return licensedStatus;
    }

    private static void InsertDiscoveryLogItems(List<DiscoveryLogItem> items, Guid batchID)
    {
      // ISSUE: object of a compiler-generated type is created
      // ISSUE: method pointer
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      // ISSUE: method pointer
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      // ISSUE: method pointer
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      // ISSUE: method pointer
      using (IDataReader dataReader = (IDataReader) new EnumerableDataReader<DiscoveryLogItem>((PropertyAccessorBase<DiscoveryLogItem>) new SinglePropertyAccessor<DiscoveryLogItem>().AddColumn("BatchID", new SinglePropertyAccessor<DiscoveryLogItem>.SinglePropertyAcccessorConvert((object) new DiscoveryImportManager.\u003C\u003Ec__DisplayClass11_0()
      {
        batchID = batchID
      }, __methodptr(\u003CInsertDiscoveryLogItems\u003Eb__0))).AddColumn("EntityType", DiscoveryImportManager.\u003C\u003Ec.\u003C\u003E9__11_1 ?? (DiscoveryImportManager.\u003C\u003Ec.\u003C\u003E9__11_1 = new SinglePropertyAccessor<DiscoveryLogItem>.SinglePropertyAcccessorConvert((object) DiscoveryImportManager.\u003C\u003Ec.\u003C\u003E9, __methodptr(\u003CInsertDiscoveryLogItems\u003Eb__11_1)))).AddColumn("DisplayName", DiscoveryImportManager.\u003C\u003Ec.\u003C\u003E9__11_2 ?? (DiscoveryImportManager.\u003C\u003Ec.\u003C\u003E9__11_2 = new SinglePropertyAccessor<DiscoveryLogItem>.SinglePropertyAcccessorConvert((object) DiscoveryImportManager.\u003C\u003Ec.\u003C\u003E9, __methodptr(\u003CInsertDiscoveryLogItems\u003Eb__11_2)))).AddColumn("NetObjectID", DiscoveryImportManager.\u003C\u003Ec.\u003C\u003E9__11_3 ?? (DiscoveryImportManager.\u003C\u003Ec.\u003C\u003E9__11_3 = new SinglePropertyAccessor<DiscoveryLogItem>.SinglePropertyAcccessorConvert((object) DiscoveryImportManager.\u003C\u003Ec.\u003C\u003E9, __methodptr(\u003CInsertDiscoveryLogItems\u003Eb__11_3)))), (IEnumerable<DiscoveryLogItem>) items))
        SqlHelper.ExecuteBulkCopy("DiscoveryLogItems", dataReader, SqlBulkCopyOptions.Default);
    }

    internal static void FillDiscoveryLogEntity(
      DiscoveryLogs discoveryLog,
      DiscoveryResultBase result,
      StartImportStatus status)
    {
      DateTime utcNow = DateTime.UtcNow;
      discoveryLog.FinishedTimeStamp = utcNow.AddTicks(-(utcNow.Ticks % 10000000L));
      discoveryLog.BatchID = new Guid?(result.BatchID);
      discoveryLog.ProfileID = result.ProfileID;
      switch (status - 4)
      {
        case 0:
          discoveryLog.Result = 3;
          discoveryLog.ResultDescription = Resources2.DiscoveryLogResult_ImportFailed;
          discoveryLog.ErrorMessage = Resources2.DiscoveryLogError_UnknownError;
          break;
        case 1:
          discoveryLog.Result = 4;
          discoveryLog.ResultDescription = Resources2.DiscoveryLogResult_ImportFailedLicenseExceeded;
          break;
        case 2:
          discoveryLog.Result = 2;
          discoveryLog.ResultDescription = Resources2.DiscoveryLogResult_ImportFinished;
          break;
      }
    }

    public delegate void CallbackDiscoveryImportFinished(
      DiscoveryResultBase result,
      Guid importID,
      StartImportStatus status);
  }
}
