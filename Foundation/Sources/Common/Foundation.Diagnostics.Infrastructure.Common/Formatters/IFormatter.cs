using SquaredInfinity.Foundation.Diagnostics.ContextDataCollectors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Foundation.Diagnostics.Formatters
{
    public interface IFormatter
    {
        string Name { get; set; }
        string Format(IDiagnosticEvent de);

        IReadOnlyList<IDataRequest> GetRequestedContextData();
    }
}
