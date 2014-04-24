using Microsoft.VisualStudio.TestTools.UnitTesting;
using SquaredInfinity.Foundation.Serialization.FlexiXml.Entities;
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
            var n = TestEntities.LinkedListNode.CreateDefaultTestHierarchy();

            var s = new FlexiXml.FlexiXmlSerializer();

            var xml = s.Serialize(n);

            Assert.IsNotNull(xml);

            var n1 = s.Deserialize<TestEntities.LinkedListNode>(xml);

            Assert.IsNotNull(n1);
            
            TestEntities.LinkedListNode.EnsureDefaultTestHierarchyPreserved(n1);
        }
    }
}
