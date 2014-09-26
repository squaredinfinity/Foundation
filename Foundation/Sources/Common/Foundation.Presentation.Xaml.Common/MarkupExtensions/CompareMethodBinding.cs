using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using SquaredInfinity.Foundation.Extensions;

namespace SquaredInfinity.Foundation.Presentation.MarkupExtensions
{
    public partial class CompareMethodBinding : SmartBinding
    {
        public string CompareMethodName { get; set; }

        public CompareMethodBinding()
        { }

        public CompareMethodBinding(string methodNameOrFullSourcePath)
        {
            var ix_last_dot = methodNameOrFullSourcePath.LastIndexOf('.');

            if (ix_last_dot < 0)
            {
                CompareMethodName = methodNameOrFullSourcePath;
            }
            else
            {
                CompareMethodName = methodNameOrFullSourcePath.Substring(ix_last_dot + 1);

                Source = methodNameOrFullSourcePath.Substring(0, ix_last_dot);
            }
        }

        protected override BindingBase InitialiseBinding(IServiceProvider serviceProvider)
        {
            var binding = new Binding();
            binding.Converter = new CompareMethodConverter(CompareMethodName);

            UpdateBindingFromSource(binding);

            return binding;
        }

        class CompareMethodConverter : IValueConverter
        {
            readonly string MethodName;

            Func<object, object, int> Compare;

            public CompareMethodConverter(string methodName)
            {
                this.MethodName = methodName;
            }

            public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            {
                if (value == null)
                    return DependencyProperty.UnsetValue;

                //if (Compare == null)
                {
                    var target = value.GetType();

                    var filterMethodInfo = target.GetMethod(MethodName);

                    if (filterMethodInfo == null)
                    {
                        Trace.WriteLine("Unable to find filter method '{0}'".FormatWith(MethodName));
                    }
                    else
                    {
                        Compare = new Func<object, object, int>((x, y) =>
                        {
                            return (int)filterMethodInfo.Invoke(value, new object[] { x, y });
                        });
                    }
                }

                return Compare;
            }

            public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            {
                return DependencyProperty.UnsetValue;
            }
        }
    }
}
