using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace SquaredInfinity.Foundation.Presentation
{
    public static class WeakEvent
    {
        public static ForTarget<T> For<T>(this T obj)
        {
            var fi = new ForTarget<T>();
            fi.SourceInstance = obj;
            fi.OwnerType = typeof(T);

            return fi;
        }

        public class ForTarget<T>
        {
            public T SourceInstance { get; internal set; }
            public Type OwnerType { get; internal set; }

            public void AddHandler<TEventArgs>(string eventName, EventHandler<TEventArgs> handler)
                where TEventArgs : global::System.EventArgs
            {
                //! WeakEventManager will fail if type passed is an interface which implements another interface in which the event is defined
                //! e.g. class C : interface I1 : interface I2
                //! Interface is defined in I2 but SourceInstance is of type I2 (i.e. not a concrete type)
                //! By getting actual type (concrete) we can guarantee that event will be resovled

                if (OwnerType.IsInterface && OwnerType.GetEvent(eventName) == null)
                {
                    var concreteType = SourceInstance.GetType();

                    var weakEventManagerType = typeof(WeakEventManager<,>);
                    var genericWeakEventManagerType = weakEventManagerType.MakeGenericType(concreteType, typeof(TEventArgs));

                    var addHandlerMethod = genericWeakEventManagerType.GetMethod("AddHandler");

                    addHandlerMethod.Invoke(null, new object[] { SourceInstance, eventName, handler });                    
                }
                else
                {
                    WeakEventManager<T, TEventArgs>.AddHandler(SourceInstance, eventName, handler);
                }
            }
        }
    }
}
