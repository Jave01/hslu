// Decompiled with JetBrains decompiler
// Type: SolarWinds.Orion.Core.BusinessLayer.PollerLimitHelper
// Assembly: SolarWinds.Orion.Core.BusinessLayer, Version=2019.4.5200.9083, Culture=neutral, PublicKeyToken=null
// MVID: E12E8C85-5CD9-4E06-8801-182E5104FADE
// Assembly location: E:\task5.dll

using SolarWinds.Logging;
using SolarWinds.Orion.Common;
using SolarWinds.Orion.Core.BusinessLayer.DAL;
using SolarWinds.Orion.Core.Common.DALs;
using System;
using System.Collections.Generic;
using System.Data;

#nullable disable
namespace SolarWinds.Orion.Core.BusinessLayer
{
  internal static class PollerLimitHelper
  {
    private static readonly Log log = new Log();

    internal static void CheckPollerLimit()
    {
      Dictionary<string, int> warningEngines = new Dictionary<string, int>();
      Dictionary<string, int> reachedEngines = new Dictionary<string, int>();
      try
      {
        DataTable engineProperty = EngineDAL.GetEngineProperty("Scale Factor");
        if (engineProperty != null)
        {
          foreach (DataRow row in (InternalDataCollectionBase) engineProperty.Rows)
          {
            int result;
            if (int.TryParse(row["PropertyValue"] as string, out result))
            {
              if (result >= Settings.PollerLimitreachedScaleFactor)
              {
                string key = row["ServerName"] as string;
                reachedEngines[key] = result;
              }
              else if (result >= Settings.PollerLimitWarningScaleFactor)
              {
                string key = row["ServerName"] as string;
                warningEngines[key] = result;
              }
            }
          }
        }
        if (warningEngines.Count > 0 || reachedEngines.Count > 0)
          PollerLimitNotificationItemDAL.Show(warningEngines, reachedEngines);
        else
          PollerLimitNotificationItemDAL.Hide();
      }
      catch (Exception ex)
      {
        PollerLimitHelper.log.Warn((object) "Exception while checking poller limit value: ", ex);
      }
    }

    internal static void SavePollingCapacityInfo()
    {
      try
      {
        SqlHelper.ExecuteNonQuery("swsp_UpdatePollingCapacityStatistics");
      }
      catch (Exception ex)
      {
        PollerLimitHelper.log.Warn((object) "Exception while saving polling capacity information: ", ex);
      }
    }
  }
}
