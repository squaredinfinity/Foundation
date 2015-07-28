using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace SquaredInfinity.Foundation.Extensions
{
    /// <summary>
    /// 
    /// </summary>
    public delegate void PropertyChangedHandler<T>(T sender);

    /// <summary>
    /// Extensions for INotifyPropertyChanged interface.
    /// </summary>
    public static class INotifyPropertyChangedExtensions
    {
        /// <summary>
        /// Notifies the specified event handler.
        /// </summary>
        /// <param name="eventHandler">The event handler.</param>
        /// <param name="expression">The expression.</param>
        public static void Notify(this PropertyChangedEventHandler eventHandler, Expression<Func<object>> expression)
        {
            // if eventHandler is null -> no handlers have been attached to the PropertyChanged event so just retrun.
            if (eventHandler == null)
            {
                return;
            }

            object value = null;
            var name = expression.GetAccessedMemberName(out value);

            foreach (var del in eventHandler.GetInvocationList())
            {
                del.DynamicInvoke(new object[] { value, new PropertyChangedEventArgs(name) });
            }
        }

        /// <summary>
        /// Subscribes to change.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="objectThatNotifies">The object that notifies.</param>
        /// <param name="expression">The expression.</param>
        /// <param name="handler">The handler.</param>
        public static void SubscribeToChange<T>(this T objectThatNotifies, Expression<Func<object>> propertyAccessExpression, PropertyChangedHandler<T> handler)
            where T : INotifyPropertyChanged
        {
            var name = propertyAccessExpression.GetAccessedMemberName();

            objectThatNotifies.PropertyChanged +=
                (s, e) =>
                {
                    if (e.PropertyName.Equals(name))
                    {
                        handler(objectThatNotifies);
                    }
                };
        }

        /// <summary>
        /// Subscribes to change.
        /// </summary>
        /// <param name="objectThatNotifies">The object that notifies.</param>
        /// <param name="expression">The expression.</param>
        /// <param name="handler">The handler.</param>
        public static void SubscribeToChange(this INotifyPropertyChanged objectThatNotifies, Expression<Func<object>> expression, PropertyChangedHandler<INotifyPropertyChanged> handler)
        {
            SubscribeToChange<INotifyPropertyChanged>(objectThatNotifies, expression, handler);
        }

        /// <summary>
        /// Subscribes to change.
        /// </summary>
        /// <param name="objectThatNotifies">The object that notifies.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="handler">The handler.</param>
        public static void SubscribeToChange(this INotifyPropertyChanged objectThatNotifies, string propertyName, PropertyChangedHandler<INotifyPropertyChanged> handler)
        {
            objectThatNotifies.PropertyChanged +=
                (s, e) =>
                {
                    if (e.PropertyName.Equals(propertyName))
                    {
                        handler(objectThatNotifies);
                    }
                };
        }
    }
}
