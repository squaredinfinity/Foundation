//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace SquaredInfinity.Foundation.Serialization.FlexiPlainText
//{
//    public class FlexiPlainTextTypeSerializationStrategy<T> : TypeSerializationStrategy<T>
//    {
//        public FlexiPlainTextTypeSerializationStrategy(FlexiPlainTextSerializer serializer, Type type, Types.Description.ITypeDescriptor typeDescriptor)
//            : base(serializer, type, typeDescriptor)
//        { }
        
//        public virtual string Serialize(object instance, FlexiPlainTextSerializationContext cx, out bool hasAlreadyBeenSerialized)
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

//            var strategy = (IFlexiXmlTypeSerializationStrategy) cx.GetTypeSerializationStrategy(instance.GetType());
//            var result_el = (XElement)null;

//            //# source has been serialized before
//            if (!isNewReference)
//            {
//                var idAttrib = new XAttribute(cx.Options.UniqueIdReferenceAttributeName, id.Id);

//                id.IncrementReferenceCount();
                
//                var result_el_name = strategy.ConstructElementNameForType(instance.GetType());
//                result_el = new XElement(result_el_name);

//                result_el.Add(idAttrib);

//                return result_el;
//            }

//            var instance_type = instance.GetType();

//            //# Construct element to which instance content will be serialized
//            var el_name = rootElementName;
            
//            if(el_name == null)
//                el_name = ConstructElementNameForType(instance_type);
            
//            var el = new XElement(el_name);

//            var item_as_string = (string)null;

//            //# Check if instance type supports conversion to and from string
//            if (TryConvertToStringIfTypeSupports(instance.GetType(), instance, out item_as_string))
//            {
//                el.Add(new XText(item_as_string));

//                return el;
//            }

//            var typeInformation = cx.Options.TypeInformation;

//            if (typeInformation == TypeInformation.LookupOnly)
//            {
//                cx.TryAddKnownType(el, instance_type);
//            }
//            else if(typeInformation == TypeInformation.Detailed)
//            {
//                throw new NotSupportedException("TypeInformation.Detailed is not supported");

//                var typeAttribute = new XAttribute(cx.Options.TypeHintAttributeName, "alias");
//                el.Add(typeAttribute);
//            }

//            //# Process all content to be serialized (this may include members but also custom content to be added to serialization output)
//            foreach(var r in ActualContentSerializationStrategies)
//            {
//                var serializedMember = (XObject)null;
//                if (TrySerializeMember(cx, el, instance, r, out serializedMember))
//                {
//                    el.Add(serializedMember);
//                }
//            }

//            //el.Add(new XAttribute(cx.Options.UniqueIdAttributeName, id.Id));
//            el.AddAnnotation(id);

//            return el;
//        }

//        protected virtual bool TrySerializeMember(
//            FlexiPlainTextSerializationContext cx, 
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

//            var instanceId = (InstanceId) null;

//            var hasBeenSerializedBefore = false;

//            if (memberValue != null)
//            {
//                var memberValueType = memberValue.GetType();

//                var isNonPublic = !memberValueType.IsNested && !memberValueType.IsPublic;
//                var isNonPublicNested = memberValueType.IsNested && !memberValueType.IsNestedPublic;

//                if((isNonPublic || isNonPublicNested) && !cx.Options.SerializeNonPublicTypes)
//                {
//                    return false;
//                }

//                hasBeenSerializedBefore = 
//                    cx.Objects_InstanceIdTracker.TryGetValue(memberValue, out instanceId);
//            }

//            var val_as_string = (string)null;

//            if(hasBeenSerializedBefore)
//            {
//                // add to parent element as ref attribute

//                var refAttributeName = strategy.MemberDescription.Name + cx.Options.UniqueIdReferenceAttributeSuffix;

//                var idrefAttribute = new XAttribute(refAttributeName, value: "");

//                var instanceIdRef = new InstanceIdRef(instanceId);

//                idrefAttribute.AddAnnotation(instanceIdRef);

//                serializedContent = idrefAttribute;

//                return true;
//            }
            
//                if(strategy.CustomDeserialize != null && strategy.CustomSerialize != null)
//                {
//                    var xo_str = strategy.CustomSerialize(parentInstance, memberValue);

//                    var attributeName = strategy.MemberDescription.Name;
//                    var value = xo_str;

//                    var attributeEl = new XAttribute(attributeName, value);

//                    serializedContent = attributeEl;
//                    return true;
//                }

//            else if (memberValue == null)
//            {
//                var attributeName = strategy.MemberDescription.Name;
//                var value = cx.Options.NullAttributeMarkupExtension;

//                var attributeEl = new XAttribute(attributeName, value);

//                serializedContent = attributeEl;
//                return true;


//                // NOTE:    Code below is no longer used.
//                //          It used to serialize null values to an element
//                //          Example:
//                //          <Object>
//                //              <Object.PropertyOne>
//                //                  <NULL serialization:null="true" />
//                //              </Object.PropertyOne/>
//                //          </Object>
//                //
//                //          but default behavior has been changed to serialize members to an xml attribute with markup extension
//                //          (over 50% more space efficient)
//                //          Example:
//                //          <Object propertyOne="{serialization:null}" />

//                // value is null
//                //
//                // => create wrapper element
//                // => add serialization:null attribute to the element

//                //var wrapperElementName = parentElement.Name + "." + strategy.MemberDescription.Name;
//                //var wrapper = new XElement(wrapperElementName);

//                //var nullEl = cx.Serialize(memberValue);

//                //wrapper.Add(nullEl);

//                //serializedContent = wrapper;
//                //return true;
//            }
//                else if (TryConvertToStringIfTypeSupports(strategy.MemberDescription.MemberType.Type, memberValue, out val_as_string))
//                {
//                    // member value supports converting to and from string
//                    //
//                    // => create attribute which will store member value (if value is a single line)
//                    // => create attached property which will store member value (if value is multi-line)

//                    if (val_as_string.Contains(Environment.NewLine))
//                    {
//                        var wrapperElementName = SanitizeParentElementName(parentElement) + "." + strategy.MemberDescription.Name;
//                        var wrapperElement = new XElement(wrapperElementName);

//                        wrapperElement.Add(new XCData(val_as_string));

//                        serializedContent = wrapperElement;
//                    }
//                    else
//                    {
//                        var attributeName = strategy.MemberDescription.Name;

//                        var attributeEl = new XAttribute(attributeName, val_as_string);

//                        serializedContent = attributeEl;
//                    }
//                    return true;
//                }
//                else
//                {
//                    // member value must be serialized
//                    //
//                    // => create wrapper element
//                    // => add serialized member data
//                    //
//                    // member wrapper is an attached element with name <parent_element_name.member_name>

//                    var wrapperElementName = SanitizeParentElementName(parentElement) + "." + strategy.MemberDescription.Name;

//                    var memberType = strategy.MemberDescription.MemberType.Type;

//                    //bool canCreateInstance = memberType.IsClass && !memberType.IsAbstract; // should probably check for public constructor (?)

//                    //if(!strategy.CanSetValue() && !canCreateInstance)
//                    //{
//                    //    var childEl = cx.Serialize(memberValue, wrapperElementName);

//                    //    return childEl;
//                    //}
//                    //else
//                    {
//                        var wrapperElement = new XElement(wrapperElementName);

//                        var childEl = cx.Serialize(memberValue);

//                        wrapperElement.Add(childEl);

//                        serializedContent = wrapperElement;
//                        return true;
//                    }
//                }
//        }
//    }
//}
