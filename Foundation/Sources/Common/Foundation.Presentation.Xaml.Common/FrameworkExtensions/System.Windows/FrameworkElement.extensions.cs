using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace SquaredInfinity.Foundation.Extensions
{
    public static class FrameworkElementExtensions
    {
        public static bool IsInViewport(this FrameworkElement fe, Window parentWindow)
        {
            if (parentWindow == null)
                return false;

            if (fe.RenderSize.IsEmptyOrZeroArea())
                return false;

            var transform = fe.TransformToVisual(parentWindow);
            var rectangle = transform.TransformBounds(new Rect(new Point(0, 0), fe.RenderSize));

            var intersection = Rect.Intersect(new Rect(new Point(0, 0), parentWindow.RenderSize), rectangle);

            if (intersection == Rect.Empty)
            {
                // framework element is not in view

                // todo: make this configurable (by number of pixels, pages)
                // render elements which are just outside of view port (currently supports only elements below)

                //var lookAhead = fe.RenderSize.Height * 50;

                //if (rectangle.Y > 0 && rectangle.Y - lookAhead <= parentWindow.RenderSize.Height)
                //    return true;

                return false;
            }
            else
            {
                return true;
            }
        }
    }
}
