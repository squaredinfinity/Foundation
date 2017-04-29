using SquaredInfinity.Types.Description;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Serialization
{
    /// <summary>
    /// Serialization Context holds state information of current serialization / deserialization operation
    /// </summary>
    public interface ISerializationContext
    {
        /// <summary>
        /// Type
        /// </summary>
        ITypeDescriptor TypeDescriptor { get; }

        /// <summary>
        /// Instance of root object being serialized
        /// </summary>
        object RootInstance { get; }
        
        // todo: this should not be exposed in that way
        ConcurrentDictionary<object, InstanceId> Objects_InstanceIdTracker { get; }

        // todo: this should not be exposed in that way
        ConcurrentDictionary<InstanceId, object> Objects_ById { get; }

        object CreateInstance(Type type, CreateInstanceContext cx);

        ITypeSerializationStrategy GetTypeSerializationStrategy(Type type);

        long GetNextUniqueId();

        TypeResolver TypeResolver { get; }
    }
}
