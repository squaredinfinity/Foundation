using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Foundation.Diagnostics
{
    public interface ILoggerImplementation
    {
        ILoggerConfigProvider Configuration { get; }

        void Configure(ConfigurationIConfigurationProvider configProvider, bool merge = false);
        void Configure(DiagnosticsConfiguration config);

        DiagnosticsConfiguration GetConfiguration();


        // todo: consider moving to ILogger
        DiagnosticEventPropertyCollection GlobalProperties { get; }

#if UPTO_DOTNET40
        IList<SinkLocation>
#else
        IReadOnlyList<SinkLocation>
#endif
 GetLogLocations();
    }
}
