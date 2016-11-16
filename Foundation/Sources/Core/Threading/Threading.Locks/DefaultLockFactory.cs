using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Foundation.Threading
{
    //public interface IDiagnosticLock : ILock
    //{
    //    OnlineStatisticsInfo ReadStats { get; }
    //    OnlineStatisticsInfo UpgradeableReadStats { get; }
    //    OnlineStatisticsInfo WriteStats { get; }
    //}

    public class DefaultLockFactory : ILockFactory
    {
        readonly object SYNC_KnownLock = new object();
        //readonly List<WeakReference<IDiagnosticLock>> KnownLocks = new List<WeakReference<IDiagnosticLock>>();

        public bool CollectDiagnostics { get; set; }

        public ILock CreateLock()
        {
            return CreateLock("");
        }

        public ILock CreateLock(string name)
        {
            //if (CollectDiagnostics)
            //{
            //    var new_lock = new DiagnosticReaderWriterLockSlimEx(name);

            //    lock (SYNC_KnownLock)
            //        KnownLocks.Add(new WeakReference<IDiagnosticLock>(new_lock));

            //    return new_lock;
            //}
            //else
            //{
                return new ReaderWriterLockSlimEx(name);
            //}
        }
    }
}
