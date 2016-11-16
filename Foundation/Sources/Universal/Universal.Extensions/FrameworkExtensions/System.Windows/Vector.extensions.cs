using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SquaredInfinity.Foundation.Extensions
{
    public static class VectorExtensions
    {
        public static bool IsClose(this Vector v1, Vector v2)
        {
            if (v1.X.IsCloseTo(v2.X))
                return v1.Y.IsCloseTo(v2.Y);
            
            return false;
        }
    }
}
