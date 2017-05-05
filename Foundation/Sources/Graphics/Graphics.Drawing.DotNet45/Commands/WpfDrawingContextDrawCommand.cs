using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media;

namespace SquaredInfinity.Graphics.Drawing.Commands
{
    public class WpfDrawingContextDrawCommand : PixelCanvasCommand
    {
        //internal Task<DrawingRenderInfo> Processing;

        Action<IPixelCanvas, DrawingContext, CancellationToken> DrawAction;

        public WpfDrawingContextDrawCommand(Action<IPixelCanvas, DrawingContext, CancellationToken> drawAction)
        {
            this.DrawAction = drawAction;

            //Processing = Task.Run(() =>
            //{
            //    var dv = new DrawingVisual();
            //    using (var cx = dv.RenderOpen())
            //    {
            //        Draw(null, cx, CancellationToken.None);
            //    }

            //    var dr = dv.Drawing;
            //    dr.Freeze();

            //    var di = new DrawingRenderInfo();

            //    di.Drawing = dr;

            //    return di;
            //});
        }

        public void Draw(IPixelCanvas pc, DrawingContext cx, CancellationToken ct)
        {
            DrawAction(pc, cx, ct);
        }
    }
}
