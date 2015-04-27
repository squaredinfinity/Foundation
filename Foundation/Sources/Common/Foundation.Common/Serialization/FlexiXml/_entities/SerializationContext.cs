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
    public class ReferenceEqualityComparer : IEqualityComparer<object>
    {
        bool IEqualityComparer<object>.Equals(object x, object y)
        {
            if (x == null)
                return false;

            if (y == null)
                return false;

            return object.ReferenceEquals(x, y);
        }

        int IEqualityComparer<object>.GetHashCode(object obj)
        {
            if (obj == null)
                return int.MinValue;

            return obj.GetHashCode();
        }
    }

    /// <summary>
    /// Serialization Context sotres state used by serializer during serialization.
    /// </summary>
    public class SerializationContext : ISerializationContext, IFlexiXmlSerializationContext
    {
        public TypeResolver TypeResolver { get; private set; }

        IXmlSerializer Serializer { get; set; }

        public ITypeDescriptor TypeDescriptor { get; private set; }

        /// <summary>
        /// Instance of root object being serialized
        /// </summary>
        public object RootInstance { get; internal set; }

        public XElement RootElement { get; internal set; }

        public SerializationOptions Options { get; internal set ; }

        readonly ConcurrentDictionary<object, InstanceId> _objects_InstanceIdTracker = 
            new ConcurrentDictionary<object, InstanceId>(new ReferenceEqualityComparer());

        public ConcurrentDictionary<object, InstanceId> Objects_InstanceIdTracker
        {
            get { return _objects_InstanceIdTracker; }
        }

        readonly ConcurrentDictionary<InstanceId, object> _objects_ById =
            new ConcurrentDictionary<InstanceId, object>();

        public ConcurrentDictionary<InstanceId, object> Objects_ById
        {
            get { return _objects_ById; }
        }

        //public readonly ConcurrentDictionary<string, XAttribute> ClrNamespaceToNamespaceDelcarationMappings = 
        //    new ConcurrentDictionary<string, XAttribute>();

        long LastUsedUniqueId = 0;

        public long GetNextUniqueId()
        {
            return Interlocked.Increment(ref LastUsedUniqueId);
        }

        Func<Type, CreateInstanceContext, object> CustomCreateInstanceWith { get; set; }

        internal SerializationContext(
            IXmlSerializer serializer, 
            ITypeDescriptor typeDescriptor, 
            TypeResolver typeResolver,
            SerializationOptions options,
            Func<Type, CreateInstanceContext, object> createInstanceWith = null)
        {
            this.Serializer = serializer;
            this.TypeDescriptor = typeDescriptor;
            this.TypeResolver = typeResolver;
            this.Options = options;
            this.CustomCreateInstanceWith = createInstanceWith;
        }

        public object CreateInstance(Type type, CreateInstanceContext cx)
        {
            if (CustomCreateInstanceWith != null)
            {
                var instance = CustomCreateInstanceWith(type, cx);
                
                return instance;
            }
            else
            {
                var targetTypeDescription = TypeDescriptor.DescribeType(type);

                var instance = targetTypeDescription.CreateInstance();

                return instance;
            }
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

        public ITypeSerializationStrategy GetTypeSerializationStrategy(Type type)
        {
            return Serializer.GetTypeSerializationStrategy(type);
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
