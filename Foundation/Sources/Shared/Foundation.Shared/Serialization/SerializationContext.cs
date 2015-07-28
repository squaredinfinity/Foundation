using SquaredInfinity.Foundation.Collections;
using SquaredInfinity.Foundation.Types.Description;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SquaredInfinity.Foundation.Serialization
{
    /// <summary>
    /// Serialization Context sotres state used by serializer during serialization.
    /// </summary>
    public abstract class SerializationContext : ISerializationContext
    {
        public TypeResolver TypeResolver { get; private set; }

        public ITypeDescriptor TypeDescriptor { get; private set; }

        public IFlexiSerializer Serializer { get; private set; }

        /// <summary>
        /// Instance of root object being serialized
        /// </summary>
        public object RootInstance { get; internal set; }
        
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

        long LastUsedUniqueId = 0;

        public long GetNextUniqueId()
        {
            return Interlocked.Increment(ref LastUsedUniqueId);
        }

        Func<Type, CreateInstanceContext, object> CustomCreateInstanceWith { get; set; }

        public SerializationContext(
            IFlexiSerializer serializer,
            ITypeDescriptor typeDescriptor,
            TypeResolver typeResolver,
            Func<Type, CreateInstanceContext, object> createInstanceWith = null)
        {
            this.Serializer = serializer;
            this.TypeDescriptor = typeDescriptor;
            this.TypeResolver = typeResolver;
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

        public ITypeSerializationStrategy GetTypeSerializationStrategy(Type type)
        {
            return Serializer.GetTypeSerializationStrategy(type);
        }
    }
}
