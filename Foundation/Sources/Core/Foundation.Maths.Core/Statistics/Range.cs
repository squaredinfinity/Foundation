using System;
using SquaredInfinity.Foundation.Extensions;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reactive.Linq;
using System.Reactive.Disposables;

namespace SquaredInfinity.Foundation.Maths.Statistics
{
    /// <summary>
    /// Contains methods for calculating Range, that is the difference between largest and smallest values.
    /// https://en.wikipedia.org/wiki/Range_(statistics)
    /// </summary>
    public static class Range
    {
        /// <summary>
        /// Calculates the Range of given samples.
        /// </summary>
        /// <param name="samples"></param>
        /// <returns>NaN if range cannot be calculated, valid double range otherwise</returns>
        public static RangeInfo Calculate(IReadOnlyList<double> samples)
        {
            var result = new RangeInfo(double.PositiveInfinity, double.NegativeInfinity);

            if (samples.Count <= 0)
                return result;

            if (samples.Count == 1)
            {
                var x = samples[0];

                result.Max = x;
                result.Min = x;

                return result;
            }

            for (int i = 0; i < samples.Count; i++)
            {
                var x = samples[i];

                if (x.IsInfinityOrNaN())
                    continue;

                if (result.Min.IsGreaterThan(x))
                    result.Min = x;
                else if (result.Max.IsLessThan(x))
                    result.Max = x;
            }

            return result;
        }

        /// <summary>
        /// Calculates the Range of given samples.
        /// </summary>
        /// <param name="samples"></param>
        /// <returns>NaN if range cannot be calculated, valid double range otherwise</returns>
        public static IObservable<RangeInfo> Calculate(IObservable<double> samples)
        {
            var result = new RangeInfo(double.PositiveInfinity, double.NegativeInfinity);

            bool isFirst = true;

            return samples.Select(x =>
            {
                if (isFirst)
                {
                    isFirst = false;

                    if(x.IsInfinityOrNaN())
                    {
                        return result;
                    }
                    else
                    {
                        result.Min = x;
                        result.Max = x;

                        return result;
                    }
                }

                if (x.IsInfinityOrNaN())
                {
                    // nothing to do here
                }
                else
                {
                    if (result.Min.IsGreaterThan(x))
                        result.Min = x;
                    else if (result.Max.IsLessThan(x))
                        result.Max = x;
                }

                return result;
            });
        }
    }
}
