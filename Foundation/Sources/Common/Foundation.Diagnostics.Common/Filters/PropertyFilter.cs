using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SquaredInfinity.Foundation.Extensions;
using SquaredInfinity.Foundation.Diagnostics.Filters.PatternEvaluation;

namespace SquaredInfinity.Foundation.Diagnostics.Filters
{
    public class PropertyFilter : Filter
    {
        readonly static ILogger Diag = InternalLogger.CreateLoggerForType<PropertyFilter>();

        public string Property { get; set; }

        public string Value { get; set; }

        readonly Lazy<PatternEvaluator> Evaluator;

        public PropertyFilter()
        {
            Evaluator =
                new Lazy<PatternEvaluator>(
                    () => PatternEvaluator.GetEvaluatorForPattern(Value),
                    System.Threading.LazyThreadSafetyMode.ExecutionAndPublication);
        }

        public override bool Evaluate(IDiagnosticEvent diagnosticEvent, ILoggerName loggerName)
        {
            bool meetsCriteria = false;

            var propertyValue = (object) null;

            if (!diagnosticEvent.Properties.TryGetValue(Property, out propertyValue))
            {
                Diag.Warning(() =>
                    "Diagnostic Property '{0}' not found on Diagnostic Event."
                    .FormatWith(Property));

                meetsCriteria = false;
            }
            else
            {
                meetsCriteria = Evaluator.Value.MatchesPattern(propertyValue.ToString());
            }

            if (Mode == FilterMode.Include)
                return meetsCriteria;
            else
                return !meetsCriteria;
        }
    }
}
