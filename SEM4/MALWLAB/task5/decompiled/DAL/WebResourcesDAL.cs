// Decompiled with JetBrains decompiler
// Type: SolarWinds.Orion.Core.BusinessLayer.DAL.WebResourcesDAL
// Assembly: SolarWinds.Orion.Core.BusinessLayer, Version=2019.4.5200.9083, Culture=neutral, PublicKeyToken=null
// MVID: E12E8C85-5CD9-4E06-8801-182E5104FADE
// Assembly location: E:\task5.dll

using SolarWinds.Logging;
using SolarWinds.Orion.Common;
using SolarWinds.Orion.Core.Common.Models;
using System;
using System.Data;
using System.Data.SqlClient;

#nullable disable
namespace SolarWinds.Orion.Core.BusinessLayer.DAL
{
  public class WebResourcesDAL
  {
    private static readonly Log log = new Log();

    public static WebResources GetSpecificResources(int viewID, string queryFilterString)
    {
      // ISSUE: method pointer
      return Collection<int, WebResource>.FillCollection<WebResources>(new Collection<int, WebResource>.CreateElement((object) null, __methodptr(CreateResource)), "SELECT * FROM Resources WHERE ViewID = @viewID" + (string.IsNullOrEmpty(queryFilterString.Trim()) ? "" : " AND " + queryFilterString), new SqlParameter[1]
      {
        new SqlParameter(nameof (viewID), (object) viewID)
      });
    }

    public static void DeleteResource(int resourceId)
    {
      using (SqlCommand textCommand = SqlHelper.GetTextCommand("DELETE FROM Resources WHERE ResourceID=@id"))
      {
        textCommand.Parameters.AddWithValue("id", (object) resourceId);
        SqlHelper.ExecuteNonQuery(textCommand);
      }
      WebResourcesDAL.DeleteResourceProperties(resourceId);
    }

    public static void DeleteResourceProperties(int resourceID)
    {
      using (SqlCommand textCommand = SqlHelper.GetTextCommand("DELETE FROM ResourceProperties WHERE ResourceID=@ResourceID"))
      {
        textCommand.Parameters.AddWithValue("@ResourceID", (object) resourceID);
        SqlHelper.ExecuteNonQuery(textCommand);
      }
    }

    public static int InsertNewResource(WebResource resource, int viewID)
    {
      using (SqlCommand textCommand = SqlHelper.GetTextCommand("\r\nINSERT INTO Resources (ViewID, ViewColumn, Position, ResourceName, ResourceFile, ResourceTitle, ResourceSubTitle)\r\nSELECT @viewID, @column, ISNULL(MAX(Position),0)+1, @resourceName, @resourceFile, @title, @subtitle\r\nFROM Resources where ViewID=@viewID and ViewColumn=@column\r\nSELECT SCOPE_IDENTITY()\r\n"))
      {
        textCommand.Parameters.AddWithValue(nameof (viewID), (object) viewID);
        textCommand.Parameters.AddWithValue("column", (object) resource.Column);
        textCommand.Parameters.AddWithValue("resourceName", (object) resource.Name);
        textCommand.Parameters.AddWithValue("resourceFile", (object) resource.File);
        textCommand.Parameters.AddWithValue("title", (object) resource.Title);
        textCommand.Parameters.AddWithValue("subtitle", (object) resource.SubTitle);
        return Convert.ToInt32(SqlHelper.ExecuteScalar(textCommand));
      }
    }

    public static void InsertNewResourceProperty(
      int resourceID,
      string propertyName,
      string propertyValue)
    {
      using (SqlCommand textCommand = SqlHelper.GetTextCommand("INSERT INTO ResourceProperties (ResourceID, PropertyName, PropertyValue) VALUES (@ResourceID, @PropertyName, @PropertyValue)"))
      {
        textCommand.Parameters.AddWithValue("@ResourceID", (object) resourceID);
        textCommand.Parameters.AddWithValue("@PropertyName", (object) propertyName);
        textCommand.Parameters.AddWithValue("@PropertyValue", (object) propertyValue);
        SqlHelper.ExecuteNonQuery(textCommand);
      }
    }

    public static void UpdateResourceProperty(
      int resourceID,
      string propertyName,
      string propertyValue)
    {
      using (SqlCommand textCommand = SqlHelper.GetTextCommand("IF (EXISTS (SELECT * FROM ResourceProperties\r\n  WHERE ResourceID = @ResourceID AND PropertyName = @PropertyName))\r\nbegin\r\n  UPDATE ResourceProperties\r\n  SET PropertyValue = @PropertyValue\r\n  WHERE ResourceID = @ResourceID AND PropertyName = @PropertyName\r\nend\r\nelse\r\nbegin\r\nIF (EXISTS (SELECT * FROM ResourceProperties\r\n\tWHERE ResourceID = @ResourceID))\r\nbegin\r\n  INSERT INTO ResourceProperties (ResourceID, PropertyName, PropertyValue)\r\n  VALUES(@ResourceID, @PropertyName, @PropertyValue)\r\nend\r\nend "))
      {
        textCommand.Parameters.AddWithValue("@ResourceID", (object) resourceID);
        textCommand.Parameters.AddWithValue("@PropertyName", (object) propertyName);
        textCommand.Parameters.AddWithValue("@PropertyValue", (object) propertyValue);
        SqlHelper.ExecuteNonQuery(textCommand);
      }
    }

    public static string GetSpecificResourceProperty(int resourceID, string queryFilterString)
    {
      using (SqlCommand textCommand = SqlHelper.GetTextCommand("SELECT TOP 1 PropertyValue FROM ResourceProperties WHERE ResourceID=@ResourceID" + (string.IsNullOrEmpty(queryFilterString.Trim()) ? "" : " AND " + queryFilterString)))
      {
        textCommand.Parameters.AddWithValue("@ResourceID", (object) resourceID);
        using (IDataReader dataReader = SqlHelper.ExecuteReader(textCommand))
          return dataReader.Read() ? DatabaseFunctions.GetString(dataReader, "PropertyValue") : string.Empty;
      }
    }

    private static WebResource CreateResource(IDataReader reader)
    {
      return reader != null ? new WebResource()
      {
        Id = (int) reader["ResourceID"],
        Column = (int) (short) reader["ViewColumn"],
        Position = (int) (short) reader["Position"],
        Title = (string) reader["ResourceTitle"],
        SubTitle = (string) reader["ResourceSubTitle"],
        Name = (string) reader["ResourceName"],
        File = ((string) reader["ResourceFile"]).Trim()
      } : throw new ArgumentNullException(nameof (reader));
    }
  }
}
