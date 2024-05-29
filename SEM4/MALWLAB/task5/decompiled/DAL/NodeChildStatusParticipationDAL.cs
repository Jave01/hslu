// Decompiled with JetBrains decompiler
// Type: SolarWinds.Orion.Core.BusinessLayer.DAL.NodeChildStatusParticipationDAL
// Assembly: SolarWinds.Orion.Core.BusinessLayer, Version=2019.4.5200.9083, Culture=neutral, PublicKeyToken=null
// MVID: E12E8C85-5CD9-4E06-8801-182E5104FADE
// Assembly location: E:\task5.dll

using SolarWinds.Logging;
using SolarWinds.Orion.Common;
using SolarWinds.Orion.Core.Common;
using SolarWinds.Orion.Core.Common.Models;
using SolarWinds.Orion.Core.Common.PackageManager;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Text;

#nullable disable
namespace SolarWinds.Orion.Core.BusinessLayer.DAL
{
  public class NodeChildStatusParticipationDAL
  {
    private static readonly Log log = new Log();

    public static void ResyncAfterStartup()
    {
      try
      {
        bool needsreflow;
        NodeChildStatusParticipationDAL.UpdateParticipationFromInstalledProducts(out needsreflow);
        if (!needsreflow)
          return;
        NodeChildStatusParticipationDAL.ReflowAllNodeChildStatus();
      }
      catch (Exception ex)
      {
        NodeChildStatusParticipationDAL.log.Error((object) "Unhandled exception when reinitailizing node child status", ex);
      }
    }

    public static void UpdateParticipationFromInstalledProducts(out bool needsreflow)
    {
      IEnumerable<PackageInfo> installedPackages = SolarWinds.Orion.Core.Common.PackageManager.PackageManager.Instance.GetInstalledPackages();
      List<ModuleInfo> installedModules = ModulesCollector.GetInstalledModules();
      using (SqlCommand textCommand = SqlHelper.GetTextCommand(new StringBuilder("UPDATE dbo.[NodeChildStatusParticipation] set Installed=0 Where ModuleName not in (").Append(string.Join(",", installedPackages.Select<PackageInfo, string>((Func<PackageInfo, string>) (package => package.PackageId)).Concat<string>(installedModules.Select<ModuleInfo, string>((Func<ModuleInfo, string>) (module => module.ProductShortName))).Select<string, string>((Func<string, string>) (name => '\''.ToString() + name.Replace("'", "''") + (object) '\'')))).Append(')').ToString()))
      {
        int num = SqlHelper.ExecuteNonQuery(textCommand);
        needsreflow = num > 0;
      }
    }

    private static SqlCommand MakeParticipationChangeQuery(
      Dictionary<string, bool> changes,
      bool value)
    {
      StringBuilder stringBuilder = new StringBuilder();
      SqlCommand sqlCommand = new SqlCommand();
      int num = 0;
      foreach (string str in changes.Where<KeyValuePair<string, bool>>((Func<KeyValuePair<string, bool>, bool>) (x => x.Value == value)).Select<KeyValuePair<string, bool>, string>((Func<KeyValuePair<string, bool>, string>) (x => x.Key)))
      {
        if (num == 0)
          stringBuilder.AppendFormat("UPDATE dbo.NodeChildStatusParticipation set Enabled={0} WHERE Excluded=0 AND EntityType in (", value ? (object) "1" : (object) "0");
        stringBuilder.AppendFormat("{0}@e{1}", num == 0 ? (object) "" : (object) ",", (object) num);
        sqlCommand.Parameters.AddWithValue("@e" + num.ToString((IFormatProvider) CultureInfo.InvariantCulture), (object) str);
        ++num;
      }
      if (num != 0)
      {
        stringBuilder.Append(")");
        sqlCommand.CommandText = stringBuilder.ToString();
      }
      return sqlCommand;
    }

    public static void ReflowAllNodeChildStatus()
    {
      SqlHelper.ExecuteStoredProc("swsp_ReflowAllNodeChildStatus", Array.Empty<SqlParameter>());
    }
  }
}
