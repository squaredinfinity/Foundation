using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Maths.Statistics
{
    public static class Min
    {
        public static double Calculate(IReadOnlyList<double> samples, bool ignoreNaNs)
        {
            var min = double.NaN;

            for (int i = 0; i < samples.Count; i++)
            {
                var x = samples[i];

                if (double.IsNaN(x))
                    continue;

                if (min > x || double.IsNaN(min))
                    min = x;
            }

            return min;
        }
    }
}
