// Decompiled with JetBrains decompiler
// Type: SolarWinds.Orion.Core.BusinessLayer.ResourceLister
// Assembly: SolarWinds.Orion.Core.BusinessLayer, Version=2019.4.5200.9083, Culture=neutral, PublicKeyToken=null
// MVID: E12E8C85-5CD9-4E06-8801-182E5104FADE
// Assembly location: E:\task5.dll

using Microsoft.VisualBasic;
using SolarWinds.Logging;
using SolarWinds.Net.SNMP;
using SolarWinds.Orion.Common;
using SolarWinds.Orion.Core.Common;
using SolarWinds.Orion.Core.Common.DALs;
using SolarWinds.Orion.Core.Common.Models;
using SolarWinds.Orion.Core.Strings;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Threading;

#nullable disable
namespace SolarWinds.Orion.Core.BusinessLayer
{
  public class ResourceLister
  {
    private static Log log = new Log();
    private SolarWinds.Orion.Core.Common.Models.Node mNetworkNode;
    private CPUPollerType mCPUType;
    private string mSysObjectID;
    private CV3SessionHandle mSNMPV3SessionHandle;
    private SNMPManagerWrapper mSNMP = new SNMPManagerWrapper();
    private bool interfacesfinished;
    private bool interfacesAllowed;
    private Resources result = new Resources();
    private ListResourcesStatus status = new ListResourcesStatus();
    private Dictionary<int, string> tempVolumesFound = new Dictionary<int, string>();
    private Resource VolumeBranch = new Resource();
    private Resource InterfaceBranch = new Resource();
    private NodeInfoResource NodeInfo = new NodeInfoResource();
    private Dictionary<string, int> RequestRetries = new Dictionary<string, int>();
    private const int maxRetries = 3;
    private static Dictionary<Guid, ListResourcesStatus> mListResourcesStatuses = new Dictionary<Guid, ListResourcesStatus>();

    public bool InterfacesAllowed
    {
      set => this.interfacesAllowed = value;
    }

    public static Resources ListResources(SolarWinds.Orion.Core.Common.Models.Node node)
    {
      if (node == null)
      {
        ResourceLister.log.Error((object) "List Resources stub: ArgumentNullException, method parameter `node` is null");
        throw new ArgumentNullException();
      }
      return new ResourceLister(node).InternalListResources();
    }

    public static Guid BeginListResources(SolarWinds.Orion.Core.Common.Models.Node node, bool includeInterfaces)
    {
      ResourceLister resourceLister = new ResourceLister(node);
      resourceLister.InterfacesAllowed = includeInterfaces;
      Guid key = Guid.NewGuid();
      ResourceLister.mListResourcesStatuses[key] = resourceLister.status;
      new Thread(new ThreadStart(resourceLister.InternalListResourcesWrapper)).Start();
      ResourceLister.log.InfoFormat("BeginListResources for node {0} ({1}), operation guid={2}", (object) node.Name, (object) node.IpAddress, (object) key);
      return key;
    }

    public static Guid BeginListResources(SolarWinds.Orion.Core.Common.Models.Node node)
    {
      return ResourceLister.BeginListResources(node, true);
    }

    public static ListResourcesStatus GetListResourcesStatus(Guid listResourcesOperationId)
    {
      ListResourcesStatus listResourcesStatus = (ListResourcesStatus) null;
      if (ResourceLister.mListResourcesStatuses.ContainsKey(listResourcesOperationId))
      {
        listResourcesStatus = ResourceLister.mListResourcesStatuses[listResourcesOperationId];
        if (listResourcesStatus.IsComplete)
          ResourceLister.mListResourcesStatuses.Remove(listResourcesOperationId);
        ResourceLister.log.InfoFormat("Status check for list resources operation {0}. Interfaces={1}, Volume={2}, Complete={3}", new object[4]
        {
          (object) listResourcesOperationId,
          (object) listResourcesStatus.InterfacesDiscovered,
          (object) listResourcesStatus.VolumesDiscovered,
          (object) listResourcesStatus.IsComplete
        });
      }
      else
        ResourceLister.log.InfoFormat("Status check for list resources operation {0}. Cannot find operation in Operations Dictionary. Returning null.", (object) listResourcesOperationId);
      return listResourcesStatus;
    }

    private ResourceLister(SolarWinds.Orion.Core.Common.Models.Node node)
    {
      this.mNetworkNode = node;
    }

    private void InternalListResourcesWrapper() => this.InternalListResources();

    private Resources InternalListResources()
    {
      try
      {
        using (ResourceLister.log.Block())
        {
          if (this.mNetworkNode.SNMPVersion == null)
          {
            this.status.Resources = this.result;
            this.status.IsComplete = true;
            return this.result;
          }
          if (this.mNetworkNode.SNMPVersion == 3)
          {
            SNMPv3AuthType snmPv3AuthType = this.mNetworkNode.ReadOnlyCredentials.SNMPv3AuthType;
            SNMPAuth snmpAuth = snmPv3AuthType == 1 ? (SNMPAuth) 1 : (snmPv3AuthType == 2 ? (SNMPAuth) 2 : (SNMPAuth) 0);
            SNMPPriv snmpPriv;
            switch (this.mNetworkNode.ReadOnlyCredentials.SNMPv3PrivacyType - 1)
            {
              case 0:
                snmpPriv = (SNMPPriv) 1;
                break;
              case 1:
                snmpPriv = (SNMPPriv) 2;
                break;
              case 2:
                snmpPriv = (SNMPPriv) 3;
                break;
              case 3:
                snmpPriv = (SNMPPriv) 4;
                break;
              default:
                snmpPriv = (SNMPPriv) 0;
                break;
            }
            CV3SessionHandle cv3SessionHandle = new CV3SessionHandle();
            cv3SessionHandle.Username = this.mNetworkNode.ReadOnlyCredentials.SNMPv3UserName;
            if (this.mNetworkNode.ReadOnlyCredentials.SNMPV3AuthKeyIsPwd)
              cv3SessionHandle.AuthPassword = this.mNetworkNode.ReadOnlyCredentials.SNMPv3AuthPassword;
            else
              cv3SessionHandle.AuthKey = this.mNetworkNode.ReadOnlyCredentials.SNMPv3AuthPassword;
            cv3SessionHandle.AuthType = snmpAuth;
            cv3SessionHandle.ContextName = this.mNetworkNode.ReadOnlyCredentials.SnmpV3Context;
            cv3SessionHandle.PrivacyType = snmpPriv;
            if (this.mNetworkNode.ReadOnlyCredentials.SNMPV3PrivKeyIsPwd)
              cv3SessionHandle.PrivacyPassword = this.mNetworkNode.ReadOnlyCredentials.SNMPv3PrivacyPassword;
            else
              cv3SessionHandle.PrivacyKey = this.mNetworkNode.ReadOnlyCredentials.SNMPv3PrivacyPassword;
            this.mSNMPV3SessionHandle = cv3SessionHandle;
          }
          try
          {
            ((SNMPRequestBase) this.mSNMP.DefaultInfo).Timeout = TimeSpan.FromMilliseconds((double) (2 * Convert.ToInt32(OrionConfiguration.GetSetting("SNMP Timeout", (object) 2500))));
            ((SNMPRequestBase) this.mSNMP.DefaultInfo).Retries = 1 + Convert.ToInt32(OrionConfiguration.GetSetting("SNMP Retries", (object) 2));
          }
          catch
          {
          }
          ResourceLister.log.Debug((object) "List resources: preparing first request.");
          SNMPRequest defaultSnmpRequest = this.GetNewDefaultSNMPRequest();
          ((SNMPRequestBase) defaultSnmpRequest).SNMPVersion = (int) this.mNetworkNode.SNMPVersion;
          ((SNMPRequestBase) defaultSnmpRequest).OIDs.Add("1.3.6.1.2.1.1.2.0");
          ((SNMPRequestBase) defaultSnmpRequest).IPAddress = this.mNetworkNode.IpAddress;
          ((SNMPRequestBase) defaultSnmpRequest).TargetPort = (int) this.mNetworkNode.SNMPPort;
          if (((SNMPRequestBase) defaultSnmpRequest).SNMPVersion == 3)
            ((SNMPRequestBase) defaultSnmpRequest).SessionHandle = this.mSNMPV3SessionHandle;
          else
            ((SNMPRequestBase) defaultSnmpRequest).Community = this.mNetworkNode.ReadOnlyCredentials.CommunityString;
          SNMPResponse snmpResponse = this.mSNMP.Query(defaultSnmpRequest, true);
          if (((SNMPRequestBase) snmpResponse).ErrorNumber != 0U)
          {
            this.mSNMP.Cancel();
            this.mSNMP.Dispose();
            this.result.ErrorNumber = ((SNMPRequestBase) snmpResponse).ErrorNumber;
            this.result.ErrorMessage = string.Format("{0} {1}", (object) Resources.LIBCODE_JM0_29, (object) ((SNMPRequestBase) snmpResponse).ErrorDescription);
            ResourceLister.log.Debug((object) ("List resources: " + this.result.ErrorMessage));
            return this.result;
          }
          this.mSysObjectID = ((SNMPRequestBase) snmpResponse).OIDs[0].Value.Trim();
          if (((SNMPRequestBase) snmpResponse).SNMPVersion == 3)
            this.mSNMPV3SessionHandle = ((SNMPRequestBase) snmpResponse).SessionHandle;
          if (this.interfacesAllowed)
          {
            this.interfacesfinished = false;
            this.DiscoverInterfaces();
          }
          else
            this.interfacesfinished = true;
          if (this.DetermineWirelessSupport())
            this.AddWirelessBranch();
          this.DetermineCPULoadSupport();
          if (this.mCPUType != null)
            this.AddCPUBranch();
          if (this.DetermineVolumeUsageSupport())
          {
            this.AddVolumeBranch();
            this.DiscoverVolumes();
          }
          this.GetNodeInfo();
          ((List<Resource>) this.result).Add((Resource) this.NodeInfo);
          while (this.mSNMP.OutstandingQueries > 0 || !this.interfacesfinished)
            Thread.Sleep(100);
          while (((List<Resource>) this.InterfaceBranch.Resources).Count > 0)
          {
            ((List<Resource>) this.result).Add(((List<Resource>) this.InterfaceBranch.Resources)[0]);
            ((List<Resource>) this.InterfaceBranch.Resources).RemoveAt(0);
          }
          ((List<Resource>) this.result).Remove(this.InterfaceBranch);
          this.mSNMP.Cancel();
          this.mSNMP.Dispose();
        }
        this.status.Resources = this.result;
        this.status.IsComplete = true;
      }
      catch (Exception ex)
      {
        ResourceLister.log.Error((object) "Exception occured when listing resources.", ex);
        return new Resources() { ErrorMessage = ex.Message };
      }
      return this.result;
    }

    private void GetNodeInfo()
    {
      ((Resource) this.NodeInfo).ResourceType = (ResourceType) 5;
      using (ResourceLister.log.Block())
      {
        int err = 0;
        string ErrDes = "";
        SNMPRequest defaultSnmpRequest = this.GetNewDefaultSNMPRequest();
        ((SNMPRequestBase) defaultSnmpRequest).Community = this.mNetworkNode.ReadOnlyCredentials.CommunityString;
        ((SNMPRequestBase) defaultSnmpRequest).IPAddress = this.mNetworkNode.IpAddress;
        ((SNMPRequestBase) defaultSnmpRequest).TargetPort = (int) this.mNetworkNode.SNMPPort;
        ((SNMPRequestBase) defaultSnmpRequest).SNMPVersion = (int) this.mNetworkNode.SNMPVersion;
        if (((SNMPRequestBase) defaultSnmpRequest).SNMPVersion == 3)
          ((SNMPRequestBase) defaultSnmpRequest).SessionHandle = this.mSNMPV3SessionHandle;
        // ISSUE: method pointer
        SNMPReply.ReplyDelegate replyDelegate = new SNMPReply.ReplyDelegate((object) this, __methodptr(NodeInfoSNMPReply_Reply));
        ((SNMPRequestBase) defaultSnmpRequest).OIDs.Add("1.3.6.1.2.1.1.2.0");
        ((SNMPRequestBase) defaultSnmpRequest).OIDs.Add("1.3.6.1.2.1.1.5.0");
        ((SNMPRequestBase) defaultSnmpRequest).OIDs.Add("1.3.6.1.2.1.1.4.0");
        ((SNMPRequestBase) defaultSnmpRequest).OIDs.Add("1.3.6.1.2.1.1.6.0");
        ((SNMPRequestBase) defaultSnmpRequest).OIDs.Add("1.3.6.1.2.1.1.1.0");
        ((SNMPRequestBase) defaultSnmpRequest).SetCallbackDelegate(replyDelegate);
        this.mSNMP.BeginQuery(defaultSnmpRequest, true, out err, out ErrDes);
        ((SNMPRequestBase) defaultSnmpRequest).OIDs.Clear();
        ((SNMPRequestBase) defaultSnmpRequest).OIDs.Add("1.3.6.1.4.1.6876.1.3.0");
        this.mSNMP.BeginQuery(defaultSnmpRequest, true, out err, out ErrDes);
      }
    }

    private void NodeInfoSNMPReply_Reply(SNMPResponse Response)
    {
      using (ResourceLister.log.Block())
      {
        if (((SNMPRequestBase) Response).ErrorNumber != 0U)
          return;
        for (int index = 0; index < ((SNMPRequestBase) Response).OIDs.Count; ++index)
        {
          COID oiD = ((SNMPRequestBase) Response).OIDs[index];
          if (oiD != null && oiD.Value != null)
          {
            if (oiD.OID == "1.3.6.1.2.1.1.2.0")
            {
              this.NodeInfo.SysObjectId = oiD.Value;
              this.NodeInfo.MachineType = NodeDetailHelper.GetSWDiscoveryVendor(oiD.Value);
              this.NodeInfo.Vendor = NodeDetailHelper.GetVendorName(oiD.Value);
            }
            else if (oiD.OID == "1.3.6.1.2.1.1.1.0")
              this.NodeInfo.Description = oiD.Value;
            else if (oiD.OID == "1.3.6.1.2.1.1.6.0")
              this.NodeInfo.SysLocation = oiD.Value;
            else if (oiD.OID == "1.3.6.1.2.1.1.5.0")
              this.NodeInfo.SysName = oiD.Value;
            else if (oiD.OID == "1.3.6.1.2.1.1.4.0")
              this.NodeInfo.SysContact = oiD.Value;
            else if (oiD.Value.StartsWith("1.3.6.1.4.1.6876.60.1"))
            {
              this.NodeInfo.SysObjectId = oiD.Value;
              this.NodeInfo.Vendor = NodeDetailHelper.GetSWDiscoveryVendor(this.NodeInfo.SysObjectId);
              ((SNMPRequestBase) Response).OIDs.Clear();
              ((SNMPRequestBase) Response).OIDs.Add("1.3.6.1.4.1.6876.1.1.0");
              ((SNMPRequestBase) Response).OIDs.Add("1.3.6.1.4.1.6876.1.2.0");
              int err = 0;
              string ErrDes = string.Empty;
              this.mSNMP.BeginQuery(new SNMPRequest(Response), true, out err, out ErrDes);
            }
            else if (oiD.OID == "1.3.6.1.4.1.6876.1.1.0")
              this.NodeInfo.MachineType = oiD.Value;
            else if (oiD.OID == "1.3.6.1.4.1.6876.1.2.0")
              this.NodeInfo.IOSVersion = oiD.Value;
          }
        }
      }
    }

    private bool DetermineWirelessSupport()
    {
      bool wirelessSupport = false;
      using (ResourceLister.log.Block())
      {
        if (!File.Exists(Path.Combine(OrionConfiguration.InstallPath, "WirelessNetworks\\WirelessPollingService.exe")))
          return wirelessSupport;
        ResourceLister.log.Debug((object) "List resources: preparing wireless support request");
        SNMPRequest defaultSnmpRequest = this.GetNewDefaultSNMPRequest();
        ((SNMPRequestBase) defaultSnmpRequest).NodeID = "Wireless";
        ((SNMPRequestBase) defaultSnmpRequest).OIDs.Add("1.2.840.10036.1.1.1.1.");
        ((SNMPRequestBase) defaultSnmpRequest).Community = this.mNetworkNode.ReadOnlyCredentials.CommunityString;
        ((SNMPRequestBase) defaultSnmpRequest).IPAddress = this.mNetworkNode.IpAddress;
        ((SNMPRequestBase) defaultSnmpRequest).TargetPort = (int) this.mNetworkNode.SNMPPort;
        ((SNMPRequestBase) defaultSnmpRequest).SNMPVersion = (int) this.mNetworkNode.SNMPVersion;
        if (this.mNetworkNode.SNMPVersion == 3)
          ((SNMPRequestBase) defaultSnmpRequest).SessionHandle = this.mSNMPV3SessionHandle;
        SNMPResponse snmpResponse = this.mSNMP.Query(defaultSnmpRequest, true);
        if (((SNMPRequestBase) snmpResponse).ErrorNumber == 0U)
        {
          if (((SNMPRequestBase) snmpResponse).OIDs[0].HexValue.Length > 0)
          {
            if (((SNMPRequestBase) snmpResponse).OIDs[0].OID.Length >= 21)
            {
              if (((SNMPRequestBase) snmpResponse).OIDs[0].OID.Substring(0, 21) == "1.2.840.10036.1.1.1.1")
                wirelessSupport = true;
            }
          }
        }
      }
      return wirelessSupport;
    }

    private void DetermineCPULoadSupport()
    {
      using (ResourceLister.log.Block())
      {
        string str1 = "";
        ResourceLister.log.Debug((object) "List resources: Preparing Cpu Load support query");
        SNMPRequest defaultSnmpRequest = this.GetNewDefaultSNMPRequest();
        COID coid = new COID();
        this.mCPUType = (CPUPollerType) 0;
        ((SNMPRequestBase) defaultSnmpRequest).OIDs.Add("1.3.6.1.2.1.1.2.0");
        ((SNMPRequestBase) defaultSnmpRequest).Community = this.mNetworkNode.ReadOnlyCredentials.CommunityString;
        ((SNMPRequestBase) defaultSnmpRequest).IPAddress = this.mNetworkNode.IpAddress;
        ((SNMPRequestBase) defaultSnmpRequest).TargetPort = (int) this.mNetworkNode.SNMPPort;
        ((SNMPRequestBase) defaultSnmpRequest).SNMPVersion = (int) this.mNetworkNode.SNMPVersion;
        if (this.mNetworkNode.SNMPVersion == 3)
          ((SNMPRequestBase) defaultSnmpRequest).SessionHandle = this.mSNMPV3SessionHandle;
        SNMPResponse snmpResponse1 = this.mSNMP.Query(defaultSnmpRequest, true);
        if (((SNMPRequestBase) snmpResponse1).ErrorNumber == 0U)
        {
          COID oiD1 = ((SNMPRequestBase) snmpResponse1).OIDs[0];
          string str2 = oiD1.Value;
          if (str2.Length > 0)
          {
            ((SNMPRequestBase) defaultSnmpRequest).OIDs.Clear();
            ((SNMPRequestBase) defaultSnmpRequest).OIDs.Add("1.3.6.1.4.1.6876.1.3.0");
            SNMPResponse snmpResponse2 = this.mSNMP.Query(defaultSnmpRequest, true);
            if (((SNMPRequestBase) snmpResponse2).ErrorNumber == 0U)
              oiD1 = ((SNMPRequestBase) snmpResponse2).OIDs[0];
            if (oiD1 != null && oiD1.Value != null && oiD1.Value.StartsWith("1.3.6.1.4.1.6876.60.1"))
            {
              str1 = oiD1.Value;
              this.mCPUType = (CPUPollerType) 3;
            }
            else if (OIDHelper.IsNexusDevice(str2))
            {
              this.mCPUType = (CPUPollerType) 2;
              coid = (COID) null;
            }
            else if (str2.StartsWith("1.3.6.1.4.1.9."))
            {
              ((SNMPRequestBase) defaultSnmpRequest).OIDs.Clear();
              ((SNMPRequestBase) defaultSnmpRequest).OIDs.Add("1.3.6.1.4.1.9.9.109.1.1.1.1.4.");
              ((SNMPRequestBase) defaultSnmpRequest).QueryType = (SNMPQueryType) 1;
              SNMPResponse mSNMPResponse = this.mSNMP.Query(defaultSnmpRequest, true);
              if (((SNMPRequestBase) mSNMPResponse).ErrorNumber == 0U)
              {
                if (((SNMPRequestBase) mSNMPResponse).OIDs[0].OID.StartsWith("1.3.6.1.4.1.9.9.109.1.1.1.1.4."))
                {
                  coid = (COID) null;
                  this.mCPUType = (CPUPollerType) 2;
                }
                else
                {
                  coid = (COID) null;
                  this.DetermineCPULoadSupportTryOldCisco(defaultSnmpRequest, mSNMPResponse);
                }
                coid = (COID) null;
              }
              else
                this.DetermineCPULoadSupportTryOldCisco(defaultSnmpRequest, mSNMPResponse);
            }
            else if (str2.StartsWith("1.3.6.1.4.1.1991."))
            {
              ((SNMPRequestBase) defaultSnmpRequest).OIDs.Clear();
              ((SNMPRequestBase) defaultSnmpRequest).OIDs.Add("1.3.6.1.4.1.1991.1.1.2.1.52.0");
              if (((SNMPRequestBase) this.mSNMP.Query(defaultSnmpRequest, true)).ErrorNumber == 0U)
              {
                if (((SNMPRequestBase) defaultSnmpRequest).OIDs[0].OID.StartsWith("1.3.6.1.4.1.1991.1.1.2.1.52.0"))
                  this.mCPUType = (CPUPollerType) 2;
                coid = (COID) null;
              }
            }
            else if (str2.StartsWith("1.3.6.1.4.1.1916."))
            {
              ((SNMPRequestBase) defaultSnmpRequest).OIDs.Clear();
              ((SNMPRequestBase) defaultSnmpRequest).OIDs.Add("1.3.6.1.4.1.1916.1.1.1.28.0");
              SNMPResponse snmpResponse3 = this.mSNMP.Query(defaultSnmpRequest, true);
              if (((SNMPRequestBase) snmpResponse3).ErrorNumber == 0U)
              {
                if (((SNMPRequestBase) snmpResponse3).OIDs[0].OID.StartsWith("1.3.6.1.4.1.1916.1.1.1.28.0"))
                  this.mCPUType = (CPUPollerType) 2;
                coid = (COID) null;
              }
            }
            else if (str2.StartsWith("1.3.6.1.4.1.2272."))
            {
              ((SNMPRequestBase) defaultSnmpRequest).OIDs.Clear();
              ((SNMPRequestBase) defaultSnmpRequest).OIDs.Add("1.3.6.1.4.1.2272.1.1.20.0");
              SNMPResponse snmpResponse4 = this.mSNMP.Query(defaultSnmpRequest, true);
              if (((SNMPRequestBase) snmpResponse4).ErrorNumber == 0U)
              {
                if (((SNMPRequestBase) snmpResponse4).OIDs[0].OID.StartsWith("1.3.6.1.4.1.2272.1.1.20.0"))
                  this.mCPUType = (CPUPollerType) 2;
                coid = (COID) null;
              }
            }
            else if (str2.StartsWith("1.3.6.1.4.1.4981."))
            {
              ((SNMPRequestBase) defaultSnmpRequest).OIDs.Clear();
              ((SNMPRequestBase) defaultSnmpRequest).OIDs.Add("1.3.6.1.4.1.4981.1.20.1.1.1.8.");
              ((SNMPRequestBase) defaultSnmpRequest).QueryType = (SNMPQueryType) 1;
              SNMPResponse snmpResponse5 = this.mSNMP.Query(defaultSnmpRequest, true);
              if (((SNMPRequestBase) snmpResponse5).ErrorNumber == 0U)
              {
                if (((SNMPRequestBase) snmpResponse5).OIDs[0].OID.StartsWith("1.3.6.1.4.1.4981.1.20.1.1.1.8."))
                  this.mCPUType = (CPUPollerType) 2;
                coid = (COID) null;
              }
            }
            else if (str2.StartsWith("1.3.6.1.4.1.4998."))
            {
              ((SNMPRequestBase) defaultSnmpRequest).OIDs.Clear();
              ((SNMPRequestBase) defaultSnmpRequest).OIDs.Add("1.3.6.1.4.1.4998.1.1.5.3.1.1.1.2.");
              ((SNMPRequestBase) defaultSnmpRequest).QueryType = (SNMPQueryType) 1;
              ((SNMPRequestBase) defaultSnmpRequest).SNMPVersion = 2;
              SNMPResponse snmpResponse6 = this.mSNMP.Query(defaultSnmpRequest, true);
              if (((SNMPRequestBase) snmpResponse6).ErrorNumber == 0U)
              {
                if (((SNMPRequestBase) snmpResponse6).OIDs[0].OID.Substring(0, "1.3.6.1.4.1.4998.1.1.5.3.1.1.1.2.".Length) == "1.3.6.1.4.1.4981.1.20.1.1.1.8.")
                  this.mCPUType = (CPUPollerType) 2;
                coid = (COID) null;
              }
            }
            else if (str2.StartsWith("1.3.6.1.4.1.25506.1.") || str2.StartsWith("1.3.6.1.4.1.2011."))
            {
              this.mCPUType = (CPUPollerType) 4;
              coid = (COID) null;
            }
            else
            {
              ((SNMPRequestBase) defaultSnmpRequest).OIDs.Clear();
              ((SNMPRequestBase) defaultSnmpRequest).OIDs.Add("1.3.6.1.2.1.25.3.3.1.2.0");
              ((SNMPRequestBase) defaultSnmpRequest).QueryType = (SNMPQueryType) 1;
              SNMPResponse mSNMPResponse = this.mSNMP.Query(defaultSnmpRequest, true);
              if (((SNMPRequestBase) mSNMPResponse).ErrorNumber == 0U)
              {
                COID oiD2 = ((SNMPRequestBase) mSNMPResponse).OIDs[0];
                if (oiD2.OID.StartsWith("1.3.6.1.2.1.25.3.3.1.2") && Information.IsNumeric((object) oiD2.Value))
                {
                  coid = (COID) null;
                  this.mCPUType = (CPUPollerType) 1;
                }
                else
                {
                  coid = (COID) null;
                  this.DetermineCPULoadSupportTryNETSNMP(defaultSnmpRequest, mSNMPResponse);
                }
                coid = (COID) null;
              }
              else
                this.DetermineCPULoadSupportTryNETSNMP(defaultSnmpRequest, mSNMPResponse);
            }
          }
        }
      }
    }

    private void DetermineCPULoadSupportTryOldCisco(
      SNMPRequest CPUSNMPRequest,
      SNMPResponse mSNMPResponse)
    {
      ((SNMPRequestBase) CPUSNMPRequest).OIDs.Clear();
      ((SNMPRequestBase) CPUSNMPRequest).OIDs.Add("1.3.6.1.4.1.9.2.1.57.0");
      ((SNMPRequestBase) CPUSNMPRequest).QueryType = (SNMPQueryType) 0;
      mSNMPResponse = this.mSNMP.Query(CPUSNMPRequest, true);
      if (((SNMPRequestBase) mSNMPResponse).ErrorNumber != 0U || !((SNMPRequestBase) mSNMPResponse).OIDs[0].OID.StartsWith("1.3.6.1.4.1.9.2.1.57.0"))
        return;
      this.mCPUType = (CPUPollerType) 2;
    }

    private void DetermineCPULoadSupportTryNETSNMP(
      SNMPRequest CPUSNMPRequest,
      SNMPResponse mSNMPResponse)
    {
      ((SNMPRequestBase) CPUSNMPRequest).OIDs.Clear();
      ((SNMPRequestBase) CPUSNMPRequest).OIDs.Add("1.3.6.1.4.1.2021.11.11.0");
      ((SNMPRequestBase) CPUSNMPRequest).QueryType = (SNMPQueryType) 0;
      mSNMPResponse = this.mSNMP.Query(CPUSNMPRequest, true);
      if (((SNMPRequestBase) mSNMPResponse).ErrorNumber == 0U)
      {
        COID oiD = ((SNMPRequestBase) mSNMPResponse).OIDs[0];
        if (oiD.OID.StartsWith("1.3.6.1.4.1.2021.11.11.0") && Information.IsNumeric((object) oiD.Value))
          this.mCPUType = (CPUPollerType) 1;
        else
          this.DetermineCPULoadSupportTryNewNETSNMP(CPUSNMPRequest);
      }
      else
        this.DetermineCPULoadSupportTryNewNETSNMP(CPUSNMPRequest);
    }

    private void DetermineCPULoadSupportTryNewNETSNMP(SNMPRequest lCPUSNMPReuest)
    {
      ((SNMPRequestBase) lCPUSNMPReuest).OIDs.Clear();
      ((SNMPRequestBase) lCPUSNMPReuest).OIDs.Add("1.3.6.1.4.1.2021.11.53.0");
      ((SNMPRequestBase) lCPUSNMPReuest).QueryType = (SNMPQueryType) 0;
      SNMPResponse snmpResponse = this.mSNMP.Query(lCPUSNMPReuest, true);
      if (((SNMPRequestBase) snmpResponse).ErrorNumber != 0U)
        return;
      COID oiD = ((SNMPRequestBase) snmpResponse).OIDs[0];
      if (oiD.OID.StartsWith("1.3.6.1.4.1.2021.11.53.0") && Information.IsNumeric((object) oiD.Value))
        this.mCPUType = (CPUPollerType) 1;
    }

    private void AddCPUBranch()
    {
      ResourceLister.log.Debug((object) "List resources: Adding `Cpu and Memory` resource");
      Resource resource = new Resource();
      resource.Name = "CPU and Memory Utilization";
      if (this.mCPUType == 3)
        resource.Name += " for VMWare ESX";
      resource.Data = -1;
      if (this.mCPUType == 2)
        resource.DataVariant = "Poller_CR";
      else if (this.mCPUType == 1)
        resource.DataVariant = "Poller_HT";
      else if (this.mCPUType == 3)
        resource.DataVariant = "Poller_VX";
      else if (this.mCPUType == 4)
        resource.DataVariant = "Poller_H3C";
      ((List<Resource>) this.result).Add(resource);
    }

    private void AddWirelessBranch()
    {
      ResourceLister.log.Debug((object) "List resources: Adding `Wireless` resource");
      Resource resource = new Resource();
      SqlCommand sqlCommand = new SqlCommand("Select WirelessAP From Nodes Where NodeID=" + this.mNetworkNode.ID.ToString());
      IDataReader dataReader;
      try
      {
        dataReader = SqlHelper.ExecuteReader(sqlCommand);
      }
      catch
      {
        return;
      }
      resource.Name = "Wireless Network Performance Monitoring";
      resource.Data = -1;
      resource.DataVariant = "Wireless";
      if (dataReader != null && !dataReader.IsClosed)
        dataReader.Close();
      ((List<Resource>) this.result).Add(resource);
    }

    private bool DetermineVolumeUsageSupport()
    {
      bool volumeUsageSupport = false;
      using (ResourceLister.log.Block())
      {
        ResourceLister.log.Debug((object) "List resources: Preparing Volume Usage support request");
        SNMPRequest defaultSnmpRequest = this.GetNewDefaultSNMPRequest();
        COID coid = new COID();
        ((SNMPRequestBase) defaultSnmpRequest).OIDs.Add("1.3.6.1.2.1.25.2.3.1.1");
        ((SNMPRequestBase) defaultSnmpRequest).QueryType = (SNMPQueryType) 1;
        ((SNMPRequestBase) defaultSnmpRequest).Community = this.mNetworkNode.ReadOnlyCredentials.CommunityString;
        ((SNMPRequestBase) defaultSnmpRequest).IPAddress = this.mNetworkNode.IpAddress;
        ((SNMPRequestBase) defaultSnmpRequest).TargetPort = (int) this.mNetworkNode.SNMPPort;
        ((SNMPRequestBase) defaultSnmpRequest).SNMPVersion = (int) this.mNetworkNode.SNMPVersion;
        if (this.mNetworkNode.SNMPVersion == 3)
          ((SNMPRequestBase) defaultSnmpRequest).SessionHandle = this.mSNMPV3SessionHandle;
        SNMPResponse snmpResponse = this.mSNMP.Query(defaultSnmpRequest, true);
        if (((SNMPRequestBase) snmpResponse).ErrorNumber == 0U)
        {
          if (((SNMPRequestBase) snmpResponse).OIDs[0].OID.StartsWith("1.3.6.1.2.1.25.2.3.1.1."))
            volumeUsageSupport = true;
        }
        else
          volumeUsageSupport = false;
      }
      return volumeUsageSupport;
    }

    private void AddVolumeBranch()
    {
      ResourceLister.log.Debug((object) "List resources: Adding `Volumes` resource group");
      this.VolumeBranch.Name = "Volume Utilization";
      this.VolumeBranch.Data = -1;
      this.VolumeBranch.DataVariant = "Volumes";
      this.VolumeBranch.ResourceType = (ResourceType) 1;
      ((List<Resource>) this.result).Add(this.VolumeBranch);
    }

    public void DiscoverVolumes()
    {
      ResourceLister.log.Debug((object) "List resources: Discovering volumes resources");
      SNMPRequest defaultSnmpRequest = this.GetNewDefaultSNMPRequest();
      ((SNMPRequestBase) defaultSnmpRequest).Community = this.mNetworkNode.ReadOnlyCredentials.CommunityString;
      ((SNMPRequestBase) defaultSnmpRequest).IPAddress = this.mNetworkNode.IpAddress;
      ((SNMPRequestBase) defaultSnmpRequest).TargetPort = (int) this.mNetworkNode.SNMPPort;
      ((SNMPRequestBase) defaultSnmpRequest).SNMPVersion = (int) this.mNetworkNode.SNMPVersion;
      if (this.mNetworkNode.SNMPVersion == 3)
        ((SNMPRequestBase) defaultSnmpRequest).SessionHandle = this.mSNMPV3SessionHandle;
      ((SNMPRequestBase) defaultSnmpRequest).QueryType = (SNMPQueryType) 1;
      ((SNMPRequestBase) defaultSnmpRequest).OIDs.Add("1.3.6.1.2.1.25.2.3.1.2");
      // ISSUE: method pointer
      SNMPReply.ReplyDelegate replyDelegate = new SNMPReply.ReplyDelegate((object) this, __methodptr(VolumesSNMPReply_Reply));
      ((SNMPRequestBase) defaultSnmpRequest).SetCallbackDelegate(replyDelegate);
      int err = 0;
      string ErrDes = "";
      this.mSNMP.BeginQuery(defaultSnmpRequest, true, out err, out ErrDes);
    }

    private void VolumesSNMPReply_Reply(SNMPResponse Response)
    {
      COID coid = new COID();
      if (((SNMPRequestBase) Response).ErrorNumber != 0U)
        return;
      COID oiD = ((SNMPRequestBase) Response).OIDs[0];
      if (oiD.OID.StartsWith("1.3.6.1.2.1.25.2.3.1.2."))
      {
        int int32 = Convert.ToInt32(oiD.OID.Substring("1.3.6.1.2.1.25.2.3.1.2".Length + 1, oiD.OID.Length - "1.3.6.1.2.1.25.2.3.1.2".Length - 1));
        switch (oiD.Value)
        {
          case "1.3.6.1.2.1.25.2.1.1":
            this.tempVolumesFound.Add(int32, "Other");
            break;
          case "1.3.6.1.2.1.25.2.1.10":
            this.tempVolumesFound.Add(int32, "NetworkDisk");
            break;
          case "1.3.6.1.2.1.25.2.1.2":
          case "1.3.6.1.4.1.23.2.27.2.1.3":
          case "1.3.6.1.4.1.23.2.27.2.1.4":
          case "1.3.6.1.4.1.23.2.27.2.1.5":
          case "1.3.6.1.4.1.23.2.27.2.1.6":
          case "1.3.6.1.4.1.23.2.27.2.1.7":
          case "1.3.6.1.4.1.23.2.27.2.1.8":
            this.tempVolumesFound.Add(int32, "RAM");
            break;
          case "1.3.6.1.2.1.25.2.1.3":
            this.tempVolumesFound.Add(int32, "VirtualMemory");
            break;
          case "1.3.6.1.2.1.25.2.1.4":
          case "1.3.6.1.4.1.23.2.27.2.1.1":
            this.tempVolumesFound.Add(int32, "FixedDisk");
            break;
          case "1.3.6.1.2.1.25.2.1.5":
            this.tempVolumesFound.Add(int32, "RemovableDisk");
            break;
          case "1.3.6.1.2.1.25.2.1.6":
            this.tempVolumesFound.Add(int32, "FloppyDisk");
            break;
          case "1.3.6.1.2.1.25.2.1.7":
            this.tempVolumesFound.Add(int32, "CompactDisk");
            break;
          case "1.3.6.1.2.1.25.2.1.8":
            this.tempVolumesFound.Add(int32, "RAMDisk");
            break;
          case "1.3.6.1.2.1.25.2.1.9":
            this.tempVolumesFound.Add(int32, "FlashMemory");
            break;
          default:
            this.tempVolumesFound.Add(int32, "FixedDisk");
            break;
        }
        SNMPRequest snR = new SNMPRequest(Response);
        ((SNMPRequestBase) snR).OIDs.Clear();
        ((SNMPRequestBase) snR).OIDs.Add(oiD.OID);
        int err = 0;
        string ErrDes = "";
        this.mSNMP.BeginQuery(snR, true, out err, out ErrDes);
      }
      else
      {
        if (!oiD.OID.StartsWith("1.3.6.1.2.1.25.2.3.1.3."))
          return;
        int int32 = Convert.ToInt32(oiD.OID.Substring("1.3.6.1.2.1.25.2.3.1.3".Length + 1, oiD.OID.Length - "1.3.6.1.2.1.25.2.3.1.3".Length - 1));
        string str = oiD.Value;
        ((SNMPRequestBase) Response).OIDs.Clear();
        ((SNMPRequestBase) Response).OIDs.Add(oiD.OID);
        int err = 0;
        string ErrDes = "";
        this.mSNMP.BeginQuery(new SNMPRequest(Response), true, out err, out ErrDes);
        if (this.tempVolumesFound.ContainsKey(int32))
        {
          ResourceLister.log.Debug((object) "List resources: Volume resource founded");
          ((List<Resource>) this.VolumeBranch.Resources).Add(new Resource()
          {
            Data = int32,
            DataVariant = "Poller_VO",
            ResourceType = (ResourceType) 1,
            Name = str,
            SubType = this.tempVolumesFound[int32]
          });
          ++this.status.VolumesDiscovered;
        }
        else
        {
          ResourceLister.log.Debug((object) "List resources: Volume resource founded");
          ((List<Resource>) this.VolumeBranch.Resources).Add(new Resource()
          {
            Data = int32,
            DataVariant = "Poller_VO",
            ResourceType = (ResourceType) 1,
            Name = str,
            SubType = "FixedDisk"
          });
          ++this.status.VolumesDiscovered;
        }
      }
    }

    private void DiscoverInterfaces()
    {
      using (ResourceLister.log.Block())
      {
        this.InterfaceBranch.Name = "";
        this.InterfaceBranch.Data = -1;
        this.InterfaceBranch.DataVariant = "Interfaces";
        this.InterfaceBranch.ResourceType = (ResourceType) 3;
        ((List<Resource>) this.result).Add(this.InterfaceBranch);
        SNMPRequest defaultSnmpRequest = this.GetNewDefaultSNMPRequest();
        ((SNMPRequestBase) defaultSnmpRequest).IPAddress = this.mNetworkNode.IpAddress;
        ((SNMPRequestBase) defaultSnmpRequest).Community = this.mNetworkNode.ReadOnlyCredentials.CommunityString;
        ((SNMPRequestBase) defaultSnmpRequest).SNMPVersion = (int) this.mNetworkNode.SNMPVersion;
        ((SNMPRequestBase) defaultSnmpRequest).TargetPort = (int) this.mNetworkNode.SNMPPort;
        if (this.mNetworkNode.SNMPVersion == 3)
          ((SNMPRequestBase) defaultSnmpRequest).SessionHandle = this.mSNMPV3SessionHandle;
        ((SNMPRequestBase) defaultSnmpRequest).QueryType = (SNMPQueryType) 4;
        ((SNMPRequestBase) defaultSnmpRequest).MaxReps = 50;
        ((SNMPRequestBase) defaultSnmpRequest).OIDs.Add("1.3.6.1.2.1.2.2.1.2");
        // ISSUE: method pointer
        SNMPReply.ReplyDelegate replyDelegate = new SNMPReply.ReplyDelegate((object) this, __methodptr(InterfacesSNMPReply_Reply));
        ((SNMPRequestBase) defaultSnmpRequest).SetCallbackDelegate(replyDelegate);
        int err = 0;
        string ErrDes = "";
        this.mSNMP.BeginQuery(defaultSnmpRequest, true, out err, out ErrDes);
      }
    }

    private void InterfacesSNMPReply_Reply(SNMPResponse Response)
    {
      COID coid = new COID();
      if (((SNMPRequestBase) Response).ErrorNumber == 0U)
      {
        for (int index = 0; index < ((SNMPRequestBase) Response).OIDs.Count; ++index)
        {
          coid = ((SNMPRequestBase) Response).OIDs[index];
          if (coid.OID.StartsWith("1.3.6.1.2.1.2.2.1.2."))
          {
            coid.Value = coid.Value.Trim();
            int int32 = Convert.ToInt32(coid.OID.Substring("1.3.6.1.2.1.2.2.1.2".Length + 1, coid.OID.Length - "1.3.6.1.2.1.2.2.1.2".Length - 1));
            ResourceLister.log.DebugFormat("List resources: Interface resource founded {0}", (object) int32);
            ResourceInterface resourceInterface = new ResourceInterface();
            ((Resource) resourceInterface).ResourceType = (ResourceType) 3;
            ((Resource) resourceInterface).Data = int32;
            resourceInterface.ifDescr = coid.Value.Replace("\0", "");
            ((List<Resource>) this.InterfaceBranch.Resources).Add((Resource) resourceInterface);
            this.InterfaceDiscover((long) int32);
          }
          else
            this.interfacesfinished = true;
        }
        if (this.interfacesfinished)
          return;
        ((SNMPRequestBase) Response).OIDs.Clear();
        ((SNMPRequestBase) Response).OIDs.Add(coid.OID);
        int err = 0;
        string ErrDes = "";
        SNMPRequest snR = new SNMPRequest(Response);
        ((SNMPRequestBase) snR).MaxReps = 50;
        ((SNMPRequestBase) snR).QueryType = (SNMPQueryType) 4;
        this.mSNMP.BeginQuery(snR, true, out err, out ErrDes);
      }
      else
      {
        ResourceLister.log.WarnFormat("Error encountered while listing resources: {0}", (object) ((SNMPRequestBase) Response).ErrorNumber);
        if (((SNMPRequestBase) Response).OIDs.Count > 0)
        {
          lock (this.RequestRetries)
          {
            if (this.RequestRetries.ContainsKey(((SNMPRequestBase) Response).OIDs[0].OID))
              this.RequestRetries[((SNMPRequestBase) Response).OIDs[0].OID]++;
            else
              this.RequestRetries[((SNMPRequestBase) Response).OIDs[0].OID] = 1;
            if (this.RequestRetries[((SNMPRequestBase) Response).OIDs[0].OID] < 3)
            {
              ResourceLister.log.WarnFormat("Retrying Request {0}", (object) ((SNMPRequestBase) Response).OIDs[0].OID);
              int err = 0;
              string ErrDes = "";
              SNMPRequest defaultSnmpRequest = this.GetNewDefaultSNMPRequest();
              ((SNMPRequestBase) defaultSnmpRequest).IPAddress = this.mNetworkNode.IpAddress;
              ((SNMPRequestBase) defaultSnmpRequest).Community = this.mNetworkNode.ReadOnlyCredentials.CommunityString;
              ((SNMPRequestBase) defaultSnmpRequest).SNMPVersion = (int) this.mNetworkNode.SNMPVersion;
              ((SNMPRequestBase) defaultSnmpRequest).TargetPort = (int) this.mNetworkNode.SNMPPort;
              if (this.mNetworkNode.SNMPVersion == 3)
                ((SNMPRequestBase) defaultSnmpRequest).SessionHandle = this.mSNMPV3SessionHandle;
              ((SNMPRequestBase) defaultSnmpRequest).QueryType = (SNMPQueryType) 1;
              ((SNMPRequestBase) defaultSnmpRequest).OIDs.Add(((SNMPRequestBase) Response).OIDs[0].OID);
              // ISSUE: method pointer
              SNMPReply.ReplyDelegate replyDelegate = new SNMPReply.ReplyDelegate((object) this, __methodptr(InterfacesSNMPReply_Reply));
              ((SNMPRequestBase) defaultSnmpRequest).SetCallbackDelegate(replyDelegate);
              this.mSNMP.BeginQuery(defaultSnmpRequest, true, out err, out ErrDes);
            }
            else
            {
              ResourceLister.log.WarnFormat("Cannot get response after several retries. IP={0}, OID={1}", (object) this.mNetworkNode.IpAddress, (object) ((SNMPRequestBase) Response).OIDs[0].OID);
              this.interfacesfinished = true;
            }
          }
        }
        else
          this.interfacesfinished = true;
      }
    }

    private SNMPRequest GetNewDefaultSNMPRequest()
    {
      SNMPRequest defaultSnmpRequest = new SNMPRequest();
      try
      {
        ((SNMPRequestBase) defaultSnmpRequest).Timeout = TimeSpan.FromMilliseconds((double) (2 * Convert.ToInt32(OrionConfiguration.GetSetting("SNMP Timeout", (object) 2500))));
        ((SNMPRequestBase) defaultSnmpRequest).Retries = 1 + Convert.ToInt32(OrionConfiguration.GetSetting("SNMP Retries", (object) 2));
      }
      catch
      {
      }
      return defaultSnmpRequest;
    }

    private SNMPRequest GetNewInterfaceSNMPRequest()
    {
      SNMPRequest defaultSnmpRequest = this.GetNewDefaultSNMPRequest();
      ((SNMPRequestBase) defaultSnmpRequest).Community = this.mNetworkNode.ReadOnlyCredentials.CommunityString;
      ((SNMPRequestBase) defaultSnmpRequest).IPAddress = this.mNetworkNode.IpAddress;
      ((SNMPRequestBase) defaultSnmpRequest).TargetPort = (int) this.mNetworkNode.SNMPPort;
      ((SNMPRequestBase) defaultSnmpRequest).SNMPVersion = (int) this.mNetworkNode.SNMPVersion;
      ((SNMPRequestBase) defaultSnmpRequest).QueryType = (SNMPQueryType) 0;
      if (((SNMPRequestBase) defaultSnmpRequest).SNMPVersion == 3)
        ((SNMPRequestBase) defaultSnmpRequest).SessionHandle = this.mSNMPV3SessionHandle;
      return defaultSnmpRequest;
    }

    private void InterfaceDiscover(long Index)
    {
      using (ResourceLister.log.Block())
      {
        int err = 0;
        string ErrDes = "";
        SNMPRequest interfaceSnmpRequest1 = this.GetNewInterfaceSNMPRequest();
        ((SNMPRequestBase) interfaceSnmpRequest1).OIDs.Add("1.3.6.1.2.1.31.1.1.1.18." + (object) Index);
        // ISSUE: method pointer
        SNMPReply.ReplyDelegate replyDelegate = new SNMPReply.ReplyDelegate((object) this, __methodptr(InterfaceSNMPReply_Reply));
        ((SNMPRequestBase) interfaceSnmpRequest1).SetCallbackDelegate(replyDelegate);
        this.mSNMP.BeginQuery(interfaceSnmpRequest1, true, out err, out ErrDes);
        SNMPRequest interfaceSnmpRequest2 = this.GetNewInterfaceSNMPRequest();
        ((SNMPRequestBase) interfaceSnmpRequest2).OIDs.Add("1.3.6.1.2.1.31.1.1.1.1." + (object) Index);
        ((SNMPRequestBase) interfaceSnmpRequest2).OIDs.Add("1.3.6.1.2.1.2.2.1.8." + (object) Index);
        ((SNMPRequestBase) interfaceSnmpRequest2).OIDs.Add("1.3.6.1.2.1.2.2.1.3." + (object) Index);
        ((SNMPRequestBase) interfaceSnmpRequest2).OIDs.Add("1.3.6.1.2.1.2.2.1.4." + (object) Index);
        ((SNMPRequestBase) interfaceSnmpRequest2).OIDs.Add("1.3.6.1.2.1.2.2.1.6." + (object) Index);
        ((SNMPRequestBase) interfaceSnmpRequest2).OIDs.Add("1.3.6.1.2.1.3.1.1.2." + (object) Index);
        ((SNMPRequestBase) interfaceSnmpRequest2).OIDs.Add("1.3.6.1.2.1.2.2.1.5." + (object) Index);
        ((SNMPRequestBase) interfaceSnmpRequest2).OIDs.Add("1.3.6.1.2.1.2.2.1.10." + (object) Index);
        ((SNMPRequestBase) interfaceSnmpRequest2).OIDs.Add("1.3.6.1.2.1.2.2.1.14." + (object) Index);
        ((SNMPRequestBase) interfaceSnmpRequest2).SetCallbackDelegate(replyDelegate);
        this.mSNMP.BeginQuery(interfaceSnmpRequest2, true, out err, out ErrDes);
        ++this.status.InterfacesDiscovered;
        ResourceLister.log.Debug((object) "List resources: Discovering interface - Queries are sended. Waiting for replies.");
      }
    }

    private ResourceInterface GetInterfaceByIndex(int i)
    {
      foreach (ResourceInterface resource in (List<Resource>) this.InterfaceBranch.Resources)
      {
        if (((Resource) resource).Data == i)
          return resource;
      }
      return (ResourceInterface) null;
    }

    private bool isResourceExist(ResourceInterface _interface, string DataVariant)
    {
      foreach (Resource resource in (List<Resource>) ((Resource) _interface).Resources)
      {
        if (resource.DataVariant == DataVariant)
          return true;
      }
      return false;
    }

    private void InterfaceSNMPReply_Reply(SNMPResponse Response)
    {
      using (ResourceLister.log.Block())
      {
        COID coid = new COID();
        if (((SNMPRequestBase) Response).ErrorNumber == 0U)
        {
          foreach (COID oiD in ((SNMPRequestBase) Response).OIDs)
          {
            if (oiD != null && oiD.Value != null)
            {
              int int32 = Convert.ToInt32(oiD.OID.Substring(oiD.OID.LastIndexOf(".") + 1));
              ResourceInterface interfaceByIndex = this.GetInterfaceByIndex(int32);
              ResourceInterface userObject = ((SNMPRequestBase) Response).UserObject as ResourceInterface;
              string oid = oiD.OID;
              if (oid == "1.3.6.1.2.1.31.1.1.1.18." + (object) int32)
              {
                if (string.IsNullOrEmpty(interfaceByIndex.ifAlias))
                  interfaceByIndex.ifAlias = oiD.Value;
              }
              else if (oid == "1.3.6.1.2.1.31.1.1.1.1." + (object) int32)
              {
                interfaceByIndex.ifName = oiD.Value;
                if (this.NodeInfo.Description.Contains("Cisco Catalyst Operating System") && interfaceByIndex.ifName.Contains("/"))
                {
                  string[] strArray = interfaceByIndex.ifName.Split('/');
                  int result1;
                  int result2;
                  if (strArray.Length == 2 && int.TryParse(strArray[0], out result1) && int.TryParse(strArray[1], out result2))
                  {
                    SNMPRequest interfaceSnmpRequest = this.GetNewInterfaceSNMPRequest();
                    ((SNMPRequestBase) interfaceSnmpRequest).OIDs.Add("1.3.6.1.4.1.9.5.1.4.1.1.4." + (object) result1 + "." + (object) result2);
                    // ISSUE: method pointer
                    ((SNMPRequestBase) interfaceSnmpRequest).SetCallbackDelegate(new SNMPReply.ReplyDelegate((object) this, __methodptr(InterfaceSNMPReply_Reply)));
                    ((SNMPRequestBase) interfaceSnmpRequest).UserObject = (object) interfaceByIndex;
                    this.mSNMP.BeginQuery(interfaceSnmpRequest, true, out int _, out string _);
                  }
                }
              }
              else if (userObject != null && userObject.ifName != null && oid == "1.3.6.1.4.1.9.5.1.4.1.1.4." + userObject.ifName.Replace('/', '.'))
                userObject.ifAlias = oiD.Value;
              else if (oid == "1.3.6.1.2.1.2.2.1.8." + (object) int32)
                interfaceByIndex.ifOperStatus = Convert.ToInt32(oiD.Value);
              else if (oiD.OID.StartsWith("1.3.6.1.2.1.2.2.1.14"))
              {
                if (oiD.ValueType != 129 && oiD.ValueType != 128 && !this.isResourceExist(interfaceByIndex, "Poller_IE"))
                  ((List<Resource>) ((Resource) interfaceByIndex).Resources).Add(new Resource()
                  {
                    ResourceType = (ResourceType) 4,
                    Data = -1,
                    DataVariant = "Poller_IE",
                    Name = "Interface Error Statistics"
                  });
              }
              else if (oiD.OID.StartsWith("1.3.6.1.2.1.2.2.1.10"))
              {
                if (oiD.ValueType != 129 && oiD.ValueType != 128 && !this.isResourceExist(interfaceByIndex, "Poller_IT"))
                  ((List<Resource>) ((Resource) interfaceByIndex).Resources).Add(new Resource()
                  {
                    ResourceType = (ResourceType) 4,
                    Data = -1,
                    DataVariant = "Poller_IT",
                    Name = "Interface Traffic Statistics"
                  });
              }
              else if (oid == "1.3.6.1.2.1.2.2.1.3." + (object) int32)
              {
                interfaceByIndex.ifType = Convert.ToInt32(oiD.Value);
                interfaceByIndex.ifTypeName = DiscoveryDatabaseDAL.GetInterfaceTypeName(interfaceByIndex.ifType);
                interfaceByIndex.ifTypeDescription = DiscoveryDatabaseDAL.GetInterfaceTypeDescription(interfaceByIndex.ifType);
              }
              else if (oid == "1.3.6.1.2.1.2.2.1.6." + (object) int32 || oid == "1.3.6.1.2.1.3.1.1.2." + (object) int32)
              {
                if (!string.IsNullOrEmpty(oiD.Value))
                  interfaceByIndex.ifMACAddress = oiD.Value;
                if (string.IsNullOrEmpty(interfaceByIndex.ifMACAddress))
                  interfaceByIndex.ifMACAddress = oiD.HexValue;
              }
              else if (oid == "1.3.6.1.2.1.2.2.1.4." + (object) int32)
              {
                int result = 0;
                int.TryParse(oiD.Value, out result);
                interfaceByIndex.ifMTU = result;
              }
              else if (oid == "1.3.6.1.2.1.2.2.1.5." + (object) int32)
                interfaceByIndex.ifSpeed = oiD.Value;
            }
          }
        }
        else
        {
          if (((SNMPRequestBase) Response).ErrorNumber != 31040U)
            return;
          ResourceLister.log.Warn((object) "Timeout while processing interface details.");
          if (((SNMPRequestBase) Response).OIDs.Count <= 0)
            return;
          lock (this.RequestRetries)
          {
            if (this.RequestRetries.ContainsKey(((SNMPRequestBase) Response).OIDs[0].OID))
              this.RequestRetries[((SNMPRequestBase) Response).OIDs[0].OID]++;
            else
              this.RequestRetries[((SNMPRequestBase) Response).OIDs[0].OID] = 1;
            if (this.RequestRetries[((SNMPRequestBase) Response).OIDs[0].OID] < 3)
            {
              ResourceLister.log.WarnFormat("Retrying Request {0}", (object) ((SNMPRequestBase) Response).OIDs[0].OID);
              int err = 0;
              string ErrDes = "";
              SNMPRequest interfaceSnmpRequest = this.GetNewInterfaceSNMPRequest();
              ((SNMPRequestBase) interfaceSnmpRequest).OIDs.Add(((SNMPRequestBase) Response).OIDs[0].OID);
              // ISSUE: method pointer
              SNMPReply.ReplyDelegate replyDelegate = new SNMPReply.ReplyDelegate((object) this, __methodptr(InterfaceSNMPReply_Reply));
              ((SNMPRequestBase) interfaceSnmpRequest).SetCallbackDelegate(replyDelegate);
              this.mSNMP.BeginQuery(interfaceSnmpRequest, true, out err, out ErrDes);
            }
            else
              ResourceLister.log.WarnFormat("Cannot get response after several retries. IP={0}, OID={1}", (object) this.mNetworkNode.IpAddress, (object) ((SNMPRequestBase) Response).OIDs[0].OID);
          }
        }
      }
    }
  }
}
