using SquaredInfinity.Foundation.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Foundation
{
    public static class GlobalConfiguration
    {
        public static ILogger Logger { get; private set; }

        public static void SetLogger(ILogger logger)
        {
            Logger = logger;
        }
    }
}
