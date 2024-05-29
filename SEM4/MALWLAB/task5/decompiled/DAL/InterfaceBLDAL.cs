// Decompiled with JetBrains decompiler
// Type: SolarWinds.Orion.Core.BusinessLayer.DAL.InterfaceBLDAL
// Assembly: SolarWinds.Orion.Core.BusinessLayer, Version=2019.4.5200.9083, Culture=neutral, PublicKeyToken=null
// MVID: E12E8C85-5CD9-4E06-8801-182E5104FADE
// Assembly location: E:\task5.dll

using SolarWinds.Logging;
using SolarWinds.Orion.Common;
using SolarWinds.Orion.Core.Common;
using SolarWinds.Orion.Core.Common.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;

#nullable disable
namespace SolarWinds.Orion.Core.BusinessLayer.DAL
{
  internal class InterfaceBLDAL
  {
    private static readonly Log log = new Log();
    private static object insertInterfaceLock = new object();
    private static bool _areInterfacesAllowed = SolarWinds.Orion.Core.Common.PackageManager.PackageManager.InstanceWithCache.IsPackageInstalled("Orion.Interfaces");

    public static Interfaces GetNodesInterfaces(IEnumerable<int> nodeIDs)
    {
      if (!InterfaceBLDAL._areInterfacesAllowed)
        return new Interfaces();
      StringBuilder stringBuilder = new StringBuilder("SELECT * FROM Interfaces WHERE NodeID IN (");
      foreach (int nodeId in nodeIDs)
      {
        stringBuilder.Append(nodeId);
        stringBuilder.Append(',');
      }
      --stringBuilder.Length;
      stringBuilder.Append(')');
      // ISSUE: method pointer
      return Collection<int, Interface>.FillCollection<Interfaces>(new Collection<int, Interface>.CreateElement((object) null, __methodptr(CreateNodeInterface)), stringBuilder.ToString(), Array.Empty<SqlParameter>());
    }

    internal static Interface CreateNodeInterface(IDataReader reader)
    {
      Interface nodeInterface = new Interface();
      for (int i = 0; i < reader.FieldCount; ++i)
      {
        string name = reader.GetName(i);
        switch (name)
        {
          case "AdminStatus":
            nodeInterface.AdminStatus = DatabaseFunctions.GetInt16(reader, i);
            break;
          case "AdminStatusLED":
            nodeInterface.AdminStatusLED = DatabaseFunctions.GetString(reader, i);
            break;
          case "Caption":
            nodeInterface.Caption = DatabaseFunctions.GetString(reader, i);
            break;
          case "CollectAvailability":
            nodeInterface.CollectAvailability = reader.IsDBNull(i) || DatabaseFunctions.GetBoolean(reader, i);
            break;
          case "Counter64":
            nodeInterface.Counter64 = DatabaseFunctions.GetString(reader, i);
            break;
          case "CustomBandwidth":
            nodeInterface.CustomBandwidth = DatabaseFunctions.GetBoolean(reader, i);
            break;
          case "FullName":
            nodeInterface.FullName = DatabaseFunctions.GetString(reader, i);
            break;
          case "IfName":
            nodeInterface.IfName = DatabaseFunctions.GetString(reader, i);
            break;
          case "InBandwidth":
            nodeInterface.InBandwidth = DatabaseFunctions.GetDouble(reader, i);
            break;
          case "InDiscardsThisHour":
            nodeInterface.InDiscardsThisHour = DatabaseFunctions.GetFloat(reader, i);
            break;
          case "InDiscardsToday":
            nodeInterface.InDiscardsToday = DatabaseFunctions.GetFloat(reader, i);
            break;
          case "InErrorsThisHour":
            nodeInterface.InErrorsThisHour = DatabaseFunctions.GetFloat(reader, i);
            break;
          case "InErrorsToday":
            nodeInterface.InErrorsToday = DatabaseFunctions.GetFloat(reader, i);
            break;
          case "InMcastPps":
            nodeInterface.InMcastPps = DatabaseFunctions.GetFloat(reader, i);
            break;
          case "InPercentUtil":
            nodeInterface.InPercentUtil = DatabaseFunctions.GetFloat(reader, i);
            break;
          case "InPktSize":
            nodeInterface.InPktSize = DatabaseFunctions.GetInt16(reader, i);
            break;
          case "InPps":
            nodeInterface.InPps = DatabaseFunctions.GetFloat(reader, i);
            break;
          case "InUcastPps":
            nodeInterface.InUcastPps = DatabaseFunctions.GetFloat(reader, i);
            break;
          case "Inbps":
            nodeInterface.InBps = DatabaseFunctions.GetFloat(reader, i);
            break;
          case "InterfaceAlias":
            nodeInterface.InterfaceAlias = DatabaseFunctions.GetString(reader, i);
            break;
          case "InterfaceID":
            nodeInterface.InterfaceID = DatabaseFunctions.GetInt32(reader, i);
            break;
          case "InterfaceIcon":
            nodeInterface.InterfaceIcon = DatabaseFunctions.GetString(reader, i);
            break;
          case "InterfaceIndex":
            nodeInterface.InterfaceIndex = DatabaseFunctions.GetInt32(reader, i);
            break;
          case "InterfaceLastChange":
            nodeInterface.InterfaceLastChange = DatabaseFunctions.GetDateTime(reader, i);
            break;
          case "InterfaceMTU":
            nodeInterface.InterfaceMTU = DatabaseFunctions.GetInt32(reader, i);
            break;
          case "InterfaceName":
            nodeInterface.InterfaceName = DatabaseFunctions.GetString(reader, i);
            break;
          case "InterfaceSpeed":
            nodeInterface.InterfaceSpeed = DatabaseFunctions.GetDouble(reader, i);
            break;
          case "InterfaceSubType":
            nodeInterface.InterfaceSubType = DatabaseFunctions.GetInt32(reader, i);
            break;
          case "InterfaceType":
            nodeInterface.InterfaceType = DatabaseFunctions.GetInt32(reader, i);
            break;
          case "InterfaceTypeDescription":
            nodeInterface.InterfaceTypeDescription = DatabaseFunctions.GetString(reader, i);
            break;
          case "InterfaceTypeName":
            nodeInterface.InterfaceTypeName = DatabaseFunctions.GetString(reader, i);
            break;
          case "LastSync":
            nodeInterface.LastSync = DatabaseFunctions.GetDateTime(reader, i);
            break;
          case "MaxInBpsTime":
            nodeInterface.MaxInBpsTime = DatabaseFunctions.GetDateTime(reader, i);
            break;
          case "MaxInBpsToday":
            nodeInterface.MaxInBpsToday = DatabaseFunctions.GetFloat(reader, i);
            break;
          case "MaxOutBpsTime":
            nodeInterface.MaxOutBpsTime = DatabaseFunctions.GetDateTime(reader, i);
            break;
          case "MaxOutBpsToday":
            nodeInterface.MaxOutBpsToday = DatabaseFunctions.GetFloat(reader, i);
            break;
          case "NextPoll":
            nodeInterface.NextPoll = DatabaseFunctions.GetDateTime(reader, i);
            break;
          case "NextRediscovery":
            nodeInterface.NextRediscovery = DatabaseFunctions.GetDateTime(reader, i);
            break;
          case "NodeID":
            nodeInterface.NodeID = DatabaseFunctions.GetInt32(reader, i);
            break;
          case "ObjectSubType":
            nodeInterface.ObjectSubType = DatabaseFunctions.GetString(reader, i);
            break;
          case "OperStatus":
            nodeInterface.OperStatus = DatabaseFunctions.GetInt16(reader, i);
            break;
          case "OperStatusLED":
            nodeInterface.OperStatusLED = DatabaseFunctions.GetString(reader, i);
            break;
          case "OutBandwidth":
            nodeInterface.OutBandwidth = DatabaseFunctions.GetDouble(reader, i);
            break;
          case "OutDiscardsThisHour":
            nodeInterface.OutDiscardsThisHour = DatabaseFunctions.GetFloat(reader, i);
            break;
          case "OutDiscardsToday":
            nodeInterface.OutDiscardsToday = DatabaseFunctions.GetFloat(reader, i);
            break;
          case "OutErrorsThisHour":
            nodeInterface.OutErrorsThisHour = DatabaseFunctions.GetFloat(reader, i);
            break;
          case "OutErrorsToday":
            nodeInterface.OutErrorsToday = DatabaseFunctions.GetFloat(reader, i);
            break;
          case "OutMcastPps":
            nodeInterface.OutMcastPps = DatabaseFunctions.GetFloat(reader, i);
            break;
          case "OutPercentUtil":
            nodeInterface.OutPercentUtil = DatabaseFunctions.GetFloat(reader, i);
            break;
          case "OutPktSize":
            nodeInterface.OutPktSize = DatabaseFunctions.GetInt16(reader, i);
            break;
          case "OutPps":
            nodeInterface.OutPps = DatabaseFunctions.GetFloat(reader, i);
            break;
          case "OutUcastPps":
            nodeInterface.OutUcastPps = DatabaseFunctions.GetFloat(reader, i);
            break;
          case "Outbps":
            nodeInterface.OutBps = DatabaseFunctions.GetFloat(reader, i);
            break;
          case "PhysicalAddress":
            nodeInterface.PhysicalAddress = DatabaseFunctions.GetString(reader, i);
            break;
          case "PollInterval":
            nodeInterface.PollInterval = DatabaseFunctions.GetInt32(reader, i);
            break;
          case "RediscoveryInterval":
            nodeInterface.RediscoveryInterval = DatabaseFunctions.GetInt32(reader, i);
            break;
          case "Severity":
            nodeInterface.Severity = DatabaseFunctions.GetInt32(reader, i);
            break;
          case "StatCollection":
            nodeInterface.StatCollection = DatabaseFunctions.GetInt16(reader, i);
            break;
          case "Status":
            nodeInterface.Status = DatabaseFunctions.GetString(reader, i);
            break;
          case "StatusLED":
            nodeInterface.StatusLED = DatabaseFunctions.GetString(reader, i);
            break;
          case "UnManageFrom":
            nodeInterface.UnManageFrom = DatabaseFunctions.GetDateTime(reader, i);
            break;
          case "UnManageUntil":
            nodeInterface.UnManageUntil = DatabaseFunctions.GetDateTime(reader, i);
            break;
          case "UnManaged":
            nodeInterface.UnManaged = DatabaseFunctions.GetBoolean(reader, i);
            break;
          case "UnPluggable":
            nodeInterface.UnPluggable = DatabaseFunctions.GetBoolean(reader, i);
            break;
          default:
            if (CustomPropertyMgr.IsCustom("Interfaces", name))
            {
              nodeInterface.CustomProperties[name] = reader[i];
              break;
            }
            InterfaceBLDAL.log.DebugFormat("Skipping Interface property {0}, value {1}", (object) name, reader[i]);
            break;
        }
      }
      return nodeInterface;
    }

    public static int CreateNewInterface(Interface _interface, bool checkIfAlreadyExists)
    {
      return InterfaceBLDAL.CreateNewInterface(_interface, checkIfAlreadyExists ? InterfaceBLDAL.SqlManagedInterfaceExists : string.Empty);
    }

    public static int CreateNewInterface(Interface _interface, string checkQuery)
    {
      string str = "\r\n        INSERT INTO [Interfaces]\r\n        ([NodeID]\r\n        ,[ObjectSubType]\r\n        ,[InterfaceName]\r\n        ,[InterfaceIndex]\r\n        ,[InterfaceType]\r\n        ,[InterfaceSubType]\r\n        ,[InterfaceTypeName]\r\n        ,[InterfaceTypeDescription]\r\n        ,[InterfaceSpeed]\r\n        ,[InterfaceMTU]\r\n        ,[InterfaceLastChange]\r\n        ,[PhysicalAddress]\r\n        ,[UnManaged]\r\n        ,[UnManageFrom]\r\n        ,[UnManageUntil]\r\n        ,[AdminStatus]\r\n        ,[OperStatus]\r\n        ,[InBandwidth]\r\n        ,[OutBandwidth]\r\n        ,[Caption]\r\n        ,[PollInterval]\r\n        ,[RediscoveryInterval]\r\n        ,[FullName]\r\n        ,[Status]\r\n        ,[StatusLED]\r\n        ,[AdminStatusLED]\r\n        ,[OperStatusLED]\r\n        ,[InterfaceIcon]\r\n        ,[Outbps]\r\n        ,[Inbps]\r\n        ,[OutPercentUtil]\r\n        ,[InPercentUtil]\r\n        ,[OutPps]\r\n        ,[InPps]\r\n        ,[InPktSize]\r\n        ,[OutPktSize]\r\n        ,[OutUcastPps]\r\n        ,[OutMcastPps]\r\n        ,[InUcastPps]\r\n        ,[InMcastPps]\r\n        ,[InDiscardsThisHour]\r\n        ,[InDiscardsToday]\r\n        ,[InErrorsThisHour]\r\n        ,[InErrorsToday]\r\n        ,[OutDiscardsThisHour]\r\n        ,[OutDiscardsToday]\r\n        ,[OutErrorsThisHour]\r\n        ,[OutErrorsToday]\r\n        ,[MaxInBpsToday]\r\n        ,[MaxInBpsTime]\r\n        ,[MaxOutBpsToday]\r\n        ,[MaxOutBpsTime]\r\n        ,[NextRediscovery]\r\n        ,[NextPoll]\r\n        ,[Counter64]\r\n        ,[StatCollection]\r\n        ,[LastSync]\r\n        ,[InterfaceAlias]\r\n        ,[IfName]\r\n        ,[Severity]\r\n        ,[CustomBandwidth]\r\n        ,[UnPluggable]\r\n        ,[CollectAvailability]\r\n        )\r\n        VALUES\r\n        (@NodeID\r\n        ,@ObjectSubType\r\n        ,@InterfaceName\r\n        ,@InterfaceIndex\r\n        ,@InterfaceType\r\n        ,@InterfaceSubType\r\n        ,@InterfaceTypeName\r\n        ,@InterfaceTypeDescription\r\n        ,@InterfaceSpeed\r\n        ,@InterfaceMTU\r\n        ,@InterfaceLastChange\r\n        ,@PhysicalAddress\r\n        ,@UnManaged\r\n        ,@UnManageFrom\r\n        ,@UnManageUntil\r\n        ,@AdminStatus\r\n        ,@OperStatus\r\n        ,@InBandwidth\r\n        ,@OutBandwidth\r\n        ,@Caption\r\n        ,@PollInterval\r\n        ,@RediscoveryInterval\r\n        ,@FullName\r\n        ,@Status\r\n        ,@StatusLED\r\n        ,@AdminStatusLED\r\n        ,@OperStatusLED\r\n        ,@InterfaceIcon\r\n        ,@Outbps\r\n        ,@Inbps\r\n        ,@OutPercentUtil\r\n        ,@InPercentUtil\r\n        ,@OutPps\r\n        ,@InPps\r\n        ,@InPktSize\r\n        ,@OutPktSize\r\n        ,@OutUcastPps\r\n        ,@OutMcastPps\r\n        ,@InUcastPps\r\n        ,@InMcastPps\r\n        ,@InDiscardsThisHour\r\n        ,@InDiscardsToday\r\n        ,@InErrorsThisHour\r\n        ,@InErrorsToday\r\n        ,@OutDiscardsThisHour\r\n        ,@OutDiscardsToday\r\n        ,@OutErrorsThisHour\r\n        ,@OutErrorsToday\r\n        ,@MaxInBpsToday\r\n        ,@MaxInBpsTime\r\n        ,@MaxOutBpsToday\r\n        ,@MaxOutBpsTime\r\n        ,@NextRediscovery\r\n        ,@NextPoll\r\n        ,@Counter64\r\n        ,@StatCollection\r\n        ,@LastSync\r\n        ,@InterfaceAlias\r\n        ,@IfName\r\n        ,@Severity\r\n        ,@CustomBandwidth\r\n        ,@UnPluggable\r\n        ,@CollectAvailability\r\n        )\r\n\r\n        SELECT Scope_Identity();";
      if (!string.IsNullOrEmpty(checkQuery))
        str = "\r\n                " + checkQuery + "\r\n                BEGIN " + str + " \r\n                END\r\n                ELSE\r\n                BEGIN\r\n                    SELECT -1;\r\n                END";
      SqlCommand textCommand = SqlHelper.GetTextCommand(str);
      _interface = new DALHelper<Interface>().Initialize(_interface);
      textCommand.Parameters.AddWithValue("NodeID", (object) _interface.NodeID);
      textCommand.Parameters.AddWithValue("ObjectSubType", (object) _interface.ObjectSubType);
      textCommand.Parameters.AddWithValue("InterfaceName", (object) _interface.InterfaceName);
      textCommand.Parameters.Add("@InterfaceIndex", SqlDbType.Int, 4).Value = (object) _interface.InterfaceIndex;
      textCommand.Parameters.AddWithValue("InterfaceType", (object) _interface.InterfaceType);
      textCommand.Parameters.AddWithValue("InterfaceSubType", (object) _interface.InterfaceSubType);
      textCommand.Parameters.AddWithValue("InterfaceTypeName", (object) _interface.InterfaceTypeName);
      textCommand.Parameters.AddWithValue("InterfaceTypeDescription", (object) _interface.InterfaceTypeDescription);
      textCommand.Parameters.AddWithValue("InterfaceSpeed", (object) _interface.InterfaceSpeed);
      textCommand.Parameters.AddWithValue("InterfaceMTU", (object) _interface.InterfaceMTU);
      if (_interface.InterfaceLastChange == DateTime.MinValue)
        textCommand.Parameters.Add("@InterfaceLastChange", SqlDbType.DateTime).Value = (object) DBNull.Value;
      else
        textCommand.Parameters.Add("@InterfaceLastChange", SqlDbType.DateTime).Value = (object) _interface.InterfaceLastChange;
      textCommand.Parameters.AddWithValue("PhysicalAddress", (object) _interface.PhysicalAddress);
      textCommand.Parameters.AddWithValue("AdminStatus", (object) _interface.AdminStatus);
      textCommand.Parameters.AddWithValue("OperStatus", (object) _interface.OperStatus);
      textCommand.Parameters.AddWithValue("InBandwidth", (object) _interface.InBandwidth);
      textCommand.Parameters.AddWithValue("OutBandwidth", (object) _interface.OutBandwidth);
      textCommand.Parameters.AddWithValue("Caption", (object) _interface.Caption);
      textCommand.Parameters.AddWithValue("PollInterval", (object) _interface.PollInterval);
      textCommand.Parameters.AddWithValue("RediscoveryInterval", (object) _interface.RediscoveryInterval);
      textCommand.Parameters.AddWithValue("FullName", (object) _interface.FullName);
      textCommand.Parameters.AddWithValue("Status", (object) _interface.Status);
      textCommand.Parameters.AddWithValue("StatusLED", (object) _interface.StatusLED);
      textCommand.Parameters.AddWithValue("AdminStatusLED", (object) _interface.AdminStatusLED);
      textCommand.Parameters.AddWithValue("OperStatusLED", (object) _interface.OperStatusLED);
      textCommand.Parameters.AddWithValue("InterfaceIcon", (object) _interface.InterfaceIcon);
      textCommand.Parameters.AddWithValue("Outbps", (object) _interface.OutBps);
      textCommand.Parameters.AddWithValue("Inbps", (object) _interface.InBps);
      textCommand.Parameters.AddWithValue("OutPercentUtil", (object) _interface.OutPercentUtil);
      textCommand.Parameters.AddWithValue("InPercentUtil", (object) _interface.InPercentUtil);
      textCommand.Parameters.AddWithValue("OutPps", (object) _interface.OutPps);
      textCommand.Parameters.AddWithValue("InPps", (object) _interface.InPps);
      textCommand.Parameters.AddWithValue("InPktSize", (object) _interface.InPktSize);
      textCommand.Parameters.AddWithValue("OutPktSize", (object) _interface.OutPktSize);
      textCommand.Parameters.AddWithValue("OutUcastPps", (object) _interface.OutUcastPps);
      textCommand.Parameters.AddWithValue("OutMcastPps", (object) _interface.OutMcastPps);
      textCommand.Parameters.AddWithValue("InUcastPps", (object) _interface.InUcastPps);
      textCommand.Parameters.AddWithValue("InMcastPps", (object) _interface.InMcastPps);
      textCommand.Parameters.AddWithValue("InDiscardsThisHour", (object) _interface.InDiscardsThisHour);
      textCommand.Parameters.AddWithValue("InDiscardsToday", (object) _interface.InDiscardsToday);
      textCommand.Parameters.AddWithValue("InErrorsThisHour", (object) _interface.InErrorsThisHour);
      textCommand.Parameters.AddWithValue("InErrorsToday", (object) _interface.InErrorsToday);
      textCommand.Parameters.AddWithValue("OutDiscardsThisHour", (object) _interface.OutDiscardsThisHour);
      textCommand.Parameters.AddWithValue("OutDiscardsToday", (object) _interface.OutDiscardsToday);
      textCommand.Parameters.AddWithValue("OutErrorsThisHour", (object) _interface.OutErrorsThisHour);
      textCommand.Parameters.AddWithValue("OutErrorsToday", (object) _interface.OutErrorsToday);
      textCommand.Parameters.AddWithValue("MaxInBpsToday", (object) _interface.MaxInBpsToday);
      if (_interface.MaxInBpsTime == DateTime.MinValue)
        textCommand.Parameters.Add("@MaxInBpsTime", SqlDbType.DateTime).Value = (object) DBNull.Value;
      else
        textCommand.Parameters.Add("@MaxInBpsTime", SqlDbType.DateTime).Value = (object) _interface.MaxInBpsTime;
      textCommand.Parameters.AddWithValue("MaxOutBpsToday", (object) _interface.MaxOutBpsToday);
      if (_interface.MaxOutBpsTime == DateTime.MinValue)
        textCommand.Parameters.Add("@MaxOutBpsTime", SqlDbType.DateTime).Value = (object) DBNull.Value;
      else
        textCommand.Parameters.Add("@MaxOutBpsTime", SqlDbType.DateTime).Value = (object) _interface.MaxOutBpsTime;
      if (_interface.NextRediscovery == DateTime.MinValue)
        textCommand.Parameters.Add("@NextRediscovery", SqlDbType.DateTime).Value = (object) DBNull.Value;
      else
        textCommand.Parameters.Add("@NextRediscovery", SqlDbType.DateTime).Value = (object) _interface.NextRediscovery;
      if (_interface.NextPoll == DateTime.MinValue)
        textCommand.Parameters.Add("@NextPoll", SqlDbType.DateTime).Value = (object) DBNull.Value;
      else
        textCommand.Parameters.Add("@NextPoll", SqlDbType.DateTime).Value = (object) _interface.NextPoll;
      textCommand.Parameters.AddWithValue("Counter64", (object) _interface.Counter64);
      textCommand.Parameters.AddWithValue("StatCollection", (object) _interface.StatCollection);
      if (_interface.LastSync == DateTime.MinValue)
        textCommand.Parameters.Add("@LastSync", SqlDbType.DateTime).Value = (object) DBNull.Value;
      else
        textCommand.Parameters.Add("@LastSync", SqlDbType.DateTime).Value = (object) _interface.LastSync;
      textCommand.Parameters.AddWithValue("InterfaceAlias", (object) _interface.InterfaceAlias);
      textCommand.Parameters.AddWithValue("IfName", (object) _interface.IfName);
      textCommand.Parameters.AddWithValue("Severity", (object) _interface.Severity);
      textCommand.Parameters.AddWithValue("CustomBandwidth", (object) _interface.CustomBandwidth);
      textCommand.Parameters.AddWithValue("UnPluggable", (object) _interface.UnPluggable);
      textCommand.Parameters.AddWithValue("UnManaged", (object) _interface.UnManaged);
      textCommand.Parameters.AddWithValue("UnManageFrom", CommonHelper.GetDateTimeValue(_interface.UnManageFrom));
      textCommand.Parameters.AddWithValue("UnManageUntil", CommonHelper.GetDateTimeValue(_interface.UnManageUntil));
      textCommand.Parameters.AddWithValue("CollectAvailability", (object) _interface.CollectAvailability);
      InterfaceBLDAL.log.DebugFormat("Inserting interface. Locking thread. NodeID: {0}, FullName: {1}", (object) _interface.NodeID, (object) _interface.FullName);
      lock (InterfaceBLDAL.insertInterfaceLock)
      {
        InterfaceBLDAL.log.DebugFormat("Inserting interface. Thread locked. NodeID: {0}, FullName: {1}", (object) _interface.NodeID, (object) _interface.FullName);
        _interface.InterfaceID = Convert.ToInt32(SqlHelper.ExecuteScalar(textCommand));
        InterfaceBLDAL.log.DebugFormat("Interface inserted with ID: {0}. NodeID: {1}, FullName: {2}", (object) _interface.InterfaceID, (object) _interface.NodeID, (object) _interface.FullName);
      }
      return _interface.ID;
    }

    private static string SqlManagedInterfaceExists
    {
      get
      {
        return "IF NOT EXISTS (SELECT * FROM Interfaces WHERE \r\n                    NodeID = @NodeID AND \r\n                    PhysicalAddress = @PhysicalAddress AND\r\n                    InterfaceName = @InterfaceName AND\r\n                    InterfaceType = @InterfaceType AND\r\n                    InterfaceSubType = @InterfaceSubType AND\r\n                    (IfName = @IfName OR @IfName = '')\r\n                    )\r\n";
      }
    }

    internal static Interfaces GetInterfaces()
    {
      // ISSUE: method pointer
      return !InterfaceBLDAL._areInterfacesAllowed ? new Interfaces() : Collection<int, Interface>.FillCollection<Interfaces>(new Collection<int, Interface>.CreateElement((object) null, __methodptr(CreateInterface)), "SELECT * FROM Interfaces", (SqlParameter[]) null);
    }

    internal static Interfaces GetNodeInterfaces(int nodeID)
    {
      if (!InterfaceBLDAL._areInterfacesAllowed)
        return new Interfaces();
      // ISSUE: method pointer
      return Collection<int, Interface>.FillCollection<Interfaces>(new Collection<int, Interface>.CreateElement((object) null, __methodptr(CreateInterface)), "SELECT * FROM Interfaces WHERE NodeID=@NodeId", new SqlParameter[1]
      {
        new SqlParameter("@NodeId", (object) nodeID)
      });
    }

    internal static Interface GetInterface(int interfaceID)
    {
      // ISSUE: method pointer
      return Collection<int, Interface>.GetCollectionItem<Interfaces>(new Collection<int, Interface>.CreateElement((object) null, __methodptr(CreateInterface)), "SELECT * FROM Interfaces WHERE InterfaceID=@InterfaceID", new SqlParameter[1]
      {
        new SqlParameter("@InterfaceID", (object) interfaceID)
      });
    }

    [Obsolete("NPM module handles deleting interfaces. Core just sends SWIS InterfaceIndication.", true)]
    internal static void DeleteInterface(int interfaceID)
    {
      InterfaceBLDAL.DeleteInterface(InterfaceBLDAL.GetInterface(interfaceID));
    }

    [Obsolete("NPM module handles deleting interfaces. Core just sends SWIS InterfaceIndication.", true)]
    internal static void DeleteInterface(Interface _interface)
    {
      SqlCommand storedProcCommand = SqlHelper.GetStoredProcCommand("swsp_DeleteInterface");
      storedProcCommand.Parameters.Add("@id", SqlDbType.Int).Value = (object) _interface.ID;
      SqlHelper.ExecuteNonQuery(storedProcCommand);
      SqlCommand textCommand;
      using (textCommand = SqlHelper.GetTextCommand("delete from Pollers where NetObject = @NetObject"))
      {
        textCommand.Parameters.Add("@NetObject", SqlDbType.VarChar, 50).Value = (object) ("I:" + (object) _interface.ID);
        SqlHelper.ExecuteNonQuery(textCommand);
      }
    }

    internal static Interface CreateInterface(IDataReader reader)
    {
      Interface @interface = new Interface();
      for (int i = 0; i < reader.FieldCount; ++i)
      {
        string name = reader.GetName(i);
        switch (name)
        {
          case "AdminStatus":
            @interface.AdminStatus = DatabaseFunctions.GetInt16(reader, i);
            break;
          case "AdminStatusLED":
            @interface.AdminStatusLED = DatabaseFunctions.GetString(reader, i);
            break;
          case "Caption":
            @interface.Caption = DatabaseFunctions.GetString(reader, i);
            break;
          case "CollectAvailability":
            @interface.CollectAvailability = reader.IsDBNull(i) || DatabaseFunctions.GetBoolean(reader, i);
            break;
          case "Counter64":
            @interface.Counter64 = DatabaseFunctions.GetString(reader, i);
            break;
          case "CustomBandwidth":
            @interface.CustomBandwidth = DatabaseFunctions.GetBoolean(reader, i);
            break;
          case "FullName":
            @interface.FullName = DatabaseFunctions.GetString(reader, i);
            break;
          case "IfName":
            @interface.IfName = DatabaseFunctions.GetString(reader, i);
            break;
          case "InBandwidth":
            @interface.InBandwidth = DatabaseFunctions.GetDouble(reader, i);
            break;
          case "InDiscardsThisHour":
            @interface.InDiscardsThisHour = DatabaseFunctions.GetFloat(reader, i);
            break;
          case "InDiscardsToday":
            @interface.InDiscardsToday = DatabaseFunctions.GetFloat(reader, i);
            break;
          case "InErrorsThisHour":
            @interface.InErrorsThisHour = DatabaseFunctions.GetFloat(reader, i);
            break;
          case "InErrorsToday":
            @interface.InErrorsToday = DatabaseFunctions.GetFloat(reader, i);
            break;
          case "InMcastPps":
            @interface.InMcastPps = DatabaseFunctions.GetFloat(reader, i);
            break;
          case "InPercentUtil":
            @interface.InPercentUtil = DatabaseFunctions.GetFloat(reader, i);
            break;
          case "InPktSize":
            @interface.InPktSize = DatabaseFunctions.GetInt16(reader, i);
            break;
          case "InPps":
            @interface.InPps = DatabaseFunctions.GetFloat(reader, i);
            break;
          case "InUcastPps":
            @interface.InUcastPps = DatabaseFunctions.GetFloat(reader, i);
            break;
          case "Inbps":
            @interface.InBps = DatabaseFunctions.GetFloat(reader, i);
            break;
          case "InterfaceAlias":
            @interface.InterfaceAlias = DatabaseFunctions.GetString(reader, i);
            break;
          case "InterfaceID":
            @interface.InterfaceID = DatabaseFunctions.GetInt32(reader, i);
            break;
          case "InterfaceIcon":
            @interface.InterfaceIcon = DatabaseFunctions.GetString(reader, i);
            break;
          case "InterfaceIndex":
            @interface.InterfaceIndex = DatabaseFunctions.GetInt32(reader, i);
            break;
          case "InterfaceLastChange":
            @interface.InterfaceLastChange = DatabaseFunctions.GetDateTime(reader, i);
            break;
          case "InterfaceMTU":
            @interface.InterfaceMTU = DatabaseFunctions.GetInt32(reader, i);
            break;
          case "InterfaceName":
            @interface.InterfaceName = DatabaseFunctions.GetString(reader, i);
            break;
          case "InterfaceSpeed":
            @interface.InterfaceSpeed = DatabaseFunctions.GetDouble(reader, i);
            break;
          case "InterfaceSubType":
            @interface.InterfaceSubType = DatabaseFunctions.GetInt32(reader, i);
            break;
          case "InterfaceType":
            @interface.InterfaceType = DatabaseFunctions.GetInt32(reader, i);
            break;
          case "InterfaceTypeDescription":
            @interface.InterfaceTypeDescription = DatabaseFunctions.GetString(reader, i);
            break;
          case "InterfaceTypeName":
            @interface.InterfaceTypeName = DatabaseFunctions.GetString(reader, i);
            break;
          case "LastSync":
            @interface.LastSync = DatabaseFunctions.GetDateTime(reader, i);
            break;
          case "MaxInBpsTime":
            @interface.MaxInBpsTime = DatabaseFunctions.GetDateTime(reader, i);
            break;
          case "MaxInBpsToday":
            @interface.MaxInBpsToday = DatabaseFunctions.GetFloat(reader, i);
            break;
          case "MaxOutBpsTime":
            @interface.MaxOutBpsTime = DatabaseFunctions.GetDateTime(reader, i);
            break;
          case "MaxOutBpsToday":
            @interface.MaxOutBpsToday = DatabaseFunctions.GetFloat(reader, i);
            break;
          case "NextPoll":
            @interface.NextPoll = DatabaseFunctions.GetDateTime(reader, i);
            break;
          case "NextRediscovery":
            @interface.NextRediscovery = DatabaseFunctions.GetDateTime(reader, i);
            break;
          case "NodeID":
            @interface.NodeID = DatabaseFunctions.GetInt32(reader, i);
            break;
          case "ObjectSubType":
            @interface.ObjectSubType = DatabaseFunctions.GetString(reader, i);
            break;
          case "OperStatus":
            @interface.OperStatus = DatabaseFunctions.GetInt16(reader, i);
            break;
          case "OperStatusLED":
            @interface.OperStatusLED = DatabaseFunctions.GetString(reader, i);
            break;
          case "OutBandwidth":
            @interface.OutBandwidth = DatabaseFunctions.GetDouble(reader, i);
            break;
          case "OutDiscardsThisHour":
            @interface.OutDiscardsThisHour = DatabaseFunctions.GetFloat(reader, i);
            break;
          case "OutDiscardsToday":
            @interface.OutDiscardsToday = DatabaseFunctions.GetFloat(reader, i);
            break;
          case "OutErrorsThisHour":
            @interface.OutErrorsThisHour = DatabaseFunctions.GetFloat(reader, i);
            break;
          case "OutErrorsToday":
            @interface.OutErrorsToday = DatabaseFunctions.GetFloat(reader, i);
            break;
          case "OutMcastPps":
            @interface.OutMcastPps = DatabaseFunctions.GetFloat(reader, i);
            break;
          case "OutPercentUtil":
            @interface.OutPercentUtil = DatabaseFunctions.GetFloat(reader, i);
            break;
          case "OutPktSize":
            @interface.OutPktSize = DatabaseFunctions.GetInt16(reader, i);
            break;
          case "OutPps":
            @interface.OutPps = DatabaseFunctions.GetFloat(reader, i);
            break;
          case "OutUcastPps":
            @interface.OutUcastPps = DatabaseFunctions.GetFloat(reader, i);
            break;
          case "Outbps":
            @interface.OutBps = DatabaseFunctions.GetFloat(reader, i);
            break;
          case "PhysicalAddress":
            @interface.PhysicalAddress = DatabaseFunctions.GetString(reader, i);
            break;
          case "PollInterval":
            @interface.PollInterval = DatabaseFunctions.GetInt32(reader, i);
            break;
          case "RediscoveryInterval":
            @interface.RediscoveryInterval = DatabaseFunctions.GetInt32(reader, i);
            break;
          case "Severity":
            @interface.Severity = DatabaseFunctions.GetInt32(reader, i);
            break;
          case "StatCollection":
            @interface.StatCollection = DatabaseFunctions.GetInt16(reader, i);
            break;
          case "Status":
            @interface.Status = DatabaseFunctions.GetString(reader, i);
            break;
          case "StatusLED":
            @interface.StatusLED = DatabaseFunctions.GetString(reader, i);
            break;
          case "UnManageFrom":
            @interface.UnManageFrom = DatabaseFunctions.GetDateTime(reader, i);
            break;
          case "UnManageUntil":
            @interface.UnManageUntil = DatabaseFunctions.GetDateTime(reader, i);
            break;
          case "UnManaged":
            @interface.UnManaged = DatabaseFunctions.GetBoolean(reader, i);
            break;
          case "UnPluggable":
            @interface.UnPluggable = DatabaseFunctions.GetBoolean(reader, i);
            break;
          default:
            if (CustomPropertyMgr.IsCustom("Interfaces", name))
            {
              @interface.CustomProperties[name] = reader[i];
              break;
            }
            InterfaceBLDAL.log.DebugFormat("Skipping Interface property {0}, value {1}", (object) name, reader[i]);
            break;
        }
      }
      return @interface;
    }
  }
}
