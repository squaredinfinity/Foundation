using SquaredInfinity.Foundation.Types.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using SquaredInfinity.Foundation.Extensions;
using System.Collections.Concurrent;
using SquaredInfinity.Foundation.Types.Description.Reflection;
using SquaredInfinity.Foundation.Types.Description;
using System.Threading;
using System.ComponentModel;
using System.Collections;
using SquaredInfinity.Foundation.Types.Description.IL;

namespace SquaredInfinity.Foundation.Serialization.FlexiXml
{
    public partial class FlexiXmlSerializer
    {
        public T Deserialize<T>(string xml)
        {
            return Deserialize<T>(xml, new FlexiXmlSerializationOptions());
        }

        public T Deserialize<T>(string xml, FlexiXmlSerializationOptions options)
        {
            var xDoc = XDocument.Parse(xml);

            return Deserialize<T>(xDoc, options);
        }

        public T Deserialize<T>(XDocument xml)
        {
            return Deserialize<T>(xml, new FlexiXmlSerializationOptions());
        }

        public T Deserialize<T>(XDocument xml, FlexiXmlSerializationOptions options)
        {
            return Deserialize<T>(xml.Root, options);
        }

        public T Deserialize<T>(XElement xml)
        {
            return Deserialize<T>(xml, new FlexiXmlSerializationOptions());
        }

        public T Deserialize<T>(XElement xml, FlexiXmlSerializationOptions options)
        {
            if (options == null)
                options = new FlexiXmlSerializationOptions();

            var cx = new FlexiXmlSerializationContext(this, TypeDescriptor, TypeResolver, options, CustomCreateInstanceWith);

            // internal settings should always be first child
            var internalSettings = xml.Elements().FirstOrDefault();

            if(internalSettings == null || internalSettings.Name != FlexiXmlSerializer.XmlNamespace.GetName("Internal"))
                internalSettings = null;


            if(internalSettings != null && options.TypeInformation != TypeInformation.None)
            {
                // find known types element
                
                // add known types element
                var knownTypes_element = internalSettings.Element(FlexiXmlSerializer.XmlNamespace.GetName("KnownTypes"));

                if (knownTypes_element != null)
                {
                    foreach (var ke in knownTypes_element.Elements(FlexiXmlSerializer.XmlNamespace.GetName("KnownType")))
                    {
                        var alias = ke.Attribute(FlexiXmlSerializer.XmlNamespace.GetName("alias")).Value;
                        var assemblyQualifiedName = ke.Attribute(FlexiXmlSerializer.XmlNamespace.GetName("assemblyQualifiedName")).Value;

                        cx.KnownTypes.Add(XName.Get(alias), Type.GetType(assemblyQualifiedName));
                    }
                }
            }


            var type = typeof(T);

            var root = cx.Deserialize(xml, type, elementNameMayContainTargetTypeName: true);
            
            return (T)root;
        }
    }
}
