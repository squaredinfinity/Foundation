using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SquaredInfinity.Extensions;
using System.Windows;
using System.Windows.Data;
using System.Windows.Controls;

namespace SquaredInfinity.Presentation.Converters
{
    public class TreeViewItemLeftMarginConverter : IValueConverter
    {
        public double MarginSize { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var item = value as TreeViewItem;
            if (item == null)
                return new Thickness(0);

            return new Thickness(MarginSize * item.GetDepth(), 0, 0, 0);

        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
