// Decompiled with JetBrains decompiler
// Type: SolarWinds.Orion.Core.BusinessLayer.DAL.ElementsUsageDAL
// Assembly: SolarWinds.Orion.Core.BusinessLayer, Version=2019.4.5200.9083, Culture=neutral, PublicKeyToken=null
// MVID: E12E8C85-5CD9-4E06-8801-182E5104FADE
// Assembly location: E:\task5.dll

using SolarWinds.Orion.Common;
using SolarWinds.Orion.Core.Common.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

#nullable disable
namespace SolarWinds.Orion.Core.BusinessLayer.DAL
{
  internal class ElementsUsageDAL
  {
    public static void Save(
      IEnumerable<ElementLicenseSaturationInfo> saturationInfoCollection)
    {
      if (saturationInfoCollection == null)
        throw new ArgumentNullException(nameof (saturationInfoCollection));
      if (!saturationInfoCollection.Any<ElementLicenseSaturationInfo>())
        return;
      using (SqlConnection connection = DatabaseFunctions.CreateConnection())
      {
        using (SqlCommand textCommand = SqlHelper.GetTextCommand(" IF NOT EXISTS (SELECT 1 FROM [dbo].[ElementUsage_Daily] WHERE Date = @Date AND ElementType = @ElementType)\r\n                INSERT INTO [dbo].[ElementUsage_Daily] (Date, ElementType, Count, MaxCount) VALUES (@Date, @ElementType, @Count, @MaxCount)"))
        {
          textCommand.Parameters.Add("@Date", SqlDbType.Date);
          textCommand.Parameters.Add("@ElementType", SqlDbType.NVarChar);
          textCommand.Parameters.Add("@Count", SqlDbType.Int);
          textCommand.Parameters.Add("@MaxCount", SqlDbType.Int);
          foreach (ElementLicenseSaturationInfo saturationInfo in saturationInfoCollection)
          {
            textCommand.Parameters["@Date"].Value = (object) DateTime.UtcNow.Date;
            textCommand.Parameters["@ElementType"].Value = (object) saturationInfo.ElementType;
            textCommand.Parameters["@Count"].Value = (object) saturationInfo.Count;
            textCommand.Parameters["@MaxCount"].Value = (object) saturationInfo.MaxCount;
            SqlHelper.ExecuteNonQuery(textCommand, connection);
          }
        }
      }
    }
  }
}
