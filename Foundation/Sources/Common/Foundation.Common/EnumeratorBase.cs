using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Foundation
{
    public abstract class EnumeratorBase<T> : IEnumerator<T>, IDisposable
    {
        T _current;
        public T Current
        {
            get { return _current; }
            protected set { _current = value; }
        }

        object System.Collections.IEnumerator.Current
        {
            get { return _current; }
        }

        public abstract bool MoveNext();

        public abstract void Reset();


        bool IsDisposed = false;

        public void Dispose()
        {
            Dispose(disposing: true);

            GC.SuppressFinalize(this);
        }

        ~EnumeratorBase()
        {
            Dispose(disposing: false);
        }

        protected void Dispose(bool disposing)
        {
            if (IsDisposed)
                return;

            if (disposing)
            {
                DisposeManagedResources();
            }

            DisposeUnmanagedResources();

            IsDisposed = true;
        }

        protected virtual void DisposeManagedResources()
        { }

        protected virtual void DisposeUnmanagedResources()
        { }
    }
}
