using SquaredInfinity.Extensions;
using SquaredInfinity.Presentation.Converters;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;
using SquaredInfinity.Extensions;

[assembly: XmlnsDefinition("http://schemas.microsoft.com/winfx/2006/xaml/presentation", "SquaredInfinity.Presentation.MarkupExtensions")]

namespace SquaredInfinity.Presentation.MarkupExtensions
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
