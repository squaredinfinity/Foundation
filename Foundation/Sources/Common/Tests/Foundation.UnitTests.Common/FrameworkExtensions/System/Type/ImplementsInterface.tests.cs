using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace SquaredInfinity.Foundation
{
    [TestClass]
    public class ImplementsInterfaceTests
    {
        [TestMethod]
        public void TypeImplementsSpecifiedInterface__ReturnsTrue()
        {
            var t = typeof(XElement);
            
            var r = t.ImplementsInterface<IXmlLineInfo>();

            Assert.IsTrue(r);
        }

        [TestMethod]
        public void TypeDoesNotImplementSpecifiedInterface__ReturnsFalse()
        {
            var t = typeof(XElement);
            
            var r = t.ImplementsInterface<IAsyncResult>();

            Assert.IsFalse(r);
        }

        [TestMethod]
        public void TypeImplementsInterfaceWithSpecifiedFullName__ReturnsTrue()
        {
            var t = typeof(XElement);
            var i = typeof(IXmlLineInfo);

            var r = t.ImplementsInterface(i.FullName);

            Assert.IsTrue(r);
        }

        [TestMethod]
        public void TypeDoesNotImplementInterfaceWithSpecifiedFullName__ReturnsFalse()
        {
            var t = typeof(XElement);
            var i = typeof(IAsyncResult);

            var r = t.ImplementsInterface(i.FullName);

            Assert.IsFalse(r);
        }
    }
}
