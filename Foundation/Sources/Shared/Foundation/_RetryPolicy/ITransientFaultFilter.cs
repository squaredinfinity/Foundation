using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SquaredInfinity.Foundation
{
    public interface ITransientFaultFilter
    {
        bool IsTransientFault(Exception ex);
    }
}
