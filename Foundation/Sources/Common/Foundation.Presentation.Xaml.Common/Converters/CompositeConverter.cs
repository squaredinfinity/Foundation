using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

namespace SquaredInfinity.Foundation.Presentation.Converters
{
    /// <summary>
    /// Performs conversion using a chain of converters, where original value is passed to first converter
    /// and every next converter gets passed a result of previous conversion.
    /// </summary>
    [ContentProperty("Converters")]
    public class CompositeConverter : IValueConverter
    {
        List<IValueConverter> _converters = new List<IValueConverter>();
        public List<IValueConverter> Converters
        {
            get { return _converters; }
            set { _converters = value; }
        }

        public CompositeConverter()
        { }

        public CompositeConverter(params IValueConverter[] converters)
        {
            this.Converters.AddRange(
                    from c in converters
                    where c != null
                    select c);
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            object result = Converters.First().Convert(value, targetType, parameter, culture);

            foreach (var converter in Converters.Skip(1))
                result = converter.Convert(result, targetType, parameter, culture);

            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }
    }
}
