using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Input;

namespace SquaredInfinity.Foundation.Presentation.MarkupExtensions
{
    public partial class CommandMethodBinding : SmartBinding
    {
        class InvokeMethodCommand : ICommand
        {
            public event EventHandler CanExecuteChanged;

            protected readonly object TargetObject;
            protected readonly string MethodName;

            protected readonly MethodInfo ExecuteMethodInfo;
            protected readonly MethodInfo CanExecuteMethodInfo;

            protected readonly bool ExecuteAcceptsParameter;
            protected readonly bool CanExecuteAcceptsParameter;

            public InvokeMethodCommand(object targetObject, string methodName)
            {
                this.TargetObject = targetObject;
                this.MethodName = methodName;

                var targetType = targetObject.GetType();

                var executeMethodInfo = targetType.GetMethod(methodName);
                var canExecuteMethodInfo = targetType.GetMethod("Can" + methodName);

                this.ExecuteMethodInfo = executeMethodInfo;
                this.CanExecuteMethodInfo = canExecuteMethodInfo;

                if (ExecuteMethodInfo == null)
                {
                    // log warning
                }
                else
                {
                    this.ExecuteAcceptsParameter = ExecuteMethodInfo.GetParameters().Any();
                }

                if (CanExecuteMethodInfo != null)
                    this.CanExecuteAcceptsParameter = CanExecuteMethodInfo.GetParameters().Any();
            }

            public bool CanExecute(object parameter)
            {
                if (CanExecuteMethodInfo == null)
                    return true;

                if (CanExecuteAcceptsParameter)
                {
                    return (bool)CanExecuteMethodInfo.Invoke(TargetObject, new object[] { parameter });
                }
                else
                {
                    return (bool)CanExecuteMethodInfo.Invoke(TargetObject, null);
                }
            }

            public void Execute(object parameter)
            {
                if (ExecuteAcceptsParameter)
                {
                    ExecuteMethodInfo.Invoke(TargetObject, new object[] { parameter });
                }
                else
                {
                    ExecuteMethodInfo.Invoke(TargetObject, null);
                }
            }
        }
    }

}
