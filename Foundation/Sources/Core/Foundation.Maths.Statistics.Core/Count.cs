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
    public static class Count
    {
        /// <summary>
        /// Calculates the Count of given samples.
        /// </summary>
        /// <param name="samples"></param>
        public static UInt64 Calculate(IReadOnlyList<double> samples)
        {
            return (UInt64) samples.Count;
        }

        /// <summary>
        /// Calculates the Count of given samples.
        /// </summary>
        /// <param name="samples"></param>
        public static IObservable<UInt64> Calculate(IObservable<double> samples)
        {
            var result = (UInt64)0;

            return samples.Select(x =>
            {
                result++;

                return result;
            });
        }

        ///// <summary>
        ///// Calculates the Count of given samples.
        ///// </summary>
        ///// <param name="samples"></param>
        //public static IObservable<BigInteger> CalculateBigInt(IObservable<double> samples)
        //{
        //    var result = BigInteger.Zero;

        //    return samples.Select(x =>
        //    {
        //        result++;

        //        return result;
        //    });
        //}
    }
}
