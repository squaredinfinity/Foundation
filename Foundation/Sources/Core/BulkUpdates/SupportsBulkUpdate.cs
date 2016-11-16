using SquaredInfinity.Foundation.ComponentModel;
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
        public ILock Lock { get; protected set; } = LockFactory.Current.CreateLock();

        int BulkUpdateCount = 0;

        public bool IsBulkUpdateInProgress()
        {
            return BulkUpdateCount != 0;
        }

        public IBulkUpdate BeginBulkUpdate()
        {
            OnBeforeBeginBulkUpdate();

            var write_lock = Lock.AcquireWriteLockIfNotHeld();

            Interlocked.Increment(ref BulkUpdateCount);

            return new BulkUpdate(this, write_lock);
        }

        protected void OnBeforeBeginBulkUpdate()
        { }

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
