using SquaredInfinity.Maths;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SquaredInfinity.Extensions
{
    public static class RectangleExtensions
    {
        public static Rect ToRect(this Rectangle r)
        {
            return new Rect(r.X, r.Y, r.Width, r.Height);
        }
    }
}
