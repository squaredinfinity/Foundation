using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Threading.Locks
{
    public interface ILockAcquisition : IDisposable
    {
        /// <summary>
        /// True if lock has been acquired and is held by a thread (not necessarily current thread).
        /// False otherwise.
        /// </summary>
        bool IsLockHeld { get; }
    }
}
