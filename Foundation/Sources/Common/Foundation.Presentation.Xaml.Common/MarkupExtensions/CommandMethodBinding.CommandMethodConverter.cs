using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;

namespace SquaredInfinity.Foundation.Presentation.MarkupExtensions
{
    public partial class CommandMethodBinding : SmartBinding
    {
        class CommandMethodConverter : IValueConverter
        {
            readonly string MethodName;

            ICommand InvokeMethodCommand;

            public CommandMethodConverter(string methodName)
            {
                this.MethodName = methodName;
            }

            public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            {
                if (value == null)
                    return DependencyProperty.UnsetValue;
                
                InvokeMethodCommand = new InvokeMethodCommand(value, MethodName);

                return InvokeMethodCommand;
            }

            public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            {
                return DependencyProperty.UnsetValue;
            }
        }
    }

}
