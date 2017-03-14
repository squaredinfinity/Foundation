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
    }
}
