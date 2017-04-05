using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace SquaredInfinity.Presentation.Converters
{
    public class ColumnHeaderRoleToBorderConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var header = value as GridViewColumnHeader; 

            if (header == null)
                return DependencyProperty.UnsetValue;

            if (header.Role == GridViewColumnHeaderRole.Padding)
                return Application.Current.Resources["GridView.ColumnHeader.Padding.Border"];

            return Application.Current.Resources["GridView.ColumnHeader.Normal.Border"];
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value;
        }
    }
}
