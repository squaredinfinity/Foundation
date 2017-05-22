﻿using SquaredInfinity.Disposables;
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
        volatile bool _isLockHeld = true;
        public bool IsLockHeld
        {
            get
            {
                if (!_isLockHeld)
                    return false;

                // if any lock is not held, return false
                // if all locks are held, return true
                
                foreach (var l in Disposables.OfType<ILockAcquisition>())
                {
                    if (!l.IsLockHeld)
                        return false;
                }

                return true;
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

            _isLockHeld = false;
        }
    }
}
