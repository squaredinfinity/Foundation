﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using SquaredInfinity.Maths.Space2D;

namespace SquaredInfinity.Extensions
{
    public static class PointExtensions
    {
        public static Point2D ToPoint2D(this Point point)
        {            
            return new Point2D(point.X, point.Y);
        }
    }
}
