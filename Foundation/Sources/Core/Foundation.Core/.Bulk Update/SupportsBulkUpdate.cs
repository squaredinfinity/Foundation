using SquaredInfinity.Foundation.Threading;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SquaredInfinity.Foundation
{
    public class SupportsBulkUpdate : NotifyPropertyChangedObject, ISupportsBulkUpdate
    {
        public ILock Lock { get; protected set; } = new ReaderWriterLockSlimEx(LockRecursionPolicy.NoRecursion);

        int BulkUpdateCount = 0;

        public bool IsBulkUpdateInProgress()
        {
            return BulkUpdateCount != 0;
        }

        public IBulkUpdate BeginBulkUpdate()
        {
            var write_lock = Lock.AcquireWriteLockIfNotHeld();

            Interlocked.Increment(ref BulkUpdateCount);

            return new BulkUpdate(this, write_lock);
        }

        public void EndBulkUpdate(IBulkUpdate bulkUpdate)
        {
            var _count = Interlocked.Decrement(ref BulkUpdateCount);

            bulkUpdate.Dispose();

            if (_count == 0)
            {
                IncrementVersion();
                OnAfterBulkUpdate();
            }
        }

        protected virtual void OnAfterBulkUpdate()
        { }
    }
}
