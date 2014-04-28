﻿using SquaredInfinity.Foundation.Types.Mapping;
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
            var cx = new SerializationContext();

            var options = new SerializationOptions();

            var root = new XElement("temp");
            UpdateElementNameFromType(root, obj.GetType(), options);
            SerializeInternal(root, obj, new ReflectionBasedTypeDescriptor(), options , cx);
            
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
                var idNodes =
                    (from e in root.DescendantsAndSelf()
                     from a in e.Attributes()
                     where a.Name == UniqueIdAttributeName
                     select a);

                foreach (var idAttrib in idNodes)
                {
                    idAttrib.Remove();
                }
            }

            var hasAnySerializationAttributes =
                    (from e in root.DescendantsAndSelf()
                     from a in e.Attributes()
                     where a.Name.NamespaceName == XmlNamespace
                     select a).Any();

            if(hasAnySerializationAttributes)
            {
                var nsAttribute = new XAttribute(XNamespace.Xmlns.GetName("serialization"), XmlNamespace);
                root.Add(nsAttribute);
            }

            return root;
        }

        void SerializeInternal(XElement target, object source, ITypeDescriptor typeDescriptor, SerializationOptions options, SerializationContext cx)
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
                            (m.CanGetValue && memberType.ImplementsInterface<IList>()) // cannot set value, but it is a collection
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
                        target.Add(new XAttribute(m.Name, val_as_string)); // todo: do mapping if needed, + conversion
                        continue;
                    }
                    else
                    {
                        // this is complex type, use its type as child element name to aid deserialization

                        if (val != null && !((val is IList) && !m.CanSetValue))
                        {
                            var wrapper = new XElement(m.Name);

                            var el = new XElement("temp");
                            UpdateElementNameFromType(el, val.GetType(), options);
                            SerializeInternal(el, val, typeDescriptor, options, cx);

                            wrapper.Add(el);

                            target.Add(wrapper);
                        }
                        else
                        {
                            // null or collection without setter
                            var el = new XElement(m.Name);
                            SerializeInternal(el, val, typeDescriptor, options, cx);
                            target.Add(el);
                        }
                    }
                }

                var list = source as IList;
                if(list != null)
                {
                    foreach(var item in list)
                    {
                        if (item == null)
                            continue; // todo: may need to specify a custom way of handling this

                        var itemType = item.GetType();

                        var item_as_string = (string) null;

                        if (TryConvertToStringIfTypeSupports(item, out item_as_string))
                        {
                            var item_string_value_element = new XElement("temp", item_as_string);
                            UpdateElementNameFromType(item_string_value_element, item.GetType(), options);
                            target.Add(item_string_value_element);
                            continue;
                        }

                        var itemElement = new XElement("temp");
                        UpdateElementNameFromType(itemElement, item.GetType(), options);
                        SerializeInternal(itemElement, item, typeDescriptor, options, cx);

                        target.Add(itemElement);
                    }
                }

                target.Add(new XAttribute(UniqueIdAttributeName, id.Id));

                target.AddAnnotation(typeDescription);
            }
            else
            {
                var idAttrib = new XAttribute(UniqueIdReferenceAttributeName, id.Id);

                id.IncrementReferenceCount();

                target.Add(idAttrib);
            }
        }

        protected virtual void UpdateElementNameFromType(XElement xel, Type type, SerializationOptions options)
        {
            if(type.IsGenericType)
            {
                var genericArgumentsSeparator_Index = type.Name.IndexOf("`");

                xel.Name = type.Name.Substring(0, genericArgumentsSeparator_Index);

                var typeAttrib = new XAttribute(TypeAttributeName, type.FullName);
                xel.Add(typeAttrib);

                var assemblyAttrib = new XAttribute(AssemblyAttributeName, type.Assembly.FullName);
                xel.Add(assemblyAttrib);
            }
            else
            {
                xel.Name = type.Name;
            }
        }
    }
}