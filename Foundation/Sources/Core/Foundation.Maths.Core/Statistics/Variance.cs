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
        public static VarianceInfo Calculate(IReadOnlyList<double> samples)
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
        public static VarianceInfo Calculate(IReadOnlyList<double> samples, VarianceMode mode)
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
        public static VarianceInfo Calculate(IReadOnlyList<double> samples, VarianceMode mode, VarianceAlgorithm algorithm)
        {
            if (mode == VarianceMode.Biased && samples.Count <= 0)
                return VarianceInfo.NaN;
            else if (mode == VarianceMode.Unbiased && samples.Count <= 1)
                return VarianceInfo.NaN;

            //# https://en.wikipedia.org/wiki/Algorithms_for_calculating_variance

            var partial_variance_info = new VarianceInfo();

            if (algorithm == VarianceAlgorithm.Online)
                partial_variance_info = M1M2_OnlineAlgorithm(samples);
            else
                throw new NotSupportedException($"{algorithm} is not supported");

            if (mode == VarianceMode.Biased)
                partial_variance_info.M2Denominator = samples.Count;
            else
                partial_variance_info.M2Denominator = (samples.Count - 1);

            return partial_variance_info;
        }

        static VarianceInfo M1M2_OnlineAlgorithm(IReadOnlyList<double> samples)
        {
            var M1 = 0.0;
            var M2 = 0.0;

            for (var n = 0; n < samples.Count;)
            {
                var x = samples[n];
                n++;
                var delta = x - M1;
                M1 += delta / n;
                M2 += delta * (x - M1);
            }

            var result = new VarianceInfo(M1, M2, double.NaN);
            return result;
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
        public static IObservable<VarianceInfo> Calculate(IObservable<double> samples)
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
        public static IObservable<VarianceInfo> Calculate(IObservable<double> samples, VarianceMode mode)
        {
            var count = 0;

            return M1M2_OnlineAlgorithm(samples).Select(M1M2 =>
            {
                count++;

                if (mode == VarianceMode.Biased && count <= 0)
                {
                    return VarianceInfo.NaN;
                }
                else if (mode == VarianceMode.Unbiased && count <= 1)
                {
                    return VarianceInfo.NaN;
                }

                if (mode == VarianceMode.Biased)
                    M1M2.M2Denominator = count;
                else
                    M1M2.M2Denominator = (count - 1);

                return M1M2;
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
        public static IObservable<VarianceInfo> Calculate(IObservable<double> samples, VarianceMode mode, VarianceAlgorithm algorithm)
        {
            return Observable.Create<VarianceInfo>(o =>
            {
                var count = 0;
                VarianceInfo m1m2 = new VarianceInfo();

                var M2_Observable = (IObservable<VarianceInfo>)null;

                if (algorithm == VarianceAlgorithm.Online)
                    M2_Observable = M1M2_OnlineAlgorithm(samples);
                else
                    throw new NotSupportedException($"{algorithm} is not supported");

                M2_Observable.Subscribe(M2 =>
                {
                    count++;

                    if (mode == VarianceMode.Biased && count <= 0)
                    {
                        m1m2 = VarianceInfo.NaN;
                        o.OnNext(m1m2);
                        return;
                    }
                    else if (mode == VarianceMode.Unbiased && count <= 1)
                    {
                        m1m2 = VarianceInfo.NaN;
                        o.OnNext(m1m2);
                        return;
                    }

                    if (mode == VarianceMode.Biased)
                        m1m2.M2Denominator = count;
                    else
                        m1m2.M2Denominator = (count - 1);

                    o.OnNext(m1m2);
                });

                return Disposable.Empty;
            });
        }

        static IObservable<VarianceInfo> M1M2_OnlineAlgorithm(IObservable<double> samples)
        {
            var m1m2 = new VarianceInfo();

            return samples.Select(x =>
            {
                m1m2.Count++;
                var delta = x - m1m2.M1;
                m1m2.M1 += delta / m1m2.Count;
                m1m2.M2 += delta * (x - m1m2.M1);

                return m1m2;
            });
        }

        #endregion
    }
}
