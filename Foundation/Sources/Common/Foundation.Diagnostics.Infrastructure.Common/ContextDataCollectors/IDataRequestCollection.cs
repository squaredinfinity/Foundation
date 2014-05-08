using SquaredInfinity.Foundation.Collections;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Foundation.Diagnostics.ContextDataCollectors
{
    public interface IDataRequestCollection : 
        ICollection<IDataRequest>,
        IBulkUpdatesCollection<IDataRequest>,
        INotifyCollectionContentChanged,
        IReadOnlyList<IDataRequest>,
        IList
    {
    }
}
