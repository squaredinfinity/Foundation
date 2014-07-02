using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Foundation.Threading
{
    public interface IAsyncAction : IDisposable
    {
        TimeSpan RequestsThrottle { get; set; }

        void RequestExecute();
    }

    public interface IAsyncAction<T> : IDisposable
    {
        TimeSpan RequestsThrottle { get; set; }

        void RequestExecute(T argument);
    }
}
