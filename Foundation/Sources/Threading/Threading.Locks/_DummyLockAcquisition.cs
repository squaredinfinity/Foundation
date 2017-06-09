using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Threading.Locks
{
    class _DummyLockAcquisition : ILockAcquisition
    {
        volatile bool _isLockHeld = true;
        public bool IsLockHeld => _isLockHeld;
        public ICorrelationToken CorrelationToken { get; private set; }

        public _DummyLockAcquisition(ICorrelationToken correlationToken)
        {
            CorrelationToken = correlationToken ?? throw new ArgumentNullException(nameof(correlationToken));
        }

        public void Dispose()
        {
            _isLockHeld = false;
        }
    }
}
