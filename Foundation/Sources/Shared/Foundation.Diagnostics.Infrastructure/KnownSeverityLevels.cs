using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Diagnostics
{
    public struct KnownSeverityLevels
    {
        public static readonly SeverityLevel Maximum =
            new SeverityLevel
            {
                Name = "Maximum",
                Value = byte.MaxValue
            };

        public static readonly SeverityLevel Critical =
            new SeverityLevel
            {
                Name = "Critical",
                Value = 250
            };

        public static readonly SeverityLevel Error =
            new SeverityLevel
            {
                Name = "Error",
                Value = 200
            };

        public static readonly SeverityLevel Warning = new SeverityLevel
        {
            Name = "Warning",
            Value = 150
        };

        public static readonly SeverityLevel Information = new SeverityLevel
        {
            Name = "Information",
            Value = 100
        };

        public static readonly SeverityLevel Verbose = new SeverityLevel
        {
            Name = "Verbose",
            Value = 50
        };

        public static readonly SeverityLevel Unspecified = new SeverityLevel
        {
            Name = "Unspecified",
            Value = 1
        };

        public static readonly SeverityLevel Minimum = new SeverityLevel
        {
            Name = "Minimum",
            Value = 0
        };

        public static SeverityLevel Parse(string name)
        {
            if (string.Equals("maximum", name, StringComparison.InvariantCultureIgnoreCase))
                return Maximum;

            if (string.Equals("critical", name, StringComparison.InvariantCultureIgnoreCase))
                return Critical;

            if (string.Equals("error", name, StringComparison.InvariantCultureIgnoreCase))
                return Error;

            if (string.Equals("warning", name, StringComparison.InvariantCultureIgnoreCase))
                return Warning;

            if (string.Equals("information", name, StringComparison.InvariantCultureIgnoreCase))
                return Information;

            if (string.Equals("verbose", name, StringComparison.InvariantCultureIgnoreCase))
                return Verbose;

            if (string.Equals("unspecified", name, StringComparison.InvariantCultureIgnoreCase))
                return Unspecified;

            if (string.Equals("Minimum", name, StringComparison.InvariantCultureIgnoreCase))
                return Minimum;

            return Unspecified;
        }
    }
}
