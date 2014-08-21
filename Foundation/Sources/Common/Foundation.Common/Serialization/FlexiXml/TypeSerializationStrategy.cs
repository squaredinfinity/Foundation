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

        protected virtual void Initialize()
        {
            //# get all source members that can be serialized
            var serializableMembers =
                (from m in TypeDescription.Members
                 where
                    m.Visibility == MemberVisibility.Public // process public members only
                    &&
                    !m.IsExplicitInterfaceImplementation // do not process explicit interface implemntations (for now, may need to change this behavior later, main problem is how to support this?)
                    &&
                    m.CanGetValue // we must be able to get a value of a member to serialize it
                 select m);

            //# create default strategies for each serializable member

            foreach (var member in serializableMembers)
            {
                var strategy = CreateSerializationStrategyForMember(member);

                ContentSerializationStrategies.Add(strategy);
            }
        }

        protected virtual IMemberSerializationStrategy CreateSerializationStrategyForMember(ITypeMemberDescription member)
        {
            var strategy = 
                new MemberSerializationStrategy(member);

            return strategy;
        }

        public virtual XElement Serialize(object instance, ITypeSerializationContext cx)
        {
            if (instance == null)
                throw new ArgumentNullException("instance");

            //# Construct element to which instance content will be serialized
            var el_name = ConstructElementNameForType(instance.GetType());
            
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

            return el;
        }

        protected virtual XObject SerializeMember(
            ITypeSerializationContext cx, 
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
                // => add xsi:nil attribute to the element

                var wrapperElementName = parentElement.Name + "." + strategy.MemberDescription.Name;

                var nullEl = new XElement(wrapperElementName);

                nullEl.Add(new XAttribute(cx.Options.NullValueAttributeName, true));

                return nullEl;
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
                // member wriapper is an attached element with name <parent_element_name.member_name>

                var wrapperElementName = parentElement.Name + "." + strategy.MemberDescription.Name;
                var wrapperElement = new XElement(wrapperElementName);

                var childEl = cx.SerializationContext.Serialize(memberValue);

                wrapperElement.Add(childEl);

                return wrapperElement;
            }
        }

        protected virtual bool TryDeserializeMember(
            XElement parentElement, 
            object parentInstance,
            IMemberSerializationStrategy strategy,
            ITypeSerializationContext cx,
            out object memberInstance)
        {
            var memberTypeCandidate = (Type)null;

            if (strategy.MemberDescription != null)
                memberTypeCandidate = strategy.MemberDescription.MemberType.Type;

            //# Find attached property for member
            var memberAttachedProperty = parentElement.FindAttachedElement(strategy.MemberName, isCaseSensitive: false);

            if(memberAttachedProperty != null)
            {
                //# deserialize from attached elment

                var memberElement = memberAttachedProperty.Elements().FirstOrDefault();

                // todo: check if member has serialization attribute suggesting which type to deserialize to

                // todo: check if there's only Value and no child elements / attributes, try to convert from string value

                //# try to derive member type from element name
                var suggestedMemberTypeName = memberElement.Name.LocalName;

                if(!string.Equals(suggestedMemberTypeName, memberTypeCandidate.Name))
                {
                    //# try to find type with suggested name
                    // todo:

                    throw new NotImplementedException();
                }

                // maybe this is not important at the moment
                //var memberInstance = strategy.GetValue(parentInstance);
                //if(memberInstance == null || memberInstance.GetType() != memberTypeCandidate)
                //{
                //    //# create new instance of member
                //    // TODO: get type description and create instance using it
                //    memberInstance = Activator.CreateInstance(memberTypeCandidate);
                //}

                // deserialize the element into the instance
                memberInstance = cx.SerializationContext.Deserialize(memberElement, memberTypeCandidate);

                return true;
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

            //var val_as_string = (string)null;

            //if (memberValue == null)
            //{
            //    // value is null
            //    //
            //    // => create wrapper element
            //    // => add xsi:nil attribute to the element

            //    var wrapperElementName = parentElement.Name + "." + strategy.MemberName;

            //    var nullEl = new XElement(wrapperElementName);

            //    nullEl.Add(new XAttribute(cx.Options.NullValueAttributeName, true));

            //    return nullEl;
            //}
            //else if (TryConvertToStringIfTypeSupports(memberValue, out val_as_string))
            //{
            //    // member value supports converting to and from string
            //    //
            //    // => create attribute which will store member value

            //    // todo:    what if conversion to string produces text which isn't valid for attribute value?
            //    //          should it be stored in CDATA element instead?
            //    //          or perhaps escaped? or configurable?
            //    var attributeName = strategy.MemberName;

            //    var attributeEl = new XAttribute(attributeName, val_as_string);

            //    return attributeEl;
            //}
            //else
            //{
            //    // member value must be serialized
            //    //
            //    // => create wrapper element
            //    // => add serialized member data
            //    //
            //    // member wriapper is an attached element with name <parent_element_name.member_name>

            //    var wrapperElementName = parentElement.Name + "." + strategy.MemberName;
            //    var wrapperElement = new XElement(wrapperElementName);

            //    var childEl = cx.SerializationContext.Serialize(memberValue);

            //    wrapperElement.Add(childEl);

            //    return wrapperElement;
            //}
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

        public object Deserialize(XElement xml, object target, ITypeSerializationContext cx)
        {           

            //# Construct target from Element Value
            //
            //  Type which are convertible to / from string are serialized as <typename>string_representation</typename>

            //if (!xml.HasElements && xml.Value != null && TryConvertFromStringIfTypeSupports(xml.Value, targetType, out value))
            //{
            //    return value;
            //}

            ////# Construct element to which instance content will be serialized
            //var el_name = ConstructElementNameForType(instance.GetType());

            //var el = new XElement(el_name);

            //var item_as_string = (string)null;

            ////# Check if instance type supports conversion to and from string
            //if (TryConvertToStringIfTypeSupports(instance, out item_as_string))
            //{
            //    el.Add(new XText(item_as_string));

            //    return el;
            //}

            //# Process all members to be deserialized
            foreach (var r in ContentSerializationStrategies)
            {
                var memberInstance = (object)null;
                
                if(TryDeserializeMember(xml, target, r, cx, out memberInstance))
                {
                    r.SetValue(target, memberInstance);
                }
            }

            return target;
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

        public override XElement Serialize(object instance, ITypeSerializationContext cx)
        {
            var el = base.Serialize(instance, cx);

            var list = instance as IEnumerable;

            foreach (var item in list)
            {
                var el_item = cx.SerializationContext.Serialize(item);

                el.Add(el_item);
            }

            return el;
        }

        //protected override XObject SerializeMember(
        //    ITypeSerializationContext cx,
        //    XElement parentElement,
        //    object parentInstance,
        //    IMemberSerializationStrategy strategy)
        //{


        //    var memberValue = strategy.GetValue(parentInstance);

        //    if (!strategy.CanSetValue() && memberValue is IList)
        //    {
        //        // cannot set value of a member 
        //        // (so the type does not matter, it will probably be initialized to correct type by object itself)
        //        // and member is a list
        //        //
        //        // => create wrapper element for the list
        //        // => serialize elements of the list

        //        // list wrapper is an attached element with name: <parent_element_name.member_name>
        //        // list wrapper for read-only property contains elements directly (as its children)

        //        var wrapperElementName = parentElement.Name + "." + strategy.MemberName;

        //        var xe = cx.SerializationContext.Serialize(memberValue);

        //        var wrapper = new XElement(wrapperElementName);
        //        wrapper.Add(xe);

        //        return wrapper;
        //    }
        //    else
        //    {
        //        // member can be set (so we do care about the actual type of its value)
        //        //
        //        // => create a wrapper element for the member
        //        // => create child element (child of the wrapper) which contains serialized data

        //        // member wrapper is an attached element with name <parent_element_name.member_name>

        //        var wrapperElementName = parentElement.Name + "." + strategy.MemberName;

        //        var el = cx.SerializationContext.Serialize(memberValue);

        //        var wrapper = new XElement(wrapperElementName);
        //        wrapper.Add(el);

        //        return wrapper;
        //    }

        //    return base.SerializeMember(cx, parentElement, parentInstance, strategy);
        //}
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
