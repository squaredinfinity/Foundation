using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

namespace SquaredInfinity.Foundation.Presentation.Converters
{
    public class EnumToValuesConverter : MarkupExtension, IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return DependencyProperty.UnsetValue;

            var en = value as Enum;
            
            // check if value is an instance of an Enum
            if (en != null)
            {
                var enumValues = Enum.GetValues(en.GetType());
                return enumValues;
            }

            // check if value is an Enum Type
            var enType = value as Type;
            if(enType != null && enType.IsEnum)
            {
                var enumValues = Enum.GetValues(enType);
                return enumValues;
            }

            return DependencyProperty.UnsetValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }
    }
}
