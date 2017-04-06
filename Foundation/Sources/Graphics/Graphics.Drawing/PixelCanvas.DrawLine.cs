using SquaredInfinity.Maths;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SquaredInfinity.Graphics.Drawing
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
            DrawLine(Bounds, x1, y1, x2, y2, color, 1);
        }

        public void DrawLine(int x1, int y1, int x2, int y2, int color, int thickness)
        {
            DrawLine(Bounds, x1, y1, x2, y2, color, thickness);
        }

        public void DrawLine(Rectangle bounds, int x1, int y1, int x2, int y2, int color, int thickness)
        {
            var x1_d = (double)x1;
            var y1_d = (double)y1;
            var x2_d = (double)x2;
            var y2_d = (double)y2;

            if (!TryCohenSutherlandClip(bounds, ref x1_d, ref y1_d, ref x2_d, ref y2_d))
                return;

            if(thickness == 1)
                DrawLineDDA((int)x1_d, (int)y1_d, (int)x2_d, (int)y2_d, color);
            else
                DrawLineWu((int)x1_d, (int)y1_d, (int)x2_d, (int)y2_d, color, 
        }
    }
}
