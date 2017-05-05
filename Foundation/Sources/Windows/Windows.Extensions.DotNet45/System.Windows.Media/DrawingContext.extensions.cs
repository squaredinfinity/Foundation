using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using SquaredInfinity.Extensions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace SquaredInfinity.Extensions
{
    public static class DrawingContextExtensions
    {
        public static void Render(
            this DrawingContext cx,
            ParallelOptions parallelOptions,
            double drawingWidth,
            double drawingHeight,
            IEnumerable<DrawingRenderInfo> drawings)
        {
            // expand drawings to desired size using transparent rectangle
            var canvas = new RectangleGeometry(new Rect(0, 0, drawingWidth, drawingHeight));
            canvas.Freeze();
            cx.DrawGeometry(Brushes.Transparent, null, canvas);

            if (parallelOptions.CancellationToken.IsCancellationRequested)
                return;

            cx.Render(parallelOptions, drawings);
        }

        public static void Render(
            this DrawingContext cx,
            ParallelOptions parallelOptions,
            IEnumerable<DrawingRenderInfo> drawings)
        {
            var finished_drawings = new ConcurrentBag<DrawingRenderInfo>();

            ParallelExtensions.ForEach(drawings, parallelOptions, dp =>
            {
                finished_drawings.Add(dp);
            });

            var di = (DrawingRenderInfo)null;

            while (finished_drawings.TryTake(out di))
            {
                if (parallelOptions.CancellationToken.IsCancellationRequested)
                    return;

                // NOTE: actual rendering must happen on thread cx has been created
                cx.Render(di);
            }
        }

        public static void Render(
            this DrawingContext cx,
            double drawingWidth,
            double drawingHeight,
            DrawingRenderInfo drawing)
        {
            // expand drawings to desired size using transparent rectangle
            var canvas = new RectangleGeometry(new Rect(0, 0, drawingWidth, drawingHeight));
            canvas.Freeze();
            cx.DrawGeometry(Brushes.Transparent, null, canvas);

            cx.Render(drawing);
        }

        public static void Render(
            this DrawingContext cx,
            DrawingRenderInfo drawing)
        {
            var pop_count = 0;

            if (drawing.Transform != null)
            {
                cx.PushTransform(drawing.Transform);
                pop_count++;
            }
            if (drawing.Opacity != null)
            {
                cx.PushOpacity(drawing.Opacity.Value);
                pop_count++;
            }
            if (drawing.Clip != null)
            {
                cx.PushClip(drawing.Clip);
                pop_count++;
            }

            cx.DrawDrawing(drawing.Drawing);

            while (pop_count-- > 0)
                cx.Pop();
        }

        public static void Render(
            this DrawingContext cx,
            Geometry geometry,
            Point geometryPositionOnBitmap,
            Brush brush,
            Pen pen)
        {
            var pop_count = 0;

            if (!geometryPositionOnBitmap.X.IsCloseTo(0.0) || geometryPositionOnBitmap.Y.IsCloseTo(0.0))
            {
                cx.PushTransform(new TranslateTransform(geometryPositionOnBitmap.X, geometryPositionOnBitmap.Y));
                pop_count++;
            }

            cx.DrawGeometry(brush, pen, geometry);

            while (pop_count-- > 0)
                cx.Pop();
        }

        public static void Render(
            this DrawingContext cx,
            int x,
            int y,
            int width,
            int height,
            Action<DrawingContext> prepareDrawingContext,
            DrawingRenderInfo drawing)
        {
            if (prepareDrawingContext != null)
                prepareDrawingContext(cx);

            cx.Render(width, height, drawing);
        }

        public static void Render(
            this DrawingContext cx,
            Visual visual,
            Point visualPositionOnBitmap,
            Size visualSize,
            FlowDirection flowDirection = FlowDirection.LeftToRight)
        {
            // size visual if needed
            var fElement = visual as FrameworkElement;
            var old_size = (Size?)null;

            if (fElement != null)
            {
                if (!fElement.ActualHeight.IsCloseTo(visualSize.Height) || !fElement.ActualWidth.IsCloseTo(visualSize.Width))
                {
                    old_size = new Size(fElement.Width, fElement.Height);

                    fElement.Width = visualSize.Width;
                    fElement.Height = visualSize.Height;
                    fElement.Arrange(new Rect(visualSize));
                    fElement.UpdateLayout();
                }
            }

            int pop_count = 0;

            var vb = new VisualBrush(visual);

            if (flowDirection == FlowDirection.RightToLeft)
            {
                var transformGroup = new TransformGroup();

                transformGroup.Children.Add(new ScaleTransform(-1, 1));

                transformGroup.Children.Add(new TranslateTransform(visualSize.Width - 1, 0));

                cx.PushTransform(transformGroup);

                pop_count++;
            }

            cx.DrawRectangle(vb, null, new Rect(visualPositionOnBitmap, visualSize));

            while (pop_count-- > 0)
                cx.Pop();
            
            if (fElement != null && old_size != null)
            {
                fElement.Width = old_size.Value.Width;
                fElement.Height = old_size.Value.Height;

                if (old_size.Value.Width.IsInfinityOrNaN() || old_size.Value.Height.IsInfinityOrNaN())
                {
                    fElement.InvalidateVisual();
                }
                else
                {
                    fElement.Arrange(new Rect(old_size.Value));
                    fElement.InvalidateVisual();//.UpdateLayout();
                }
            }
        }
    }
}
