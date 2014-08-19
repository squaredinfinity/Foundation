using SquaredInfinity.Foundation.Diagnostics.Configuration;
using SquaredInfinity.Foundation.Diagnostics.Filters;
using SquaredInfinity.Foundation.Diagnostics.Sinks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Foundation.Diagnostics
{
    /// <summary>
    /// Common Infrastructure interface to be implemented by all Loggers.
    /// </summary>
    public interface ILogger
    {
        #region Critical

        void Critical(Exception ex, string message = null);
        void CriticalFormat(Exception ex, string message, params object[] messageArgs);
        void Critical(Exception ex, Func<string> getMessage);
        void Critical(Exception ex, Func<string> getMessage, params object[] attachedObjects);

        void Critical(string message);
        void CriticalFormat(string message, params object[] messageArgs);
        void Critical(Func<string> getMessage);
        void Critical(Func<string> getMessage, params object[] attachedObjects);

        #endregion

        #region Error

        void Error(Exception ex, string message = null);
        void ErrorFormat(Exception ex, string message, params object[] messageArgs);
        void Error(Exception ex, Func<string> getMessage);
        void Error(Exception ex, Func<string> getMessage, params object[] attachedObjects);

        void Error(string message);
        void ErrorFormat(string message, params object[] messageArgs);
        void Error(Func<string> getMessage);
        void Error(Func<string> getMessage, params object[] attachedObjects);

        #endregion

        #region Warning

        void Warning(Exception ex, string message = null);
        void WarningFormat(Exception ex, string message, params object[] messageArgs);
        void Warning(Exception ex, Func<string> getMessage);
        void Warning(Exception ex, Func<string> getMessage, params object[] attachedObjects);

        void Warning(string message);
        void WarningFormat(string message, params object[] messageArgs);
        void Warning(Func<string> getMessage);
        void Warning(Func<string> getMessage, params object[] attachedObjects);

        #endregion

        #region Information

        void Information(Exception ex, string message = null);
        void InformationFormat(Exception ex, string message, params object[] messageArgs);
        void Information(Exception ex, Func<string> getMessage);
        void Information(Exception ex, Func<string> getMessage, params object[] attachedObjects);

        void Information(string message);
        void InformationFormat(string message, params object[] messageArgs);
        void Information(Func<string> getMessage);
        void Information(Func<string> getMessage, params object[] attachedObjects);

        #endregion

        #region Verbose

        void Verbose(Exception ex, string message = null);
        void VerboseFormat(Exception ex, string message, params object[] messageArgs);
        void Verbose(Exception ex, Func<string> getMessage);
        void Verbose(Exception ex, Func<string> getMessage, params object[] attachedObjects);

        void Verbose(string message);
        void VerboseFormat(string message, params object[] messageArgs);
        void Verbose(Func<string> getMessage);
        void Verbose(Func<string> getMessage, params object[] attachedObjects);

        #endregion

        #region Raw

        void WriteRawLine(string message);
        void WriteRaw(string message);

        #endregion

        void Event(SeverityLevel severity, string category);
        void Event(SeverityLevel severity, string category, params object[] attachedObjects);

        void ProcessDiagnosticEvent(IDiagnosticEvent de);

        IDiagnosticEventPropertyCollection GlobalProperties { get; }

        /// <summary>
        /// Name of a logger
        /// </summary>
        ILoggerName Name { get; set; }

        /// <summary>
        /// Parent of a logger
        /// </summary>
        ILogger Parent { get; set; }

        /// <summary>
        /// Returns a cloned copy of current configuration.
        /// You can modify the copy and apply it using ApplyConfiguration() method.
        /// </summary>
        /// <returns></returns>
        IDiagnosticsConfiguration GetConfigurationClone();

        /// <summary>
        /// Applies specified configuration.
        /// </summary>
        /// <param name="newConfiguration"></param>
        void ApplyConfiguration(IDiagnosticsConfiguration newConfiguration);

        // TODO
        //public static IReadOnlyList<ISinkLocation> GetLogLocations()
        //{
        //    //List<ISinkLocation> result = new List<ISinkLocation>();

        //    //// make a copy of reference to current configuration
        //    //// to make sure that any changes (which would replace Config)
        //    //// will not be applied to this method before it exits
        //    //var config_ref = Config;

        //    //for (int i = 0; i < config_ref.SinkDefinitions.Count; i++)
        //    //{
        //    //    var sink = config_ref.SinkDefinitions[i];

        //    //    result.Add(sink.SinkLocation);
        //    //}

        //    //return result;


        //    return null;
        //}
    }
}
