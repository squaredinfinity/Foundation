﻿using SquaredInfinity.Foundation.Maths.Space2D;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SquaredInfinity.Foundation.Graphics.Drawing
{
    public partial class
#if UNSAFE
        UnsafePixelCanvas
#else
 PixelCanvas
#endif
    {
        public void DrawRectangle(Rectangle bounds, int x1, int y1, int width, int height, int color)
        {
            var x1_d = (double)x1;
            var y1_d = (double)y1;
            var x2_d = (double)x1 + width;
            var y2_d = (double)y1 + height;

            if (!TryCohenSutherlandClip(bounds, ref x1_d, ref y1_d, ref x2_d, ref y2_d))
                return;

            DrawRectangle((int)x1_d, (int)y1_d, (int)(x2_d -x1_d), (int)(y2_d - y1_d), color);
        }

        /// <summary>
        /// Draws a rectangle at specified x and y coordinates.
        /// This method does not perform any checks and will throw if rectangle is outside of canvas bounds.
        /// If this is the case use an overload that accepts Bounds
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="color"></param>
        public void DrawRectangle(int x, int y, int width, int height, int color)
        {
            var x2 = x + width;
            var y2 = y + height;

            for (; x < x2; x++)
            {
                for (int _y = y; _y < y2; _y++)
                {
                    this[x, _y] = color;
                }
            }
        }

        public void DrawLine(int x1, int y1, int x2, int y2, int color)
        {
            DrawLineDDA(x1, y1, x2, y2, color);
        }

        public void DrawLineDDA(Rectangle bounds, int x1, int y1, int x2, int y2, int color)
        {
            var x1_d = (double)x1;
            var y1_d = (double)y1;
            var x2_d = (double)x2;
            var y2_d = (double)y2;

            if (!TryCohenSutherlandClip(bounds, ref x1_d, ref y1_d, ref x2_d, ref y2_d))
                return;

            DrawLineDDA((int)x1_d, (int)y1_d, (int)x2_d, (int)y2_d, color);
        }

        /// <summary>
        /// Digital Differential Analyzer
        /// http://en.wikipedia.org/wiki/Digital_differential_analyzer_(graphics_algorithm)
        /// </summary>
        /// <param name="bmp"></param>
        /// <param name="x1"></param>
        /// <param name="y1"></param>
        /// <param name="x2"></param>
        /// <param name="y2"></param>
        /// <param name="color"></param>
        public void DrawLineDDA(int x1, int y1, int x2, int y2, int color)
        {
            // http://en.wikipedia.org/wiki/Bresenham%27s_line_algorithm

            // cartesian slope-intercept http://en.wikipedia.org/wiki/Linear_equation#Slope.E2.80.93intercept_form
            // y = mx + b
            // m - slope of the line
            // b - y intercept (y coordinate of location where the line crosses the y axis)

            // write above equation as function of both x and y
            // slope is the "rise over run" or dx / dy
            //
            // y = mx + b
            // y = dy/dx * x + b
            // dx * y = dy * x + dx * b
            // 0 = dy*x - dx*y + dx*b
            // f(x,y) = 0 = Ax + By + C
            //  where
            //  A = dy
            //  B = -dx
            //  C = dx*b

            var dx = x2 - x1;
            var dy = y2 - y1;

         
            //# calculate number of steps as greather of max(abs(dx), abs(dy))
            // use maximum possible number of steps for better accuracy
            // do not user Math class here for quicker execution
            var dx_abs = dx;
            if (dx_abs < 0)
                dx_abs = -dx;

            var dy_abs = dy;
            if (dy_abs < 0)
                dy_abs = -dy;

            // calculate x and y steps

            var x_step = 0.0f;
            var y_step = 0.0f;

            var steps = dx_abs;            
            if (dx_abs < dy_abs)
                steps = dy_abs;

            x_step = dx / (float)steps;
            y_step = dy / (float)steps;

            if (steps == 0)
                return;

            // line start point
            var x = (float)x1;
            var y = (float)y1;

            
            for (int i = 0; i <= steps; i++)
            {
                if (y < _height && y >= 0 && x < _width && x >= 0)
                {
                    this[(int)y * _width + (int)x] = color;
                }

                x += x_step;
                y += y_step;
            }
        }
    }
}
