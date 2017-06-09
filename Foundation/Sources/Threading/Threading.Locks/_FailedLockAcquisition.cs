using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Threading.Locks
{
    class _FailedLockAcquisition : ILockAcquisition
    {
        public bool IsLockHeld => false;

        public ICorrelationToken CorrelationToken { get; private set; }

        public _FailedLockAcquisition(ICorrelationToken correlationToken)
        {
            CorrelationToken = correlationToken ?? throw new ArgumentNullException(nameof(correlationToken));
        }

        public void Dispose() { }
    }
}
