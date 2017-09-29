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
    public class MultiplyNumberConverter : IValueConverter
    {
        public int Multiplier { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value is Int16)
            {
                return ((Int16)value) * Multiplier;
            }

            if (value is UInt16)
            {
                return ((UInt16)value) * Multiplier;
            }

            if (value is Int32)
            {
                return ((Int32)value) * Multiplier;
            }

            if (value is UInt32)
            {
                return ((UInt32)value) * Multiplier;
            }

            return DependencyProperty.UnsetValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
