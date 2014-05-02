using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SquaredInfinity.Foundation.Threading
{
    public partial class ReaderWriterLockSlimEx
    {        class ReadLockAcquisition : IReadLockAcquisition
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

            public void Dispose()
            {
                if (Owner.InternalLock.IsUpgradeableReadLockHeld)
                    Owner.InternalLock.ExitUpgradeableReadLock();

                if (Owner.InternalLock.IsReadLockHeld)
                    Owner.InternalLock.ExitReadLock();
            }
        }
    }
}
