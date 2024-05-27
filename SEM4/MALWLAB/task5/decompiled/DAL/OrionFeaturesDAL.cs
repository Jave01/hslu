// Decompiled with JetBrains decompiler
// Type: SolarWinds.Orion.Core.BusinessLayer.DAL.OrionFeaturesDAL
// Assembly: SolarWinds.Orion.Core.BusinessLayer, Version=2019.4.5200.9083, Culture=neutral, PublicKeyToken=null
// MVID: E12E8C85-5CD9-4E06-8801-182E5104FADE
// Assembly location: E:\task5.dll

using SolarWinds.Orion.Common;
using SolarWinds.Orion.Core.Common.DALs;
using SolarWinds.Orion.Core.Common.Data;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

#nullable disable
namespace SolarWinds.Orion.Core.BusinessLayer.DAL
{
  internal class OrionFeaturesDAL : IOrionFeaturesDAL
  {
    public IEnumerable<SolarWinds.Orion.Core.Models.OrionFeature.OrionFeature> GetItems()
    {
      return (IEnumerable<SolarWinds.Orion.Core.Models.OrionFeature.OrionFeature>) EntityHydrator.PopulateCollectionFromSql<SolarWinds.Orion.Core.Models.OrionFeature.OrionFeature>("SELECT Name, Enabled FROM OrionFeatures");
    }

    public void Update(IEnumerable<SolarWinds.Orion.Core.Models.OrionFeature.OrionFeature> features)
    {
      using (SqlConnection connection = DatabaseFunctions.CreateConnection())
      {
        using (SqlTransaction sqlTransaction = connection.BeginTransaction())
        {
          SqlHelper.ExecuteNonQuery(SqlHelper.GetTextCommand("TRUNCATE TABLE OrionFeatures"), connection, sqlTransaction);
          // ISSUE: reference to a compiler-generated field
          // ISSUE: reference to a compiler-generated field
          // ISSUE: reference to a compiler-generated field
          // ISSUE: method pointer
          // ISSUE: reference to a compiler-generated field
          // ISSUE: reference to a compiler-generated field
          // ISSUE: reference to a compiler-generated field
          // ISSUE: method pointer
          using (EnumerableDataReader<SolarWinds.Orion.Core.Models.OrionFeature.OrionFeature> enumerableDataReader = new EnumerableDataReader<SolarWinds.Orion.Core.Models.OrionFeature.OrionFeature>((PropertyAccessorBase<SolarWinds.Orion.Core.Models.OrionFeature.OrionFeature>) new SinglePropertyAccessor<SolarWinds.Orion.Core.Models.OrionFeature.OrionFeature>().AddColumn("Name", OrionFeaturesDAL.\u003C\u003Ec.\u003C\u003E9__1_0 ?? (OrionFeaturesDAL.\u003C\u003Ec.\u003C\u003E9__1_0 = new SinglePropertyAccessor<SolarWinds.Orion.Core.Models.OrionFeature.OrionFeature>.SinglePropertyAcccessorConvert((object) OrionFeaturesDAL.\u003C\u003Ec.\u003C\u003E9, __methodptr(\u003CUpdate\u003Eb__1_0)))).AddColumn("Enabled", OrionFeaturesDAL.\u003C\u003Ec.\u003C\u003E9__1_1 ?? (OrionFeaturesDAL.\u003C\u003Ec.\u003C\u003E9__1_1 = new SinglePropertyAccessor<SolarWinds.Orion.Core.Models.OrionFeature.OrionFeature>.SinglePropertyAcccessorConvert((object) OrionFeaturesDAL.\u003C\u003Ec.\u003C\u003E9, __methodptr(\u003CUpdate\u003Eb__1_1)))), features))
            SqlHelper.ExecuteBulkCopy("OrionFeatures", (IDataReader) enumerableDataReader, connection, sqlTransaction, SqlBulkCopyOptions.Default);
          sqlTransaction.Commit();
        }
      }
    }
  }
}
