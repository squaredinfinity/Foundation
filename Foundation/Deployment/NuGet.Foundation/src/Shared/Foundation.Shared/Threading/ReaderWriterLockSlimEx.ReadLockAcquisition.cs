using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SquaredInfinity.Foundation.Threading
{
    public partial class ReaderWriterLockSlimEx
    {        
        class ReadLockAcquisition : IReadLockAcquisition
        {
            ReaderWriterLockSlimEx Owner;

            public ReadLockAcquisition(ReaderWriterLockSlimEx owner)
            {
                this.Owner = owner;
            }

            public IWriteLockAcquisition AcquireWriteLock()
            {
                Owner.InternalLock.EnterWriteLock();

                return new WriteLockAcquisition(Owner);
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

                var ok = Owner.InternalLock.TryEnterWriteLock(timeout);

                writeLockAcquisition = new WriteLockAcquisition(Owner);
                return true;
            }

            public void Dispose()
            {
                if (Owner.InternalLock.IsReadLockHeld)
                    Owner.InternalLock.ExitReadLock();
            }
        }
    }
}
