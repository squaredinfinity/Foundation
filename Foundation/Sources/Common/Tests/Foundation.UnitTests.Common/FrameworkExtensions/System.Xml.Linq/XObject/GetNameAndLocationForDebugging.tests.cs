using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace SquaredInfinity.Foundation.Extensions
{
    [TestClass]
    public class GetNameAndLocationForDebuggingTests
    {
        string Xml =
@"<el>
    <el2 attrib=""val"" />
</el>";

        [TestMethod]
        public void HandlesXAttribute()
        {
            var xdoc = XDocument.Parse(Xml, LoadOptions.SetLineInfo);

            var r = (xdoc.Root.FirstNode as XElement).GetNameAndLocationForDebugging();

            Assert.AreEqual("el2 (2, 6)", r);
        }

        [TestMethod]
        public void HandlesXElement()
        {
            var xdoc = XDocument.Parse(Xml, LoadOptions.SetLineInfo);

            var r = (xdoc.Root).GetNameAndLocationForDebugging();

            Assert.AreEqual("el (1, 2)", r);
        }
    }
}
