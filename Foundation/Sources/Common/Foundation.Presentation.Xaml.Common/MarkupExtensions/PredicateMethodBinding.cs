using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace SquaredInfinity.Foundation.Presentation.MarkupExtensions
{
    public partial class PredicateMethodBinding : SmartBinding
    {
        public string MethodName { get; set; }

        public PredicateMethodBinding()
        { }

        public PredicateMethodBinding(string methodName)
        {
            MethodName = methodName;
        }

        protected override BindingBase InitialiseBinding(IServiceProvider serviceProvider)
        {
            var binding = new Binding();
            binding.Converter = new PredicateMethodConverter(MethodName);
            binding.Mode = BindingMode.OneTime;

            UpdateBindingFromSource(binding);

            return binding;
        }
    }
}
