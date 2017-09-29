using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

namespace SquaredInfinity.Windows.Converters
{
    /// <summary>
    /// Performs conversion using a chain of converters, where original value is passed to first converter
    /// and every next converter gets passed a result of previous conversion.
    /// </summary>
    [ContentProperty("Converters")]
    public class CompositeConverter : IValueConverter, IMultiValueConverter
    {
        List<object> _converters = new List<object>();
        public List<object> Converters
        {
            get { return _converters; }
            set { _converters = value; }
        }

        public CompositeConverter()
        { }

        public CompositeConverter(params object[] converters)
        {
            this.Converters.AddRange(
                    from c in converters
                    where c != null
                    select c);
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var first = Converters.First() as IValueConverter;

            if (first == null)
                return DependencyProperty.UnsetValue;

            object result = first.Convert(value, targetType, parameter, culture);

            foreach (var converter in Converters.Skip(1).Cast<IValueConverter>())
                result = converter.Convert(result, targetType, parameter, culture);

            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var first = Converters.First() as IMultiValueConverter;

            if (first == null)
                return DependencyProperty.UnsetValue;

            object result = first.Convert(values, targetType, parameter, culture);

            foreach (var converter in Converters.Skip(1).Cast<IValueConverter>())
                result = converter.Convert(result, targetType, parameter, culture);

            return result;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
