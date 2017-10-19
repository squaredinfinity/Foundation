using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SquaredInfinity.Disposables
{
    /// <summary>
    /// Base class for a disposable object.
    /// This class does not define finalizer.
    /// Use DisposableObjectWithFinalizer instead where needed.
    /// </summary>
    public abstract class DisposableObject : IDisposable
    {
        /// <summary>
        /// Collection of disposables which will be disposed when this instance disposes.
        /// </summary>
        protected CompositeDisposable Disposables { get; } = new CompositeDisposable();

        // NOTE: this does not need to be volatile because Interlocked.CompareExchange is used when accessed.
        int _isDisposed = 0;
        /// <summary>
        /// True if this instance has been disposed, false otherwise
        /// </summary>
        public bool IsDisposed => _isDisposed == 1;


        public void Dispose()
        {
            // Disposable may be called many times, but should only execute once
            //
            // see: https://msdn.microsoft.com/en-us/library/system.idisposable.dispose.aspx
            // If an object's Dispose method is called more than once, the object must ignore all calls after the first one. 
            // The object must not throw an exception if its Dispose method is called multiple times. 
            // Instance methods other than Dispose can throw an ObjectDisposedException when resources are already disposed.

            if (Interlocked.CompareExchange(ref _isDisposed, 1, 0) == 1)
                return;

            DisposeManagedResources();

            Disposables.Dispose();

            DisposeUnmanagedResources();

            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Disposes managed resources
        /// </summary>
        protected virtual void DisposeManagedResources()
        { }

        /// <summary>
        /// Disposes unmanaged resources
        /// </summary>
        protected virtual void DisposeUnmanagedResources()
        { }

        //  NOTE: 
        //  Finalizer not to be added to this type
        //~DisposableObject() { }

        /// <summary>
        /// Creates a disposable object which invokes specified action when disposed.
        /// The action is guaranteed to run no more than once.
        /// Disposable object will hold a strong reference to the action.
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        public static IDisposable Create(Action action)
        {
            if (action == null)
                throw new ArgumentNullException(nameof(action));

            return new _ActionDisposable(action);
        }

        /// <summary>
        /// Creates a new disposable object which doesn't do anything when disposed.
        /// </summary>
        /// <returns></returns>
        public static IDisposable Create()
        {
            return new _DoNothingDisposable();
        }
    }
}
