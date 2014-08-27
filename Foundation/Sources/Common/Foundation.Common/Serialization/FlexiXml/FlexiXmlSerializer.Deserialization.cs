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
        public T Deserialize<T>(string xml)
        {
            return Deserialize<T>(xml, new SerializationOptions());
        }

        public T Deserialize<T>(string xml, SerializationOptions options)
        {
            var xDoc = XDocument.Parse(xml);

            return Deserialize<T>(xDoc, options);
        }

        public T Deserialize<T>(XDocument xml)
        {
            return Deserialize<T>(xml, new SerializationOptions());
        }

        public T Deserialize<T>(XDocument xml, SerializationOptions options)
        {
            return Deserialize<T>(xml.Root, options);
        }

        public T Deserialize<T>(XElement xml)
        {
            return Deserialize<T>(xml, new SerializationOptions());
        }

        public T Deserialize<T>(XElement xml, SerializationOptions options)
        {
            if (options == null)
                options = new SerializationOptions();

            var cx = new SerializationContext(this, TypeDescriptor, TypeResolver, options, CustomCreateInstanceWith);

            var type = typeof(T);

            var root = cx.Deserialize(xml, type, elementNameMayContainTargetTypeName: true);
            
            return (T)root;
        }

        //object DeserializeInternal(
        //    Type targetType, 
        //    XElement xml, 
        //    ITypeDescriptor typeDescriptor,
        //    SerializationOptions options, 
        //    DeserializationContext cx)
        //{
        //    throw new NotImplementedException();

        //    //var targetTypeDescription = typeDescriptor.DescribeType(targetType);

        //    ////# Construct target from Element Value
        //    //if (!xml.HasElements && xml.Value != null && TryConvertFromStringIfTypeSupports(xml.Value, targetType, out value))
        //    //{
        //    //    return value;
        //    //}

        //    ////# Get a new instance of target
        //    //if(!targetType.IsValueType && !TryCreateInstace(targetType, new CreateInstanceContext(), out target))
        //    //{
        //    //    //todo: log error
        //    //    return null;
        //    //}            

        //    ////# keep track of target instance for future use
        //    //var id_attrib = xml.Attribute(options.UniqueIdAttributeName);

        //    //if(id_attrib != null)
        //    //{
        //    //    var instanceId = new InstanceId(id_attrib.Value);
                
        //    //    cx.Objects_InstanceIdTracker.AddOrUpdate(instanceId, target);
        //    //}

        //    //DeserializeInternal(target, targetTypeDescription, targetType, xml, typeDescriptor, options, cx);

        //    //return target;
        //}
        
        //void DeserializeInternal(
        //    object target,
        //    ITypeDescription targetTypeDescription,
        //    Type targetType, 
        //    XElement xml, 
        //    ITypeDescriptor typeDescriptor, 
        //    SerializationOptions options, 
        //    DeserializationContext cx)
        //{
        //    throw new NotImplementedException();

        //    //var memberMappingCandidateAttributes =
        //    //    (from a in xml.Attributes()
        //    //     where !a.Name.LocalName.EndsWith(options.UniqueIdReferenceAttributeSuffix)
        //    //            && a.Name.Namespace == XNamespace.None
        //    //     select a);

        //    ////# Map element attributes to target memebers
        //    //foreach (var attribute in memberMappingCandidateAttributes)
        //    //{
        //    //    var member =
        //    //        (from f in targetTypeDescription.Members
        //    //         where 
        //    //         !f.IsExplicitInterfaceImplementation
        //    //         && string.Equals(f.Name, attribute.Name.LocalName, StringComparison.InvariantCultureIgnoreCase)
        //    //         && f.CanSetValue 
        //    //         && f.CanGetValue
        //    //         select f).FirstOrDefault();

        //    //    if (member == null)
        //    //    {
        //    //        if (attribute.Name != options.UniqueIdAttributeName && attribute.Name != options.UniqueIdReferenceAttributeName)
        //    //        {
        //    //            // log warning
        //    //        }
        //    //    }
        //    //    else
        //    //    {
        //    //        var memberType = member.MemberType.Type;

        //    //        var value = (object)null;

        //    //        if (TryConvertFromStringIfTypeSupports(attribute.Value, memberType, out value))
        //    //        {
        //    //            member.SetValue(target, value);
        //    //        }
        //    //    }
        //    //}

        //    //var propertyReferenceAttributes =
        //    //   (from a in xml.Attributes()
        //    //    where a.Name.LocalName.EndsWith(options.UniqueIdReferenceAttributeSuffix)
        //    //    && a.Name.Namespace == XNamespace.None
        //    //    select a);

        //    ////# Resolve references of id ref attributes (e.g. someProperty.ref="xxx")
        //    //foreach (var attribute in propertyReferenceAttributes)
        //    //{
        //    //    var attribLocalName = attribute.Name.LocalName;

        //    //    var memberName = attribLocalName.Substring(0, attribLocalName.Length - options.UniqueIdReferenceAttributeSuffix.Length);

        //    //    var member =
        //    //        (from f in targetTypeDescription.Members
        //    //         where
        //    //         !f.IsExplicitInterfaceImplementation
        //    //         && string.Equals(f.Name, memberName, StringComparison.InvariantCultureIgnoreCase)
        //    //         && f.CanSetValue
        //    //         && f.CanGetValue
        //    //         select f).FirstOrDefault();

        //    //    var instanceId = new InstanceId(attribute.Value);

        //    //    object referenced_instance = null;

        //    //    if (cx.Objects_InstanceIdTracker.TryGetValue(instanceId, out referenced_instance))
        //    //    {
        //    //        member.SetValue(target, referenced_instance);
        //    //    }
        //    //    else
        //    //    {
        //    //        // todo: log, this should always resolve unless serialization xml is corrupted
        //    //    }
        //    //}

        //    ////# Process target List
        //    //if (targetType.ImplementsInterface<IList>())
        //    //{
        //    //    DeserializeList(xml, target as IList, typeDescriptor, options, cx);
        //    //}
        //    //else
        //    //{
        //    //    // not a list, it may contain Child Elements (non-attached) which should be mapped to properties

        //    //    foreach(var el in xml.Elements())
        //    //    {
        //    //        if (el.IsAttached())
        //    //            continue;

        //    //        DeserializeMemberFromElement(el, target, targetTypeDescription, typeDescriptor, options, cx);
        //    //    }
        //    //}

        //    ////# Process attached elements which map to target members
        //    //foreach (var el in xml.AttachedElements())
        //    //{
        //    //    DeserializeMemberFromElement(el, target, targetTypeDescription, typeDescriptor, options, cx);
        //    //}
        //}

        //void DeserializeMemberFromElement(
        //    XElement el,
        //    object target, 
        //    ITypeDescription 
        //    targetTypeDescription, 
        //    ITypeDescriptor typeDescriptor,
        //    SerializationOptions options, 
        //    DeserializationContext cx)
        //{
        //    var elName = el.Name.LocalName;

        //    if (el.IsAttached())
        //    {
        //        elName = elName.Substring(elName.IndexOf(".") + 1);
        //    }

        //    // try to find member

        //    var member =
        //        (from m in targetTypeDescription.Members
        //         where
        //         !m.IsExplicitInterfaceImplementation
        //         && string.Equals(m.Name, elName, StringComparison.InvariantCultureIgnoreCase)
        //         select m).FirstOrDefault();

        //    if (member == null)
        //    {
        //        // todo: log ?
        //        return;
        //    }

        //    // ID REF Attribute is used to indicate that this element should point to the instance identified by id_ref attribute
        //    var id_ref_attrib = el.Attribute(options.UniqueIdReferenceAttributeName);

        //    if (id_ref_attrib != null)
        //    {
        //        var instanceId = new InstanceId(id_ref_attrib.Value);

        //        var referencedValue = (object) null;

        //        if (cx.Objects_InstanceIdTracker.TryGetValue(instanceId, out referencedValue))
        //        {
        //            member.SetValue(target, referencedValue);
        //            return;
        //        }
        //        else
        //        {
        //            // todo: log, this should always resolve unless serialization xml is corrupted
        //            return;
        //        }
        //    }

        //    var memberType = member.MemberType.Type;

        //    var memberValue = member.GetValue(target);

        //    if (!el.HasElements && !el.Value.IsNullOrEmpty())
        //    {
        //        // no child elements, just a string value
        //        // try to convert this string value to a member value

        //        if (member.CanSetValue)
        //        {
        //            var value = DeserializeInternal(memberType, el, typeDescriptor, options, cx);

        //            member.SetValue(target, value);
        //            return;
        //        }
        //        else
        //        {
        //            // log warning
        //            return;
        //        }
        //    }
        //    else
        //    {
        //        //var typeAttribute = (XAttribute)null;// wrapper.Attribute(TypeAttributeName);
        //        //        //    var elementType = (Type)null;
        //        //        //    if (typeAttribute != null)
        //        //        //    {
        //        //        //        elementType =
        //        //        //            TypeResolver.ResolveType(
        //        //        //            typeAttribute.Value,
        //        //        //            ignoreCase: true);
        //        //        //    }

        //        var childElements =
        //            (from xel in el.Elements()
        //             where !xel.IsAttached()
        //             select xel).ToList();

        //            if (childElements.Count == 1)
        //            {
        //                if (TryUpdateMemberFromPotentialWrapperElement(target, memberType, member, childElements.Single(), typeDescriptor, options, cx))
        //                    return;
        //            }

        //            if (memberValue == null)
        //            {
        //                if (member.CanSetValue)
        //                {
        //                    if (memberType.IsAbstract || memberType.IsInterface)
        //                    {
        //                        // log warning
        //                        return;
        //                    }

        //                    var value = DeserializeInternal(memberType, el, typeDescriptor, options, cx);
        //                    member.SetValue(target, value);
        //                    return;
        //                }
        //                else
        //                {
        //                    // cannot set value and existing value is null, log warning
        //                    return;
        //                }
        //            }
        //            else
        //            {
        //                // use type of actual member value instead of member type
        //                memberType = memberValue.GetType();

        //                var memberTypeDescription = typeDescriptor.DescribeType(memberType);

        //                DeserializeInternal(memberValue, memberTypeDescription, memberType, el, typeDescriptor, options, cx);
        //            }
        //        }
        //}

        //bool TryUpdateMemberFromPotentialWrapperElement(
        //    object target,
        //    Type memberType,
        //    ITypeMemberDescription member,
        //    XElement wrapperCandidate,
        //    ITypeDescriptor typeDescriptor,
        //    SerializationOptions options,
        //    DeserializationContext cx)
        //{

        //    var wrapperCandidateType =
        //        TypeResolver.ResolveType(
        //        wrapperCandidate.Name.LocalName,
        //        ignoreCase: true,
        //        baseTypes: new Type[] { memberType });

        //    if (wrapperCandidateType != null && memberType.IsAssignableFrom(wrapperCandidateType))
        //    {
        //        // this is a wrapper, deserialize it and assign
        //        var value = DeserializeInternal(wrapperCandidateType, wrapperCandidate, typeDescriptor, options, cx);

        //        if (member.CanSetValue)
        //        {
        //            member.SetValue(target, value);

        //            return true;
        //        }
        //        else
        //        {
        //            if (!member.CanGetValue)
        //                return false;

        //            var existingValue = member.GetValue(target);

        //            if (existingValue == null)
        //                return false;

        //            // todo: map value
        //        }
        //    }

        //    return false;
        //}

        //void DeserializeList(
        //    XElement xml,
        //    IList targetList, 
        //    ITypeDescriptor typeDescriptor,
        //    SerializationOptions options,
        //    DeserializationContext cx)
        //{
        //    var targetListItemTypes = targetList.GetCompatibleItemsTypes();

        //    var non_attached_elements =
        //        (from el in xml.Elements()
        //         where !el.IsAttached()
        //         select el).ToArray();

        //    for (int i = 0; i < non_attached_elements.Length; i++)
        //    {
        //        var itemEl = non_attached_elements[i];

        //        var itemType = ResolveType(itemEl, targetListItemTypes, options, cx);


        //        if (itemType == null)
        //        {
        //            // todo: log
        //            continue;
        //        }

        //        var item = DeserializeInternal(itemType, itemEl, typeDescriptor, options, cx);

        //        targetList.Add(item);
        //    }
        //}

        //Type ResolveType(XElement el, IReadOnlyList<Type> baseTypes, SerializationOptions options, DeserializationContext cx)
        //{
        //    var namespaceAttribute = el.Attributes(options.NamespaceAttributeName).FirstOrDefault();

        //    var ns = (XAttribute) null;

        //    //if (namespaceAttribute != null && cx.ClrNamespaceToNamespaceDelcarationMappings.TryGetValue(namespaceAttribute.Value, out ns))
        //    //{
        //    //    var clr_namespace_parts = ns.Value.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);

        //    //    var clr_namespace = clr_namespace_parts[0];

        //    //    var clr_assembly = (string) null;

        //    //    if (clr_namespace_parts.Length > 1)
        //    //        clr_assembly = clr_namespace_parts[1];                

        //    //    var t =
        //    //        TypeExtensions.ResolveType(
        //    //        clr_namespace + el.Name.LocalName,
        //    //        ignoreCase: true,
        //    //        baseTypes: baseTypes);

        //    //    return t;
        //    //}
        //    //else
        //    {
        //        var t =
        //            TypeResolver.ResolveType(
        //            el.Name.LocalName,
        //            ignoreCase: true,
        //            baseTypes: baseTypes);

        //        return t;
        //    }

        //    return null;
        //}
    }
}
