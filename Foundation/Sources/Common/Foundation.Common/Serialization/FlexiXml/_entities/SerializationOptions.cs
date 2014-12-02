using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using SquaredInfinity.Foundation.Extensions;

namespace SquaredInfinity.Foundation.Serialization.FlexiXml
{
    public class SerializationOptions
    {

        /// <summary>
        /// Name of attribute in Serialization namespace which will signify that XML Element represents CLR NULL value.
        /// Default is 'null'.
        /// </summary>
        public XName NullValueAttributeName { get; set; }

        /// <summary>
        /// Value of an attribute which will represent CLR NULL value.
        /// Default is 'null'.
        /// </summary>
        public string NullAttributeMarkupExtension { get; private set; }

        public XName UniqueIdAttributeName { get; set; }
        public XName UniqueIdReferenceAttributeName { get; set; }
        public string UniqueIdReferenceAttributeSuffix { get; set; }

        public string SerializationNamespaceName { get; set; }

        //public XName NamespaceAttributeName { get; set; }

        public SerializationOptions()
        {
            SerializationNamespaceName = "serialization";
            NullValueAttributeName = FlexiXmlSerializer.XmlNamespace.GetName("null");
            UniqueIdAttributeName = FlexiXmlSerializer.XmlNamespace.GetName("id");
            UniqueIdReferenceAttributeName = FlexiXmlSerializer.XmlNamespace.GetName("id-ref");
            UniqueIdReferenceAttributeSuffix = ".ref";
            //NamespaceAttributeName = FlexiXmlSerializer.XmlNamespace.GetName("ns");

            NullAttributeMarkupExtension = 
                    "{0}{1}:{2}{3}"
                    .FormatWith(
                        "{",
                        SerializationNamespaceName, 
                        NullValueAttributeName.LocalName.ToString(),
                        "}");
        }

        public bool IsAttributeValueSerializationExtension(XAttribute attrib)
        {
            if (attrib == null)
                return false;

            if (attrib.Value.IsNullOrEmpty())
                return false;

            if (!attrib.Value.StartsWith("{") || !attrib.Value.EndsWith("}"))
                return false;

            // {} is an escape sequence (like in XAML)
            // not really used anywhere at the moment
            if (attrib.Value.StartsWith("{}"))
                return false;

            return true;
        }

        bool _serializeNonPublicTypes = true;
        public bool SerializeNonPublicTypes 
        {
            get { return _serializeNonPublicTypes; }
            set { _serializeNonPublicTypes = value; }
        }
    }
}
