using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Windows.MarkupExtensions
{
    static class _Diagnostics
    {
        public static TraceSource TraceSource = new TraceSource("SquaredInfinity.Windows.MarkupExtensions", SourceLevels.Off);
    }
}
