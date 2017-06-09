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
        class _LockAcquisition : DisposableObject, ILockAcquisition
        {
            AsyncReaderWriterLock Owner;
            LockType LockType;
            public ICorrelationToken CorrelationToken { get; private set; }

            public bool IsLockHeld { get; private set; }

            public _LockAcquisition(AsyncReaderWriterLock owner, LockType lockType, ICorrelationToken correlationToken)
            {
                Owner = owner ?? throw new ArgumentNullException(nameof(owner));
                CorrelationToken = correlationToken ?? throw new ArgumentNullException(nameof(correlationToken));

                if (lockType != LockType.Read && lockType != LockType.Write)
                    throw new NotSupportedException(lockType.ToString());

                LockType = lockType;

                IsLockHeld = true;
            }

            protected override void DisposeManagedResources()
            {
                base.DisposeManagedResources();

                switch (LockType)
                {
                    case LockType.Read:
                        Owner.ReleaseReadLock(CorrelationToken);
                        break;
                    case LockType.Write:
                        Owner.ReleaseWriteLock(CorrelationToken);
                        break;
                    case LockType.UpgradeableRead:
                    default:
                        throw new NotSupportedException(LockType.ToString());
                }

                IsLockHeld = false;
            }
        }
    }
}
