// Decompiled with JetBrains decompiler
// Type: SolarWinds.Orion.Core.BusinessLayer.SwisUriParser
// Assembly: SolarWinds.Orion.Core.BusinessLayer, Version=2019.4.5200.9083, Culture=neutral, PublicKeyToken=null
// MVID: E12E8C85-5CD9-4E06-8801-182E5104FADE
// Assembly location: E:\task5.dll

using SolarWinds.Data.Utility;
using System;
using System.Collections.Generic;
using System.Linq;

#nullable disable
namespace SolarWinds.Orion.Core.BusinessLayer
{
  internal class SwisUriParser : ISwisUriParser
  {
    public string GetEntityId(string uriStr)
    {
      SwisUri swisUri = SwisUri.Parse(uriStr);
      List<SwisUriFilter> source = new List<SwisUriFilter>()
      {
        swisUri.Filter
      };
      for (SwisUriNavigation navigation = swisUri.Navigation; navigation != null; navigation = navigation.Navigation)
        source.Add(navigation.Filter);
      if (((Dictionary<string, SwisUriFilterValue>) source.Last<SwisUriFilter>()).Values.Count > 1)
        throw new InvalidOperationException("GetEntityId does not support multiple key entities");
      return source.SelectMany<SwisUriFilter, SwisUriFilterValue>((Func<SwisUriFilter, IEnumerable<SwisUriFilterValue>>) (uriFilter => (IEnumerable<SwisUriFilterValue>) ((Dictionary<string, SwisUriFilterValue>) uriFilter).Values)).Last<SwisUriFilterValue>().Value;
    }
  }
}
