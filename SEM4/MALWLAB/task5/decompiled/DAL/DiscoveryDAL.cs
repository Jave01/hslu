// Decompiled with JetBrains decompiler
// Type: SolarWinds.Orion.Core.BusinessLayer.DAL.DiscoveryDAL
// Assembly: SolarWinds.Orion.Core.BusinessLayer, Version=2019.4.5200.9083, Culture=neutral, PublicKeyToken=null
// MVID: E12E8C85-5CD9-4E06-8801-182E5104FADE
// Assembly location: E:\task5.dll

using SolarWinds.Orion.Common;
using SolarWinds.Orion.Core.Common;
using SolarWinds.Orion.Core.Common.Models;
using SolarWinds.Orion.Core.Discovery.DataAccess;
using SolarWinds.Orion.Core.Models.Enums;
using SolarWinds.Orion.Core.Models.OldDiscoveryModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

#nullable disable
namespace SolarWinds.Orion.Core.BusinessLayer.DAL
{
  public static class DiscoveryDAL
  {
    public const string subTypeICMP = "ICMP";
    public const string subTypeSNMP = "SNMP";
    public const int UNDEFINED_VALUE = -2;

    [Obsolete("This method belongs to old discovery process.", true)]
    public static StartImportStatus ImportDiscoveryResults(
      Guid importID,
      List<DiscoveryResult> discoveryResults)
    {
      return (StartImportStatus) 2;
    }

    [Obsolete("This method belongs to old discovery process.", true)]
    public static bool IsImportInProgress(int discoveryProfileID) => false;

    [Obsolete("This method belongs to old discovery process.", true)]
    public static string GetCPUPollerTypeByOID(string oid) => string.Empty;

    [Obsolete("This method belongs to old discovery process.", true)]
    public static Intervals GetEnginesPollingIntervals(int engineID) => new Intervals();

    public static Intervals GetSettingsPollingIntervals()
    {
      return new Intervals()
      {
        RediscoveryInterval = int.Parse(SettingsDAL.Get("SWNetPerfMon-Settings-Default Rediscovery Interval")),
        NodePollInterval = int.Parse(SettingsDAL.Get("SWNetPerfMon-Settings-Default Node Poll Interval")),
        VolumePollInterval = int.Parse(SettingsDAL.Get("SWNetPerfMon-Settings-Default Volume Poll Interval")),
        NodeStatPollInterval = int.Parse(SettingsDAL.Get("SWNetPerfMon-Settings-Default Node Stat Poll Interval")),
        VolumeStatPollInterval = int.Parse(SettingsDAL.Get("SWNetPerfMon-Settings-Default Volume Stat Poll Interval"))
      };
    }

    public static List<SnmpEntry> GetAllCredentials()
    {
      SqlCommand textCommand = SqlHelper.GetTextCommand("\r\n    Select Distinct 1 As SnmpVersion, CommunityString, Null as SNMPUser, Null as Context, Null as AuthPassword, Null as EncryptPassword, \r\n0 as AuthLevel, Null as AuthMethod, 0 as EncryptMethod From dbo.DiscoverySNMPCredentials\r\nUnion\r\n(\r\n\tSELECT 3 As SnmpVersion, Null as CommunityString, SNMPUser, Context, AuthPassword, EncryptPassword, AuthLevel, AuthMethod, EncryptMethod \r\n\tFROM DiscoverySNMPCredentialsV3\r\n)");
      List<SnmpEntry> allCredentials = new List<SnmpEntry>()
      {
        new SnmpEntry()
        {
          Name = "public",
          Version = (SNMPVersion) 1,
          Selected = true
        },
        new SnmpEntry()
        {
          Name = "private",
          Version = (SNMPVersion) 1,
          Selected = true
        }
      };
      using (IDataReader dataReader = SqlHelper.ExecuteReader(textCommand))
      {
        while (dataReader.Read())
        {
          string str = DatabaseFunctions.GetString(dataReader, "CommunityString");
          int int32_1 = DatabaseFunctions.GetInt32(dataReader, "SnmpVersion");
          if (!str.Equals("public", StringComparison.OrdinalIgnoreCase) && !str.Equals("private", StringComparison.OrdinalIgnoreCase))
          {
            if (int32_1 == 3)
            {
              DiscoverySNMPCredentialsV3Entry.AuthenticationMethods int32_2 = (DiscoverySNMPCredentialsV3Entry.AuthenticationMethods) DatabaseFunctions.GetInt32(dataReader, "AuthMethod");
              DiscoverySNMPCredentialsV3Entry.EncryptionMethods int32_3 = (DiscoverySNMPCredentialsV3Entry.EncryptionMethods) DatabaseFunctions.GetInt32(dataReader, "EncryptMethod");
              SnmpEntry snmpEntry = new SnmpEntry()
              {
                UserName = DatabaseFunctions.GetString(dataReader, "SNMPUser"),
                Context = DatabaseFunctions.GetString(dataReader, "Context"),
                AuthPassword = DatabaseFunctions.GetString(dataReader, "AuthPassword"),
                PrivPassword = DatabaseFunctions.GetString(dataReader, "EncryptPassword"),
                AuthLevel = (SnmpAuthenticationLevel) DatabaseFunctions.GetInt32(dataReader, "AuthLevel"),
                AuthMethod = int32_2 == 2 ? (SnmpAuthMethod) 1 : (SnmpAuthMethod) 0,
                Version = (SNMPVersion) 3,
                Selected = true
              };
              switch (int32_3 - 2)
              {
                case 0:
                  snmpEntry.PrivMethod = (SnmpPrivMethod) 1;
                  break;
                case 1:
                  snmpEntry.PrivMethod = (SnmpPrivMethod) 2;
                  break;
                case 2:
                  snmpEntry.PrivMethod = (SnmpPrivMethod) 3;
                  break;
                default:
                  snmpEntry.PrivMethod = (SnmpPrivMethod) 0;
                  break;
              }
              allCredentials.Add(snmpEntry);
            }
            else
              allCredentials.Add(new SnmpEntry()
              {
                Name = str,
                Version = (SNMPVersion) 1,
                Selected = true
              });
          }
        }
      }
      return allCredentials;
    }
  }
}
