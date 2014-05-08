using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Foundation.Diagnostics
{
    /// <summary>
    /// Contains information about a diagnostic event that occured.
    /// </summary>
    public interface IDiagnosticEvent
    {
        string Message { get; }
        
        Exception ExceptionObject { get; }

        string Category { get; }

        SeverityLevel Severity { get; }

        IDiagnosticEventPropertyCollection Properties { get; }

        IDiagnosticEventPropertyCollection AdditionalContextData { get; }

        IAttachedObjectCollection AttachedObjects { get; }

        string LoggerName { get; set; }

        DateTime? DateTimeLocal { get; }

        DateTime? DateTimeUtc { get; }

        void EvaluateAndPinAllDataIfNeeded();

        void PinAdditionalContextDataIfNeeded();
    }
}
