using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SquaredInfinity
{
    public class GCMemoryUsagePressure
    {
        readonly long AllocatedBytes;

        public GCMemoryUsagePressure(long allocatedBytes)
        {
            AllocatedBytes = allocatedBytes;

            if (allocatedBytes > 0)
                GC.AddMemoryPressure(AllocatedBytes);
            else
                GC.SuppressFinalize(this);
        }

        ~GCMemoryUsagePressure()
        {
            GC.RemoveMemoryPressure(AllocatedBytes);
        }
    }
}
