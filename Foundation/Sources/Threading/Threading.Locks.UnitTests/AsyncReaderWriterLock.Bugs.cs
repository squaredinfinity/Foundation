using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SquaredInfinity.Threading;
using System.Threading.Tasks;
using System.Threading;
using SquaredInfinity.Threading.Locks;
using System.Collections.Generic;
using System.Diagnostics;

namespace Threading.Locks.UnitTests
{
    [TestClass]
    public class AsyncReaderWriterLock__Bugs
    {
        [TestMethod]
        public void BUG001__async_locks_not_released_properly()
        {
            foreach (var lt in new[] { LockType.Read, LockType.Write })
            {
                foreach (var rp in new[] { LockRecursionPolicy.NoRecursion, LockRecursionPolicy.SupportsRecursion })
                {
                    Trace.WriteLine($"TESTING: {lt}, {rp}");

                    var l = new AsyncReaderWriterLock(rp);

                    var tasks = new List<Task>();

                    for (int i = 0; i < 5; i++)
                    {
                        var t =
                        Task.Factory.StartNew(async () =>
                        {
                                var a = (ILockAcquisition)null;

                                switch (lt)
                                {
                                    case LockType.Read:
                                        a = await l.AcquireReadLockAsync();
                                        break;
                                    case LockType.Write:
                                        a = await l.AcquireWriteLockAsync();
                                        break;
                                    default:
                                        throw new NotSupportedException(lt.ToString());
                                }

                                Thread.Sleep(10);

                                a.Dispose();
                        });
                        tasks.Add(t);
                    }

                    //Task.WhenAll(tasks).Wait();

                    if (!Task.WhenAll(tasks).Wait(500))
                        Assert.Fail("test failed, locks possibly not released properly");
                }
            }
        }

        [TestMethod]
        public void BUG002__queued_async_locks_acquired_on_wrong_thread()
        {
            foreach (var lt in new[] { LockType.Read, LockType.Write })
            {
                foreach (var rp in new[] { LockRecursionPolicy.NoRecursion, LockRecursionPolicy.SupportsRecursion })
                {
                    Trace.WriteLine($"TESTING: {lt}, {rp}");

                    var l = new AsyncReaderWriterLock(rp);

                    var tasks = new List<Task>();

                    bool all_acquired_on_correct_thread = true;

                    for (int i = 0; i < 5; i++)
                    {
                        var t =
                        Task.Factory.StartNew(async () =>
                        {
                            var a = (ILockAcquisition)null;

                            switch (lt)
                            {
                                case LockType.Read:
                                    a = await l.AcquireReadLockAsync();
                                    break;
                                case LockType.Write:
                                    a = await l.AcquireWriteLockAsync();
                                    break;
                                default:
                                    throw new NotSupportedException(lt.ToString());
                            }

                            switch (lt)
                            {
                                case LockType.Read:
                                    if (!l.ReadOwnerThreadIds.Contains(Environment.CurrentManagedThreadId))
                                        all_acquired_on_correct_thread = false;
                                    break;
                                case LockType.Write:
                                    if (l.WriteOwnerThreadId != Environment.CurrentManagedThreadId)
                                        all_acquired_on_correct_thread = false;
                                    break;
                                default:
                                    throw new NotSupportedException(lt.ToString());
                            }

                            Thread.Sleep(10);

                            a.Dispose();
                        });
                        tasks.Add(t);
                    }

                    //Task.WhenAll(tasks).Wait();

                    if (!Task.WhenAll(tasks).Wait(500))
                       Assert.Fail();

                    Assert.IsTrue(all_acquired_on_correct_thread);
                }
            }
        }
    }
}
