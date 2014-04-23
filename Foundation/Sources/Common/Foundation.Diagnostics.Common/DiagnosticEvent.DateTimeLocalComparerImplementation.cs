using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Foundation.Diagnostics
{
    /// <summary>
    /// Represents information about the diagnostic event.
    /// This information will be passed to Filters and Sinks.
    /// </summary>
    public partial class DiagnosticEvent
    {
        class DateTimeLocalComparerImplementation : IComparer, IComparer<DiagnosticEvent>
        {
            public int Compare(object x, object y)
            {
                return Compare(x as DiagnosticEvent, y as DiagnosticEvent);
            }
            public int Compare(DiagnosticEvent x, DiagnosticEvent y)
            {
                if (x == null && y == null)
                    return 0;

                if (x == null)
                    return 1;

                if (y == null)
                    return -1;

                var xDate = x.DateTimeLocal;
                var yDate = y.DateTimeLocal;

                if (xDate == null && y.DateTimeLocal == null)
                    return 0;

                if (xDate == null)
                    return 1;

                if (yDate == null)
                    return -1;

                return DateTime.Compare(xDate.Value, yDate.Value);
            }
        }
    }
}
