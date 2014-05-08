using SquaredInfinity.Foundation.Diagnostics.Configuration;
using SquaredInfinity.Foundation.Diagnostics.ContextDataCollectors;
using SquaredInfinity.Foundation.Diagnostics.Filters;
using SquaredInfinity.Foundation.Diagnostics.Fluent;
using SquaredInfinity.Foundation.Diagnostics.Formatters;
using SquaredInfinity.Foundation.Diagnostics.Sinks;
using SquaredInfinity.Foundation.Extensions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Foundation.Diagnostics
{
    public partial class DiagnosticLogger : ILogger, IHideObjectInheritedMembers
    {
        Lazy<ILogger> Diagnostics = new Lazy<ILogger>(() => new InternalDiagnosticLogger("SquaredInfinity.Diagnostics.DiagnosticLogger"));

        public ILoggerName Name { get; set; }
        public ILogger Parent { get; set; }

        //public ILoggerConfigProvider Configuration
        //{
        //    get { return (ILoggerConfigProvider)this; }
        //}
        
        /// <summary>
        /// Returns a cloned copy of current configuration.
        /// You can modify the copy and apply it using ApplyConfiguration() method.
        /// </summary>
        /// <returns></returns>
        public DiagnosticsConfiguration GetConfigurationClone()
        {
            return Config.DeepClone();
        }

        /// <summary>
        /// Applies specified configuration.
        /// </summary>
        /// <param name="newConfiguration"></param>
        public void ApplyConfiguration(DiagnosticsConfiguration newConfiguration)
        {
            var clone = newConfiguration.DeepClone();

            this.Config = clone;
        }

        /// <summary>
        /// Current configuration.
        /// </summary>
        DiagnosticsConfiguration Config { get; set; }
        
        ConfigurationCache ConfigCache = new ConfigurationCache();

        DiagnosticEventPropertyCollection _globalProperties = new DiagnosticEventPropertyCollection();
        /// <summary>
        /// Global Properties
        /// </summary>
        public IDiagnosticEventPropertyCollection GlobalProperties
        {
            get { return _globalProperties; }
        }

        public DiagnosticLogger()
        { }

        public DiagnosticLogger(string name)
        {
            this.Name = new LoggerName(name);
        }

        /// <summary>
        /// Processes specified Diagnostic Event.
        /// Attaches context information, applies filters and sends to Sinks.
        /// </summary>
        /// <param name="de"></param>
        public virtual void ProcessDiagnosticEvent(IDiagnosticEvent de)
        {
            // if logging disabled, exit early to improve performance
            if (!Config.Settings.EnableLogging)
                return;

            // make a copy of reference to current configuration
            // to make sure that any changes (which would replace Config)
            // will not be applied to this method before it exits
            var config_ref = Config;

            ProcessDiagnosticEventInternal(de, config_ref);
        }

        void ProcessDiagnosticEventInternal(IDiagnosticEvent de, DiagnosticsConfiguration config_ref)
        {
            // NOTE:    This method is *performance critical* and should be optimized for quickest execution even at cost of readability
            //          This method is static because it should not use any instance members of DiagnosticLogger

            try
            {
                //# Include Caller Info
                if (config_ref.Settings.IncludeCallerInfo || config_ref.Settings.UseCallerNameAsLoggerName)
                {
                    de.IncludeCallerInformation(
                        config_ref.Settings.UseCallerNameAsLoggerName,
                        config_ref.Settings.IncludeCallerInfo);
                }

                //# Override logger name only if it has not already been set (custom name might have been used)
                if (de.LoggerName == null)
                    de.LoggerName = Name.ToString();

                //# COPY GLOBAL PROPERTIES
                de.Properties.AddOrUpdateRange(GlobalProperties);

                //# EVALUATE AND PIN DIAGNOSTIC EVENT DATA
                de.EvaluateAndPinAllDataIfNeeded();

                //# GLOBAL FILTER
                
                //! Filtering on a global level must happen *after* logger name is set and properties pinned
                //! This is because filters are most likely use their values

                if (config_ref.GlobalFilters.Count > 0 && !MeetsFiltersCriteria(de, Name, config_ref.Filters))
                    return;

                //# GET SINKS

                // only use sinks which meet filtering criteria for this event

                var mustWaitForWriteSinks =
                    (from s in config_ref.Sinks.MustWaitForWriteSinks
                     where s.Filter.Evaluate(de, Name)
                     select s).ToArray();

                var fireAndForgetSinks =
                    (from s in config_ref.Sinks.FireAndForgetRawMessageSinks
                     where s.Filter.Evaluate(de, Name)
                     select s).ToArray();

                var allSinks = mustWaitForWriteSinks.Concat(fireAndForgetSinks).ToArray();

                //# REQUESTED CONTEXT DATA

                // get requested context data (requested by sinks via formatters)

                var isAdditionalContextDataRequested = false;

                var allRequestedContextData = new List<IDataRequest>(capacity: 27);

                for(int sink_ix = 0; sink_ix < allSinks.Length; sink_ix++)
                {
                    var sink = allSinks[sink_ix];

                    var requestedContextData = sink.GetRequestedContextData();

                    for(int cd_ix = 0; cd_ix < requestedContextData.Count; cd_ix++)
                    {
                        var cd = requestedContextData[cd_ix];

                        allRequestedContextData.Add(cd);

                        if (cd.Data == "Event.AdditionalContextData")
                            isAdditionalContextDataRequested = true;
                    }
                }

                allRequestedContextData = allRequestedContextData.Distinct().ToList(); // todo: check if performance of distinct is good enough
                
                //# INCLUDE DATA REQUESTED BY SINKS

                var requestedData = config_ref.ContextDataCollectors.Collect(allRequestedContextData);
                de.Properties.AddOrUpdateRange(requestedData);

                //# GET ADDITIONAL CONTEXT INFORMATION

                if (isAdditionalContextDataRequested)
                {
                    var additionalContextdataCollectors = config_ref.AdditionalContextDataCollectors;

                    for (int i = 0; i < additionalContextdataCollectors.Count; i++)
                    {
                        var collector = additionalContextdataCollectors[i];

                        // if collector has no filter then it should be used.
                        if (collector.Filter == null)
                        {
                            de.AdditionalContextData.AddOrUpdateRange(collector.CollectData());
                            de.PinAdditionalContextDataIfNeeded();
                            break;
                        }

                        if (collector.Filter.Evaluate(de, this.Name))
                        {
                            de.AdditionalContextData.AddOrUpdateRange(collector.CollectData());
                            de.PinAdditionalContextDataIfNeeded();
                            break;
                        }
                    }
                }

                //# WRITE TO SINKS

                if (mustWaitForWriteSinks.Length > 0)
                {
                    if (mustWaitForWriteSinks.Length == 1)
                    {
                        var sink = mustWaitForWriteSinks.Single();

                        sink.Write(de);
                    }
                    else
                    {
                        ParallelExtensions.ForEach(mustWaitForWriteSinks, (sink) =>
                        {
                            sink.Write(de);
                        });
                    }
                }

                // then write to all fire-and-forget sinks

                if (fireAndForgetSinks.Length > 0)
                    ParallelExtensions.ForEachNoWait(
                        fireAndForgetSinks,
                        (sink) =>
                        {
                            if (sink.Filter.Evaluate(de, Name))
                                sink.Write(de);
                        });
            }
            catch (Exception ex)
            {
                Diagnostics.Value.Error(ex, "Failed to log diagnostic event.");
            }
        }
        
        /// <summary>
        /// Checks if specified Diagnostic Event meets filtering criteria.
        /// </summary>
        /// <param name="de"></param>
        /// <param name="loggerName"></param>
        /// <param name="filters"></param>
        /// <returns></returns>
        internal bool MeetsFiltersCriteria(IDiagnosticEvent de, ILoggerName loggerName, IFilterCollection filters)
        {
            //# check this loger's filters
            for (int i = 0; i < filters.Count; i++)
            {
                var filter = filters[i];

                bool shouldLog = filter.Evaluate(de, loggerName);

                if (!shouldLog)
                    return false;
            }

            var parentLogger = Parent as DiagnosticLogger;

            if (parentLogger != null)
                return parentLogger.MeetsFiltersCriteria(de, loggerName, filters);

            return true;
        }

        //public void Configure(Configuration.IConfigurationProvider configProvider, bool merge = false)
        //{
        //    if (merge)
        //    {
        //        var configToMerge = configProvider.LoadConfiguration();
        //        var currentConfig = GetConfiguration();

        //        // todo: merge all

        //        foreach (var filter in configToMerge.GlobalFilters)
        //        {
        //            if (filter is LazyFilterReference)
        //            {
        //                var resolvedFilter =
        //                    (from f in currentConfig.FilterDefinitions
        //                     where f.Name == filter.Name
        //                     select f).FirstOrDefault();

        //                currentConfig.GlobalFilters.Add(resolvedFilter);
        //            }
        //        }

        //        foreach (var sink in configToMerge.SinkDefinitions)
        //        {
        //            if (sink is EmailSink)
        //                ((EmailSink)sink).BodyFormatter = currentConfig.Formatters.Where(f => f.Name == "generic").FirstOrDefault();

        //            if (sink.Filter != null && sink.Filter is LazyFilterReference)
        //            {
        //                sink.Filter =
        //                    (from f in currentConfig.FilterDefinitions
        //                     where f.Name == sink.Filter.Name
        //                     select f).FirstOrDefault();
        //            }

        //            currentConfig.SinkDefinitions.Add(sink);
        //        }

        //        foreach (var sink in configToMerge.Sinks)
        //            currentConfig.Sinks.Add(sink);

        //        Configure(currentConfig);
        //    }
        //    else
        //    {
        //        Configure(configProvider.LoadConfiguration());
        //    }
        //}
        //public void Configure(DiagnosticsConfiguration config)
        //{
        //    config.LogConfigCache = new ConfigurationCache();

        //    config.SinkDefinitions.RefreshCache();

        //    foreach (var sink in config.SinkDefinitions)
        //        sink.Initialize();

        //    config.Sinks.RefreshCache();

        //    //# get log levels enabled states
        //    var de = new DiagnosticEvent();
        //    de.EvaluateAndPinAllDataIfNeeded();

        //    de.Severity = SeverityLevels.Critical;
        //    config.LogConfigCache.ShouldProcessCriticals = MeetsFiltersCriteria(de, Name, config);

        //    de.Severity = SeverityLevels.Error;
        //    config.LogConfigCache.ShouldProcessErrors = MeetsFiltersCriteria(de, Name, config);

        //    de.Severity = SeverityLevels.Warning;
        //    config.LogConfigCache.ShouldProcessWarnings = MeetsFiltersCriteria(de, Name, config);

        //    de.Severity = SeverityLevels.Information;
        //    config.LogConfigCache.ShouldProcessInformation = MeetsFiltersCriteria(de, Name, config);

        //    de.Severity = SeverityLevels.Verbose;
        //    config.LogConfigCache.ShouldProcessVerbose = MeetsFiltersCriteria(de, Name, config);

        //    //# get raw messages enabled states
        //    de.Severity = SeverityLevels.Maximum;
        //    de.HasRawMessage = true;
        //    config.LogConfigCache.ShouldProcessRawMessages = MeetsFiltersCriteria(de, Name, config);

        //    //# replace current config with new config
        //    //NOTE: we are setting some values on newCache, make sure that there's a memory barrier in place
        //    Thread.MemoryBarrier();
        //    Config = config;
        //}

        //public DiagnosticsConfiguration GetConfiguration()
        //{
        //    // make a copy of reference to current configuration
        //    // to make sure that any changes (which would replace Config)
        //    // will not be applied to this method before it exits
        //    var config_ref = Config;

        //    if (config_ref == null)
        //        return new DiagnosticsConfiguration();

        //    var config = new DiagnosticsConfiguration();

        //    // TODO: all should be cloned, not copied
        //    // TODO: remember that some objects store references to other objects, those should be preserved (reassignment 

        //    //# Sinks

        //    config.SinkDefinitions = new SinkCollection(config_ref.SinkDefinitions.ToArray());
        //    config.Sinks = config_ref.Sinks;

        //    //# Fiters

        //    config.FilterDefinitions = config_ref.FilterDefinitions;

        //    config.GlobalFilters = config_ref.GlobalFilters;

        //    config.Formatters = new FormatterCollection(config_ref.Formatters.ToArray());
        //    config.SeverityLevels = new SeverityLevelCollection(config_ref.SeverityLevels.ToArray());

        //    //# Context Data Collectors

        //    if (config_ref.GlobalContextDataCollectors != null)
        //        config.GlobalContextDataCollectors = config_ref.GlobalContextDataCollectors.Clone();

        //    config.AdditionalContextDataCollectors = config_ref.AdditionalContextDataCollectors.Clone();

        //    return config;
        //}
    }
}
