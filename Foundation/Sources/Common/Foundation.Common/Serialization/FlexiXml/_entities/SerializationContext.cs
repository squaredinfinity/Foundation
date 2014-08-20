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
    class SerializationContext
    {
        public ITypeDescriptor TypeDescriptor { get; private set; }

        public readonly ConcurrentDictionary<object, InstanceId> Objects_InstanceIdTracker = 
            new ConcurrentDictionary<object, InstanceId>();

        public readonly ConcurrentDictionary<string, XAttribute> ClrNamespaceToNamespaceDelcarationMappings = 
            new ConcurrentDictionary<string, XAttribute>();

        long LastUsedUniqueId = 0;

        public long GetNextUniqueId()
        {
            return Interlocked.Increment(ref LastUsedUniqueId);
        }

        public SerializationContext(ITypeDescriptor typeDescriptor)
        {
            this.TypeDescriptor = typeDescriptor;
        }
    }

    public interface ITypeSerializationContext
    {

    }

    public interface ISerializationResolverContext
    {
        XElement CurrentElement { get; }
        object CurrentInstance { get; }

        XElement RootElement { get; }
        object RootInstance { get; }

        XElement Serialize(object obj, string rootElementName);

        string ConstructRootRelementForType(Type type);
    }

    public class SerializationResolverContext : ISerializationResolverContext
    {

        public XElement CurrentElement { get; private set; }

        public object CurrentInstance { get; private set; }

        public XElement RootElement { get; private set; }

        public object RootInstance { get; private set; }

        FlexiXmlSerializer Serializer { get; set; }
        SerializationContext SerializationContext { get; set; }
        SerializationOptions SerializationOptions { get; set; }

        SerializationResolverContext(
            FlexiXmlSerializer serializer,
            SerializationContext serializationContext,
            SerializationOptions serializationOptions,

            XElement currentElement, 
            object currentInstance, 
            XElement rootElement,
            object rootInstance)
        {
            this.Serializer = serializer;
            this.SerializationContext = serializationContext;
            this.SerializationOptions = serializationOptions;

            this.CurrentElement = currentElement;
            this.CurrentInstance = currentInstance;
            this.RootElement = rootElement;
            this.RootInstance = rootInstance;
        }


        public XElement Serialize(object obj, string rootElementName)
        {
            var xe = new XElement(rootElementName);

            Serializer.SerializeInternal(xe, obj, SerializationOptions, SerializationContext);

            return xe;
        }


        public string ConstructRootRelementForType(Type type)
        {
            return Serializer.ConstructRootElementForType(type);
        }
    }
}
