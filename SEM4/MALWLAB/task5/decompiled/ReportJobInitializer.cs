// Decompiled with JetBrains decompiler
// Type: SolarWinds.Orion.Core.BusinessLayer.ReportJobInitializer
// Assembly: SolarWinds.Orion.Core.BusinessLayer, Version=2019.4.5200.9083, Culture=neutral, PublicKeyToken=null
// MVID: E12E8C85-5CD9-4E06-8801-182E5104FADE
// Assembly location: E:\task5.dll

using SolarWinds.Common.Utility;
using SolarWinds.Logging;
using SolarWinds.Orion.Core.Actions.Utility;
using SolarWinds.Orion.Core.Common.DALs;
using SolarWinds.Orion.Core.Common.Models;
using SolarWinds.Orion.Core.Models.Actions;
using SolarWinds.Orion.Core.Models.Actions.Contexts;
using SolarWinds.Orion.Core.Models.MacroParsing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

#nullable disable
namespace SolarWinds.Orion.Core.BusinessLayer
{
  public class ReportJobInitializer
  {
    private static readonly Log log = new Log();

    public static void AddActionsToScheduler(
      ReportJobConfiguration config,
      CoreBusinessLayerService service)
    {
      if (!config.Enabled)
        return;
      ReportingActionContext reportingContext = new ReportingActionContext()
      {
        AccountID = config.AccountID,
        UrlsGroupedByLeftPart = ReportJobInitializer.GroupUrls(config),
        WebsiteID = config.WebsiteID
      };
      ((ActionContextBase) reportingContext).MacroContext.Add((ContextBase) new ReportingContext()
      {
        AccountID = config.AccountID,
        ScheduleName = config.Name,
        ScheduleDescription = config.Description,
        LastRun = config.LastRun,
        WebsiteID = config.WebsiteID
      });
      ((ActionContextBase) reportingContext).MacroContext.Add((ContextBase) new GenericContext());
      int num = 0;
      if (config.Schedules == null)
        return;
      foreach (ReportSchedule schedule in config.Schedules)
      {
        DateTime dateTime = !schedule.EndTime.HasValue ? DateTime.MaxValue : schedule.EndTime.Value;
        Scheduler.Instance.Add(new ScheduledTask(string.Format("ReportJob-{0}_{1}", (object) config.ReportJobID, (object) num), (TimerCallback) (o =>
        {
          ReportJobInitializer.log.Info((object) "Starting action execution");
          foreach (ActionDefinition action in config.Actions)
            service.ExecuteAction(action, (ActionContextBase) reportingContext);
          config.LastRun = new DateTime?(DateTime.Now.ToUniversalTime());
          ReportJobDAL.UpdateLastRun(config.ReportJobID, config.LastRun);
        }), (object) null, schedule.CronExpression, schedule.StartTime, dateTime, config.LastRun, schedule.CronExpressionTimeZoneInfo), true);
        ++num;
      }
    }

    public static Dictionary<string, List<string>> GroupUrls(ReportJobConfiguration config)
    {
      StringBuilder errors = new StringBuilder();
      StringComparer strcmp = StringComparer.OrdinalIgnoreCase;
      Dictionary<string, List<string>> dictionary = new Dictionary<string, List<string>>((IEqualityComparer<string>) strcmp);
      if (config == null)
      {
        ReportJobInitializer.log.ErrorFormat("GroupUrls(ReportJobConfiguration) config is NULL {0}", (object) Environment.StackTrace);
        return dictionary;
      }
      try
      {
        List<string> list = config.Reports.Select<ReportTuple, string>((Func<ReportTuple, string>) (report => string.Format("{0}/Orion/Report.aspx?ReportID={1}", (object) WebsitesDAL.GetSiteAddress(config.WebsiteID), (object) report.ID))).Union<string>(config.Urls.Select<string, string>((Func<string, string>) (url => !url.Contains<char>('?') ? url + "?" : url))).ToList<string>();
        foreach (string uriString in list)
        {
          if (uriString.IndexOf("/Orion/", StringComparison.OrdinalIgnoreCase) < 0)
          {
            if (!dictionary.ContainsKey(OrionWebClient.UseDefaultWebsiteIdentifier))
              dictionary.Add(OrionWebClient.UseDefaultWebsiteIdentifier, new List<string>());
            dictionary[OrionWebClient.UseDefaultWebsiteIdentifier].Add(uriString);
          }
          else
          {
            string uriLeftPart;
            try
            {
              Uri result;
              if (!Uri.TryCreate(uriString, UriKind.Absolute, out result))
              {
                errors.AppendFormat("Invalid URL {0} \r\n", (object) uriString);
                continue;
              }
              uriLeftPart = result.GetLeftPart(UriPartial.Authority);
            }
            catch (Exception ex)
            {
              errors.AppendFormat("Invalid URL {0}. {1}\r\n", (object) uriString, (object) ex);
              continue;
            }
            if (!dictionary.ContainsKey(uriLeftPart))
              dictionary.Add(uriLeftPart, list.Where<string>((Func<string, bool>) (u =>
              {
                try
                {
                  Uri result;
                  if (Uri.TryCreate(u, UriKind.Absolute, out result))
                    return strcmp.Equals(uriLeftPart, result.GetLeftPart(UriPartial.Authority));
                  errors.AppendFormat("Invalid URL {0} \r\n", (object) u);
                  return false;
                }
                catch (Exception ex)
                {
                  errors.AppendFormat("Invalid URL {0}. {1}\r\n", (object) u, (object) ex);
                  return false;
                }
              })).ToList<string>());
          }
        }
      }
      catch (Exception ex)
      {
        errors.AppendFormat("Unexpected exception {0}", (object) ex);
      }
      if (errors.Length > 0)
      {
        StringBuilder stringBuilder = new StringBuilder().AppendFormat("Errors in ReportJob-{0}({1}) @ Engine {2} & Website {3} \r\n", (object) config.ReportJobID, (object) config.Name, (object) config.EngineId, (object) config.WebsiteID).Append((object) errors);
        ReportJobInitializer.log.Error((object) stringBuilder);
      }
      return dictionary;
    }
  }
}
