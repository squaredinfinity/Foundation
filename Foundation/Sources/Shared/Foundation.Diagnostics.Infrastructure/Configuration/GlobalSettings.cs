using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Diagnostics.Configuration
{
    public class GlobalSettings
    {
        /// <summary>
        /// True to use calling member full name as a logger name.
        /// For example, calling .Information(...) from method Main() will append .Main() to logger name
        /// This is very simple way of tracking execution flow (without need to implement aspect based approaches or simillar patterns)
        /// This also allows you to easily control which classes / methods are producing logs.
        /// </summary>
        public bool UseCallerNameAsLoggerName { get; set; }

        /// <summary>
        /// True to add caller info (caller member name, file path, line number) in each diagnostic event so it can included in logs.
        /// Note that this has a negative performance impact and can reveal sensitive information about your software.
        /// </summary>
        public bool IncludeCallerInfo { get; set; }

        /// <summary>
        /// When true (default), logging is enabled.
        /// When false, logging is disabled.
        /// Set to false if you want to disable all logging.
        /// </summary>
        public bool EnableLogging { get; set; }

        public GlobalSettings()
        {
            UseCallerNameAsLoggerName = false;
            IncludeCallerInfo = false;
            EnableLogging = true;
        }
    }
}
