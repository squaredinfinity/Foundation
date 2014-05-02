using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Foundation.Threading
{
    public interface ILongRunningOperation
    {
        TimeSpan Throttle { get; set; }

        void RequestExecute();
    }
}
