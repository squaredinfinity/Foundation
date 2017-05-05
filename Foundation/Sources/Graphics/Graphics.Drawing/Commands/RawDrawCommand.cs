using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SquaredInfinity.Graphics.Drawing.Commands
{
    public class RawDrawCommand : PixelCanvasCommand
    {
        Action<IPixelCanvas, CancellationToken> DrawAction;

        public RawDrawCommand(Action<IPixelCanvas, CancellationToken> drawAction)
        {
            this.DrawAction = drawAction;
        }

        public void Draw(IPixelCanvas pc, CancellationToken ct)
        {
            DrawAction(pc, ct);
        }
    }
}
