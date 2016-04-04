using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Foundation.Maths.Statistics
{
    public static class StdDev
    {
        public static double Calculate(IReadOnlyList<double> samples)
        {
            return Calculate(samples, VarianceMode.Unbiased);
        }

        public static double Calculate(IReadOnlyList<double> samples, VarianceMode varianceMode)
        {
            return Calculate(samples, varianceMode, VarianceAlgorithm.Online);
        }

        public static double Calculate(IReadOnlyList<double> samples, VarianceMode varianceMode, VarianceAlgorithm varianceAlgorithm)
        {
            var variance = Variance.Calculate(samples, varianceMode, varianceAlgorithm);

            return Math.Sqrt(variance);
        }
    }
}
