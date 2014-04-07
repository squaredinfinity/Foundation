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
        /// <summary>
        /// Default precision used when comparing two double numbers.
        /// </summary>
        public const double DefaultPrecision = 0.00000000001d;

        /// <summary>
        /// Checks if two double precision numbers are equal given the precision of [DoubleExtensions.DefaultPrecision]
        /// For example: 0.01 != 0.1 * 0.1 due to floating point accuracy problems [http://en.wikipedia.org/wiki/Floating_point#Accuracy_problems] (0.1 * 0.1 == 0.010000000000000002)
        /// </summary>
        /// <param name="x1">The x1.</param>
        /// <param name="x2">The x2.</param>
        /// <returns></returns>
        public static bool AreVeryClose(double x1, double x2)
        {
            return AreVeryClose(x1, x2, DefaultPrecision);
        }

        /// <summary>
        /// Checks if two double precision numbers are equal given some precision.
        /// For example: 0.01 != 0.1 * 0.1 due to floating point accuracy problems [http://en.wikipedia.org/wiki/Floating_point#Accuracy_problems] (0.1 * 0.1 == 0.010000000000000002)
        /// </summary>
        /// <param name="x1">The x1.</param>
        /// <param name="x2">The x2.</param>
        /// <param name="precision">The precision.</param>
        /// <returns></returns>
        public static bool AreVeryClose(double x1, double x2, double precision)
        {
            var d = Math.Abs(x1 - x2);

            return (d <= precision || d == 0);
        }
    }
}
