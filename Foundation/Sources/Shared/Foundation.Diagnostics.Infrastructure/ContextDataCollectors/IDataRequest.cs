using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Diagnostics.ContextDataCollectors
{
    public interface IDataRequest
    {
        string Data { get; }
        bool IsAsync { get; }
        bool IsCached { get; }
    }
}
