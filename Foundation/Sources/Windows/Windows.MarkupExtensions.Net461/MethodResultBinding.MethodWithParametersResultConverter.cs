using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Data;

namespace SquaredInfinity.Windows.MarkupExtensions
{
    public partial class MethodResultBinding : SmartBinding
    {
        public class MethodWithParametersResultConverter : IMultiValueConverter
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
                    {
                        //InternalTrace.Error("Binding error: Method Name is empty");
                        return DependencyProperty.UnsetValue;
                    }

                    if (MethodInfo == null)
                    {
                        var target = value.GetType();

                        MethodInfo = target.GetMethod(MethodName);

                        // still cannot find the method, return unset value
                        if(MethodInfo == null)
                        {
                            //InternalTrace.Error("Binding error: Cannot find method {0} on type {1}".FormatWith(MethodName, target.Name));
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
                    //InternalTrace.Error("Binding error: Method Result Binding for method {0} failed: {1}".FormatWith(MethodName, ex.ToString()));
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
