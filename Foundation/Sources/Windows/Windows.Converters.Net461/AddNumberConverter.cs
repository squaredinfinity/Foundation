using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace SquaredInfinity.Windows.Converters
{
    public class AddNumberConverter : IValueConverter
    {
        public int NumberToAdd { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value is Int16)
            {
                return ((Int16) value) + NumberToAdd;
            }

            if (value is UInt16)
            {
                return ((UInt16)value) + NumberToAdd;
            }

            if (value is Int32)
            {
                return ((Int32)value) + NumberToAdd;
            }

            if (value is UInt32)
            {
                return ((UInt32)value) + NumberToAdd;
            }

            return DependencyProperty.UnsetValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
