using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Foundation.Diagnostics.Loggers
{
    public class TraceLogger : ILogger
    {
        public void Critical(Exception ex, string message = null)
        {
            throw new NotImplementedException();
        }

        public void CriticalFormat(Exception ex, string message, params object[] messageArgs)
        {
            throw new NotImplementedException();
        }

        public void Critical(Exception ex, Func<string> getMessage)
        {
            throw new NotImplementedException();
        }

        public void Critical(Exception ex, Func<string> getMessage, params object[] attachedObjects)
        {
            throw new NotImplementedException();
        }

        public void Critical(string message)
        {
            throw new NotImplementedException();
        }

        public void CriticalFormat(string message, params object[] messageArgs)
        {
            throw new NotImplementedException();
        }

        public void Critical(Func<string> getMessage)
        {
            throw new NotImplementedException();
        }

        public void Critical(Func<string> getMessage, params object[] attachedObjects)
        {
            throw new NotImplementedException();
        }

        public void Error(Exception ex, string message = null)
        {
            throw new NotImplementedException();
        }

        public void ErrorFormat(Exception ex, string message, params object[] messageArgs)
        {
            throw new NotImplementedException();
        }

        public void Error(Exception ex, Func<string> getMessage)
        {
            throw new NotImplementedException();
        }

        public void Error(Exception ex, Func<string> getMessage, params object[] attachedObjects)
        {
            throw new NotImplementedException();
        }

        public void Error(string message)
        {
            throw new NotImplementedException();
        }

        public void ErrorFormat(string message, params object[] messageArgs)
        {
            throw new NotImplementedException();
        }

        public void Error(Func<string> getMessage)
        {
            throw new NotImplementedException();
        }

        public void Error(Func<string> getMessage, params object[] attachedObjects)
        {
            throw new NotImplementedException();
        }

        public void Warning(Exception ex, string message = null)
        {
            throw new NotImplementedException();
        }

        public void WarningFormat(Exception ex, string message, params object[] messageArgs)
        {
            throw new NotImplementedException();
        }

        public void Warning(Exception ex, Func<string> getMessage)
        {
            throw new NotImplementedException();
        }

        public void Warning(Exception ex, Func<string> getMessage, params object[] attachedObjects)
        {
            throw new NotImplementedException();
        }

        public void Warning(string message)
        {
            throw new NotImplementedException();
        }

        public void WarningFormat(string message, params object[] messageArgs)
        {
            throw new NotImplementedException();
        }

        public void Warning(Func<string> getMessage)
        {
            throw new NotImplementedException();
        }

        public void Warning(Func<string> getMessage, params object[] attachedObjects)
        {
            throw new NotImplementedException();
        }

        public void Information(Exception ex, string message = null)
        {
            throw new NotImplementedException();
        }

        public void InformationFormat(Exception ex, string message, params object[] messageArgs)
        {
            throw new NotImplementedException();
        }

        public void Information(Exception ex, Func<string> getMessage)
        {
            throw new NotImplementedException();
        }

        public void Information(Exception ex, Func<string> getMessage, params object[] attachedObjects)
        {
            throw new NotImplementedException();
        }

        public void Information(string message)
        {
            throw new NotImplementedException();
        }

        public void InformationFormat(string message, params object[] messageArgs)
        {
            throw new NotImplementedException();
        }

        public void Information(Func<string> getMessage)
        {
            throw new NotImplementedException();
        }

        public void Information(Func<string> getMessage, params object[] attachedObjects)
        {
            throw new NotImplementedException();
        }

        public void Verbose(Exception ex, string message = null)
        {
            throw new NotImplementedException();
        }

        public void VerboseFormat(Exception ex, string message, params object[] messageArgs)
        {
            throw new NotImplementedException();
        }

        public void Verbose(Exception ex, Func<string> getMessage)
        {
            throw new NotImplementedException();
        }

        public void Verbose(Exception ex, Func<string> getMessage, params object[] attachedObjects)
        {
            throw new NotImplementedException();
        }

        public void Verbose(string message)
        {
            throw new NotImplementedException();
        }

        public void VerboseFormat(string message, params object[] messageArgs)
        {
            throw new NotImplementedException();
        }

        public void Verbose(Func<string> getMessage)
        {
            throw new NotImplementedException();
        }

        public void Verbose(Func<string> getMessage, params object[] attachedObjects)
        {
            throw new NotImplementedException();
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

        public Configuration.IDiagnosticsConfiguration GetConfigurationClone()
        {
            throw new NotSupportedException();
        }

        public void ApplyConfiguration(Configuration.IDiagnosticsConfiguration newConfiguration)
        {
            throw new NotSupportedException();
        }
    }
}
