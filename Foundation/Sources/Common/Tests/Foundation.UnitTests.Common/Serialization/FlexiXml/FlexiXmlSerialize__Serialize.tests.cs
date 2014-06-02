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

        [TestMethod]
        public void CollectionWithNullableValueTypeItems__CanSerialize()
        {
            var list = new List<int?>();
            list.Add(null);
            list.Add(1);
            list.Add(2);
            list.Add(null);
            list.Add(4);
            list.Add(null);

            var s = new FlexiXml.FlexiXmlSerializer();

            var xml = s.Serialize(list);

            Assert.IsNotNull(xml);

            var deserialized = s.Deserialize<List<int?>>(xml);

            Assert.AreEqual(6, deserialized.Count);
            Assert.IsNull(deserialized[0]);
            Assert.IsNull(deserialized[3]);
            Assert.IsNull(deserialized[5]);

            Assert.AreEqual(1, deserialized[1]);
            Assert.AreEqual(2, deserialized[2]);
            Assert.AreEqual(4, deserialized[4]);
        }
    }
}
