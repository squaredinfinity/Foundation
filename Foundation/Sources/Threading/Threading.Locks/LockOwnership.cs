using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Threading.Locks
{
    /// <summary>
    /// Provides details about ownership of a lock.
    /// </summary>
    [DebuggerDisplay("{DebuggerDisplay}")]
    public class LockOwnership
    {
        /// <summary>
        /// Correlation token of the lock owner.
        /// </summary>
        public ICorrelationToken CorrelationToken { get; private set; }
        /// <summary>
        /// Number of times this lock has been acquired with given <see cref="CorrelationToken"/>.
        /// </summary>
        /// <seealso cref="CorrelationToken"/>
        public int AcquisitionCount { get; private set; }

        internal LockOwnership(ICorrelationToken ct, int acquisitionCount)
        {
            CorrelationToken = ct;
            AcquisitionCount = acquisitionCount;
        }

        public string DebuggerDisplay => $"{CorrelationToken}, count: {AcquisitionCount}";
    }
}
