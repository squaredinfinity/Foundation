using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Diagnostics.TextTemplates
{
    public class FunctionCollection : IEnumerable<Function>
    {
        Dictionary<string, Function> InternalStorage = new Dictionary<string, Function>(27, StringComparer.InvariantCultureIgnoreCase);

        public FunctionCollection()
        {
            InternalStorage = new Dictionary<string, Function>(27);
        }

        public void AddFunction(Function func)
        {
            InternalStorage.Add(func.Name, func);
        }

        public bool TryGetFunction(string name, out Function function)
        {
            function = null;

            if (!InternalStorage.ContainsKey(name))
                return false;

            function = InternalStorage[name];

            return true;
        }

        public IEnumerator<Function> GetEnumerator()
        {
            return InternalStorage.Values.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return InternalStorage.Values.GetEnumerator();
        }
    }
}
