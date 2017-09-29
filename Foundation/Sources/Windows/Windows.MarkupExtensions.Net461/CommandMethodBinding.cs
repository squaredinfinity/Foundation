using System;
using System.Windows.Data;

namespace SquaredInfinity.Windows.MarkupExtensions
{
    public partial class CommandMethodBinding : SmartBinding
    {
        public string ExecuteMethodName { get; set; }
        public string CanExecuteMethodName { get; set; }
        public string CanExecutePropertyName { get; set; }

        public string CanExecuteTriggerPropertyName { get; set; }
        
        public CommandMethodBinding()
        { }

        public CommandMethodBinding(string executeMethodNameOrFullSourcePath)
        {
            var ix_last_dot = executeMethodNameOrFullSourcePath.LastIndexOf('.');

            if (ix_last_dot < 0)
            {
                ExecuteMethodName = executeMethodNameOrFullSourcePath;
            }
            else
            {
                ExecuteMethodName = executeMethodNameOrFullSourcePath.Substring(ix_last_dot + 1);

                Source = executeMethodNameOrFullSourcePath.Substring(0, ix_last_dot);
            }
        }

        protected override BindingBase InitialiseBinding(IServiceProvider serviceProvider)
        {
            if (CanExecuteMethodName == null)
                CanExecuteMethodName = "Can" + ExecuteMethodName;

            if (CanExecutePropertyName == null)
                CanExecutePropertyName = "Can" + ExecuteMethodName;

            var binding = new Binding();
            binding.Converter = new CommandMethodConverter(ExecuteMethodName, CanExecuteMethodName, CanExecutePropertyName, CanExecuteTriggerPropertyName);
            UpdateBindingFromSource(binding);

            return binding;
        }
    }

}
