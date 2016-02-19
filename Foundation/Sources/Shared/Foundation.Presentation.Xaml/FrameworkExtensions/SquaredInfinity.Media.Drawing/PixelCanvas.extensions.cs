using SquaredInfinity.Foundation.Media.Drawing;
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
using System.Windows.Shapes;

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
            var l = new Line();
            l.X1 = x1;
            l.Y1 = y1;
            l.X2 = x2;
            l.Y2 = y2;
            l.Stroke = new SolidColorBrush(color);
            l.StrokeThickness = width;

            l.Arrange(pc.Bounds);

            var sw = Stopwatch.StartNew();

            var bitmap_source = l.RenderToBitmap(pc.Bounds.Size, new Point(x1, y1));

            var pixels = new int[pc.Length];

            bitmap_source.CopyPixels(pixels, pc.Stride, 0);

            var pc2 = new PixelArrayCanvas(pc.Width, pc.Height);
            pc2.ReplaceFromPixels(pixels, pc.Width, pc.Height);

            pc.Blit(pc2, BlendMode.Alpha);
        }

        public static void DrawGeometry(this IPixelCanvas pc, int x, int y, Geometry geometry, Brush fillBrush, Pen pen)
        {
            var bitmap_source = geometry.RenderToBitmap(pc.Bounds.Size, new Point(x, y), geometry.Bounds.Size, fillBrush, pen, PixelFormats.Pbgra32);

            var pixels = new int[pc.Length];

            bitmap_source.CopyPixels(pixels, pc.Stride, 0);

            var pc2 = new PixelArrayCanvas(pc.Width, pc.Height);
            pc2.ReplaceFromPixels(pixels, pc.Width, pc.Height);

            pc.Blit(pc2, BlendMode.Alpha);
        }


        public static int GetColor(this IPixelCanvas pc, System.Windows.Media.Color color)
        {
            return pc.GetColor(color.A, color.R, color.G, color.B);
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
    }
}
