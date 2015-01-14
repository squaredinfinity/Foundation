using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Foundation.Media.Drawing
{
    public partial class
#if UNSAFE
        UnsafePixelCanvas
#else
 PixelCanvas
#endif
    {
        public void DrawLine(int x1, int y1, int x2, int y2, int color)
        {
            DrawLineDDA(x1, y1, x2, y2, color);
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
            //# calculate dx and dy
            var dx = x2 - x1;
            var dy = y2 - y1;

            //# calculate number of steps as greather of max(abs(dx), abs(dy))
            var dx_abs = dx;
            if (dx_abs < 0)
                dx_abs = -dx;

            var dy_abs = dy;
            if (dy_abs < 0)
                dy_abs = -dy;

            var steps = dx_abs;

            if (dx_abs < dy_abs)
                steps = dy_abs;

            if (steps == 0)
                return;

            var x_step = dx / steps;
            var y_step = dy / steps;

            var x = x1;
            var y = y1;            

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
