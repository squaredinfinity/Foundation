using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Windows.Resources
{
    static class ResourcesDiagnostics
    {
        public static TraceSource TraceSource = new TraceSource("SquaredInfinity.Windows.Resources", SourceLevels.Off);
    }
}
