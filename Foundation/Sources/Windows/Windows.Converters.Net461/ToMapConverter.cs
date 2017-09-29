using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace SquaredInfinity.Windows.Converters
{
    public class ToMapConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var result = new Dictionary<string, object>(StringComparer.InvariantCultureIgnoreCase);

            for (int i = 0; i < values.Length; i += 2)
            {
                result.Add(values[i].ToString(), values[i + 1]);
            }

            return result;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
