using SquaredInfinity.Foundation.ContextDataCollectors;
using SquaredInfinity.Foundation.Diagnostics;
using SquaredInfinity.Foundation.Diagnostics.ContextDataCollectors;
using SquaredInfinity.Foundation.Diagnostics.Filters;
using SquaredInfinity.Foundation.Diagnostics.Formatters;
using SquaredInfinity.Foundation.Diagnostics.Sinks.File;
using SquaredInfinity.Foundation.Extensions;
using SquaredInfinity.Foundation.Serialization.FlexiXml;
using SquaredInfinity.Foundation.Types.Description.Reflection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Foundation.Diagnostics.Walkthrough.Common
{
    class Program
    {
        static void Main(string[] args)
        {
            //#     DiagnosticLogger can be used quickly without any configuration.
            //      By default diagnostic data will be logged to "Logs" directory.

            DiagnosticLogger.Global.Information("This required no configuration at all.");

            //#     Custom configuration can be applied from xml file

//            var config = SquaredInfinity.Foundation.Diagnostics.Configuration.XmlConfigurationProvider


            var config = DiagnosticLogger.Global.GetConfigurationClone();

            //# add default filters

            // exclude everything
            config.Repository.Filters.Add(new LoggerNameFilter { Name = "exclude-all", Mode = FilterMode.Exclude });

            // include everything
            config.Repository.Filters.Add(new LoggerNameFilter { Name = "include-all", Mode = FilterMode.Include });

            // Include messages with specific Severity Level
            config.Repository.Filters.Add(new SeverityFilter { Name = "critical", Mode = FilterMode.Include, Severity = KnownSeverityLevels.Critical });
            config.Repository.Filters.Add(new SeverityFilter { Name = "error", Mode = FilterMode.Include, Severity = KnownSeverityLevels.Error });
            config.Repository.Filters.Add(new SeverityFilter { Name = "warning", Mode = FilterMode.Include, Severity = KnownSeverityLevels.Warning });
            config.Repository.Filters.Add(new SeverityFilter { Name = "information", Mode = FilterMode.Include, Severity = KnownSeverityLevels.Information });
            config.Repository.Filters.Add(new SeverityFilter { Name = "verbose", Mode = FilterMode.Include, Severity = KnownSeverityLevels.Verbose });

            // Exclude messages with specific Severity Level
            config.Repository.Filters.Add(new SeverityFilter { Name = "!critical", Mode = FilterMode.Exclude, Severity = KnownSeverityLevels.Critical });
            config.Repository.Filters.Add(new SeverityFilter { Name = "!error", Mode = FilterMode.Exclude, Severity = KnownSeverityLevels.Error });
            config.Repository.Filters.Add(new SeverityFilter { Name = "!warning", Mode = FilterMode.Exclude, Severity = KnownSeverityLevels.Warning });
            config.Repository.Filters.Add(new SeverityFilter { Name = "!information", Mode = FilterMode.Exclude, Severity = KnownSeverityLevels.Information });
            config.Repository.Filters.Add(new SeverityFilter { Name = "!verbose", Mode = FilterMode.Exclude, Severity = KnownSeverityLevels.Verbose });

            //
            //      Include only messages from specific Severity Level ranges
            //      Naming Convention:
            //        [a,b] - closed interval from a (included) to b (included)
            //        (a,b] - left half-open interval from a (excluded) to b (included)
            //        [a,b) - right half-open interval from a (included) to b (excluded)
            //        (a,b) - open interval from a (excluded) to b (excluded)

            config.Repository.Filters.Add(new SeverityFilter { Name = "[warning,error]", Mode = FilterMode.Include, From = KnownSeverityLevels.Warning, To = KnownSeverityLevels.Error });
            config.Repository.Filters.Add(new SeverityFilter { Name = "[warning,error)", Mode = FilterMode.Include, From = KnownSeverityLevels.Error, ExclusiveTo = KnownSeverityLevels.Error });

            config.Repository.Filters.Add(new SeverityFilter { Name = "(warning,max]", Mode = FilterMode.Include, ExclusiveFrom = KnownSeverityLevels.Warning });
            config.Repository.Filters.Add(new SeverityFilter { Name = "[warning,max]", Mode = FilterMode.Include, From = KnownSeverityLevels.Warning });

            config.Repository.Filters.Add(new SeverityFilter { Name = "(information,max]", Mode = FilterMode.Include, ExclusiveFrom = KnownSeverityLevels.Information });
            config.Repository.Filters.Add(new SeverityFilter { Name = "[information,max]", Mode = FilterMode.Include, From = KnownSeverityLevels.Information });

            config.Repository.Filters.Add(new SeverityFilter { Name = "(verbose,max]", Mode = FilterMode.Include, ExclusiveFrom = KnownSeverityLevels.Verbose });
            config.Repository.Filters.Add(new SeverityFilter { Name = "[verbose,max]", Mode = FilterMode.Include, From = KnownSeverityLevels.Verbose });

            config.Repository.Filters.Add(new SeverityFilter { Name = "[min,max]", Mode = FilterMode.Include, From = KnownSeverityLevels.Minimum });

            //      Include messages based on property values
            config.Repository.Filters.Add(new PropertyFilter { Name = "contains-raw-message", Mode = FilterMode.Include, Property = "Event.HasRawMessage", Value = "true" });
            config.Repository.Filters.Add(new PropertyFilter { Name = "!contains-raw-message", Mode = FilterMode.Exclude, Property = "Event.HasRawMessage", Value = "true" });

            // Filter events based on event source!!
            // messages logged using System.Diagnostics namespace (e.g. Trace.WriteLine)
            config.Repository.Filters.Add(new PropertyFilter { Name = "system.diagnostics.trace", Mode = FilterMode.Include, Property = "Event.Source", Value = "System.Diagnostics.Trace" });
            config.Repository.Filters.Add(new PropertyFilter { Name = "system.diagnostics.debug", Mode = FilterMode.Include, Property = "Event.Source", Value = "System.Diagnostics.Debug" });

            config.Repository.Filters.Add(new PropertyFilter { Name = "system.diagnostics", Mode = FilterMode.Include, Property = "Event.Source", Value = "System.Diagnostics" });
            config.Repository.Filters.Add(new PropertyFilter { Name = "!system.diagnostics", Mode = FilterMode.Exclude, Property = "Event.Source", Value = "System.Diagnostics" });

            // internal
            config.Repository.Filters.Add(new PropertyFilter { Name = "internal", Mode = FilterMode.Include, Property = "Event.Source", Value = "SquaredInfinity.Foundation.Diagnostics.*" });
            config.Repository.Filters.Add(new PropertyFilter { Name = "!internal", Mode = FilterMode.Exclude, Property = "Event.Source", Value = "SquaredInfinity.Foundation.Diagnostics.*" });

            //    Filters based on event category
            config.Repository.Filters.Add(new PropertyFilter { Name = "application-lifecycle", Mode = FilterMode.Include, Property = "Event.Category", Value = "Application-Lifecycle.*" });
            config.Repository.Filters.Add(new PropertyFilter { Name = "!application-lifecycle", Mode = FilterMode.Exclude, Property = "Event.Category", Value = "Application-Lifecycle.*" });

            config.Repository.Filters.Add(new PropertyFilter { Name = "application-lifecycle.install", Mode = FilterMode.Include, Property = "Event.Category", Value = "Application-Lifecycle.Install" });
            config.Repository.Filters.Add(new PropertyFilter { Name = "!application-lifecycle.install", Mode = FilterMode.Exclude, Property = "Event.Category", Value = "Application-Lifecycle.Install" });

            // add common data collectors
            // those will be used if referenced from sinks
            var edc = new EnvironmentDataCollector();
            config.ContextDataCollectors.Add(edc);
           
            // add additional data collectors
            // those will be used if specific event occurs

            var edc_application_lifecycle = new EnvironmentDataCollector();
            //edc_application_lifecycle.Filter = config.Repository.Filters
            edc_application_lifecycle.RequestedData.Add(EnvironmentData.ApplicationVersion);
            edc_application_lifecycle.RequestedData.Add(EnvironmentData.ThreadUICulture);
            edc_application_lifecycle.RequestedData.Add(EnvironmentData.ThreadCulture);
            edc_application_lifecycle.RequestedData.Add(EnvironmentData.EnvironmentUserDomainName);
            edc_application_lifecycle.RequestedData.Add(EnvironmentData.EnvironmentUserName);
            edc_application_lifecycle.RequestedData.Add(EnvironmentData.EnvironmentMachineName);
            edc_application_lifecycle.RequestedData.Add(EnvironmentData.EnvironmentOsVersion);
            edc_application_lifecycle.RequestedData.Add(EnvironmentData.EnvironmentOsVersionPlatform);
            edc_application_lifecycle.RequestedData.Add(EnvironmentData.EnvironmentOsVersionVersion);
            edc_application_lifecycle.RequestedData.Add(EnvironmentData.EnvironmentOsVersionServicePack);
            edc_application_lifecycle.RequestedData.Add(EnvironmentData.EnvironmentVersion);
            edc_application_lifecycle.RequestedData.Add(EnvironmentData.EnvironmentCurrentDirectory);
            edc_application_lifecycle.RequestedData.Add(EnvironmentData.EnvironmentSystemDirectory);
            edc_application_lifecycle.RequestedData.Add(EnvironmentData.EnvironmentIs64BitOperatingSystem);
            edc_application_lifecycle.RequestedData.Add(EnvironmentData.EnvironmentIs64BitProcess);
            edc_application_lifecycle.RequestedData.Add(EnvironmentData.EnvironmentProcessorCount);
            edc_application_lifecycle.RequestedData.Add(EnvironmentData.EnvironmentCommandLineArgs);
            edc_application_lifecycle.RequestedData.Add(EnvironmentData.EnvironmentUserInteractive);
            edc_application_lifecycle.RequestedData.Add(EnvironmentData.DeploymentIsNetworkDeplyed);
            edc_application_lifecycle.RequestedData.Add(EnvironmentData.DeploymentActivationUri);
            edc_application_lifecycle.RequestedData.Add(EnvironmentData.DeploymentCurrentVersion);
            edc_application_lifecycle.RequestedData.Add(EnvironmentData.DeploymentDataDirectory);
            edc_application_lifecycle.RequestedData.Add(EnvironmentData.DeploymentIsFirstRun);
            edc_application_lifecycle.RequestedData.Add(EnvironmentData.DeploymentTimeOfLastUpdateCheck);
            edc_application_lifecycle.RequestedData.Add(EnvironmentData.DeploymentUpdatedApplicationFullName);
            edc_application_lifecycle.RequestedData.Add(EnvironmentData.DeploymentUpdatedVersion);
            edc_application_lifecycle.RequestedData.Add(EnvironmentData.DeploymentUpdateLocation);

            config.AdditionalContextDataCollectors.Add(edc_application_lifecycle);

            var fileSink = new FileSink();
            fileSink.FileNamePattern = "log.txt";
            fileSink.FileLocationPattern = @"{AppDomain.CurrentDomain.BaseDirectory}\Logs\";
            fileSink.Formatter = new PatternFormatter() { Pattern = 
            @"
        {NewLine}
            --
        {NewLine}
        [{Event.Level}{?Event.HasCategory=>'\:'}{?Event.HasCategory=>Event.Category} thread:{Thread.Id}] {?Event.HasMessage=>Event.Message}
        {?Event.HasLoggerName=>NewLine}
        {?Event.HasLoggerName=>'Logger\:'} {?Event.HasLoggerName=>Event.LoggerName}
        {?Event.HasCallerInfo=>NewLine}
        {?Event.HasCallerInfo=>'Caller\:'}
        {?Event.HasCallerInfo=>Caller.FullName} {?Event.HasCallerInfo=>Caller.File} {?Event.HasCallerInfo=>'\:'} {?Event.HasCallerInfo=>Caller.LineNumber}
        {?Event.HasException=>NewLine}
        {?Event.HasException=>Event.Exception:dumpWithHeader('Exception')}
        {?Event.HasAdditionalContextData=>NewLine}
        {?Event.HasAdditionalContextData=>Event.AdditionalContextData:dumpWithHeader('Context Data')}
        {?Event.HasAttachedObjects=>NewLine}
        {?Event.HasAttachedObjects=>Event.AttachedObjects:dumpWithHeader('Attached Objects')}"};


            config.Sinks.Add(fileSink);

            var serializer = new FlexiXmlSerializer(new ReflectionBasedTypeDescriptor());

            serializer.GetOrCreateTypeSerializationStrategy<Filter>()
                .IgnoreAllMembers()
                .SerializeMember(x => x.Name, x => x.Name != null)
                .SerializeMember(x => x.Mode, x => x.Mode != null);

            serializer.GetOrCreateTypeSerializationStrategy<LoggerNameFilter>()
                .IgnoreAllMembers()
                .CopySerializationSetupFromBaseClass()
                .SerializeMember(x => x.Pattern, x => x.Pattern != null);

            serializer.GetOrCreateTypeSerializationStrategy<PropertyFilter>()
                .IgnoreAllMembers()
                .CopySerializationSetupFromBaseClass()
                .SerializeMember(x => x.Property, x => x.Property != null)
                .SerializeMember(x => x.Value, x => x.Value != null);

            serializer.GetOrCreateTypeSerializationStrategy<SeverityFilter>()
                .IgnoreAllMembers()
                .CopySerializationSetupFromBaseClass()
                .SerializeMember(x => x.Severity, x => x.Severity != null) // todo SerializeMember as specific value, read back
                //.SerializeMember(x => x.Severity, x => x.Severity != null, x => x.Severity.ToString(), s => KnownSeverityLevels.Parse(s))
                .SerializeMember(x => x.To, x => x.To != null)
                .SerializeMember(x => x.From, x => x.From != null)
                .SerializeMember(x => x.ExclusiveTo, x => x.ExclusiveTo != null)
                .SerializeMember(x => x.ExclusiveFrom, x => x.ExclusiveFrom != null);


            var so = new SerializationOptions();
            so.SerializeNonPublicTypes = false;
            so.TypeInformation = TypeInformation.LookupOnly;



            var xml = serializer.Serialize(config, so);
            File.WriteAllText("1.config", xml.ToString());

            DiagnosticLogger.Global.ApplyConfiguration(config);

            DiagnosticLogger.Global.Information("Let's start!");
            DiagnosticLogger.Global.Warning("Let's start!");
            DiagnosticLogger.Global.Critical("Let's start!");

            DiagnosticLogger.Global
                .AsInformation()
                .ApplicationLifecycleEvent
                .Startup();


            DiagnosticLogger.Global
                .AsInformation()
                .ApplicationLifecycleEvent
                .Shutdown();
        }
    }
}
