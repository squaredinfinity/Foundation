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

        public FormatterCollection Formatters { get; set; }
        public SeverityLevelCollection SeverityLevels { get; set; }

        public TypeDescriptorCollection TypeDescriptors { get; set; }

        #region Filters

        /// <summary>
        /// Collection of filters which can be reused in various components.
        /// </summary>
        public FilterCollection FilterDefinitions { get; set; }

        public FilterCollection GlobalFilters { get; set; }

        #endregion

        #region Sinks

        /// <summary>
        /// Collection of sinks which can be reused in various components.
        /// Not all sinks must be used.
        /// </summary>
        public SinkCollection SinkDefinitions { get; set; }

        /// <summary>
        /// Collections of sinks to be used for logging.
        /// All sinks in this list will receive diagnostic events.
        /// </summary>
        public SinkCollection Sinks { get; set; }

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
