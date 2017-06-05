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
            volatile ICorrelationToken CorrelationToken;

            public bool IsLockHeld { get; private set; }

            public _ReadLockAcquisition(AsyncReaderWriterLock owner, ICorrelationToken correlationToken, IDisposable disposeWhenDone = null)
            {
                IsLockHeld = true;

                Owner = owner;
                CorrelationToken = correlationToken;

                Disposables.AddIfNotNull(disposeWhenDone);
            }

            protected override void DisposeManagedResources()
            {
                base.DisposeManagedResources();

                Owner.ReleaseReadLock(CorrelationToken);
                
                IsLockHeld = false;
            }
        }
    }
}
