// Decompiled with JetBrains decompiler
// Type: SolarWinds.Orion.Core.BusinessLayer.DAL.INodeBLDAL
// Assembly: SolarWinds.Orion.Core.BusinessLayer, Version=2019.4.5200.9083, Culture=neutral, PublicKeyToken=null
// MVID: E12E8C85-5CD9-4E06-8801-182E5104FADE
// Assembly location: E:\task5.dll

using SolarWinds.Orion.Core.Common.Models;
using System.Collections.Generic;

#nullable disable
namespace SolarWinds.Orion.Core.BusinessLayer.DAL
{
  public interface INodeBLDAL
  {
    Node GetNode(int nodeId);

    Node GetNodeWithOptions(int nodeId, bool includeInterfaces, bool includeVolumes);

    void UpdateNode(Node node);

    Nodes GetNodes(bool includeInterfaces, bool includeVolumes);

    void UpdateNode(IDictionary<string, object> properties, int nodeId);
  }
}
