// Decompiled with JetBrains decompiler
// Type: SolarWinds.Orion.Core.BusinessLayer.DAL.DependencyDAL
// Assembly: SolarWinds.Orion.Core.BusinessLayer, Version=2019.4.5200.9083, Culture=neutral, PublicKeyToken=null
// MVID: E12E8C85-5CD9-4E06-8801-182E5104FADE
// Assembly location: E:\task5.dll

using SolarWinds.Orion.Common;
using SolarWinds.Orion.Core.Common.Models;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

#nullable disable
namespace SolarWinds.Orion.Core.BusinessLayer.DAL
{
  public class DependencyDAL
  {
    public static IList<Dependency> GetAllDependencies()
    {
      IList<Dependency> allDependencies = (IList<Dependency>) new List<Dependency>();
      using (SqlCommand textCommand = SqlHelper.GetTextCommand("SELECT * FROM [dbo].[Dependencies]"))
      {
        using (IDataReader reader = SqlHelper.ExecuteReader(textCommand))
        {
          while (reader.Read())
            allDependencies.Add(DependencyDAL.CreateDependency(reader));
        }
      }
      return allDependencies;
    }

    public static Dependency GetDependency(int id)
    {
      Dependency dependency = (Dependency) null;
      using (SqlCommand textCommand = SqlHelper.GetTextCommand("SELECT * FROM [dbo].[Dependencies] WHERE DependencyId = @id"))
      {
        textCommand.Parameters.AddWithValue("@id", (object) id);
        using (IDataReader reader = SqlHelper.ExecuteReader(textCommand))
        {
          if (reader.Read())
            dependency = DependencyDAL.CreateDependency(reader);
        }
      }
      return dependency;
    }

    public static void SaveDependency(Dependency dependency)
    {
      if (dependency == null)
        return;
      using (IDataReader dataReader = SqlHelper.ExecuteStoredProcReader("swsp_DependencyUpsert", new SqlParameter[7]
      {
        new SqlParameter("@DependencyId", (object) dependency.Id),
        new SqlParameter("@Name", (object) dependency.Name),
        new SqlParameter("@ParentUri", (object) dependency.ParentUri),
        new SqlParameter("@ChildUri", (object) dependency.ChildUri),
        new SqlParameter("@AutoManaged", (object) dependency.AutoManaged),
        new SqlParameter("@EngineID", (object) dependency.EngineID),
        new SqlParameter("@Category", (object) dependency.Category)
      }))
      {
        if (dataReader == null || dataReader.IsClosed || !dataReader.Read())
          return;
        dependency.Id = dataReader.GetInt32(0);
        dependency.LastUpdateUTC = dataReader.GetDateTime(1);
      }
    }

    public static void DeleteDependency(Dependency dependency)
    {
      if (dependency == null)
        return;
      using (SqlCommand textCommand = SqlHelper.GetTextCommand("\r\nSET NOCOUNT OFF;\r\nDelete FROM [dbo].[Dependencies]\r\n WHERE DependencyId = @id"))
      {
        textCommand.Parameters.AddWithValue("@id", (object) dependency.Id);
        SqlHelper.ExecuteNonQuery(textCommand);
      }
    }

    public static int DeleteDependencies(List<int> listIds)
    {
      if (listIds.Count == 0)
        return 0;
      string str1 = string.Empty;
      string str2 = string.Empty;
      foreach (int listId in listIds)
      {
        str1 = string.Format("{0}{1}'{2}'", (object) str1, (object) str2, (object) listId);
        str2 = ", ";
      }
      using (SqlCommand textCommand = SqlHelper.GetTextCommand(string.Format("\r\nSET NOCOUNT OFF;\r\nDelete FROM [dbo].[Dependencies]\r\n WHERE DependencyId in ({0})", (object) str1)))
        return SqlHelper.ExecuteNonQuery(textCommand);
    }

    private static Dependency CreateDependency(IDataReader reader)
    {
      Dependency dependency = (Dependency) null;
      if (reader != null)
        dependency = new Dependency()
        {
          Id = reader.GetInt32(reader.GetOrdinal("DependencyId")),
          Name = reader.GetString(reader.GetOrdinal("Name")),
          ParentUri = reader.GetString(reader.GetOrdinal("ParentUri")),
          ChildUri = reader.GetString(reader.GetOrdinal("ChildUri")),
          LastUpdateUTC = reader.GetDateTime(reader.GetOrdinal("LastUpdateUTC")),
          AutoManaged = reader.GetBoolean(reader.GetOrdinal("AutoManaged")),
          EngineID = reader.GetInt32(reader.GetOrdinal("EngineID")),
          Category = reader.GetInt32(reader.GetOrdinal("Category"))
        };
      return dependency;
    }
  }
}
