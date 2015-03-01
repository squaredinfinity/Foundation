using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace SquaredInfinity.Foundation.Presentation.Converters
{
    public class EnumerableCombiningConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if(values == null)
                return DependencyProperty.UnsetValue;

            var result = Enumerable.Empty<object>();

            foreach(var enumerable in values.OfType<IEnumerable<object>>())
            {
                result = result.Concat(enumerable);
            }

            return result;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
