using System;
using System.Windows.Data;

namespace SquaredInfinity.Windows.MarkupExtensions
{
    /// <summary>
    /// Binding to RelativeSource TemplatedParent
    /// </summary>
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
