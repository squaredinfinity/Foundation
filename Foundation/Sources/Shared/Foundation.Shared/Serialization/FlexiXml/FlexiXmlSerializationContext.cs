using SquaredInfinity.Foundation.Collections;
using SquaredInfinity.Foundation.Types.Description;
using SquaredInfinity.Foundation.Extensions;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace SquaredInfinity.Foundation.Serialization.FlexiXml
{
    /// <summary>
    /// Serialization Context sotres state used by serializer during serialization.
    /// </summary>
    public class FlexiXmlSerializationContext : SerializationContext, IFlexiXmlSerializationContext
    {
        public XElement RootElement { get; internal set; }

        public FlexiXmlSerializationOptions Options { get; internal set ; }

        public FlexiXmlSerializationContext(
            IFlexiSerializer serializer, 
            ITypeDescriptor typeDescriptor, 
            TypeResolver typeResolver,
            FlexiXmlSerializationOptions options,
            Func<Type, CreateInstanceContext, object> createInstanceWith = null)
            : base(serializer, typeDescriptor, typeResolver, createInstanceWith)
        {
            this.Options = options;
        }
        
        public XElement Serialize(object instance)
        {
            return Serialize(instance, rootElementName: null);
        }

        public XElement Serialize(object instance, string rootElementName)
        {
            if (instance == null)
            {
                var nullElementName = rootElementName;
                if (nullElementName == null)
                    nullElementName = "NULL";

                var nullEl = new XElement(nullElementName);

                nullEl.Add(new XAttribute(Options.NullValueAttributeName, true));

                return nullEl;
            }

            var strategy = (IFlexiXmlTypeSerializationStrategy) GetTypeSerializationStrategy(instance.GetType());

            bool hasAlreadyBeenSerialized = false;

            var result_el = strategy.Serialize(instance, this, rootElementName, out hasAlreadyBeenSerialized);

            return result_el;
        }

        public object Deserialize(XElement xml, Type targetType, bool elementNameMayContainTargetTypeName)
        {
            //# check NULL attribute
            var null_attrib = xml.Attribute(Options.NullValueAttributeName);
            if (null_attrib != null && null_attrib.Value != null && null_attrib.Value.ToLower() == "true")
            {
                return null;
            }

            //# check if has only one child with NULL attribute (i.e. wrapped element signifies null)
            if(xml.Descendants().Count() == 1)
            {
                var nullElementCandidate = xml.Descendants().Single();

                null_attrib = nullElementCandidate.Attribute(Options.NullValueAttributeName);
                if (null_attrib != null && null_attrib.Value != null && null_attrib.Value.ToLower() == "true")
                {
                    return null;
                }
            }

            //# try to derive member type from element name
            var suggestedMemberTypeName = xml.Name.LocalName;

            if (elementNameMayContainTargetTypeName && !string.Equals(suggestedMemberTypeName, targetType.Name))
            {
                //# try to find type with suggested name
                var typeCandidate = (Type)null;
                
                //# first look in KnownTypes map
                typeCandidate =
                        TypeResolver.ResolveTypes(
                        candidates: KnownTypes.GetValues(XName.Get(suggestedMemberTypeName)).EmptyIfNull(),
                        baseTypes: new Type[] { targetType }).FirstOrDefault();


                //# then try to find in loaded assemblies (slower than lookup)
                if (typeCandidate == null)
                {
                    typeCandidate =
                        TypeResolver.ResolveType(
                        suggestedMemberTypeName,
                        ignoreCase: true,
                        baseTypes: new Type[] { targetType });
                }

                if (typeCandidate != null)
                {
                    targetType = typeCandidate;
                }
            }

            var strategy = (IFlexiXmlTypeSerializationStrategy) GetTypeSerializationStrategy(targetType);

            return strategy.Deserialize(xml, targetType, this);
        }

        public void Deserialize(XElement xml, object target)
        {
            var strategy = (IFlexiXmlTypeSerializationStrategy) GetTypeSerializationStrategy(target.GetType());

            strategy.Deserialize(xml, target, this);
        }

        MultiMap<XName, Type> _knownTypes = new MultiMap<XName, Type>();
        public MultiMap<XName, Type> KnownTypes
        {
            get { return _knownTypes; }
        }

        public bool TryAddKnownType(XElement el, Type type)
        {
            return KnownTypes.Add(el.Name, type);
        }
    }
}
