using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Foundation.Diagnostics.Fluent
{
    public class ApplicationLifecycleEvent
    {
        DiagnosticEventBuilder Builder { get; set; }

        public ApplicationLifecycleEvent(DiagnosticEventBuilder builder)
        {
            this.Builder = builder;
        }

        public void Startup()
        {
            Builder.LogEvent(ApplicationLifecycleEvents.Startup);
        }

        public void Shutdown()
        {
            Builder.LogEvent(ApplicationLifecycleEvents.Shutdown);
        }

        public void Install()
        {
            Builder.LogEvent(ApplicationLifecycleEvents.Install);
        }

    }
}
