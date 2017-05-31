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
        #region BeginBulkUpdateAsync

        public async Task<IBulkUpdate> BeginBulkUpdateAsync() 
            => await BeginBulkUpdateAsync(AsyncOptions.Default);
        public async Task<IBulkUpdate> BeginBulkUpdateAsync(CancellationToken ct) 
            => await BeginBulkUpdateAsync(new AsyncOptions(ct));
        public async Task<IBulkUpdate> BeginBulkUpdateAsync(TimeSpan timeout) 
            => await BeginBulkUpdateAsync(new AsyncOptions(timeout));
        public async Task<IBulkUpdate> BeginBulkUpdateAsync(TimeSpan timeout, CancellationToken ct)
            => await BeginBulkUpdateAsync(new AsyncOptions(timeout, ct));
        public async Task<IBulkUpdate> BeginBulkUpdateAsync(int millisecondsTimeout) 
            => await BeginBulkUpdateAsync(new AsyncOptions(millisecondsTimeout));
        public async Task<IBulkUpdate> BeginBulkUpdateAsync(int millisecondsTimeout, CancellationToken ct) 
            => await BeginBulkUpdateAsync(new AsyncOptions(millisecondsTimeout, ct));

        public async Task<IBulkUpdate> BeginBulkUpdateAsync(AsyncOptions options)
        {
            var la = (ILockAcquisition)null;

            if (Lock != null)
            {
                la =
                    await
                    Lock
                    .AcquireWriteLockAsync(options)
                    .ConfigureAwait(options.ContinueOnCapturedContext);

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
