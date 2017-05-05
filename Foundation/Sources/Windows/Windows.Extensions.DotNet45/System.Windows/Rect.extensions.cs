using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SquaredInfinity.Extensions
{
    public static class RectExtensions
    {
        public static bool IsCloseTo(this Rect r1, Rect r2)
        {
            if (r1.IsEmpty)
                return r2.IsEmpty;

            if (!r2.IsEmpty && r1.X.IsCloseTo(r2.X) 
                && 
                (r1.Y.IsCloseTo(r2.Y) && r1.Height.IsCloseTo(r2.Height)))
                return r1.Width.IsCloseTo(r2.Width);
            
            return false;
        }
    }
}
