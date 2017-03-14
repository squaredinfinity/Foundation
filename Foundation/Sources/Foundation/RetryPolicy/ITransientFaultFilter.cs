using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SquaredInfinity
{
    public interface ITransientFaultFilter
    {
        bool IsTransientFault(Exception ex);
    }
}
