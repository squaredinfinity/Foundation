using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows.Data;

namespace SquaredInfinity.Foundation.Presentation.Converters
{
    public class NumbersSumConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var sum = 0.0m;

            var addend = 0.0m;

            for (int i = 0; i < values.Length; i++)
            {
                var raw_addend = values[i] as IConvertible;


                if (raw_addend == null)
                    continue;

                addend = System.Convert.ToDecimal(raw_addend);

                sum += addend;
            }

            return sum;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
