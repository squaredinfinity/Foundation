using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Foundation.Diagnostics.Fluent
{
    public class SecurityEvent
    {
        DiagnosticEventBuilder Builder { get; set; }

        public void AccessAttempt(object resourceId)
        {
            Builder.AppendObject("SecurityEvent.ResourceId", resourceId);
            Builder.LogEvent(SecurityEvents.AccessAttempt);
        }

        public void AccessAttempt(string resourceType, object resourceId)
        {
            Builder.AppendObject("SecurityEvent.ResourceType", resourceType);
            Builder.AppendObject("SecurityEvent.ResourceId", resourceId);
            Builder.LogEvent(SecurityEvents.AccessAttempt);
        }

        public void AccessGranted(object resourceId)
        {
            Builder.AppendObject("SecurityEvent.ResourceId", resourceId);
            Builder.LogEvent(SecurityEvents.AccessGranted);
        }

        public void AccessGranted(string resourceType, object resourceId)
        {
            Builder.AppendObject("SecurityEvent.ResourceType", resourceType);
            Builder.AppendObject("SecurityEvent.ResourceId", resourceId);
            Builder.LogEvent(SecurityEvents.AccessGranted);
        }

        public SecurityEvent(DiagnosticEventBuilder builder)
        {
            this.Builder = builder;
        }
    }
}
