using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reactive.Linq;
using System.Threading;
using System.Numerics;

namespace SquaredInfinity.Foundation.Maths.Statistics
{
    public static class Total
    {
        /// <summary>
        /// Calculates the Total of given samples.
        /// </summary>
        /// <param name="samples"></param>
        public static Decimal Calculate(IReadOnlyList<double> samples)
        {
            var x = 0.0M;

            for (int i = 0; i < samples.Count; i++)
                x += Convert.ToDecimal(samples[i]);

            return x;
        }

        /// <summary>
        /// Calculates the Count of given samples.
        /// </summary>
        /// <param name="samples"></param>
        public static IObservable<Decimal> Calculate(IObservable<double> samples)
        {
            var result = (Decimal)0;

            return samples.Select(x =>
            {
                result += Convert.ToDecimal(x);

                return result;
            });
        }
    }
}
