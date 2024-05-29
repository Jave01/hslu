// Decompiled with JetBrains decompiler
// Type: SolarWinds.Orion.Core.BusinessLayer.MaintUpdateNotifySvc.CustomerEnvironmentInfoPack
// Assembly: SolarWinds.Orion.Core.BusinessLayer, Version=2019.4.5200.9083, Culture=neutral, PublicKeyToken=null
// MVID: E12E8C85-5CD9-4E06-8801-182E5104FADE
// Assembly location: E:\task5.dll

using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

#nullable disable
namespace SolarWinds.Orion.Core.BusinessLayer.MaintUpdateNotifySvc
{
  [GeneratedCode("System.Xml", "4.8.3761.0")]
  [DebuggerStepThrough]
  [DesignerCategory("code")]
  [XmlType(Namespace = "http://www.solarwinds.com/contracts/IMaintUpdateNotifySvc/2009/09")]
  [Serializable]
  public class CustomerEnvironmentInfoPack : INotifyPropertyChanged
  {
    private ModuleInfo[] modulesField;
    private string oSVersionField;
    private string orionDBVersionField;
    private string sQLVersionField;
    private Guid customerUniqueIdField;
    private DateTime lastUpdateCheckField;

    [XmlArray(Order = 0)]
    [XmlArrayItem("Module")]
    public ModuleInfo[] Modules
    {
      get => this.modulesField;
      set
      {
        this.modulesField = value;
        this.RaisePropertyChanged(nameof (Modules));
      }
    }

    [XmlAttribute]
    public string OSVersion
    {
      get => this.oSVersionField;
      set
      {
        this.oSVersionField = value;
        this.RaisePropertyChanged(nameof (OSVersion));
      }
    }

    [XmlAttribute]
    public string OrionDBVersion
    {
      get => this.orionDBVersionField;
      set
      {
        this.orionDBVersionField = value;
        this.RaisePropertyChanged(nameof (OrionDBVersion));
      }
    }

    [XmlAttribute]
    public string SQLVersion
    {
      get => this.sQLVersionField;
      set
      {
        this.sQLVersionField = value;
        this.RaisePropertyChanged(nameof (SQLVersion));
      }
    }

    [XmlAttribute]
    public Guid CustomerUniqueId
    {
      get => this.customerUniqueIdField;
      set
      {
        this.customerUniqueIdField = value;
        this.RaisePropertyChanged(nameof (CustomerUniqueId));
      }
    }

    [XmlAttribute]
    public DateTime LastUpdateCheck
    {
      get => this.lastUpdateCheckField;
      set
      {
        this.lastUpdateCheckField = value;
        this.RaisePropertyChanged(nameof (LastUpdateCheck));
      }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    protected void RaisePropertyChanged(string propertyName)
    {
      PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
      if (propertyChanged == null)
        return;
      propertyChanged((object) this, new PropertyChangedEventArgs(propertyName));
    }
  }
}
