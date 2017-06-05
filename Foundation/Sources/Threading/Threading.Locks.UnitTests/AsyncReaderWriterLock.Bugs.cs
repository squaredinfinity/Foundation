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
                    var iterations = 10;
                    CountdownEvent count = new CountdownEvent(iterations);

                    var l = new AsyncReaderWriterLock(rp);
                    
                    Parallel.For(0, iterations, async i =>
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
                        using (a)
                        {
                            count.Signal();
                            Trace.WriteLine($"{lt} {rp} {i}");

                            //Thread.Sleep(10);
                        }
                    });

                    count.Wait();
                }
            }
        }

        [TestMethod]
        public void BUG002__queued_async_locks_acquired_on_wrong_thread()
        {
            foreach (var lt in new[] { LockType.Read })// , LockType.Write })
            {
                foreach (var rp in new[] { LockRecursionPolicy.NoRecursion , LockRecursionPolicy.SupportsRecursion })
                {
                    Trace.WriteLine($"TESTING: {lt}, {rp}");
                    int interation_count = 10;
                    CountdownEvent count = new CountdownEvent(interation_count);

                    var l = new AsyncReaderWriterLock(rp);

                    var tasks = new List<Task>();

                    bool all_acquired_on_correct_thread = true;

                    for (int i = 0; i < interation_count; i++)
                    {
                        var _i = i;

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

                            using (a)
                            {
                                count.Signal();

                                switch (lt)
                                {
                                    case LockType.Read:
                                        if(!l.ReadOwners.Where(x => x.CorrelationToken == ThreadCorrelationToken.FromCurrentThread).Any());
                                        {
                                            all_acquired_on_correct_thread = false;
                                        }
                                        break;
                                    case LockType.Write:
                                        if (l.WriteOwner.CorrelationToken != ThreadCorrelationToken.FromCurrentThread)
                                            all_acquired_on_correct_thread = false;
                                        break;
                                    default:
                                        throw new NotSupportedException(lt.ToString());
                                }

                                Thread.Sleep(10);
                            }
                        });
                        tasks.Add(t);
                    }

                    count.Wait();
                    Task.WaitAll(tasks.ToArray());

                    Assert.IsTrue(all_acquired_on_correct_thread);
                }
            }
        }

        [TestMethod]
        public async Task MyTestMethod()
        {
            var l = new AsyncReaderWriterLock();
            var l2 = new AsyncReaderWriterLock();
            l.AddChild(l2);

            for(int i = 0; i < 100; i++)
            {
                Task.Run(async () =>
                {
                    using (await l.AcquireWriteLockAsync())
                    {
                       
                    }
                });

                Task.Run(async () =>
                {
                    using (var x = await l2.AcquireWriteLockAsync())
                    {
                        
                    }
                });
            }

            Thread.Sleep(5000);
        }
    }
}
