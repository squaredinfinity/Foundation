using SquaredInfinity.Diagnostics.ContextDataCollectors;
using SquaredInfinity.Diagnostics.Filters;
using SquaredInfinity.Diagnostics.Formatters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Diagnostics.Sinks
{
    public interface ISink
    {
        /// <summary>
        /// Name of this sink
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// List of tags associated with this sink.
        /// Tags can be used to specifiy only specific sinks to log some diagnostic events.
        /// </summary>
        IList<string> Tags { get; set; }

        /// <summary>
        /// True if Diagnostics engine should wait for this sink to finish writing before continuing.
        /// False when this is fire-and-forget sink
        /// </summary>
        bool MustWaitForWrite { get; set; }

        /// <summary>
        /// Location where the log is stored (e.g. path to a file, address of a web server)
        /// </summary>
        ISinkLocation SinkLocation { get; }

        IFormatter Formatter { get; set; }

        IFilter Filter { get; set; }

        void Write(IDiagnosticEvent de);

        void Initialize();

        /// <summary>
        /// Returns a list of DataRequests that are used by this sink.
        /// This list should be cached internally when possible.
        /// </summary>
        /// <returns></returns>
        IReadOnlyList<IDataRequest> GetRequestedContextData();

        /// <summary>
        /// Called by Diagnostic infrastructure when application is about to exit.
        /// </summary>
        void OnApplicationExit();
    }
}
