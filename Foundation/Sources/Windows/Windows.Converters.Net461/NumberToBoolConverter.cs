using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace SquaredInfinity.Windows.Converters
{
    public class NumberToBoolConverter : IValueConverter
    {
        public NumberToBoolConverterMode Mode { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var n = System.Convert.ToDecimal(value);

            switch (Mode)
            {
                case NumberToBoolConverterMode.TrueWhenPositive:
                    return n > 0;
                case NumberToBoolConverterMode.TrueWhenNegative:
                    return n < 0;
            }

            return DependencyProperty.UnsetValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public enum NumberToBoolConverterMode
    {
        Default,
        TrueWhenPositive = Default,
        TrueWhenNegative
    }
}
