using SquaredInfinity.Maths.Space2D;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace SquaredInfinity.Extensions
{
    public static class RectangleExtensions
    {
        public static Rect ToRect(this Rectangle rectangle)
        {
            return new Rect(rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height);
        }
         
        public static Size ToSize(this Rectangle rectangle)
        {
            return new Size(rectangle.Width, rectangle.Height);
        }
    }
}
