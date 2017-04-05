using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace SquaredInfinity.Presentation.MarkupExtensions
{
    public partial class PredicateMethodBinding : SmartBinding
    {
        public string PredicateMethodName { get; set; }

        public PredicateMethodBinding()
        { }
        
        public PredicateMethodBinding(string methodNameOrFullSourcePath)
        {
            var ix_last_dot = methodNameOrFullSourcePath.LastIndexOf('.');

            if (ix_last_dot < 0)
            {
                PredicateMethodName = methodNameOrFullSourcePath;
            }
            else
            {
                PredicateMethodName = methodNameOrFullSourcePath.Substring(ix_last_dot + 1);

                Source = methodNameOrFullSourcePath.Substring(0, ix_last_dot);
            }
        }

        protected override BindingBase InitialiseBinding(IServiceProvider serviceProvider)
        {
            var binding = new Binding();
            binding.Converter = new PredicateMethodConverter(PredicateMethodName);

            UpdateBindingFromSource(binding);

            return binding;
        }
    }
}
