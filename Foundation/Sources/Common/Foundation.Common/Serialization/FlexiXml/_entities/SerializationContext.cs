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
    public interface ISerializationContext
    {
        ITypeDescriptor TypeDescriptor { get; }

        /// <summary>
        /// Instance of root object being serialized
        /// </summary>
        object RootInstance { get; }

        XElement RootElement { get; }

        // todo: this should not be exposed in that way
        ConcurrentDictionary<object, InstanceId> Objects_InstanceIdTracker { get; }

        SerializationOptions Options { get; }
        
        XElement Serialize(object item);
    }

    /// <summary>
    /// Serialization Context sotres state used by serializer during serialization.
    /// </summary>
    public class SerializationContext : ISerializationContext
    {
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

        //public readonly ConcurrentDictionary<string, XAttribute> ClrNamespaceToNamespaceDelcarationMappings = 
        //    new ConcurrentDictionary<string, XAttribute>();

        long LastUsedUniqueId = 0;

        public long GetNextUniqueId()
        {
            return Interlocked.Increment(ref LastUsedUniqueId);
        }

        internal SerializationContext(IXmlSerializer serializer, ITypeDescriptor typeDescriptor, SerializationOptions options)
        {
            this.Serializer = serializer;
            this.TypeDescriptor = typeDescriptor;
            this.Options = options;
        }
        
        public XElement Serialize(object instance)
        {
            if (instance == null)
                throw new ArgumentNullException("instance");

            bool isNewReference = false;

            //# track source reference
            var id =
                Objects_InstanceIdTracker.GetOrAdd(
                instance,
                (_) =>
                {
                    isNewReference = true;

                    return new InstanceId(GetNextUniqueId());
                });

            var strategy = GetTypeSerializationStrategy(instance.GetType());
            var result_el = (XElement)null;

            //# source has been serialized before
            if (!isNewReference)
            {
                var idAttrib = new XAttribute(Options.UniqueIdReferenceAttributeName, id.Id);

                id.IncrementReferenceCount();

                var result_el_name = strategy.ConstructElementNameForType(instance.GetType());
                result_el = new XElement(result_el_name);

                result_el.Add(idAttrib);

                return result_el;
            }

            var type_cx = new TypeSerializationContext(this, null, instance);

            // todo: why pass instance here and not through context ?
            result_el = strategy.Serialize(instance, type_cx);
            
            result_el.Add(new XAttribute(Options.UniqueIdAttributeName, id.Id));

            return result_el;
        }

        public ITypeSerializationStrategy GetTypeSerializationStrategy(Type type)
        {
            return Serializer.GetTypeSerializationStrategy(type);
        }

    }
    
    public interface ITypeSerializationContext
    {
        SerializationOptions Options { get; }
        ISerializationContext SerializationContext { get; }
        object CurrentInstance { get; }
        XElement CurrentElement { get; }

        //string ConstructElementNameForType(Type type);
    }

    public class TypeSerializationContext : ITypeSerializationContext
    {
        public object CurrentInstance { get; private set; }
        public XElement CurrentElement { get; private set; }

        public SerializationOptions Options { get; private set; }
        public ISerializationContext SerializationContext { get; private set; }

        internal TypeSerializationContext(ISerializationContext serializationContext, XElement currentElement, object currentInstance)
        {
            this.SerializationContext = serializationContext;
            this.CurrentElement = currentElement;
            this.CurrentInstance = currentInstance;

            this.Options = SerializationContext.Options;
        }
    }

    public interface ITypePartSerializationContext
    {
        SerializationOptions Options { get; }
        ITypeSerializationContext TypeSerializationContext { get; }
        ISerializationContext SerializationContext { get; }
    }

    public class TypePartSerializationContext : ITypePartSerializationContext
    {
        public SerializationOptions Options { get; private set; }
        public ISerializationContext SerializationContext { get; private set; }
        public ITypeSerializationContext TypeSerializationContext { get; private set; }

        internal TypePartSerializationContext(ITypeSerializationContext typeSerializationContext)
        {
            this.TypeSerializationContext = typeSerializationContext;
            this.SerializationContext = TypeSerializationContext.SerializationContext;
            this.Options = TypeSerializationContext.SerializationContext.Options;
        }
    }
}
