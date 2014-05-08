using SquaredInfinity.Foundation.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Foundation.Diagnostics
{
    internal partial class InternalDiagnosticLogger : DiagnosticLogger
    {
        static readonly Sinks.MemorySink CachingLoggerSink = new Sinks.MemorySink();
        
        public InternalDiagnosticLogger(string name)
            : base()
        {
            //Config = new DiagnosticsConfiguration();
            //Config.LogConfigCache = new ConfigurationCache(allowAll: true);
        }

        public override void ProcessDiagnosticEvent(IDiagnosticEvent de)
        {
            try
            {
                System.Diagnostics.Trace.WriteLine("[SquaredInfinity.Diagnostics]: " + de.Message);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.TraceError("[SquaredInfinity.Diagnostics]: " + ex.ToString());
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
