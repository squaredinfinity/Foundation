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
            throw new NotImplementedException();
        }

        protected override ITypeSerializationStrategy<T> CreateDefaultTypeSerializationStrategy<T>(ITypeDescriptor typeDescriptor)
        {
            throw new NotImplementedException();
        }

        protected override ITypeSerializationStrategy CreateDefaultTypeSerializationStrategy(
            Type type, 
            ITypeDescriptor typeDescriptor)
        {
            if(typeof(IEnumerable).IsAssignableFrom(type) && !(type == typeof(string)))
            {
                var result =
                    new FlexiXmlEnumerableTypeSerializationStrategy(this, type, typeDescriptor);

                return result;

                throw new NotImplementedException();
            }
            else
            {
                var result =
                    new FlexiXmlTypeSerializationStrategy(this, type, typeDescriptor);

                return result;
            }
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
                        throw;
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
                ex.TryAddContextData("target type", () => targetType.FullName);
                InternalTrace.Information(ex, "Failed to create instance of type.");
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


        public IFlexiXmlTypeSerializationStrategy GetTypeSerializationStrategy(Type type)
        {
            return (IFlexiXmlTypeSerializationStrategy)base.GetTypeSerializationStrategy(type);
        }
    }
}
