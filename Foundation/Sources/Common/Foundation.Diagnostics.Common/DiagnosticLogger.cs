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
    public partial class DiagnosticLogger : ILogger, ILoggerImplementation, IHideObjectInheritedMembers
    {
        Lazy<ILogger> Diagnostics = new Lazy<ILogger>(() => new InternalDiagnosticLogger("SquaredInfinity.Diagnostics.DiagnosticLogger"));

        public ILoggerName Name { get; set; }
        public ILoggerImplementation Parent { get; set; }

        public ILogger Log
        {
            get { return (ILogger)this; }
        }
        //public ILoggerConfigProvider Configuration
        //{
        //    get { return (ILoggerConfigProvider)this; }
        //}
        
        public DiagnosticsConfiguration GetConfigurationClone()
        {
            return Config.DeepClone();
        }

        public void ApplyConfiguration(DiagnosticsConfiguration newConfiguration)
        {
            var clone = newConfiguration.DeepClone();

            this.Config = clone;
        }

        DiagnosticsConfiguration Config { get; set; }
        
        ConfigurationCache ConfigCache = new ConfigurationCache();

        DiagnosticEventPropertyCollection _globalProperties = new DiagnosticEventPropertyCollection();
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

        public Exception LogAndReplace(Exception ex)
        {
            throw new NotImplementedException();
        }

        public void ProcessAuditEvent(DiagnosticEvent de)
        {
            //if (de == null)
            //    return;

            //// TODO: read config specific to Audit
            //var config_ref = Volatile.Read(ref TraceConfigCache);

            //ProcessDiagnosticEvent(de, config_ref);
        }

        void IncludeCallerInformation(IDiagnosticEvent de, bool useCallerNameAsLoggerName, bool includeCallerInfo)
        {
            StackFrame callerFrame = null;
            MethodBase callerMethod = null;

            GetCallerInfo(ref callerFrame, ref callerMethod);

            if (useCallerNameAsLoggerName)
            {
                de.LoggerName = callerMethod.DeclaringType.FullName + "." + callerMethod.Name;
            }

            if (includeCallerInfo)
            {
                de.Properties.AddOrUpdate("Event.HasCallerInfo", true);

                de.Properties.AddOrUpdate("Caller.TypeName", callerMethod.DeclaringType.FullName);
                de.Properties.AddOrUpdate("Caller.MemberName", callerMethod.Name);
                de.Properties.AddOrUpdate("Caller.FullName", callerMethod.DeclaringType.FullName + "." + callerMethod.Name);
                de.Properties.AddOrUpdate("Caller.File", callerFrame.GetFileName());
                de.Properties.AddOrUpdate("Caller.LineNumber", callerFrame.GetFileLineNumber());
                de.Properties.AddOrUpdate("Caller.Column", callerFrame.GetFileColumnNumber());
                de.Properties.AddOrUpdate("Caller.ILOffset", callerFrame.GetILOffset());
                de.Properties.AddOrUpdate("Caller.NativeOffset", callerFrame.GetNativeOffset());
            }
        }

        static void GetCallerInfo(ref StackFrame callerFrame, ref MethodBase callerMethod)
        {
            var st = new StackTrace(1, fNeedFileInfo: true);

            int frameOffset = 0;

            do
            {
                callerFrame = st.GetFrame(frameOffset++);
                callerMethod = callerFrame.GetMethod();
            }
            while (callerFrame != null && callerMethod != null && callerMethod.DeclaringType != null && callerMethod.DeclaringType.Namespace.StartsWith("SquaredInfinity.Foundation.Diagnostics"));
        }

        public virtual void ProcessDiagnosticEvent(IDiagnosticEvent de)
        {
            // make a copy of reference to current configuration
            // to make sure that any changes (which would replace Config)
            // will not be applied to this method before it exits
            var config_ref = Config;

            ProcessDiagnosticEventInternal(de, config_ref);
        }

        private void ProcessDiagnosticEventInternal(IDiagnosticEvent de, DiagnosticsConfiguration config_ref)
        {
            // NOTE:    This method is *performance critical* and should be optimized for quickest execution even at cost of readability
            //          This method is static because it should not use any instance members of DiagnosticLogger

            try
            {
                //if (config_ref.GlobalSettings.IncludeCallerInfo || config_ref.GlobalSettings.UseCallerNameAsLoggerName)
                //{
                //    IncludeCallerInformation(
                //        de,
                //        config_ref.GlobalSettings.UseCallerNameAsLoggerName,
                //        config_ref.GlobalSettings.IncludeCallerInfo);
                //}

                // override logger name only if it has not already been set (custom name might have been used)
                if (de.LoggerName == null)
                    de.LoggerName = Name.ToString();

                // todo: potential performance issue?
                foreach (var p in GlobalProperties)
                    de.Properties.AddOrUpdate(p.UniqueName, p.Value);

                //# EVALUATE AND PIN DIAGNOSTIC EVENT DATA
                de.EvaluateAndPinAllDataIfNeeded();

                //# GLOBAL FILTER
                if (config_ref.GlobalFilters.Count > 0 && !MeetsFiltersCriteria(de, Name, config_ref.Filters))
                    return;

                //# GET SINKS

                var mustWaitForWriteSinks =
                    (from s in config_ref.Sinks.MustWaitForWriteSinks
                     where s.Filter.Evaluate(de, Name)
                     select s).ToArray();

                var fireAndForgetSinks =
                    (from s in config_ref.Sinks.FireAndForgetRawMessageSinks
                     where s.Filter.Evaluate(de, Name)
                     select s).ToArray();

                var allSinks = mustWaitForWriteSinks.Concat(fireAndForgetSinks).ToArray();


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
                        Parallel.ForEach(mustWaitForWriteSinks, (sink) =>
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
        
        public bool MeetsFiltersCriteria(IDiagnosticEvent de, ILoggerName loggerName, IFilterCollection filters)
        {
            //# check this loger's filters
            for (int i = 0; i < filters.Count; i++)
            {
                var filter = filters[i];

                bool shouldLog = filter.Evaluate(de, loggerName);

                if (!shouldLog)
                    return false;
            }

            //# check parent loger's filter TODO
            if (Parent != null)
                return Parent.MeetsFiltersCriteria(de, loggerName, filters);

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


        public IReadOnlyList<ISinkLocation> GetLogLocations()
        {
            //List<ISinkLocation> result = new List<ISinkLocation>();

            //// make a copy of reference to current configuration
            //// to make sure that any changes (which would replace Config)
            //// will not be applied to this method before it exits
            //var config_ref = Config;

            //for (int i = 0; i < config_ref.SinkDefinitions.Count; i++)
            //{
            //    var sink = config_ref.SinkDefinitions[i];

            //    result.Add(sink.SinkLocation);
            //}

            //return result;


            return null;
        }
    }
}
