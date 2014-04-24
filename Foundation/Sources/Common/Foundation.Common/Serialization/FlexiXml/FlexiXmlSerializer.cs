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
    public class SerializationOptions
    {

    }

    public class InstanceId : IEquatable<InstanceId>
    {
        long _id;
        public long Id
        {
            get { return _id; }
            private set { _id = value; }
        }

        long _referenceCount;
        public long ReferenceCount
        {
            get { return _referenceCount; }
        }

        public InstanceId(long id)
        {
            this.Id = id;
        }

        public void IncrementReferenceCount()
        {
            Interlocked.Increment(ref _referenceCount);
        }

        public static implicit operator long(InstanceId instanceId)
        {
            return instanceId.Id;
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as InstanceId);
        }

        public bool Equals(InstanceId other)
        {
            return this.Id == other.Id;
        }
    }

    public class SerializationContext
    {
        public readonly ConcurrentDictionary<object, InstanceId> Objects_InstanceIdTracker = new ConcurrentDictionary<object, InstanceId>();

        long LastUsedUniqueId = 0;

        public long GetNextUniqueId()
        {
            return Interlocked.Increment(ref LastUsedUniqueId);
        }
    }

    public class DeserializationContext
    {
        public readonly ConcurrentDictionary<InstanceId, object> Objects_InstanceIdTracker = new ConcurrentDictionary<InstanceId, object>();
    }

    public class FllexXmlSerializer : ISerializer
    {
        static readonly string XmlNamespace = "http://schemas.squaredinfinity.com/serialization/flexixml";

        public XElement Serialize(object obj)
        {
            var rootName = obj.GetType().Name;

            var cx = new SerializationContext();

            var x = SerializeInternal(rootName, obj, new ReflectionBasedTypeDescriptor(), new SerializationOptions(), cx);

            // post process
            // e.g. move nodes with an id, which are referenced somewhere to some other part of xml

            // remove serialization attributes where not needed

            var uniqueIds = cx.Objects_InstanceIdTracker.Values.ToArray();

            var areAllEntitiesUnreferenced = true;
            List<InstanceId> referencedIds = new List<InstanceId>();

            for (int i = 0; i < uniqueIds.Length; i++)
            {
                var uid = uniqueIds[i];

                if (uid.ReferenceCount > 0)
                {
                    referencedIds.Add(uid);
                    areAllEntitiesUnreferenced = false;
                }
            }

            if (areAllEntitiesUnreferenced)
            {
                var serializationNodes =
                    (from e in x.DescendantsAndSelf()
                     from a in e.Attributes()
                     where a.Name.NamespaceName == XmlNamespace
                     select a);

                foreach (var serializationAttribute in serializationNodes)
                {
                    serializationAttribute.Remove();
                }
            }
            else
            {
                var nsAttribute = new XAttribute(XNamespace.Xmlns + "serialization", XmlNamespace);
                x.Add(nsAttribute);
            }

            return x;
        }

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

            result = typeConverter.ConvertTo(stringRepresentation, resultType);

            return true;
        }

        public XElement SerializeInternal(string name, object source, ITypeDescriptor typeDescriptor, SerializationOptions options, SerializationContext cx)
        {
            bool isNewReference = false;

            var id =
                cx.Objects_InstanceIdTracker.GetOrAdd(
                source,
                (_) =>
                {
                    isNewReference = true;

                    return new InstanceId(cx.GetNextUniqueId());
                });

            if (isNewReference)
            {
                var xel = new XElement(name);

                var typeDescription = typeDescriptor.DescribeType(source.GetType());

                var serializableMembers =
                    (from m in typeDescription.Members
                     //where m.Visibility == MemberVisibility.Public
                     where m.CanGetValue
                     && m.CanSetValue
                     select m);

                foreach (var m in serializableMembers)
                {
                    var val = m.GetValue(source);

                    if (val == null) // todo: may need to specify a custom way of handling this
                        continue;

                    var val_as_string = (string)null;

                    if (TryConvertToStringIfTypeSupports(val, out val_as_string))
                    {
                        xel.Add(new XAttribute(m.Name, val_as_string)); // todo: do mapping if needed, + conversion
                        continue;
                    }

                    xel.Add(SerializeInternal(m.Name, val, typeDescriptor, options, cx));
                }

                xel.Add(new XAttribute(XName.Get("id", XmlNamespace), id.Id));

                xel.AddAnnotation(typeDescription);

                return xel;
            }
            else
            {
                var idEl = new XElement(name);
                var idAttrib = new XAttribute(XName.Get("id-ref", XmlNamespace), id.Id);

                id.IncrementReferenceCount();

                idEl.Add(idAttrib);

                return idEl;
            }
        }


        public T Deserialize<T>(XDocument xml)
        {
            return Deserialize<T>(xml.Root);
        }

        public T Deserialize<T>(XElement xml)
        {
            var cx = new DeserializationContext();

            var type = typeof(T);

            var root = DeserializeInternal(type, xml, new ReflectionBasedTypeDescriptor(), new SerializationOptions(), cx);

            return (T)root;
        }

        public object DeserializeInternal(Type targetType, XElement xml, ITypeDescriptor typeDescriptor, SerializationOptions options, DeserializationContext cx)
        {
            var typeDescription = typeDescriptor.DescribeType(targetType);

            object target = (object)null;

            // check serialization control attributes first

            var id_ref_attrib = xml.Attribute(XName.Get("id-ref", XmlNamespace));

            if(id_ref_attrib != null)
            {
                var id = Int64.Parse(id_ref_attrib.Value);

                if (cx.Objects_InstanceIdTracker.TryGetValue(new InstanceId(id), out target))
                {
                    return target;
                }
                else
                {
                    // todo: log, this should always resolve unless serialization xml is corrupted
                    return null;
                }
            }

            target = Activator.CreateInstance(targetType, nonPublic: true);

            var id_attrib = xml.Attribute(XName.Get("id", XmlNamespace));

            if(id_attrib != null)
            {
                var id = Int64.Parse(id_attrib.Value);

                cx.Objects_InstanceIdTracker.AddOrUpdate(new InstanceId(id), target);
            }

            foreach (var attribute in xml.Attributes())
            {
                var member =
                    (from f in typeDescription.Members
                     where f.Name == attribute.Name
                     select f).FirstOrDefault();

                if (member == null)
                    continue;

                var memberType = Type.GetType(member.FullMemberTypeName);

                var value = (object)null;

                if (TryConvertFromStringIfTypeSupports(attribute.Value, memberType, out value))
                {
                    member.SetValue(target, value);
                }
            }

            foreach (var el in xml.Elements())
            {
                var member =
                    (from f in typeDescription.Members
                     where f.Name == el.Name
                     select f).FirstOrDefault();

                var memberType = Type.GetType(member.AssemblyQualifiedMemberTypeName);

                var value = DeserializeInternal(memberType, el, typeDescriptor, options, cx);

                member.SetValue(target, value);
            }

            return target;
        }
    }
}
