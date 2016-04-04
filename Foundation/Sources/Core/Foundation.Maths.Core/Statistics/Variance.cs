using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Foundation.Maths.Statistics
{
    /// <summary>
    /// Contains methods for calculating Variance
    /// https://en.wikipedia.org/wiki/Variance
    /// </summary>
    public static class Variance
    {
        //# Naming Conventions:
        //  M2 - Sum of squares of differences {\sum(x - \bar x)^2}

        /// <summary>
        /// Calculates a Variance of a sample (set of numbers) using Unbiased Mode (Bessel's correction).
        /// </summary>
        /// <remarks>
        /// https://en.wikipedia.org/wiki/Variance
        /// Variance measures how far the set of numbers are spread out.
        /// Variance == 0 indicates that all numbers are identical.
        /// Variance is always non-negative.
        /// Variance close to 0 indicates that the numbers tend to be very close to the mean (and hence to each other).
        /// </remarks>
        /// <param name="samples">array of samples</param>
        /// <param name="mode">calculation mode (biased or unbiased)</param>
        public static double Calculate(IReadOnlyList<double> samples)
        {
            return Calculate(samples, VarianceMode.Unbiased);
        }

        /// <summary>
        /// Calculates a Variance of a sample (set of numbers).
        /// </summary>
        /// <remarks>
        /// https://en.wikipedia.org/wiki/Variance
        /// Variance measures how far the set of numbers are spread out.
        /// Variance == 0 indicates that all numbers are identical.
        /// Variance is always non-negative.
        /// Variance close to 0 indicates that the numbers tend to be very close to the mean (and hence to each other).
        /// </remarks>
        /// <param name="samples">array of samples</param>
        /// <param name="mode">calculation mode (biased or unbiased)</param>
        public static double Calculate(IReadOnlyList<double> samples, VarianceMode mode)
        {
            return Calculate(samples, mode, VarianceAlgorithm.Online);
        }

        /// <summary>
        /// Calculates a Variance of a sample (set of numbers).
        /// </summary>
        /// <remarks>
        /// https://en.wikipedia.org/wiki/Variance
        /// Variance measures how far the set of numbers are spread out.
        /// Variance == 0 indicates that all numbers are identical.
        /// Variance is always non-negative.
        /// Variance close to 0 indicates that the numbers tend to be very close to the mean (and hence to each other).
        /// </remarks>
        /// <param name="samples">array of samples</param>
        /// <param name="mode">calculation mode (biased or unbiased)</param>
        /// <param name="algorithm">algorithm to use for calculations</param>
        public static double Calculate(IReadOnlyList<double> samples, VarianceMode mode, VarianceAlgorithm algorithm)
        {
            if (mode == VarianceMode.Biased && samples.Count <= 0)
                return double.NaN;
            else if (mode == VarianceMode.Unbiased && samples.Count <= 1)
                return double.NaN;

            //# https://en.wikipedia.org/wiki/Algorithms_for_calculating_variance

            var M2 = double.NaN;

            if (algorithm == VarianceAlgorithm.Online)
                M2 = M2_OnlineAlgorithm(samples);
            else
                throw new NotSupportedException($"{algorithm} is not supported");

            if (mode == VarianceMode.Biased)
                return M2 / samples.Count;
            else
                return M2 / (samples.Count - 1);
        }

        static double M2_OnlineAlgorithm(IReadOnlyList<double> samples)
        {
            var mean = 0.0;
            var M2 = 0.0;

            for (var n = 0; n < samples.Count;)
            {
                var x = samples[n];
                n++;
                var delta = x - mean;
                mean += delta / n;
                M2 += delta * (x - mean);
            }

            return M2;
        }

        #region Observable Implementation

        /// <summary>
        /// Calculates a Variance of a sample (set of numbers) using Unbiased Mode (Bessel's correction).
        /// </summary>
        /// <remarks>
        /// https://en.wikipedia.org/wiki/Variance
        /// Variance measures how far the set of numbers are spread out.
        /// Variance == 0 indicates that all numbers are identical.
        /// Variance is always non-negative.
        /// Variance close to 0 indicates that the numbers tend to be very close to the mean (and hence to each other).
        /// </remarks>
        /// <param name="samples">observable samples</param>
        /// <param name="mode">calculation mode (biased or unbiased)</param>
        public static IObservable<double> Calculate(IObservable<double> samples)
        {
            return Calculate(samples, VarianceMode.Unbiased);
        }

        /// <summary>
        /// Calculates a Variance of a sample (set of numbers).
        /// </summary>
        /// <remarks>
        /// https://en.wikipedia.org/wiki/Variance
        /// Variance measures how far the set of numbers are spread out.
        /// Variance == 0 indicates that all numbers are identical.
        /// Variance is always non-negative.
        /// Variance close to 0 indicates that the numbers tend to be very close to the mean (and hence to each other).
        /// </remarks>
        /// <param name="samples">observable samples</param>
        /// <param name="mode">calculation mode (biased or unbiased)</param>
        public static IObservable<double> Calculate(IObservable<double> samples, VarianceMode mode)
        {
            var count = 0;

            return M2_OnlineAlgorithm(samples).Select(M2 =>
            {
                count++;

                if (mode == VarianceMode.Biased && count <= 0)
                {
                    return double.NaN;
                }
                else if (mode == VarianceMode.Unbiased && count <= 1)
                {
                    return double.NaN;
                }

                if (mode == VarianceMode.Biased)
                    return M2 / count;
                else
                    return M2 / (count - 1);
            });
        }

        /// <summary>
        /// Calculates a Variance of a sample (set of numbers).
        /// </summary>
        /// <remarks>
        /// https://en.wikipedia.org/wiki/Variance
        /// Variance measures how far the set of numbers are spread out.
        /// Variance == 0 indicates that all numbers are identical.
        /// Variance is always non-negative.
        /// Variance close to 0 indicates that the numbers tend to be very close to the mean (and hence to each other).
        /// </remarks>
        /// <param name="samples">observable samples</param>
        /// <param name="mode">calculation mode (biased or unbiased)</param>
        /// <param name="algorithm">algorithm to use for calculations</param>
        public static IObservable<double> Calculate(IObservable<double> samples, VarianceMode mode, VarianceAlgorithm algorithm)
        {
            return Observable.Create<double>(o =>
            {
                var count = 0;

                var M2_Observable = (IObservable<double>)null;

                if (algorithm == VarianceAlgorithm.Online)
                    M2_Observable = M2_OnlineAlgorithm(samples);
                else
                    throw new NotSupportedException($"{algorithm} is not supported");

                M2_Observable.Subscribe(M2 =>
                {
                    count++;

                    if (mode == VarianceMode.Biased && count <= 0)
                    {
                        o.OnNext(double.NaN);
                        return;
                    }
                    else if (mode == VarianceMode.Unbiased && count <= 1)
                    {
                        o.OnNext(double.NaN);
                        return;
                    }

                    if (mode == VarianceMode.Biased)
                        o.OnNext(M2 / count);
                    else
                        o.OnNext(M2 / (count - 1));
                });

                return Disposable.Empty;
            });
        }

        static IObservable<double> M2_OnlineAlgorithm(IObservable<double> samples)
        {
            var n = 0;
            var mean = 0.0;
            var M2 = 0.0;
            
            return samples.Select(x =>
            {
                n++;
                var delta = x - mean;
                mean += delta / n;
                M2 += delta * (x - mean);

                return M2;
            });
        }

        #endregion
    }
}
