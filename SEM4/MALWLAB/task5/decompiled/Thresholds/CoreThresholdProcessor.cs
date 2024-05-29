// Decompiled with JetBrains decompiler
// Type: SolarWinds.Orion.Core.BusinessLayer.Thresholds.CoreThresholdProcessor
// Assembly: SolarWinds.Orion.Core.BusinessLayer, Version=2019.4.5200.9083, Culture=neutral, PublicKeyToken=null
// MVID: E12E8C85-5CD9-4E06-8801-182E5104FADE
// Assembly location: E:\task5.dll

using SolarWinds.Orion.Core.Common.ExpressionEvaluator;
using SolarWinds.Orion.Core.Common.ExpressionEvaluator.Functions;
using SolarWinds.Orion.Core.Common.Models.Thresholds;
using SolarWinds.Orion.Core.Common.Thresholds;
using SolarWinds.Orion.Core.Models;
using SolarWinds.Orion.Core.Strings;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Globalization;
using System.Linq;

#nullable disable
namespace SolarWinds.Orion.Core.BusinessLayer.Thresholds
{
  [Export(typeof (IThresholdDataProcessor))]
  public class CoreThresholdProcessor : ExprEvaluationEngine, IThresholdDataProcessor
  {
    private const string MeanVariableName = "mean";
    private const string StdDevVariableName = "std_dev";
    private const string MinVariableName = "min";
    private const string MaxVariableName = "max";
    private Dictionary<string, Variable> _variables = new Dictionary<string, Variable>();
    private readonly IFunctionsDefinition _functions = (IFunctionsDefinition) new MathFunctionsDefinition();
    private readonly CoreThresholdPreProcessor _preProcessor = new CoreThresholdPreProcessor();

    public CoreThresholdProcessor()
    {
      this.VariableConvertor = new Func<string, Variable>(this.ConvertVariable);
    }

    protected virtual IEnumerable<string> AllowedVariables
    {
      get => (IEnumerable<string>) this._variables.Keys;
    }

    protected virtual IFunctionsDefinition Functions => this._functions;

    public ValidationResult IsFormulaValid(
      string formula,
      ThresholdLevel level,
      ThresholdOperatorEnum thresholdOperator)
    {
      if (this.Log.IsDebugEnabled)
        this.Log.DebugFormat("Validating formula: {0} ...", (object) formula);
      ValidationResult validationResult;
      try
      {
        validationResult = this._preProcessor.PreValidateFormula(formula, level, thresholdOperator);
        if (validationResult.IsValid)
        {
          formula = this._preProcessor.PreProcessFormula(formula, level, thresholdOperator);
          this._variables = CoreThresholdProcessor.CreateVariables(CoreThresholdProcessor.CreateDefaultBaselineValues());
          this.TryParse(formula, true);
          validationResult = ValidationResult.CreateValid();
        }
      }
      catch (InvalidInputException ex)
      {
        validationResult = !ex.HasError ? new ValidationResult(false, ((Exception) ex).Message) : new ValidationResult(false, (IEnumerable<string>) ex.Errors.Select<ExprEvalErrorDescription, string>((Func<ExprEvalErrorDescription, string>) (er => CoreThresholdProcessor.GetErrorMessage(er))).ToArray<string>());
      }
      catch (Exception ex)
      {
        this.Log.Error((object) string.Format("Unexpected error when validating formula: {0} ", (object) formula), ex);
        validationResult = new ValidationResult(false, ex.Message);
      }
      return validationResult;
    }

    public double ComputeThreshold(
      string formula,
      BaselineValues baselineValues,
      ThresholdLevel level,
      ThresholdOperatorEnum thresholdOperator)
    {
      if (this.Log.IsVerboseEnabled)
        this.Log.VerboseFormat("Computing formula: {0}, values: [{1}]", new object[2]
        {
          (object) formula,
          (object) baselineValues
        });
      if (string.IsNullOrEmpty(formula))
        return 0.0;
      try
      {
        formula = this._preProcessor.PreProcessFormula(formula, level, thresholdOperator);
        this._variables = CoreThresholdProcessor.CreateVariables(baselineValues);
        return this.EvaluateDynamic(formula, (IDictionary<string, Variable>) this._variables, (object) null);
      }
      catch (InvalidInputException ex)
      {
        string message = !ex.HasError ? ((Exception) ex).Message : string.Join(" ", ex.Errors.Select<ExprEvalErrorDescription, string>(new Func<ExprEvalErrorDescription, string>(CoreThresholdProcessor.GetErrorMessage)).ToArray<string>());
        if (this.Log.IsInfoEnabled)
          this.Log.Info((object) string.Format("Parsing error: {0} when evaluating formula: {1}, values: {2}", (object) message, (object) formula, (object) baselineValues), (Exception) ex);
        throw new Exception(message, (Exception) ex);
      }
      catch (Exception ex)
      {
        this.Log.Error((object) string.Format("Unexpected error when evaluating formula: {0}, values: {1}", (object) formula, (object) baselineValues), ex);
        throw;
      }
    }

    public virtual bool IsBaselineValuesValid(BaselineValues baselineValues)
    {
      if (baselineValues == null)
        throw new ArgumentNullException(nameof (baselineValues));
      return baselineValues.Mean.HasValue && baselineValues.StdDev.HasValue && baselineValues.Max.HasValue && baselineValues.Min.HasValue && baselineValues.MinDateTime.HasValue && baselineValues.MaxDateTime.HasValue && baselineValues.Timestamp.HasValue;
    }

    private Variable ConvertVariable(string name)
    {
      string key = !string.IsNullOrEmpty(name) ? name.ToLowerInvariant() : throw new Exception("Variable name can't be null or empty.");
      return this._variables.ContainsKey(key) ? this._variables[key] : throw new Exception(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Can't convert variable {0}.", (object) CoreThresholdProcessor.FormatVariable(name)));
    }

    private static Dictionary<string, Variable> CreateVariables(BaselineValues baselineValues)
    {
      return new Dictionary<string, Variable>()
      {
        {
          "mean",
          new Variable()
          {
            Type = typeof (double),
            Name = "mean",
            Value = (object) baselineValues.Mean
          }
        },
        {
          "std_dev",
          new Variable()
          {
            Type = typeof (double),
            Name = "std_dev",
            Value = (object) baselineValues.StdDev
          }
        },
        {
          "min",
          new Variable()
          {
            Type = typeof (double),
            Name = "min",
            Value = (object) baselineValues.Min
          }
        },
        {
          "max",
          new Variable()
          {
            Type = typeof (double),
            Name = "max",
            Value = (object) baselineValues.Max
          }
        }
      };
    }

    private static BaselineValues CreateDefaultBaselineValues()
    {
      return new BaselineValues()
      {
        Count = 1,
        Max = new double?(1.0),
        Mean = new double?(1.0),
        Min = new double?(1.0),
        StdDev = new double?(1.0)
      };
    }

    private static string GetErrorMessage(ExprEvalErrorDescription err)
    {
      switch ((int) err.Type)
      {
        case 0:
          return err.Message;
        case 1:
        case 2:
          return string.IsNullOrEmpty(err.InvalidText) ? string.Format((IFormatProvider) CultureInfo.InvariantCulture, Resources.LIBCODE_ZT0_12, (object) err.CharPosition) : string.Format((IFormatProvider) CultureInfo.InvariantCulture, Resources.LIBCODE_ZT0_13, (object) err.InvalidText, (object) err.CharPosition);
        case 3:
          return string.Format((IFormatProvider) CultureInfo.InvariantCulture, Resources.LIBCODE_ZT0_14, (object) CoreThresholdProcessor.FormatVariable(err.InvalidText));
        case 4:
          return string.Format((IFormatProvider) CultureInfo.InvariantCulture, Resources.LIBCODE_ZT0_15, (object) err.InvalidText);
        case 5:
          return string.Format((IFormatProvider) CultureInfo.InvariantCulture, Resources.LIBCODE_ZT0_16, (object) err.InvalidText);
        default:
          return string.Empty;
      }
    }

    private static string FormatVariable(string name)
    {
      return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "${0}{1}{2}", (object) "{", (object) name, (object) "}");
    }
  }
}
