using SquaredInfinity.Maths;
using SquaredInfinity.Extensions;
using System.Windows;
using System.Windows.Media;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using SquaredInfinity.Graphics.Drawing;

namespace SquaredInfinity.Extensions
{
    public static partial class IPixelCanvasExtensions
    {
        public static void DrawDrawings(
    this IPixelCanvas pc,
    int x,
    int y,
    int width,
    int height,
    BlendMode blendMode,
    IEnumerable<DrawingRenderInfo> drawings)
        {
            pc.DrawDrawings(x, y, width, height, blendMode, ParallelExtensions.GetDefaultParallelOptions(CancellationToken.None), null, drawings);
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
            pc.DrawDrawings(x, y, width, height, blendMode, ParallelExtensions.GetDefaultParallelOptions(CancellationToken.None), prepareDrawingContext, drawings);
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
            var dv = new DrawingVisual();

            using (var cx = dv.RenderOpen())
            {
                if (prepareDrawingContext != null)
                    prepareDrawingContext(cx);

                cx.Render(parallelOptions, width, height, drawings);
            }

            pc.DrawVisual(0, 0, dv, blendMode);
        }

        public static void DrawDrawing(
            this IPixelCanvas pc,
            int x,
            int y,
            int width,
            int height,
            BlendMode blendMode,
            Action<DrawingContext> prepareDrawingContext,
            DrawingRenderInfo drawing)
        {
            var dv = new DrawingVisual();

            using (var cx = dv.RenderOpen())
            {
                if (prepareDrawingContext != null)
                    prepareDrawingContext(cx);

                cx.Render(width, height, drawing);
            }

            pc.DrawVisual(0, 0, dv, blendMode);
        }
    }
}
