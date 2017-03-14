using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SquaredInfinity.Threading
{
    public interface IInvocationThrottle
    {
        void InvokeAsync(Action action);

        void InvokeAsync(Action<CancellationToken> cancellableAction);

        void InvokeAsync(Action<object, CancellationToken> cancellableActionWithState, object state);
    }
}
