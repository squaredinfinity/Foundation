using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SquaredInfinity.Disposables;
using System.Collections;
using System.Collections.Generic;

namespace Disposables.UnitTests
{
    [TestClass]
    public class CompositeDisposableTests
    {
        [TestMethod]
        public void disposes_contents()
        {
            var cd = new CompositeDisposable();
            var cd2 = new CompositeDisposable();

            cd.Add(cd2);

            cd.Dispose();

            Assert.IsTrue(cd.IsDisposed);
            Assert.IsTrue(cd2.IsDisposed);
        }

        [TestMethod]
        public void it_is_safle_to_dispose_twice()
        {
            var cd = new CompositeDisposable();
            var cd2 = new CompositeDisposable();

            cd.Add(cd2);

            cd.Dispose();
            cd.Dispose(); // no exception thrown here
        }
    }
}
