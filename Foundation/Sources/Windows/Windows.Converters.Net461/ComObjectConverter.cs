using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Data;

namespace SquaredInfinity.Windows.Converters
{
    /// <summary>
    /// Not sure if this is safe to use, may be better to map ComObject to custom type first just to be safe.
    /// </summary>
    public class ComObjectConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value;
        }
    }
}
