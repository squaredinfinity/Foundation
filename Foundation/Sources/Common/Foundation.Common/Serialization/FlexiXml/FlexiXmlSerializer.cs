using SquaredInfinity.Foundation.Serialization.FlexiXml.Types.Mapping;
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

namespace SquaredInfinity.Foundation.Serialization.FlexiXml
{
    public class SerializationOptions
    {

    }

    public class SerializationContext
    {
        public readonly ConcurrentDictionary<object, object> Objects_InstanceIdTracker = new ConcurrentDictionary<object, object>();
    }

    public class FllexXmlSerializer : ISerializer
    {
        public XDocument Serialize(object obj)
        {
            var x = SerializeInternal(obj, new SerializationOptions(), new SerializationContext());
            return null;
        }

        bool IsBuiltInSimpleValueType(object obj)
        {
            return obj is sbyte
                || obj is byte
                || obj is short
                || obj is ushort
                || obj is int
                || obj is uint
                || obj is long
                || obj is ulong
                || obj is float
                || obj is double
                || obj is decimal
                || obj is string
                || obj is Enum;
        }

        XObject SerializeInternal(object source, SerializationOptions options, SerializationContext cx)
        {
            var descriptor = new ReflectionBasedTypeDescriptor();

            var description = descriptor.DescribeType(source.GetType());

            return SerializeInternal("name", source, description, options, cx);
        }

        public XObject SerializeInternal(string name, object source, ITypeDescription typeDescription, SerializationOptions options, SerializationContext cx)
        {
            var xel = new XElement(name);

            var serializableMembers =
                (from m in typeDescription.Members
                 where m.Visibility == MemberVisibility.Public
                 && m.CanGetValue
                 && m.CanSetValue
                 select m);

            foreach (var m in serializableMembers)
            {
                var val = m.GetValue(source);

                if (IsBuiltInSimpleValueType(source))
                {
                    xel.Add(new XAttribute(m.Name, val)); // todo: do mapping if needed, + conversion
                    continue;
                }

                bool isCloneNew = false;

                var clone =
                    cx.Objects_InstanceIdTracker.GetOrAdd(
                    source,
                    (_) =>
                    {
                        isCloneNew = true;
                        return CreateClonePrototype(targetType);
                    });

                var sourceType = source.GetType();

                if (isCloneNew)
                    MapInternal(source, clone, sourceType, targetType, options, cx);
            }

            return xel;
        }

        XElement ToXml(FlexiMappedInstance map, string name)
        {
            var result = new XElement(name);

            foreach(var key in map.Members.Keys)
            {
                var m = map.Members[key];

                result.Add(ToXml(m, key));
            }

            if(map.OriginalValue != null)
                result.Value = map.OriginalValue.ToString();

            return result;
        }

        public T Deserialize<T>(System.Xml.Linq.XDocument xml)
        {
            throw new NotImplementedException();
        }
    }
}
