// Decompiled with JetBrains decompiler
// Type: SolarWinds.Orion.Core.BusinessLayer.BusinessLayerSettings
// Assembly: SolarWinds.Orion.Core.BusinessLayer, Version=2019.4.5200.9083, Culture=neutral, PublicKeyToken=null
// MVID: E12E8C85-5CD9-4E06-8801-182E5104FADE
// Assembly location: E:\task5.dll

using SolarWinds.Orion.Core.Common.Configuration;
using SolarWinds.Orion.Core.SharedCredentials.Credentials;
using SolarWinds.Settings;
using System;

#nullable disable
namespace SolarWinds.Orion.Core.BusinessLayer
{
  internal class BusinessLayerSettings : SettingsBase, IBusinessLayerSettings
  {
    private static readonly Lazy<IBusinessLayerSettings> instance = new Lazy<IBusinessLayerSettings>((Func<IBusinessLayerSettings>) (() => (IBusinessLayerSettings) new BusinessLayerSettings()));
    public static Func<IBusinessLayerSettings> Factory = (Func<IBusinessLayerSettings>) (() => BusinessLayerSettings.instance.Value);

    private BusinessLayerSettings()
    {
    }

    public static IBusinessLayerSettings Instance => BusinessLayerSettings.Factory();

    [Setting(Default = 30, AllowServerOverride = true, ServiceRestartDependencies = new string[] {"OrionModuleEngine"})]
    public int DBConnectionRetryInterval { get; internal set; }

    [Setting(Default = 300, AllowServerOverride = true, ServiceRestartDependencies = new string[] {"OrionModuleEngine"})]
    public int DBConnectionRetryIntervalOnFail { get; internal set; }

    [Setting(Default = 10, AllowServerOverride = true, ServiceRestartDependencies = new string[] {"OrionModuleEngine"})]
    public int DBConnectionRetries { get; internal set; }

    [Setting(Default = "net.pipe://localhost/solarwinds/jobengine/scheduler", AllowServerOverride = true, ServiceRestartDependencies = new string[] {"OrionModuleEngine"})]
    public string JobSchedulerEndpointNetPipe { get; internal set; }

    [Setting(Default = "net.tcp://{0}:17777/solarwinds/jobengine/scheduler/ssl", AllowServerOverride = true, ServiceRestartDependencies = new string[] {"OrionModuleEngine"})]
    public string JobSchedulerEndpointTcpPipe { get; internal set; }

    [Obsolete("use SolarWinds.Orion.Core.Common.Configuration.HttpProxySettings without using WCF")]
    public bool ProxyAvailable => !HttpProxySettings.Instance.IsDisabled;

    [Obsolete("use SolarWinds.Orion.Core.Common.Configuration.HttpProxySettings without using WCF")]
    public string UserName
    {
      get
      {
        IHttpProxySettings instance = HttpProxySettings.Instance;
        if (!instance.IsValid)
          return (string) null;
        if (instance.UseSystemDefaultProxy)
          return string.Empty;
        return !(instance.Credential is UsernamePasswordCredential credential) ? (string) null : credential.Username;
      }
    }

    [Obsolete("use SolarWinds.Orion.Core.Common.Configuration.HttpProxySettings without using WCF")]
    public string Password
    {
      get
      {
        IHttpProxySettings instance = HttpProxySettings.Instance;
        if (!instance.IsValid)
          return (string) null;
        return !(instance.Credential is UsernamePasswordCredential credential) ? (string) null : credential.Password;
      }
    }

    [Obsolete("use SolarWinds.Orion.Core.Common.Configuration.HttpProxySettings without using WCF")]
    public string ProxyAddress
    {
      get
      {
        IHttpProxySettings instance = HttpProxySettings.Instance;
        return !instance.IsValid ? string.Empty : new Uri(instance.Uri).Host;
      }
    }

    [Obsolete("use SolarWinds.Orion.Core.Common.Configuration.HttpProxySettings without using WCF")]
    public int ProxyPort
    {
      get
      {
        IHttpProxySettings instance = HttpProxySettings.Instance;
        return !instance.IsValid ? 0 : new Uri(instance.Uri).Port;
      }
    }

    [Setting(Default = "http://thwackfeeds.solarwinds.com/blogs/orion-product-team-blog/rss.aspx", AllowServerOverride = true, ServiceRestartDependencies = new string[] {"OrionModuleEngine"})]
    public string OrionProductTeamBlogUrl { get; internal set; }

    [Setting(Default = 60, AllowServerOverride = true, ServiceRestartDependencies = new string[] {"OrionModuleEngine"})]
    public int LicenseSaturationCheckInterval { get; internal set; }

    [Setting(Default = 90, AllowServerOverride = true, ServiceRestartDependencies = new string[] {"OrionModuleEngine"})]
    public int MaintenanceExpirationWarningDays { get; internal set; }

    [Setting(Default = 15, AllowServerOverride = true, ServiceRestartDependencies = new string[] {"OrionModuleEngine"})]
    public int MaintenanceExpiredShowAgainAtDays { get; internal set; }

    [Setting(Default = "00:05:00", AllowServerOverride = true, ServiceRestartDependencies = new string[] {"OrionModuleEngine"})]
    public TimeSpan PollerLimitTimer { get; internal set; }

    [Setting(Default = "00:30:00", AllowServerOverride = true, ServiceRestartDependencies = new string[] {"OrionModuleEngine"})]
    public TimeSpan CheckDatabaseLimitTimer { get; internal set; }

    [Setting(Default = "00:05:00", AllowServerOverride = true, ServiceRestartDependencies = new string[] {"OrionModuleEngine"})]
    public TimeSpan CheckForOldLogsTimer { get; internal set; }

    [Setting(Default = "00:00:30", AllowServerOverride = true, ServiceRestartDependencies = new string[] {"OrionModuleEngine"})]
    public TimeSpan UpdateEngineTimer { get; internal set; }

    [Setting(Default = "00:02:00", AllowServerOverride = true, ServiceRestartDependencies = new string[] {"OrionModuleEngine"})]
    public TimeSpan RemoteCollectorStatusCacheExpiration { get; internal set; }

    [Setting(Default = false, AllowServerOverride = true, ServiceRestartDependencies = new string[] {"OrionModuleEngine"})]
    public bool MaintenanceModeEnabled { get; internal set; }

    [Setting(Default = "00:01:00", AllowServerOverride = true, ServiceRestartDependencies = new string[] {"OrionModuleEngine"})]
    public TimeSpan SettingsToRegistryFrequency { get; internal set; }

    [Setting(Default = "00:00:10", AllowServerOverride = true, ServiceRestartDependencies = new string[] {"OrionModuleEngine"})]
    public TimeSpan DiscoveryUpdateNetObjectStatusWaitForChangesDelay { get; internal set; }

    [Setting(Default = "00:02:00", AllowServerOverride = true, ServiceRestartDependencies = new string[] {"OrionModuleEngine"})]
    public TimeSpan DiscoveryUpdateNetObjectStatusStartupDelay { get; internal set; }

    [Setting(Default = true, AllowServerOverride = true, ServiceRestartDependencies = new string[] {"OrionModuleEngine"})]
    public bool EnableLimitationReplacement { get; internal set; }

    [Setting(Default = true, AllowServerOverride = true, ServiceRestartDependencies = new string[] {"OrionModuleEngine"})]
    public bool EnableTechnologyPollingAssignmentsChangesAuditing { get; internal set; }

    [Setting(Default = 5, AllowServerOverride = true, ServiceRestartDependencies = new string[] {"OrionModuleEngine"})]
    public int LimitationSqlExaggeration { get; internal set; }

    [Setting(Default = "00:10:00", AllowServerOverride = true)]
    public TimeSpan AgentDiscoveryPluginsDeploymentTimeLimit { get; internal set; }

    [Setting(Default = "00:10:00", AllowServerOverride = true)]
    public TimeSpan AgentDiscoveryJobTimeout { get; internal set; }

    [Setting(Default = "1.00:00:00", AllowServerOverride = true, ServiceRestartDependencies = new string[] {"OrionModuleEngine"}, Description = "Time for which we try to perform safe certificate maintenance. If the certificate maintenance can't be done in a safe way - no damage to the system or data - for given time, we inform user and let him confirm maintenance with knowledge what will break.")]
    public TimeSpan SafeCertificateMaintenanceTrialPeriod { get; internal set; }

    [Setting(Default = "00:05:00", AllowServerOverride = true, ServiceRestartDependencies = new string[] {"OrionModuleEngine"}, Description = "Frequency with which certificate maintenance task result is checked.")]
    public TimeSpan CertificateMaintenanceTaskCheckInterval { get; internal set; }

    [Setting(Default = "00:05:00", AllowServerOverride = true, ServiceRestartDependencies = new string[] {"OrionModuleEngine"}, Description = "After how long a notification about certificate maintenance approval reappears if customer acknowledges it and does not approve certificate maintenance.")]
    public TimeSpan CertificateMaintenanceNotificationReappearPeriod { get; internal set; }

    [Setting(Default = "00:01:00", AllowServerOverride = true, ServiceRestartDependencies = new string[] {"OrionModuleEngine"}, Description = "How often Core polls AMS to get fresh status of agent certificate update for certificate maintenance.")]
    public TimeSpan CertificateMaintenanceAgentPollFrequency { get; internal set; }

    [Setting(Default = "00:00:30", AllowServerOverride = true, ServiceRestartDependencies = new string[] {"OrionModuleEngine"}, Description = "How long Core waits for results from test jobs.")]
    public TimeSpan TestJobTimeout { get; internal set; }

    [Setting(Default = "00:10:00", AllowServerOverride = true, ServiceRestartDependencies = new string[] {"OrionModuleEngine"})]
    public TimeSpan BackgroundInventoryCheckTimer { get; internal set; }

    [Setting(Default = 50, AllowServerOverride = true, ServiceRestartDependencies = new string[] {"OrionModuleEngine"})]
    public int BackgroundInventoryParallelTasksCount { get; internal set; }

    [Setting(Default = 10, AllowServerOverride = true, ServiceRestartDependencies = new string[] {"OrionModuleEngine"})]
    public int BackgroundInventoryRetriesCount { get; internal set; }

    [Setting(Default = 10000, AllowServerOverride = true, ServiceRestartDependencies = new string[] {"OrionModuleEngine"})]
    public int ThresholdsProcessingBatchSize { get; internal set; }

    [Setting(Default = "Core_All", AllowServerOverride = true, ServiceRestartDependencies = new string[] {"OrionModuleEngine"})]
    public string ThresholdsProcessingDefaultTimeFrame { get; internal set; }

    [Setting(Default = "00:05:00", AllowServerOverride = true, ServiceRestartDependencies = new string[] {"OrionModuleEngine"})]
    public TimeSpan ThresholdsProcessingDefaultTimer { get; internal set; }

    [Setting(Default = true, AllowServerOverride = true, ServiceRestartDependencies = new string[] {"OrionModuleEngine"})]
    public bool ThresholdsProcessingEnabled { get; internal set; }

    [Setting(Default = "${USE_BASELINE}", AllowServerOverride = true, ServiceRestartDependencies = new string[] {"OrionModuleEngine"})]
    public string ThresholdsUseBaselineCalculationMacro { get; internal set; }

    [Setting(Default = "${USE_BASELINE_WARNING}", AllowServerOverride = true, ServiceRestartDependencies = new string[] {"OrionModuleEngine"})]
    public string ThresholdsUseBaselineWarningCalculationMacro { get; internal set; }

    [Setting(Default = "${USE_BASELINE_CRITICAL}", AllowServerOverride = true, ServiceRestartDependencies = new string[] {"OrionModuleEngine"})]
    public string ThresholdsUseBaselineCriticalCalculationMacro { get; internal set; }

    [Setting(Default = "${MEAN}+2*${STD_DEV}", AllowServerOverride = true, ServiceRestartDependencies = new string[] {"OrionModuleEngine"})]
    public string ThresholdsDefaultWarningFormulaForGreater { get; internal set; }

    [Setting(Default = "${MEAN}-2*${STD_DEV}", AllowServerOverride = true, ServiceRestartDependencies = new string[] {"OrionModuleEngine"})]
    public string ThresholdsDefaultWarningFormulaForLess { get; internal set; }

    [Setting(Default = "${MEAN}+3*${STD_DEV}", AllowServerOverride = true, ServiceRestartDependencies = new string[] {"OrionModuleEngine"})]
    public string ThresholdsDefaultCriticalFormulaForGreater { get; internal set; }

    [Setting(Default = "${MEAN}-3*${STD_DEV}", AllowServerOverride = true, ServiceRestartDependencies = new string[] {"OrionModuleEngine"})]
    public string ThresholdsDefaultCriticalFormulaForLess { get; internal set; }

    [Setting(Default = 50, AllowServerOverride = true, ServiceRestartDependencies = new string[] {"OrionModuleEngine"})]
    public int ThresholdsHistogramChartIntervalsCount { get; internal set; }

    [Setting(Default = 12, AllowServerOverride = true, ServiceRestartDependencies = new string[] {"OrionModuleEngine"})]
    public int EvaluationExpirationCheckIntervalHours { get; internal set; }

    [Setting(Default = 14, AllowServerOverride = true, ServiceRestartDependencies = new string[] {"OrionModuleEngine"})]
    public int EvaluationExpirationNotificationDays { get; internal set; }

    [Setting(Default = 7, AllowServerOverride = true, ServiceRestartDependencies = new string[] {"OrionModuleEngine"})]
    public int EvaluationExpirationShowAgainAtDays { get; internal set; }

    [Setting(Default = 7, AllowServerOverride = true, ServiceRestartDependencies = new string[] {"OrionModuleEngine"})]
    public int CachedWebImageExpirationPeriodDays { get; internal set; }

    [Setting(Default = "00:10:00", AllowServerOverride = true, ServiceRestartDependencies = new string[] {"OrionModuleEngine"}, Description = "How often we synchronize Orion.Features.")]
    public TimeSpan OrionFeatureRefreshTimer { get; internal set; }
  }
}
