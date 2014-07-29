using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Foundation.Threading
{
    public interface IAsyncAction : IDisposable
    {
        IAsyncOperationRequest RequestExecute();
    }

    public interface IAsyncAction<T> : IDisposable
    {
        IAsyncOperationRequest RequestExecute(T argument);
    }
}
