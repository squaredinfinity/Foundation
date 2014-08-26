using SquaredInfinity.Foundation.Types.Description;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using SquaredInfinity.Foundation.Extensions;

namespace SquaredInfinity.Foundation.Serialization.FlexiXml
{
    public interface IMemberSerializationStrategy
    {
        ITypeMemberDescription MemberDescription { get; }

        string MemberName { get; }

        bool CanGetValue();
        object GetValue(object memberOwner);

        bool CanSetValue();
        void SetValue(object memberOwner, object newValue);
    }

    public class MemberSerializationStrategy : IMemberSerializationStrategy
    {
        public string MemberName { get; private set; }
        public ITypeMemberDescription MemberDescription { get; private set; }
        
        public MemberSerializationStrategy(ITypeMemberDescription memberDescription)
        {
            this.MemberDescription = memberDescription;
            this.MemberName = MemberDescription.Name;
        }

        public object GetValue(object memberOwner)
        {
            return MemberDescription.GetValue(memberOwner);
        }

        public void SetValue(object memberOwner, object newValue)
        {
            MemberDescription.SetValue(memberOwner, newValue);
        }
        public bool CanGetValue()
        {
            return MemberDescription.CanGetValue;
        }

        public bool CanSetValue()
        {
            return MemberDescription.CanSetValue;
        }
    }

    //public class CustomTypeMemberSerializationStrategy<TOwner, TMember> : ITypeMemberSerializationStrategy
    //{
    //    public string MemberName { get; private set; }

    //    public CustomTypeMemberSerializationStrategy(string memberName, Func<T)
    //    {
    //        this.MemberDescription = memberDescription;
    //        this.MemberName = MemberDescription.Name;
    //    }

    //    public object GetValue(object memberOwner)
    //    {
    //        return MemberDescription.GetValue(memberOwner);
    //    }

    //    public void SetValue(object memberOwner, object newValue)
    //    {
    //        MemberDescription.SetValue(memberOwner, newValue);
    //    }
    //}

    public class TypeSerializationStrategy : ITypeSerializationStrategy
    {
        public Version Version { get; set; }

        public Type Type { get; set; }

        public Types.Description.ITypeDescription TypeDescription { get; set; }

        public Types.Description.ITypeDescriptor TypeDescriptor { get; set; }

        public TypeSerializationStrategy(Type type, Types.Description.ITypeDescriptor typeDescriptor)
        {
            this.Type = type;
            this.TypeDescriptor = typeDescriptor;

            this.TypeDescription = TypeDescriptor.DescribeType(type);

            Initialize();
        }

        readonly List<IMemberSerializationStrategy> _contentSerializationStrategies = 
            new List<IMemberSerializationStrategy>();

        protected List<IMemberSerializationStrategy> ContentSerializationStrategies 
        {
            get { return _contentSerializationStrategies; }
        }

        protected void Initialize()
        {
            //# get all source members that can be serialized
            var serializableMembers =
                (from m in TypeDescription.Members
                 where CanSerializeMember(m)
                 select m);

            //# create default strategies for each serializable member

            foreach (var member in serializableMembers)
            {
                var strategy = CreateSerializationStrategyForMember(member);

                ContentSerializationStrategies.Add(strategy);
            }
        }

        protected virtual bool CanSerializeMember(ITypeMemberDescription member)
        {
            // public members only
            if (member.Visibility != MemberVisibility.Public)
                return false;

            // explicit interface implementations are not supported
            if (member.IsExplicitInterfaceImplementation)
                return false;

            // only members which have getters can be serialized
            if (!member.CanGetValue)
                return false;

            // if member is read-only, serialize it only if it's enumerable and not IReadOnlyList or IReadOnlyCollection
            if (!member.CanSetValue)
            {
                if (!typeof(IEnumerable).IsAssignableFrom(member.MemberType.Type))
                    return false;

                if (member.MemberType.Type.IsGenericType)
                {
                    var genericTypeDefinition = member.MemberType.Type.GetGenericTypeDefinition();

                    if (genericTypeDefinition == typeof(IReadOnlyCollection<>))
                        return false;

                    if (genericTypeDefinition == typeof(IReadOnlyList<>))
                        return false;

                    if (genericTypeDefinition == typeof(IReadOnlyDictionary<,>))
                        return false;
                }
            }

            // read-only members which are not enumerable should not be serialized
            // because they could not be deserialized after
            // for read-only enumerable members there's always a chance th
            //if (!member.CanSetValue && !typeof(IEnumerable).IsAssignableFrom(member.MemberType.Type))
            //    return false;

            return true;
        }

        protected virtual IMemberSerializationStrategy CreateSerializationStrategyForMember(ITypeMemberDescription member)
        {
            var strategy = 
                new MemberSerializationStrategy(member);

            return strategy;
        }

        public XElement Serialize(object instance, ISerializationContext cx)
        {
            return Serialize(instance, cx, rootElementName: null);
        }
        
        public virtual XElement Serialize(object instance, ISerializationContext cx, string rootElementName)
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

            var strategy = cx.GetTypeSerializationStrategy(instance.GetType());
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

            //# Construct element to which instance content will be serialized
            var el_name = rootElementName;
            
            if(el_name == null)
                el_name = ConstructElementNameForType(instance.GetType());
            
            var el = new XElement(el_name);

            var item_as_string = (string)null;

            //# Check if instance type supports conversion to and from string
            if (TryConvertToStringIfTypeSupports(instance, out item_as_string))
            {
                el.Add(new XText(item_as_string));

                return el;
            }

            //# Process all content to be serialized (this may include members but also custom content to be added to serialization output)
            foreach(var r in ContentSerializationStrategies)
            {
                var serializedMember = SerializeMember(cx, el, instance, r);

                el.Add(serializedMember);
            }

            el.Add(new XAttribute(cx.Options.UniqueIdAttributeName, id.Id));
            el.AddAnnotation(id);

            return el;
        }

        protected virtual XObject SerializeMember(
            ISerializationContext cx, 
            XElement parentElement, 
            object parentInstance,
            IMemberSerializationStrategy strategy)
        {
            var memberValue = strategy.GetValue(parentInstance);

            var val_as_string = (string)null;

            if (memberValue == null)
            {
                // value is null
                //
                // => create wrapper element
                // => add serialization:null attribute to the element

                var wrapperElementName = parentElement.Name + "." + strategy.MemberDescription.Name;
                var wrapper = new XElement(wrapperElementName);

                var nullEl = cx.Serialize(memberValue);

                wrapper.Add(nullEl);

                return wrapper;
            }
            else if (TryConvertToStringIfTypeSupports(memberValue, out val_as_string))
            {
                // member value supports converting to and from string
                //
                // => create attribute which will store member value

                // todo:    what if conversion to string produces text which isn't valid for attribute value?
                //          should it be stored in CDATA element instead?
                //          or perhaps escaped? or configurable?
                var attributeName = strategy.MemberDescription.Name;

                var attributeEl = new XAttribute(attributeName, val_as_string);

                return attributeEl;
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

                    return wrapperElement;
                }
            }
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
            ISerializationContext cx,
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

                    memberInstance = cx.Deserialize(memberAttachedProperty, memberTypeCandidate, elementNameMayContainTargetTypeName: false);
                    return true;
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
                            memberInstance = cx.Deserialize(memberElement, memberTypeCandidate, elementNameMayContainTargetTypeName: true);
                            return true;
                            // todo: log warning
                        }
                        else
                        {
                            cx.Deserialize(memberElement, memberInstance);
                        }

                        return true;
                    }
                    else
                    {
                        //# deserialize the element into a new instance
                        memberInstance = cx.Deserialize(memberElement, memberTypeCandidate, elementNameMayContainTargetTypeName: true);

                        return true;
                    }
                }

                return false;
            }
            
            var memberAttribute =
                (from a in parentElement.Attributes()
                 where string.Equals(a.Name.LocalName, strategy.MemberName, StringComparison.InvariantCultureIgnoreCase)
                 select a).FirstOrDefault();

            if(memberAttribute != null)
            {
                //# deserialize from attribute

                if(TryConvertFromStringIfTypeSupports(memberAttribute.Value, memberTypeCandidate, out memberInstance))
                {
                    return true;
                }

                // todo: log a warning, member cannot be deserialized from attribute
                throw new Exception();
            }

            // TODO: log warning, member cannot be deserialized because couldn't find it
            memberInstance = null;
            return false;
        }

        protected virtual bool TryConvertToStringIfTypeSupports(object obj, out string result)
        {
            var typeConverter = System.ComponentModel.TypeDescriptor.GetConverter(obj);

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

        protected virtual bool TryConvertFromStringIfTypeSupports(string stringRepresentation, Type resultType, out object result)
        {
            var typeConverter = System.ComponentModel.TypeDescriptor.GetConverter(resultType);

            if (typeConverter == null || !typeConverter.CanConvertFrom(typeof(string)))
            {
                result = null;
                return false;
            }

            result = typeConverter.ConvertFrom(stringRepresentation);

            return true;
        }

        // todo: in theory this can take This.Type instead of passing one in
        public virtual string ConstructElementNameForType(Type type)
        {
            if (type.IsGenericType)
            {
                var genericArgumentsSeparator_Index = type.Name.IndexOf("`");

                var name = type.Name.Substring(0, genericArgumentsSeparator_Index);

                return name;

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
            ISerializationContext cx)
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
            ISerializationContext cx)
        {
            //# Process all members to be deserialized
            foreach (var r in ContentSerializationStrategies)
            {
                var memberInstance = (object)null;

                if (TryDeserializeMember(xml, target, r, cx, out memberInstance))
                {
                    // if member cannot be set then TryDeserializeMember will reuse existing instance
                    // it will still return true because serialization was succesfull,
                    // but the resulting instance should not be applied

                    if(r.CanSetValue())
                        r.SetValue(target, memberInstance);
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

    public class EnumerableTypeSerializationStrategy : TypeSerializationStrategy
    {
        public EnumerableTypeSerializationStrategy(Type type, ITypeDescriptor typeDescriptor)
            : base(type, typeDescriptor)
        { }

        public override XElement Serialize(object instance, ISerializationContext cx, string rootElementName)
        {
            var el = base.Serialize(instance, cx, rootElementName);

            var list = instance as IEnumerable;

            foreach (var item in list)
            {
                var el_item = cx.Serialize(item);

                el.Add(el_item);
            }

            return el;
        }

        public override void Deserialize(XElement xml, object target, ISerializationContext cx)
        {
            base.Deserialize(xml, target, cx);

            var targetList = target as IList;

            if (targetList != null)
            {
                //# deserialize list elements

                var targetListDefaultElementType = targetList.GetCompatibleItemsTypes().FirstOrDefault();

                var itemElements =
                    (from el in xml.Elements()
                     where !el.IsAttached()
                     select el);

                foreach (var item_el in itemElements)
                {
                    var item = cx.Deserialize(item_el, targetListDefaultElementType, elementNameMayContainTargetTypeName: true);

                    targetList.Add(item);
                }
            }
        }
    }

    public class TypeSerializationStrategy<T> : TypeSerializationStrategy, ITypeSerializationStrategy<T>
    {
        public TypeSerializationStrategy(Types.Description.ITypeDescriptor typeDescriptor)
            : base(typeof(T), typeDescriptor)
        { }

        public ITypeSerializationStrategy<T> IgnoreAllMembers()
        {
            return this;
        }


        public ITypeSerializationStrategy<T> IgnoreMember(System.Linq.Expressions.Expression<Func<object>> memberExpression)
        {
            return this;
        }
        
        public ITypeSerializationStrategy<T> ResolveReferenceWith<TypeToResolve>(Action<ReferenceResolutionContext<T, TypeToResolve>> resolveReference)
        {
            return this;
        }


        public ITypeSerializationStrategy<T> IgnoreMember(System.Linq.Expressions.Expression<Func<T>> memberExpression)
        {
            return this;
        }

        public ITypeSerializationStrategy<T> SerializeMember(System.Linq.Expressions.Expression<Func<T>> memberExpression)
        {
            return this;
        }
    }
}
