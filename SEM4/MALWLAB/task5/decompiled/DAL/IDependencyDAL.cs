// Decompiled with JetBrains decompiler
// Type: SolarWinds.Orion.Core.BusinessLayer.DAL.IDependencyDAL
// Assembly: SolarWinds.Orion.Core.BusinessLayer, Version=2019.4.5200.9083, Culture=neutral, PublicKeyToken=null
// MVID: E12E8C85-5CD9-4E06-8801-182E5104FADE
// Assembly location: E:\task5.dll

using SolarWinds.Orion.Core.Common.Models;
using System.Collections.Generic;

#nullable disable
namespace SolarWinds.Orion.Core.BusinessLayer.DAL
{
  public interface IDependencyDAL
  {
    IList<Dependency> GetAllDependencies();

    Dependency GetDependency(int id);

    void SaveDependency(Dependency depenedency);

    void DeleteDependency(Dependency dependency);
  }
}
