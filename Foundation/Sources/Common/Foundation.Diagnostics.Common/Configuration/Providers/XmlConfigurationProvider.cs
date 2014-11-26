using SquaredInfinity.Foundation.Serialization.FlexiXml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Foundation.Diagnostics.Configuration.Providers
{
    public class XmlConfigurationProvider : IConfigurationProvider
    {
        IDiagnosticsConfiguration Configuration { get; set; }

        public XmlConfigurationProvider(string configurationXml)
        {
            FlexiXmlSerializer serializer = new FlexiXmlSerializer();

            this.Configuration = serializer.Deserialize<IDiagnosticsConfiguration>(configurationXml);
        }

        public IDiagnosticsConfiguration LoadConfiguration()
        {
            return Configuration;
        }
    }
}
