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

            readonly WeakReference<object> TargetObjectReference;

            readonly string ExecuteMethodName;
            readonly string CanExecuteTriggerPropertyName;

            readonly MethodInfo ExecuteMethodInfo_NoParameters;
            readonly MethodInfo ExecuteMethodInfo_OneParameter;
            
            readonly MethodInfo CanExecuteMethodInfo_NoParameters;
            readonly MethodInfo CanExecuteMethodInfo_OneParameter;

            readonly Type ParameterType;

            readonly PropertyInfo CanExecutePropertyInfo;

            IDisposable CanExecuteNotifyPropertyChangedSubscription;

            public InvokeMethodCommand(
                object targetObject, 
                string executeMethodName, 
                string canExecuteMethodName, 
                string canExecutePropertyName,
                string canExecuteTriggerPropertyName)
            {
                TargetObjectReference = new WeakReference<object>(targetObject, trackResurrection: false);

                ExecuteMethodName = executeMethodName;
                CanExecuteTriggerPropertyName = canExecuteTriggerPropertyName;

                var targetType = targetObject.GetType();

                //# Find Execute Method

                // first try to find execute method which accepts exactly one parameter
                // if this fails, find one without any parameters
                if (!executeMethodName.IsNullOrEmpty())
                {
                    ExecuteMethodInfo_OneParameter =
                        (from m in targetType.GetMethods()
                         where m.GetParameters().Length == 1 && m.Name == executeMethodName
                         select m).FirstOrDefault();

                    if (ExecuteMethodInfo_OneParameter != null)
                    {
                        ParameterType = ExecuteMethodInfo_OneParameter.GetParameters().Single().ParameterType;
                    }
                    
                    ExecuteMethodInfo_NoParameters =
                            (from m in targetType.GetMethods()
                             where m.GetParameters().Length == 0 && m.Name == executeMethodName
                             select m).FirstOrDefault();
                }

                if(ExecuteMethodInfo_OneParameter == null && ExecuteMethodInfo_NoParameters == null)
                {
                    InternalTrace.Error("Binding error: Cannot find public method {0} on type {1}. Method should be public and accept single or no parameters.".FormatWith(ExecuteMethodName, targetType.FullName));
                }

                //# Find Can Execute Method
                if (!canExecuteMethodName.IsNullOrEmpty())
                {
                    // first try to find can execute method which accepts exactly one parameter
                    // if this fails, find one without any parameters
                    CanExecuteMethodInfo_OneParameter =
                        (from m in targetType.GetMethods()
                         where m.GetParameters().Length == 1 && m.Name == canExecuteMethodName
                         select m).FirstOrDefault();

                    CanExecuteMethodInfo_NoParameters =
                    (from m in targetType.GetMethods()
                     where m.GetParameters().Length == 0 && m.Name == canExecuteMethodName
                     select m).FirstOrDefault();
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
                        (targetObject as INotifyPropertyChanged).CreateWeakEventHandler()
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
                var targetObject = (object)null;

                if (!TargetObjectReference.TryGetTarget(out targetObject))
                    return false;

                if(parameter == null)
                {
                    //# parameter is null
                    //  if can execute accepts reference type as parameter -> execute it passing null
                    //  NOTE:   if can execute accepts value type parameter, than we should not just pass null to it
                    //          since it would be internally converted to a default value (see http://msdn.microsoft.com/en-us/library/a89hcwhh(v=vs.110).aspx)
                    //          In that case we will just call can execute without parameters or get value of can execute property

                    if(CanExecuteMethodInfo_OneParameter != null 
                        && !ParameterType.IsValueType)
                    {
                        return (bool)CanExecuteMethodInfo_OneParameter.Invoke(targetObject, new[] { parameter });
                    }

                    if(CanExecuteMethodInfo_NoParameters != null)
                        return (bool)CanExecuteMethodInfo_NoParameters.Invoke(targetObject, null);

                    if(CanExecutePropertyInfo != null)
                        return (bool)CanExecutePropertyInfo.GetValue(targetObject);
                }
                else
                {
                    //# parameter has value
                    //  pass it to can execute method

                    if (CanExecuteMethodInfo_OneParameter != null)
                    {
                        //  make sure to convert to compatible type first
                        var compatibleParameterValue = parameter.Convert(ParameterType);

                        return (bool)CanExecuteMethodInfo_OneParameter.Invoke(targetObject, new[] { compatibleParameterValue });
                    }

                    // we can get here if xaml specifies command parameter, but can execute does not exept it
                    // todo: log information

                    if(CanExecuteMethodInfo_NoParameters != null)
                        return (bool)CanExecuteMethodInfo_NoParameters.Invoke(targetObject, null);

                    if(CanExecutePropertyInfo != null)
                        return (bool)CanExecutePropertyInfo.GetValue(targetObject);
                }

                return true;
            }

            public void Execute(object parameter)
            {
                var targetObject = (object)null;

                if (!TargetObjectReference.TryGetTarget(out targetObject))
                    return;

                if (parameter == null)
                {
                    // if execute accepts reference type as parameter -> execute it passing null
                    // NOTE:    if execute accepts value type parameter, than we should not just pass null to it
                    //          since it would be internally converted to a default value (see http://msdn.microsoft.com/en-us/library/a89hcwhh(v=vs.110).aspx)
                    //          In that case we will just call execute without parameters

                    if (ExecuteMethodInfo_OneParameter != null
                        && !ParameterType.IsValueType)
                    {
                        //  make sure to convert to compatible type first                        
                        // NOTE: but parameter is null, so is casting still needed here?
                        //var compatibleParameterValue = parameter.Convert(ParameterType);

                        ExecuteMethodInfo_OneParameter.Invoke(targetObject, new[] { parameter });
                        return;
                    }

                    if (ExecuteMethodInfo_NoParameters != null)
                    {
                        ExecuteMethodInfo_NoParameters.Invoke(targetObject, null);
                        return;
                    }
                }
                else
                {
                    // parameter has value, pass it to execute method
                    if (ExecuteMethodInfo_OneParameter != null)
                    {
                        //  make sure to convert to compatible type first
                        var compatibleParameterValue = parameter.Convert(ParameterType);

                        ExecuteMethodInfo_OneParameter.Invoke(targetObject, new[] { compatibleParameterValue });
                        return;
                    }

                    // we can get here if xaml specifies command parameter, but execute does not exept it
                    // todo: log warning or information

                    if (ExecuteMethodInfo_NoParameters != null)
                        ExecuteMethodInfo_NoParameters.Invoke(targetObject, null); ;
                }
            }

            ~InvokeMethodCommand()
            {

            }
        }
    }

}
