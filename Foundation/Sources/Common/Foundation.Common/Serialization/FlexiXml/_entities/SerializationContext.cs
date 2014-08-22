using SquaredInfinity.Foundation.Types.Description;
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
    public class SerializationContext : ISerializationContext
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
            new ConcurrentDictionary<object, InstanceId>();

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

        internal SerializationContext(
            IXmlSerializer serializer, 
            ITypeDescriptor typeDescriptor, 
            TypeResolver typeResolver,
            SerializationOptions options)
        {
            this.Serializer = serializer;
            this.TypeDescriptor = typeDescriptor;
            this.TypeResolver = typeResolver;
            this.Options = options;
        }
        
        public XElement Serialize(object instance)
        {
            if (instance == null)
            {
                var nullEl = new XElement("NULL");

                nullEl.Add(new XAttribute(Options.NullValueAttributeName, true));

                return nullEl;
            }

            var strategy = GetTypeSerializationStrategy(instance.GetType());

            var result_el = strategy.Serialize(instance, this);

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

            // todo: check if member has serialization attribute suggesting which type to deserialize to

            //# try to derive member type from element name
            var suggestedMemberTypeName = xml.Name.LocalName;

            if (elementNameMayContainTargetTypeName && !string.Equals(suggestedMemberTypeName, targetType.Name))
            {
                //# try to find type with suggested name

                var typeCandidate =
                    TypeResolver.ResolveType(
                    suggestedMemberTypeName,
                    ignoreCase: true,
                    baseTypes: new Type[] { targetType });

                if (typeCandidate != null)
                {
                    targetType = typeCandidate;
                }
            }

            var strategy = GetTypeSerializationStrategy(targetType);

            return strategy.Deserialize(xml, targetType, this);
        }

        public void Deserialize(XElement xml, object target)
        {
            var strategy = GetTypeSerializationStrategy(target.GetType());

            strategy.Deserialize(xml, target, this);
        }

        public ITypeSerializationStrategy GetTypeSerializationStrategy(Type type)
        {
            return Serializer.GetTypeSerializationStrategy(type);
        }
    }
}
