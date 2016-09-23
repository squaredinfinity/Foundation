using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SquaredInfinity.Foundation.Threading
{
    public interface IAsyncLoop
    {
        void Start(TimeSpan loopIterationDelay, CancellationToken cancellationToken);
        //TimeSpan LoopIterationDelay { get; set; }

        //void Cancel();
    }
}
