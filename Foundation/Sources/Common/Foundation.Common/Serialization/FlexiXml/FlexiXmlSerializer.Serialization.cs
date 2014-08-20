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
using SquaredInfinity.Foundation.Types.Description.IL;

namespace SquaredInfinity.Foundation.Serialization.FlexiXml
{
    public partial class FlexiXmlSerializer
    {
        public XElement Serialize(object obj)
        {
            return Serialize(obj, rootElementName: null);
        }

        public XElement Serialize(object obj, string rootElementName)
        {
            var cx = new SerializationContext(DefaultTypeDescriptor);

            var options = new SerializationOptions();

            var root = (XElement)null;

            if (rootElementName.IsNullOrEmpty())
            {
                if(obj == null)
                {
                    rootElementName = "NULL";
                }
                else
                {
                    rootElementName = ConstructRootElementForType(obj.GetType());
                }
            }
            
            root = new XElement(rootElementName);

            SerializeInternal(root, obj, options , cx);
            
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
        
        internal void SerializeInternal(XElement target, object source, SerializationOptions options, SerializationContext cx)
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
                        itemType = sourceList.GetCompatibleItemsTypes().First();

                        // if item type is nullable, use underlying nullable type as item type
                        var nullableItemType = Nullable.GetUnderlyingType(itemType);
                        if (nullableItemType != null)
                            itemType = nullableItemType;

                        var itemElementName = ConstructRootElementForType(itemType);
                        var itemElement = new XElement(itemElementName);

                        target.Add(itemElement);
                    }
                    else
                    {
                        itemType = item.GetType();

                        var item_as_string = (string)null;

                        if (TryConvertToStringIfTypeSupports(item, out item_as_string))
                        {
                            var item_string_value_element_name = ConstructRootElementForType(item.GetType());
                            var item_string_value_element = new XElement(item_string_value_element_name, item_as_string);
                            target.Add(item_string_value_element);
                            continue;
                        }

                        var itemElementName = ConstructRootElementForType(itemType);
                        var itemElement = new XElement(itemElementName);
                        SerializeInternal(itemElement, item, options, cx);

                        target.Add(itemElement);
                    }
                }
            }

            var typeDescription = cx.TypeDescriptor.DescribeType(source.GetType());

            var strategy = 
                GetOrCreateTypeSerializationStrategy(
                typeDescription.Type, 
                () => CreateDefaultTypeSerializationStrategy(typeDescription.Type));

            //foreach(var resover in strategy.SerializationResolvers)
            //{
            //    // todo
            //}

            #region OLD

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

                //if (m.CanSetValue)
                {
                    if (TryConvertToStringIfTypeSupports(val, out val_as_string))
                    {
                        var attributeName = m.Name;                        

                        target.Add(new XAttribute(attributeName, val_as_string)); // todo: do mapping if needed, + conversion
                        continue;
                    }
                    else
                    {
                        { 
                            if (!m.CanSetValue && val is IList)
                            {
                                var el = new XElement(target.Name + "." + m.Name);
                                SerializeInternal(el, val, options, cx);
                                target.Add(el);
                            }
                            else
                            {
                                var wrapper = new XElement(target.Name + "." + m.Name);

                                var elName = ConstructRootElementForType(val.GetType());
                                var el = new XElement(elName);

                                SerializeInternal(el, val, options, cx);

                                wrapper.Add(el);

                                target.Add(wrapper);
                            }
                        }
                    }
                }
                //else
                {

                }

            #endregion
            }

            target.Add(new XAttribute(options.UniqueIdAttributeName, id.Id));

            target.AddAnnotation(typeDescription);
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
