using SquaredInfinity.Foundation.Diagnostics.ContextDataCollectors;
using SquaredInfinity.Foundation.Diagnostics.Formatters;
using SquaredInfinity.Foundation.Diagnostics.Sinks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Foundation.Diagnostics.Configuration
{
    public class DiagnosticsConfiguration
    {
        internal ConfigurationCache LogConfigCache;

        internal List<DataRequest> RequestedContextDataCache = new List<DataRequest>();

        public GlobalSettings GlobalSettings { get; set; }

        public IFormatterCollection Formatters { get; set; }
        public ISeverityLevelCollection SeverityLevels { get; set; }

        public ITypeDescriptorCollection TypeDescriptors { get; set; }

        #region Filters

        /// <summary>
        /// Collection of filters which can be reused in various components.
        /// </summary>
        public IFilterCollection FilterDefinitions { get; set; }

        public IFilterCollection GlobalFilters { get; set; }

        #endregion

        #region Sinks

        /// <summary>
        /// Collection of sinks which can be reused in various components.
        /// Not all sinks must be used.
        /// </summary>
        public ISinkCollection SinkDefinitions { get; set; }

        /// <summary>
        /// Collections of sinks to be used for logging.
        /// All sinks in this list will receive diagnostic events.
        /// </summary>
        public ISinkCollection Sinks { get; set; }

        // TODO: is this still needed?
        public SinkCollection InternalSinks { get; set; }

        #endregion

        #region Context Data Collectors

        public ContextDataCollectorCollection GlobalContextDataCollectors { get; set; }
        public ContextDataCollectorCollection AdditionalContextDataCollectors { get; set; }

        #endregion

        public DiagnosticsConfiguration()
        {
            this.GlobalSettings = new GlobalSettings();

            this.Formatters = new FormatterCollection();
            this.SeverityLevels = new SeverityLevelCollection();

            this.TypeDescriptors = new TypeDescriptorCollection();

            this.FilterDefinitions = new FilterCollection();
            this.GlobalFilters = new FilterCollection();

            this.GlobalContextDataCollectors = new ContextDataCollectorCollection();
            this.AdditionalContextDataCollectors = new ContextDataCollectorCollection();

            this.SinkDefinitions = new SinkCollection();
            this.Sinks = new SinkCollection();
        }
    }
}
