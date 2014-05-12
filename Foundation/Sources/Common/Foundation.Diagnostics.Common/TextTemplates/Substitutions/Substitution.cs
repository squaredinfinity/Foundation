using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Foundation.Diagnostics.TextTemplates.Substitutions
{
    public abstract class Substitution
    {
        public string Input { get; set; }
        public abstract object EvaluateOutput(TextTemplateProcessingContext cxl);
    }
}
