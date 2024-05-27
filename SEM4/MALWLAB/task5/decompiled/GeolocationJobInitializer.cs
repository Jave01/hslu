// Decompiled with JetBrains decompiler
// Type: SolarWinds.Orion.Core.BusinessLayer.GeolocationJobInitializer
// Assembly: SolarWinds.Orion.Core.BusinessLayer, Version=2019.4.5200.9083, Culture=neutral, PublicKeyToken=null
// MVID: E12E8C85-5CD9-4E06-8801-182E5104FADE
// Assembly location: E:\task5.dll

using SolarWinds.Common.Utility;
using SolarWinds.Logging;
using SolarWinds.Orion.Core.Common;
using SolarWinds.Orion.Core.Common.DALs;
using SolarWinds.Orion.Core.Models.Actions;
using SolarWinds.Orion.Core.Models.Actions.Contexts;
using System.Threading;

#nullable disable
namespace SolarWinds.Orion.Core.BusinessLayer
{
  public class GeolocationJobInitializer
  {
    private static readonly Log log = new Log();
    public const string JobNamingPattern = "GeolocationJob-{0}";

    public static void AddActionsToScheduler(CoreBusinessLayerService service)
    {
      GeolocationActionContext geolocationContext = new GeolocationActionContext();
      string[] availableForGeolocation = WorldMapPointsDAL.GetEntitiesAvailableForGeolocation();
      int num = 1;
      foreach (string str1 in availableForGeolocation)
      {
        string currentEntity = str1;
        Scheduler.Instance.Add(new ScheduledTask(string.Format("GeolocationJob-{0}", (object) num), (TimerCallback) (o =>
        {
          string str2;
          if (!Settings.IsAutomaticGeolocationEnabled || !WebSettingsDAL.TryGet(string.Format("{0}_GeolocationField", (object) currentEntity), ref str2) || string.IsNullOrWhiteSpace(str2))
            return;
          GeolocationJobInitializer.log.Info((object) "Starting action execution");
          CoreBusinessLayerService businessLayerService = service;
          ActionDefinition actionDefinition = new ActionDefinition();
          actionDefinition.ActionTypeID = "Geolocation";
          actionDefinition.Enabled = true;
          ActionProperties actionProperties = new ActionProperties();
          actionProperties.Add("StreetAddress", str2);
          actionProperties.Add("Entity", currentEntity);
          actionProperties.Add("MapQuestApiKey", WorldMapPointsDAL.GetMapQuestKey());
          actionDefinition.Properties = actionProperties;
          GeolocationActionContext context = geolocationContext;
          businessLayerService.ExecuteAction(actionDefinition, (ActionContextBase) context);
        }), (object) null, Settings.AutomaticGeolocationCheckInterval));
        ++num;
      }
    }
  }
}
