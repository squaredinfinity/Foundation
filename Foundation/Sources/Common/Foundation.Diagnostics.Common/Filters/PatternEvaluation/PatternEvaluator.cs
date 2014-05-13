using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SquaredInfinity.Foundation.Extensions;

namespace SquaredInfinity.Foundation.Diagnostics.Filters.PatternEvaluation
{
    public class PatternEvaluator
    {
        protected string RawPattern { get; set; }

        public PatternEvaluator(string pattern)
        {
            RawPattern = pattern;
        }

        public virtual bool MatchesPattern(string input)
        {
            return input == RawPattern;
        }

        public static PatternEvaluator GetEvaluatorForPattern(string pattern)
        {
            if (pattern.IsNullOrEmpty())
            {
                return new PatternEvaluator(string.Empty);
            }
            else if (pattern.EndsWith("*"))
            {
                return new WildcardPatternEvaluator(pattern);
            }
            else
            {
                return new PatternEvaluator(pattern);
            }
        }
    }
}
