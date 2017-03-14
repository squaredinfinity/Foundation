using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

#if CORE
using Trace = System.Diagnostics.Debug;
#endif

//namespace SquaredInfinity.Foundation
//{
//#if CORE
//    internal enum TraceEventType
//    {
//        Critical = 1,
//        Error = 2,
//        Warning = 4,
//        Information = 8,
//        Verbose = 16,
//        Start = 256,
//        Stop = 512,
//        Suspend = 1024,
//        Resume = 2048,
//        Transfer = 4096
//    }

//#endif

//    internal partial class InternalTrace
//    {
//#if !CORE
//        public static readonly SourceSwitch Switch;
//#endif

//        static string Name { get; set; }

//        static InternalTrace()
//        {
//#if !CORE
//            string asmName = typeof(InternalTrace).Assembly.GetName().Name;

//            // assemblies are name using xx.xx.xx.platform pattern
//            // remove last part of the name

//            Name = asmName.Substring(0, asmName.LastIndexOf('.'));

//#if DEBUG
//            Switch = new SourceSwitch(Name, "Information");
//#else
//            Switch = new SourceSwitch(Name, "Warning");
//#endif
//#endif
//        }

//#region Error

//        public static void Error(Exception ex, Func<string> getMessage)
//        {
//#if !CORE
//            if (!Switch.ShouldTrace(TraceEventType.Error))
//                return;
//#endif

//            ProcessDiagnosticEvent(TraceEventType.Error, getMessage);
//        }

//        public static void Error(Exception ex, string message)
//        {
//#if !CORE
//            if (!Switch.ShouldTrace(TraceEventType.Error))
//                return;
//#endif

//            ProcessDiagnosticEvent(TraceEventType.Error, message);
//        }

//        public static void Error(string message)
//        {
//#if !CORE
//            if (!Switch.ShouldTrace(TraceEventType.Error))
//                return;
//#endif

//            ProcessDiagnosticEvent(TraceEventType.Error, message);
//        }


//        public static void Error(Func<string> getMessage)
//        {
//#if !CORE
//            if (!Switch.ShouldTrace(TraceEventType.Error))
//                return;
//#endif

//            ProcessDiagnosticEvent(TraceEventType.Error, getMessage);
//        }

//#endregion

//#region Warning

//        public static void Warning(Exception ex, Func<string> getMessage)
//        {
//#if !CORE
//            if (!Switch.ShouldTrace(TraceEventType.Warning))
//                return;
//#endif

//            ProcessDiagnosticEvent(TraceEventType.Warning, getMessage);
//        }

//        public static void Warning(Exception ex, string message)
//        {
//#if !CORE
//            if (!Switch.ShouldTrace(TraceEventType.Warning))
//                return;
//#endif

//            ProcessDiagnosticEvent(TraceEventType.Warning, message);
//        }

//        public static void Warning(string message)
//        {
//#if !CORE
//            if (!Switch.ShouldTrace(TraceEventType.Warning))
//                return;
//#endif

//            ProcessDiagnosticEvent(TraceEventType.Warning, message);
//        }


//        public static void Warning(Func<string> getMessage)
//        {
//#if !CORE
//            if (!Switch.ShouldTrace(TraceEventType.Warning))
//                return;
//#endif

//            ProcessDiagnosticEvent(TraceEventType.Warning, getMessage);
//        }

//#endregion

//#region Information

//        public static void Information(Exception ex, string message)
//        {
//#if !CORE
//            if (!Switch.ShouldTrace(TraceEventType.Information))
//                return;
//#endif

//            ProcessDiagnosticEvent(TraceEventType.Information, message, ex);
//        }

//        public static void Information(string message)
//        {
//#if !CORE
//            if (!Switch.ShouldTrace(TraceEventType.Information))
//                return;
//#endif

//            ProcessDiagnosticEvent(TraceEventType.Information, message);
//        }


//        public static void Information(Func<string> getMessage)
//        {
//#if !CORE
//            if (!Switch.ShouldTrace(TraceEventType.Information))
//                return;
//#endif

//            ProcessDiagnosticEvent(TraceEventType.Information, getMessage);
//        }

//        #endregion

//        public static void Verbose(string message)
//        {
//#if !CORE
//            if (!Switch.ShouldTrace(TraceEventType.Verbose))
//                return;
//#endif

//            ProcessDiagnosticEvent(TraceEventType.Verbose, message);
//        }

//        public static void Verbose(Func<string> getMessage)
//        {
//#if !CORE
//            if (!Switch.ShouldTrace(TraceEventType.Verbose))
//                return;
//#endif

//            ProcessDiagnosticEvent(TraceEventType.Verbose, getMessage);
//        }

//        static void ProcessDiagnosticEvent(TraceEventType type, string message, Exception ex = null)
//        {
//            var prefix = "";

//            if (type == TraceEventType.Verbose)
//                prefix = "VERBOSE: ";
//            else if (type == TraceEventType.Information)
//                prefix = "INFORMATION: ";
//            else if (type == TraceEventType.Warning)
//                prefix = "WARNNING: ";
//            else if (type == TraceEventType.Error)
//                prefix = "ERROR: ";
//            else if (type == TraceEventType.Critical)
//                prefix = "CRITICAL: ";
            
//            if(string.IsNullOrEmpty(message))
//                Trace.WriteLine("[" + prefix + Name + "]: " + "<no message>");
//            else
//                Trace.WriteLine("[" + prefix + Name + "]: " + message);

//            if (ex != null)
//                Trace.WriteLine("[" + prefix + Name + "]: " + ex.ToString());
//        }
        
//        static void ProcessDiagnosticEvent(TraceEventType type, Func<string> getMessage, Exception ex = null)
//        {
//            try
//            {
//                var msg = getMessage();

//                ProcessDiagnosticEvent(type, msg, ex);
//            }
//            catch(Exception _ex)
//            {
//                if(ex != null)
//                    ex = new AggregateException(ex, _ex);

//                ProcessDiagnosticEvent(TraceEventType.Critical, "Error processing diagnostic event", ex);
//            }
//        }
//    }
//}
