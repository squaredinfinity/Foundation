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

namespace SquaredInfinity.Foundation.Serialization.FlexiXml
{
    public class SerializationOptions
    {

    }

    public class InstanceId
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

            for(int i = 0;  i < uniqueIds.Length; i++)
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

                foreach(var serializationAttribute in serializationNodes)
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

                    if (IsBuiltInSimpleValueType(val))
                    {
                        xel.Add(new XAttribute(m.Name, val)); // todo: do mapping if needed, + conversion
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
                var idAttrib = new XAttribute(XName.Get("id-ref", "http://schemas.squaredinfinity.com/serialization/flexixml"), id.Id);

                id.IncrementReferenceCount();

                idEl.Add(idAttrib);

                return idEl;
            }
        }


        public T Deserialize<T>(System.Xml.Linq.XDocument xml)
        {
            throw new NotImplementedException();
        }
    }
}
