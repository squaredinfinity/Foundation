using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Input;
using SquaredInfinity.Foundation.Extensions;
using System.Windows;

namespace SquaredInfinity.Foundation.Presentation.MarkupExtensions
{
    public partial class CommandMethodBinding : SmartBinding
    {
        /// <summary>
        /// Implementation of ICommand which calls method with [MethodName] when executing.
        /// Calls Can[MethodName] to check enabled state.
        /// If [MethodName] owner implements INotifyPropertyChanged then changes to property called Can[MethodName] will be monitored
        /// and when changed CanExecute state will be reevaluated.
        /// </summary>
        class InvokeMethodCommand : ICommand
        {
            public event EventHandler CanExecuteChanged;

            readonly object TargetObject;
            readonly string ExecuteMethodName;
            readonly string CanExecuteTriggerPropertyName;

            readonly MethodInfo ExecuteMethodInfo;
            readonly MethodInfo CanExecuteMethodInfo;
            readonly PropertyInfo CanExecutePropertyInfo;
            
            readonly bool ExecuteAcceptsParameter;
            readonly bool CanExecuteAcceptsParameter;

            IDisposable CanExecuteNotifyPropertyChangedSubscription;

            public InvokeMethodCommand(
                object targetObject, 
                string executeMethodName, 
                string canExecuteMethodName, 
                string canExecutePropertyName,
                string canExecuteTriggerPropertyName)
            {
                TargetObject = targetObject;
                ExecuteMethodName = executeMethodName;
                CanExecuteTriggerPropertyName = canExecuteTriggerPropertyName;

                var targetType = targetObject.GetType();

                //# Find Execute Method

                // first try to find execute method which accepts exactly one parameter
                // if this fails, find one without any parameters
                if (!executeMethodName.IsNullOrEmpty())
                {
                    ExecuteMethodInfo =
                        (from m in targetType.GetMethods()
                         where m.GetParameters().Length == 1 && m.Name == executeMethodName
                         select m).FirstOrDefault();

                    if (ExecuteMethodInfo == null)
                    {
                        ExecuteMethodInfo =
                        (from m in targetType.GetMethods()
                         where m.GetParameters().Length == 0 && m.Name == executeMethodName
                         select m).FirstOrDefault();
                    }
                    if (ExecuteMethodInfo == null)
                    {
                        // todo: log warning
                    }
                    else
                    {
                        ExecuteAcceptsParameter = ExecuteMethodInfo.GetParameters().Any();
                    }
                }

                //# Find Can Execute Method
                if (!canExecuteMethodName.IsNullOrEmpty())
                {
                    // first try to find can execute method which accepts exactly one parameter
                    // if this fails, find one without any parameters
                    CanExecuteMethodInfo =
                        (from m in targetType.GetMethods()
                         where m.GetParameters().Length == 1 && m.Name == canExecuteMethodName
                         select m).FirstOrDefault();

                    if (CanExecuteMethodInfo == null)
                    {
                        CanExecuteMethodInfo =
                        (from m in targetType.GetMethods()
                         where m.GetParameters().Length == 0 && m.Name == canExecuteMethodName
                         select m).FirstOrDefault();
                    }

                    if (CanExecuteMethodInfo != null)
                        CanExecuteAcceptsParameter = CanExecuteMethodInfo.GetParameters().Any();
                }

                //# Find Can Execute Property
                if (!canExecutePropertyName.IsNullOrEmpty())
                {
                    CanExecutePropertyInfo =
                        (from p in targetType.GetProperties()
                         where p.CanRead && p.Name == canExecutePropertyName && p.PropertyType == typeof(bool)
                         select p).FirstOrDefault();
                }

                //# Subscribe to change in INotifyPropertyChanged
                var inpc = targetObject as INotifyPropertyChanged;
                if(inpc != null)
                {
                    CanExecuteNotifyPropertyChangedSubscription =
                        inpc.CreateWeakEventHandler()
                        .ForEvent<PropertyChangedEventHandler, PropertyChangedEventArgs>(
                        (s, h) => s.PropertyChanged += h,
                        (s, h) => s.PropertyChanged -= h)
                        .Subscribe((s, args) => 
                            {
                                if (args.PropertyName == canExecuteMethodName
                                    || args.PropertyName == canExecuteTriggerPropertyName)
                                {
                                    RaiseCanExecuteChanged();
                                }
                            });
                }
            }

            void RaiseCanExecuteChanged()
            {
                if (CanExecuteChanged != null)
                {
                    // it is required for CanExecuteChanged handlers to run on UI Thread
                    // because most XAML control handlers (e.g. on button control) will try to access UI elements in some way.

                    var dispatcher = UIService.GetMainThreadDispatcher();

                    if(!dispatcher.CheckAccess())
                    {
                        dispatcher.Invoke(new Action(() => RaiseCanExecuteChanged()));
                        return;
                    }

                    CanExecuteChanged(this, EventArgs.Empty);
                }
            }

            public bool CanExecute(object parameter)
            {
                if (CanExecuteMethodInfo == null)
                {
                    if (CanExecutePropertyInfo == null)
                        return true;
                    else
                        return (bool)CanExecutePropertyInfo.GetValue(TargetObject);
                }

                if (CanExecuteAcceptsParameter)
                    return (bool)CanExecuteMethodInfo.Invoke(TargetObject, new object[] { parameter });
                else
                    return (bool)CanExecuteMethodInfo.Invoke(TargetObject, null);
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
