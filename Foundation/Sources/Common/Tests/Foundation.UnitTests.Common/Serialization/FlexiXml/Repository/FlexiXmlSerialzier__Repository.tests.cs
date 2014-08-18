using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SquaredInfinity.Foundation.Extensions;
using System.Reflection;
using System.Xml.Linq;
using SquaredInfinity.Foundation.Serialization.FlexiXml.Repository._entities;

namespace SquaredInfinity.Foundation.Serialization.FlexiXml.Repository
{
    [TestClass]
    public class FlexiXmlSerialzier__Repository
    {
        [TestMethod]
        public void CanCustomizeHowReferencesAreResolved()
        {
            var xml =
                Assembly.GetExecutingAssembly()
                .GetManifestResourceStream(@"SquaredInfinity.Foundation.Serialization.FlexiXml.Repository.Resources.repository.xml")
                .ReadToEnd();

            var xDoc = XDocument.Parse(xml);

            var s = new FlexiXmlSerializer();

            var options = new SerializationOptions();

            options.UniqueIdAttributeName = XName.Get("name");
            options.UniqueIdReferenceAttributeName = XName.Get("ref");

            var o = s.Deserialize<Configuration>(xDoc, options);

            Assert.IsNotNull(o);

            Assert.AreEqual(2, o.Repository.Formatters.Count);
            Assert.AreEqual(2, o.Emails.Count);

            Assert.AreSame(o.Repository.Formatters[0], o.Emails[0].BodyFormatter);
            Assert.AreSame(o.Repository.Formatters[1], o.Emails[0].SubjectFormatter);

            Assert.AreSame(o.Repository.Formatters[0], o.Emails[1].BodyFormatter);
            Assert.AreSame(o.Repository.Formatters[1], o.Emails[1].SubjectFormatter);
        }
    }
}
