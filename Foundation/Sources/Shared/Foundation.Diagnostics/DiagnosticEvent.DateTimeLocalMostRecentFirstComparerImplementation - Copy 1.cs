using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Diagnostics
{
    /// <summary>
    /// Represents information about the diagnostic event.
    /// This information will be passed to Filters and Sinks.
    /// </summary>
    public partial class DiagnosticEvent
    {
        class DateTimeLocalMostRecentFirstComparerImplementation : IComparer, IComparer<DiagnosticEvent>
        {
            public int Compare(object x, object y)
            {
                return DateTimeLocalComparer.Compare(x, y) * -1;
            }

            public int Compare(DiagnosticEvent x, DiagnosticEvent y)
            {
                return DateTimeLocalComparer.Compare(x, y) * -1;
            }
        }
    }
}
