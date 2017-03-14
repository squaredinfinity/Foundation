using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SquaredInfinity.Threading
{
    public partial class ReaderWriterLockSlimEx
    {        
        class ReadLockAcquisition : IReadLockAcquisition
        {
            ReaderWriterLockSlimEx Owner;
            IDisposable Disposable;

            public ReadLockAcquisition(ReaderWriterLockSlimEx owner, IDisposable disposeWhenDone)
            {
                this.Owner = owner;
                this.Disposable = disposeWhenDone;
            }

            public IWriteLockAcquisition AcquireWriteLock()
            {
                // lock parent first
                Owner.InternalLock.EnterWriteLock();
                // then its children
                var disposables = Owner.LockChildren(LockModes.Write);

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
                var ok = Owner.TryLockChildren(LockModes.Write, timeout, out disposables);

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

            public void Dispose()
            {
                if (Owner.InternalLock.IsReadLockHeld)
                    Owner.InternalLock.ExitReadLock();

                 Disposable?.Dispose();
            }
        }
    }
}
