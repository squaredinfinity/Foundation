using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace SquaredInfinity.Extensions
{
    public static class StopwatchExtensions
    {
        public static TimeSpan GetElapsedAndRestart(this Stopwatch sw)
        {
            var result = sw.Elapsed;

            sw.Restart();

            return result;
        }
    }
}
