using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SquaredInfinity.Presentation.Behaviors
{
    public partial class DialogHost
    {
        #region Width

        public static readonly DependencyProperty WidthProperty =
            DependencyProperty.RegisterAttached(
            "Width",
            typeof(double?),
            typeof(DialogHost),
            new PropertyMetadata(null));

        public static void SetWidth(FrameworkElement element, double? value)
        {
            element.SetValue(WidthProperty, value);
        }

        public static double? GetWidth(FrameworkElement element)
        {
            return (double?)element.GetValue(WidthProperty);
        }

        #endregion
    }
}
