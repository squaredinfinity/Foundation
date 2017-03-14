using SquaredInfinity.Diagnostics.Filters.PatternEvaluation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Diagnostics.Filters
{
    public class LoggerNameFilter : Filter
    {
        public string Pattern { get; set; }

        readonly Lazy<PatternEvaluator> Evaluator;

        public LoggerNameFilter()
        {
            Evaluator =
                new Lazy<PatternEvaluator>(
                    () => PatternEvaluator.GetEvaluatorForPattern(Pattern),
                    System.Threading.LazyThreadSafetyMode.ExecutionAndPublication);
        }

        public override bool Evaluate(IDiagnosticEvent diagnosticEvent, ILoggerName loggerName)
        {
            bool meetsCriteria = true;

            if (!Evaluator.Value.MatchesPattern(diagnosticEvent.LoggerName))
            {
                meetsCriteria = false;
            }
            else
            {
                foreach (var name in loggerName.NamePartHierarchy)
                {
                    if (!Evaluator.Value.MatchesPattern(name))
                    {
                        meetsCriteria = false;
                        break;
                    }
                }
            }

            if (Mode == FilterMode.Include)
                return meetsCriteria;
            else
                return !meetsCriteria;
        }
    }
}
