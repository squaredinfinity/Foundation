using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Foundation.Diagnostics
{
    public partial class DiagnosticLogger : ILogger
    {
        #region Critical

        void ILogger.Critical(Exception ex, string message)
        {
            if (!Config.Cache.ShouldProcessCriticals)
                return;

            var de =
                DiagnosticEvent.PrepareDiagnosticEvent(
                    KnownSeverityLevels.Critical,
                    message,
                    exception: ex);

            ProcessDiagnosticEvent(de);
        }

        void ILogger.CriticalFormat(Exception ex, string message, params object[] messageArgs)
        {
            if (!Config.Cache.ShouldProcessCriticals)
                return;

            var de =
                DiagnosticEvent.PrepareDiagnosticEvent(
                    KnownSeverityLevels.Critical,
                    message,
                    messageArgs,
                    ex);

            ProcessDiagnosticEvent(de);
        }

        void ILogger.Critical(Exception ex, Func<string> getMessage)
        {
            if (!Config.Cache.ShouldProcessCriticals)
                return;

            var de =
                DiagnosticEvent.PrepareDiagnosticEvent(
                    KnownSeverityLevels.Critical,
                    getMessage,
                    ex);

            ProcessDiagnosticEvent(de);
        }

        void ILogger.Critical(Exception ex, Func<string> getMessage, params object[] attachedObjects)
        {
            if (!Config.Cache.ShouldProcessCriticals)
                return;

            var de =
                DiagnosticEvent.PrepareDiagnosticEvent(
                    KnownSeverityLevels.Critical,
                    getMessage,
                    ex,
                    attachedObjects: attachedObjects.Select((o) => new AttachedObject(o)).ToArray());

            ProcessDiagnosticEvent(de);
        }

        void ILogger.Critical(string message)
        {
            if (!Config.Cache.ShouldProcessCriticals)
                return;

            var de =
                DiagnosticEvent.PrepareDiagnosticEvent(
                    KnownSeverityLevels.Critical,
                    message);

            ProcessDiagnosticEvent(de);
        }

        void ILogger.CriticalFormat(string message, params object[] messageArgs)
        {
            if (!Config.Cache.ShouldProcessCriticals)
                return;

            var de =
                DiagnosticEvent.PrepareDiagnosticEvent(
                    KnownSeverityLevels.Critical,
                    message,
                    messageArgs);

            ProcessDiagnosticEvent(de);
        }

        void ILogger.Critical(Func<string> getMessage)
        {
            if (!Config.Cache.ShouldProcessCriticals)
                return;

            var de =
                DiagnosticEvent.PrepareDiagnosticEvent(
                    KnownSeverityLevels.Critical,
                    getMessage);

            ProcessDiagnosticEvent(de);
        }

        void ILogger.Critical(Func<string> getMessage, params object[] attachedObjects)
        {
            if (!Config.Cache.ShouldProcessCriticals)
                return;

            var de =
                DiagnosticEvent.PrepareDiagnosticEvent(
                    KnownSeverityLevels.Critical,
                    getMessage,
                    attachedObjects: attachedObjects.Select((o) => new AttachedObject(o)).ToArray());

            ProcessDiagnosticEvent(de);
        }

        #endregion

        #region Error

        void ILogger.Error(Exception ex, string message)
        {
            if (!Config.Cache.ShouldProcessErrors)
                return;

            var de =
                DiagnosticEvent.PrepareDiagnosticEvent(
                    KnownSeverityLevels.Error,
                    message,
                    exception: ex);

            ProcessDiagnosticEvent(de);
        }

        void ILogger.ErrorFormat(Exception ex, string message, params object[] messageArgs)
        {
            if (!Config.Cache.ShouldProcessErrors)
                return;

            var de =
                DiagnosticEvent.PrepareDiagnosticEvent(
                    KnownSeverityLevels.Error,
                    message,
                    messageArgs,
                    ex);

            ProcessDiagnosticEvent(de);
        }

        void ILogger.Error(Exception ex, Func<string> getMessage)
        {
            if (!Config.Cache.ShouldProcessErrors)
                return;

            var de =
                DiagnosticEvent.PrepareDiagnosticEvent(
                    KnownSeverityLevels.Error,
                    getMessage,
                    ex);

            ProcessDiagnosticEvent(de);
        }

        void ILogger.Error(Exception ex, Func<string> getMessage, params object[] attachedObjects)
        {
            if (!Config.Cache.ShouldProcessErrors)
                return;

            var de =
                DiagnosticEvent.PrepareDiagnosticEvent(
                    KnownSeverityLevels.Error,
                    getMessage,
                    ex,
                    attachedObjects: attachedObjects.Select((o) => new AttachedObject(o)).ToArray());

            ProcessDiagnosticEvent(de);
        }

        void ILogger.Error(string message)
        {
            if (!Config.Cache.ShouldProcessErrors)
                return;

            var de =
                DiagnosticEvent.PrepareDiagnosticEvent(
                    KnownSeverityLevels.Error,
                    message);

            ProcessDiagnosticEvent(de);
        }

        void ILogger.ErrorFormat(string message, params object[] messageArgs)
        {
            if (!Config.Cache.ShouldProcessErrors)
                return;

            var de =
                DiagnosticEvent.PrepareDiagnosticEvent(
                    KnownSeverityLevels.Error,
                    message,
                    messageArgs);

            ProcessDiagnosticEvent(de);
        }

        void ILogger.Error(Func<string> getMessage)
        {
            if (!Config.Cache.ShouldProcessErrors)
                return;

            var de =
                DiagnosticEvent.PrepareDiagnosticEvent(
                    KnownSeverityLevels.Error,
                    getMessage);

            ProcessDiagnosticEvent(de);
        }

        void ILogger.Error(Func<string> getMessage, params object[] attachedObjects)
        {
            if (!Config.Cache.ShouldProcessErrors)
                return;

            var de =
                DiagnosticEvent.PrepareDiagnosticEvent(
                    KnownSeverityLevels.Error,
                    getMessage,
                    attachedObjects: attachedObjects.Select((o) => new AttachedObject(o)).ToArray());

            ProcessDiagnosticEvent(de);
        }

        #endregion

        #region Warning

        void ILogger.Warning(Exception ex, string message)
        {
            if (!Config.Cache.ShouldProcessErrors)
                return;

            var de =
                DiagnosticEvent.PrepareDiagnosticEvent(
                    KnownSeverityLevels.Warning,
                    message,
                    exception: ex);

            ProcessDiagnosticEvent(de);
        }

        void ILogger.WarningFormat(Exception ex, string message, params object[] messageArgs)
        {
            if (!Config.Cache.ShouldProcessErrors)
                return;

            var de =
                DiagnosticEvent.PrepareDiagnosticEvent(
                    KnownSeverityLevels.Warning,
                    message,
                    messageArgs,
                    ex);

            ProcessDiagnosticEvent(de);
        }

        void ILogger.Warning(Exception ex, Func<string> getMessage)
        {
            if (!Config.Cache.ShouldProcessErrors)
                return;

            var de =
                DiagnosticEvent.PrepareDiagnosticEvent(
                    KnownSeverityLevels.Warning,
                    getMessage,
                    ex);

            ProcessDiagnosticEvent(de);
        }

        void ILogger.Warning(Exception ex, Func<string> getMessage, params object[] attachedObjects)
        {
            if (!Config.Cache.ShouldProcessErrors)
                return;

            var de =
                DiagnosticEvent.PrepareDiagnosticEvent(
                    KnownSeverityLevels.Warning,
                    getMessage,
                    ex,
                    attachedObjects: attachedObjects.Select((o) => new AttachedObject(o)).ToArray());

            ProcessDiagnosticEvent(de);
        }

        void ILogger.Warning(string message)
        {
            if (!Config.Cache.ShouldProcessErrors)
                return;

            var de =
                DiagnosticEvent.PrepareDiagnosticEvent(
                    KnownSeverityLevels.Warning,
                    message);

            ProcessDiagnosticEvent(de);
        }

        void ILogger.WarningFormat(string message, params object[] messageArgs)
        {
            if (!Config.Cache.ShouldProcessErrors)
                return;

            var de =
                DiagnosticEvent.PrepareDiagnosticEvent(
                    KnownSeverityLevels.Warning,
                    message,
                    messageArgs);

            ProcessDiagnosticEvent(de);
        }

        void ILogger.Warning(Func<string> getMessage)
        {
            if (!Config.Cache.ShouldProcessErrors)
                return;

            var de =
                DiagnosticEvent.PrepareDiagnosticEvent(
                    KnownSeverityLevels.Warning,
                    getMessage);

            ProcessDiagnosticEvent(de);
        }

        void ILogger.Warning(Func<string> getMessage, params object[] attachedObjects)
        {
            if (!Config.Cache.ShouldProcessErrors)
                return;

            var de =
                DiagnosticEvent.PrepareDiagnosticEvent(
                    KnownSeverityLevels.Warning,
                    getMessage,
                    attachedObjects: attachedObjects.Select((o) => new AttachedObject(o)).ToArray());

            ProcessDiagnosticEvent(de);
        }

        #endregion

        #region Information

        void ILogger.Information(Exception ex, string message)
        {
            if (!Config.Cache.ShouldProcessErrors)
                return;

            var de =
                DiagnosticEvent.PrepareDiagnosticEvent(
                    KnownSeverityLevels.Information,
                    message,
                    exception: ex);

            ProcessDiagnosticEvent(de);
        }

        void ILogger.InformationFormat(Exception ex, string message, params object[] messageArgs)
        {
            if (!Config.Cache.ShouldProcessErrors)
                return;

            var de =
                DiagnosticEvent.PrepareDiagnosticEvent(
                    KnownSeverityLevels.Information,
                    message,
                    messageArgs,
                    ex);

            ProcessDiagnosticEvent(de);
        }

        void ILogger.Information(Exception ex, Func<string> getMessage)
        {
            if (!Config.Cache.ShouldProcessErrors)
                return;

            var de =
                DiagnosticEvent.PrepareDiagnosticEvent(
                    KnownSeverityLevels.Information,
                    getMessage,
                    ex);

            ProcessDiagnosticEvent(de);
        }

        void ILogger.Information(Exception ex, Func<string> getMessage, params object[] attachedObjects)
        {
            if (!Config.Cache.ShouldProcessErrors)
                return;

            var de =
                DiagnosticEvent.PrepareDiagnosticEvent(
                    KnownSeverityLevels.Information,
                    getMessage,
                    ex,
                    attachedObjects: attachedObjects.Select((o) => new AttachedObject(o)).ToArray());

            ProcessDiagnosticEvent(de);
        }

        void ILogger.Information(string message)
        {
            if (!Config.Cache.ShouldProcessErrors)
                return;

            var de =
                DiagnosticEvent.PrepareDiagnosticEvent(
                    KnownSeverityLevels.Information,
                    message);

            ProcessDiagnosticEvent(de);
        }

        void ILogger.InformationFormat(string message, params object[] messageArgs)
        {
            if (!Config.Cache.ShouldProcessErrors)
                return;

            var de =
                DiagnosticEvent.PrepareDiagnosticEvent(
                    KnownSeverityLevels.Information,
                    message,
                    messageArgs);

            ProcessDiagnosticEvent(de);
        }

        void ILogger.Information(Func<string> getMessage)
        {
            if (!Config.Cache.ShouldProcessErrors)
                return;

            var de =
                DiagnosticEvent.PrepareDiagnosticEvent(
                    KnownSeverityLevels.Information,
                    getMessage);

            ProcessDiagnosticEvent(de);
        }

        void ILogger.Information(Func<string> getMessage, params object[] attachedObjects)
        {
            if (!Config.Cache.ShouldProcessErrors)
                return;

            var de =
                DiagnosticEvent.PrepareDiagnosticEvent(
                    KnownSeverityLevels.Information,
                    getMessage,
                    attachedObjects: attachedObjects.Select((o) => new AttachedObject(o)).ToArray());

            ProcessDiagnosticEvent(de);
        }

        #endregion

        #region Verbose

        void ILogger.Verbose(Exception ex, string message)
        {
            if (!Config.Cache.ShouldProcessErrors)
                return;

            var de =
                DiagnosticEvent.PrepareDiagnosticEvent(
                    KnownSeverityLevels.Verbose,
                    message,
                    exception: ex);

            ProcessDiagnosticEvent(de);
        }
        
        void ILogger.VerboseFormat(Exception ex, string message, params object[] messageArgs)
        {
            if (!Config.Cache.ShouldProcessErrors)
                return;

            var de =
                DiagnosticEvent.PrepareDiagnosticEvent(
                    KnownSeverityLevels.Verbose,
                    message,
                    messageArgs,
                    ex);

            ProcessDiagnosticEvent(de);
        }

        void ILogger.Verbose(Exception ex, Func<string> getMessage)
        {
            if (!Config.Cache.ShouldProcessErrors)
                return;

            var de =
                DiagnosticEvent.PrepareDiagnosticEvent(
                    KnownSeverityLevels.Verbose,
                    getMessage,
                    ex);

            ProcessDiagnosticEvent(de);
        }

        void ILogger.Verbose(Exception ex, Func<string> getMessage, params object[] attachedObjects)
        {
            if (!Config.Cache.ShouldProcessErrors)
                return;

            var de =
                DiagnosticEvent.PrepareDiagnosticEvent(
                    KnownSeverityLevels.Verbose,
                    getMessage,
                    ex,
                    attachedObjects: attachedObjects.Select((o) => new AttachedObject(o)).ToArray());

            ProcessDiagnosticEvent(de);
        }

        void ILogger.Verbose(string message)
        {
            if (!Config.Cache.ShouldProcessErrors)
                return;

            var de =
                DiagnosticEvent.PrepareDiagnosticEvent(
                    KnownSeverityLevels.Verbose,
                    message);

            ProcessDiagnosticEvent(de);
        }

        void ILogger.VerboseFormat(string message, params object[] messageArgs)
        {
            if (!Config.Cache.ShouldProcessErrors)
                return;

            var de =
                DiagnosticEvent.PrepareDiagnosticEvent(
                    KnownSeverityLevels.Verbose,
                    message,
                    messageArgs);

            ProcessDiagnosticEvent(de);
        }

        void ILogger.Verbose(Func<string> getMessage)
        {
            if (!Config.Cache.ShouldProcessErrors)
                return;

            var de =
                DiagnosticEvent.PrepareDiagnosticEvent(
                    KnownSeverityLevels.Verbose,
                    getMessage);

            ProcessDiagnosticEvent(de);
        }

        void ILogger.Verbose(Func<string> getMessage, params object[] attachedObjects)
        {
            if (!Config.Cache.ShouldProcessErrors)
                return;

            var de =
                DiagnosticEvent.PrepareDiagnosticEvent(
                    KnownSeverityLevels.Verbose,
                    getMessage,
                    attachedObjects: attachedObjects.Select((o) => new AttachedObject(o)).ToArray());

            ProcessDiagnosticEvent(de);
        }

        #endregion

        #region Raw

        public void WriteRawLine(string message)
        {
            WriteRaw(message + Environment.NewLine);
        }

        public void WriteRaw(string message)
        {
            try
            {
                if (!Config.Cache.ShouldProcessRawMessages)
                    return;

                // first write to sinks that we must wait for to finish writing
                var rawMessageSinks = Config.Sinks.MustWaitForWriteRawMessageSinks;

                for (int i = 0; i < rawMessageSinks.Count; i++)
                {
                    var sink = rawMessageSinks[i];
                    sink.Write(message);
                }

                var fireAndForgetSinks = Config.Sinks.FireAndForgetRawMessageSinks;

                for (int i = 0; i < fireAndForgetSinks.Count; i++)
                {
                    var sink = fireAndForgetSinks[i];
                    sink.Write(message);
                }
            }
            catch (Exception ex)
            {
                InternalTrace.Error(ex, "Failed to write raw message");
            }
        }

        #endregion

        #region Event

        void ILogger.Event(SeverityLevel severity, string category)
        {
            // TODO:
            //if (!ConfigCache.ShouldProcessErrors)
            //  return;

            var de =
                DiagnosticEvent.PrepareDiagnosticEvent(
                    severity,
                    category: category);

            ProcessDiagnosticEvent(de);
        }

        void ILogger.Event(SeverityLevel severity, string category, params object[] attachedObjects)
        {
            // TODO:
            //if (!ConfigCache.ShouldProcessErrors)
            //  return;

            var de =
                DiagnosticEvent.PrepareDiagnosticEvent(
                    severity,
                    category: category,
                    attachedObjects: attachedObjects.Select((o) => new AttachedObject(o)).ToArray());

            ProcessDiagnosticEvent(de);
        }

        #endregion

        public void ProcessLogEvent(DiagnosticEvent de)
        {
            if (de == null)
                return;

            ProcessDiagnosticEvent(de);
        }
    }
}
