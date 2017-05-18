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
        class UpgradeableReadLockAcquisition : DisposableObject, IUpgradeableReadLockAcquisition
        {
            ReaderWriterLockSlimEx Owner;

            bool IsUpdgradedToWrite = false;            

            public UpgradeableReadLockAcquisition(ReaderWriterLockSlimEx owner, IDisposable disposeWhenDone)
            {
                this.Owner = owner;
                Disposables.Add(disposeWhenDone);
            }

            public IReadLockAcquisition DowngradeToReadLock()
            {
                Owner.InternalLock.EnterReadLock();

                var disposables = Owner.LockChildren(LockType.Read);

                return new ReadLockAcquisition(Owner, disposables);
            }

            public bool TryDowngradeToReadLock(TimeSpan timeout, out IReadLockAcquisition readLockAcquisition)
            {
                var disposables = (IDisposable)null;

                var ok = Owner.TryLockChildren(LockType.Read, timeout, out disposables);

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

                IsUpdgradedToWrite = true;
                
                // then its children
                var disposables = Owner.LockChildren(LockType.Write);

                return new WriteLockAcquisition(Owner, disposables);
            }

            public bool TryUpgradeToWriteLock(TimeSpan timeout, out IWriteLockAcquisition writeLockAcquisition)
            {
                throw new NotImplementedException();
            }

            protected override void DisposeManagedResources()
            {
                base.DisposeManagedResources();
            
                if (IsUpdgradedToWrite)
                {
                    if(Owner.InternalLock.IsUpgradeableReadLockHeld)
                        Owner.InternalLock.ExitUpgradeableReadLock();
                }
                else
                {
                    Owner.InternalLock.ExitUpgradeableReadLock();
                }
            }
        }
    }
}
