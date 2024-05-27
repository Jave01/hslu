// Decompiled with JetBrains decompiler
// Type: SolarWinds.Orion.Core.BusinessLayer.MaintUpdateNotifySvc.ModuleInfo
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
  public class ModuleInfo : INotifyPropertyChanged
  {
    private string productDisplayNameField;
    private string hotfixVersionField;
    private string versionField;
    private string productTagField;
    private string licenseInfoField;

    [XmlAttribute]
    public string ProductDisplayName
    {
      get => this.productDisplayNameField;
      set
      {
        this.productDisplayNameField = value;
        this.RaisePropertyChanged(nameof (ProductDisplayName));
      }
    }

    [XmlAttribute]
    public string HotfixVersion
    {
      get => this.hotfixVersionField;
      set
      {
        this.hotfixVersionField = value;
        this.RaisePropertyChanged(nameof (HotfixVersion));
      }
    }

    [XmlAttribute]
    public string Version
    {
      get => this.versionField;
      set
      {
        this.versionField = value;
        this.RaisePropertyChanged(nameof (Version));
      }
    }

    [XmlAttribute]
    public string ProductTag
    {
      get => this.productTagField;
      set
      {
        this.productTagField = value;
        this.RaisePropertyChanged(nameof (ProductTag));
      }
    }

    [XmlAttribute]
    public string LicenseInfo
    {
      get => this.licenseInfoField;
      set
      {
        this.licenseInfoField = value;
        this.RaisePropertyChanged(nameof (LicenseInfo));
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
