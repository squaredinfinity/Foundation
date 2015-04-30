using Microsoft.VisualStudio.TestTools.UnitTesting;
using SquaredInfinity.Foundation.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Foundation.Serialization.FlexiXml.KnownTypesLookup
{
    [TestClass]
    public class KnownTypesLookup
    {
        [TestMethod]
        public void UsesKnownTypesLookup()
        {
            // in this test Entity on root is serialized as TestEntityA, but KnownTypes mappign is set (all in xml) to TestEntityB
            // Entity should therefore resolve to TestEntityB

            var xml =
                Assembly.GetExecutingAssembly()
                .GetManifestResourceStream(@"SquaredInfinity.Foundation.Serialization.FlexiXml.KnownTypesLookup.KnownTypesLookup.xml")
                .ReadToEnd();

            var s = new FlexiXmlSerializer();

            var op = new FlexiXmlSerializationOptions();
            op.TypeInformation = TypeInformation.LookupOnly;

            var root = s.Deserialize<TestRoot>(xml, op);

            Assert.IsNotNull(root);

            Assert.AreEqual(typeof(TestEntityB), root.Entity.GetType());
            Assert.AreEqual(13, root.Entity.Id);

        }
    }
}
