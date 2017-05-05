using SquaredInfinity.Maths;
using SquaredInfinity.Extensions;
using System.Windows;
using System.Windows.Media;
using System;

namespace SquaredInfinity.Graphics.Drawing
{
    public static partial class IPixelCanvasExtensions
    {
        public static void DrawVisual(this IPixelCanvas pc, int x, int y, ContainerVisual visual, BlendMode blendMode)
        {
            if (visual.ContentBounds.IsEmpty)
                return;

            var width = (int)visual.ContentBounds.Width;
            var height = (int)visual.ContentBounds.Height;

            var bmp = visual.RenderToBitmap(visual.ContentBounds.Size, new Point(0, 0));

            if (blendMode == BlendMode.Copy && width == pc.Width && height == pc.Height)
            {
                var pac = pc as PixelArrayCanvas;
                if (pac == null)
                {
                    throw new NotSupportedException("BlendMode.Copy is only supported with PixelArrayCanvas");
                }

                bmp.CopyPixels(pac.Pixels, pc.Stride, 0);
            }
            else
            {
                var pc2 = new PixelArrayCanvas(width, height);

                bmp.CopyPixels(pc2.Pixels, pc2.Stride, 0);

                pc.Blit(
                    new Rectangle(x, y, width, height),
                    pc2,
                    new Rectangle(0, 0, width, height),
                    255, 255, 255, 255, blendMode);
            }
        }
    }
}
