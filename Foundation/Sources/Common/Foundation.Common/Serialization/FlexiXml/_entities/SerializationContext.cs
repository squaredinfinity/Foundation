using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SquaredInfinity.Foundation.Serialization.FlexiXml
{
    /// <summary>
    /// Serialization Context sotres state used by serializer during serialization.
    /// </summary>
    class SerializationContext
    {
        public readonly ConcurrentDictionary<object, InstanceId> Objects_InstanceIdTracker = new ConcurrentDictionary<object, InstanceId>();

        long LastUsedUniqueId = 0;

        public long GetNextUniqueId()
        {
            return Interlocked.Increment(ref LastUsedUniqueId);
        }
    }
}
