// Decompiled with JetBrains decompiler
// Type: SolarWinds.Orion.Core.BusinessLayer.DAL.MibDAL
// Assembly: SolarWinds.Orion.Core.BusinessLayer, Version=2019.4.5200.9083, Culture=neutral, PublicKeyToken=null
// MVID: E12E8C85-5CD9-4E06-8801-182E5104FADE
// Assembly location: E:\task5.dll

using SolarWinds.Logging;
using SolarWinds.Orion.Common;
using SolarWinds.Orion.Core.Common.Models;
using SolarWinds.Orion.Core.Common.Models.Mib;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

#nullable disable
namespace SolarWinds.Orion.Core.BusinessLayer.DAL
{
  internal class MibDAL
  {
    private const string TreeColumns = "Index, MIB, Name, Primary, OID, Description, Access, Status, Units, Enum, TypeS";
    private const string EnumColumns = "Name, Value, Enum";
    private static readonly Log _myLog = new Log();
    private static CancellationTokenSource _cancellationTokenSource;
    private static object _tokenLock = new object();

    private static CancellationTokenSource CancellationTokenSource
    {
      get
      {
        lock (MibDAL._tokenLock)
          return MibDAL._cancellationTokenSource;
      }
      set
      {
        lock (MibDAL._tokenLock)
          MibDAL._cancellationTokenSource = value;
      }
    }

    public Oid GetOid(string oid) => this.GetOid(oid, true) ?? this.GetOid(oid, false);

    private Oid GetOid(string oid, bool clean)
    {
      using (OleDbConnection dbConnection = MibHelper.GetDBConnection())
      {
        dbConnection.Open();
        return this.GetOid(oid, dbConnection, clean);
      }
    }

    private Oid GetOid(string oid, OleDbConnection connection, bool clean)
    {
      Oid oid1 = (Oid) null;
      using (OleDbCommand oleDbCommand = new OleDbCommand())
      {
        string empty = string.Empty;
        string str = !clean ? string.Format("Select TOP 1 {0} from Tree WHERE Primary = -1 AND OID=@Oid;", (object) "Index, MIB, Name, Primary, OID, Description, Access, Status, Units, Enum, TypeS") : string.Format("Select TOP 1 {0} from Tree WHERE Primary = -1 AND OID=@Oid AND Description <> 'unknown';", (object) "Index, MIB, Name, Primary, OID, Description, Access, Status, Units, Enum, TypeS");
        oleDbCommand.CommandText = str;
        oleDbCommand.Parameters.AddWithValue("Oid", (object) oid);
        using (IDataReader reader = OleDbHelper.ExecuteReader(oleDbCommand, connection))
        {
          if (reader.Read())
            oid1 = this.CreateOid(reader, connection);
        }
      }
      return oid1;
    }

    public MemoryStream GetIcon(string oid) => throw new NotImplementedException();

    public Dictionary<string, MemoryStream> GetIcons()
    {
      byte[] numArray = new byte[0];
      Dictionary<string, MemoryStream> icons = new Dictionary<string, MemoryStream>();
      using (OleDbConnection dbConnection = MibHelper.GetDBConnection())
      {
        using (OleDbCommand oleDbCommand = new OleDbCommand())
        {
          dbConnection.Open();
          oleDbCommand.CommandText = "Select OID, [Small Icon] From Icons";
          using (IDataReader dataReader = OleDbHelper.ExecuteReader(oleDbCommand, dbConnection))
          {
            while (dataReader.Read())
            {
              if (!(dataReader["Small Icon"] is DBNull))
              {
                byte[] buffer = (byte[]) dataReader["Small Icon"];
                icons.Add(dataReader["OID"].ToString(), new MemoryStream(buffer, true));
              }
            }
          }
        }
      }
      return icons;
    }

    public Oids GetChildOids(string parentOid)
    {
      List<string> uniqueChildOids = this.GetUniqueChildOids(parentOid);
      Oids childOids = new Oids();
      using (OleDbConnection dbConnection = MibHelper.GetDBConnection())
      {
        dbConnection.Open();
        foreach (string oid1 in uniqueChildOids)
        {
          Oid oid2 = this.GetOid(oid1, dbConnection, true) ?? this.GetOid(oid1, dbConnection, false);
          ((Collection<string, Oid>) childOids).Add((object) oid2);
        }
      }
      return childOids;
    }

    public List<string> GetUniqueChildOids(string parentOid)
    {
      List<string> uniqueChildOids = new List<string>();
      using (OleDbConnection dbConnection = MibHelper.GetDBConnection())
      {
        dbConnection.Open();
        using (OleDbCommand oleDbCommand = new OleDbCommand())
        {
          oleDbCommand.CommandText = string.Format("Select DISTINCT Name, OID, Index from Tree WHERE Primary = -1 AND ParentOID=@parentOid order by index;", (object) "Index, MIB, Name, Primary, OID, Description, Access, Status, Units, Enum, TypeS");
          oleDbCommand.Parameters.AddWithValue(nameof (parentOid), (object) parentOid);
          using (IDataReader dataReader = OleDbHelper.ExecuteReader(oleDbCommand, dbConnection))
          {
            while (dataReader.Read())
              uniqueChildOids.Add(DatabaseFunctions.GetString(dataReader, "OID"));
          }
        }
      }
      return uniqueChildOids;
    }

    public OidEnums GetEnums(string enumName)
    {
      OidEnums enums = new OidEnums();
      if (string.IsNullOrEmpty(enumName))
        return enums;
      using (OleDbConnection dbConnection = MibHelper.GetDBConnection())
      {
        dbConnection.Open();
        using (OleDbCommand oleDbCommand = new OleDbCommand())
        {
          oleDbCommand.CommandText = string.Format("Select {0} from Enums WHERE Name=@name order by Value;", (object) "Name, Value, Enum");
          oleDbCommand.Parameters.AddWithValue("name", (object) enumName);
          using (IDataReader dataReader = OleDbHelper.ExecuteReader(oleDbCommand, dbConnection))
          {
            while (dataReader.Read())
              ((Collection<string, OidEnum>) enums).Add((object) new OidEnum()
              {
                Id = DatabaseFunctions.GetDouble(dataReader, 1).ToString(),
                Name = DatabaseFunctions.GetString(dataReader, 2)
              });
          }
        }
      }
      return enums;
    }

    public Oids GetSearchingOidsByDescription(string searchCriteria, string searchMIBsCriteria)
    {
      throw new NotImplementedException();
    }

    public void CancelRunningCommand()
    {
      if (MibDAL.CancellationTokenSource == null)
        return;
      try
      {
        MibDAL.CancellationTokenSource.Cancel();
      }
      catch (AggregateException ex)
      {
      }
    }

    public Oids GetSearchingOidsByName(string searchCriteria)
    {
      List<string> stringList = new List<string>();
      Oids searchingOidsByName = new Oids();
      using (OleDbConnection connection = MibHelper.GetDBConnection())
      {
        connection.Open();
        MibDAL.CancellationTokenSource = new CancellationTokenSource();
        using (OleDbCommand oleDbCommand = new OleDbCommand())
        {
          oleDbCommand.CommandText = string.Format("SELECT TOP 250 {0} FROM Tree WHERE (Primary = -1) AND ( Name LIKE @SearchValue OR Description LIKE '%' + @SearchValue + '%' OR Mib LIKE @SearchValue)", (object) "Index, MIB, Name, Primary, OID, Description, Access, Status, Units, Enum, TypeS");
          oleDbCommand.Parameters.AddWithValue("@SearchValue", (object) searchCriteria);
          using (IDataReader reader = OleDbHelper.ExecuteReader(oleDbCommand, connection))
          {
            foreach (Oid oid in (Collection<string, Oid>) Task.Factory.StartNew<Oids>((Func<Oids>) (() => this.getOidsFromReader(reader, connection)), MibDAL.CancellationTokenSource.Token).Result)
              ((Collection<string, Oid>) searchingOidsByName).Add((object) oid);
          }
        }
      }
      return searchingOidsByName;
    }

    private Oids getOidsFromReader(IDataReader reader, OleDbConnection connection)
    {
      Oids oidsFromReader = new Oids();
      while (reader.Read())
      {
        MibDAL.CancellationTokenSource.Token.ThrowIfCancellationRequested();
        Oid oid = this.CreateOid(reader, connection);
        ((Collection<string, Oid>) oidsFromReader).Add((object) oid);
      }
      return oidsFromReader;
    }

    public bool IsMibDatabaseAvailable() => MibHelper.IsMIBDatabaseAvailable();

    private Oid CreateOid(IDataReader reader, OleDbConnection connection)
    {
      Oid oid = new Oid()
      {
        ID = DatabaseFunctions.GetString(reader, 4),
        Name = DatabaseFunctions.GetString(reader, 2),
        Description = DatabaseFunctions.GetString(reader, 5),
        MIB = DatabaseFunctions.GetString(reader, 1),
        Access = (AccessType) (int) DatabaseFunctions.GetByte(reader, 6),
        Status = (StatusType) (int) DatabaseFunctions.GetByte(reader, 7),
        Units = DatabaseFunctions.GetString(reader, 8),
        StringType = DatabaseFunctions.GetString(reader, 10)
      };
      oid.HasChildren = this.HasChildren(oid.ID, connection);
      oid.Enums = this.GetEnums(DatabaseFunctions.GetString(reader, 9));
      oid.TreeIndex = DatabaseFunctions.GetInt32(reader, 0).ToString();
      MibHelper.CleanupDescription(oid);
      MibHelper.SetTypeInfo(oid);
      return oid;
    }

    private bool HasChildren(string oid, OleDbConnection connection)
    {
      using (OleDbCommand oleDbCommand = new OleDbCommand())
      {
        oleDbCommand.CommandText = "Select COUNT(*) from Tree WHERE Primary = -1 AND ParentOID=@oid;";
        oleDbCommand.Parameters.AddWithValue("parentOid", (object) oid);
        if ((int) OleDbHelper.ExecuteScalar(oleDbCommand, connection) > 0)
          return true;
      }
      return false;
    }

    private enum EnumColumnOrder
    {
      EnumName,
      EnumValue,
      EnumEnum,
    }
  }
}
