using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Foundation.Serialization.FlexiXml
{
    /// <summary>
    /// Deserialization Context holds state used by serializer during deserialization.
    /// </summary>
    class DeserializationContext
    {
        public readonly ConcurrentDictionary<InstanceId, object> Objects_InstanceIdTracker = 
            new ConcurrentDictionary<InstanceId, object>();
    }
}
