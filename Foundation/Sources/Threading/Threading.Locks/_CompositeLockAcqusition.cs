using SquaredInfinity.Disposables;
using SquaredInfinity.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Threading.Locks
{
    class _CompositeLockAcqusition : DisposableObject, ILockAcquisition
    {
        public bool IsLockHeld
        {
            get
            {
                // assume all ok
                var all_ok = true;

                // prove any isn't ok
                foreach (var l in Disposables.OfType<ILockAcquisition>())
                {
                    if (!l.IsLockHeld)
                    {
                        all_ok = false;
                        break;
                    }
                }

                return all_ok;
            }
        }

        public _CompositeLockAcqusition()
        { }

        public _CompositeLockAcqusition(IEnumerable<ILockAcquisition> locks)
        {
            Disposables.AddRange(locks);
        }

        public _CompositeLockAcqusition(IReadOnlyList<ILockAcquisition> locks)
        {
            Disposables.AddRange(locks);
        }

        protected override void DisposeManagedResources()
        {
            base.DisposeManagedResources();
        }
    }
}
