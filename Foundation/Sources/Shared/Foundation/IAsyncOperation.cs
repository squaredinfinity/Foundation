//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading;
//using System.Threading.Tasks;

//namespace SquaredInfinity.Threading
//{
//    public interface IAsyncAction : IDisposable
//    {
//        IAsyncOperationRequest RequestExecute();
//        IAsyncOperationRequest RequestExecute(Action<CancellationToken> beforeExecute, Action<CancellationToken> afterExecute);
//    }

//    public interface IAsyncAction<T> : IDisposable
//    {
//        IAsyncOperationRequest RequestExecute(T argument);
//    }
//}
