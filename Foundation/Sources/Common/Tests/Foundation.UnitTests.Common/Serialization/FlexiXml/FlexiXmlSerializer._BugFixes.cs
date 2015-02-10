using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Foundation.Serialization.FlexiXml
{
	[TestClass]
    public class FlexiXmlSerializer_BUGFIXES
    {
        #region BUG 001

        [TestMethod]
        [Description("deserialization fails when root element has not child elemnts")]
        public void Bug001__root_with_no_child_elements()
        {
            var c1 = new Bug001_Class();

            var s = new FlexiXmlSerializer();

            var xml = s.Serialize(c1);

            var c2 = s.Deserialize<Bug001_Class>(xml);

            Assert.IsNotNull(c2);
        }

        public class Bug001_Class
        {}

        #endregion

        #region BUG 002

        [TestMethod]
        public void Bug001__readonly_properties_should_be_serializable_when_added_explicitly()
        {
            // todo: need to use something else here, (other than platform class) because it only can be modified in a constructor

            var c1 = new Bug002_Class(new OperatingSystem(PlatformID.Xbox, new Version("1.0.0.0")));

            var s = new FlexiXmlSerializer();

            s.GetOrCreateTypeSerializationStrategy<Bug002_Class>()
                .SerializeMember(x => x.OS);
            s.GetOrCreateTypeSerializationStrategy<OperatingSystem>()
                .SerializeMember(x => x.Platform);

            var xml = s.Serialize(c1);

            var c2 = s.Deserialize<Bug002_Class>(xml);



            Assert.IsNotNull(c2);
            Assert.AreEqual(PlatformID.Xbox, c2.OS.Platform);
        }

        public class Bug002_Class
        {
            OperatingSystem _os;
            public OperatingSystem OS { get { return _os; } }

            public Bug002_Class()
            {
                _os = new OperatingSystem(PlatformID.Win32Windows, new Version("1.0.0.0"));
            }

            public Bug002_Class(OperatingSystem os)
            {
                _os = os;
            }
        }

        #endregion
    }
}
