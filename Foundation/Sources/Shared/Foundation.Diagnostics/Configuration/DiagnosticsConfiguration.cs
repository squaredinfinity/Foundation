using SquaredInfinity.Foundation.Diagnostics.ContextDataCollectors;
using SquaredInfinity.Foundation.Diagnostics.Filters;
using SquaredInfinity.Foundation.Diagnostics.Formatters;
using SquaredInfinity.Foundation.Diagnostics.Sinks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Foundation.Diagnostics.Configuration
{


    public class DiagnosticsConfiguration : IDiagnosticsConfiguration
    {
        public IConfigurationRepository Repository { get; set; }

        public GlobalSettings Settings { get; set; }
        
        /// <summary>
        /// Global filters can be used to control which event are processed even before they get to any sink
        /// </summary>
        public IFilterCollection GlobalFilters { get; set; }

        /// <summary>
        /// Collections of sinks to be used for logging.
        /// All sinks in this list will receive diagnostic events.
        /// </summary>
        public ISinkCollection Sinks { get; set; }

        /// <summary>
        /// Context Data Collectors registered globally which will be used to retrieve context data as requested by sinks.
        /// </summary>
        public IContextDataCollectorCollection ContextDataCollectors { get; set; }

        /// <summary>
        /// Additional Context Data Collectors will be used to retrieve additional context data depending on specified filter criteria.
        /// For example, you may want to include additional, more detailed (and expensive to get) information when application-lifecycle.startup event is logged, 
        /// but do not require it when verbose messages are logged.
        /// </summary>
        public IContextDataCollectorCollection AdditionalContextDataCollectors { get; set; }

        public DiagnosticsConfiguration()
        {
            this.Settings = new GlobalSettings();

            this.GlobalFilters = new FilterCollection();

            this.ContextDataCollectors = new ContextDataCollectorCollection();
            this.AdditionalContextDataCollectors = new ContextDataCollectorCollection();

            this.Sinks = new SinkCollection();

            this.Repository = new ConfigurationRepository();
        }
    }
}
