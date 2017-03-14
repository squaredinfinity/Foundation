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
        #region MinWidth

        public static readonly DependencyProperty MinWidthProperty =
            DependencyProperty.RegisterAttached(
            "MinWidth",
            typeof(double?),
            typeof(DialogHost),
            new PropertyMetadata(null));

        public static void SetMinWidth(FrameworkElement element, double? value)
        {
            element.SetValue(MinWidthProperty, value);
        }

        public static double? GetMinWidth(FrameworkElement element)
        {
            return (double?)element.GetValue(MinWidthProperty);
        }

        #endregion
    }
}
