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
    public partial class AsyncLock
    {
        class _WriteLockAcquisition : DisposableObject, IWriteLockAcquisition
        {
            AsyncLock Owner;

            public bool IsLockHeld { get; private set; }

            public _WriteLockAcquisition(AsyncLock owner, IDisposable disposeWhenDone)
            {
                IsLockHeld = true;

                this.Owner = owner;

                Disposables.AddIfNotNull(disposeWhenDone);
            }

            protected override void DisposeManagedResources()
            {
                base.DisposeManagedResources();

                Owner.ReleaseLock();
                
                IsLockHeld = false;
            }
        }
    }
}
