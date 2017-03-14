using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Diagnostics.Formatters
{
    public class FormatterCollection : Collection<IFormatter>, IFormatterCollection
    {
        public FormatterCollection() { }

        public FormatterCollection(IFormatter[] items)
        {
            for (int i = 0; i < items.Length; i++)
            {
                var item = items[i];

                Add(item);
            }
        }

        public bool TryFindByName(string name, out IFormatter result)
        {
            result =
                (from ll in this
                 where string.Equals(ll.Name, name, StringComparison.InvariantCultureIgnoreCase)
                 select ll).FirstOrDefault();

            return result != null;
        }
    }
}
