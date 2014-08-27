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

        public XElement Serialize(object obj, string rootElementName)
        {
            if(obj == null)
                throw new ArgumentNullException("obj");
            
            var options = new SerializationOptions();

            var objType = obj.GetType();

            var cx = new SerializationContext(this, TypeDescriptor, TypeResolver, options);

            cx.RootElement = cx.Serialize(obj, rootElementName);            

            //# remove serialization attributes where not needed            
            
            var idNodes =
                (from e in cx.RootElement.DescendantsAndSelf()
                    select e);

            foreach (var node in idNodes)
            {
                var instanceId = node.Annotation<InstanceId>();

                if(instanceId == null)
                    continue;

                if(instanceId.ReferenceCount == 0)
                {
                    node.Attribute(options.UniqueIdAttributeName).Remove();
                }
            }

            var hasAnySerializationAttributes =
                    (from e in cx.RootElement.DescendantsAndSelf()
                     from a in e.Attributes()
                     where a.Name.NamespaceName == XmlNamespace
                     select a).Any();

            if(hasAnySerializationAttributes)
            {
                var nsAttribute = new XAttribute(XNamespace.Xmlns.GetName("serialization"), XmlNamespace);
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
