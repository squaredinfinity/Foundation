using System;
using SquaredInfinity.Extensions;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media;

namespace SquaredInfinity.Graphics.Drawing.Commands
{
    public class PixelCanvasCommandsEngine
    {
        public void Execute(IPixelCanvas pc, IReadOnlyList<IPixelCanvasCommand> commands, CancellationToken ct)
        {
            Execute(pc, () => new DrawingVisual(), commands, ct);
        }

        public void Execute(
            IPixelCanvas pc,
            Func<DrawingVisual> createDrawingVisual,
            IReadOnlyList<IPixelCanvasCommand> commands,
            CancellationToken ct)
        {
            var wpf_drawing_visual = (DrawingVisual)null;
            var wpf_drawing_context = (DrawingContext)null;

            var blend_mode = BlendMode.Copy;

            for (int i = 0; i < commands.Count; i++)
            {
                if (ct.IsCancellationRequested)
                    return;

                var cmd = commands[i];

                var wpf_command = cmd as WpfDrawingContextDrawCommand;
                var raw_command = cmd as RawDrawCommand;

                if (wpf_command == null) // flush previous drawings
                {
                    if (wpf_drawing_context != null)
                    {
                        wpf_drawing_context.Close();
                        if (!wpf_drawing_visual.ContentBounds.IsEmpty)
                        {
                            pc.DrawVisual((int)wpf_drawing_visual.ContentBounds.Left, (int)wpf_drawing_visual.ContentBounds.Top, wpf_drawing_visual, blend_mode);
                            blend_mode = BlendMode.Alpha; // further blends must be alpha
                        }

                        wpf_drawing_context = null;
                        wpf_drawing_visual = null;
                    }
                }

                if (raw_command != null)
                {
                    raw_command.Draw(pc, ct);
                    blend_mode = BlendMode.Alpha; // further blends must be alpha
                }

                if (wpf_command != null)
                {
                    if (wpf_drawing_context == null)
                    {
                        wpf_drawing_visual = createDrawingVisual();
                        wpf_drawing_context = wpf_drawing_visual.RenderOpen();
                    }

                    wpf_command.Draw(pc, wpf_drawing_context, ct);
                }
            }

            if (wpf_drawing_context != null)
            {
                if (ct.IsCancellationRequested)
                    return;

                ((IDisposable)wpf_drawing_context).Dispose();
                if (!wpf_drawing_visual.ContentBounds.IsEmpty)
                    pc.DrawVisual((int)wpf_drawing_visual.ContentBounds.Left, (int)wpf_drawing_visual.ContentBounds.Top, wpf_drawing_visual, blend_mode);
            }
        }
    }
}
