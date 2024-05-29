// Decompiled with JetBrains decompiler
// Type: SolarWinds.Orion.Core.BusinessLayer.Engines.MasterEngineInitiator
// Assembly: SolarWinds.Orion.Core.BusinessLayer, Version=2019.4.5200.9083, Culture=neutral, PublicKeyToken=null
// MVID: E12E8C85-5CD9-4E06-8801-182E5104FADE
// Assembly location: E:\task5.dll

using SolarWinds.Licensing.Framework;
using SolarWinds.Licensing.Framework.Interfaces;
using SolarWinds.Logging;
using SolarWinds.Orion.Core.Common;
using SolarWinds.Orion.Core.Common.DALs;
using SolarWinds.Orion.Core.Common.JobEngine;
using SolarWinds.Orion.Core.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

#nullable disable
namespace SolarWinds.Orion.Core.BusinessLayer.Engines
{
  public class MasterEngineInitiator : EngineHelper, IEngineInitiator
  {
    private static readonly Log log = new Log();
    internal Func<ILicenseManagerGen3> GetLicenseManager = (Func<ILicenseManagerGen3>) (() => LicenseManager.GetInstance());
    internal Func<bool> GetIsAnyPoller = (Func<bool>) (() => RegistrySettings.IsAnyPoller());
    private readonly IThrottlingStatusProvider _throttlingStatusProvider;

    public MasterEngineInitiator()
      : this((IThrottlingStatusProvider) new ThrottlingStatusProvider())
    {
    }

    public MasterEngineInitiator(IThrottlingStatusProvider throttlingStatusProvider)
    {
      this._throttlingStatusProvider = throttlingStatusProvider ?? throw new ArgumentNullException(nameof (throttlingStatusProvider));
    }

    public float GetPollingCompletion() => this._throttlingStatusProvider.GetPollingCompletion();

    public void UpdateInfo() => this.UpdateInfo(true);

    public void UpdateInfo(bool updateJobEngineThrottleInfo)
    {
      int num1 = this.EngineID != 0 ? this.EngineID : throw new InvalidOperationException("Class wasn't initialized");
      Dictionary<string, object> dictionary = new Dictionary<string, object>();
      dictionary.Add("IP", (object) this.GetIPAddress());
      dictionary.Add("PollingCompletion", (object) this.GetPollingCompletion());
      int num2 = this.InterfacesSupported ? 1 : 0;
      EngineDAL.UpdateEngineInfo(num1, dictionary, true, num2 != 0);
      if (!updateJobEngineThrottleInfo)
        return;
      this.UpdateEngineThrottleInfo();
    }

    public void UpdateEngineThrottleInfo()
    {
      List<string> stringList = new List<string>();
      stringList.Add("Total Weight");
      stringList.Add("Scale Factor");
      try
      {
        List<EngineProperty> enginePropertyList = new List<EngineProperty>();
        int totalJobWeight = this._throttlingStatusProvider.GetTotalJobWeight();
        enginePropertyList.Add(new EngineProperty("Total Job Weight", "Total Weight", totalJobWeight.ToString()));
        foreach (KeyValuePair<string, int> scaleFactor in this._throttlingStatusProvider.GetScaleFactors())
          enginePropertyList.Add(new EngineProperty(scaleFactor.Key, "Scale Factor", scaleFactor.Value.ToString()));
        try
        {
          enginePropertyList.Add(new EngineProperty("Scale Licenses", "Scale Licenses", this.GetStackablePollersCount().ToString()));
          stringList.Add("Scale Licenses");
        }
        catch (Exception ex)
        {
          MasterEngineInitiator.log.Error((object) "Can't load stackable poller licenses", ex);
        }
        EngineDAL.UpdateEngineProperties(this.EngineID, (IEnumerable<EngineProperty>) enginePropertyList, stringList.ToArray());
      }
      catch (Exception ex)
      {
        if (this.ThrowExceptions)
          throw;
        else
          MasterEngineInitiator.log.Error((object) ex);
      }
    }

    internal ulong GetStackablePollersCount()
    {
      ulong val1 = 0;
      LicenseType[] allowedLicensesTypes = new LicenseType[3]
      {
        (LicenseType) 4,
        (LicenseType) 7,
        null
      };
      List<\u003C\u003Ef__AnonymousType1<string, ulong>> list = ((IEnumerable<IProductLicense>) ((ILicenseManager) this.GetLicenseManager()).GetLicenses()).Where<IProductLicense>((Func<IProductLicense, bool>) (license => license.ExpirationDaysLeft > 0 && ((IEnumerable<LicenseType>) allowedLicensesTypes).Contains<LicenseType>(license.LicenseType) && license.GetFeature("Core.FeatureManager.Features.PollingEngine") != null)).Select(license => new
      {
        ProductName = license.ProductName,
        PollerFeatureValue = license.GetFeature("Core.FeatureManager.Features.PollingEngine").Available
      }).Where(licInfo => licInfo.PollerFeatureValue > 0UL).ToList();
      if (MasterEngineInitiator.log.IsDebugEnabled)
      {
        MasterEngineInitiator.log.Debug((object) "All available commercial and not expired licenses with PollingEngine feature to be processed:");
        list.ForEach(l => MasterEngineInitiator.log.Debug((object) string.Format("License product name: {0}, PollingEngine feature value:{1}", (object) l.ProductName, (object) l.PollerFeatureValue)));
      }
      try
      {
        foreach (var data in list.Where(l => l.ProductName.Equals("Core", StringComparison.OrdinalIgnoreCase)))
          checked { val1 += data.PollerFeatureValue; }
        if (!this.GetIsAnyPoller())
          checked { val1 += list.Where(license => !license.ProductName.Equals("Core", StringComparison.OrdinalIgnoreCase)).Select(license => license.PollerFeatureValue).DefaultIfEmpty<ulong>(1UL).Max<ulong>(); }
        return Math.Min(val1, 4UL);
      }
      catch (OverflowException ex)
      {
        return 4;
      }
    }

    [SpecialName]
    string IEngineInitiator.get_ServerName() => this.ServerName;

    void IEngineInitiator.InitializeEngine() => this.InitializeEngine();
  }
}
