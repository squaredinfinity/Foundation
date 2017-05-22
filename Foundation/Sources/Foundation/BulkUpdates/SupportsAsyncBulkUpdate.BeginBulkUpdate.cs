using SquaredInfinity.ComponentModel;
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
    public partial class SupportsAsyncBulkUpdate
    {
        #region BeginBulkUpdate

        public IBulkUpdate BeginBulkUpdate() => BeginBulkUpdate(SyncOptions.Default);

        public IBulkUpdate BeginBulkUpdate(SyncOptions options)
        {
            var la = (ILockAcquisition)null;

            if (Lock != null)
            {
                la = Lock.AcquireWriteLock(options);

                if (!la.IsLockHeld)
                {
                    // failed to acquire lock
                    return new _FailedBulkUpdate();
                }
            }

            Interlocked.Increment(ref BulkUpdateCount);

            OnBeginBulkUpdate();

            return new _BulkUpdate(la, DisposableObject.Create(() =>
            {
                EndBulkUpdate();
            }));
        }

        #endregion
    }
}
