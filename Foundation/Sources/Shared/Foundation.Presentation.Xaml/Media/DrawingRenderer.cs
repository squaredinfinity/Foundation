using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace SquaredInfinity.Foundation.Presentation.Media
{
    public class DrawingRenderer
    {
        public virtual void Render(
            DrawingContext cx,
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

            Render(cx, parallelOptions, drawings);
        }

        public virtual void Render(
            DrawingContext cx,
            ParallelOptions parallelOptions,
            IEnumerable<DrawingRenderInfo> drawings)
        {
            var finished_drawings = new ConcurrentBag<DrawingRenderInfo>();

            Parallel.ForEach(drawings, parallelOptions, dp =>
            {
                finished_drawings.Add(dp);
            });

            var di = (DrawingRenderInfo)null;

            while (finished_drawings.TryTake(out di))
            {
                // NOTE: actual rendering must happen on thread cx has been created
                Render(cx, di);
            }
        }

        public virtual void Render(
            DrawingContext cx,
            double drawingWidth,
            double drawingHeight,
            DrawingRenderInfo drawing)
        {
            // expand drawings to desired size using transparent rectangle
            var canvas = new RectangleGeometry(new Rect(0, 0, drawingWidth, drawingHeight));
            canvas.Freeze();
            cx.DrawGeometry(Brushes.Transparent, null, canvas);

            Render(cx, drawing);
        }

        public virtual void Render(
            DrawingContext cx,
            DrawingRenderInfo drawing)
        {
            if (drawing.Transform != null)
                cx.PushTransform(drawing.Transform);
            if (drawing.Opacity != null)
                cx.PushOpacity(drawing.Opacity.Value);
            if (drawing.Clip != null)
                cx.PushClip(drawing.Clip);

            cx.DrawDrawing(drawing.Drawing);

            if (drawing.Clip != null)
                cx.Pop();
            if (drawing.Opacity != null)
                cx.Pop();
            if (drawing.Transform != null)
                cx.Pop();
        }
    }
}
