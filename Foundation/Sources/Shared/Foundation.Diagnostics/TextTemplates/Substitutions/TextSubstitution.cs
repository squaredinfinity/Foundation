using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Diagnostics.TextTemplates.Substitutions
{
    /// <summary>
    /// Substitues with given value
    /// </summary>
    public class TextSubstitution : Substitution
    {
        public object Output { get; set; }

        public override object EvaluateOutput(TextTemplateProcessingContext cx)
        {
            return Output;
        }

        public TextSubstitution(string input, object output)
        {
            this.Input = input;
            this.Output = output;
        }
    }
}
