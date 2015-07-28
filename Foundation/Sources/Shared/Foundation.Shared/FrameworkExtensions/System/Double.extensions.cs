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
        /// It is a very small (the smallest possible) number for which 1.0 + DefaultPrecision != 1.0
        /// The distance from 1.0 to the next largest double-precision number, that is eps = 2^(-52). 
        /// Double Mantissa is 52 bits long.
        /// http://msdn.microsoft.com/en-us/library/System.Double(v=vs.110).aspx
        /// </summary>
        public const double DefaultPrecision = 2.220446049250313080847263336181640625E-16;

        /// <summary>
        /// Checks if two double precision numbers are equal given the precision of [DoubleExtensions.DefaultPrecision]
        /// For example: 0.01 != 0.1 * 0.1 due to floating point accuracy problems [http://en.wikipedia.org/wiki/Floating_point#Accuracy_problems] (0.1 * 0.1 == 0.010000000000000002)
        /// </summary>
        /// <param name="d1">The d1.</param>
        /// <param name="d2">The d2.</param>
        /// <returns></returns>
        public static bool AreClose(double d1, double d2)
        {
            return AreClose(d1, d2, DefaultPrecision);
        }

        /// <summary>
        /// Checks if two double precision numbers are equal given some precision.
        /// For example: 0.01 != 0.1 * 0.1 due to floating point accuracy problems [http://en.wikipedia.org/wiki/Floating_point#Accuracy_problems] (0.1 * 0.1 == 0.010000000000000002)
        /// </summary>
        /// <param name="d1">The d1.</param>
        /// <param name="d2">The d2.</param>
        /// <param name="precision">The precision.</param>
        /// <returns></returns>
        public static bool AreClose(double d1, double d2, double precision)
        {
            var d = Math.Abs(d1 - d2);

            return (d <= precision || d == 0);
        }

        /// <summary>
        /// Checks if double precision number is equal to another double precision number given the precision of [DoubleExtensions.DefaultPrecision].
        /// For example: 0.01 != 0.1 * 0.1 due to floating point accuracy problems [http://en.wikipedia.org/wiki/Floating_point#Accuracy_problems] (0.1 * 0.1 == 0.010000000000000002)
        /// </summary>
        /// <param name="d1">The d1.</param>
        /// <param name="d2">The d2.</param>
        /// <param name="precision">The precision.</param>
        /// <returns></returns>
        public static bool IsCloseTo(this double d1, double d2)
        {
            return AreClose(d1, d2);
        }

        /// <summary>
        /// Checks if double precision number is equal to another double precision number given some precision.
        /// For example: 0.01 != 0.1 * 0.1 due to floating point accuracy problems [http://en.wikipedia.org/wiki/Floating_point#Accuracy_problems] (0.1 * 0.1 == 0.010000000000000002)
        /// </summary>
        /// <param name="d1">The d1.</param>
        /// <param name="d2">The d2.</param>
        /// <param name="precision">The precision.</param>
        /// <returns></returns>
        public static bool IsCloseTo(this double d1, double d2, double precision)
        {
            return AreClose(d1, d2, precision);
        }

        /// <summary>
        /// Checks if double precision number is less than another double precision number given the precision of [DoubleExtensions.DefaultPrecision].
        /// For example: 0.01 != 0.1 * 0.1 due to floating point accuracy problems [http://en.wikipedia.org/wiki/Floating_point#Accuracy_problems] (0.1 * 0.1 == 0.010000000000000002)
        /// </summary>
        /// <param name="d1">The d1.</param>
        /// <param name="d2">The d2.</param>
        /// <param name="precision">The precision.</param>
        /// <returns></returns>
        public static bool IsLessThan(this double d1, double d2)
        {
            if (d1 < d2)
                return !AreClose(d1, d2);

            return false;
        }

        /// <summary>
        /// Checks if double precision number is less than another double precision number given some precision.
        /// For example: 0.01 != 0.1 * 0.1 due to floating point accuracy problems [http://en.wikipedia.org/wiki/Floating_point#Accuracy_problems] (0.1 * 0.1 == 0.010000000000000002)
        /// </summary>
        /// <param name="d1">The d1.</param>
        /// <param name="d2">The d2.</param>
        /// <param name="precision">The precision.</param>
        /// <returns></returns>
        public static bool IsLessThan(this double d1, double d2, double precision)
        {
            if (d1 < d2)
                return !AreClose(d1, d2, precision);

            return false;
        }

        /// <summary>
        /// Checks if double precision number is greater than another double precision number given the precision of [DoubleExtensions.DefaultPrecision].
        /// For example: 0.01 != 0.1 * 0.1 due to floating point accuracy problems [http://en.wikipedia.org/wiki/Floating_point#Accuracy_problems] (0.1 * 0.1 == 0.010000000000000002)
        /// </summary>
        /// <param name="d1">The d1.</param>
        /// <param name="d2">The d2.</param>
        /// <param name="precision">The precision.</param>
        /// <returns></returns>
        public static bool IsGreaterThan(this double d1, double d2)
        {
            if (d1 > d2)
                return !AreClose(d1, d2);

            return false;
        }

        /// <summary>
        /// Checks if double precision number is greater than another double precision number given some precision.
        /// For example: 0.01 != 0.1 * 0.1 due to floating point accuracy problems [http://en.wikipedia.org/wiki/Floating_point#Accuracy_problems] (0.1 * 0.1 == 0.010000000000000002)
        /// </summary>
        /// <param name="d1">The d1.</param>
        /// <param name="d2">The d2.</param>
        /// <param name="precision">The precision.</param>
        /// <returns></returns>
        public static bool IsGreaterThan(this double d1, double d2, double precision)
        {
            if (d1 > d2)
                return !AreClose(d1, d2, precision);

            return false;
        }

        /// <summary>
        /// Checks if double precision number is less than or equal to another double precision number given the precision of [DoubleExtensions.DefaultPrecision].
        /// For example: 0.01 != 0.1 * 0.1 due to floating point accuracy problems [http://en.wikipedia.org/wiki/Floating_point#Accuracy_problems] (0.1 * 0.1 == 0.010000000000000002)
        /// </summary>
        /// <param name="d1">The d1.</param>
        /// <param name="d2">The d2.</param>
        /// <param name="precision">The precision.</param>
        /// <returns></returns>
        public static bool IsLessThanOrClose(this double d1, double d2)
        {
            if (d1 >= d2)
                return AreClose(d1, d2);

            return true;
        }

        /// <summary>
        /// Checks if double precision number is less than or equal to another double precision number given some precision.
        /// For example: 0.01 != 0.1 * 0.1 due to floating point accuracy problems [http://en.wikipedia.org/wiki/Floating_point#Accuracy_problems] (0.1 * 0.1 == 0.010000000000000002)
        /// </summary>
        /// <param name="d1">The d1.</param>
        /// <param name="d2">The d2.</param>
        /// <param name="precision">The precision.</param>
        /// <returns></returns>
        public static bool IsLessThanOrClose(this double d1, double d2, double precision)
        {
            if (d1 >= d2)
                return AreClose(d1, d2, precision);

            return true;
        }

        /// <summary>
        /// Checks if double precision number is greater than or equal to another double precision number given the precision of [DoubleExtensions.DefaultPrecision].
        /// For example: 0.01 != 0.1 * 0.1 due to floating point accuracy problems [http://en.wikipedia.org/wiki/Floating_point#Accuracy_problems] (0.1 * 0.1 == 0.010000000000000002)
        /// </summary>
        /// <param name="d1">The d1.</param>
        /// <param name="d2">The d2.</param>
        /// <param name="precision">The precision.</param>
        /// <returns></returns>
        public static bool IsGreaterThanOrClose(this double d1, double d2)
        {
            if (d1 <= d2)
                return AreClose(d1, d2);

            return true;
        }

        /// <summary>
        /// Checks if double precision number is greater than or equal to another double precision number given some precision.
        /// For example: 0.01 != 0.1 * 0.1 due to floating point accuracy problems [http://en.wikipedia.org/wiki/Floating_point#Accuracy_problems] (0.1 * 0.1 == 0.010000000000000002)
        /// </summary>
        /// <param name="d1">The d1.</param>
        /// <param name="d2">The d2.</param>
        /// <param name="precision">The precision.</param>
        /// <returns></returns>
        public static bool IsGreaterThanOrClose(this double d1, double d2, double precision)
        {
            if (d1 <= d2)
                return AreClose(d1, d2, precision);

            return true;
        }

        /// <summary>
        /// Returns True if double is Infinity (positive or negative) or NaN
        /// </summary>
        /// <param name="d"></param>
        /// <returns></returns>
        public static bool IsInfinityOrNaN(this double d)
        {
            if (double.IsInfinity(d))
                return true;

            return double.IsNaN(d);
        }

        /// <summary>
        /// Returns True if double is Infinity (positive or negative)
        /// </summary>
        /// <param name="d"></param>
        /// <returns></returns>
        public static bool IsInfinity(this double d)
        {
            return double.IsInfinity(d);
        }

        public static double GetValueOrDefaultIfInfinityOrNaN(this double? d, double defaultValue)
        {
            if (d == null)
                return defaultValue;

            if (d.Value.IsInfinityOrNaN())
                return defaultValue;

            return d.Value;
        }

        public static double GetValueOrDefaultIfInfinityOrNaN(this double d, double defaultValue)
        {
            if (d.IsInfinityOrNaN())
                return defaultValue;

            return d;
        }

        /// <summary>
        /// Returns next double value
        /// </summary>
        /// <param name="d"></param>
        /// <returns></returns>
        public static double NextDouble(this double d)
        {
            var bits = BitConverter.DoubleToInt64Bits(d);

            long next_bits;

            // if Int64 constructed from double bits is non-negative, then we can just add '1' to go to the next double number
            if (bits >= 0)
                next_bits = bits + 1;

            // if Int64 constructed from double is equal long.MinValue (-0) then next available double representation is the same as (In64)1
            else if (bits == long.MinValue)
                next_bits = 1;
            
            // number is negative, we must substract Int64 representation by '1' in order for double to be increased by '1 step'
            else  // number is negative, so decrement to go "up"
                next_bits = bits - 1;

            return BitConverter.Int64BitsToDouble(next_bits);
        }

        /// <summary>
        /// Clamps specified value to be within specified range [min,max]
        /// </summary>
        /// <param name="d"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static double Clamp(this double d, double inclusiveMin, double inclusiveMax, double? NaN_value = null)
        {
            if ((NaN_value == null && double.IsNaN(d)))
                return inclusiveMin;

            return Math.Max(inclusiveMin, Math.Min(d, inclusiveMax));
        }

        public static double RoundDown(this double d)
        {
            return RoundDown(d, 0);
        }

        public static double RoundDown(this double d, int precision)
        {
            if (d.IsCloseTo(0.0))
                return 0.0;

            var original_precision = (int)Math.Floor(Math.Log10(Math.Abs(d)));

            var modifier = (0 - original_precision) * -1;

            // normalize to '0 precision' - most significant digit is a unit
            var value = d * Math.Pow(10, original_precision - precision - modifier);

            // floor
            value = Math.Floor(value);

            // denormalize

            value = value * Math.Pow(10, precision - original_precision + modifier);

            return value;
        }

        public static double RoundUp(this double d)
        {
            return RoundUp(d, 0);
        }

        public static double RoundUp(this double d, int precision)
        {
            if (d.IsCloseTo(0.0))
                return 0.0;

            var original_precision = (int)Math.Floor(Math.Log10(Math.Abs(d)));

            var modifier = (0 - original_precision) * -1;

            // normalize to '0 precision' - most significant digit is a unit
            var value = d * Math.Pow(10, original_precision - precision - modifier);
            
            // ceiling
            value = Math.Ceiling(value);

            // denormalize

            value = value * Math.Pow(10, precision - original_precision + modifier);

            return value;
        }
        
    }
}
