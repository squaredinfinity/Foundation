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

        public CommandMethodBinding(string methodNameOrFullSourcePath)
        {
            var ix_last_dot = methodNameOrFullSourcePath.LastIndexOf('.');

            if (ix_last_dot < 0)
            {
                MethodName = methodNameOrFullSourcePath;
            }
            else
            {
                MethodName = methodNameOrFullSourcePath.Substring(ix_last_dot + 1);

                Source = methodNameOrFullSourcePath.Substring(0, ix_last_dot);
            }
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
