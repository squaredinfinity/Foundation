using SquaredInfinity.Foundation.Diagnostics.Sinks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Foundation.Diagnostics
{
    public partial class DiagnosticLogger
    {
        /// <summary>
        /// Returns a global instance of ILogger.
        /// This instance can be accessed anywhere in the application,
        /// but preferred way may be to resovle it using Dependency Injection instead.
        /// </summary>
        public static ILogger Global { get; private set; }

        static DiagnosticLogger()
        {
            Global = new DiagnosticLogger(string.Empty);
        }

        public static void SetGlobalLogger(ILogger newGlobalLogger)
        {
            Global = newGlobalLogger;
        }
    }
}
