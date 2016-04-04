using SquaredInfinity.Foundation.Maths.Space2D;
using SquaredInfinity.Foundation.Media.Drawing;
using SquaredInfinity.Foundation.Presentation.Media;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Shapes = System.Windows.Shapes;

namespace SquaredInfinity.Foundation.Extensions
{
    public static class PixelCanvasExtensions
    {
        public static WriteableBitmap ToWriteableBitmap(this IPixelCanvas pc)
        {
            var wb = new WriteableBitmap(pc.Width, pc.Height, 96, 96, PixelFormats.Pbgra32, palette: null);

            var bounds = new Int32Rect(0, 0, pc.Width, pc.Height);

            wb.WritePixels(bounds, pc.GetPixels(), pc.Stride, 0);

            return wb;
        }

        public static WriteableBitmap ToFrozenWriteableBitmap(this IPixelCanvas pc)
        {
            var wb = pc.ToWriteableBitmap();

            wb.Freeze();

            return wb;
        }

        public static void DrawLine(this IPixelCanvas pc, int x1, int y1, int x2, int y2, System.Windows.Media.Color color)
        {
            pc.DrawLine(x1, y1, x2, y2, pc.GetColor(color.A, color.R, color.G, color.B));
        }

        public static void DrawLineDDA(this IPixelCanvas pc, int x1, int y1, int x2, int y2, System.Windows.Media.Color color)
        {
            pc.DrawLineDDA(pc.Bounds, x1, y1, x2, y2, pc.GetColor(color.A, color.R, color.G, color.B));
        }

        public static void DrawLineWu(this IPixelCanvas pc, int x1, int y1, int x2, int y2, System.Windows.Media.Color color)
        {
            pc.DrawLineWu(pc.Bounds, x1, y1, x2, y2, pc.GetColor(color.A, color.R, color.G, color.B));
        }
        
        public static void DrawLine(this IPixelCanvas pc, int x1, int y1, int x2, int y2, System.Windows.Media.Color color, double width)
        {
            var l = new Shapes.Line();
            l.X1 = x1;
            l.Y1 = y1;
            l.X2 = x2;
            l.Y2 = y2;
            l.Stroke = new SolidColorBrush(color);
            l.StrokeThickness = width;

            l.Arrange(pc.Bounds.ToRect());

            var sw = Stopwatch.StartNew();

            var bitmap_source = l.RenderToBitmap(pc.Bounds.ToSize(), new Point(x1, y1));

            var pixels = new int[pc.Length];

            bitmap_source.CopyPixels(pixels, pc.Stride, 0);

            var pc2 = new PixelArrayCanvas(pc.Width, pc.Height);
            pc2.ReplaceFromPixels(pixels, pc.Width, pc.Height);

            pc.Blit(pc2, BlendMode.Alpha);
        }

        public static void DrawGeometry(
            this IPixelCanvas pc,
            int x,
            int y,
            Geometry geometry,
            Brush fillBrush,
            Pen pen)
        {
            pc.DrawGeometry(x, y, geometry, fillBrush, pen, BlendMode.Alpha);
        }

        public static void DrawGeometry(
            this IPixelCanvas pc,
            int x,
            int y,
            Geometry geometry, 
            Brush fillBrush, 
            Pen pen,
            BlendMode blendMode)
        {
            var size = geometry.GetRenderBounds(pen).Size;
            var width = (int)size.Width;
            var height = (int)size.Height;
            
            var bmp = 
                geometry.RenderToBitmap(
                    size,
                    new Point(),
                    size,
                    fillBrush, 
                    pen, 
                    PixelFormats.Pbgra32);

            int[] pixels;

            if (blendMode == BlendMode.Copy && width == pc.Width && height == pc.Height)
            {
                pixels = pc.GetPixels();
                bmp.CopyPixels(pixels, pc.Stride, 0);
            }
            else
            {
                var pc2 = new PixelArrayCanvas(width, height);
                pixels = pc2.GetPixels();

                bmp.CopyPixels(pixels, pc2.Stride, 0);

                pc.Blit(
                    new Rectangle(x, y, width, height),
                    pc2,
                    new Rectangle(0, 0, width, height),
                    255, 255, 255, 255, blendMode);
            }
        }

        public static void DrawDrawings(
            this IPixelCanvas pc,
            int x,
            int y,
            int width,
            int height,
            BlendMode blendMode,
            IEnumerable<DrawingRenderInfo> drawings)
        {
            pc.DrawDrawings(x, y, width, height, blendMode, ParallelExtensions.DefaultParallelOptions, null, drawings);
        }

        public static void DrawDrawings(
            this IPixelCanvas pc,
            int x,
            int y,
            int width,
            int height,
            BlendMode blendMode,
            Action<DrawingContext> prepareDrawingContext,
            IEnumerable<DrawingRenderInfo> drawings)
        {
            pc.DrawDrawings(x, y, width, height, blendMode, ParallelExtensions.DefaultParallelOptions, prepareDrawingContext, drawings);
        }

        public static void DrawDrawings(
            this IPixelCanvas pc,
            int x,
            int y,
            int width,
            int height,
            BlendMode blendMode,
            ParallelOptions parallelOptions,
            IEnumerable<DrawingRenderInfo> drawings)
        {
            pc.DrawDrawings(x, y, width, height, blendMode, parallelOptions, null, drawings);
        }

        public static void DrawDrawings(
            this IPixelCanvas pc, 
            int x, 
            int y, 
            int width, 
            int height,
            BlendMode blendMode,
            ParallelOptions parallelOptions,
            Action<DrawingContext> prepareDrawingContext,
            IEnumerable<DrawingRenderInfo> drawings)
        {
            DrawingRenderer renderer = new DrawingRenderer();

            var dv = new DrawingVisual();

            using (var cx = dv.RenderOpen())
            {
                if(prepareDrawingContext != null)
                    prepareDrawingContext(cx);

                renderer.Render(cx, parallelOptions, width, height, drawings);
            }

            pc.DrawVisual(0, 0, dv, blendMode);
        }


        public static int GetColor(this IPixelCanvas pc, System.Windows.Media.Color color)
        {
            return pc.GetColor(color.A, color.R, color.G, color.B);
        }

        public static Color GetColor(this IPixelCanvas pc, int color)
        {
            var a = ((color >> 24) & 0xff);
            var r = ((color >> 16) & 0xff);
            var g = ((color >> 8) & 0xff);
            var b = ((color) & 0xff);

            return Color.FromArgb((byte)a, (byte)r, (byte)g, (byte)b);
        }


        /// <summary>
        /// Replaces content of this Pixel Canvas with content from specified stream.
        /// Pixels and Dimensions will be updated.
        /// Old Pixels will be lost.
        /// </summary>
        /// <param name="pc"></param>
        /// <param name="stream"></param>
        public static void ReplaceFromStream(this IPixelCanvas pc, Stream stream)
        {
            if (stream == null)
                throw new ArgumentNullException("stream");

            var bmp = new BitmapImage();

            bmp.BeginInit();
            bmp.StreamSource = stream;
            bmp.UriSource = null;
            bmp.EndInit();


            var wb = new WriteableBitmap(bmp);

            int[] pixels = new int[wb.PixelHeight * wb.PixelWidth];

            wb.CopyPixels(pixels, wb.BackBufferStride, 0);

            pc.ReplaceFromPixels(pixels, wb.PixelWidth, wb.PixelHeight); 
        }

        public static void Save(this IPixelCanvas pc, string fullPath)
        {
            var bmp = pc.ToFrozenWriteableBitmap();

            using (FileStream fs = new FileStream(fullPath, FileMode.OpenOrCreate))
            {
                PngBitmapEncoder encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(bmp));
                encoder.Save(fs);
                fs.Close();
            }
        }


        public static void DrawVisual(this IPixelCanvas pc, int x, int y, ContainerVisual visual, BlendMode blendMode)
        {
            var width = (int)visual.ContentBounds.Width;
            var height = (int)visual.ContentBounds.Height;

            var bmp = visual.RenderToBitmap();

            int[] pixels;

            if (blendMode == BlendMode.Copy && width == pc.Width && height == pc.Height)
            {
                pixels = pc.GetPixels();
                bmp.CopyPixels(pixels, pc.Stride, 0);
            }
            else
            {
                var pc2 = new PixelArrayCanvas(width, height);
                pixels = pc2.GetPixels();

                bmp.CopyPixels(pixels, pc2.Stride, 0);

                pc.Blit(
                    new Rectangle(x, y, width, height),
                    pc2,
                    new Rectangle(0, 0, width, height),
                    255, 255, 255, 255, blendMode);
            }
        }

#if DEBUG

        /// <summary>
        /// http://en.wikipedia.org/wiki/Alpha_compositing
        /// </summary>
        /// <param name="destination_rect"></param>
        /// <param name="source"></param>
        /// <param name="source_rect"></param>
        /// <param name="alpha"></param>
        /// <param name="red"></param>
        /// <param name="green"></param>
        /// <param name="blue"></param>
        /// <param name="blendMode"></param>
        public static void DEBUG__Blit(
            this IPixelCanvas pc,
            Rectangle destination_rect,
            IPixelCanvas source,
            Rect source_rect,
            byte alpha,
            byte red,
            byte green,
            byte blue,
            BlendMode blendMode)
        {
            int debug_counter = 0;

            var debug_destination_pc = new PixelArrayCanvas(pc.Width, pc.Height);
            debug_destination_pc.Blit(pc, BlendMode.Copy);
            debug_destination_pc.Save(@"c:\temp\blend\{0}_destination.bmp".FormatWith(debug_counter));

            var debug_source_pc = new PixelArrayCanvas(source.Width, source.Height);
            debug_source_pc.Blit(pc, BlendMode.Copy);
            debug_source_pc.Save(@"c:\temp\blend\{0}_source.bmp".FormatWith(debug_counter));

            // http://keithp.com/~keithp/porterduff/p253-porter.pdf
            // http://en.wikipedia.org/wiki/Alpha_compositing

            // tint with transparent color, nothing to do here, since this would make every source pixel transparent
            if (alpha == 0)
                return;

            if (!pc.IntersectsWith(destination_rect))
                return;

            // tinted if color is not opaque white
            var is_tinted = alpha != 255 || red != 255 || green != 255 || blue != 255;

            var source_width = source.Width;
            var source_pixels = source;
            var source_length = source.Length;

            int source_rectangle_x = (int)source_rect.X;
            int source_rectangle_y = (int)source_rect.Y;

            var destination_width = pc.Width;
            var destination_height = pc.Height;
            var destination_pixels = pc;
            var destination_length = pc.Length;

            int destination_rectangle_x = (int)destination_rect.X;
            int destination_rectangle_y = (int)destination_rect.Y;
            int destination_rect_width = (int)destination_rect.Width;
            int destination_rect_height = (int)destination_rect.Height;

            int sourceIdx = -1;
            int idx;
            double ii;
            double jj;

            int source_pixel = 0;
            int source_alpha = 0;
            int source_red = 0;
            int source_green = 0;
            int source_blue = 0;

            int destination_alpha;
            int destination_red;
            int destination_green;
            int destination_blue;

            var source_rectangle_width = source_rect.Width;

            var sdx = source_rect.Width / destination_rect.Width;
            var sdy = source_rect.Height / destination_rect.Height;

            int lastii = -1;
            int lastjj = -1;

            jj = source_rectangle_y;


            int current_x;
            int current_y = destination_rectangle_y;

            // each row in destination rectangle
            for (int destination_rect_row = 0; destination_rect_row < destination_rect_height; destination_rect_row++)
            {
                // check if current y is within destination rectangle bounds (todo: >= 0? should it be destination_rect_y ?)
                if (current_y >= 0 && current_y < destination_height)
                {
                    ii = source_rectangle_x;
                    idx = destination_rectangle_x + current_y * destination_width;

                    // start from left most column in destination rectangle
                    current_x = destination_rectangle_x;

                    // each column in destination rectangle
                    for (int destination_rect_col = 0; destination_rect_col < destination_rect_width; destination_rect_col++)
                    {
                        // todo: >= 0?
                        if (current_x >= 0 && current_x < destination_width)
                        {
                            if ((int)ii != lastii || (int)jj != lastjj)
                            {
                                sourceIdx = (int)ii + (int)jj * source_width;

                                if (sourceIdx >= 0 && sourceIdx < source_length)
                                {
                                    // retrieve current source pixel and it's channels values
                                    source_pixel = source_pixels[sourceIdx];

                                    if (blendMode == BlendMode.Copy)
                                    {
                                        destination_pixels[idx] = source_pixel;

                                        current_x++;
                                        idx++;
                                        ii += sdx;

                                        continue;
                                    }

                                    if (source_pixel != 0)
                                    {
                                        source_alpha = ((source_pixel >> 24) & 0xff);
                                        source_red = ((source_pixel >> 16) & 0xff);
                                        source_green = ((source_pixel >> 8) & 0xff);
                                        source_blue = ((source_pixel) & 0xff);

                                        // tint the pixel if needed
                                        if (is_tinted && source_alpha != 0)
                                        {
                                            source_alpha = (((source_alpha * alpha) * 0x8081) >> 23);
                                            source_red = ((((((source_red * red) * 0x8081) >> 23) * alpha) * 0x8081) >> 23);
                                            source_green = ((((((source_green * green) * 0x8081) >> 23) * alpha) * 0x8081) >> 23);
                                            source_blue = ((((((source_blue * blue) * 0x8081) >> 23) * alpha) * 0x8081) >> 23);

                                            source_pixel = (source_alpha << 24) | (source_red << 16) | (source_green << 8) | source_blue;
                                        }
                                    }
                                    else
                                    {
                                        source_alpha = 0;
                                    }
                                }
                                else
                                {
                                    source_alpha = 0;
                                }
                            }

                            if (source_alpha == 0)
                            {
                                current_x++;
                                idx++;
                                ii += sdx;

                                continue;
                            }
                            else
                            {
                                // get destination pixel
                                int destPixel = destination_pixels[idx];

                                destination_alpha = ((destPixel >> 24) & 0xff);

                                // destination is transparent or source is opaque,
                                // just replace destination pixel with source pixel
                                if (blendMode == BlendMode.Alpha && (source_alpha == 255 || destination_alpha == 0))
                                {
                                    // debug code
                                    debug_counter++;
                                    debug_destination_pc[idx] = pc.GetColor(System.Windows.Media.Colors.Red);
                                    debug_source_pc[sourceIdx] = pc.GetColor(System.Windows.Media.Colors.Red);
                                    debug_destination_pc.Save(@"c:\temp\blend\{0}_destination.bmp".FormatWith(debug_counter));
                                    debug_source_pc.Save(@"c:\temp\blend\{0}_source.bmp".FormatWith(debug_counter));
                                    // end debug code

                                    destination_pixels[idx] = source_pixel;

                                    current_x++;
                                    idx++;
                                    ii += sdx;

                                    continue;
                                }

                                // get destination pixel rgb values
                                destination_red = ((destPixel >> 16) & 0xff);
                                destination_green = ((destPixel >> 8) & 0xff);
                                destination_blue = ((destPixel) & 0xff);

                                if (blendMode == BlendMode.Alpha)
                                {
                                    var isa = 255 - source_alpha;

                                    // debug code
                                    debug_destination_pc[idx] = pc.GetColor(System.Windows.Media.Colors.Red);
                                    debug_source_pc[sourceIdx] = pc.GetColor(System.Windows.Media.Colors.Red);
                                    var deb_part = new PixelArrayCanvas(3, 1);
                                    deb_part[0] = source_pixel;
                                    deb_part[1] = destPixel;
                                    // end debug code

                                    destPixel = (((((source_alpha << 8) + isa * destination_alpha) >> 8) & 0xff) << 24) |
                                                (((((source_red << 8) + isa * destination_red) >> 8) & 0xff) << 16) |
                                                (((((source_green << 8) + isa * destination_green) >> 8) & 0xff) << 8) |
                                                 ((((source_blue << 8) + isa * destination_blue) >> 8) & 0xff);

                                    // debug code
                                    deb_part[2] = destPixel;
                                    debug_counter++;
                                    debug_destination_pc.Save(@"c:\temp\blend\{0}_destination.bmp".FormatWith(debug_counter));
                                    debug_source_pc.Save(@"c:\temp\blend\{0}_source.bmp".FormatWith(debug_counter));
                                    deb_part.Save(@"c:\temp\blend\{0}_blend.bmp".FormatWith(debug_counter));
                                    //end debug code
                                }
                                else
                                    throw new NotSupportedException();

                                destination_pixels[idx] = destPixel;
                            }
                        }

                        current_x++;
                        idx++;
                        ii += sdx;
                    }
                }
                jj += sdy;
                current_y++;
            }

            // debug code
            debug_destination_pc.Save(@"c:\temp\blend\final_destination.bmp".FormatWith(debug_counter));
            debug_source_pc.Save(@"c:\temp\blend\final_source.bmp".FormatWith(debug_counter));
            pc.Save(@"c:\temp\blend\final_blend.bmp".FormatWith(debug_counter));
            // end debug code
        }

#endif
    }
}
