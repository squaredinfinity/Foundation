using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SquaredInfinity.Foundation.Extensions;

namespace SquaredInfinity.Foundation.Diagnostics
{
    /// <summary>
    /// Trace Logger writes messages to Sytstem.Diagnostics.Trace output.
    /// It's behavior can be modified using app.config or web.config
    /// </summary>
    internal class InternalLogger : ILogger
    {
        public readonly SourceSwitch InternalLoggerSwitch;

        public InternalLogger(string name)
        {
            new SourceSwitch(
                name,
#if DEBUG
                defaultSwitchValue: "Information");
#else
                defaultSwitchValue: "Error");
#endif
        }

        public void Critical(Exception ex, string message = null)
        {
            if (!InternalLoggerSwitch.ShouldTrace(TraceEventType.Critical))
                return;

            var ev = new InternalDiagnosticEvent();
            ev.Message = message;
            ev.ExceptionObject = ex;
            ev.Severity = KnownSeverityLevels.Critical;

            ProcessDiagnosticEvent(ev);
        }

        public void CriticalFormat(Exception ex, string message, params object[] messageArgs)
        {
            if (!InternalLoggerSwitch.ShouldTrace(TraceEventType.Critical))
                return;

            var ev = new InternalDiagnosticEvent();
            ev.Message = message.FormatWith(messageArgs);
            ev.ExceptionObject = ex;
            ev.Severity = KnownSeverityLevels.Critical;

            ProcessDiagnosticEvent(ev);
        }

        public void Critical(Exception ex, Func<string> getMessage)
        {
            if (!InternalLoggerSwitch.ShouldTrace(TraceEventType.Critical))
                return;

            var ev = new InternalDiagnosticEvent();
            ev.Message = getMessage();
            ev.ExceptionObject = ex;
            ev.Severity = KnownSeverityLevels.Critical;

            ProcessDiagnosticEvent(ev);
        }

        public void Critical(Exception ex, Func<string> getMessage, params object[] attachedObjects)
        {
            throw new NotSupportedException();
        }

        public void Critical(string message)
        {
            if (!InternalLoggerSwitch.ShouldTrace(TraceEventType.Critical))
                return;

            var ev = new InternalDiagnosticEvent();
            ev.Message = message;
            ev.Severity = KnownSeverityLevels.Critical;

            ProcessDiagnosticEvent(ev);
        }

        public void CriticalFormat(string message, params object[] messageArgs)
        {
            if (!InternalLoggerSwitch.ShouldTrace(TraceEventType.Critical))
                return;

            var ev = new InternalDiagnosticEvent();
            ev.Message = message.FormatWith(messageArgs);
            ev.Severity = KnownSeverityLevels.Critical;

            ProcessDiagnosticEvent(ev);
        }

        public void Critical(Func<string> getMessage)
        {
            if (!InternalLoggerSwitch.ShouldTrace(TraceEventType.Critical))
                return;

            var ev = new InternalDiagnosticEvent();
            ev.Message = getMessage();
            ev.Severity = KnownSeverityLevels.Critical;

            ProcessDiagnosticEvent(ev);
        }

        public void Critical(Func<string> getMessage, params object[] attachedObjects)
        {
            throw new NotSupportedException();
        }

        public void Error(Exception ex, string message = null)
        {
            if (!InternalLoggerSwitch.ShouldTrace(TraceEventType.Error))
                return;

            var ev = new InternalDiagnosticEvent();
            ev.ExceptionObject = ex;
            ev.Message = message;
            ev.Severity = KnownSeverityLevels.Error;

            ProcessDiagnosticEvent(ev);
        }

        public void ErrorFormat(Exception ex, string message, params object[] messageArgs)
        {
            if (!InternalLoggerSwitch.ShouldTrace(TraceEventType.Error))
                return;

            var ev = new InternalDiagnosticEvent();
            ev.ExceptionObject = ex;
            ev.Message = message.FormatWith(messageArgs);
            ev.Severity = KnownSeverityLevels.Error;

            ProcessDiagnosticEvent(ev);
        }

        public void Error(Exception ex, Func<string> getMessage)
        {
            if (!InternalLoggerSwitch.ShouldTrace(TraceEventType.Error))
                return;

            var ev = new InternalDiagnosticEvent();
            ev.ExceptionObject = ex;
            ev.Message = getMessage();
            ev.Severity = KnownSeverityLevels.Error;

            ProcessDiagnosticEvent(ev);
        }

        public void Error(Exception ex, Func<string> getMessage, params object[] attachedObjects)
        {
            throw new NotSupportedException();
        }

        public void Error(string message)
        {
            if (!InternalLoggerSwitch.ShouldTrace(TraceEventType.Error))
                return;

            var ev = new InternalDiagnosticEvent();
            ev.Message = message;
            ev.Severity = KnownSeverityLevels.Error;

            ProcessDiagnosticEvent(ev);
        }

        public void ErrorFormat(string message, params object[] messageArgs)
        {
            if (!InternalLoggerSwitch.ShouldTrace(TraceEventType.Error))
                return;

            var ev = new InternalDiagnosticEvent();
            ev.Message = message.FormatWith(messageArgs);
            ev.Severity = KnownSeverityLevels.Error;

            ProcessDiagnosticEvent(ev);
        }

        public void Error(Func<string> getMessage)
        {
            if (!InternalLoggerSwitch.ShouldTrace(TraceEventType.Error))
                return;

            var ev = new InternalDiagnosticEvent();
            ev.Message = getMessage();
            ev.Severity = KnownSeverityLevels.Error;

            ProcessDiagnosticEvent(ev);
        }

        public void Error(Func<string> getMessage, params object[] attachedObjects)
        {
            throw new NotSupportedException();
        }

        public void Warning(Exception ex, string message = null)
        {
            if (!InternalLoggerSwitch.ShouldTrace(TraceEventType.Warning))
                return;

            var ev = new InternalDiagnosticEvent();
            ev.ExceptionObject = ex;
            ev.Message = message;
            ev.Severity = KnownSeverityLevels.Warning;

            ProcessDiagnosticEvent(ev);
        }

        public void WarningFormat(Exception ex, string message, params object[] messageArgs)
        {
            if (!InternalLoggerSwitch.ShouldTrace(TraceEventType.Warning))
                return;

            var ev = new InternalDiagnosticEvent();
            ev.ExceptionObject = ex;
            ev.Message = message.FormatWith(messageArgs);
            ev.Severity = KnownSeverityLevels.Warning;

            ProcessDiagnosticEvent(ev);
        }

        public void Warning(Exception ex, Func<string> getMessage)
        {
            if (!InternalLoggerSwitch.ShouldTrace(TraceEventType.Warning))
                return;

            var ev = new InternalDiagnosticEvent();
            ev.ExceptionObject = ex;
            ev.Message = getMessage();
            ev.Severity = KnownSeverityLevels.Warning;

            ProcessDiagnosticEvent(ev);
        }

        public void Warning(Exception ex, Func<string> getMessage, params object[] attachedObjects)
        {
            throw new NotSupportedException();
        }

        public void Warning(string message)
        {
            if (!InternalLoggerSwitch.ShouldTrace(TraceEventType.Warning))
                return;

            var ev = new InternalDiagnosticEvent();
            ev.Message = message;
            ev.Severity = KnownSeverityLevels.Warning;

            ProcessDiagnosticEvent(ev);
        }

        public void WarningFormat(string message, params object[] messageArgs)
        {
            if (!InternalLoggerSwitch.ShouldTrace(TraceEventType.Warning))
                return;

            var ev = new InternalDiagnosticEvent();
            ev.Message = message.FormatWith(messageArgs);
            ev.Severity = KnownSeverityLevels.Warning;

            ProcessDiagnosticEvent(ev);
        }

        public void Warning(Func<string> getMessage)
        {
            if (!InternalLoggerSwitch.ShouldTrace(TraceEventType.Warning))
                return;

            var ev = new InternalDiagnosticEvent();
            ev.Message = getMessage();
            ev.Severity = KnownSeverityLevels.Warning;

            ProcessDiagnosticEvent(ev);
        }

        public void Warning(Func<string> getMessage, params object[] attachedObjects)
        {
            throw new NotSupportedException();
        }

        public void Information(Exception ex, string message = null)
        {
            if (!InternalLoggerSwitch.ShouldTrace(TraceEventType.Information))
                return;

            var ev = new InternalDiagnosticEvent();
            ev.ExceptionObject = ex;
            ev.Message = message;
            ev.Severity = KnownSeverityLevels.Information;

            ProcessDiagnosticEvent(ev);
        }

        public void InformationFormat(Exception ex, string message, params object[] messageArgs)
        {
            if (!InternalLoggerSwitch.ShouldTrace(TraceEventType.Information))
                return;

            var ev = new InternalDiagnosticEvent();
            ev.ExceptionObject = ex;
            ev.Message = message.FormatWith(messageArgs);
            ev.Severity = KnownSeverityLevels.Information;

            ProcessDiagnosticEvent(ev);
        }

        public void Information(Exception ex, Func<string> getMessage)
        {
            if (!InternalLoggerSwitch.ShouldTrace(TraceEventType.Information))
                return;

            var ev = new InternalDiagnosticEvent();
            ev.ExceptionObject = ex;
            ev.Message = getMessage();
            ev.Severity = KnownSeverityLevels.Information;

            ProcessDiagnosticEvent(ev);
        }

        public void Information(Exception ex, Func<string> getMessage, params object[] attachedObjects)
        {
            throw new NotSupportedException();
        }

        public void Information(string message)
        {
            if (!InternalLoggerSwitch.ShouldTrace(TraceEventType.Information))
                return;

            var ev = new InternalDiagnosticEvent();
            ev.Message = message;
            ev.Severity = KnownSeverityLevels.Information;

            ProcessDiagnosticEvent(ev);
        }

        public void InformationFormat(string message, params object[] messageArgs)
        {
            if (!InternalLoggerSwitch.ShouldTrace(TraceEventType.Information))
                return;

            var ev = new InternalDiagnosticEvent();
            ev.Message = message.FormatWith(messageArgs);
            ev.Severity = KnownSeverityLevels.Information;

            ProcessDiagnosticEvent(ev);
        }

        public void Information(Func<string> getMessage)
        {
            if (!InternalLoggerSwitch.ShouldTrace(TraceEventType.Information))
                return;

            var ev = new InternalDiagnosticEvent();
            ev.Message = getMessage();
            ev.Severity = KnownSeverityLevels.Information;

            ProcessDiagnosticEvent(ev);
        }

        public void Information(Func<string> getMessage, params object[] attachedObjects)
        {
            throw new NotSupportedException();
        }

        public void Verbose(Exception ex, string message = null)
        {
            throw new NotSupportedException();
        }

        public void VerboseFormat(Exception ex, string message, params object[] messageArgs)
        {
            throw new NotSupportedException();
        }

        public void Verbose(Exception ex, Func<string> getMessage)
        {
            throw new NotSupportedException();
        }

        public void Verbose(Exception ex, Func<string> getMessage, params object[] attachedObjects)
        {
            throw new NotSupportedException();
        }

        public void Verbose(string message)
        {
            throw new NotSupportedException();
        }

        public void VerboseFormat(string message, params object[] messageArgs)
        {
            throw new NotSupportedException();
        }

        public void Verbose(Func<string> getMessage)
        {
            throw new NotSupportedException();
        }

        public void Verbose(Func<string> getMessage, params object[] attachedObjects)
        {
            throw new NotSupportedException();
        }

        public void WriteRawLine(string message)
        {
            throw new NotSupportedException();
        }

        public void WriteRaw(string message)
        {
            throw new NotSupportedException();
        }

        public void Event(SeverityLevel severity, string category)
        {
            throw new NotSupportedException();
        }

        public void Event(SeverityLevel severity, string category, params object[] attachedObjects)
        {
            throw new NotSupportedException();
        }

        public void ProcessDiagnosticEvent(IDiagnosticEvent de)
        {
            var prefix = "";

            if (de.Severity == KnownSeverityLevels.Information)
                prefix = "INFORMATION: ";
            else if (de.Severity == KnownSeverityLevels.Warning)
                prefix = "WARNNING: ";
            else if (de.Severity == KnownSeverityLevels.Error)
                prefix = "ERROR: ";

            System.Diagnostics.Trace.WriteLine("[" + prefix + "SquaredInfinity.Diagnostics]: " + de.Message.EmptyIfNull());

            if(de.ExceptionObject != null)
                System.Diagnostics.Trace.WriteLine("[" + prefix + "SquaredInfinity.Diagnostics]: " + de.ExceptionObject.ToString());
        }

        public IDiagnosticEventPropertyCollection GlobalProperties
        {
            get { throw new NotSupportedException(); }
        }

        public ILoggerName Name
        {
            get
            {
                throw new NotSupportedException();
            }
            set
            {
                throw new NotSupportedException();
            }
        }

        public ILogger Parent
        {
            get
            {
                throw new NotSupportedException();
            }
            set
            {
                throw new NotSupportedException();
            }
        }

        private class InternalDiagnosticEvent : IDiagnosticEvent
        {
            public string Message { get; set; }

            public Exception ExceptionObject { get; set; }

            public string Category
            {
                get { throw new NotImplementedException(); }
            }

            public SeverityLevel Severity { get; set; }

            public IDiagnosticEventPropertyCollection Properties
            {
                get { throw new NotImplementedException(); }
            }

            public IDiagnosticEventPropertyCollection AdditionalContextData
            {
                get { throw new NotImplementedException(); }
            }

            public IAttachedObjectCollection AttachedObjects
            {
                get { throw new NotImplementedException(); }
            }

            public string LoggerName
            {
                get
                {
                    throw new NotImplementedException();
                }
                set
                {
                    throw new NotImplementedException();
                }
            }

            public DateTime? DateTimeLocal
            {
                get { throw new NotImplementedException(); }
            }

            public DateTime? DateTimeUtc
            {
                get { throw new NotImplementedException(); }
            }

            public void EvaluateAndPinAllDataIfNeeded()
            {
                throw new NotImplementedException();
            }

            public void PinAdditionalContextDataIfNeeded()
            {
                throw new NotImplementedException();
            }

            public void IncludeCallerInformation(bool useCallerNameAsLoggerName, bool includeCallerInfo)
            {
                throw new NotImplementedException();
            }
        }


        public Configuration.IDiagnosticsConfiguration GetConfigurationClone()
        {
            throw new NotImplementedException();
        }

        public void ApplyConfiguration(Configuration.IDiagnosticsConfiguration newConfiguration)
        {
            throw new NotImplementedException();
        }
    }
}
