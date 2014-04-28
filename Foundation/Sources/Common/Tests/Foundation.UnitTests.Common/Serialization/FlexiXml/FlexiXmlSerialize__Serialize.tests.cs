using Microsoft.VisualStudio.TestTools.UnitTesting;
using SquaredInfinity.Foundation.Serialization.FlexiXml.TestEntities;
using SquaredInfinity.Foundation.TestEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Foundation.Serialization.FlexiXml
{
    [TestClass]
    public class FlexiXmlSerialize__Serialize
    {
        [TestMethod]
        public void CanSerializeTypes()
        {
            var n = LinkedListNode.CreateDefaultTestHierarchy();

            var s = new FlexiXml.FlexiXmlSerializer();

            var xml = s.Serialize(n);

            Assert.IsNotNull(xml);
        }

        [TestMethod]
        public void TypeWithCollectionReadOnlyProperty__CanSerialize()
        {
            var o = new TypeWithCollectionReadOnlyProperty();
            o.Items.Add(7);
            o.Items.Add(13);

            var s = new FlexiXml.FlexiXmlSerializer();

            var xml = s.Serialize(o);

            Assert.IsNotNull(xml);

            Assert.IsTrue(xml.Descendants().Count() == 3, "Not all collection items have been serialized");
        }
    }
}
