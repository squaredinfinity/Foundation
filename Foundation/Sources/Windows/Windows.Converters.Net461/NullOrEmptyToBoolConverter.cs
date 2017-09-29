using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace SquaredInfinity.Windows.Converters
{
    public class NullOrEmptyToBoolConverter : IValueConverter
    {
        public NullOrEmptyToBoolConverterMode Mode { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var valueWhenNullOrEmpty = false;
            var valueWhenNotNullOrEmpty = true;

            switch (Mode)
            {
                case NullOrEmptyToBoolConverterMode.FalseWhenNullOrEmpty:
                    break;
                case NullOrEmptyToBoolConverterMode.TrueWhenNullOrEmpty:
                    valueWhenNullOrEmpty = true;
                    valueWhenNotNullOrEmpty = false;
                    break;
            }

            if(value == null || value == DependencyProperty.UnsetValue)
                return valueWhenNullOrEmpty;

            //+ String
            if (value is string)
            {
                if (string.IsNullOrEmpty(value as string))
                    return valueWhenNullOrEmpty;
                else
                    return valueWhenNotNullOrEmpty;
            }

            //+ ICollection
            var valueAsICollection = value as ICollection;
            if (valueAsICollection != null && valueAsICollection.Count <= 0)
                return valueWhenNullOrEmpty;
            
            return valueWhenNotNullOrEmpty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public enum NullOrEmptyToBoolConverterMode
    {
        Default,
        FalseWhenNullOrEmpty = Default,
        TrueWhenNullOrEmpty
    }
}
