using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SquaredInfinity.Foundation.Extensions
{
    public static class SizeExtensions
    {
        public static bool IsCloseTo(this Size s1, Size s2)
        {
            if (s1.Width.IsCloseTo(s2.Width))
                return s1.Height.IsCloseTo(s2.Height);
            
            return false;
        }

        public static bool IsEmptyOrZeroArea(this Size s)
        {
            if (s.IsEmpty)
                return true;

            return s.Width == 0 && s.Height == 0;
        }

        public static bool IsInfinite(this Size s)
        {
            return double.IsInfinity(s.Width) && double.IsInfinity(s.Height);
        }
    }
}
