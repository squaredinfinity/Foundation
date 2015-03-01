using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Data;

namespace SquaredInfinity.Foundation.Presentation.MarkupExtensions
{
    public partial class MethodResultBinding : SmartBinding
    {
        class MethodWithParametersResultConverter : IMultiValueConverter
        {
            readonly string MethodName;
            readonly object[] HardParameters;
            readonly int NonParameterBindingsCount;
            MethodInfo MethodInfo;

            public MethodWithParametersResultConverter(string methodName, int nonParameterBindingsCount, IEnumerable<object> hardParameters)
            {
                this.MethodName = methodName;
                this.NonParameterBindingsCount = nonParameterBindingsCount;

                if (hardParameters != null && hardParameters.Any())
                {
                    this.HardParameters = hardParameters.ToArray();
                }
            }

            public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
            {
                try
                {
                    var value = values.FirstOrDefault();

                    if (value == null || value == DependencyProperty.UnsetValue)
                        return DependencyProperty.UnsetValue;

                    var parameters = values.Skip(NonParameterBindingsCount);

                    if (MethodName == null)
                        return DependencyProperty.UnsetValue;

                    if (MethodInfo == null)
                    {
                        var target = value.GetType();

                        MethodInfo = target.GetMethod(MethodName);

                        // still cannot find the method, return unset value
                        if(MethodInfo == null)
                        {
                            return DependencyProperty.UnsetValue;
                        }
                    }

                    if (HardParameters == null)
                    {
                        return MethodInfo.Invoke(value, parameters.ToArray());
                    }
                    else
                    {
                        return MethodInfo.Invoke(value, HardParameters);
                    }
                }
                catch (Exception ex)
                {
                    //todo
                   // Logger.LogException(ex);
                    return DependencyProperty.UnsetValue;
                }
            }

            public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
            {
                throw new NotImplementedException();
            }
        }
    }
}
