using SquaredInfinity.Maths;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SquaredInfinity.Extensions
{
    public static class Point2DExtensions
    {
        public static Point ToPoint(this Point2D p)
        {
            return new Point(p.X, p.Y);
        }
    }
}
