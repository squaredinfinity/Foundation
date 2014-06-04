using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SquaredInfinity.Foundation.Extensions;
using System.Reflection;
using System.Xml.Linq;

namespace SquaredInfinity.Foundation.Serialization.FlexiXml
{
    [TestClass]
    public class FlexiXmlSerialzier__Repository
    {
        [TestMethod]
        public void ElementsFromRepositoryCanBeReferencedUsingReferenceAttributes()
        {
            var xml =
                Assembly.GetExecutingAssembly()
                .GetManifestResourceStream(@"SquaredInfinity.Foundation.Serialization.FlexiXml.Resources.Repository.repository.xml")
                .ReadToEnd();

            var xDoc = XDocument.Parse(xml);

            var s = new FlexiXmlSerializer();

            var options = new SerializationOptions();
            options.UniqueIdAttributeName = XName.Get("name");
            options.UniqueIdReferenceAttributeName = XName.Get("ref");

            var o = s.Deserialize<TestRoot>(xDoc, options);
        }

        public class TestRoot
        {
            public Repository Repository { get; set; }

            List<Email> _emails;
            public List<Email> Emails { get { return _emails; } }

            TestRoot()
            {
                _emails = new List<Email>();
            }
        }

        public class Repository
        {
            public List<IFormatter> Formatters { get; set; }
        }

        public class Email
        {
            public IFormatter SubjectFormatter { get; set; }
            public IFormatter BodyFormatter { get; set; }
        }

        public interface IFormatter
        {
            string Content { get; }
        }
            
        public class GenericFormatter : IFormatter
        {

            string _content;
            public string Content
            {
                get { return _content; }
                set { _content = value; }
            }
        }

        public class CustomFormatter : IFormatter
        {

            string _content;
            public string Content
            {
                get { return _content; }
                set { _content = value; }
            }
        }
    }
}
