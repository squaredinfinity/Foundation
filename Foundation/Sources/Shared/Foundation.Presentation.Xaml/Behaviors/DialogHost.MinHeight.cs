using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SquaredInfinity.Foundation.Presentation.Behaviors
{
    public partial class DialogHost
    {
        #region MinHeight

        public static readonly DependencyProperty MinHeightProperty =
            DependencyProperty.RegisterAttached(
            "MinHeight",
            typeof(double?),
            typeof(DialogHost),
            new PropertyMetadata(null));

        public static void SetMinHeight(FrameworkElement element, double? value)
        {
            element.SetValue(MinHeightProperty, value);
        }

        public static double? GetMinHeight(FrameworkElement element)
        {
            return (double?)element.GetValue(MinHeightProperty);
        }

        #endregion
    }
}
