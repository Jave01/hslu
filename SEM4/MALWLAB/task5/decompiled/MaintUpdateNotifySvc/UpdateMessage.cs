// Decompiled with JetBrains decompiler
// Type: SolarWinds.Orion.Core.BusinessLayer.MaintUpdateNotifySvc.UpdateMessage
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
  public class UpdateMessage : INotifyPropertyChanged
  {
    private DateTime publishDateField;
    private string maintenanceMessageField;

    [XmlElement(Order = 0)]
    public DateTime PublishDate
    {
      get => this.publishDateField;
      set
      {
        this.publishDateField = value;
        this.RaisePropertyChanged(nameof (PublishDate));
      }
    }

    [XmlElement(Order = 1)]
    public string MaintenanceMessage
    {
      get => this.maintenanceMessageField;
      set
      {
        this.maintenanceMessageField = value;
        this.RaisePropertyChanged(nameof (MaintenanceMessage));
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
