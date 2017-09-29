using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace SquaredInfinity.Windows.MarkupExtensions
{
    public class ElementBinding : SmartBinding
    {
        public ElementBinding()
        { }

        public ElementBinding(string source)
        {
            base.Source = source;
        }

        protected override BindingBase InitialiseBinding(IServiceProvider serviceProvider)
        {
            var binding = new Binding();

            UpdateBindingFromSource(binding);

            binding.Converter = Converter;
            binding.ConverterParameter = ConverterParameter;

            return binding;
        }
    }
}
