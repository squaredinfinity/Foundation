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
    /// <summary>
    /// Performs conversion using a chain of converters, where original value is passed to first converter
    /// and every next converter gets passed a result of previous conversion.
    /// First Converter must always be a MultiValueConverter.
    /// Subsequent Converters must always be IValueConverters.
    /// </summary>
    public class MixedCompositeConverter : IMultiValueConverter
    {
        /// <summary>
        /// Contains one IMultiValueConverter (first) followed by one or more IValueConverters
        /// </summary>
        readonly IEnumerable<object> Converters;

        public MixedCompositeConverter(params object[] converters)
        {
            this.Converters =
                    from c in converters
                    where c != null
                    select c;
        }

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var multi = Converters.First() as IMultiValueConverter;

            object result = multi.Convert(values, targetType, parameter, culture);

            foreach (var converter in Converters.Skip(1).Cast<IValueConverter>())
            {
                result = converter.Convert(result, targetType, parameter, culture);
            }

            return result;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
