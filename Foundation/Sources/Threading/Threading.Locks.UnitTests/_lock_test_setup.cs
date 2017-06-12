using SquaredInfinity.Threading;
using SquaredInfinity.Threading.Locks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Threading.Locks.UnitTests
{
    class _lock_test_setup
    {
        public _lock_test_setup(Func<LockRecursionPolicy, IAsyncLock> getLock)
        {
            _getLock = () => getLock(RecursionPolicy);
        }

        public LockRecursionPolicy RecursionPolicy { get; private set; }

        public LockType LockType { get; private set; }
        
        // AsyncLock reports all locks as Write
        public LockState ExpectedLockState => GetLock() is AsyncLock ? LockState.Write : (LockState)(int)LockType;

        public bool IsAsync { get; private set; }

        Func<IAsyncLock> _getLock;
        public IAsyncLock GetLock() => _getLock();

        public static IEnumerable<_lock_test_setup> AllTestSetups { get; private set; }
        public static IEnumerable<_lock_test_setup> AllAsyncTestSetups => AllTestSetups.Where(x => x.IsAsync == true && x.RecursionPolicy == LockRecursionPolicy.NoRecursion);

        static _lock_test_setup()
        {
            var all_setups = new List<_lock_test_setup>();

            foreach (var rp in new[] { LockRecursionPolicy.NoRecursion, LockRecursionPolicy.SupportsRecursion })
                foreach (var lt in new[] { LockType.Read, LockType.Write })
                    foreach (var ia in new[] { true, false })
                    {
                        all_setups.Add(new _lock_test_setup(_rp => new AsyncLock(_rp))
                        { LockType = lt, RecursionPolicy = rp, IsAsync = ia });

                        all_setups.Add(new _lock_test_setup(_rp => new AsyncReaderWriterLock(_rp))
                        { LockType = lt, RecursionPolicy = rp, IsAsync = ia });
                    }

            AllTestSetups = all_setups;
        }

        public override string ToString()
        {
            return $"{LockType}, {RecursionPolicy}, async:{IsAsync}, {GetLock().GetType().Name}";
        }
    }
}
