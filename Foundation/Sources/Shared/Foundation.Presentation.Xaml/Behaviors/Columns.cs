using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using SquaredInfinity.Foundation.Extensions;
using System.Windows;
using System.Windows.Controls.Primitives;

namespace SquaredInfinity.Foundation.Presentation.Behaviors
{
    public static class Columns
    {
        public static readonly DependencyProperty MinWidthProperty =
            DependencyProperty.RegisterAttached(
            "MinWidth",
            typeof(double),
            typeof(Columns),
            new PropertyMetadata(OnMinWidthChanged));

        static void OnMinWidthChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var lv = d as ListView;

            if (lv == null)
                return;

            lv.AddHandler(Thumb.DragDeltaEvent, new DragDeltaEventHandler(OnThumDragDelta), handledEventsToo: true);
        }

        static void OnThumDragDelta(object sender, DragDeltaEventArgs e)
        {
            var thumb = e.OriginalSource as Thumb;

            if (thumb == null)
                return;

            var header = thumb.TemplatedParent as GridViewColumnHeader;

            // Header may be null if, for example, thumb is part of ScrollViewer (i.e. not a thumb we are monitoring)
            if (header == null)
                return;

            var minWidth = GetMinWidth(sender as ListView);

            if (header.Column.ActualWidth.IsLessThan(minWidth))
            {
                header.Column.Width = minWidth;
            }
        }

        public static void SetMinWidth(System.Windows.Controls.ListView element, double value)
        {
            element.SetValue(MinWidthProperty, value);
        }

        public static double GetMinWidth(System.Windows.Controls.ListView element)
        {
            return (double)element.GetValue(MinWidthProperty);
        }
    }
}
