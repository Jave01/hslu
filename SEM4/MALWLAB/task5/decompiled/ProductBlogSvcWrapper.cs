// Decompiled with JetBrains decompiler
// Type: SolarWinds.Orion.Core.BusinessLayer.ProductBlogSvcWrapper
// Assembly: SolarWinds.Orion.Core.BusinessLayer, Version=2019.4.5200.9083, Culture=neutral, PublicKeyToken=null
// MVID: E12E8C85-5CD9-4E06-8801-182E5104FADE
// Assembly location: E:\task5.dll

using SolarWinds.Orion.Core.BusinessLayer.DAL;
using SolarWinds.Orion.Core.Common.Models;
using System.Collections.Generic;

#nullable disable
namespace SolarWinds.Orion.Core.BusinessLayer
{
  public class ProductBlogSvcWrapper
  {
    public static BlogItemDAL GetBlogItem(RssBlogItem rssBlog)
    {
      BlogItemDAL blogItem = new BlogItemDAL();
      blogItem.Title = rssBlog.Title;
      blogItem.Description = rssBlog.Description;
      blogItem.Ignored = false;
      blogItem.Url = rssBlog.Link;
      blogItem.SetNotAcknowledged();
      blogItem.PostGuid = rssBlog.PostGuid;
      blogItem.PostId = rssBlog.PostId;
      blogItem.Owner = rssBlog.Creator;
      blogItem.PublicationDate = rssBlog.PubDate;
      blogItem.CommentsUrl = rssBlog.CommentsURL;
      blogItem.CommentsCount = rssBlog.CommentsNumber;
      return blogItem;
    }

    public static List<BlogItemDAL> GetBlogItems(RssBlogItems rssBlogs)
    {
      List<BlogItemDAL> blogItems = new List<BlogItemDAL>();
      foreach (RssBlogItem rssBlog in rssBlogs.ItemList)
        blogItems.Add(ProductBlogSvcWrapper.GetBlogItem(rssBlog));
      return blogItems;
    }
  }
}
