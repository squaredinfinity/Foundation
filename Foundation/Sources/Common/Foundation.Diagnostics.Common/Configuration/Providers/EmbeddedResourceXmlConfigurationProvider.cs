using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Foundation.Diagnostics.Configuration.Providers
{
    public class EmbeddedResourceXmlConfigurationProvider : IConfigurationProvider
    {
        Assembly ResourceAssembly { get; set; }
        string ResourceName { get; set; }

        public EmbeddedResourceXmlConfigurationProvider(Assembly resourceAssembly, string resourceName)
        {
            if (resourceAssembly == null)
                throw new ArgumentException("resourceAssembly must not be null.");

            if (resourceName.IsNullOrEmpty())
                throw new ArgumentException("resourceName must not be null or empty.");

            ResourceAssembly = resourceAssembly;
            ResourceName = resourceName;
        }

        public Entities.DiagnosticsConfiguration LoadConfiguration()
        {
            using (var xmlStream = ResourceAssembly.GetManifestResourceStream(ResourceName))
            {
                if (xmlStream == null)
                    throw new ArgumentException(
                        "Resource {0} cannot be found in assembly {1}"
                        .FormatWith(ResourceName, ResourceAssembly.FullName));

                var configXml = new StreamReader(xmlStream).ReadToEnd();

                var xmlConfigProvider = new XmlConfigurationProvider(configXml);

                return xmlConfigProvider.LoadConfiguration();
            }
        }
    }
}
