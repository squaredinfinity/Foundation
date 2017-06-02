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
        class _ReadLockAcquisition : DisposableObject, IReadLockAcquisition
        {
            AsyncReaderWriterLock Owner;
            volatile int OwnerThreadId;

            public bool IsLockHeld { get; private set; }

            public _ReadLockAcquisition(AsyncReaderWriterLock owner, int ownerThreadId, IDisposable disposeWhenDone = null)
            {
                IsLockHeld = true;

                Owner = owner;
                OwnerThreadId = ownerThreadId;

                Disposables.AddIfNotNull(disposeWhenDone);
            }

            protected override void DisposeManagedResources()
            {
                base.DisposeManagedResources();

                Owner.ReleaseReadLock(OwnerThreadId);
                
                IsLockHeld = false;
            }
        }
    }
}
