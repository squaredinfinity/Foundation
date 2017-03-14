using SquaredInfinity.Foundation.Maths;
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
            var actual_rectangle = new Rectangle(x1, y1, width, height);
            actual_rectangle.Clip(bounds);

            if (actual_rectangle.IsEmpty)
                return;

            DrawRectangle((int)actual_rectangle.X, (int)actual_rectangle.Y, (int)actual_rectangle.Width, (int)actual_rectangle.Height, color);
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
    }
}
