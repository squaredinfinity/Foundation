using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace SquaredInfinity.Extensions
{
    public static class FrameworkElementExtensions
    {
        public static string GetDebugName(this FrameworkElement fe)
        {
            if (fe.Name.IsNullOrEmpty())
                return fe.GetType().Name;
            else
                return fe.Name;
        }

        /// <summary>
        /// BindingOperation.ClearBinding alternative which handles Data Template scenarios
        /// see details: http://social.msdn.microsoft.com/Forums/vstudio/en-US/e45c7a9d-840d-4508-8c81-ef40f1c74c10/bindingoperationsclearbinding-reverting-to-data-template-binding?forum=wpf
        /// </summary>
        /// <param name="depObj"></param>
        /// <param name="depProp"></param>
        public static void ClearBindingOrReplaceWithDummyValue(this FrameworkElement depObj, DependencyProperty depProp)
        {
            BindingOperations.ClearBinding(depObj, depProp);

            // if binding was set in Data Template, ClearBinding may not work
            // replace binding with a dummy value
            if (BindingOperations.IsDataBound(depObj, depProp))
                depObj.SetBinding(depProp, "<binding_removed__ignore_this_message>");
        }

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
