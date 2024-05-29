// Decompiled with JetBrains decompiler
// Type: SolarWinds.Orion.Core.BusinessLayer.DAL.ViewsDAL
// Assembly: SolarWinds.Orion.Core.BusinessLayer, Version=2019.4.5200.9083, Culture=neutral, PublicKeyToken=null
// MVID: E12E8C85-5CD9-4E06-8801-182E5104FADE
// Assembly location: E:\task5.dll

using SolarWinds.Logging;
using SolarWinds.Orion.Core.Common.Models;
using System;
using System.Data;
using System.Data.SqlClient;

#nullable disable
namespace SolarWinds.Orion.Core.BusinessLayer.DAL
{
  public class ViewsDAL
  {
    private static readonly Log log = new Log();

    public static Views GetSummaryDetailsViews()
    {
      // ISSUE: method pointer
      return Collection<int, WebView>.FillCollection<Views>(new Collection<int, WebView>.CreateElement((object) null, __methodptr(CreateView)), "SELECT * FROM Views WHERE (NOT(ViewType LIKE 'Volume%')) AND ((ViewType LIKE 'Summary') OR (ViewType LIKE '%Details'))", (SqlParameter[]) null);
    }

    private static WebView CreateView(IDataReader reader)
    {
      return new WebView()
      {
        ViewType = reader["ViewType"].ToString().Trim(),
        ViewID = Convert.ToInt32(reader["ViewID"]),
        ViewTitle = reader["ViewTitle"].ToString().Trim(),
        ViewGroupName = reader["ViewGroupName"].ToString().Trim()
      };
    }
  }
}
