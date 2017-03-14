using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Diagnostics.ContextDataCollectors
{
    public interface IContextDataCollectorCollection :
        ICollection<IContextDataCollector>,
//        IBulkUpdatesCollection<IContextDataCollector>,
        //INotifyCollectionContentChanged,
        IList<IContextDataCollector>
    {
        IDiagnosticEventPropertyCollection Collect();
        IDiagnosticEventPropertyCollection Collect(IReadOnlyList<IDataRequest> requestedContextData);
    }
}
