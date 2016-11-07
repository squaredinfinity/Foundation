using SquaredInfinity.Foundation.Maths.Space2D;
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
        public void DrawLineAA(Rectangle bounds, int x1, int y1, int x2, int y2, int line_thickness, int color)
        {
            var x1_d = (double)x1;
            var y1_d = (double)y1;
            var x2_d = (double)x2;
            var y2_d = (double)y2;

            if (!TryCohenSutherlandClip(bounds, ref x1_d, ref y1_d, ref x2_d, ref y2_d))
                return;

            DrawLineAA((int)x1_d, (int)y1_d, (int)x2_d, (int)y2_d, line_thickness, color);
        }

        static int[] leftEdgeX = new int[8192];
        static int[] rightEdgeX = new int[8192];

        public void DrawLineAA(int x1, int y1, int x2, int y2, int line_thickness, int color, BlendMode blendMode = BlendMode.Copy)
        {
            if (line_thickness <= 0) return;

            var source_alpha = ((color >> 24) & 0xff);

            // color is transparent, no need to draw anything at all
            if (source_alpha == 0)
                return;

            var source_red = ((color >> 16) & 0xff);
            var source_green = ((color >> 8) & 0xff);
            var source_blue = ((color) & 0xff);

            var tmp = 0;

            // ensure y1 < y2, swap points if needed

            if (y1 > y2)
            {
                // swap x
                tmp = x1;
                x1 = x2;
                x2 = tmp;

                // swap y
                tmp = y1;
                y1 = y2;
                y2 = tmp;
            }

            int destination_alpha = 0;
            int destination_red = 0;
            int destination_green = 0;
            int destination_blue = 0;

            #region Vertical Line 

            // x1 == x2
            // from before we know that y1 <= y2

            if (x1 == x2)
            {
                x1 -= line_thickness / 2;
                x2 += line_thickness / 2;

                // make sure new values don't go outside of bounds
                if (x1 < 0)
                    x1 = 0;
                if (x2 >= _width)
                    x2 = _width - 1;

                // foreach x (width pixel of a vertical line)
                for (var x = x1; x <= x2; x++)
                {
                    for (var y = y1; y <= y2; y++)
                    {
                        if (blendMode == BlendMode.Copy)
                        {
                            this[x, y] = color;
                            continue;
                        }
                        else
                        {
                            // get destination pixel
                            var dest_pixel = this[x, y];

                            destination_alpha = ((dest_pixel >> 24) & 0xff);

                            // destination is transparent or source is opaque,
                            // just replace destination pixel with source pixel
                            if (source_alpha == 255 || destination_alpha == 0)
                            {
                                this[x, y] = color;
                                continue;
                            }

                            // get destination pixel rgb values
                            destination_red = ((dest_pixel >> 16) & 0xff);
                            destination_green = ((dest_pixel >> 8) & 0xff);
                            destination_blue = ((dest_pixel) & 0xff);

                            var isa = 255 - source_alpha;

                            dest_pixel = (((((source_alpha << 8) + isa * destination_alpha) >> 8) & 0xff) << 24) |
                                            (((((source_red << 8) + isa * destination_red) >> 8) & 0xff) << 16) |
                                            (((((source_green << 8) + isa * destination_green) >> 8) & 0xff) << 8) |
                                            ((((source_blue << 8) + isa * destination_blue) >> 8) & 0xff);

                            this[x, y] = dest_pixel;
                        }
                    }
                }

                return;
            }

            #endregion

            #region Horizontal Line

            if (y1 == y2)
            {
                // ensure x1 < x2

                if (x1 > x2)
                {
                    // swap x
                    tmp = x1;
                    x1 = x2;
                    x2 = tmp;
                }

                y1 -= line_thickness / 2;
                y2 += line_thickness / 2;

                // make sure new values don't go outside of bounds
                if (y1 < 0)
                    y1 = 0;
                if (y2 >= _height)
                    x2 = _height - 1;

                for (var y = y1; y <= y2; y++)
                {
                    for (var x = x1; x <= x2; x++)
                    {
                        if (blendMode == BlendMode.Copy)
                        {
                            this[x, y] = color;
                            continue;
                        }
                        else
                        {
                            // get destination pixel
                            var dest_pixel = this[x, y];

                            destination_alpha = ((dest_pixel >> 24) & 0xff);

                            // destination is transparent or source is opaque,
                            // just replace destination pixel with source pixel
                            if (source_alpha == 255 || destination_alpha == 0)
                            {
                                this[x, y] = color;
                                continue;
                            }

                            // get destination pixel rgb values
                            destination_red = ((dest_pixel >> 16) & 0xff);
                            destination_green = ((dest_pixel >> 8) & 0xff);
                            destination_blue = ((dest_pixel) & 0xff);

                            var isa = 255 - source_alpha;

                            dest_pixel = (((((source_alpha << 8) + isa * destination_alpha) >> 8) & 0xff) << 24) |
                                            (((((source_red << 8) + isa * destination_red) >> 8) & 0xff) << 16) |
                                            (((((source_green << 8) + isa * destination_green) >> 8) & 0xff) << 8) |
                                            ((((source_blue << 8) + isa * destination_blue) >> 8) & 0xff);

                            this[x, y] = dest_pixel;
                        }
                    }
                }

                return;
            }

            #endregion

            var m = (y2 - y1) / (x2 - x1); // slope of the line

            // width of a rectangluar bounds of the line
            var dx = x2 - x1;
            // height of the rectangular bounds of the line
            var dy = y2 - y1;

            // lenght of the line
            // using Pythagorean Theorem
            //  len^2 = dx^2 + dy^2
            //  len = sqrt(dx^2 + dy^2)
            var len = Math.Sqrt(dx * dx + dy * dy);


            var x_step = (line_thickness * dy / len);
            var y_step = (line_thickness * dx / len);


            // alight points with step sizes
            x1 += (int)(x_step / 2);
            y1 -= (int)(y_step / 2);
            x2 += (int)(x_step / 2);
            y2 -= (int)(y_step / 2);

            var sx = -x_step;
            var sy = +y_step;

            var ix1 = (int)x1;
            var iy1 = (int)y1;

            var ix2 = (int)x2;
            var iy2 = (int)y2;

            var ix3 = (int)(x1 + sx);
            var iy3 = (int)(y1 + sy);

            var ix4 = (int)(x2 + sx);
            var iy4 = (int)(y2 + sy);

            if (line_thickness == 2)
            {
                if (Math.Abs(dy) < Math.Abs(dx))
                {
                    if (x1 < x2)
                    {
                        iy3 = iy1 + 2;
                        iy4 = iy2 + 2;
                    }
                    else
                    {
                        iy1 = iy3 + 2;
                        iy2 = iy4 + 2;
                    }
                }
                else
                {
                    ix1 = ix3 + 2;
                    ix2 = ix4 + 2;
                }
            }

            int starty = Math.Min(Math.Min(iy1, iy2), Math.Min(iy3, iy4));
            int endy = Math.Max(Math.Max(iy1, iy2), Math.Max(iy3, iy4));

            if (starty < 0) starty = -1;
            if (endy >= _height) endy = _height + 1;

            for (int y = starty + 1; y < endy - 1; y++)
            {
                leftEdgeX[y] = -1 << 16;
                rightEdgeX[y] = 1 << 16 - 1;
            }


            LineEdge(_width, _height, ix1, iy1, ix2, iy2, source_alpha, source_red, source_green, source_blue, minEdge: sy > 0, leftEdge: false);
            LineEdge(_width, _height, ix3, iy3, ix4, iy4, source_alpha, source_red, source_green, source_blue, minEdge: sy < 0, leftEdge: true);

            if (line_thickness > 1)
            {
                LineEdge(_width, _height, ix1, iy1, ix3, iy3, source_alpha, source_red, source_green, source_blue, minEdge: true, leftEdge: sy > 0);
                LineEdge(_width, _height, ix2, iy2, ix4, iy4, source_alpha, source_red, source_green, source_blue, minEdge: false, leftEdge: sy < 0);
            }

            if (x1 < x2)
            {
                if (iy2 >= 0 && iy2 < _height) rightEdgeX[iy2] = Math.Min(ix2, rightEdgeX[iy2]);
                if (iy3 >= 0 && iy3 < _height) leftEdgeX[iy3] = Math.Max(ix3, leftEdgeX[iy3]);
            }
            else
            {
                if (iy1 >= 0 && iy1 < _height) rightEdgeX[iy1] = Math.Min(ix1, rightEdgeX[iy1]);
                if (iy4 >= 0 && iy4 < _height) leftEdgeX[iy4] = Math.Max(ix4, leftEdgeX[iy4]);
            }

            // fill the line

            for (int y = starty + 1; y < endy - 1; y++)
            {
                leftEdgeX[y] = Math.Max(leftEdgeX[y], 0);
                rightEdgeX[y] = Math.Min(rightEdgeX[y], _width - 1);

                for (int x = leftEdgeX[y]; x <= rightEdgeX[y]; x++)
                {
                    if (blendMode == BlendMode.Copy)
                    {
                        this[x, y] = color;
                        continue;
                    }
                    else
                    {
                        // get destination pixel
                        var dest_pixel = this[x, y];

                        destination_alpha = ((dest_pixel >> 24) & 0xff);

                        // destination is transparent or source is opaque,
                        // just replace destination pixel with source pixel
                        if (source_alpha == 255 || destination_alpha == 0)
                        {
                            this[x, y] = color;
                            continue;
                        }

                        // get destination pixel rgb values
                        destination_red = ((dest_pixel >> 16) & 0xff);
                        destination_green = ((dest_pixel >> 8) & 0xff);
                        destination_blue = ((dest_pixel) & 0xff);

                        var isa = 255 - source_alpha;

                        dest_pixel = (((((source_alpha << 8) + isa * destination_alpha) >> 8) & 0xff) << 24) |
                                        (((((source_red << 8) + isa * destination_red) >> 8) & 0xff) << 16) |
                                        (((((source_green << 8) + isa * destination_green) >> 8) & 0xff) << 8) |
                                        ((((source_blue << 8) + isa * destination_blue) >> 8) & 0xff);

                        this[x, y] = dest_pixel;
                    }
                }
            }
        }

        private static void Swap<T>(ref T a, ref T b)
        {
            T t = a;
            a = b;
            b = t;
        }

        void LineEdge(
            int width,
            int height,
            int x1,
            int y1,
            int x2,
            int y2,
            int a,
            int r,
            int g,
            int b,
            bool minEdge,
            bool leftEdge)
        {
            if (x1 == x2)
                return;

            if (y1 == y2)
                return;

            Byte off = 0;

            if (minEdge)
                off = 0xff;

            if (y1 > y2)
            {
                Swap(ref x1, ref x2);
                Swap(ref y1, ref y2);
            }

            int dx = (x2 - x1);
            if (x1 > x2)
                dx = (x1 - x2);

            int dy = (y2 - y1);

            int x = x1;
            int y = y1;

            UInt16 m = 0;

            if (dx > dy)
                m = (ushort)(((dy << 16) / dx));
            else
                m = (ushort)(((dx << 16) / dy));

            UInt16 e = 0;

            int rs, gs, bs;
            int rd, gd, bd;

            Int32 d;

            int ta = a;

            e = 0;

            int destination_alpha;
            int destination_red;
            int destination_green;
            int destination_blue;

            ta = (byte)((a * (UInt16)(((((UInt16)(e >> 8))) ^ off))) >> 8);
            var isa = 255 - ta;

            if (dx >= dy)
            {
                while (dx-- != 0)
                {
                    if ((UInt16)(e + m) <= e) // Roll
                    {
                        y++;
                    }

                    e += m;

                    if (x1 < x2) x++;
                    else x--;

                    if (y < 0 || y >= height)
                        continue;

                    if (leftEdge) leftEdgeX[y] = Math.Max(x + 1, leftEdgeX[y]);
                    else rightEdgeX[y] = Math.Min(x - 1, rightEdgeX[y]);

                    if (x < 0 || x >= width)
                        continue;


                    // get destination pixel
                    int dest_pixel = this[x, y];

                    // get destination pixel argb values
                    destination_alpha = ((dest_pixel >> 24) & 0xff);
                    destination_red = ((dest_pixel >> 16) & 0xff);
                    destination_green = ((dest_pixel >> 8) & 0xff);
                    destination_blue = ((dest_pixel) & 0xff);

                    dest_pixel = (((((ta << 8) + isa * destination_alpha) >> 8) & 0xff) << 24) |
                                    (((((r << 8) + isa * destination_red) >> 8) & 0xff) << 16) |
                                    (((((g << 8) + isa * destination_green) >> 8) & 0xff) << 8) |
                                    ((((b << 8) + isa * destination_blue) >> 8) & 0xff);

                    this[x, y] = dest_pixel;
                }
            }
            else
            {
                off ^= 0xff;

                while (--dy != 0)
                {
                    if ((UInt16)(e + m) <= e) // Roll
                    {
                        if (x1 < x2) x++;
                        else x--;
                    }

                    e += m;

                    y++;

                    if (x < 0 || x >= width) continue;
                    if (y < 0 || y >= height) continue;

                    //

                    ta = (byte)((a * (UInt16)(((((UInt16)(e >> 8))) ^ off))) >> 8);

                    // get destination pixel
                    int dest_pixel = this[x, y];

                    // get destination pixel argb values
                    destination_alpha = ((dest_pixel >> 24) & 0xff);
                    destination_red = ((dest_pixel >> 16) & 0xff);
                    destination_green = ((dest_pixel >> 8) & 0xff);
                    destination_blue = ((dest_pixel) & 0xff);

                    dest_pixel = (((((ta << 8) + isa * destination_alpha) >> 8) & 0xff) << 24) |
                                    (((((r << 8) + isa * destination_red) >> 8) & 0xff) << 16) |
                                    (((((g << 8) + isa * destination_green) >> 8) & 0xff) << 8) |
                                    ((((b << 8) + isa * destination_blue) >> 8) & 0xff);

                    this[x, y] = dest_pixel;

                    if (leftEdge) leftEdgeX[y] = x + 1;
                    else rightEdgeX[y] = x - 1;
                }
            }

        }
    }
}
