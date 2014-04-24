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
    public partial class FlexiXmlSerializer
    {
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

        XElement SerializeInternal(string name, object source, ITypeDescriptor typeDescriptor, SerializationOptions options, SerializationContext cx)
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

                xel.Add(new XAttribute(UniqueIdAttributeName, id.Id));

                xel.AddAnnotation(typeDescription);

                return xel;
            }
            else
            {
                var idEl = new XElement(name);
                var idAttrib = new XAttribute(UniqueIdReferenceAttributeName, id.Id);

                id.IncrementReferenceCount();

                idEl.Add(idAttrib);

                return idEl;
            }
        }
    }
}
