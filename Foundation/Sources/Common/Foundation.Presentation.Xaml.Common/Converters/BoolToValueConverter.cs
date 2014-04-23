using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace SquaredInfinity.Foundation.Presentation.Converters
{
    public class BoolToValueConverter : IValueConverter
    {
        public object ValueWhenTrue { get; set; }
        public object ValueWhenFalse { get; set; }

        public BoolToValueConverter()
        {
            ValueWhenTrue = DependencyProperty.UnsetValue;
            ValueWhenFalse = DependencyProperty.UnsetValue;
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var b = (bool)value;

            if (b)
                return ValueWhenTrue;
            else
                return ValueWhenFalse;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
