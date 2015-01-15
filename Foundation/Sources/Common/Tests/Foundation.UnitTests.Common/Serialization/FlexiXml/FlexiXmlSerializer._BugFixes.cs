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
    }
}
