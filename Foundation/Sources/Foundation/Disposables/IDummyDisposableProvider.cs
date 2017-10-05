using System;
using System.Collections.Generic;
using System.Text;

namespace SquaredInfinity.Disposables
{
    /// <summary>
    /// Defines a method to create a dummy disposable.
    /// Dummy disposable does nothing when .Dispose() is called.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IDummyDisposableProvider<T>
        where T : IDisposable
    {
        /// <summary>
        /// Returns an instance of this object which implements dummy .Dispose().
        /// Calling .Dispose() on that instance will not result with instance being disposed.
        /// </summary>
        /// <returns></returns>
        T AsDummyDisposable();
    }
}
