using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace SquaredInfinity.Windows.Converters
{
    public class CollectionPositionToBoolConverter : IMultiValueConverter
    {
        CollectionPositionToBoolConverterMode _mode = CollectionPositionToBoolConverterMode.TrueWhenFirst;
        public CollectionPositionToBoolConverterMode Mode
        {
            get { return _mode; }
            set { _mode = value; }
        }

        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            // 1: collection
            // 2: item

            var itemIndex = -1;

            var item = values[1];

            var list = values[0] as IList;
            if (list != null)
            {
                itemIndex = list.IndexOf(item);
            }
            else
            {
                return DependencyProperty.UnsetValue;
            }

            switch (Mode)
            {
                case CollectionPositionToBoolConverterMode.TrueWhenFirst:
                    return itemIndex == 0;
                case CollectionPositionToBoolConverterMode.TrueWhenLast:
                    return itemIndex == list.Count - 1;
                default:
                    break;
            }

            return DependencyProperty.UnsetValue;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public enum CollectionPositionToBoolConverterMode
    {
        Default = 0,
        TrueWhenFirst = Default,
        TrueWhenLast
    }
}
