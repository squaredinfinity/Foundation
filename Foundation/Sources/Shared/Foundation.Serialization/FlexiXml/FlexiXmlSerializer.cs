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
    public partial class FlexiXmlSerializer : FlexiSerializer, IFlexiXmlSerializer
    {
        internal static readonly XNamespace XmlNamespace = XNamespace.Get("http://schemas.squaredinfinity.com/serialization/flexixml");

        public FlexiXmlSerializer()
        { 

        }

        public FlexiXmlSerializer(ITypeDescriptor typeDescriptor)
        {
            this.TypeDescriptor = typeDescriptor;
        }
        
        FlexiXmlTypeSerializationStrategy<T> CreateDefaultTypeSerializationStrategy<T>()
        {
            return CreateDefaultTypeSerializationStrategy<T>(TypeDescriptor);
        }

        FlexiXmlTypeSerializationStrategy<T> CreateDefaultTypeSerializationStrategy<T>(ITypeDescriptor typeDescriptor)
        {
            var type = typeof(T);

            if (typeof(IDictionary).IsAssignableFrom(type))
            {
                var result =
                    new FlexiXmlDictionaryTypeSerializationStrategy<T, object>(this, type, typeDescriptor);

                return result;
            }
            else if (typeof(IEnumerable).IsAssignableFrom(type) && !(type == typeof(string)))
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
        
        FlexiXmlEnumerableTypeSerializationStrategy<T, TItem> CreateDefaultEnumerableTypeSerializationStrategy<T, TItem>()
        {
            return CreateDefaultEnumerableTypeSerializationStrategy<T, TItem>(TypeDescriptor);
        }

        FlexiXmlEnumerableTypeSerializationStrategy<T, TItem> CreateDefaultEnumerableTypeSerializationStrategy<T, TItem>(ITypeDescriptor typeDescriptor)
        {
            return new FlexiXmlEnumerableTypeSerializationStrategy<T, TItem>(this, typeof(T), typeDescriptor);
        }

        protected override ITypeSerializationStrategy CreateDefaultTypeSerializationStrategy(
            Type type, 
            ITypeDescriptor typeDescriptor)
        {
            if(typeof(IDictionary).IsAssignableFrom(type))
            {
                var result = 
                    new FlexiXmlDictionaryTypeSerializationStrategy<object, object>(this, type, typeDescriptor);

                return result;
            }
            else if(typeof(IEnumerable).IsAssignableFrom(type) && !(type == typeof(string)))
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

        public FlexiXmlTypeSerializationStrategy<T> GetOrCreateTypeSerializationStrategy<T>()
        {
            return GetOrCreateTypeSerializationStrategy<T>(() => CreateDefaultTypeSerializationStrategy<T>());
        }

        FlexiXmlTypeSerializationStrategy<T> GetOrCreateTypeSerializationStrategy<T>(
            Func<FlexiXmlTypeSerializationStrategy<T>> create)
        {
            return (FlexiXmlTypeSerializationStrategy<T>) base.GetOrCreateTypeSerializationStrategy(typeof(T), create);
        }

        FlexiXmlEnumerableTypeSerializationStrategy<T, TItem> CreateDefaultTypeSerializationStrategy<T, TItem>()
            where T : IEnumerable<TItem>
        {
            return CreateDefaultEnumerableTypeSerializationStrategy<T, TItem>(TypeDescriptor);
        }

        public FlexiXmlEnumerableTypeSerializationStrategy<T, TItem> GetOrCreateEnumerableTypeSerializationStrategy<T, TItem>()
            where T : IEnumerable<TItem>
        {
            return (FlexiXmlEnumerableTypeSerializationStrategy<T, TItem>)GetOrCreateTypeSerializationStrategy<T>(() => CreateDefaultEnumerableTypeSerializationStrategy<T, TItem>());
        }

        protected override void ConfigureDefaultStrategies()
        {
            base.ConfigureDefaultStrategies();

            

            //var ts = GetOrCreateTypeSerializationStrategy(typeof(Dictionary<,>), () => CreateDefaultTypeSerializationStrategy(typeof(Dictionary<,>), TypeDescriptor));
            
            //ts.IgnoreMember("Keys");
            //ts.IgnoreMember("Values");

            //this.GetOrCreateTypeSerializationStrategy<Dictionary<object, object>>()
            //    .IgnoreMember(x => x.Keys)
            //    .IgnoreMember(x => x.Values);
        }

        T ISerializer.Deserialize<T>(SerializedDataInfo data)
        {
            var xml_string = data.Encoding.GetString(data.Bytes);

            return this.Deserialize<T>(xml_string);
        }

        SerializedDataInfo ISerializer.Serialize<T>(T obj)
        {
            var xml = this.Serialize(obj);
            var xml_string = xml.ToString(SaveOptions.None);

            var bytes = Encoding.UTF8.GetBytes(xml_string);

            var sdi = new SerializedDataInfo(bytes, Encoding.UTF8);

            return sdi;
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
