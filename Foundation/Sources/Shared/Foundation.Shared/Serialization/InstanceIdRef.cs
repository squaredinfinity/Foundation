using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Foundation.Serialization
{
    public class InstanceIdRef : IEquatable<InstanceIdRef>
    {
        public InstanceId InstanceId { get; private set; }

        public InstanceIdRef(InstanceId instanceId)
        {
            this.InstanceId = instanceId;
            InstanceId.IncrementReferenceCount();
        }

        public override int GetHashCode()
        {
            return InstanceId.GetHashCode();
        }

        public override bool Equals(object other)
        {
            return Equals(other as InstanceIdRef);
        }

        public bool Equals(InstanceIdRef other)
        {
            if (other == null)
                return false;

            if (object.ReferenceEquals(this, other))
                return true;

            return
                object.Equals(this.InstanceId, other.InstanceId);
        }
    }
}
