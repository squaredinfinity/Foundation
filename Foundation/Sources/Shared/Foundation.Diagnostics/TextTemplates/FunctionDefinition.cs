using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Diagnostics.TextTemplates
{
    public class FunctionDefinition
    {
        public string Name { get; set; }
        public List<object> Parameters { get; private set; }

        public FunctionDefinition()
        {
            Parameters = new List<object>();
        }
    }
}
