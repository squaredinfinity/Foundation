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
        class UpgradeableReadLockAcquisition : IReadLockAcquisition
        {
            ReaderWriterLockSlimEx Owner;

            public UpgradeableReadLockAcquisition(ReaderWriterLockSlimEx owner)
            {
                this.Owner = owner;
            }

            public IWriteLockAcquisition AcquireWriteLock()
            {
                Owner.InternalLock.EnterWriteLock();

                return new WriteLockAcquisition(Owner);
            }

            public void Dispose()
            {
                if (Owner.InternalLock.IsUpgradeableReadLockHeld)
                    Owner.InternalLock.ExitUpgradeableReadLock();
            }
        }
    }
}
