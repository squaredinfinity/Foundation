using SquaredInfinity.Foundation.Extensions;
using SquaredInfinity.Foundation.Presentation.Converters;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

[assembly: XmlnsDefinition("http://schemas.microsoft.com/winfx/2006/xaml/presentation", "SquaredInfinity.Foundation.Presentation.MarkupExtensions")]

namespace SquaredInfinity.Foundation.Presentation.MarkupExtensions
{
    public partial class ViewModelBinding : SmartBinding
    {
        public ViewModelBinding()
        { }

        public ViewModelBinding(string source)
        {
            base.Source = source;
        }

        protected override BindingBase InitialiseBinding(IServiceProvider serviceProvider)
        {
            var multiBinding = new MultiBinding();

            var contextBinding = new Binding();
            UpdateBindingFromSource(contextBinding);
            contextBinding.Mode = BindingMode.OneTime;
            multiBinding.Bindings.Add(contextBinding);
            multiBinding.Converter = new MixedCompositeConverter(
                new ViewModelBindingConverter(),
                Converter);

            return multiBinding;
        }
    }

    public partial class TemplateViewModelBinding : TemplateBindingExtension
    {
        public TemplateViewModelBinding()
        { }

        public TemplateViewModelBinding(DependencyProperty property)
        {
            if (property != null)
            {
                base.Property = property;

                Converter = new ViewModelBindingConverter();
            }
            else
            {
                throw new ArgumentNullException("property");
            }
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return ProvideValue(serviceProvider);
        }
    }

        class ViewModelBindingConverter : IMultiValueConverter, IValueConverter
        {
            public ViewModelBindingConverter()
            { }

            public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
            {
                try
                {
                    var source = values.FirstOrDefault();

                    return Convert(source, targetType, parameter, culture);                    
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

            public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            {
                var source = value;

                if (source == null)
                    return source;

                var sourceType = source.GetType();

                var sourceTypeName = sourceType.Name;

                var vmTypeName = sourceTypeName + "ViewModel";

                var vmType = TypeExtensions.ResolveType(vmTypeName, ignoreCase: true);

                if (vmType == null)
                    return source;

                var constructor_with_source = vmType.GetConstructor(new Type[] { sourceType });

                if (constructor_with_source != null)
                {
                    var result = constructor_with_source.Invoke(new object[] { source });
                    return result;
                }

                var constructor = vmType.GetConstructor(new Type[] { });

                if (constructor != null)
                {
                    var result = constructor.Invoke(null);
                    return result;
                }

                return source;
            }

            public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            {
                throw new NotImplementedException();
            }
        }
}
