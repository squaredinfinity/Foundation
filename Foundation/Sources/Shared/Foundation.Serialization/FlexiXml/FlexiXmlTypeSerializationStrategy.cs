using SquaredInfinity.Foundation.Types.Description;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using SquaredInfinity.Foundation.Extensions;
using System.Diagnostics;

namespace SquaredInfinity.Foundation.Serialization.FlexiXml
{
    public class FlexiXmlTypeSerializationStrategy<T> : FlexiXmlTypeSerializationStrategy<FlexiXmlTypeSerializationStrategy<T>, T>
    {
        public FlexiXmlTypeSerializationStrategy(FlexiXmlSerializer serializer, Type type, Types.Description.ITypeDescriptor typeDescriptor)
            : base(serializer, type, typeDescriptor)
        { }
    }

    public class FlexiXmlTypeSerializationStrategy<T1, T> : TypeSerializationStrategy<T1, T>, IFlexiXmlTypeSerializationStrategy
        where T1 : class, ITypeSerializationStrategy<T1, T>
    {
        public FlexiXmlTypeSerializationStrategy(FlexiXmlSerializer serializer, Type type, Types.Description.ITypeDescriptor typeDescriptor)
            : base(serializer, type, typeDescriptor)
        { }

        public XElement Serialize(object instance, IFlexiXmlSerializationContext cx, out bool hasAlreadyBeenSerialized)
        {
            return Serialize(instance, cx, rootElementName: null, hasAlreadyBeenSerialized: out hasAlreadyBeenSerialized);
        }

        public virtual XElement Serialize(object instance, IFlexiXmlSerializationContext cx, string rootElementName, out bool hasAlreadyBeenSerialized)
        {
            if (instance == null)
                throw new ArgumentNullException("instance");

            bool isNewReference = false;

            //# track source reference
            var id =
                cx.Objects_InstanceIdTracker.GetOrAdd(
                instance,
                (_) =>
                {
                    isNewReference = true;

                    return new InstanceId(cx.GetNextUniqueId());
                });

            hasAlreadyBeenSerialized = !isNewReference;

            var strategy = (IFlexiXmlTypeSerializationStrategy) cx.GetTypeSerializationStrategy(instance.GetType());
            var result_el = (XElement)null;

            //# source has been serialized before
            if (!isNewReference)
            {
                var idAttrib = new XAttribute(cx.Options.UniqueIdReferenceAttributeName, id.Id);

                id.IncrementReferenceCount();
                
                var result_el_name = strategy.ConstructElementNameForType(instance.GetType());
                result_el = new XElement(result_el_name);

                result_el.Add(idAttrib);

                return result_el;
            }

            var instance_type = instance.GetType();

            //# Construct element to which instance content will be serialized
            var el_name = rootElementName;
            
            if(el_name == null)
                el_name = ConstructElementNameForType(instance_type);
            
            var el = new XElement(el_name);

            var item_as_string = (string)null;

            //# Check if instance type supports conversion to and from string
            if (TryConvertToStringIfTypeSupports(instance.GetType(), instance, out item_as_string))
            {
                el.Add(new XText(item_as_string));

                return el;
            }

            var typeInformation = cx.Options.TypeInformation;

            if (typeInformation == TypeInformation.LookupOnly)
            {
                cx.TryAddKnownType(el, instance_type);
            }
            else if(typeInformation == TypeInformation.Detailed)
            {
                throw new NotSupportedException("TypeInformation.Detailed is not supported");

                var typeAttribute = new XAttribute(cx.Options.TypeHintAttributeName, "alias");
                el.Add(typeAttribute);
            }

            //# Process all content to be serialized (this may include members but also custom content to be added to serialization output)
            foreach(var r in ActualMembersSerializationStrategies.Values)
            {
                if (r.SerializationOption == MemberSerializationOption.ImplicitIgnore || r.SerializationOption == MemberSerializationOption.ExplicitIgnore)
                    continue;

                var serializedMember = (XObject)null;
                if (TrySerializeMember(cx, el, instance, r.Strategy, out serializedMember))
                {
                    el.Add(serializedMember);
                }
            }

            //el.Add(new XAttribute(cx.Options.UniqueIdAttributeName, id.Id));
            el.AddAnnotation(id);

            return el;
        }

        protected virtual bool TrySerializeMember(
            IFlexiXmlSerializationContext cx, 
            XElement parentElement, 
            object parentInstance,
            IMemberSerializationStrategy strategy,
            out XObject serializedContent)
        {
            serializedContent = null;

            if (strategy.ShouldSerializeMember != null)
            {
                var shouldSerialize = strategy.ShouldSerializeMember(parentInstance);

                if (!shouldSerialize)
                {
                    return false;
                }
            }

            var memberValue = strategy.GetValue(parentInstance);

            //# check if member instance has been serialized before

            var instanceId = (InstanceId) null;

            var hasBeenSerializedBefore = false;

            if (memberValue != null)
            {
                var memberValueType = memberValue.GetType();

                var isNonPublic = !memberValueType.IsNested && !memberValueType.IsPublic;
                var isNonPublicNested = memberValueType.IsNested && !memberValueType.IsNestedPublic;

                if((isNonPublic || isNonPublicNested) && !cx.Options.SerializeNonPublicTypes)
                {
                    return false;
                }

                hasBeenSerializedBefore = 
                    cx.Objects_InstanceIdTracker.TryGetValue(memberValue, out instanceId);
            }

            var val_as_string = (string)null;

            if(hasBeenSerializedBefore)
            {
                // add to parent element as ref attribute

                var refAttributeName = strategy.MemberDescription.Name + cx.Options.UniqueIdReferenceAttributeSuffix;

                var idrefAttribute = new XAttribute(refAttributeName, value: "");

                var instanceIdRef = new InstanceIdRef(instanceId);

                idrefAttribute.AddAnnotation(instanceIdRef);

                serializedContent = idrefAttribute;

                return true;
            }
            
                if(strategy.CustomDeserialize != null && strategy.CustomSerialize != null)
                {
                    var xo_str = strategy.CustomSerialize(parentInstance, memberValue);

                    var attributeName = strategy.MemberDescription.Name;
                    var value = xo_str;

                    var attributeEl = new XAttribute(attributeName, value);

                    serializedContent = attributeEl;
                    return true;
                }

            else if (memberValue == null)
            {
                var attributeName = strategy.MemberDescription.Name;
                var value = cx.Options.NullAttributeMarkupExtension;

                var attributeEl = new XAttribute(attributeName, value);

                serializedContent = attributeEl;
                return true;


                // NOTE:    Code below is no longer used.
                //          It used to serialize null values to an element
                //          Example:
                //          <Object>
                //              <Object.PropertyOne>
                //                  <NULL serialization:null="true" />
                //              </Object.PropertyOne/>
                //          </Object>
                //
                //          but default behavior has been changed to serialize members to an xml attribute with markup extension
                //          (over 50% more space efficient)
                //          Example:
                //          <Object propertyOne="{serialization:null}" />

                // value is null
                //
                // => create wrapper element
                // => add serialization:null attribute to the element

                //var wrapperElementName = parentElement.Name + "." + strategy.MemberDescription.Name;
                //var wrapper = new XElement(wrapperElementName);

                //var nullEl = cx.Serialize(memberValue);

                //wrapper.Add(nullEl);

                //serializedContent = wrapper;
                //return true;
            }
                else if (TryConvertToStringIfTypeSupports(strategy.MemberDescription.MemberType.Type, memberValue, out val_as_string))
                {
                    // member value supports converting to and from string
                    //
                    // => create attribute which will store member value (if value is a single line)
                    // => create attached property which will store member value (if value is multi-line)

                    if (val_as_string.Contains(Environment.NewLine))
                    {
                        var wrapperElementName = SanitizeParentElementName(parentElement) + "." + strategy.MemberDescription.Name;
                        var wrapperElement = new XElement(wrapperElementName);

                        wrapperElement.Add(new XCData(val_as_string));

                        serializedContent = wrapperElement;
                    }
                    else
                    {
                        var attributeName = strategy.MemberDescription.Name;

                        var attributeEl = new XAttribute(attributeName, val_as_string);

                        serializedContent = attributeEl;
                    }
                    return true;
                }
                else
                {
                    // member value must be serialized
                    //
                    // => create wrapper element
                    // => add serialized member data
                    //
                    // member wrapper is an attached element with name <parent_element_name.member_name>

                    var wrapperElementName = SanitizeParentElementName(parentElement) + "." + strategy.MemberDescription.Name;

                    var memberType = strategy.MemberDescription.MemberType.Type;

                    //bool canCreateInstance = memberType.IsClass && !memberType.IsAbstract; // should probably check for public constructor (?)

                    //if(!strategy.CanSetValue() && !canCreateInstance)
                    //{
                    //    var childEl = cx.Serialize(memberValue, wrapperElementName);

                    //    return childEl;
                    //}
                    //else
                    {
                        var wrapperElement = new XElement(wrapperElementName);

                        var childEl = cx.Serialize(memberValue);

                        wrapperElement.Add(childEl);

                        serializedContent = wrapperElement;
                        return true;
                    }
                }
        }

        public T1 SerializeMemberAsAttribute<TMember>(
            System.Linq.Expressions.Expression<Func<T, object>> memberExpression,
            Func<T, bool> shouldSerializeMember,
            Func<T, TMember, string> serialize,
            Func<XAttribute, TMember> deserialize)
        {
            var memberName = memberExpression.GetAccessedMemberName();

            var si = (MemberSerializationStrategyInfo)null;

            if (ActualMembersSerializationStrategies.TryGetValue(memberName, out si))
            {
                if (shouldSerializeMember != null)
                {
                    si.Strategy.ShouldSerializeMember = new Func<object, bool>((_target) =>
                    {
                        var local = shouldSerializeMember;
                        return local((T)_target);
                    });
                }

                si.Strategy.CustomSerialize = new Func<object, object, string>((owner, member) => serialize((T)owner, (TMember)member));
                si.Strategy.CustomDeserialize = new Func<object, object>(attrib => deserialize((XAttribute)attrib));

                si.SerializationOption = MemberSerializationOption.ExplicitSerialize;
            }
            else
            {
                throw new NotImplementedException();
            }

            return this as T1;
        }

        string SanitizeParentElementName(XElement parentElement)
        {
            string sanitizedParentName = string.Empty;

            var parentDotIndex = parentElement.Name.LocalName.LastIndexOf('.');

            if (parentDotIndex >= 0)
            {
                sanitizedParentName = parentElement.Name.LocalName.Substring(parentDotIndex + 1); // +1 to remove the '.' too
            }
            else
            {
                sanitizedParentName = parentElement.Name.LocalName;
            }

            return sanitizedParentName;
        }

        protected virtual bool TryDeserializeMember(
            XElement parentElement, 
            object parentInstance,
            IMemberSerializationStrategy strategy,
            IFlexiXmlSerializationContext cx,
            out object memberInstance)
        {
            var memberTypeCandidate = (Type)null;

            memberInstance = null;

            if (strategy.MemberDescription != null)
            {
                memberTypeCandidate = strategy.MemberDescription.MemberType.Type;
                memberInstance = strategy.GetValue(parentInstance);
            }

            //# Find attached property for member
            var memberAttachedProperty = parentElement.FindAttachedElement(strategy.MemberName, isCaseSensitive: false);

            if(memberAttachedProperty != null)
            {
                if(memberTypeCandidate == typeof(string))
                {
                    var cdata = memberAttachedProperty.Nodes().OfType<XCData>().FirstOrDefault();

                    if(cdata != null)
                    {
                        memberInstance = cdata.Value;
                        return true;
                    }
                }

                var memberAttachedProeprtyInnerText = memberAttachedProperty.InnerText();

                //# deserialize from attached elment (XML VALUE)
                //      - no wrapper element, just xml value
                //      - target will be convertible from string
                //      - member type candidate must be known or member instance already present
                //
                //  e.g. <Root.SquenceNumber>13</Root.SequenceNumber>

                if (memberTypeCandidate != null && !memberAttachedProperty.HasElements && !memberAttachedProeprtyInnerText.IsNullOrEmpty())
                {
                    // todo: when member instance is already present

                    if (DeserializationOutcome.Success == 
                        cx.TryDeserialize(
                            memberAttachedProperty, 
                            memberTypeCandidate, 
                            elementNameMayContainTargetTypeName: false, 
                            deserializedInstance: out memberInstance))
                    {
                        return true;
                    }
                }

                //# deserialize collection from attached element (DIRECT)
                //      - member instance must already be present and is IEnumerable
                //      - member is read-only
                //      - no xml value should be present, but there should be child elements
                //
                //  <Root.Items id="13">                    -- Items property on Root type must be read-only
                //      <Items.Version>7</Items.Version>
                //      <Item1 />
                //      <Item2 />
                //  </Root.Items>

                //if (!strategy.CanSetValue())
                //{
                //    if (memberAttachedProeprtyInnerText.IsNullOrEmpty())
                //    {
                //        bool canCreateInstance = 
                //            strategy.MemberDescription.MemberType.Type.IsClass && 
                //            !strategy.MemberDescription.MemberType.Type.IsAbstract; // should probably check for public constructor (?)

                //        if (!canCreateInstance)
                //        {
                //            if (memberInstance != null && memberInstance is IEnumerable)
                //            {
                //                cx.Deserialize(memberAttachedProperty, memberInstance);
                //                return true;
                //            }

                //            if (memberInstance == null)
                //            {
                //                // we cannot assign deserialized instance
                //                // but we still should deserialize it in case it is being referenced somewhere else
                //                memberInstance = cx.Deserialize(memberAttachedProperty, strategy.MemberDescription.MemberType.Type, elementNameMayContainTargetTypeName: true);
                //                return true;
                //            }
                //        }
                //    }
                //}

                //# deserialize from attached element (WRAPPER)
                //      - there must be exactly one child element
                //      - no inner text
                //  
                //  e.g:
                //  <root.member>
                //      <membertype id="13">
                //          <membertype.property>...</membertype.property>
                //      </membertype>
                //  </root.member>

                if (memberAttachedProperty.Elements().Count() == 1 && memberAttachedProeprtyInnerText.IsNullOrEmpty())
                {
                    var memberElement = memberAttachedProperty.Elements().FirstOrDefault();

                    //# handle read-only members
                    if (!strategy.CanSetValue())
                    {
                        if (memberInstance == null)
                        {
                            // we cannot assign deserialized instance
                            // but we still should deserialize it in case it is being referenced somewhere else
                            if (DeserializationOutcome.Success ==
                                cx.TryDeserialize(
                                    memberElement,
                                    memberTypeCandidate,
                                    elementNameMayContainTargetTypeName: true,
                                    deserializedInstance: out memberInstance))
                            {
                                return true;
                            }
                        }
                        else
                        {
                            //# keep track of target instance for future use
                            var id_attrib = memberElement.Attribute(cx.Options.UniqueIdAttributeName);

                            if (id_attrib != null)
                            {
                                var instanceId = new InstanceId(id_attrib.Value);

                                if (!cx.Objects_ById.TryAdd(instanceId, memberInstance))
                                {
                                    // todo: log, this is most likely xml corruption (two elements with same id)
                                    throw new Exception("Unable to update instance reference. Same Instance Id already exists.");
                                }
                            }

                            cx.Deserialize(memberElement, memberInstance);
                        }

                        return true;
                    }
                    else
                    {
                        //# deserialize the element into a new instance
                        if(DeserializationOutcome.Success == 
                            cx.TryDeserialize(
                                memberElement, 
                                memberTypeCandidate, 
                                elementNameMayContainTargetTypeName: true, 
                                deserializedInstance: out memberInstance))
                        {
                            return true;
                        }
                    }
                }

                return false;
            }
            
            //# look for member reference attribute (membername.ref by default)

            var memberReferenceAttributeName = strategy.MemberName + cx.Options.UniqueIdReferenceAttributeSuffix;

            var memberReferenceAttribute =
                (from a in parentElement.Attributes()
                 where string.Equals(a.Name.LocalName, memberReferenceAttributeName, StringComparison.InvariantCultureIgnoreCase)
                 select a).FirstOrDefault();

            if(memberReferenceAttribute != null)
            {
                var instanceId = new InstanceId(memberReferenceAttribute.Value);

                var target = (object)null;

                if (cx.Objects_ById.TryGetValue(instanceId, out target))
                {
                    memberInstance = target;
                    return true;
                }
                else
                {
                    // todo: log, this should always resolve unless serialization xml is corrupted
                    throw new InvalidOperationException();
                }
            }

            var memberAttribute =
                (from a in parentElement.Attributes()
                 where  a.Name.Namespace == System.Xml.Linq.XNamespace.None 
                        && 
                        string.Equals(a.Name.LocalName, strategy.MemberName, StringComparison.InvariantCultureIgnoreCase)
                 select a).FirstOrDefault();

            if(memberAttribute != null)
            {
                //# deserialize from attribute

                //# use custom deserialziation
                if (strategy.CustomDeserialize != null)
                {
                    memberInstance = strategy.CustomDeserialize(memberAttribute);
                    return true;
                }

                if (cx.Options.IsAttributeValueSerializationExtension(memberAttribute))
                {
                    if(memberAttribute.Value == cx.Options.NullAttributeMarkupExtension)
                    {
                        memberInstance = null;
                        return true;
                    }
                }

                if(TryConvertFromStringIfTypeSupports(memberAttribute.Value, memberTypeCandidate, out memberInstance))
                {
                    return true;
                }

                // todo: log a warning, member cannot be deserialized from attribute
                throw new Exception($"Member {memberAttribute.Name} cannot be deserialized fro attribute");
            }

            // TODO: log warning, member cannot be deserialized because couldn't find it
            memberInstance = null;
            return false;
        }

        // todo: in theory this can take This.Type instead of passing one in
        public virtual string ConstructElementNameForType(Type type)
        {
            if (type.IsGenericType)
            {
                var genericArgumentsSeparator_Index = type.Name.IndexOf("`");

                // it is possible that type is generic by does not have ' in name (e.g. KeyCollection, Keys property on Dictionary<T1,T2>)
                if (genericArgumentsSeparator_Index == -1)
                    return type.Name;
                else
                {
                    var name = type.Name.Substring(0, genericArgumentsSeparator_Index);

                    return name;
                }

                //var ns = type.Namespace;

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
            else if (type.IsArray)
            {
                throw new NotSupportedException();
            }
            else
            {
                return type.Name;
            }
        }
        
        public virtual object Deserialize(
            XElement xml, 
            Type targetType,
            IFlexiXmlSerializationContext cx)
        {
            var target = (object)null;

            //# Check Serialization Control Attributes

            // ID REF Attribute is used to indicate that this element should point to the instance identified by id_ref attribute

            var id_ref_attrib = xml.Attribute(cx.Options.UniqueIdReferenceAttributeName);

            if (id_ref_attrib != null)
            {
                var instanceId = new InstanceId(id_ref_attrib.Value);

                if (cx.Objects_ById.TryGetValue(instanceId, out target))
                {
                    return target;
                }
                else
                {
                    // todo: log, this should always resolve unless serialization xml is corrupted
                    throw new InvalidOperationException();
                }
            }

            //# try to deserialize from xml value (inner text)
            //
            // e.g. <Int32>13</Int32>

            var innerText = xml.InnerText();

            if(!xml.HasElements && !innerText.IsNullOrEmpty() && TryConvertFromStringIfTypeSupports(innerText, targetType, out target))
            {
                return target;
            }

            var createinstance_cx = new CreateInstanceContext();

            target = cx.CreateInstance(targetType, createinstance_cx);

            if(target == null)
            {
                // todo: unable to create instance of the type
                return target;
            }

            //if(cx.IsFullyConstruct)

            //# keep track of target instance for future use
            var id_attrib = xml.Attribute(cx.Options.UniqueIdAttributeName);

            if (id_attrib != null)
            {
                var instanceId = new InstanceId(id_attrib.Value);

                if (!cx.Objects_ById.TryAdd(instanceId, target))
                {
                    // todo: log, this is most likely xml corruption (two elements with same id)
                    throw new Exception("Unable to update instance reference. Same Instance Id already exists.");
                }
            }

            Deserialize(xml, target, cx);

            return target;
        }

        public virtual void Deserialize(
            XElement xml, 
            object target,
            IFlexiXmlSerializationContext cx)
        {
            //# Process all members to be deserialized
            foreach (var r in ActualMembersSerializationStrategies.Values)
            {
                var memberInstance = (object)null;

                if (TryDeserializeMember(xml, target, r.Strategy, cx, out memberInstance))
                {
                    // if member cannot be set then TryDeserializeMember will reuse existing instance
                    // it will still return true because serialization was succesfull,
                    // but the resulting instance should not be applied

                    if(r.Strategy.CanSetValue())
                        r.Strategy.SetValue(target, memberInstance);
                }
            }
        }
    }

    //public class ArrayTypeSerializationStrategy : TypeSerializationStrategy
    //{
    //    protected override void Initialize()
    //    {
    //        // do not call base implementation
    //        // by default we will not serialize members of an array
    //        // (unless explicity overriden by user at later stage)
    //    }

    //    public override XElement Serialize(object instance, ITypeSerializationContext cx)
    //    {
    //        var el = base.Serialize(instance, cx);            

    //        //# Construct element to which instance content will be serialized
    //        var el_name = cx.ConstructElementNameForType(instance.GetType());
    //        var el = new XElement(el_name);

    //        var resolverContext =
    //            new SerializationResolverContext(
    //                cx.Serializer,
    //                cx.SerializationContext,
    //                cx.SerializationOptions,
    //                el,
    //                instance);

    //        //# Process all content to be serialized (this may include members but also custom content to be added to serialization output)
    //        foreach (var r in ContentSerializationStrategies)
    //        {
    //            var x = r.Serialize(resolverContext);

    //            if (x != null)
    //                el.Add(x);
    //        }

    //        //# Handle Array types

    //        var array = instance as Array;
    //        var list = instance as IList;

    //        //if(sourceArray != null)
    //        //{
    //        //    throw new NotSupportedException("Array serialization is not supported at the moment.");
    //        //}
    //        //else if (sourceList != null)
    //        //{
    //        //    foreach (var item in sourceList)
    //        //    {
    //        //        var itemType = (Type)null;

    //        //        if (item == null)
    //        //        {
    //        //            itemType = sourceList.GetCompatibleItemsTypes().First();

    //        //            // if item type is nullable, use underlying nullable type as item type
    //        //            var nullableItemType = Nullable.GetUnderlyingType(itemType);
    //        //            if (nullableItemType != null)
    //        //                itemType = nullableItemType;

    //        //            var itemElementName = ConstructRootElementForType(itemType);
    //        //            var itemElement = new XElement(itemElementName);

    //        //            result.Add(itemElement);
    //        //        }
    //        //        else
    //        //        {
    //        //            itemType = item.GetType();

    //        //            var item_as_string = (string)null;

    //        //            if (TryConvertToStringIfTypeSupports(item, out item_as_string))
    //        //            {
    //        //                var item_string_value_element_name = ConstructRootElementForType(item.GetType());
    //        //                var item_string_value_element = new XElement(item_string_value_element_name, item_as_string);
    //        //                result.Add(item_string_value_element);
    //        //                continue;
    //        //            }

    //        //            var itemElementName = ConstructRootElementForType(itemType);
    //        //            var itemElement = new XElement(itemElementName);
    //        //            SerializeInternal(itemElement, item, options, cx);

    //        //            result.Add(itemElement);
    //        //        }
    //        //    }
    //        //}

    //        return el;
    //    }
        
    //}
}
