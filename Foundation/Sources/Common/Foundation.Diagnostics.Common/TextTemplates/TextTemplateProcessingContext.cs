using SquaredInfinity.Foundation.Diagnostics.TextTemplates.Substitutions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Foundation.Diagnostics.TextTemplates
{
    public class TextTemplateProcessingContext
    {
        public Dictionary<string, object> Variables { get; private set; }

        public SubstitutionCollection Substitutions { get; private set; }

        public FunctionCollection Functions { get; private set; }

        public TextTemplateProcessingContext()
        {
            Variables = new Dictionary<string, object>();
            Substitutions = new SubstitutionCollection();
            Functions = new FunctionCollection();
        }
    }
}
