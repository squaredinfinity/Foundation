using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SquaredInfinity.Foundation.Extensions;

namespace SquaredInfinity.Foundation.Diagnostics
{
    internal partial class InternalDiagnosticLogger
    {
        /// <summary>
        /// Internal logger for diagnostics of this assembly.
        /// It should use System.Diagnostic namespace only
        /// </summary>
        internal static class EmergencyLogger
        {
            public static readonly BooleanSwitch InternalLoggerSwitch =
                new BooleanSwitch(
                    "SquaredInfinity.Diagnostics",
                    "SquaredInfinity.Diagnostics",
#if DEBUG
                defaultSwitchValue: "true");
#else
                defaultSwitchValue: "false");
#endif

            public static void TraceError(Exception ex, string message = null)
            {
                if (!InternalLoggerSwitch.Enabled)
                    return;

                System.Diagnostics.Trace.WriteLine(message);
                System.Diagnostics.Trace.WriteLine(ex.ToString());
            }

            public static void TraceWarning(Func<string> getMessage)
            {
                if (!InternalLoggerSwitch.Enabled)
                    return;

                try
                {
                    System.Diagnostics.Trace.WriteLine(getMessage());
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Trace.WriteLine(ex.ToString());
                }
            }

            public static void TraceInformation(DiagnosticEvent de)
            {
                if (!InternalLoggerSwitch.Enabled)
                    return;

                try
                {
                    System.Diagnostics.Trace.WriteLine(de.DumpToString());
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Trace.WriteLine(ex.ToString());
                }
            }
        }
    }
}
