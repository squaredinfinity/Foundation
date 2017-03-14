using SquaredInfinity.Serialization.FlexiXml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace SquaredInfinity.Diagnostics.Configuration.Providers
{
    public class XmlConfigurationProvider : IConfigurationProvider
    {
        IDiagnosticsConfiguration Configuration { get; set; }

        FlexiXmlSerializer Serializer = new FlexiXmlSerializer();

        public XmlConfigurationProvider()
        {
            InitializeSerializer();
        }

        void InitializeSerializer()
        {
        }

        public XmlConfigurationProvider(string configurationXml)
        {
            this.Configuration = Serializer.Deserialize<IDiagnosticsConfiguration>(configurationXml);
        }

        public IDiagnosticsConfiguration LoadConfiguration()
        {
            return Configuration;
        }

        public XElement SaveConfiguration(IDiagnosticsConfiguration configuration)
        {
            FlexiXmlSerializationOptions o = new FlexiXmlSerializationOptions();
            o.SerializeNonPublicTypes = false;

            return Serializer.Serialize(configuration);
        }
    }
}
