using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using SquaredInfinity.Foundation.Extensions;

namespace SquaredInfinity.Foundation.Serialization.FlexiXml
{
    public class FlexiXmlSerializationOptions
    {

        /// <summary>
        /// Name of attribute in Serialization namespace which will signify that XML Element represents CLR NULL value.
        /// Default is 'null'.
        /// </summary>
        public XName NullValueAttributeName { get; set; }

        /// <summary>
        /// Name of an element in Serialization namespace which will signify that XML Element represents dictionary Key Value Pair value.
        /// Default is 'kvp'.
        /// </summary>
        public XName KeyValuePairElementName { get; set; }

        /// <summary>
        /// Value of an attribute which will represent CLR NULL value.
        /// Default is '{serialization:null}'.
        /// </summary>
        public string NullAttributeMarkupExtension { get; set; }

        public XName UniqueIdAttributeName { get; set; }
        public XName UniqueIdReferenceAttributeName { get; set; }
        public string UniqueIdReferenceAttributeSuffix { get; set; }


        /// <summary>
        /// TODO: this will be used for DETAILED Type Information, not supported at the moment
        /// </summary>
        public XName TypeHintAttributeName { get; set; }

        public string SerializationNamespaceName { get; set; }

        //public XName NamespaceAttributeName { get; set; }

        public FlexiXmlSerializationOptions()
        {
            SerializationNamespaceName = "serialization";
            NullValueAttributeName = FlexiXmlSerializer.XmlNamespace.GetName("null");
            KeyValuePairElementName = FlexiXmlSerializer.XmlNamespace.GetName("kvp");
            UniqueIdAttributeName = FlexiXmlSerializer.XmlNamespace.GetName("id");
            UniqueIdReferenceAttributeName = FlexiXmlSerializer.XmlNamespace.GetName("id-ref");
            UniqueIdReferenceAttributeSuffix = ".ref";

            TypeHintAttributeName = FlexiXmlSerializer.XmlNamespace.GetName("type");


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


        TypeInformation _typeInformation = TypeInformation.None;
        /// <summary>
        /// Specifies how much type information should be included in output stream.
        /// None - no information will be included
        /// LookupOnly - lookup information of known types will be included, this offers balance between speed of deserialization and size of output stream
        /// Detailed - detailed type information will be included. This provides quickest deserialization but also adds to output stream length
        /// </summary>
        public TypeInformation TypeInformation
        {
            get { return _typeInformation; }
            set { _typeInformation = value; }
        }
    }

    public enum TypeInformation
    {
        None = 0,
        LookupOnly,
        Detailed
    }
}
