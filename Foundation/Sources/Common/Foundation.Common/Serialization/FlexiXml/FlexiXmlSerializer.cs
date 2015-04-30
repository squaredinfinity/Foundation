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
using System.Reflection;
using SquaredInfinity.Foundation.Types.Description.IL;
using System.Collections;

namespace SquaredInfinity.Foundation.Serialization.FlexiXml
{
    public partial class FlexiXmlSerializer : FlexiSerializer, IXmlSerializer
    {
        internal static readonly XNamespace XmlNamespace = XNamespace.Get("http://schemas.squaredinfinity.com/serialization/flexixml");

        public FlexiXmlSerializer()
        { }

        public FlexiXmlSerializer(ITypeDescriptor typeDescriptor)
        {
            this.TypeDescriptor = typeDescriptor;
        }

        protected override IEnumerableTypeSerializationStrategy<T, I> CreateDefaultTypeSerializationStrategy<T, I>(ITypeDescriptor typeDescriptor)
        {
            var result =
                    new FlexiXmlEnumerableTypeSerializationStrategy<T, I>(this, typeof(T), typeDescriptor);

            return result;
        }

        protected override ITypeSerializationStrategy<T> CreateDefaultTypeSerializationStrategy<T>(ITypeDescriptor typeDescriptor)
        {
            var type = typeof(T);

            if (typeof(IEnumerable).IsAssignableFrom(type) && !(type == typeof(string)))
            {
                var result =
                    new FlexiXmlEnumerableTypeSerializationStrategy<T, object>(this, type, typeDescriptor);

                return result;
            }
            else
            {
                var result =
                    new FlexiXmlTypeSerializationStrategy<T>(this, type, typeDescriptor);

                return result;
            }
        }

        protected override ITypeSerializationStrategy CreateDefaultTypeSerializationStrategy(
            Type type, 
            ITypeDescriptor typeDescriptor)
        {
            if(typeof(IEnumerable).IsAssignableFrom(type) && !(type == typeof(string)))
            {
                var result =
                    new FlexiXmlEnumerableTypeSerializationStrategy<object, object>(this, type, typeDescriptor);

                return result;
            }
            else
            {
                var result =
                    new FlexiXmlTypeSerializationStrategy<object>(this, type, typeDescriptor);

                return result;
            }
        }



//        var ss = serializer.GetOrCreateSerializationStrategyForType<MyEntity>()
//.IgnoreAllMembers()
//.SerializeMember(
//    x => x.IntegerMember,
//    m => new Attrbute("ItegerMember", m),
//    el => el.GetAttribute("IntegerMember").Value.ToInt())

//.SerializeMember(x => x.IntegerValue, xName: "integer-xxx")
//    - default, serialize to either attribute (if can be converted to string) or element; use member name as attribute / element name
//    - to attribute or to xml ? based on availability of converter to/from string
//    - from xml -> -||-

//.SerializeMember(
//    x => x.IntegerName,
//    m => new Attribute("xxx-attribute", m))
//    a => a.Value);

//    // default, return string and a conversion will be applied (one of built-in conversions)
//    // otherwise specify actual conversion (e.g. int.Parse(a.Value))
//    - to attribute
//    - from xml -> 

//.SerializeMember(
//    x => x.IntegerName,
//    m => new XElement("xxx", m),
//    el => el.Value);

    }
}
