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
    public class BoolToVisibilityConverter : IValueConverter, IMultiValueConverter
    {
        public BoolToVisibilityConverterMode Mode { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Visibility trueValue = Visibility.Visible;
            Visibility falseValue = Visibility.Collapsed;

            switch (Mode)
            {
                case BoolToVisibilityConverterMode.CollapsedWhenTrue:
                    trueValue = Visibility.Collapsed;
                    falseValue = Visibility.Visible;
                    break;
                case BoolToVisibilityConverterMode.VisibleWhenTrue:
                    trueValue = Visibility.Visible;
                    falseValue = Visibility.Collapsed;
                    break;
                case BoolToVisibilityConverterMode.VisibleWhenTrueHiddenWhenFalse:
                    trueValue = Visibility.Visible;
                    falseValue = Visibility.Hidden;
                    break;
            }

            if ((bool)value)
                return trueValue;
            else
                return falseValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            // all values should be boolean
            // if any value is UnSet, then by default ignore it (this behavior may need to be configurable later)

            var result = (bool?) null;

            foreach(var val in values)
            {
                if (val == DependencyProperty.UnsetValue)
                    continue;

                var bool_val = System.Convert.ToBoolean(val);

                if (result.HasValue)
                    result = result.Value && bool_val;
                else
                    result = bool_val;
            }

            if (result.HasValue)
                return result.Value;
            else
                return DependencyProperty.UnsetValue;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public enum BoolToVisibilityConverterMode
    {
        Default,
        VisibleWhenTrue = Default,
        CollapsedWhenTrue,
        HiddenWhenTrue,
        VisibleWhenTrueHiddenWhenFalse
    }
}
