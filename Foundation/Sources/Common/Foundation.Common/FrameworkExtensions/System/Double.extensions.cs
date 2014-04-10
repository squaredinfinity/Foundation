using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SquaredInfinity.Foundation.Extensions
{
    /// <summary>
    /// Extensions of a Double type.
    /// </summary>
    public static partial class DoubleExtensions
    {
        const double DBL_EPSILON = 2.22044604925031E-16;
        const float  FLT_MIN = 1.175494E-38f;

        /// <summary>
        /// Default precision used when comparing two double numbers.
        /// </summary>
        public const double DefaultPrecision = 0.00000000001d;

        /// <summary>
        /// Checks if two double precision numbers are equal given the precision of [DoubleExtensions.DefaultPrecision]
        /// For example: 0.01 != 0.1 * 0.1 due to floating point accuracy problems [http://en.wikipedia.org/wiki/Floating_point#Accuracy_problems] (0.1 * 0.1 == 0.010000000000000002)
        /// </summary>
        /// <param name="d1">The x1.</param>
        /// <param name="d2">The x2.</param>
        /// <returns></returns>
        public static bool AreVeryClose(double d1, double d2)
        {
            return AreVeryClose(d1, d2, DefaultPrecision);
        }

        /// <summary>
        /// Checks if two double precision numbers are equal given some precision.
        /// For example: 0.01 != 0.1 * 0.1 due to floating point accuracy problems [http://en.wikipedia.org/wiki/Floating_point#Accuracy_problems] (0.1 * 0.1 == 0.010000000000000002)
        /// </summary>
        /// <param name="d1">The x1.</param>
        /// <param name="d2">The x2.</param>
        /// <param name="precision">The precision.</param>
        /// <returns></returns>
        public static bool AreVeryClose(double d1, double d2, double precision)
        {
            var d = Math.Abs(d1 - d2);

            return (d <= precision || d == 0);
        }

        public static bool IsVeryCloseTo(this double d1, double d2)
        {
            return AreVeryClose(d1, d2);
        }

        public static bool IsLessThan(this double d1, double d2)
        {
            if (d1 < d2)
                return !AreVeryClose(d1, d2);

            return false;
        }

        public static bool IsGreaterThan(this double d1, double d2)
        {
            if (d1 > d2)
                return !AreVeryClose(d1, d2);

            return false;
        }

        public static bool IsLessThanOrClose(this double d1, double d2)
        {
            if (d1 >= d2)
                return AreVeryClose(d1, d2);

            return true;
        }

        public static bool IsGreaterThanOrClose(this double d1, double d2)
        {
            if (d1 <= d2)
                return AreVeryClose(d1, d2);

            return true;
        }
    }
}
