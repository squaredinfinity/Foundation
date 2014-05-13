using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Foundation.Diagnostics.Filters
{
    /// <summary>
    /// Filtering precedense:
    /// 1. Severity
    /// 2. FlomSeveritty and ToSeverity
    /// 3. ExclusiveFromSeverity and ExclusiveToSeverity
    /// </summary>
    public sealed class SeverityFilter : Filter
    {
        /// <summary>
        /// Severity that will be matched by this filter
        /// </summary>
        public SeverityLevel? Severity { get; set; }

        /// <summary>
        /// Inclusive From
        /// </summary>
        public SeverityLevel? From { get; set; }

        /// <summary>
        /// Inclusive To
        /// </summary>
        public SeverityLevel? To { get; set; }

        public SeverityLevel? ExclusiveFrom { get; set; }

        public SeverityLevel? ExclusiveTo { get; set; }

        public override bool Evaluate(IDiagnosticEvent diagnosticEvent, ILoggerName loggerName)
        {
            return EvaluateInternal(diagnosticEvent.Severity);
        }

        internal bool EvaluateInternal(SeverityLevel severity)
        {
            bool meetsCriteria = false;

            if (Severity != null)
            {
                meetsCriteria = severity == Severity;
            }
            else if (From != null && To != null)
            {
                meetsCriteria = Severity >= From && severity <= To;
            }
            else if (From != null)
            {
                meetsCriteria = severity >= From;
            }
            else if (To != null)
            {
                meetsCriteria = severity <= To;
            }
            else
            {
                if (ExclusiveFrom != null && ExclusiveTo != null)
                {
                    meetsCriteria = severity >= ExclusiveFrom && severity <= ExclusiveTo;
                }
                else if (ExclusiveFrom != null)
                {
                    meetsCriteria = severity > ExclusiveFrom;
                }
                else if (ExclusiveFrom != null)
                {
                    meetsCriteria = severity < ExclusiveTo;
                }
            }

            if (Mode == FilterMode.Include)
                return meetsCriteria;
            else
                return !meetsCriteria;
        }
    }
}
