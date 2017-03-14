using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Diagnostics
{
    public static class ApplicationLifecycleEvents
    {
        public static readonly string Install = "Application-Lifecycle.Install";
        public static readonly string Startup = "Application-Lifecycle.Startup";
        public static readonly string Shutdown = "Application-Lifecycle.Shutdown";
    }
}
