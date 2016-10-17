using SquaredInfinity.Foundation.Maths.Statistics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Foundation.Threading
{
    public interface ILockFactory
    {
        bool CollectDiagnostics { get; set; }
        ILock CreateLock();
    }

    public interface IDiagnosticLock : ILock
    {
        OnlineStatisticsInfo ReadStats { get; }
        OnlineStatisticsInfo UpgradeableReadStats { get; }
        OnlineStatisticsInfo WriteStats { get; }
    }

    public class LockFactory : ILockFactory
    {
        readonly object SYNC_KnownLock = new object();
        readonly List<WeakReference<IDiagnosticLock>> KnownLocks = new List<WeakReference<IDiagnosticLock>>();

        public bool CollectDiagnostics { get; set; }

        public ILock CreateLock()
        {
            if (CollectDiagnostics)
            {
                var new_lock = new DiagnosticReaderWriterLockSlimEx();

                lock (SYNC_KnownLock)
                    KnownLocks.Add(new WeakReference<IDiagnosticLock>(new_lock));

                return new_lock;
            }
            else
            {
                return new ReaderWriterLockSlimEx();
            }
        }
    }

    public static class Lock
    {
        public static ILockFactory Factory { get; set; } = new LockFactory();
    }
}
