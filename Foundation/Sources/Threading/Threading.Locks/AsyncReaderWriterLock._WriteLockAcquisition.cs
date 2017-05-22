using SquaredInfinity.Disposables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SquaredInfinity.Extensions;

namespace SquaredInfinity.Threading.Locks
{
    public partial class AsyncReaderWriterLock
    {
        class _WriteLockAcquisition : DisposableObject, IReadLockAcquisition
        {
            AsyncReaderWriterLock Owner;

            public bool IsLockHeld { get; private set; }

            public _WriteLockAcquisition(AsyncReaderWriterLock owner, IDisposable disposeWhenDone)
            {
                IsLockHeld = true;

                this.Owner = owner;

                Disposables.AddIfNotNull(disposeWhenDone);
            }

            protected override void DisposeManagedResources()
            {
                base.DisposeManagedResources();


                // TODO


                IsLockHeld = false;
            }
        }
    }
}
