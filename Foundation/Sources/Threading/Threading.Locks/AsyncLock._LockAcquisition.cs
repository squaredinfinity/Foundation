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
        class _LockAcquisition : DisposableObject, ILockAcquisition
        {
            AsyncLock Owner;
            public ICorrelationToken CorrelationToken { get; private set; }

            public bool IsLockHeld { get; private set; }

            public _LockAcquisition(AsyncLock owner, ICorrelationToken correlationToken)
            {
                IsLockHeld = true;

                Owner = owner;
                CorrelationToken = correlationToken;
            }

            protected override void DisposeManagedResources()
            {
                base.DisposeManagedResources();

                Owner.ReleaseLock(CorrelationToken);
                
                IsLockHeld = false;
            }
        }
    }
}
