using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SquaredInfinity.Diagnostics.ContextDataCollectors
{
    public interface IDataCollectionContext
    {
        Process CurrentProcess { get; }

        Thread CurrentThread { get; }
    }
}
