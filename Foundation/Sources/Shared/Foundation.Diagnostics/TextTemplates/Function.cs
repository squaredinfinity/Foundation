using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Diagnostics.TextTemplates
{
    public class Function
    {
        public string Name { get; private set; }

        public delegate object FunctionDelegate(object originalValue, object input, IEnumerable<object> parameters);

        readonly FunctionDelegate InternalFunc;

        public object Process(object originalValue, object input, IEnumerable<object> parameters)
        {
            return InternalFunc(originalValue, input, parameters);
        }

        public Function(string name, FunctionDelegate func)
        {
            Name = name.ToLower();
            InternalFunc = func;
        }
    }
}
