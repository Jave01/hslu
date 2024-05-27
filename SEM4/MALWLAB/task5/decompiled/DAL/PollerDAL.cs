// Decompiled with JetBrains decompiler
// Type: SolarWinds.Orion.Core.BusinessLayer.DAL.PollerDAL
// Assembly: SolarWinds.Orion.Core.BusinessLayer, Version=2019.4.5200.9083, Culture=neutral, PublicKeyToken=null
// MVID: E12E8C85-5CD9-4E06-8801-182E5104FADE
// Assembly location: E:\task5.dll

using SolarWinds.Orion.Common;
using SolarWinds.Orion.Core.Common.DALs;
using SolarWinds.Orion.Core.Common.Models;
using System;
using System.Data;
using System.Data.SqlClient;

#nullable disable
namespace SolarWinds.Orion.Core.BusinessLayer.DAL
{
  [Obsolete("Please start using SolarWinds.Orion.Core.Common.DALs.PollersDAL instead")]
  public class PollerDAL
  {
    private static PollersDAL pollersDAL = new PollersDAL();

    public static PollerAssignment GetPoller(int pollerID)
    {
      try
      {
        return PollerDAL.pollersDAL.GetAssignment(pollerID) ?? throw new NullReferenceException();
      }
      catch (Exception ex)
      {
        throw new ArgumentOutOfRangeException("PollerID", string.Format("Poller with Id {0} does not exist", (object) pollerID));
      }
    }

    public static int InsertPoller(PollerAssignment poller)
    {
      int num;
      PollerDAL.pollersDAL.Insert(poller, ref num);
      return num;
    }

    public static void DeletePoller(int pollerID)
    {
      PollerDAL.pollersDAL.DeletePollerByID(pollerID);
    }

    public static PollerAssignments GetPollersForNode(int nodeId)
    {
      PollerAssignments pollersForNode = new PollerAssignments();
      pollersForNode.Add(PollerDAL.pollersDAL.GetNetObjectPollers("N", nodeId, Array.Empty<string>()));
      return pollersForNode;
    }

    public static PollerAssignments GetAllPollersForNode(int nodeId, bool includeInterfacePollers)
    {
      PollerAssignments allPollersForNode = new PollerAssignments();
      string str = "SELECT PollerID, PollerType, NetObjectType, NetObjectID, Enabled FROM Pollers WHERE NetObject = @NetObject ";
      if (includeInterfacePollers)
        str += "OR NetObject IN\r\n                        (\r\n                            SELECT 'I:' + RTRIM(LTRIM(STR(InterfaceID))) FROM Interfaces WHERE NodeID=@NodeID\r\n                        )";
      using (SqlCommand textCommand = SqlHelper.GetTextCommand(str))
      {
        if (includeInterfacePollers)
          textCommand.Parameters.AddWithValue("@NodeID", (object) nodeId);
        textCommand.Parameters.Add("@NetObject", SqlDbType.VarChar, 50).Value = (object) string.Format("N:{0}", (object) nodeId);
        using (IDataReader reader = SqlHelper.ExecuteReader(textCommand))
        {
          while (reader.Read())
          {
            PollerAssignment poller = PollerDAL.CreatePoller(reader);
            ((Collection<int, PollerAssignment>) allPollersForNode).Add(poller.PollerID, poller);
          }
        }
      }
      return allPollersForNode;
    }

    public static PollerAssignments GetPollersForVolume(int volumeId)
    {
      PollerAssignments pollersForVolume = new PollerAssignments();
      pollersForVolume.Add(PollerDAL.pollersDAL.GetNetObjectPollers("V", volumeId, Array.Empty<string>()));
      return pollersForVolume;
    }

    private static PollerAssignment CreatePoller(IDataReader reader)
    {
      PollerAssignment poller = new PollerAssignment();
      for (int i = 0; i < reader.FieldCount; ++i)
      {
        string name = reader.GetName(i);
        switch (name)
        {
          case "PollerType":
            poller.PollerType = DatabaseFunctions.GetString(reader, i);
            break;
          case "NetObjectType":
            poller.NetObjectType = DatabaseFunctions.GetString(reader, i);
            break;
          case "NetObjectID":
            poller.NetObjectID = DatabaseFunctions.GetInt32(reader, name);
            break;
          case "PollerID":
            poller.PollerID = DatabaseFunctions.GetInt32(reader, i);
            break;
          case "Enabled":
            poller.Enabled = DatabaseFunctions.GetBoolean(reader, i);
            break;
          default:
            throw new ApplicationException("Couldn't create poller - unknown field.");
        }
      }
      return poller;
    }
  }
}
