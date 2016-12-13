using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Windows;
using SquaredInfinity.Foundation.Extensions;
using System.Diagnostics;

namespace SquaredInfinity.Foundation
{
  

    public interface IEventSubscriptionPrototype<TEventSource>
    {
        IEventSubscription<TEventSource, TDelegate, TEventArgs> ForEvent<TDelegate, TEventArgs>(
            Action<TEventSource, TDelegate> addHandler,
            Action<TEventSource, TDelegate> removeHandler);

        IEventSubscription<TEventSource, EventHandler<TEventArgs>, TEventArgs> ForEvent<TEventArgs>(
            Action<TEventSource, EventHandler<TEventArgs>> addHandler,
            Action<TEventSource, EventHandler<TEventArgs>> removeHandler);

        IEventSubscription<TEventSource, EventHandler, EventArgs> ForEvent(
            Action<TEventSource, EventHandler> addHandler,
            Action<TEventSource, EventHandler> removeHandler);
    }

    class EventSubscriptionPrototype<TEventSource> : IEventSubscriptionPrototype<TEventSource>
    {
        TEventSource EventSource;

        public EventSubscriptionPrototype(TEventSource eventSource)
        {
            this.EventSource = eventSource;
        }

        public IEventSubscription<TEventSource, TDelegate, TEventArgs> ForEvent<TDelegate, TEventArgs>(
            Action<TEventSource, TDelegate> addHandler,
            Action<TEventSource, TDelegate> removeHandler)
        {
            var subscription = new EventSubscription<TEventSource, TDelegate, TEventArgs>(EventSource, addHandler, removeHandler);

            return subscription;
        }

        public IEventSubscription<TEventSource, EventHandler<TEventArgs>, TEventArgs> ForEvent<TEventArgs>(
            Action<TEventSource, EventHandler<TEventArgs>> addHandler,
            Action<TEventSource, EventHandler<TEventArgs>> removeHandler)
        {
            var subscription = new EventSubscription<TEventSource, EventHandler<TEventArgs>, TEventArgs>(EventSource, addHandler, removeHandler);

            return subscription;
        }

        public IEventSubscription<TEventSource, EventHandler, EventArgs> ForEvent(
            Action<TEventSource, EventHandler> addHandler,
            Action<TEventSource, EventHandler> removeHandler)
        {
            var subscription = new EventSubscription<TEventSource, EventHandler, EventArgs>(EventSource, addHandler, removeHandler);

            return subscription;
        }
    }

    public interface IEventSubscription<TEventSource, TDelegate, TEventArgs> : IDisposable
    {
        IEventSubscription<TEventSource, TDelegate, TEventArgs> Throttle(TimeSpan min);
        IEventSubscription<TEventSource, TDelegate, TEventArgs> Throttle(TimeSpan min, TimeSpan max);
        IDisposable Subscribe(TDelegate onEvent);
    }

    internal interface IWeakEventHandler<TDelegate>
    {
        TDelegate Handler { get; }
    }

    public class EventSubscription<TEventSource, TDelegate, TEventArgs> : IEventSubscription<TEventSource, TDelegate, TEventArgs>
    {
        IWeakEventHandler<TDelegate> WeakEventHandler;

        TEventSource EventSource;

        Action<TEventSource, TDelegate> AddHandler;
        Action<TEventSource, TDelegate> RemoveHandler;

        object EventSubscriberStrongReference;

        public EventSubscription(TEventSource EventSource, Action<TEventSource, TDelegate> addHandler, Action<TEventSource, TDelegate> removeHandler)
        {
            this.EventSource = EventSource;
            this.AddHandler = addHandler;
            this.RemoveHandler = removeHandler;
        }

        TimeSpan? ThrottleMin = null;
        TimeSpan? ThrottleMax = null;

        public IEventSubscription<TEventSource, TDelegate, TEventArgs> Throttle(TimeSpan min)
        {
            ThrottleMin = min;

            return this;
        }

        public IEventSubscription<TEventSource, TDelegate, TEventArgs> Throttle(TimeSpan min, TimeSpan max)
        {
            ThrottleMin = min;
            ThrottleMax = max;
            return this;
        }

        public IDisposable Subscribe(TDelegate onEvent)
        {
            // cast to Delegate so we can extract Method
            // NOTE: currently where TDelegate : Delegate generic constraint is not supported by C#
            var onEventDelegate = onEvent as Delegate;

            // keep reference to delegate target alive
            // for as long as this subscription is not being collected by Garbage Collector
            EventSubscriberStrongReference = onEventDelegate.Target;

            // Create new instance of Weak Event Handler
            Type TYPE_WeakEventHandler =
                typeof(WeakEventHandler<,,,>)
                .MakeGenericType(
                onEventDelegate.Method.DeclaringType,
                typeof(TEventSource),
                typeof(TDelegate),
                typeof(TEventArgs));

            ConstructorInfo wehConstructor =
                TYPE_WeakEventHandler
                .GetConstructors()
                .First();

            var constructorParameters =
                new object[] { EventSource, onEvent, ThrottleMin, ThrottleMax };

            WeakEventHandler = wehConstructor.Invoke(constructorParameters) as IWeakEventHandler<TDelegate>;

            AddHandler(EventSource, WeakEventHandler.Handler);

            return this;
        }

        #region IDisposable

        ~EventSubscription()
        {
            Dispose();
        }

        public void Dispose()
        {
            RemoveHandler(EventSource, WeakEventHandler.Handler);

            GC.SuppressFinalize(this);
        }

        #endregion
    }

    /// <summary>
    /// Provides a layer between Event Subscriber and Event Provider.
    /// Normally using EventProvider.Event += EventSubscriber.EventHandler; would create a strong reference from EventProvider to EventSubscriber.
    /// This strong reference would prevent EventSubscriber from Garbage Collection.
    /// 
    /// WeakEventHandler holds a Weak-Reference to EventSubscriber allowing it to be Garbage Collected when needed.
    /// </summary>
    /// <typeparam name="TEventSubscriber"></typeparam>
    /// <typeparam name="TEventSource"></typeparam>
    /// <typeparam name="TDelegate"></typeparam>
    /// <typeparam name="TEventArgs"></typeparam>
    class WeakEventHandler<TEventSubscriber, TEventSource, TDelegate, TEventArgs> : IWeakEventHandler<TDelegate>
        where TEventSubscriber : class
        where TEventSource : class
        where TEventArgs : EventArgs
    {
        delegate void OpenEventHandler(TEventSubscriber eventSubscriber, object sender, TEventArgs e);

        OpenEventHandler OpenHandler;

        public TDelegate Handler { get; private set; }

        WeakReference<TEventSubscriber> EventSubscriber_ref;

        InvocationThrottle Throttle = null;

        public WeakEventHandler(
            TEventSource eventSource,
            TDelegate onEvent,
            TimeSpan? throttleMin,
            TimeSpan? throttleMax)
        {
            if (throttleMin != null)
            {
                if (throttleMax != null)
                    Throttle = new InvocationThrottle(throttleMin.Value, throttleMax.Value);
                else
                    Throttle = new InvocationThrottle(throttleMin.Value);

                Initialize(onEvent, "InvokeWithThrottle");
            }
            else
            {
                Initialize(onEvent, "Invoke");
            }
        }

        void Initialize(TDelegate onEvent, string invokeMethodName)
        {
            Handler =
                ReflectionUtils
                .CreateDelegate<TDelegate>(this, typeof(WeakEventHandler<TEventSubscriber, TEventSource, TDelegate, TEventArgs>)
                .GetMethod(invokeMethodName));

            var onEventDelegate = onEvent as Delegate;

            OpenHandler = (OpenEventHandler)Delegate.CreateDelegate(typeof(OpenEventHandler), null, onEventDelegate.Method);

            EventSubscriber_ref = new WeakReference<TEventSubscriber>(onEventDelegate.Target as TEventSubscriber);
        }


        public void InvokeWithThrottle(object sender, TEventArgs e)
        {
            Throttle.InvokeAsync(() => Invoke(sender, e));
        }

        public void Invoke(object sender, TEventArgs e)
        {
            if (OpenHandler == null)
                return;

            var eventSubscriber = (TEventSubscriber)null;

            if (EventSubscriber_ref.TryGetTarget(out eventSubscriber))
            {
                OpenHandler(eventSubscriber, sender, e);
            }
            else
            {
                OpenHandler = null;
            }
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
                    throw new InvalidOperationException($"Could not find static event {eventName} on type {targetType.FullName}.");
                }
            }
            else
            {
                eventInfo = ReflectionUtils.GetEventInfo(targetType, eventName, isStaticEvent);

                if (eventInfo == null)
                {
                    throw new InvalidOperationException($"Could not find event {eventName} on type {targetType.FullName}.");
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
                throw new InvalidOperationException($"Delegate sender should be of type {senderType.FullName}.");
            }

            var eventArgsType = typeof(TEventArgs);

            if (!eventArgsType.IsAssignableFrom(delegateInvokeParameters[1].ParameterType))
            {
                throw new InvalidOperationException($"Delegate event args should be of type {eventArgsType.FullName}");
            }
        }

        public static EventInfo GetEventInfo(this Type type, string eventName, bool isStatic)
        {
            return type.GetEvent(eventName, isStatic ? BindingFlags.Static | BindingFlags.Public : BindingFlags.Instance | BindingFlags.Public);
        }
    }
}
