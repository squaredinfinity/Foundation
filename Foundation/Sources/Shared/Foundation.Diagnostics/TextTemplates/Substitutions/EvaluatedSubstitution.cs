using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Diagnostics.TextTemplates.Substitutions
{
    /// <summary>
    /// Evaluates substituted value on each call.
    /// Does not cache the result.
    /// </summary>
    public class EvaluatedSubstitution : Substitution
    {
        public Func<TextTemplateProcessingContext, object> DoEvaluateOutput { get; set; }

        public EvaluatedSubstitution(string input, Func<TextTemplateProcessingContext, object> evaluateOutput)
        {
            this.Input = input;
            this.DoEvaluateOutput = evaluateOutput;
        }

        public override object EvaluateOutput(TextTemplateProcessingContext cx)
        {
            return DoEvaluateOutput(cx);
        }
    }
}
