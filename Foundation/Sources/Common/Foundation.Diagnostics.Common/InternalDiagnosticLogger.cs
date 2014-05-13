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
    internal partial class InternalDiagnosticLogger : DiagnosticLogger
    {
        static readonly Sinks.MemorySink CachingLoggerSink = new Sinks.MemorySink();

        public static readonly BooleanSwitch InternalLoggerSwitch =
                new BooleanSwitch(
                    "SquaredInfinity.Foundation.Diagnostics",
                    "SquaredInfinity.Foundation.Diagnostics",
#if DEBUG
                defaultSwitchValue: "true");
#else
                defaultSwitchValue: "false");
#endif
        
        public InternalDiagnosticLogger(string name)
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
                if (!InternalLoggerSwitch.Enabled)
                    return;

                var prefix = "";

                if (de.Severity == KnownSeverityLevels.Information)
                    prefix = "INFORMATION: ";
                else if (de.Severity == KnownSeverityLevels.Warning)
                    prefix = "WARNNING: ";
                else if (de.Severity == KnownSeverityLevels.Error)
                    prefix = "ERROR: ";

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
