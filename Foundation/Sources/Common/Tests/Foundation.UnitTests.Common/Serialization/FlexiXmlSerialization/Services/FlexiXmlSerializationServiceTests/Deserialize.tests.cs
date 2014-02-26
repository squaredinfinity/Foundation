using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Foundation.Serialization.FlexiXmlSerialization.Services.FlexiXmlSerializationServiceTests
{
    [TestClass]
    public class DeserializeTests
    {
        [TestMethod]
        public void AutomaticallyDeserializesPropertiesOfSimpleTypes()
        {
            var xml = @"<Entity StringProperty=""str"" Int32Property=""13"" EnumProperty=""Monday"" />";

            var s = new FlexiXmlSerializationService();

            var r = s.Deserialize<TestEntities.Entity>(xml);

            Assert.IsNotNull(r);
            Assert.AreEqual("str", r.StringProperty);
            Assert.AreEqual(13, r.Int32Property);
            Assert.AreEqual(DayOfWeek.Monday, r.EnumProperty);
        }
    }
}
