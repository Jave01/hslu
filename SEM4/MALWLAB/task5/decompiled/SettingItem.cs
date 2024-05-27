// Decompiled with JetBrains decompiler
// Type: SolarWinds.Orion.Core.BusinessLayer.SettingItem
// Assembly: SolarWinds.Orion.Core.BusinessLayer, Version=2019.4.5200.9083, Culture=neutral, PublicKeyToken=null
// MVID: E12E8C85-5CD9-4E06-8801-182E5104FADE
// Assembly location: E:\task5.dll

using SolarWinds.Orion.Common;
using System.Data.SqlClient;

#nullable disable
namespace SolarWinds.Orion.Core.BusinessLayer
{
  internal class SettingItem : SynchronizeItem
  {
    public SettingItem.ColumnType Column;
    public string SettingID;

    public SettingItem(string settingID)
      : this(settingID, SettingItem.ColumnType.CurrentValue)
    {
    }

    public SettingItem(string settingID, SettingItem.ColumnType columnType)
    {
      this.SettingID = settingID;
      this.Column = columnType;
    }

    public override object GetDatabaseValue()
    {
      using (SqlCommand textCommand = SqlHelper.GetTextCommand(string.Format("SELECT {0} FROM Settings WHERE SettingID=@SettingID", (object) this.Column.ToString())))
      {
        textCommand.Parameters.AddWithValue("SettingID", (object) this.SettingID);
        return SqlHelper.ExecuteScalar(textCommand);
      }
    }

    public override string ToString()
    {
      return string.Format("{0}/{1}", (object) this.SettingID, (object) this.Column);
    }

    internal enum ColumnType
    {
      CurrentValue,
      Description,
    }
  }
}
