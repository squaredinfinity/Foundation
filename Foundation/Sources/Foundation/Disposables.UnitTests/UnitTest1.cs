using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SquaredInfinity.Disposables;
using System.Collections;
using System.Collections.Generic;

namespace Disposables.UnitTests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            var l = new List<int> { 1, 2, 3 };
            WeakReference<IList> wr = new WeakReference<IList>(l);

            ReferenceDisposable<IList> d = new ReferenceDisposable<IList>(l, _ => { });

            l = null;

            GC.Collect(GC.MaxGeneration);
            GC.Collect(GC.MaxGeneration);

            Assert.IsTrue(d.IsDisposed);
        }
    }
}
