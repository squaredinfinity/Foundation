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
        class UpgradeableReadLockAcquisition : IUpgradeableReadLockAcquisition
        {
            ReaderWriterLockSlimEx Owner;
            IDisposable Disposable;

            public UpgradeableReadLockAcquisition(ReaderWriterLockSlimEx owner, IDisposable disposeWhenDone)
            {
                this.Owner = owner;
                this.Disposable = disposeWhenDone;
            }

            public IReadLockAcquisition DowngradeToReadLock()
            {
                Owner.InternalLock.EnterReadLock();

                var disposables = Owner.LockChildren(LockModes.Read);

                return new ReadLockAcquisition(Owner, disposables);
            }

            public bool TryDowngradeToReadLock(TimeSpan timeout, out IReadLockAcquisition readLockAcquisition)
            {
                var disposables = (IDisposable)null;

                var ok = Owner.TryLockChildren(LockModes.Read, timeout, out disposables);

                if (ok)
                {
                    ok = Owner.InternalLock.TryEnterReadLock(timeout.Milliseconds);

                    if (ok)
                    {
                        readLockAcquisition = new ReadLockAcquisition(Owner, disposables);

                        return true;
                    }
                    else
                    {
                        disposables?.Dispose();
                    }
                }

                readLockAcquisition = null;
                return false;
            }

            public IWriteLockAcquisition UpgradeToWriteLock()
            {
                // lock parent first
                Owner.InternalLock.EnterWriteLock();
                // then its children
                var disposables = Owner.LockChildren(LockModes.Write);

                return new WriteLockAcquisition(Owner, disposables);
            }

            public bool TryUpgradeToWriteLock(TimeSpan timeout, out IWriteLockAcquisition writeLockAcquisition)
            {
                throw new NotImplementedException();
            }


            public void Dispose()
            {
                if (Owner.InternalLock.IsUpgradeableReadLockHeld)
                    Owner.InternalLock.ExitUpgradeableReadLock();

                Disposable?.Dispose();
            }
        }
    }
}
