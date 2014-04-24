using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SquaredInfinity.Foundation.Serialization.FlexiXml
{
    /// <summary>
    /// Uniquely identifies instance of object participating in serialization.
    /// This is used to deal with circular references.
    /// </summary>
    class InstanceId : IEquatable<InstanceId>
    {
        long _id;
        public long Id
        {
            get { return _id; }
            private set { _id = value; }
        }

        long _referenceCount;
        public long ReferenceCount
        {
            get { return _referenceCount; }
        }

        public InstanceId(long id)
        {
            this.Id = id;
        }

        public void IncrementReferenceCount()
        {
            Interlocked.Increment(ref _referenceCount);
        }

        public static implicit operator long(InstanceId instanceId)
        {
            return instanceId.Id;
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as InstanceId);
        }

        public bool Equals(InstanceId other)
        {
            return this.Id == other.Id;
        }
    }
}
