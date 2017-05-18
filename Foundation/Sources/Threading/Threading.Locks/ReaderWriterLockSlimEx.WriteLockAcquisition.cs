﻿using SquaredInfinity.Disposables;
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
        class WriteLockAcquisition : DisposableObject, IWriteLockAcquisition
        {
            ReaderWriterLockSlimEx Owner;

            public bool IsLockHeld { get; private set; }

            public WriteLockAcquisition(ReaderWriterLockSlimEx owner, IDisposable disposeWhenDone)
            {
                this.Owner = owner;

                Disposables.Add(disposeWhenDone);

                IsLockHeld = true;
            }

            protected override void DisposeManagedResources()
            {
                base.DisposeManagedResources();

                Owner.InternalLock.ExitWriteLock();

                IsLockHeld = false;
            }
        }
    }
}
