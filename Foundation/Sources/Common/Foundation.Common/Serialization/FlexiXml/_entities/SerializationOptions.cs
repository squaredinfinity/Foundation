using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace SquaredInfinity.Foundation.Serialization.FlexiXml
{
    public class SerializationOptions
    {

        public XName NullValueAttributeName { get; set; }
        public XName UniqueIdAttributeName { get; set; }
        public XName UniqueIdReferenceAttributeName { get; set; }
        public string UniqueIdReferenceAttributeSuffix { get; set; }

        public XName NamespaceAttributeName { get; set; }

        public SerializationOptions()
        {
            NullValueAttributeName = FlexiXmlSerializer.XmlNamespace.GetName("null");
            UniqueIdAttributeName = FlexiXmlSerializer.XmlNamespace.GetName("id");
            UniqueIdReferenceAttributeName = FlexiXmlSerializer.XmlNamespace.GetName("id-ref");
            UniqueIdReferenceAttributeSuffix = ".ref";
            NamespaceAttributeName = FlexiXmlSerializer.XmlNamespace.GetName("ns");
        }
    }
}
