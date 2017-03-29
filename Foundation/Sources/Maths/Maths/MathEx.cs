using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Maths
{
    public static class MathEx
    {
        public static T Max<T>(T a, T b)
            where T : IComparable<T>
        {
            if (a.CompareTo(b) >= 0)
                return a;

            return b;
        }

        public static T Min<T>(T a, T b)
            where T : IComparable<T>
        {
            if (a.CompareTo(b) <= 0)
                return a;

            return b;
        }

        /// <summary>
        /// https://en.wikipedia.org/wiki/Nth_root
        /// Returns nth root of given number
        /// </summary>
        /// <returns></returns>
        public static double nrt(double d, int n)
        {
            return Math.Pow(d, 1.0 / n);
        }
    }
}
