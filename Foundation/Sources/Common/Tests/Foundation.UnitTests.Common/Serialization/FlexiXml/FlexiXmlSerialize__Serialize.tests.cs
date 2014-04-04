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
    public class FlexiXmlSerialize__Serialize
    {
        [TestMethod]
        public void CanSerializeTypes()
        {
        //    var n = TestEntities.LinkedListNode.CreateDefaultTestHierarchy();

            var n = new TestEntities.LinkedListNode { Id = 13 };
            var s = new FlexiXml.FllexXmlSerializer();

            var xml = s.Serialize(n);

            Assert.IsNotNull(xml);
        }
    }
}
