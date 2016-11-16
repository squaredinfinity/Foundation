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
        #region Height

        public static readonly DependencyProperty HeightProperty =
            DependencyProperty.RegisterAttached(
            "Height",
            typeof(double?),
            typeof(DialogHost),
            new PropertyMetadata(null));

        public static void SetHeight(FrameworkElement element, double? value)
        {
            element.SetValue(HeightProperty, value);
        }

        public static double? GetHeight(FrameworkElement element)
        {
            return (double?)element.GetValue(HeightProperty);
        }

        #endregion
    }
}
