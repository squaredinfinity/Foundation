using System;
using System.Collections.Generic;
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
        public static bool IsInViewport(this FrameworkElement fe, FrameworkElement parentElement)
        {
            if (parentElement == null)
                return false;

            var scrollViewer = parentElement as ScrollViewer;

            if (scrollViewer == null)
                scrollViewer = parentElement.FindDescendant<ScrollViewer>();

            FrameworkElement viewPortElement = scrollViewer;

            if (viewPortElement == null)
                viewPortElement = parentElement;

            var transform = fe.TransformToVisual(viewPortElement);
            var rectangle = transform.TransformBounds(new Rect(new Point(0, 0), fe.RenderSize));

            var intersection = Rect.Intersect(new Rect(new Point(0, 0), viewPortElement.RenderSize), rectangle);

            if (intersection == Rect.Empty)
            {
                // framework element is not in view

                return false;

                // todo: make this configurable (by number of pixels, pages)
                // render elements which are just outside of view port

                //rectangle.Inflate(scrollViewer.RenderSize.Width * .5, scrollViewer.RenderSize.Height * .5);
                //intersection = Rect.Intersect(new Rect(new Point(0, 0), scrollViewer.RenderSize), rectangle);

                //return intersection != Rect.Empty;

                //return false;
            }
            else
            {
                return true;
            }
        }
    }
}
