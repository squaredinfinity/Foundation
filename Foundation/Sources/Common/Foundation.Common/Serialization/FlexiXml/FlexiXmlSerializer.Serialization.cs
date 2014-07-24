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
            UpdateElementNameFromType(root, obj.GetType(), options, cx);
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
                     where a.Name == options.UniqueIdAttributeName
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

            //# track source reference
            var id =
                cx.Objects_InstanceIdTracker.GetOrAdd(
                source,
                (_) =>
                {
                    isNewReference = true;

                    return new InstanceId(cx.GetNextUniqueId());
                });

            //# source has been serialized before
            if (!isNewReference)
            {
                var idAttrib = new XAttribute(options.UniqueIdReferenceAttributeName, id.Id);

                id.IncrementReferenceCount();

                target.Add(idAttrib);

                return;
            }

            //# Serialize list items

            var sourceArray = source as Array;
            var sourceList = source as IList;

            if(sourceArray != null)
            {
                throw new NotSupportedException("Array serialization is not supported at the moment.");
            }
            else if (sourceList != null)
            {
                foreach (var item in sourceList)
                {
                    var itemType = (Type)null;

                    if (item == null)
                    {
                        itemType = sourceList.GetItemsTypes().First();

                        // if item type is nullable, use underlying nullable type as item type
                        var nullableItemType = Nullable.GetUnderlyingType(itemType);
                        if (nullableItemType != null)
                            itemType = nullableItemType;

                        var itemElement = new XElement("temp");
                        
                        UpdateElementNameFromType(itemElement, itemType, options, cx);

                        target.Add(itemElement);
                    }
                    else
                    {
                        itemType = item.GetType();

                        var item_as_string = (string)null;

                        if (TryConvertToStringIfTypeSupports(item, out item_as_string))
                        {
                            var item_string_value_element = new XElement("temp", item_as_string);
                            UpdateElementNameFromType(item_string_value_element, item.GetType(), options, cx);
                            target.Add(item_string_value_element);
                            continue;
                        }

                        var itemElement = new XElement("temp");
                        UpdateElementNameFromType(itemElement, itemType,    options, cx);
                        SerializeInternal(itemElement, item, typeDescriptor, options, cx);

                        target.Add(itemElement);
                    }
                }
            }

            var typeDescription = typeDescriptor.DescribeType(source.GetType());

            //# get all source members that can be serialized
            var serializableMembers =
                (from m in typeDescription.Members
                 where
                    m.Visibility == MemberVisibility.Public // process public members only
                    &&
                    !m.IsExplicitInterfaceImplementation // do not process explicit interface implemntations
                    &&
                    m.CanGetValue
                 select m);

            foreach (var m in serializableMembers)
            {
                var val = m.GetValue(source);

                if (val == null) // todo: may need to specify a custom way of handling this
                    continue;

                var val_as_string = (string)null;

                if (m.CanSetValue)
                {
                    if (TryConvertToStringIfTypeSupports(val, out val_as_string))
                    {
                        var attributeName = m.Name;                        

                        target.Add(new XAttribute(attributeName, val_as_string)); // todo: do mapping if needed, + conversion
                        continue;
                    }
                    else
                    {
                        var wrapper = new XElement(target.Name + "." + m.Name);

                        var el = new XElement("temp");
                        UpdateElementNameFromType(el, val.GetType(), options, cx);
                        SerializeInternal(el, val, typeDescriptor, options, cx);

                        wrapper.Add(el);

                        target.Add(wrapper);
                    }
                }
                else
                {
                    if (val is IList)
                    {
                        var el = new XElement(target.Name + "." + m.Name);
                        SerializeInternal(el, val, typeDescriptor, options, cx);
                        target.Add(el);
                    }
                }
            }

            target.Add(new XAttribute(options.UniqueIdAttributeName, id.Id));

            target.AddAnnotation(typeDescription);
        }

        void UpdateElementNameFromType(XElement xel, Type type, SerializationOptions options, SerializationContext cx)
        {
            if(type.IsGenericType)
            {
                var genericArgumentsSeparator_Index = type.Name.IndexOf("`");

                xel.Name = type.Name.Substring(0, genericArgumentsSeparator_Index);

                var ns = type.Namespace;

                //cx.ClrNamespaceToNamespaceDelcarationMappings.GetOrAdd(
                //    ns,
                //    _ =>
                //    {
                //        var nsDeclarationAttribute = new XAttribute(XNamespace.Xmlns.GetName("serialization"), XmlNamespace);
                //        root.Add(nsDeclarationAttribute);
                //    });

                //var nsAttrib = new XAttribute(NamespaceAttributeName, type.FullName);
                //xel.Add(typeAttrib);
            }
            else if(type.IsArray)
            {

            }
            else
            {
                xel.Name = type.Name;
            }
        }

        public XElement Serialize<T>(IEnumerable<T> items, string rootElementName, Func<T, XElement> getItemElement)
        {
            var root = new XElement(rootElementName);

            foreach(var item in items)
            {
                root.Add(getItemElement(item));
            }

            return root;
        }
    }
}
