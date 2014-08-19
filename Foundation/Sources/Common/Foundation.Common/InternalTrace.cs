using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SquaredInfinity.Foundation.Extensions;
using System.Runtime.InteropServices;

namespace SquaredInfinity.Foundation
{
    internal partial class InternalTrace
    {
        public static readonly SourceSwitch Switch;

        static InternalTrace()
        {
#if DEBUG
            Switch = new SourceSwitch(Name, "Information");
#else
            Switch = new SourceSwitch(Name, "Error");
#endif
        }

        #region Error

        public static void Error(Exception ex, Func<string> getMessage)
        {
            if (!Switch.ShouldTrace(TraceEventType.Error))
                return;

            ProcessDiagnosticEvent(TraceEventType.Error, getMessage);
        }

        public static void Error(Exception ex, string message)
        {
            if (!Switch.ShouldTrace(TraceEventType.Error))
                return;

            ProcessDiagnosticEvent(TraceEventType.Error, message);
        }

        public static void Error(string message)
        {
            if (!Switch.ShouldTrace(TraceEventType.Error))
                return;

            ProcessDiagnosticEvent(TraceEventType.Error, message);
        }


        public static void Error(Func<string> getMessage)
        {
            if (!Switch.ShouldTrace(TraceEventType.Error))
                return;

            ProcessDiagnosticEvent(TraceEventType.Error, getMessage);
        }

        #endregion

        #region Warning

        public static void Warning(Exception ex, Func<string> getMessage)
        {
            if (!Switch.ShouldTrace(TraceEventType.Warning))
                return;

            ProcessDiagnosticEvent(TraceEventType.Warning, getMessage);
        }

        public static void Warning(Exception ex, string message)
        {
            if (!Switch.ShouldTrace(TraceEventType.Warning))
                return;

            ProcessDiagnosticEvent(TraceEventType.Warning, message);
        }

        public static void Warning(string message)
        {
            if (!Switch.ShouldTrace(TraceEventType.Warning))
                return;

            ProcessDiagnosticEvent(TraceEventType.Warning, message);
        }


        public static void Warning(Func<string> getMessage)
        {
            if (!Switch.ShouldTrace(TraceEventType.Warning))
                return;

            ProcessDiagnosticEvent(TraceEventType.Warning, getMessage);
        }

        #endregion

        #region Information

        public static void Information(string message)
        {
            if (!Switch.ShouldTrace(TraceEventType.Information))
                return;

            ProcessDiagnosticEvent(TraceEventType.Information, message);
        }


        public static void Information(Func<string> getMessage)
        {
            if (!Switch.ShouldTrace(TraceEventType.Information))
                return;

            ProcessDiagnosticEvent(TraceEventType.Information, getMessage);
        }

        #endregion

        static void ProcessDiagnosticEvent(TraceEventType type, string message, Exception ex = null)
        {
            var prefix = "";

            if (type == TraceEventType.Information)
                prefix = "INFORMATION: ";
            else if (type == TraceEventType.Warning)
                prefix = "WARNNING: ";
            else if (type == TraceEventType.Error)
                prefix = "ERROR: ";
            else if (type == TraceEventType.Critical)
                prefix = "CRITICAL: ";

            if(string.IsNullOrEmpty(message))
                System.Diagnostics.Trace.WriteLine("[" + prefix + Name + "]: " + "<no message>");
            else
                System.Diagnostics.Trace.WriteLine("[" + prefix + Name + "]: " + message);

            if (ex != null)
                System.Diagnostics.Trace.WriteLine("[" + prefix + Name + "]: " + ex.ToString());
        }
        
        static void ProcessDiagnosticEvent(TraceEventType type, Func<string> getMessage, Exception ex = null)
        {
            try
            {
                var msg = getMessage();

                ProcessDiagnosticEvent(type, msg, ex);
            }
            catch(Exception _ex)
            {
                if(ex != null)
                    ex = new AggregateException(ex, _ex);

                ProcessDiagnosticEvent(TraceEventType.Critical, "Error processing diagnostic event", ex);
            }
        }
    }
}
