using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Collections;

namespace SquaredInfinity.Windows.Converters
{
    public class CollectionLastItemToVisibilityConverter : IMultiValueConverter
    {
        bool collapsedWhenLast = true;
        public bool CollapsedWhenLast
        {
            get { return collapsedWhenLast; }
            set { collapsedWhenLast = value; }
        }

        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var visibilityWhenLast = Visibility.Collapsed;
            var visibilityWhenNotLast = Visibility.Visible;

            if (!CollapsedWhenLast)
            {
                visibilityWhenLast = Visibility.Visible;
                visibilityWhenNotLast = Visibility.Collapsed;
            }

            //! 1st param - List
            //! 2nd param - item

            IList list = values[0] as IList;

            var isLast = list.IndexOf(values[1]) == list.Count - 1;

            if (isLast)
                return visibilityWhenLast;
            else
                return visibilityWhenNotLast;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
