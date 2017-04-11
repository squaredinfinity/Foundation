using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Tagging
{
    public class MultiValue
    {
        readonly HashSet<object> Storage = new HashSet<object>();

        public MultiValue() { }

        public MultiValue(object firstValue)
        {
            Storage.Add(firstValue);
        }

        public void Add(object value)
        {
            Storage.Add(value);
        }

        public IReadOnlyList<object> GetAll()
        {
            return Storage.ToArray();
        }

        public void Clear()
        {
            Storage.Clear();
        }
    }
}
