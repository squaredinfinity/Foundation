using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SquaredInfinity.Foundation.Extensions;
using System.Threading.Tasks;

namespace SquaredInfinity.Foundation.PropertySystem
{
    public class ExtendedPropertyUniqueIdentifier : IEquatable<ExtendedPropertyUniqueIdentifier>
    {
        public string OwnerUniqueName { get; private set; }
        public string PropertyUniqueName { get; private set; }

        public ExtendedPropertyUniqueIdentifier(string ownerUniqueName, string propertyUniqueName)
        {
            if (ownerUniqueName.IsNullOrEmpty())
            {
                var ex = new ArgumentException("ownerUniqueName");
                ex.TryAddContextData("ownerUnqiueName", () => ownerUniqueName);
                throw ex;
            }

            if (propertyUniqueName.IsNullOrEmpty())
            {
                var ex = new ArgumentException("propertyUniqueName");
                ex.TryAddContextData("propertyUniqueName", () => propertyUniqueName);
                throw ex;
            }

            this.OwnerUniqueName = ownerUniqueName;
            this.PropertyUniqueName = propertyUniqueName;
        }

        public ExtendedPropertyUniqueIdentifier(string propertyFullName)
        {
            var parts = propertyFullName.Split('.', StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length != 2)
            {
                var ex = new ArgumentException("propertyFullName");
                ex.TryAddContextData("propertyFullName", () => propertyFullName);
                throw ex;
            }

            this.OwnerUniqueName = parts[0];
            this.PropertyUniqueName = parts[1];
        }

        public override int GetHashCode()
        {
            return OwnerUniqueName.GetHashCode() ^ PropertyUniqueName.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as ExtendedPropertyUniqueIdentifier);
        }

        public bool Equals(ExtendedPropertyUniqueIdentifier other)
        {
            if (other == null)
                return false;

            return
                string.Equals(OwnerUniqueName, other.OwnerUniqueName)
                &&
                string.Equals(PropertyUniqueName, other.PropertyUniqueName);
        }

        public override string ToString()
        {
            return "{0}.{1}".FormatWith(OwnerUniqueName, PropertyUniqueName);
        }
    }
}
