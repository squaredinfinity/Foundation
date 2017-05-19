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

        public void Dispose()
        {
            _isLockHeld = false;
        }
    }
}
