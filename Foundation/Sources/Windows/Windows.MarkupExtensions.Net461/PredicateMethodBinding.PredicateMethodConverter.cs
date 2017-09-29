using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace SquaredInfinity.Windows.MarkupExtensions
{
    public partial class PredicateMethodBinding : SmartBinding
    {
        class PredicateMethodConverter : IValueConverter
        {
            readonly string MethodName;

            Predicate<object> Filter;

            public PredicateMethodConverter(string methodName)
            {
                this.MethodName = methodName;
            }

            public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            {
                if (value == null || value == DependencyProperty.UnsetValue)
                    return DependencyProperty.UnsetValue;

                //if (Filter == null)
                {
                    var target = value.GetType();

                    var filterMethodInfo = target.GetMethod(MethodName);

                    Filter = new Predicate<object>((item) =>
                    {
                        return (bool)filterMethodInfo.Invoke(value, new object[] { item });
                    });
                }

                return Filter;
            }

            public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            {
                return DependencyProperty.UnsetValue;
            }
        }
    }
}
