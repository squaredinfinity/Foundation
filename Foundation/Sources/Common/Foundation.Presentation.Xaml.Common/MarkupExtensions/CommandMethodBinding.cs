using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace SquaredInfinity.Foundation.Presentation.MarkupExtensions
{
    public partial class CommandMethodBinding : SmartBinding
    {
        public string MethodName { get; set; }

        public CommandMethodBinding()
        { }

        public CommandMethodBinding(string methodName)
        {
            MethodName = methodName;
        }

        protected override BindingBase InitialiseBinding(IServiceProvider serviceProvider)
        {
            var binding = new Binding();
            binding.Converter = new CommandMethodConverter(MethodName);

            UpdateBindingFromSource(binding);

            return binding;
        }
    }

}
