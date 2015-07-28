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
using SquaredInfinity.Foundation.Extensions;

[assembly: XmlnsDefinition("http://schemas.microsoft.com/winfx/2006/xaml/presentation", "SquaredInfinity.Foundation.Presentation.MarkupExtensions")]

namespace SquaredInfinity.Foundation.Presentation.MarkupExtensions
{
    public partial class TemplatedParentBinding : SmartBinding
    {
        public TemplatedParentBinding()
        { }

        public TemplatedParentBinding(string source)
        {
            base.Source = source;
        }

        protected override BindingBase InitialiseBinding(IServiceProvider serviceProvider)
        {
            var binding = new Binding();

            UpdateBindingFromSource(binding);

            binding.RelativeSource = new RelativeSource { Mode = RelativeSourceMode.TemplatedParent };

            binding.Converter = Converter;
            binding.ConverterParameter = ConverterParameter;

            return binding;
        }
    }
}
