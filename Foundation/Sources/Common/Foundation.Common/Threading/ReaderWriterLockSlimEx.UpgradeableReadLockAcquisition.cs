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
        class UpgradeableReadLockAcquisition : IUpgradeableReadLockAcquisition
        {
            ReaderWriterLockSlimEx Owner;

            public UpgradeableReadLockAcquisition(ReaderWriterLockSlimEx owner)
            {
                this.Owner = owner;
            }

            public IReadLockAcquisition DowngradeToReadLock()
            {
                Owner.InternalLock.EnterReadLock();

                return new ReadLockAcquisition(Owner);
            }

            public bool TryDowngradeToReadLock(TimeSpan timeout, out IReadLockAcquisition readLockAcquisition)
            {
                if(Owner.InternalLock.TryEnterReadLock(timeout.Milliseconds))
                {
                    readLockAcquisition = new ReadLockAcquisition(Owner);

                    return true;
                }
                else
                {
                    readLockAcquisition = null;
                    return false;
                }
            }

            public IWriteLockAcquisition UpgradeToWriteLock()
            {
                Owner.InternalLock.EnterWriteLock();

                Owner.InternalLock.ExitUpgradeableReadLock();

                return new WriteLockAcquisition(Owner);
            }

            public bool TryUpgradeToWriteLock(TimeSpan timeout, out IWriteLockAcquisition writeLockAcquisition)
            {
                throw new NotImplementedException();
            }


            public void Dispose()
            {
                if (Owner.InternalLock.IsUpgradeableReadLockHeld)
                    Owner.InternalLock.ExitUpgradeableReadLock();
            }
        }
    }
}
