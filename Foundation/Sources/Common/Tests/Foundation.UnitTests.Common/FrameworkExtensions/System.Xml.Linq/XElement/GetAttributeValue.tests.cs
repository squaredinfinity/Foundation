using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using SquaredInfinity.Foundation;

namespace SquaredInfinity.Foundation.Extensions
{
    [TestClass]
    public class GetAttributeValueTests
    {
        [TestMethod]
        public void AttributeExistsAndHasValue__ReturnsValueOfTheAttribute()
        {
            var el = new XElement("el");

            var attrib = new XAttribute("attrib", 13);

            el.Add(attrib);

            var v = el.GetAttributeValue("attrib");

            Assert.AreEqual("13", v);
        }

        [TestMethod]
        public void AttributeExistsWithoutValue__ReturnsNullByDefault()
        {
            var el = new XElement("el");

            var attrib = new XAttribute("attrib", "");

            el.Add(attrib);

            var v = el.GetAttributeValue("attrib");

            Assert.AreEqual(null, v);
        }

        [TestMethod]
        public void AttributeExistsWithoutValue__ReturnsSpecifiedDefaultValue()
        {
            var el = new XElement("el");

            var attrib = new XAttribute("attrib", "");

            el.Add(attrib);

            var v = el.GetAttributeValue("attrib", "default-value");

            Assert.AreEqual("default-value", v);
        }

        [TestMethod]
        public void AttributeDoesNotExist__ReturnsNullByDefault()
        {
            var el = new XElement("el");
            
            var v = el.GetAttributeValue("attrib");

            Assert.AreEqual(null, v);
        }

        [TestMethod]
        public void AttributeDoesNotExist__ReturnsSpecifiedDefaultValue()
        {
            var el = new XElement("el");

            var v = el.GetAttributeValue("attrib", "default-value");

            Assert.AreEqual("default-value", v);
        }
    }
}
