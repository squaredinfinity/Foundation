using SquaredInfinity.Disposables;
using SquaredInfinity.Threading;
using SquaredInfinity.Threading.Locks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SquaredInfinity
{
    class _BulkUpdate : DisposableObject, IBulkUpdate
    {
        readonly ILockAcquisition LockAcquisition;

        volatile bool _hasStarted = true;
        public bool HasStarted => _hasStarted;

        public _BulkUpdate(ILockAcquisition lockAcquisition, IDisposable disposeWhenFinished)
        {
            // Note:    Lock acquisition stored separately
            //          to guarantee that it will be disposed last

            Disposables.Add(disposeWhenFinished);

            LockAcquisition = lockAcquisition;
        }

        protected override void DisposeManagedResources()
        {
            base.DisposeManagedResources();

            _hasStarted = false;

            LockAcquisition?.Dispose();
        }
    }
}
