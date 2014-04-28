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

namespace SquaredInfinity.Foundation.Serialization.FlexiXml
{
    public partial class FlexiXmlSerializer : IXmlSerializer
    {
        internal static readonly XNamespace XmlNamespace = XNamespace.Get("http://schemas.squaredinfinity.com/serialization/flexixml");
        internal static readonly XName UniqueIdAttributeName = XmlNamespace.GetName("id");
        internal static readonly XName UniqueIdReferenceAttributeName = XmlNamespace.GetName("id-ref");

        internal static readonly XName AssemblyAttributeName = XmlNamespace.GetName("assembly");
        internal static readonly XName TypeAttributeName = XmlNamespace.GetName("type");

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

        public Func<Type, CreateInstanceContext, object> CustomCreateInstanceWith { get; set; }


        // todo context should be local to serialzation (type)
        // todo: this may also try to reuse type mapper code rather than copy it
        protected bool TryCreateInstace(Type targetType, CreateInstanceContext create_cx, out object newInstance)
        {
            newInstance = null;

            try
            {
                if (CustomCreateInstanceWith != null)
                {
                    try
                    {
                        newInstance = CustomCreateInstanceWith(targetType, create_cx);
                        return true;
                    }
                    catch(Exception ex)
                    {
                        // todo: log
                    }
                }
                
                var constructor = targetType
                    .GetConstructor(
                    BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic,
                    binder: null,
                    types: Type.EmptyTypes,
                    modifiers: null);

                if (constructor != null)
                {
                    newInstance = constructor.Invoke(null);

                    return true;
                }
            }
            catch (Exception ex)
            {
                // todo: internal logging
            }

            return false;
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
