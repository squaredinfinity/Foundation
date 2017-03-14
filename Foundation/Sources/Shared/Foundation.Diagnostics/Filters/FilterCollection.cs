using SquaredInfinity.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Diagnostics.Filters
{
    public class FilterCollection : CollectionEx<IFilter>, IFilterCollection
    {
        public FilterCollection() { }

        public FilterCollection(IFilter[] items)
            : base(items)
        { }

        public IFilter this[string filterName]
        {
            get
            {
                var result =
                    (from f in this
                     where string.Equals(f.Name, filterName, StringComparison.InvariantCultureIgnoreCase)
                     select f).FirstOrDefault();

                return result;
            }
        }

    }
}
