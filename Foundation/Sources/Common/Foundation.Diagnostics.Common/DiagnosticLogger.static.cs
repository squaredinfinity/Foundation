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
        public static ILogger Global { get; private set; }

        public static void SetGlobalLogger(ILogger newGlobalLogger)
        {
            Global = newGlobalLogger;
        }

        public static IReadOnlyList<ISinkLocation> GetLogLocations()
        {
            //List<ISinkLocation> result = new List<ISinkLocation>();

            //// make a copy of reference to current configuration
            //// to make sure that any changes (which would replace Config)
            //// will not be applied to this method before it exits
            //var config_ref = Config;

            //for (int i = 0; i < config_ref.SinkDefinitions.Count; i++)
            //{
            //    var sink = config_ref.SinkDefinitions[i];

            //    result.Add(sink.SinkLocation);
            //}

            //return result;


            return null;
        }
    }
}
