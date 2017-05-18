using SquaredInfinity.Disposables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SquaredInfinity.Threading.Locks
{
    public partial class ReaderWriterLockSlimEx
    {        
        class ReadLockAcquisition : DisposableObject, IReadLockAcquisition
        {
            ReaderWriterLockSlimEx Owner;

            public ReadLockAcquisition(ReaderWriterLockSlimEx owner, IDisposable disposeWhenDone)
            {
                this.Owner = owner;
                Disposables.Add(disposeWhenDone);
            }

            public IWriteLockAcquisition AcquireWriteLock()
            {
                // lock parent first
                Owner.InternalLock.EnterWriteLock();
                // then its children
                var disposables = Owner.LockChildren(LockType.Write);

                return new WriteLockAcquisition(Owner, disposables);
            }

            public bool TryAcquireWriteLock(TimeSpan timeout, out IWriteLockAcquisition writeLockAcquisition)
            {
                if (Owner.InternalLock.RecursionPolicy == LockRecursionPolicy.NoRecursion &&
                    (Owner.InternalLock.IsReadLockHeld || Owner.InternalLock.IsUpgradeableReadLockHeld || Owner.InternalLock.IsWriteLockHeld))
                {
                    writeLockAcquisition = null;
                    return false;
                }

                if (Owner.InternalLock.RecursionPolicy == LockRecursionPolicy.SupportsRecursion &&
                    !(Owner.InternalLock.IsUpgradeableReadLockHeld || Owner.InternalLock.IsWriteLockHeld))
                {
                    writeLockAcquisition = null;
                    return false;
                }

                var disposables = (IDisposable)null;
                var ok = Owner.TryLockChildren(LockType.Write, timeout, out disposables);

                if (ok)
                {
                    ok = Owner.InternalLock.TryEnterWriteLock(timeout);

                    if (ok)
                    {
                        writeLockAcquisition = new WriteLockAcquisition(Owner, disposables);
                        return true;
                    }
                    else
                    {
                        disposables?.Dispose();
                    }
                }

                writeLockAcquisition = null;
                return false;
            }

            protected override void DisposeManagedResources()
            {
                base.DisposeManagedResources();

                Owner.InternalLock.ExitReadLock();
            }
        }
    }
}
