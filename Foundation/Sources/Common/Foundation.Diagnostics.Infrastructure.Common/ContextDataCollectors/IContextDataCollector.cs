using SquaredInfinity.Foundation.Diagnostics.ContextDataCollectors;
using SquaredInfinity.Foundation.Diagnostics.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SquaredInfinity.Foundation.Diagnostics.ContextDataCollectors
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

        bool TryGetData(IDataRequest dataRequest, IDataCollectionContext context, out object result);

        IContextDataCollector Clone();
    }
}
