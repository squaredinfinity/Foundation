﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace SquaredInfinity.Foundation.Extensions
{
    [TestClass]
    public class Type__ImplementsInterface
    {
        public interface IA { }
        public abstract class A : IA { }
        public class B : A { }
        public class C { }

        [TestMethod]
        public void TypeImplementsSpecifiedInterface__ReturnsTrue()
        {
            var t = typeof(XElement);            
            var r = t.ImplementsInterface<IXmlLineInfo>();
            Assert.IsTrue(r);

            t = typeof(B);
            r = t.ImplementsInterface<IA>();
            Assert.IsTrue(r);
        }

        [TestMethod]
        public void TypeDoesNotImplementSpecifiedInterface__ReturnsFalse()
        {
            var t = typeof(XElement);            
            var r = t.ImplementsInterface<IAsyncResult>();
            Assert.IsFalse(r);

            t = typeof(C);
            r = t.ImplementsInterface<IA>();
            Assert.IsFalse(r);
        }

        [TestMethod]
        public void TypeImplementsInterfaceWithSpecifiedFullName__ReturnsTrue()
        {
            var t = typeof(XElement);
            var i = typeof(IXmlLineInfo);
            var r = t.ImplementsInterface(i.FullName);
            Assert.IsTrue(r);

            t = typeof(B);
            i = typeof(IA);
            r = t.ImplementsInterface(i.FullName);
            Assert.IsTrue(r);
        }

        [TestMethod]
        public void TypeDoesNotImplementInterfaceWithSpecifiedFullName__ReturnsFalse()
        {
            var t = typeof(XElement);
            var i = typeof(IAsyncResult);
            var r = t.ImplementsInterface(i.FullName);
            Assert.IsFalse(r);

            t = typeof(C);
            i = typeof(IA);
            r = t.ImplementsInterface(i.FullName);
            Assert.IsFalse(r);
        }
    }
}
