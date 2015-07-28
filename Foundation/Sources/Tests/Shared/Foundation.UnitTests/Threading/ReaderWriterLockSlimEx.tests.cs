using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SquaredInfinity.Foundation.Threading
{
    [TestClass]
    public class ReaderWriterLockSlimExTests
    {
        TimeSpan TEST_DefaultTimeout = TimeSpan.FromMilliseconds(50);

        [TestMethod]
        public void XXX()
        {
            //var l = new ReaderWriterLockSlimEx();

            //using (l.AcquireReadLock())
            //{
            //    // access lock here so it's not optimized-away after release build
            //    Assert.IsNotNull(l);
            //}

            var l = new ReaderWriterLockSlim(LockRecursionPolicy.NoRecursion);

            l.EnterWriteLock();

            Assert.IsTrue(l.IsWriteLockHeld);

            Task.Factory.StartNew(() =>
            {
                Assert.IsFalse(l.IsWriteLockHeld);
            }).Wait();
            
        }

        [TestMethod]
        public void CanAcquireAndReleaseReadLock()
        {
            var l = new ReaderWriterLockSlimEx();

            using(l.AcquireReadLock())
            {
                // access lock here so it's not optimized-away after release build
                Assert.IsNotNull(l);
            }
        }

        [TestMethod]
        public void CanAcquireAndReleaseUpgradeableReadLock()
        {
            var l = new ReaderWriterLockSlimEx();

            using (l.AcquireUpgradeableReadLock())
            {
                // access lock here so it's not optimized-away after release build
                Assert.IsNotNull(l);
            }
        }

        [TestMethod]
        public void CanAcquireAndReleaseWriteLock()
        {
            var l = new ReaderWriterLockSlimEx();

            using (l.AcquireWriteLock())
            {
                // access lock here so it's not optimized-away after release build
                Assert.IsNotNull(l);
            }
        }

        [TestMethod]
        public void CanUpgradeUpgradeableReadLock()
        {
            var l = new ReaderWriterLockSlimEx();

            using (var rl = l.AcquireUpgradeableReadLock())
            {
                // access lock here so it's not optimized-away after release build
                Assert.IsNotNull(l);

                using(rl.UpgradeToWriteLock())
                {
                    // access lock here so it's not optimized-away after release build
                    Assert.IsNotNull(l);
                }
            }
        }

        [TestMethod]
        public void WriteLockIsHeld__AnotherThreadBlocksOnReadLockAcquisition()
        {
            var l = new ReaderWriterLockSlimEx();

            using (l.AcquireWriteLock())
            {
                Task.Factory.StartNew(() =>
                    {
                        var acquisition = (IReadLockAcquisition)null;
                        
                        if(l.TryAcquireReadLock(TEST_DefaultTimeout, out acquisition))
                        {
                            // should not get here
                            Assert.Fail();
                        }
                    }).Wait();
            }
        }

        [TestMethod]
        public void ReadLockIsHeld__CannotAcquireUpgradeableReadLock()
        {
            var l = new ReaderWriterLockSlimEx();

            using (var rl = l.AcquireReadLock())
            {
                var acquisition = (IUpgradeableReadLockAcquisition)null;

                if (l.TryAcquireUpgradeableReadLock(TEST_DefaultTimeout, out acquisition))
                {
                    // should not get here
                    Assert.Fail();
                }
            }
        }

        [TestMethod]
        public void ReadLockIsHeld__AnotherThreadDoesNotBlockOnReadLockAcquisition()
        {
            var l = new ReaderWriterLockSlimEx();

            using (l.AcquireReadLock())
            {
                Task.Factory.StartNew(() =>
                {
                    var acquisition = (IReadLockAcquisition)null;

                    if (!l.TryAcquireReadLock(TEST_DefaultTimeout, out acquisition))
                    {
                        // should be able to acquire
                        Assert.Fail();
                    }
                }).Wait();
            }
        }

        [TestMethod]
        public void ReadLockIsHeld__AnotherThreadBlocksOnWriteLockAcquisition()
        {
            var l = new ReaderWriterLockSlimEx();

            using (l.AcquireReadLock())
            {
                Task.Factory.StartNew(() =>
                {
                    var acquisition = (IWriteLockAcquisition)null;

                    if (l.TryAcquireWriteLock(TEST_DefaultTimeout, out acquisition))
                    {
                        // should not acquire
                        Assert.Fail();
                    }
                }).Wait();
            }
        }

        [TestMethod]
        public void ReadLockIsHeld__AnotherThreadDoesNotBlockOnUpgradeableReadLockAcquisition()
        {
            var l = new ReaderWriterLockSlimEx();

            using (l.AcquireReadLock())
            {
                Task.Factory.StartNew(() =>
                {
                    var acquisition = (IUpgradeableReadLockAcquisition)null;

                    if (!l.TryAcquireUpgradeableReadLock(TEST_DefaultTimeout, out acquisition))
                    {
                        // should be able to acquire
                        Assert.Fail();
                    }
                }).Wait();
            }
        }

        [TestMethod]
        public void UpgradeableReadLockIsHeld__AnotherThreadDoesNotBlockOnUpgradeableReadLockAcquisition()
        {
            // NOTE:    this is how Reader Writer Lock Slim works
            //          only one thread can own Upgradeable Read Lock at the time

            var l = new ReaderWriterLockSlimEx();

            using (l.AcquireUpgradeableReadLock())
            {
                Task.Factory.StartNew(() =>
                {
                    var acquisition = (IUpgradeableReadLockAcquisition)null;

                    if (l.TryAcquireUpgradeableReadLock(TEST_DefaultTimeout, out acquisition))
                    {
                        Assert.Fail();
                    }

                }).Wait();
            }
        }

        [TestMethod]
        public void NoRecursion__ReadLockIsHeld__AnotherLockCannotBeAcquired()
        {
            var l = new ReaderWriterLockSlimEx();

            IReadLockAcquisition r;
            IUpgradeableReadLockAcquisition ur;
            IWriteLockAcquisition w;

            var a1 = l.AcquireReadLock();
            var a2 = l.TryAcquireReadLock(TEST_DefaultTimeout, out r);
            var a3 = l.TryAcquireUpgradeableReadLock(TEST_DefaultTimeout, out ur);
            var a4 = l.TryAcquireWriteLock(TEST_DefaultTimeout, out w);

            Assert.IsFalse(a2);
            Assert.IsFalse(a3);
            Assert.IsFalse(a4);
        }

        [TestMethod]
        public void NoRecursion__UpgradeableReadLockIsHeld__AnotherLockCannotBeAcquired()
        {
            var l = new ReaderWriterLockSlimEx();

            IReadLockAcquisition r;
            IUpgradeableReadLockAcquisition ur;
            IWriteLockAcquisition w;

            var a1 = l.AcquireReadLock();
            var a2 = l.TryAcquireReadLock(TEST_DefaultTimeout, out r);
            var a3 = l.TryAcquireUpgradeableReadLock(TEST_DefaultTimeout, out ur);
            var a4 = l.TryAcquireWriteLock(TEST_DefaultTimeout, out w);

            Assert.IsFalse(a2);
            Assert.IsFalse(a3);
            Assert.IsFalse(a4);
        }

        [TestMethod]
        public void NoRecursion__WriteLockIsHeld__AnotherLockCannotBeAcquired()
        {
            var l = new ReaderWriterLockSlimEx();

            IReadLockAcquisition r;
            IUpgradeableReadLockAcquisition ur;
            IWriteLockAcquisition w;

            var a1 = l.AcquireReadLock();
            var a2 = l.TryAcquireReadLock(TEST_DefaultTimeout, out r);
            var a3 = l.TryAcquireUpgradeableReadLock(TEST_DefaultTimeout, out ur);
            var a4 = l.TryAcquireWriteLock(TEST_DefaultTimeout, out w);

            Assert.IsFalse(a2);
            Assert.IsFalse(a3);
            Assert.IsFalse(a4);
        }

        [TestMethod]
        public void SupportsRecursion__ReadLockIsHeld__OnlyAnotherReadLockCanBeAcquired()
        {
            var l = new ReaderWriterLockSlimEx(LockRecursionPolicy.SupportsRecursion);

            IReadLockAcquisition r;
            IUpgradeableReadLockAcquisition ur;
            IWriteLockAcquisition w;

            var a1 = l.AcquireReadLock();
            var a2 = l.TryAcquireReadLock(TEST_DefaultTimeout, out r);
            var a3 = l.TryAcquireUpgradeableReadLock(TEST_DefaultTimeout, out ur);
            var a4 = l.TryAcquireWriteLock(TEST_DefaultTimeout, out w);

            Assert.IsTrue(a2);
            Assert.IsFalse(a3);
            Assert.IsFalse(a4);
        }

        [TestMethod]
        public void SupportsRecursion__UpgradeableReadLockIsHeld__AnotherLockCanBeAcquired()
        {
            var l = new ReaderWriterLockSlimEx(LockRecursionPolicy.SupportsRecursion);

            IReadLockAcquisition r;
            IUpgradeableReadLockAcquisition ur;
            IWriteLockAcquisition w;

            var a1 = l.AcquireReadLock();
            var a2 = l.TryAcquireReadLock(TEST_DefaultTimeout, out r);

            var a3 = l.TryAcquireUpgradeableReadLock(TEST_DefaultTimeout, out ur);
            var a4 = l.TryAcquireWriteLock(TEST_DefaultTimeout, out w);

            Assert.IsTrue(a2);
            Assert.IsFalse(a3);
            Assert.IsFalse(a4);
        }

        [TestMethod]
        public void SupportsRecursion__WriteLockIsHeld__AnotherLockCanBeAcquired()
        {
            var l = new ReaderWriterLockSlimEx(LockRecursionPolicy.SupportsRecursion);

            IReadLockAcquisition r;
            IUpgradeableReadLockAcquisition ur;
            IWriteLockAcquisition w;

            var a1 = l.AcquireReadLock();
            var a2 = l.TryAcquireReadLock(TEST_DefaultTimeout, out r);
            var a3 = l.TryAcquireUpgradeableReadLock(TEST_DefaultTimeout, out ur);
            var a4 = l.TryAcquireWriteLock(TEST_DefaultTimeout, out w);

            Assert.IsTrue(a2);
            Assert.IsFalse(a3);
            Assert.IsFalse(a4);
        }
    }
}
