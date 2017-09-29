using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace SquaredInfinity.Windows.Converters
{
    public enum IsOfTypeConverterMode
    {
        IsExactMatch,
        IsAssignableFrom
    }

    public class IsOfTypeConverter : IValueConverter
    {
        public IsOfTypeConverterMode Mode { get; set; }

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
                return DependencyProperty.UnsetValue;

            var comparand = parameter as Type;

            if(parameter == null)
            {
                // todo: log warning
                return DependencyProperty.UnsetValue;
            }

            if(Mode == IsOfTypeConverterMode.IsExactMatch)
            {
                return value.GetType() == comparand;
            }
            
            if(Mode == IsOfTypeConverterMode.IsAssignableFrom)
            {
                return value.GetType().IsAssignableFrom(comparand);
            }

            return DependencyProperty.UnsetValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
