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

            public ReadLockAcquisition(ReaderWriterLockSlimEx owner, bool isSuccesfull)
            {
                this.Owner = owner;
                this.IsSuccesfull = isSuccesfull;
            }

            public IWriteLockAcquisition AcquireWriteLock()
            {
                if (!IsSuccesfull)
                    throw new InvalidOperationException("Cannot acquire Write Lock using unsuccesfull Read Lock Acquisition.");

                Owner.InternalLock.EnterWriteLock();

                return new WriteLockAcquisition(Owner, isSuccesfull: true);
            }

            public IWriteLockAcquisition TryAcquireWriteLock(TimeSpan timeout)
            {
                if (!IsSuccesfull)
                    throw new InvalidOperationException("Cannot acquire Write Lock using unsuccesfull Read Lock Acquisition.");

                if (Owner.InternalLock.RecursionPolicy == LockRecursionPolicy.NoRecursion && 
                    (Owner.InternalLock.IsReadLockHeld || Owner.InternalLock.IsUpgradeableReadLockHeld || Owner.InternalLock.IsWriteLockHeld))
                {
                    return new WriteLockAcquisition(Owner, isSuccesfull: false);
                }

                if (Owner.InternalLock.RecursionPolicy == LockRecursionPolicy.SupportsRecursion && 
                    !(Owner.InternalLock.IsUpgradeableReadLockHeld || Owner.InternalLock.IsWriteLockHeld))
                {
                    return new WriteLockAcquisition(Owner, isSuccesfull: false);
                }

                var ok = Owner.InternalLock.TryEnterWriteLock(timeout);

                return new WriteLockAcquisition(Owner, isSuccesfull: ok);
            }

            public void Dispose()
            {
                if (!IsSuccesfull)
                    return;

                if (Owner.InternalLock.IsReadLockHeld)
                    Owner.InternalLock.ExitReadLock();
            }

            public bool IsSuccesfull { get; private set; }
        }
    }
}
