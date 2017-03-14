using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;

namespace SquaredInfinity.Presentation.MarkupExtensions
{
    public partial class CommandMethodBinding : SmartBinding
    {
        class CommandMethodConverter : IValueConverter
        {

            //ICommand InvokeMethodCommand;
            readonly string ExecuteMethodName;
            readonly string CanExecuteMethodName;
            readonly string CanExecutePropertyName;
            readonly string CanExecuteTriggerPropertyName;

            public CommandMethodConverter(
                string executeMethodName, 
                string canExecuteMethodName, 
                string canExecutePropertyName, 
                string canExecuteTriggerPropertyName)
            {
                ExecuteMethodName = executeMethodName;
                CanExecuteMethodName = canExecuteMethodName;
                CanExecutePropertyName = canExecutePropertyName;
                CanExecuteTriggerPropertyName = canExecuteTriggerPropertyName;
            }

            public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            {
                if (value == null || value == DependencyProperty.UnsetValue)
                {
                    return DependencyProperty.UnsetValue;
                }
                
                return new InvokeMethodCommand(
                    value, 
                    ExecuteMethodName, 
                    CanExecuteMethodName, 
                    CanExecutePropertyName,
                    CanExecuteTriggerPropertyName);
            }

            public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            {
                return DependencyProperty.UnsetValue;
            }

            ~CommandMethodConverter()
            {

            }
        }
    }

}
