using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SquaredInfinity.Disposables
{
    public class _ActionDisposable : DisposableObject, IDisposable
    {
        // NOTE: this does not have to be volatile as Interlocked.Exchange will always be used to access it
        Action DisposeAction;

        /// <summary>
        /// Constructs a new disposable with the given action used for disposal.
        /// </summary>
        /// <param name="dispose">Disposal action which will be run upon calling Dispose.</param>
        public _ActionDisposable(Action disposeAction)
        {
            this.DisposeAction = disposeAction;
        }

        protected override void DisposeManagedResources()
        {
            var f = Interlocked.Exchange(ref DisposeAction, null);

            f?.Invoke();

            base.DisposeManagedResources();
        }
    }
}
