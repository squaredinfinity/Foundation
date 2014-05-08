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
    /// Provides methods and properties of DiagnosticLogger which are used internally by the library.
    /// </summary>
    /// <remarks>
    /// Those are part of separate interface in order to minimize noise in intellisense for end-users.
    /// While EditorBrowsableAttribute could be used, it doesn't provide enough flexibility:
    ///     These members should be visible internally and hidden for external users
    ///     EditorBorwsableAttribute hides/shows these member to everyone or noone
    /// </remarks>
    public interface ILoggerImplementation : ILogger
    {
        //ILoggerConfigProvider Configuration { get; }

        //void Configure(ConfigurationIConfigurationProvider configProvider, bool merge = false);
        //void Configure(DiagnosticsConfiguration config);

        //DiagnosticsConfiguration GetConfiguration();


        // todo: consider moving to ILogger
        IDiagnosticEventPropertyCollection GlobalProperties { get; }

        IReadOnlyList<ISinkLocation> GetLogLocations();

        /// <summary>
        /// Name of a logger
        /// </summary>
        ILoggerName Name { get; set; }

        /// <summary>
        /// Parent of a logger
        /// </summary>
        ILoggerImplementation Parent { get; set; }

        /// <summary>
        /// Determines if given Diagnostic Event meets filtering criteria
        /// </summary>
        /// <param name="diagnosticEvent"></param>
        /// <param name="loggerName"></param>
        /// <param name="config"></param>
        /// <returns>true if event should be logged, false otherwise1</returns>
        bool MeetsFiltersCriteria(IDiagnosticEvent diagnosticEvent, ILoggerName loggerName, IFilterCollection filters);
    }
}
