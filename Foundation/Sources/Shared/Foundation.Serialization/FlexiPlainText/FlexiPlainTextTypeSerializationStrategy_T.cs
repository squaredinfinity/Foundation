//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace SquaredInfinity.Foundation.Serialization.FlexiPlainText
//{
//    public class FlexiPlainTextTypeSerializationStrategy<T> : FlexiPlainTextTypeSerializationStrategy<FlexiPlainTextTypeSerializationStrategy<T>, T>
//    {
//        public FlexiPlainTextTypeSerializationStrategy(FlexiPlainTextSerializer serializer, Type type, Types.Description.ITypeDescriptor typeDescriptor)
//            : base(serializer, type, typeDescriptor)
//        { }
//    }

//    public class FlexiPlainTextTypeSerializationStrategy<T1, T> : TypeSerializationStrategy<T1, T>, IFlexiPlainTextSerializationStrategy
//        where T1 : class, ITypeSerializationStrategy<T1, T>
//    {
//        public FlexiPlainTextTypeSerializationStrategy(FlexiPlainTextSerializer serializer, Type type, Types.Description.ITypeDescriptor typeDescriptor)
//            : base(serializer, type, typeDescriptor)
//        { }
        
//        public virtual string Serialize(object instance, IFlexiPlainTextSerializationContext cx, out bool hasAlreadyBeenSerialized)
//        {
//            if (instance == null)
//                throw new ArgumentNullException("instance");

//            bool isNewReference = false;

//            //# track source reference
//            var id =
//                cx.Objects_InstanceIdTracker.GetOrAdd(
//                instance,
//                (_) =>
//                {
//                    isNewReference = true;

//                    return new InstanceId(cx.GetNextUniqueId());
//                });

//            hasAlreadyBeenSerialized = !isNewReference;

//            var strategy = (IFlexiPlainTextSerializationStrategy)cx.GetTypeSerializationStrategy(instance.GetType());
//            var result = new StringBuilder();

//            //# source has been serialized before
//            if (!isNewReference)
//            {
//                id.IncrementReferenceCount();

//                result.Append(strategy.GetNameFromType(instance.GetType()));
//                result.Append(".ref=");
//                result.Append(id.Id);
                

//                return result.ToString();
//            }

//            var instance_type = instance.GetType();

//            //# Construct element to which instance content will be serialized
//            var el_name = (string)null;

//            if (el_name == null)
//                el_name = GetNameFromType(instance_type);

//            var item_as_string = (string)null;

//            //# Check if instance type supports conversion to and from string
//            if (TryConvertToStringIfTypeSupports(instance.GetType(), instance, out item_as_string))
//            {
//                result.Append(item_as_string);

//                return result.ToString();
//            }

//            //# Process all content to be serialized (this may include members but also custom content to be added to serialization output)
//            foreach (var r in ActualMembersSerializationStrategies.Values)
//            {
//                if (r.SerializationOption == MemberSerializationOption.ImplicitIgnore || r.SerializationOption == MemberSerializationOption.ExplicitIgnore)
//                    continue;

//                var serializedMember = (string)null;

//                if (TrySerializeMember(cx, 1, instance, r.Strategy, out serializedMember))
//                {
//                    result.AppendLine(serializedMember);
//                }
//            }

//            //el.AddAnnotation(id);

//            return result.ToString();
//        }

//        protected virtual bool TrySerializeMember(
//            IFlexiPlainTextSerializationContext cx,
//            int indent,
//            object parentInstance,
//            IMemberSerializationStrategy strategy,
//            out string serializedContent)
//        {
//            serializedContent = null;

//            if (strategy.ShouldSerializeMember != null)
//            {
//                var shouldSerialize = strategy.ShouldSerializeMember(parentInstance);

//                if (!shouldSerialize)
//                {
//                    return false;
//                }
//            }

//            var memberValue = strategy.GetValue(parentInstance);

//            //# check if member instance has been serialized before

//            var instanceId = (InstanceId)null;

//            var hasBeenSerializedBefore = false;

//            if (memberValue != null)
//            {
//                var memberValueType = memberValue.GetType();

//                var isNonPublic = !memberValueType.IsNested && !memberValueType.IsPublic;
//                var isNonPublicNested = memberValueType.IsNested && !memberValueType.IsNestedPublic;

//                if ((isNonPublic || isNonPublicNested) && !cx.Options.SerializeNonPublicTypes)
//                {
//                    return false;
//                }

//                hasBeenSerializedBefore =
//                    cx.Objects_InstanceIdTracker.TryGetValue(memberValue, out instanceId);
//            }

//            var val_as_string = (string)null;

//            if (hasBeenSerializedBefore)
//            {
//                // add to parent element as ref attribute

//                var refAttributeName = strategy.MemberDescription.Name + cx.Options.UniqueIdReferenceAttributeSuffix;

//                var idrefAttribute = new XAttribute(refAttributeName, value: "");

//                var instanceIdRef = new InstanceIdRef(instanceId);

//                idrefAttribute.AddAnnotation(instanceIdRef);

//                serializedContent = idrefAttribute;

//                return true;
//            }

//            if (strategy.CustomDeserialize != null && strategy.CustomSerialize != null)
//            {
//                var xo_str = strategy.CustomSerialize(parentInstance, memberValue);

//                var attributeName = strategy.MemberDescription.Name;
//                var value = xo_str;

//                var attributeEl = new XAttribute(attributeName, value);

//                serializedContent = attributeEl;
//                return true;
//            }

//            else if (memberValue == null)
//            {
//                var attributeName = strategy.MemberDescription.Name;
//                var value = cx.Options.NullAttributeMarkupExtension;

//                var attributeEl = new XAttribute(attributeName, value);

//                serializedContent = attributeEl;
//                return true;
//            }
//            else if (TryConvertToStringIfTypeSupports(strategy.MemberDescription.MemberType.Type, memberValue, out val_as_string))
//            {
//                // member value supports converting to and from string
//                //
//                // => create attribute which will store member value (if value is a single line)
//                // => create attached property which will store member value (if value is multi-line)

//                if (val_as_string.Contains(Environment.NewLine))
//                {
//                    var wrapperElementName = SanitizeParentElementName(parentElement) + "." + strategy.MemberDescription.Name;
//                    var wrapperElement = new XElement(wrapperElementName);

//                    wrapperElement.Add(new XCData(val_as_string));

//                    serializedContent = wrapperElement;
//                }
//                else
//                {
//                    var attributeName = strategy.MemberDescription.Name;

//                    var attributeEl = new XAttribute(attributeName, val_as_string);

//                    serializedContent = attributeEl;
//                }
//                return true;
//            }
//            else
//            {
//                // member value must be serialized
//                //
//                // => create wrapper element
//                // => add serialized member data
//                //
//                // member wrapper is an attached element with name <parent_element_name.member_name>

//                var wrapperElementName = SanitizeParentElementName(parentElement) + "." + strategy.MemberDescription.Name;

//                var memberType = strategy.MemberDescription.MemberType.Type;

//                //bool canCreateInstance = memberType.IsClass && !memberType.IsAbstract; // should probably check for public constructor (?)

//                //if(!strategy.CanSetValue() && !canCreateInstance)
//                //{
//                //    var childEl = cx.Serialize(memberValue, wrapperElementName);

//                //    return childEl;
//                //}
//                //else
//                {
//                    var wrapperElement = new XElement(wrapperElementName);

//                    var childEl = cx.Serialize(memberValue);

//                    wrapperElement.Add(childEl);

//                    serializedContent = wrapperElement;
//                    return true;
//                }
//            }
//        }

//        public virtual string GetNameFromType(Type type)
//        {
//            if (type.IsGenericType)
//            {
//                var genericArgumentsSeparator_Index = type.Name.IndexOf("`");

//                // it is possible that type is generic by does not have ' in name (e.g. KeyCollection, Keys property on Dictionary<T1,T2>)
//                if (genericArgumentsSeparator_Index == -1)
//                    return type.Name;
//                else
//                {
//                    var name = type.Name.Substring(0, genericArgumentsSeparator_Index);

//                    return name;
//                }
//            }
//            else if (type.IsArray)
//            {
//                throw new NotSupportedException();
//            }
//            else
//            {
//                return type.Name;
//            }
//        }
//    }
//}
