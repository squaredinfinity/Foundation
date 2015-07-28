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
        public XElement Serialize(object obj)
        {
            if (obj == null)
                throw new ArgumentNullException("obj");

            return Serialize(obj, rootElementName: null);
        }

        public XElement Serialize(object obj, FlexiXmlSerializationOptions options)
        {
            if (obj == null)
                throw new ArgumentNullException("obj");

            return Serialize(obj, rootElementName: null, options: options);
        }

        
        public XElement Serialize(object obj, string rootElementName)
        {
            return Serialize(obj, rootElementName, new FlexiXmlSerializationOptions());
        }

        public XElement Serialize(object obj, string rootElementName, FlexiXmlSerializationOptions options)
        {
            if(obj == null)
                throw new ArgumentNullException("obj");
            
            var objType = obj.GetType();

            var cx = new FlexiXmlSerializationContext(this, TypeDescriptor, TypeResolver, options);

            cx.RootElement = cx.Serialize(obj, rootElementName);

            bool hasAnySerializationAttributesOrElements = false;

            //# Expand Instance Id and Id-Ref attributes where needed
            
            var idNodes =
                (from e in cx.RootElement.DescendantsAndSelf()
                    select e);

            foreach (var node in idNodes)
            {
                var instanceId = node.Annotation<InstanceId>();

                if (instanceId != null)
                {
                    if (instanceId.ReferenceCount == 0)
                    {
                        // nothing to do here
                    }
                    else
                    {
                        var idAttribute = new XAttribute(options.UniqueIdAttributeName, instanceId.Id);
                        node.Add(idAttribute);

                        hasAnySerializationAttributesOrElements = true;
                    }
                }

                // process attributes
                foreach(var attribute in node.Attributes())
                {
                    var instanceIdRef = attribute.Annotation<InstanceIdRef>();

                    if(instanceIdRef != null)
                    {
                        if(!attribute.Value.IsNullOrEmpty())
                        {
                            // todo: this should never happen
                        }
                        else
                        {
                            attribute.Value = instanceIdRef.InstanceId.ToString();
                        }
                    }
                }
            }

            var internalSettingsElement = new XElement(FlexiXmlSerializer.XmlNamespace.GetName("Internal"));
            var hasInternalSettings = false;

            if (options.TypeInformation == TypeInformation.LookupOnly)
            {
                // add known types element
                var knownTypes_element = new XElement(FlexiXmlSerializer.XmlNamespace.GetName("KnownTypes"));

                var knownTypeElementName = FlexiXmlSerializer.XmlNamespace.GetName("KnownType");
                var aliasAttributeName = FlexiXmlSerializer.XmlNamespace.GetName("alias");
                var assemblyQualifiedNameAttributeName = FlexiXmlSerializer.XmlNamespace.GetName("assemblyQualifiedName");

                foreach(var kt in cx.KnownTypes)
                {
                    foreach (var t in kt.Value)
                    {
                        var kt_el = new XElement(knownTypeElementName);

                        kt_el.AddAttribute(aliasAttributeName, kt.Key.ToString());
                        kt_el.AddAttribute(assemblyQualifiedNameAttributeName, t.AssemblyQualifiedName);

                        knownTypes_element.Add(kt_el);
                    }
                }

                internalSettingsElement.Add(knownTypes_element);
                hasInternalSettings = true;
            }

            if(hasInternalSettings)
            {
                hasAnySerializationAttributesOrElements = true;

                // internal settings should always be the first child for quicker lookup during deserialization
                cx.RootElement.AddFirst(internalSettingsElement);
            }

            if (!hasAnySerializationAttributesOrElements)
            {
                hasAnySerializationAttributesOrElements =
                        (from e in cx.RootElement.DescendantsAndSelf()
                         from a in e.Attributes()
                         where
                            a.Name.NamespaceName == XmlNamespace
                            ||
                            cx.Options.IsAttributeValueSerializationExtension(a)
                         select a).Any();
            }

            if(hasAnySerializationAttributesOrElements)
            {
                var nsAttribute = new XAttribute(XNamespace.Xmlns.GetName(options.SerializationNamespaceName), XmlNamespace);
                cx.RootElement.Add(nsAttribute);
            }

            return cx.RootElement;
        }

        public XElement Serialize<T>(IEnumerable<T> items, string rootElementName, Func<T, XElement> getItemElement)
        {
            var root = new XElement(rootElementName);

            foreach(var item in items)
            {
                root.Add(getItemElement(item));
            }

            return root;
        }

    }
}
