using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace SquaredInfinity.Windows.Converters
{
    public class IndentToLeftMarginConverter : IValueConverter
    {
        public int IndentLevelMarginSize { get; set; }

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var indent = (int)value;

            return new Thickness(indent * IndentLevelMarginSize, 0, 0, 0);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
