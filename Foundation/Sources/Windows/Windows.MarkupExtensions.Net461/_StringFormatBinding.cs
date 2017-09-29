//using SquaredInfinity.Extensions;
//using SquaredInfinity.Presentation.Converters;
//using System;
//using System.Collections.Generic;
//using System.Globalization;
//using System.Linq;
//using System.Text;
//using System.Windows;
//using System.Windows.Data;
//using System.Windows.Markup;
//using SquaredInfinity.Extensions;

//namespace SquaredInfinity.Windows.MarkupExtensions
//{
//    public partial class StringFormatBinding : SmartBinding
//    {
//        public string StringFormatBindingPath { get; set; }

//        public StringFormatBinding()
//        { }

//        public StringFormatBinding(string source)
//        {
//            base.Source = source;
//        }

//        protected override BindingBase InitialiseBinding(IServiceProvider serviceProvider)
//        {
//            // string format binding is a multibinding where:
//            // 1 - text to format
//            // 2 - format string

//            var multiBinding = new MultiBinding();

//            var contextBinding = new Binding();
//            UpdateBindingFromSource(contextBinding);
//            multiBinding.Bindings.Add(contextBinding);

//            var stringFormatBinding = new Binding();
//            UpdateBindingFromSource(stringFormatBinding, StringFormatBindingPath);
//            multiBinding.Bindings.Add(stringFormatBinding);
            
//            multiBinding.Converter = new MixedCompositeConverter(
//                new StringFormatBindingConverter(),
//                Converter);

//            return multiBinding;
//        }
//    }

//        class StringFormatBindingConverter : IMultiValueConverter
//        {
//            public StringFormatBindingConverter()
//            { }

//            public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
//            {
//                try
//                {
//                    if (values.Length < 2)
//                        return DependencyProperty.UnsetValue;

//                    var val1 = values[0];
//                    var val2 = values[1];

//                    if (val1 == DependencyProperty.UnsetValue || val2 == DependencyProperty.UnsetValue)
//                        return DependencyProperty.UnsetValue;

//                    var valueToFormat = val1 as IFormattable;
//                    var formatString = val2 as string;

//                    if (valueToFormat == null)
//                        return DependencyProperty.UnsetValue;

//                    if(formatString == null || formatString == string.Empty)
//                        return valueToFormat.ToString();

//                    var result = valueToFormat.ToString(formatString, NumberFormatInfo.CurrentInfo);

//                    return result;
//                }
//                catch (Exception ex)
//                {
//                    return DependencyProperty.UnsetValue;
//                }
//            }

//            public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
//            {
//                throw new NotImplementedException();
//            }
//        }
//}
