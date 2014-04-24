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

namespace SquaredInfinity.Foundation.Serialization.FlexiXml
{
    public partial class FlexiXmlSerializer : IXmlSerializer
    {
        internal static readonly string XmlNamespace = "http://schemas.squaredinfinity.com/serialization/flexixml";
        internal static readonly XName UniqueIdAttributeName = XName.Get("id", XmlNamespace);
        internal static readonly XName UniqueIdReferenceAttributeName = XName.Get("id-ref", XmlNamespace);

        readonly TypeSerializationStrategiesConcurrentDictionary TypeSerializationStrategies =
            new TypeSerializationStrategiesConcurrentDictionary();

        bool TryConvertToStringIfTypeSupports(object obj, out string result)
        {
            var typeConverter = TypeDescriptor.GetConverter(obj);

            if (typeConverter == null
                // what is serialized will need to be deserialized
                // check if conversion can work both ways
                || !typeConverter.CanConvertFrom(typeof(string))
                || !typeConverter.CanConvertTo(typeof(string)))
            {
                result = null;
                return false;
            }

            result = (string)typeConverter.ConvertTo(obj, typeof(string));

            return true;
        }

        bool TryConvertFromStringIfTypeSupports(string stringRepresentation, Type resultType, out object result)
        {
            var typeConverter = TypeDescriptor.GetConverter(resultType);

            if (typeConverter == null || !typeConverter.CanConvertFrom(typeof(string)))
            {
                result = null;
                return false;
            }

            result = typeConverter.ConvertFrom(stringRepresentation);

            return true;
        }

        public ITypeSerializationStrategy<T> GetOrCreateTypeSerializationStrategy<T>()
        {
            return GetOrCreateTypeSerializationStrategy<T>(() => CreateDefaultTypeSerializationStrategy<T>());
        }

        public ITypeSerializationStrategy<T> GetOrCreateTypeSerializationStrategy<T>(
            Func<ITypeSerializationStrategy<T>> create)
        {
            var key = typeof(T);

            return (ITypeSerializationStrategy<T>)TypeSerializationStrategies
                .GetOrAdd(key, (ITypeSerializationStrategy)create());
        }

        ITypeSerializationStrategy<T> CreateDefaultTypeSerializationStrategy<T>()
        {
            return CreateDefaultTypeSerializationStrategy<T>(
                new ReflectionBasedTypeDescriptor());
        }
        ITypeSerializationStrategy<T> CreateDefaultTypeSerializationStrategy<T>(
            ITypeDescriptor typeDescriptor)
        {

            var result =
                new TypeSerializationStrategy<T>(
                    typeDescriptor);

            return result;
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
