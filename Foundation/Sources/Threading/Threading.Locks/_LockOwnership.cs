using SquaredInfinity.Disposables;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using SquaredInfinity.Extensions;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Threading.Locks
{
    /// <summary>
    /// Manages the ownership of a lock
    /// </summary>
    [DebuggerDisplay("{DebuggerDisplay}")]
    class _LockOwnership : DisposableObject
    {
        readonly object Lock = new object();
        public bool IsOwned => _CorrelationTokenToAcquisitionCount.Count > 0;
        public bool ContainsAcquisition(ICorrelationToken correlationToken) => _CorrelationTokenToAcquisitionCount.ContainsKey(correlationToken);

        readonly Dictionary<ICorrelationToken, int> _CorrelationTokenToAcquisitionCount = new Dictionary<ICorrelationToken, int>();
        public IReadOnlyDictionary<ICorrelationToken, int> CorrelationTokenToAcquisitionCount
        {
            get
            {
                lock (Lock)
                {
                    return _CorrelationTokenToAcquisitionCount.ToDictionary(x => x.Key, x => x.Value);
                }
            }
        }

        public void AddAcquisition(ICorrelationToken ct)
        {
            lock (Lock)
            {
                if (_CorrelationTokenToAcquisitionCount.ContainsKey(ct))
                {
                    _CorrelationTokenToAcquisitionCount[ct]++;
                }
                else
                {
                    _CorrelationTokenToAcquisitionCount[ct] = 1;
                }
            }
        }

        public void RemoveAcquisition(ICorrelationToken ct)
        {
            lock (Lock)
            {
                if (_CorrelationTokenToAcquisitionCount.ContainsKey(ct))
                {
                    _CorrelationTokenToAcquisitionCount[ct]--;

                    if (_CorrelationTokenToAcquisitionCount[ct] == 0)
                    {
                        _CorrelationTokenToAcquisitionCount.Remove(ct);
                    }
                }
                else
                {
                    throw new InvalidOperationException("attempt to remove acquisition which isn't held.");
                }
            }
        }

        public _LockOwnership(IDisposable disposeWhenLockReleased)
        {
            Disposables.AddIfNotNull(disposeWhenLockReleased);
        }

        public string DebuggerDisplay => $"Owned by {_CorrelationTokenToAcquisitionCount.Count}";
    }
}
