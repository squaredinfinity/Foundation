using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SquaredInfinity.Extensions;

namespace SquaredInfinity.Diagnostics.Filters.PatternEvaluation
{
    /// <summary>
    /// at the moment the only supported wildcard is * at the end of the pattern
    /// </summary>
    public class WildcardPatternEvaluator : PatternEvaluator
    {
        string SanitizedPattern { get; set; }

        public WildcardPatternEvaluator(string pattern)
            : base(pattern)
        {
            if (!pattern.EndsWith("*"))
            {
                var ex = new ArgumentException("Pattern should end with '*'");
                ex.TryAddContextData("pattern", () => pattern);
                throw ex;
            }

            SanitizedPattern = pattern.Substring(0, pattern.Length - 1);
        }

        public override bool MatchesPattern(string input)
        {
            return input.StartsWith(SanitizedPattern);
        }
    }
}
