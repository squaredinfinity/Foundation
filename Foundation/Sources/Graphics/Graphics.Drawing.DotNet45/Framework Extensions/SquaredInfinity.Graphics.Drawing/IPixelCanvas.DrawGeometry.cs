using SquaredInfinity.Maths;
using SquaredInfinity.Extensions;
using System.Windows;
using System.Windows.Media;

namespace SquaredInfinity.Graphics.Drawing
{
    public static partial class IPixelCanvasExtensions
    {
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
    }
}
