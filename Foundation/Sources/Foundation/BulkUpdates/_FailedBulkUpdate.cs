using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity
{
    class _FailedBulkUpdate : IBulkUpdate
    {
        public bool HasStarted => false;

        public void Dispose() { }
    }
}
