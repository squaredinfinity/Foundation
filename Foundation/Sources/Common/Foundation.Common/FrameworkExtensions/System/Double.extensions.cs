﻿using System;
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
    }
}