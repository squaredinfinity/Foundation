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

        object DeserializeInternal(Type targetType, XElement xml, ITypeDescriptor typeDescriptor, SerializationOptions options, DeserializationContext cx)
        {
            var typeDescription = typeDescriptor.DescribeType(targetType);

            object target = (object)null;

            //# check serialization control attributes first

            var id_ref_attrib = xml.Attribute(UniqueIdReferenceAttributeName);

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


            //# Get a new instance of target
            if(!targetType.IsValueType && !TryCreateInstace(targetType, new CreateInstanceContext(), out target))
            {
                //todo: log error
                return null;
            }            

            //# keep track of target instance for future use
            var id_attrib = xml.Attribute(UniqueIdAttributeName);

            if(id_attrib != null)
            {
                var id = Int64.Parse(id_attrib.Value);

                cx.Objects_InstanceIdTracker.AddOrUpdate(new InstanceId(id), target);
            }

            var value = (object)null;

            //# Construct target from Element Value
            if (!xml.HasElements && xml.Value != null && TryConvertFromStringIfTypeSupports(xml.Value, targetType, out value))
            {
                return value;
            }

            //# Map element attributes to target memebers
            foreach (var attribute in xml.Attributes())
            {
                var member =
                    (from f in typeDescription.Members
                     where 
                     !f.IsExplicitInterfaceImplementation
                     && f.Name == attribute.Name
                     && f.CanSetValue 
                     && f.CanGetValue
                     select f).FirstOrDefault();

                if (member == null)
                    continue;

                var memberType = Type.GetType(member.AssemblyQualifiedMemberTypeName);

                value = (object)null;

                if (TryConvertFromStringIfTypeSupports(attribute.Value, memberType, out value))
                {
                    member.SetValue(target, value);
                }
            }

            //# Process target List
            if (targetType.ImplementsInterface<IList>())
            {
                DeserializeList(xml, target as IList, typeDescriptor, options, cx);
            }

            //# Process attached elements which map to target members
            foreach (var el in xml.AttachedElements())
            {
                var elName = el.Name.LocalName;

                if(el.IsAttached())
                {
                    elName = elName.Substring(elName.IndexOf(".") + 1);
                }

                var member =
                    (from m in typeDescription.Members
                     where
                     !m.IsExplicitInterfaceImplementation
                     && m.Name == elName
                     select m).FirstOrDefault();

                if (member == null)
                {
                    // todo: log ?
                    continue;
                }

                var memberType = Type.GetType(member.AssemblyQualifiedMemberTypeName);

                if (member.CanSetValue)
                {
                    if (!el.HasElements && !el.Value.IsNullOrEmpty())
                    {
                        throw new NotImplementedException();
                        // not supported at the moment
                        // for element to have value, mapped member would need to be convertable to string
                        // but members convertable to string are serialized to attributes by default
                    }
                    else
                    {
                        // element should have exactly one non-attached sub-element
                        // which acts like a wrapper

                        var wrapper =
                            (from xel in el.Elements()
                             where !xel.IsAttached()
                             select xel).Single();

                        var typeAttribute = (XAttribute)null;// wrapper.Attribute(TypeAttributeName);

                        var elementType = (Type)null;

                        if (typeAttribute != null)
                        {
                            elementType =
                                TypeResolver.ResolveType(
                                typeAttribute.Value,
                                ignoreCase: true);
                        }
                        else
                        {
                            elementType =
                                    TypeResolver.ResolveType(
                                    wrapper.Name.LocalName,
                                    ignoreCase: true,
                                    baseTypes: new List<Type> { memberType });
                        }


                        if (elementType == null)
                        {
                            // type could not be resolver
                            // todo: log
                            continue;
                        }

                        value = DeserializeInternal(elementType, wrapper, typeDescriptor, options, cx);

                        member.SetValue(target, value);
                    }
                }
                else
                {
                    var list = member.GetValue(target) as IList;

                    if(list != null)
                    {
                        DeserializeList(el, list, typeDescriptor, options, cx);
                    }
                }
            }

            return target;
        }

        void DeserializeList(
            XElement xml,
            IList targetList, 
            ITypeDescriptor typeDescriptor,
            SerializationOptions options,
            DeserializationContext cx)
        {
            var targetListItemTypes = targetList.GetItemsTypes();

            var non_attached_elements =
                (from el in xml.Elements()
                 where !el.IsAttached()
                 select el).ToArray();

            for (int i = 0; i < non_attached_elements.Length; i++)
            {
                var itemEl = non_attached_elements[i];

                var itemType = ResolveType(itemEl, targetListItemTypes, options, cx);


                if (itemType == null)
                {
                    // todo: log
                    continue;
                }

                var item = DeserializeInternal(itemType, itemEl, typeDescriptor, options, cx);

                targetList.Add(item);
            }
        }

        Type ResolveType(XElement el, IReadOnlyList<Type> baseTypes, SerializationOptions options, DeserializationContext cx)
        {
            var namespaceAttribute = el.Attributes(NamespaceAttributeName).FirstOrDefault();

            var ns = (XAttribute) null;

            //if (namespaceAttribute != null && cx.ClrNamespaceToNamespaceDelcarationMappings.TryGetValue(namespaceAttribute.Value, out ns))
            //{
            //    var clr_namespace_parts = ns.Value.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);

            //    var clr_namespace = clr_namespace_parts[0];

            //    var clr_assembly = (string) null;

            //    if (clr_namespace_parts.Length > 1)
            //        clr_assembly = clr_namespace_parts[1];                

            //    var t =
            //        TypeExtensions.ResolveType(
            //        clr_namespace + el.Name.LocalName,
            //        ignoreCase: true,
            //        baseTypes: baseTypes);

            //    return t;
            //}
            //else
            {
                var t =
                    TypeResolver.ResolveType(
                    el.Name.LocalName,
                    ignoreCase: true,
                    baseTypes: baseTypes);

                return t;
            }

            return null;
        }
    }
}
