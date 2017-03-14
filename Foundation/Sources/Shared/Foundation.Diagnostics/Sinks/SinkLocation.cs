using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Diagnostics.Sinks
{
    public class SinkLocation : ISinkLocation
    {
        class LocationEqualityComparer_Implementation : IEqualityComparer<SinkLocation>
        {
            public bool Equals(SinkLocation x, SinkLocation y)
            {
                if (x == null || y == null)
                    return false;

                return StringComparer.InvariantCultureIgnoreCase.Compare(x.Location, y.Location) == 0;
            }

            public int GetHashCode(SinkLocation sinkLocation)
            {
                return sinkLocation.Location.GetHashCode();
            }
        }

        public static readonly IEqualityComparer<SinkLocation> LocationEqualityComparer = new LocationEqualityComparer_Implementation();

        public string Schema { get; private set; }

        public string Location { get; private set; }

        public SinkLocation(string schema, string path)
        {
            this.Schema = schema;
            this.Location = path;
        }
    }
}
