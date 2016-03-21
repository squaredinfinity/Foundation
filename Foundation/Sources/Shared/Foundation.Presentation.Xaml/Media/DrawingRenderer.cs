using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace SquaredInfinity.Foundation.Presentation.Media
{
    public class DrawingRenderer
    {
        public virtual void Render(
            DrawingContext cx, 
            double drawingWidth, 
            double drawingHeight,
            IEnumerable<DrawingRenderInfo> drawings)
        {
            // expand drawings to desired size using transparent rectangle
            var canvas = new RectangleGeometry(new Rect(0, 0, drawingWidth, drawingHeight));
            canvas.Freeze();
            cx.DrawGeometry(Brushes.Transparent, null, canvas);

            Render(cx, drawings);
        }

        public virtual void Render(
            DrawingContext cx,
            IEnumerable<DrawingRenderInfo> drawings)
        {
            var finished_drawings = new ConcurrentBag<DrawingRenderInfo>();

            Parallel.ForEach(drawings, dp =>
            {
                finished_drawings.Add(dp);
            });

            foreach (var di in drawings)
            {
                if (di.Transform != null)
                    cx.PushTransform(di.Transform);
                if (di.Opacity != null)
                    cx.PushOpacity(di.Opacity.Value);
                if (di.Clip != null)
                    cx.PushClip(di.Clip);

                cx.DrawDrawing(di.Drawing);

                if (di.Clip != null)
                    cx.Pop();
                if (di.Opacity != null)
                    cx.Pop();
                if (di.Transform != null)
                    cx.Pop();
            }
        }
    }
}
