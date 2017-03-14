using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Threading
{
    public class WeakDelegate<TDelegate>
    //where TDelegate : class
    {
        bool IsStatic;
        MethodInfo ActionMethod;

        WeakReference<object> TargetReference;
        WeakReference<object> OwnerReference;

        object TargetInstance;


        public WeakDelegate(object owner, TDelegate action, bool keepTargetAlive = false)
        {
            var actionDelegate = (action as Delegate);
            this.ActionMethod = actionDelegate.GetMethodInfo();
            this.IsStatic = ActionMethod.IsStatic;

            if (actionDelegate.Target != null)
            {
                this.TargetReference = new WeakReference<object>(actionDelegate.Target, trackResurrection: false);

                if (keepTargetAlive)
                {
                    this.TargetInstance = actionDelegate.Target;
                }
            }

            if (owner != null)
                this.OwnerReference = new WeakReference<object>(owner, trackResurrection: false);
        }

        static readonly object[] EmptyParameters = new object[] { };

        public bool TryExecute()
        {
            var result = (object)null;

            return TryExecute(out result, EmptyParameters);
        }

        public bool TryExecute(object[] parameters)
        {
            var result = (object)null;

            return TryExecute(out result, parameters);
        }

        public bool TryExecute(out object result)
        {
            return TryExecute(out result, EmptyParameters);
        }

        public bool TryExecute(out object result, object[] parameters)
        {
            result = null;

            if (IsDisposed)
                return false;

            var target = (object)null;

            var owner = (object)null;

            if (!OwnerReference.TryGetTarget(out owner))
            {
                // Owner instance is gone
                // This Action should no longer execute
                return false;
            }

            if (IsStatic)
            {
                result = ActionMethod.Invoke(null, parameters);
                return true;
            }
            else
            {
                if (!TargetReference.TryGetTarget(out target))
                {
                    // Action delegate points to an instance method
                    // but the instance is gone
                    // This Action should no longer execute
                    return false;
                }

                result = ActionMethod.Invoke(target, parameters);

                return true;
            }
        }

        #region IDisposable

        bool IsDisposed = false;

        public void Dispose()
        {
            Dispose(disposing: true);

            GC.SuppressFinalize(this);
        }

        ~WeakDelegate()
        {
            Dispose(disposing: false);
        }

        protected void Dispose(bool disposing)
        {
            TargetInstance = null;
            IsDisposed = true;
        }

        #endregion
    }
}
