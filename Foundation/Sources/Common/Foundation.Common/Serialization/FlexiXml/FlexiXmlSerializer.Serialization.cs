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

        public XElement Serialize(object obj, SerializationOptions options)
        {
            if (obj == null)
                throw new ArgumentNullException("obj");

            return Serialize(obj, rootElementName: null, options: options);
        }

        
        public XElement Serialize(object obj, string rootElementName)
        {
            return Serialize(obj, rootElementName, new SerializationOptions());
        }

        public XElement Serialize(object obj, string rootElementName, SerializationOptions options)
        {
            if(obj == null)
                throw new ArgumentNullException("obj");
            
            var objType = obj.GetType();

            var cx = new SerializationContext(this, TypeDescriptor, TypeResolver, options);

            cx.RootElement = cx.Serialize(obj, rootElementName);

            bool hasAnySerializationAttributes = false;

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

                        hasAnySerializationAttributes = true;
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

            if (!hasAnySerializationAttributes)
            {
                hasAnySerializationAttributes =
                        (from e in cx.RootElement.DescendantsAndSelf()
                         from a in e.Attributes()
                         where
                            a.Name.NamespaceName == XmlNamespace
                            ||
                            cx.Options.IsAttributeValueSerializationExtension(a)
                         select a).Any();
            }

            if(hasAnySerializationAttributes)
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
