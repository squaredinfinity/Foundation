using SquaredInfinity.Disposables;
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
        class WriteLockAcquisition : DisposableObject, IWriteLockAcquisition
        {
            ReaderWriterLockSlimEx Owner;

            public WriteLockAcquisition(ReaderWriterLockSlimEx owner, IDisposable disposeWhenDone)
            {
                this.Owner = owner;
                Disposables.Add(disposeWhenDone);
            }

            protected override void DisposeManagedResources()
            {
                base.DisposeManagedResources();

                Owner.InternalLock.ExitWriteLock();
            }
        }
    }
}
