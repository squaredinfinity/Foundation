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

        XElement SerializeInternal(XName name, object source, ITypeDescriptor typeDescriptor, SerializationOptions options, SerializationContext cx)
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
                     let memberType = Type.GetType(m.AssemblyQualifiedMemberTypeName)
                     where 
                        m.Visibility == MemberVisibility.Public // process public members only
                        && 
                        (
                            (m.CanGetValue && m.CanSetValue) // can set and get value
                            ||
                            (m.CanGetValue && memberType.ImplementsInterface<IEnumerable>()) // cannot set value, but it is a collection
                        )
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
                    else
                    {
                        // this is complex type, use its type as child element name to aid deserialization

                        if (val != null && !((val is IEnumerable) && !m.CanSetValue))
                        {
                            var contentEl = new XElement(m.Name);

                            contentEl.Add(SerializeInternal(ToValidXName(val.GetType(), options), val, typeDescriptor, options, cx));

                            xel.Add(contentEl);
                        }
                        else
                        {
                            xel.Add(SerializeInternal(m.Name, val, typeDescriptor, options, cx));
                        }
                    }
                }

                var enumerable = source as IEnumerable;
                if(enumerable != null)
                {
                    foreach(var item in enumerable)
                    {
                        if (item == null)
                            continue; // todo: may need to specify a custom way of handling this

                        var itemType = item.GetType();

                        var item_as_string = (string) null;

                        if (TryConvertToStringIfTypeSupports(item, out item_as_string))
                        {
                            xel.Add(new XElement(ToValidXName(itemType, options), item_as_string)); // todo: do mapping if needed, + conversion
                            continue;
                        }

                        xel.Add(SerializeInternal(itemType.Name, item, typeDescriptor, options, cx));
                    }
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


        //XObject ConstructXObjectForMember(
        //    string memberName, 
        //    Type memberType, 
        //    string memberValue, 
        //    SerializationOptions options, 
        //    SerializationContext cx)
        //{
        //    var value_as_string = (string)null;

        //    if (TryConvertToStringIfTypeSupports(memberValue, out value_as_string))
        //    {
        //        return new XAttribute(ToValidXName(memberName), value_as_string);
        //    }

        //    // non-string-convertible types are wrapped in an element which represents their name

        //    var wrapperEl = new XElement(ToValidXName(memberType.Name));

        //    return wrapperEl;
        //}

        //XObject ConstructXObjectForListItem(
        //    Type itemType,
        //    SerializationOptions options,
        //    SerializationContext cx)
        //{
        //    var wrapperEl = new XElement(ToValidXName(itemType.Name));

        //    return wrapperEl;
        //}

        protected virtual XName ToValidXName(Type type, SerializationOptions options)
        {
            if(type.IsGenericType)
            {
                var args = type.GetGenericArguments();

                var sb_args = new StringBuilder();

                foreach (var arg in args)
                    sb_args.Append("_" + arg.Name);

                var firstInvalidCharIndex = type.Name.IndexOf("`");

                var name = type.Name.Substring(0, firstInvalidCharIndex) + sb_args.ToString();

                return XName.Get(name);
            }
            else
            {
                return XName.Get(type.Name);
            }
        }

        bool IsValidXName(string name)
        {
            var xname = (XName) null;

            try
            {
                xname = XName.Get(name);

                return true;
            }
            catch
            {
                // memberName contains invalid character
            }

            // use xnmae here to prevent it from being optimized-out
            return xname != null;
        }
    }
}
