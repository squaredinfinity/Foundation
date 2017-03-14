using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SquaredInfinity.Extensions;
using System.Threading;

namespace SquaredInfinity.Serialization
{

    /// <summary>
    /// Uniquely identifies instance of object participating in serialization.
    /// This is used to deal with circular references.
    /// Individual instances are identified by auto-increasing Id number, or by unique name
    /// </summary>
    public class InstanceId : IEquatable<InstanceId>
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

        string _name;
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public InstanceId(string nameOrId)
        {
            if (nameOrId.IsNullOrEmpty())
                throw new ArgumentException("nameOrId");

            var id = 0L;

            if (Int64.TryParse(nameOrId, out id))
                Id = id;
            else
                Name = nameOrId;
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

        public static implicit operator string(InstanceId instanceId)
        {
            return instanceId.Name;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;
                hash = hash * 23 + Id.GetHashCode();

                if (Name != null)
                    hash = hash * 23 + Name.GetHashCode();

                return hash;
            }
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as InstanceId);
        }

        public bool Equals(InstanceId other)
        {
            return
                long.Equals(this.Id, other.Id)
                &&
                string.Equals(Name, other.Name);
        }

        public override string ToString()
        {
            if (Name != null)
                return Name;

            return Id.ToString();
        }
    }
}
