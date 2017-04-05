using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace SquaredInfinity.Presentation.Converters
{
    public class ToDoubleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            try
            {
                if (value == null)
                    return DependencyProperty.UnsetValue;

                var nullableUnderlyingType = Nullable.GetUnderlyingType(value.GetType());

                if (nullableUnderlyingType != null)
                    return (double)System.Convert.ToDouble(nullableUnderlyingType);

                return (double)System.Convert.ToDouble(value);
            }
            catch(Exception ex)
            {
                // log
                return DependencyProperty.UnsetValue;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
