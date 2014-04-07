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
    public class BoolToVisibilityConverter : IValueConverter
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
    }

    public enum BoolToVisibilityConverterMode
    {
        Default,
        VisibleWhenTrue = Default,
        CollapsedWhenTrue,
        VisibleWhenTrueHiddenWhenFalse
    }
}
