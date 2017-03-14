using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SquaredInfinity.Extensions;
using SquaredInfinity.Diagnostics.Filters.PatternEvaluation;

namespace SquaredInfinity.Diagnostics.Filters
{
    public class PropertyFilter : Filter
    {
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
                InternalTrace.Warning(() =>
                    $"Diagnostic Property '{Property}' not found on Diagnostic Event.");

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
