using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SquaredInfinity.Disposables
{
    /// <summary>
    /// Action-based disposbable which holds a reference to an instance
    /// </summary>
    public class ReferenceDisposable<TRef> : IDisposable
    {
        volatile Action<TRef> DoDispose;
        TRef Reference;

        public bool IsDisposed => DoDispose == null;

        public ReferenceDisposable(TRef obj, Action<TRef> dispose)
        {
            Reference = obj;
            DoDispose = dispose;
        }

        public void Dispose()
        { 
            var action = Interlocked.Exchange<Action<TRef>>(ref this.DoDispose, (Action<TRef>)null);

            if (action == null)
                return;

            action(Reference);
        }

        public static IDisposable Create<T>(T obj, Action<T> dispose)
        {
            return new ReferenceDisposable<T>(obj, dispose);
        }
    }
}
