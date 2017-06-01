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
    public partial class SupportsBulkUpdate
    {
        #region BeginBulkUpdate

        public IBulkUpdate BeginBulkUpdate() 
            => BeginBulkUpdate(SyncOptions.Default);
        public IBulkUpdate BeginBulkUpdate(CancellationToken ct) 
            => BeginBulkUpdate(new SyncOptions(ct));
        public IBulkUpdate BeginBulkUpdate(TimeSpan timeout)
            => BeginBulkUpdate(new SyncOptions(timeout));
        public IBulkUpdate BeginBulkUpdate(TimeSpan timeout, CancellationToken ct) 
            => BeginBulkUpdate(new SyncOptions(timeout, ct));
        public IBulkUpdate BeginBulkUpdate(int millisecondsTimeout)
            => BeginBulkUpdate(new SyncOptions(millisecondsTimeout));
        public IBulkUpdate BeginBulkUpdate(int millisecondsTimeout, CancellationToken ct)
            => BeginBulkUpdate(new SyncOptions(millisecondsTimeout, ct));

        public IBulkUpdate BeginBulkUpdate(SyncOptions options)
        {
            var la = (ILockAcquisition)null;

            if (Lock != null)
            {
                la = Lock.AcquireWriteLock(options);

                if (!la.IsLockHeld)
                {
                    la.Dispose();

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
