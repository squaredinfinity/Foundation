﻿using SquaredInfinity.Foundation.Threading;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Foundation
{
    public class BulkUpdate : IBulkUpdate
    {
        readonly ISupportsBulkUpdate Owner;
        readonly IWriteLockAcquisition LockAcquisition;
        bool HasFinished = false;

        public BulkUpdate(ISupportsBulkUpdate owner, IWriteLockAcquisition lockAcquisition)
        {
            this.Owner = owner;
            LockAcquisition = lockAcquisition;
        }

        public void Dispose()
        {
            if (HasFinished)
                return;

            HasFinished = true;
            Owner.EndBulkUpdate(this);

            if (LockAcquisition != null)
                LockAcquisition.Dispose();
        }
    }
}
