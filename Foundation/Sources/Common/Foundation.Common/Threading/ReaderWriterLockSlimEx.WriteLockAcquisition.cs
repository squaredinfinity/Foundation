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
        class WriteLockAcquisition : IWriteLockAcquisition
        {
            ReaderWriterLockSlimEx Owner;

            public WriteLockAcquisition(ReaderWriterLockSlimEx owner, bool isSuccesfull)
            {
                this.Owner = owner;
                this.IsSuccesfull = isSuccesfull;
            }

            public void Dispose()
            {
                if (!IsSuccesfull)
                    return;

                if (Owner.InternalLock.IsWriteLockHeld)
                    Owner.InternalLock.ExitWriteLock();
            }

            public bool IsSuccesfull { get; private set; }
        }
    }
}
