// Decompiled with JetBrains decompiler
// Type: SolarWinds.Orion.Core.BusinessLayer.MibHelper
// Assembly: SolarWinds.Orion.Core.BusinessLayer, Version=2019.4.5200.9083, Culture=neutral, PublicKeyToken=null
// MVID: E12E8C85-5CD9-4E06-8801-182E5104FADE
// Assembly location: E:\task5.dll

using SolarWinds.Logging;
using SolarWinds.NPM.Common.Models;
using SolarWinds.Orion.Core.Common.Models;
using SolarWinds.Orion.Core.Common.Models.Mib;
using System;
using System.Data.OleDb;
using System.IO;
using System.Text;

#nullable disable
namespace SolarWinds.Orion.Core.BusinessLayer
{
  public static class MibHelper
  {
    private static OleDbConnection connection;
    private static object connectionLock = new object();
    private static string dbConnectionString;
    private static Log log = new Log();

    private static OleDbConnection CurrentConnection
    {
      get
      {
        lock (MibHelper.connectionLock)
          return MibHelper.connection;
      }
      set
      {
        lock (MibHelper.connectionLock)
          MibHelper.connection = value;
      }
    }

    public static void ForceConnectionClose()
    {
      OleDbConnection.ReleaseObjectPool();
      MibHelper.CurrentConnection.Close();
    }

    public static void CleanupDescription(Oid oid)
    {
      oid.Description = oid.Description.Replace("\r\n", "\n");
      oid.Description = oid.Description.Replace("\r", "\n");
      oid.Description = oid.Description.Replace("\n", "\r\n");
    }

    public static void SetTypeInfo(Oid oid)
    {
      if (((Collection<string, OidEnum>) oid.Enums).Count > 0)
      {
        oid.PollType = (CustomPollerType) 2;
        oid.VariableType = (OidVariableType) 12;
      }
      else
      {
        switch (oid.StringType.ToUpper())
        {
          case "BITS":
          case "DISPLAYSTRING":
          case "DISPLAY_STRING":
          case "OCTECTSTRING":
          case "OPAQUE":
            oid.PollType = (CustomPollerType) 2;
            oid.VariableType = (OidVariableType) 1;
            break;
          case "COUNTER":
          case "COUNTER32":
          case "COUNTER64":
            oid.PollType = (CustomPollerType) 0;
            oid.VariableType = (OidVariableType) 5;
            break;
          case "GAUGE":
          case "GAUGE32":
            oid.PollType = (CustomPollerType) 1;
            oid.VariableType = (OidVariableType) 6;
            break;
          case "INTEGER":
          case "UINTEGER32":
          case "UNSIGNEDINTEGER32":
            oid.PollType = (CustomPollerType) 1;
            oid.VariableType = (OidVariableType) 6;
            break;
          case "IP":
          case "IP ADDRESS":
          case "IP-ADDRESS":
          case "IP_ADDRESS":
            oid.PollType = (CustomPollerType) 2;
            oid.VariableType = (OidVariableType) 4;
            break;
          case "OBJECT IDENTIFIER":
          case "OBJECT-IDENTIFIER":
          case "OBJECTIDENTIFIER":
          case "OBJECT_IDENTIFIER":
          case "OID":
            oid.PollType = (CustomPollerType) 2;
            oid.VariableType = (OidVariableType) 1;
            break;
          case "SEQUENCE":
            oid.PollType = (CustomPollerType) 2;
            oid.VariableType = (OidVariableType) 17;
            break;
          case "TIMETICKS":
            oid.PollType = (CustomPollerType) 2;
            oid.VariableType = (OidVariableType) 13;
            break;
          default:
            oid.PollType = (CustomPollerType) 2;
            oid.VariableType = (OidVariableType) 0;
            break;
        }
      }
    }

    public static OleDbConnection GetDBConnection()
    {
      if (string.IsNullOrEmpty(MibHelper.dbConnectionString))
      {
        StringBuilder stringBuilder = new StringBuilder("Provider=Microsoft.Jet.OLEDB.4.0;");
        stringBuilder.Append("Data Source=");
        stringBuilder.Append(MibHelper.FindMibDbPath() + "MIBs.cfg");
        stringBuilder.Append(";Mode=Read;OLE DB Services=-1;Persist Security Info=False;Jet OLEDB:Database ");
        stringBuilder.Append("Password=SW_MIBs");
        MibHelper.dbConnectionString = stringBuilder.ToString();
      }
      return MibHelper.CurrentConnection = new OleDbConnection(MibHelper.dbConnectionString);
    }

    private static string FindMibDbPath()
    {
      string mibDbPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + "\\SolarWinds\\";
      if (File.Exists(mibDbPath + "MIBs.cfg"))
        return mibDbPath;
      MibHelper.log.DebugFormat("Could not find MIBs Database. Please, download MIBs Database from http://solarwinds.s3.amazonaws.com/solarwinds/Release/MIB-Database/MIBs.zip and decompress the MIBs.cfg file to " + mibDbPath + " to correct this problem", Array.Empty<object>());
      throw new ApplicationException("Unable to determine Mibs.cfg location");
    }

    public static bool IsMIBDatabaseAvailable()
    {
      try
      {
        MibHelper.FindMibDbPath();
        return true;
      }
      catch (ApplicationException ex)
      {
        return false;
      }
    }

    public static string FormatSearchCriteria(string searchCriteria)
    {
      string str1 = string.Empty;
      string str2 = searchCriteria;
      char[] chArray = new char[1]{ ' ' };
      foreach (string str3 in str2.Split(chArray))
      {
        if (!string.IsNullOrEmpty(str3.Trim()))
          str1 = str1 + str3.Replace("*", "%") + " ";
      }
      return str1.TrimEnd();
    }
  }
}
