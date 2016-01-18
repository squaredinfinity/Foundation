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
        public void Bug002__readonly_properties_should_be_serializable_when_added_explicitly()
        {
            // todo: need to use something else here, (other than platform class) because it only can be modified in a constructor

            var c1 = new Bug002_Class(new OperatingSystem(PlatformID.Xbox, new Version("1.0.0.0")));

            var s = new FlexiXmlSerializer();

            s.GetOrCreateTypeSerializationStrategy<Bug002_Class>()
                .SerializeMember(x => x.OS);
            s.GetOrCreateTypeSerializationStrategy<OperatingSystem>()
                .SerializeMember(x => x.Platform);

            var xml = s.Serialize(c1);

            var operatingSystemNode =
                (from n in xml.Descendants()
                 where n.Name == "OperatingSystem"
                 select n).FirstOrDefault();

            var platform = operatingSystemNode.Attribute("Platform").Value;

            Assert.AreEqual(platform, "Xbox");

            var c2 = s.Deserialize<Bug002_Class>(xml);

            Assert.IsNotNull(c2);

            // NOTE: Platform on c2 will not be deserialized properly, because it is a read-only property, it can only be serialized
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

        #region BUG 003

        [TestMethod]
        public void Bug003__id_property_conflicts_with_serializationId()
        {
            // type contains Id property
            // during deserialization xml attribute serialization:id is confused with attribute to actual Id

            var c1 = new Bug003_Class();
            var id = Guid.NewGuid();
            c1.Id = id;

            var c2 = new Bug003_Class();
            c2.Id = id;
            c2.Parent = c1;

            c1.Parent = c2;

            var s = new FlexiXmlSerializer();
            
            s.GetOrCreateTypeSerializationStrategy<Bug003_Class>()
                .SerializeMember(x => x.Id);
            
            var xml = s.Serialize(c2);

            // remove Id attribute (but keep serialization:Id)
            xml.Attribute("Id").Remove();

            // note: normally this would fail if xml does not have Id (Guid) attribute but does have serialization:Id attribute
            // this is because serialization:Id (int) would be treated as Guid and fail to covnert

            var c3 = s.Deserialize<Bug003_Class>(xml);

            Assert.IsNotNull(c3);
            Assert.AreEqual(Guid.Empty, c3.Id);
            Assert.AreEqual(id, c3.Parent.Id);
        }

        public class Bug003_Class
        {
            Guid _id;
            public Guid Id
            {
                get { return _id; }
                set { _id = value; }
            }

            Bug003_Class _parent;
            public Bug003_Class Parent
            {
                get { return _parent; }
                set { _parent = value; }
            }
        }

        #endregion

        #region BUG 004

        [TestMethod]
        public void Bug004__DeserializationFailsIfDictionaryAlreadyContainsAKey__ExistingValueShouldBeOverridenInstead()
        {
            var dict = new BUG004_Dict();
            dict.Add(7, "seven");

            var s = new FlexiXmlSerializer();

            var xml = s.Serialize(dict);

            var dict2 = s.Deserialize<BUG004_Dict>(xml);

            // would throw exception by now

            Assert.IsNotNull(dict2);
            Assert.AreEqual(2, dict2.Count);
            Assert.AreEqual("zero", dict2[0]);
            Assert.AreEqual("seven", dict2[7]);
        }

        public class BUG004_Dict : Dictionary<int, string>
        {
            public BUG004_Dict()
            {
                this.Add(0, "zero");
            }
        }

        #endregion

        #region BUG next


        #endregion

    }
}
