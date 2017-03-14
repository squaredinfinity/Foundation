using SquaredInfinity.Diagnostics.ContextDataCollectors;
using SquaredInfinity.Diagnostics.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SquaredInfinity.Diagnostics.ContextDataCollectors
{
    public interface IContextDataCollector
    {
        /// <summary>
        /// True if this collector should execute asynchronously, False otherwise.
        /// </summary>
        bool IsAsync { get; set; }

        string Name { get; set; }

        IFilter Filter { get; set; }

        IDiagnosticEventPropertyCollection CollectData();
        IDiagnosticEventPropertyCollection CollectData(IReadOnlyList<IDataRequest> requestedContextData);

        bool TryGetData(IDataRequest dataRequest, IDataCollectionContext context, out object result);
    }
}
