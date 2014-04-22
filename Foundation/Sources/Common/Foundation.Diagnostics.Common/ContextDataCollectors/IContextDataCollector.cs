using SquaredInfinity.Foundation.Diagnostics.ContextDataCollectors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SquaredInfinity.Foundation.ContextDataCollectors
{
    public interface IContextDataCollector
    {
        /// <summary>
        /// True if this collector should execute asynchronously, False otherwise.
        /// </summary>
        bool IsAsync { get; set; }

        string Name { get; set; }

        IFilter Filter { get; set; }

#if UPTO_DOTNET40
        IDictionary<string, object> 
#else
        IReadOnlyDictionary<string, object>
#endif
 CollectData();
        bool TryGetData(DataRequest dataRequest, DataCollectionContext context, out object result);

        IContextDataCollector Clone();

#if UPTO_DOTNET40
        IDictionary<DiagnosticEventProperty>
#else
        IReadOnlyDictionary<string, object>>
#endif
 CollectData(IList<DataRequest> requestedContextData);
    }
}
