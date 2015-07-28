using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Foundation.Diagnostics.Filters
{
    public interface IFilterCollection :
        ICollection<IFilter>, 
        //IBulkUpdatesCollection<IFilter>, 
        //INotifyCollectionContentChanged,
        IList<IFilter>
    {
    }
}
