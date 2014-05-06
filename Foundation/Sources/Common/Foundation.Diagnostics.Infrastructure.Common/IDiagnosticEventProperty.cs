using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Foundation.Diagnostics
{
    public interface IDiagnosticEventProperty
    {
        string UniqueName { get; }

        object Value { get; }
    }
}
