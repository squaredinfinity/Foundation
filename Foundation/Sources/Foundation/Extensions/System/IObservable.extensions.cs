using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Extensions
{
    public static class IObservableExtensions
    {
        /// <summary>
        /// Creates a weak subscription.
        /// Unlike default Subscribe method, this will not keep the target alive (unless it is included in clousure of onNext method)
        /// </summary>
        /// <typeparam name="TItem"></typeparam>
        /// <typeparam name="TTarget"></typeparam>
        /// <param name="observable"></param>
        /// <param name="target">Target of the subscription. When target is collected the subscription will be disposed.</param>
        /// <param name="onNext"> </param>
        /// <returns></returns>
        public static IDisposable WeakSubscribe<TItem, TTarget>(this IObservable<TItem> observable, TTarget target, Action<TTarget, TItem> onNext) where TTarget : class
        {
            // if onNext is an instance method then there's a possiblity it will hold reference to the target (if it's instance on the target object)
            // just to be sure and avoid confusion all instance methods are disallowed
            // only explicitly static methods or methods with static implementation (e.g. anonymous lambdas) are allowed.
            // note that user of this method must take extra care not to create a closure over target itself

            if (onNext.Target != null && object.ReferenceEquals(onNext.Target, target))
                throw new ArgumentException("onNext action cannot be an instance method on target. Use static method or lambda expression (t,i) => t.method()");

            var weak_observer = new WeakObserver<TItem, TTarget>(onNext);
            weak_observer.TargetReference = new WeakReference(target);

            weak_observer.Subscription = observable.Subscribe(weak_observer);

            return weak_observer.Subscription;
        }

        class WeakObserver<TItem, TTarget> : IObserver<TItem>
        {
            internal WeakReference TargetReference { get; set; }
            internal IDisposable Subscription { get; set; }

            Action<TTarget, TItem> DoOnNext { get; set; }

            public WeakObserver(Action<TTarget, TItem> onNext)
            {
                DoOnNext = onNext;
            }

            public void OnCompleted()
            { }

            public void OnError(Exception error)
            { }

            public void OnNext(TItem item)
            {
                if (Subscription == null)
                    return;

                var currentTarget = (TTarget)TargetReference.Target;

                if (currentTarget != null)
                    DoOnNext(currentTarget, item);
                else
                {
                    // target reference is gone, clean up
                    Subscription?.Dispose();
                }
            }
        }
    }
}
