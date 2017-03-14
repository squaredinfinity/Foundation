using SquaredInfinity.Diagnostics.ContextDataCollectors;
using SquaredInfinity.Diagnostics.Filters;
using SquaredInfinity.Diagnostics.Sinks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Diagnostics.Configuration
{
    public interface IDiagnosticsConfiguration
    {
        IConfigurationRepository Repository { get; }

        GlobalSettings Settings { get; }

        /// <summary>
        /// Global filters can be used to control which event are processed even before they get to any sink
        /// </summary>
        IFilterCollection GlobalFilters { get; }

        /// <summary>
        /// Collections of sinks to be used for logging.
        /// All sinks in this list will receive diagnostic events.
        /// </summary>
        ISinkCollection Sinks { get; }

        /// <summary>
        /// Context Data Collectors registered globally which will be used to retrieve context data as requested by sinks.
        /// </summary>
        IContextDataCollectorCollection ContextDataCollectors { get; }

        /// <summary>
        /// Additional Context Data Collectors will be used to retrieve additional context data depending on specified filter criteria.
        /// For example, you may want to include additional, more detailed (and expensive to get) information when application-lifecycle.startup event is logged, 
        /// but do not require it when verbose messages are logged.
        /// </summary>
        IContextDataCollectorCollection AdditionalContextDataCollectors { get; }

    }
}
