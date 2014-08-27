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
            return Deserialize<T>(xml, new SerializationOptions());
        }

        public T Deserialize<T>(string xml, SerializationOptions options)
        {
            var xDoc = XDocument.Parse(xml);

            return Deserialize<T>(xDoc, options);
        }

        public T Deserialize<T>(XDocument xml)
        {
            return Deserialize<T>(xml, new SerializationOptions());
        }

        public T Deserialize<T>(XDocument xml, SerializationOptions options)
        {
            return Deserialize<T>(xml.Root, options);
        }

        public T Deserialize<T>(XElement xml)
        {
            return Deserialize<T>(xml, new SerializationOptions());
        }

        public T Deserialize<T>(XElement xml, SerializationOptions options)
        {
            if (options == null)
                options = new SerializationOptions();

            var cx = new SerializationContext(this, TypeDescriptor, TypeResolver, options, CustomCreateInstanceWith);

            var type = typeof(T);

            var root = cx.Deserialize(xml, type, elementNameMayContainTargetTypeName: true);
            
            return (T)root;
        }
    }
}
