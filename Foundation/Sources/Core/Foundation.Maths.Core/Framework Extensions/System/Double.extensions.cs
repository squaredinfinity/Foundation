using System;
using System.Collections.Generic;
using System.Globalization;
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
        /// Maximum allowed order of magnitude of a double number.
        /// </summary>
        /// <remarks>
        /// (int)Math.Floor(Math.Log10(double.MaxValue))
        /// </remarks>
        public static int MaxOrderOfMagnitude = 308;

        /// <summary>
        /// Default precision used when comparing two double numbers.
        /// It is a very small (the smallest possible) number for which 1.0 + DefaultPrecision != 1.0
        /// The distance from 1.0 to the next largest double-precision number, that is eps = 2^(-52). 
        /// Double Mantissa is 52 bits long.
        /// http://msdn.microsoft.com/en-us/library/System.Double(v=vs.110).aspx
        /// </summary>
        public const double DefaultPrecision = 2.220446049250313080847263336181640625E-16;

        public const double MaxSignificantFigures = 15;

        /// <summary>
        /// Checks if two double precision numbers are equal given the precision of [DoubleExtensions.DefaultPrecision]
        /// For example: 0.01 != 0.1 * 0.1 due to floating point accuracy problems [http://en.wikipedia.org/wiki/Floating_point#Accuracy_problems] (0.1 * 0.1 == 0.010000000000000002)
        /// By default comparing same special values (NaN vs NaN, -Infinity vs -Infinity etc.) will return false.
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
        /// In addition checks if two doubles represent same special value (e.g. -Infinity == -Infinity, but -Infintiy != +Infinity)
        /// -Infintiy == -Infintiy
        /// +Infinity == +Infinity
        /// NaN == NaN
        /// </summary>
        /// <param name="d1">The d1.</param>
        /// <param name="d2">The d2.</param>
        /// <param name="precision">The precision.</param>
        /// <returns></returns>
        public static bool IsCloseToOrSameSpecialValue(this double d1, double d2)
        {
            if (AreClose(d1, d2))
                return true;

            if (double.IsNaN(d1) && double.IsNaN(d2))
                return true;

            if (double.IsNegativeInfinity(d1) && double.IsNegativeInfinity(d2))
                return true;

            if (double.IsPositiveInfinity(d1) && double.IsPositiveInfinity(d2))
                return true;

            return false;
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
            if (double.IsNaN(d))
            {
                if ((NaN_value == null))
                    return inclusiveMin;
                else
                    return NaN_value.Value;
            }

            return Math.Max(inclusiveMin, Math.Min(d, inclusiveMax));
        }

        public static double RoundDown(this double d)
        {
            return RoundDown(d, 0);
        }

        public static double RoundToSignificantFigures(this double d, int numberOfSignificantFigures, MidpointRounding mode = MidpointRounding.AwayFromZero)
        {
            if (numberOfSignificantFigures > MaxSignificantFigures)
                throw new ArgumentException($"{nameof(numberOfSignificantFigures)} must be <= {MaxSignificantFigures}");

            // double is accurate to MaxSignificantFigures by default
            // it will automatically do the rounding of 15th figure
            // so there's nothing to do here
            //if (numberOfSignificantFigures == MaxSignificantFigures)
            //    return d;

            //  NOTE:
            //      s - significant figure to keep
            //      r - digit to use for rounding of last significant figure
            //      i - digit to ignore
            //
            //      Cannot use Math.Round() here as it only rounds to maximum 15 fraction digits
            //      This means that integers with more than 15 digits wouldn't be rounded
            //      as well as fractions such as 0.000xxxxx... - in this case at most (15 - 3) significant fraction digits can be used for rounding (because of .000)

            //  integer part of the fraction contains all required significant figures and some
            //  ssssssssssriii

            // work on positive numbers from now on
            var n = Math.Abs(d);

            var order_of_magnitude = (int)Math.Floor(Math.Log10(n));

            var omg = order_of_magnitude;

            var result = (decimal)n;

            // double is only precise to 15 digits (17 internally)
            for(int i = order_of_magnitude - 15; i < order_of_magnitude - numberOfSignificantFigures; i++)
            {
                var digit = n.DigitAt(i);
                var m = digit * Math.Pow(10, i);

                result -= (decimal)m;
            }

            // get next digit for rounding
            var r = d.DigitAt(order_of_magnitude - numberOfSignificantFigures);

            // add the result of rounding of 0.r to the r+1 position in the finial integer
            // 0.r rounded will be either 1 or 0

            var r_rounded = (int)Math.Round(((double)r / 10), 0, mode);

            if (r_rounded != 0)
                result += (r_rounded * (decimal)Math.Pow(10, order_of_magnitude - numberOfSignificantFigures + 1));

            var shift_magnitude = Math.Abs(order_of_magnitude) - 1 + numberOfSignificantFigures;

            result *= (decimal)Math.Pow(10, shift_magnitude);

            result = Math.Truncate(result);

            result *= (decimal)Math.Pow(10, shift_magnitude * -1);

            if (d < 0)
                result *= -1;

            return (double)result;
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

        /// <summary>
        /// Returns order of magnitude of specified number
        /// https://en.wikipedia.org/wiki/Order_of_magnitude
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        static int OrderOfMagnitude(this double number)
        {
            // todo: handle 0 ?

            return (int)Math.Floor(Math.Log10(Math.Abs(number)));
        }

        /// <summary>
        /// Returns digit at specified position.
        /// </summary>
        /// <param name="number"></param>
        /// <param name="orderOfMagnitude"></param>
        /// <returns></returns>
        public static int DigitAt(this double number, int orderOfMagnitude)
        {
            if (number.IsCloseTo(0.0))
                return 0;

            //  NOTE:
            //          omg - Order of Magnitude
            //
            //  ASSUMPTIONS:
            //          1. double has 15-16 digits of precision
            //          2. decimal has 28-29 digits of precision
            //          3. double has larger range of values than decimal
            // 
            //  ALGORITHM:
            //          1. bring 15 most significant digits from double into a range supported by decimal
            //          2. perform operations on decimal type to avoid rounding errors

            // work on positive numbers (because Log10 doesn't work with negative numbers)
            var n = Math.Abs(number);

            // get omg of n
            var n_order_of_magnitude = (int)Math.Floor(Math.Log10(Math.Abs(n)));

            // requested omg greater than n_omg
            if (n_order_of_magnitude < orderOfMagnitude)
                return 0;

            if (Math.Abs(n_order_of_magnitude) > 28)
            {
                throw new NotSupportedException("Double numbers with order of magnitude > 28 and < -28 are not supported.");
            }

            // n_omg is >= requested omg
            var omg_diff = n_order_of_magnitude - orderOfMagnitude;

            return (int)(Math.Truncate((decimal)n / (decimal)Math.Pow(10, n_order_of_magnitude - omg_diff)) % 10);
        }

        /// <summary>
        /// Round towards zero.
        /// </summary>
        /// <param name="d"></param>
        /// <returns></returns>
        public static double Fix(this double d)
        {
            if (d > 0.0)
                return d.RoundDown();
            else
                return d.RoundUp();
        }

        ///// <summary>
        ///// Zeroes least significant digits in a given number starting from a digit at given position.
        ///// </summary>
        ///// <param name="number"></param>
        ///// <param name="inclusiveStartPosition">Position is 0 for units, 1 for tens, 2 for hundereds etc.</param>
        ///// <returns></returns>
        //static double (this double number, int inclusiveStartPosition)
        //{
        //    if (number.IsCloseTo(0.0))
        //        return 0;

        //    return CheatingZeroLeastSignificantDigits(number, inclusiveStartPosition);

        //    //var n = Math.Abs(number);

        //    //var result = 0.0;

        //    //var pos = (int)Math.Floor(Math.Log10(n));

        //    //while (pos > inclusiveStartPosition)
        //    //{
        //    //    var part_digit = (int)(n / Math.Pow(10, pos) % 10);

        //    //    var part = part_digit * Math.Pow(10, pos);

        //    //    n -= part;

        //    //    if (Math.Log10(Math.Abs(n)) < -16 && pos > inclusiveStartPosition)
        //    //    {
        //    //        // number has less digits than start position,
        //    //        // return the number itself

        //    //        return number;
        //    //    }

        //    //    result += part;

        //    //    pos = (int)Math.Floor(Math.Log10(n));
        //    //}

        //    //// result is always positive at this point,
        //    //// update the sign if needed
        //    //if(number < 0)
        //    //    result *= -1;

        //    //return result;
        //}

        #region Cheating


        /// <summary>
        /// Returns digit at specified position.
        /// </summary>
        /// <param name="number"></param>
        /// <param name="position">the position of digit to return. units position is 0, tens position is 1, hunderets position is 2 etc.</param>
        /// <returns></returns>
        static int CheatingDigitAt(this double number, int position)
        {
            if (number.IsInfinityOrNaN())
                throw new ArgumentException("number cannot be NaN or Infinity");

            var str = number.ToString("G99", CultureInfo.InvariantCulture);

            var fraction_separator_position = str.IndexOf(",");

            var position_from_begining_of_string = 0;

            if (fraction_separator_position == -1)
            {
                // no fraction part, last item in chars array has position == 0 (units)
                position_from_begining_of_string = str.Length - 1 - position;
            }
            else
            {
                position_from_begining_of_string = fraction_separator_position - position;
            }

            var result = int.Parse(str[position_from_begining_of_string].ToString(), CultureInfo.InvariantCulture);

            return result;
        }

        static double CheatingZeroLeastSignificantDigits(this double number, int inclusiveStartPosition)
        {
            if (number.IsInfinityOrNaN())
                return number;

            var str = number.ToString("G99", CultureInfo.InvariantCulture);

            var chars = str.ToCharArray();

            var fraction_separator_position = str.IndexOf(",");

            var start_position_from_begining_of_array = 0;

            if (fraction_separator_position == -1)
            {
                // no fraction part, last item in chars array has position == 0 (units)
                start_position_from_begining_of_array = chars.Length - 1 - inclusiveStartPosition;
            }
            else
            {
                start_position_from_begining_of_array = fraction_separator_position - inclusiveStartPosition;
            }

            for (int i = start_position_from_begining_of_array; i < chars.Length; i++)
                chars[i] = '0';

            return double.Parse(new string(chars), CultureInfo.InvariantCulture);
        }

        #endregion
    }
}
