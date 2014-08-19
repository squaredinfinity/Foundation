using SquaredInfinity.Foundation.Diagnostics.Configuration;
using SquaredInfinity.Foundation.Extensions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Foundation.Diagnostics
{
    internal partial class InternalLogger : DiagnosticLogger
    {
        static readonly Sinks.MemorySink CachingLoggerSink = new Sinks.MemorySink();

        public static readonly SourceSwitch InternalLoggerSwitch =
                new SourceSwitch(
                    "SquaredInfinity.Diagnostics",
#if DEBUG
                defaultSwitchValue: "Information");
#else
                defaultSwitchValue: "Error");
#endif
        
        public InternalLogger(string name)
            : base()
        {
            // configure this logger to allaw everything
            var config = new DiagnosticsConfigurationWithCache(new ConfigurationCache(allowAll: true));

            ApplyConfiguration(config);
        }

        public override void ProcessDiagnosticEvent(IDiagnosticEvent de)
        {
            try
            {
                if (InternalLoggerSwitch.Level == SourceLevels.Off)
                    return;

                var prefix = "";

                if (de.Severity == KnownSeverityLevels.Critical)
                {
                    if (!InternalLoggerSwitch.ShouldTrace(TraceEventType.Critical))
                        return;

                    prefix = "CRITICAL: ";
                }
                else if (de.Severity == KnownSeverityLevels.Error)
                {
                    if (!InternalLoggerSwitch.ShouldTrace(TraceEventType.Error))
                        return;

                    prefix = "ERROR: ";
                }
                else if (de.Severity == KnownSeverityLevels.Warning)
                {
                    if (!InternalLoggerSwitch.ShouldTrace(TraceEventType.Warning))
                        return;

                    prefix = "WARNNING: ";
                }
                else if (de.Severity == KnownSeverityLevels.Information)
                {
                    if (!InternalLoggerSwitch.ShouldTrace(TraceEventType.Information))
                        return;

                    prefix = "INFORMATION: ";
                }
                else if (de.Severity == KnownSeverityLevels.Verbose)
                {
                    if (!InternalLoggerSwitch.ShouldTrace(TraceEventType.Verbose))
                        return;
                }

                System.Diagnostics.Trace.WriteLine("[" + prefix + "SquaredInfinity.Diagnostics]: " + de.Message);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.TraceError("[CRITICAL: SquaredInfinity.Diagnostics]: " + ex.ToString());
            }
        }

        internal static ILogger CreateLoggerForThisType()
        {
            return ILoggerExtensions.CreateLoggerForThisType(null, framesToSkip: 1);
        }

        internal static ILogger CreateLoggerForType<TType>()
        {
            return ILoggerExtensions.CreateLoggerForType<TType>(null);
        }

        internal static ILogger CreateLoggerForType(object instance)
        {
            return ILoggerExtensions.CreateLoggerForType(null, instance);
        }
    }
}
