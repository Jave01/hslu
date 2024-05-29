// Decompiled with JetBrains decompiler
// Type: SolarWinds.Orion.Core.BusinessLayer.DAL.SysLogDAL
// Assembly: SolarWinds.Orion.Core.BusinessLayer, Version=2019.4.5200.9083, Culture=neutral, PublicKeyToken=null
// MVID: E12E8C85-5CD9-4E06-8801-182E5104FADE
// Assembly location: E:\task5.dll

using SolarWinds.Orion.Common;
using System.Collections.Specialized;
using System.Data;
using System.Data.SqlClient;

#nullable disable
namespace SolarWinds.Orion.Core.BusinessLayer.DAL
{
  internal class SysLogDAL
  {
    public static StringDictionary GetSeverities()
    {
      StringDictionary severities = new StringDictionary();
      using (SqlCommand textCommand = SqlHelper.GetTextCommand("Select SeverityCode, SeverityName From SysLogSeverities WITH(NOLOCK) Order By SeverityCode"))
      {
        using (IDataReader dataReader = SqlHelper.ExecuteReader(textCommand))
        {
          while (dataReader.Read())
            severities.Add(DatabaseFunctions.GetByte(dataReader, "SeverityCode").ToString(), DatabaseFunctions.GetString(dataReader, "SeverityName"));
        }
      }
      return severities;
    }

    public static StringDictionary GetFacilities()
    {
      StringDictionary facilities = new StringDictionary();
      using (SqlCommand textCommand = SqlHelper.GetTextCommand("Select FacilityCode, FacilityName From SysLogFacilities WITH(NOLOCK) Order By FacilityCode"))
      {
        using (IDataReader dataReader = SqlHelper.ExecuteReader(textCommand))
        {
          while (dataReader.Read())
            facilities.Add(DatabaseFunctions.GetByte(dataReader, "FacilityCode").ToString(), DatabaseFunctions.GetString(dataReader, "FacilityName"));
        }
      }
      return facilities;
    }
  }
}
