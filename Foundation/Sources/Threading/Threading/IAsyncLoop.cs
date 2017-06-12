using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SquaredInfinity.Threading
{
    /// <summary>
    /// Represents an asynchronous loop with specified wait time between each iteration.
    /// </summary>
    public interface IAsyncLoop : IDisposable
    {
        void Start(TimeSpan loopIterationDelay, CancellationToken cancellationToken);
    }
}
