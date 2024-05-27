// Decompiled with JetBrains decompiler
// Type: SolarWinds.Orion.Core.Auditing.AuditDatabaseDecoratedContainer
// Assembly: SolarWinds.Orion.Core.BusinessLayer, Version=2019.4.5200.9083, Culture=neutral, PublicKeyToken=null
// MVID: E12E8C85-5CD9-4E06-8801-182E5104FADE
// Assembly location: E:\task5.dll

using SolarWinds.Orion.Core.Common.Indications;
using System;
using System.Collections.Generic;

#nullable disable
namespace SolarWinds.Orion.Core.Auditing
{
  public class AuditDatabaseDecoratedContainer : AuditDataContainer
  {
    private string accountId;
    private DateTime indicationTime;
    private string message;

    public AuditDatabaseDecoratedContainer(
      AuditDataContainer adc,
      AuditNotificationContainer anc,
      string message)
      : base(adc)
    {
      if (anc == null)
        throw new ArgumentNullException(nameof (anc));
      if (string.IsNullOrEmpty(message))
        throw new ArgumentNullException(nameof (message));
      object obj;
      this.accountId = anc.IndicationProperties == null || !((Dictionary<string, object>) anc.IndicationProperties).TryGetValue(IndicationConstants.AccountId, out obj) ? "SYSTEM" : obj as string;
      this.indicationTime = anc.GetIndicationPropertyValue<DateTime>(nameof (IndicationTime));
      this.indicationTime = this.indicationTime.Kind != DateTimeKind.Unspecified ? this.indicationTime.ToUniversalTime() : DateTime.SpecifyKind(this.indicationTime, DateTimeKind.Utc);
      this.message = message;
    }

    public string AccountId => this.accountId;

    public DateTime IndicationTime => this.indicationTime;

    public string Message => this.message;
  }
}
