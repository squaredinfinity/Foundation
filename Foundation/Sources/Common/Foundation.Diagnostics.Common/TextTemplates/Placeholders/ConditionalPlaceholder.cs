using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SquaredInfinity.Foundation.Extensions;

namespace SquaredInfinity.Foundation.Diagnostics.TextTemplates.Placeholders
{
    internal class ConditionalPlaceholder : Placeholder
    {
        static readonly ILogger Diagnostics = InternalDiagnosticLogger.CreateLoggerForType<ConditionalPlaceholder>();

        public string TrueOutcomeSubstitutionPattern { get; set; }
        public List<FunctionDefinition> TrueOutcomeProcessFunctions { get; set; }

        public override bool TryEvaluate(TextTemplateProcessingContext cx, out object result)
        {
            result = (object)null;

            // if was able to evalue condition, check if it is true, if it is the process True Outcome, otherwise return empty string
            if (TryEvaluateInternal(cx, SubstitutionPattern, Functions, out result))
            {
                if (result is bool && (bool)result == true)
                {
                    if (TryEvaluateInternal(cx, TrueOutcomeSubstitutionPattern, TrueOutcomeProcessFunctions, out result))
                    {
                        return true;
                    }
                    else
                    {
                        Diagnostics.Information(() => "Unable to process True branch for placeholder '{0}'".FormatWith(RawValue));
                        return false;
                    }
                }
                else
                {
                    result = "";
                    return true;
                }
            }
            else
            {
                result = null;
                return false;
            }
        }

    }
}
