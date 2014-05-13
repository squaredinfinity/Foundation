using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Windows;
using SquaredInfinity.Foundation.Extensions;

namespace SquaredInfinity.Foundation
{
    public interface IWeakEventHandler<TSource, TDelegate, TEventArgs> : IDisposable
    {
        IWeakEventHandler<TSource, TDelegate, TEventArgs> Throttle(TimeSpan timespan);
        IDisposable Subscribe(Action<TSource, TEventArgs> onEvent);
    }

    class InstanceWeakEventHandler<TSource, TDelegate, TEventArgs> : IWeakEventHandler<TSource, TDelegate, TEventArgs>
    {
        TDelegate del;

        TSource source;
        Action<TSource, TDelegate> addHandler;
        Action<TSource, TDelegate> removeHandler;
        Action<TSource, TEventArgs> onEvent;

        public InstanceWeakEventHandler(TSource source, Action<TSource, TDelegate> addHandler, Action<TSource, TDelegate> removeHandler)
        {
            this.source = source;
            this.addHandler = addHandler;
            this.removeHandler = removeHandler;
        }

        public IWeakEventHandler<TSource, TDelegate, TEventArgs> Throttle(TimeSpan timespan)
        {
            return this;
        }

        public IDisposable Subscribe(Action<TSource, TEventArgs> onEvent)
        {
            var x = new Action<object, TEventArgs>(OnEvent);

            del = ReflectionUtils.CreateDelegate<TDelegate>(x, x.GetType().GetMethod("Invoke"));

            addHandler(source, del);

            this.onEvent = onEvent;

            return this;
        }
        
        void OnEvent(object sender, TEventArgs args)
        {
            onEvent((TSource)sender, args);
        }

        public void Dispose()
        {
            if(removeHandler != null && del != null)
                removeHandler(source, del);
        }
    }


    internal static class ReflectionUtils
    {
        public static TDelegate CreateDelegate<TDelegate>(object o, MethodInfo method)
        {
            return (TDelegate) (object) Delegate.CreateDelegate(typeof(TDelegate), o, method);
        }

        public static Delegate CreateDelegate(Type delegateType, object o, MethodInfo method)
        {
            return Delegate.CreateDelegate(delegateType, o, method);
        }

        public static void GetEventMethods<TSender, TEventArgs>(
            Type targetType, 
            object target, 
            string eventName, 
            out MethodInfo addMethod, 
            out MethodInfo removeMethod, 
            out Type delegateType)
        {
            EventInfo eventInfo;

            bool isStaticEvent = target == null;

            if (isStaticEvent)
            {
                eventInfo = ReflectionUtils.GetEventInfo(targetType, eventName, isStaticEvent);

                if (eventInfo == null)
                { 
                    throw new InvalidOperationException("Could not find static event {0} on type {1}.".FormatWith(eventName, targetType.FullName));
                }
            }
            else
            {
                eventInfo = ReflectionUtils.GetEventInfo(targetType, eventName, isStaticEvent);

                if (eventInfo == null)
                {
                    throw new InvalidOperationException("Could not find event {0} on type {1}.".FormatWith(eventName, targetType.FullName));
                }
            }

            // try to find add and remove methods

            addMethod = eventInfo.GetAddMethod();
            removeMethod = eventInfo.GetRemoveMethod();

            if (addMethod == null)
            {
                throw new InvalidOperationException("Add method is missing");
            }
            
            if (removeMethod == null)
            {
                throw new InvalidOperationException("Remove method is missing");
            }

            // ensure method parameters
            
            ParameterInfo[] addMethodParameters = addMethod.GetParameters();

            // Add Method should have only one parameter

            if (addMethodParameters.Length != 1)
            {
                throw new InvalidOperationException("Add method should take one parameter");
            }

            // Remove method should have only one parameter

            ParameterInfo[] removeMethodParameters = removeMethod.GetParameters();

            if (removeMethodParameters.Length != 1)
            {
                throw new InvalidOperationException("Remove method should take one parameter");
            }

            // Find delegate type
            // delegate type is passed as a first parameter to event add method
            delegateType = addMethodParameters[0].ParameterType;

            // ensure delegate Invoke metyhod parameters
            MethodInfo delegateInvokeMethod = delegateType.GetMethod("Invoke");

            ParameterInfo[] delegateInvokeParameters = delegateInvokeMethod.GetParameters();

            if (delegateInvokeParameters.Length != 2)
            {
                throw new InvalidOperationException("Event Pattern requires two parameters (sender and args)");
            }
                        
            var senderType = typeof(TSender);

            if (!senderType.IsAssignableFrom(delegateInvokeParameters[0].ParameterType))
            {
                throw new InvalidOperationException("Delegate sender should be of type {0}.".FormatWith(senderType.FullName));
            }

            var eventArgsType = typeof(TEventArgs);

            if (!eventArgsType.IsAssignableFrom(delegateInvokeParameters[1].ParameterType))
            {
                throw new InvalidOperationException("Delegate event args should be of type {0}".FormatWith(eventArgsType.FullName));
            }
        }

        public static EventInfo GetEventInfo(this Type type, string eventName, bool isStatic)
        {
            return type.GetEvent(eventName, isStatic ? BindingFlags.Static | BindingFlags.Public : BindingFlags.Instance | BindingFlags.Public);
        }
    }

    public static class WeakEventExtensions
    {
        public static WeakEventHandlerPrototype<TSource> CreateWeakEventHandler<TSource>(this TSource source)
        {
            return new WeakEventHandlerPrototype<TSource>(source);
        }
    }

    public class WeakEventHandlerPrototype<TSource>
    {
        internal TSource Source;

        public WeakEventHandlerPrototype(TSource source)
        {
            this.Source = source;
        }

        public IWeakEventHandler<TSource, TDelegate, TEventArgs> ForEvent<TDelegate, TEventArgs>(
            Action<TSource, TDelegate> addHandler,
            Action<TSource, TDelegate> removeHandler)
        {
            var x = new InstanceWeakEventHandler<TSource, TDelegate, TEventArgs>(Source, addHandler, removeHandler);

            return x;
        }

        public IWeakEventHandler<TSource, EventHandler<TEventArgs>, TEventArgs> ForEvent<TEventArgs>(
            Action<TSource, EventHandler<TEventArgs>> addHandler,
            Action<TSource, EventHandler<TEventArgs>> removeHandler)
        {
            var x = new InstanceWeakEventHandler<TSource, EventHandler<TEventArgs>, TEventArgs>(Source, addHandler, removeHandler);

            return x;
        }
    }
}
