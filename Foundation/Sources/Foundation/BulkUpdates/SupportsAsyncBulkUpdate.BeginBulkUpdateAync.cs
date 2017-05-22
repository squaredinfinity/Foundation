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
        #region BeginBulkUpdateAsync
        
        public async Task<IBulkUpdate> BeginBulkUpdateAsync()
        {
            var ao = AsyncOptions.Default;

            return
                await
                BeginBulkUpdateAsync(ao)
                .ConfigureAwait(ao.ContinueOnCapturedContext);
        }

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
