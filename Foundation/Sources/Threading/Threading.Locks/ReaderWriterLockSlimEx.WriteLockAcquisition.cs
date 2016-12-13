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
            IDisposable Disposable;

            public WriteLockAcquisition(ReaderWriterLockSlimEx owner, IDisposable disposeWhenDone)
            {
                this.Owner = owner;
                this.Disposable = disposeWhenDone;
            }

            public void Dispose()
            {
                if (Owner.InternalLock.IsWriteLockHeld)
                    Owner.InternalLock.ExitWriteLock();

                Disposable?.Dispose();
            }
        }
    }
}
