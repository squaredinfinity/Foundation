using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Foundation.Diagnostics.Filters
{
    public interface IFilter
    {
        string Name { get; set; }
        FilterMode Mode { get; set; }

        bool Evaluate(IDiagnosticEvent diagnosticEvent, ILoggerName loggerName);
    }
}
