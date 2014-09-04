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

                using(rl.AcquireWriteLock())
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
                        var acquisition = l.TryAcquireReadLock(TEST_DefaultTimeout);

                        Assert.IsFalse(acquisition.IsSuccesfull);
                    }).Wait();
            }
        }

        [TestMethod]
        public void WriteLockIsHeld__xxx()
        {
            var l = new ReaderWriterLockSlimEx(LockRecursionPolicy.SupportsRecursion);

            using (l.AcquireWriteLock())
            {
                var acquisition = l.TryAcquireReadLock(TEST_DefaultTimeout);
                Assert.IsFalse(acquisition.IsSuccesfull);
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
                    var acquisition = l.TryAcquireReadLock(TEST_DefaultTimeout);

                    Assert.IsTrue(acquisition.IsSuccesfull);
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
                    var acquisition = l.TryAcquireWriteLock(TEST_DefaultTimeout);

                    Assert.IsFalse(acquisition.IsSuccesfull);
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
                    var acquisition = l.TryAcquireUpgradeableReadLock(TEST_DefaultTimeout);

                    Assert.IsTrue(acquisition.IsSuccesfull);
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
                    var acquisition = l.TryAcquireUpgradeableReadLock(TEST_DefaultTimeout);

                    // try acquire failed, meaning that this thread has to wait for lock to be released
                    Assert.IsFalse(acquisition.IsSuccesfull);
                }).Wait();
            }
        }

        [TestMethod]
        public void NoRecursion__ReadLockIsHeld__AnotherLockCannotBeAcquired()
        {
            var l = new ReaderWriterLockSlimEx();

            var a1 = l.AcquireReadLock();
            var a2 = l.TryAcquireReadLock(TEST_DefaultTimeout);
            var a3 = l.TryAcquireUpgradeableReadLock(TEST_DefaultTimeout);
            var a4 = l.TryAcquireWriteLock(TEST_DefaultTimeout);

            Assert.IsFalse(a2.IsSuccesfull);
            Assert.IsFalse(a3.IsSuccesfull);
            Assert.IsFalse(a4.IsSuccesfull);
        }

        [TestMethod]
        public void NoRecursion__UpgradeableReadLockIsHeld__AnotherLockCannotBeAcquired()
        {
            var l = new ReaderWriterLockSlimEx();

            var a1 = l.AcquireUpgradeableReadLock();
            var a2 = l.TryAcquireReadLock(TEST_DefaultTimeout);
            var a3 = l.TryAcquireUpgradeableReadLock(TEST_DefaultTimeout);
            var a4 = l.TryAcquireWriteLock(TEST_DefaultTimeout);

            Assert.IsFalse(a2.IsSuccesfull);
            Assert.IsFalse(a3.IsSuccesfull);
            Assert.IsFalse(a4.IsSuccesfull);
        }

        [TestMethod]
        public void NoRecursion__WriteLockIsHeld__AnotherLockCannotBeAcquired()
        {
            var l = new ReaderWriterLockSlimEx();

            var a1 = l.AcquireWriteLock();
            var a2 = l.TryAcquireReadLock(TEST_DefaultTimeout);
            var a3 = l.TryAcquireUpgradeableReadLock(TEST_DefaultTimeout);
            var a4 = l.TryAcquireWriteLock(TEST_DefaultTimeout);

            Assert.IsFalse(a2.IsSuccesfull);
            Assert.IsFalse(a3.IsSuccesfull);
            Assert.IsFalse(a4.IsSuccesfull);
        }

        [TestMethod]
        public void SupportsRecursion__ReadLockIsHeld__OnlyAnotherReadLockCanBeAcquired()
        {
            var l = new ReaderWriterLockSlimEx(LockRecursionPolicy.SupportsRecursion);

            var a1 = l.AcquireReadLock();
            var a2 = l.TryAcquireReadLock(TEST_DefaultTimeout);
            var a3 = l.TryAcquireUpgradeableReadLock(TEST_DefaultTimeout);
            var a4 = l.TryAcquireWriteLock(TEST_DefaultTimeout);

            Assert.IsTrue(a2.IsSuccesfull);
            Assert.IsFalse(a3.IsSuccesfull);
            Assert.IsFalse(a4.IsSuccesfull);
        }

        [TestMethod]
        public void SupportsRecursion__UpgradeableReadLockIsHeld__AnotherLockCanBeAcquired()
        {
            var l = new ReaderWriterLockSlimEx(LockRecursionPolicy.SupportsRecursion);

            var a1 = l.AcquireUpgradeableReadLock();
            var a2 = l.TryAcquireReadLock(TEST_DefaultTimeout);
            var a3 = l.TryAcquireUpgradeableReadLock(TEST_DefaultTimeout);
            var a4 = l.TryAcquireWriteLock(TEST_DefaultTimeout);

            Assert.IsTrue(a2.IsSuccesfull);
            Assert.IsTrue(a3.IsSuccesfull);
            Assert.IsTrue(a4.IsSuccesfull);
        }

        [TestMethod]
        public void SupportsRecursion__WriteLockIsHeld__AnotherLockCanBeAcquired()
        {
            var l = new ReaderWriterLockSlimEx(LockRecursionPolicy.SupportsRecursion);

            var a1 = l.AcquireWriteLock();
            var a2 = l.TryAcquireReadLock(TEST_DefaultTimeout);
            var a3 = l.TryAcquireUpgradeableReadLock(TEST_DefaultTimeout);
            var a4 = l.TryAcquireWriteLock(TEST_DefaultTimeout);

            Assert.IsTrue(a2.IsSuccesfull);
            Assert.IsTrue(a3.IsSuccesfull);
            Assert.IsTrue(a4.IsSuccesfull);
        }
    }
}
