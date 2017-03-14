using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SquaredInfinity.Threading
{
    public interface IAsyncLoop : IDisposable
    {
        void Start(TimeSpan loopIterationDelay, CancellationToken cancellationToken);
        //TimeSpan LoopIterationDelay { get; set; }

        //void Cancel();
    }
}
