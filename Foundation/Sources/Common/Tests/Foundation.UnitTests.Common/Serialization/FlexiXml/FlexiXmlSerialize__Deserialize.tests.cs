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
    public class FlexiXmlSerialize__Deserialize
    {
        [TestMethod]
        public void CanDeserializeSerializedTypes()
        {
            var n = LinkedListNode.CreateDefaultTestHierarchy();

            var s = new FlexiXml.FlexiXmlSerializer();

            var xml = s.Serialize(n);

            Assert.IsNotNull(xml);

            var n1 = s.Deserialize<LinkedListNode>(xml);

            Assert.IsNotNull(n1);
            
            LinkedListNode.EnsureDefaultTestHierarchyPreserved(n1);
        }

        [TestMethod]
        public void TypeWithCollectionReadOnlyProperty__CanDeserialize()
        {
            var o = new TypeWithCollectionReadOnlyProperty();
            o.Items.Add(7);
            o.Items.Add(13);

            var s = new FlexiXml.FlexiXmlSerializer();

            var xml = s.Serialize(o);

            var o2 = s.Deserialize<TypeWithCollectionReadOnlyProperty>(xml);

            Assert.IsNotNull(o2);
            Assert.IsNotNull(o2.Items);
            Assert.AreEqual(2, o2.Items.Count);

            Assert.AreEqual(7, o2.Items[0]);
            Assert.AreEqual(13, o2.Items[1]);
        }
    }
}
