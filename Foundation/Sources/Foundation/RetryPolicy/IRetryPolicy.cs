using SquaredInfinity.Threading;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SquaredInfinity
{
    public interface IRetryPolicy
    {
        void Execute(Action action);
        void Execute(CancellationToken ct, Action action);
        void Execute(TimeSpan timeout, Action action);
        void Execute(TimeSpan timeout, CancellationToken ct, Action action);
        void Execute(int millisecondsTimeout, Action action);
        void Execute(int millisecondsTimeout, CancellationToken ct, Action action);
        void Execute(SyncOptions syncOptions, Action action);

        void Execute(IRetryPolicyOptions options, Action action);
        void Execute(IRetryPolicyOptions options, CancellationToken ct, Action action);
        void Execute(IRetryPolicyOptions options, TimeSpan timeout, Action action);
        void Execute(IRetryPolicyOptions options, TimeSpan timeout, CancellationToken ct, Action action);
        void Execute(IRetryPolicyOptions options, int millisecondsTimeout, Action action);
        void Execute(IRetryPolicyOptions options, int millisecondsTimeout, CancellationToken ct, Action action);
        void Execute(IRetryPolicyOptions options, SyncOptions syncOptions, Action action);

        Task ExecuteAsync(Action action);
        Task ExecuteAsync(CancellationToken ct, Action action);
        Task ExecuteAsync(TimeSpan timeout, Action action);
        Task ExecuteAsync(TimeSpan timeout, CancellationToken ct, Action action);
        Task ExecuteAsync(int millisecondsTimeout, Action action);
        Task ExecuteAsync(int millisecondsTimeout, CancellationToken ct, Action action);
        Task ExecuteAsync(AsyncOptions asyncOptions, Action action);

        Task ExecuteAsync(IRetryPolicyOptions options, Action action);
        Task ExecuteAsync(IRetryPolicyOptions options,CancellationToken ct, Action action);
        Task ExecuteAsync(IRetryPolicyOptions options,TimeSpan timeout, Action action);
        Task ExecuteAsync(IRetryPolicyOptions options,TimeSpan timeout, CancellationToken ct, Action action);
        Task ExecuteAsync(IRetryPolicyOptions options,int millisecondsTimeout, Action action);
        Task ExecuteAsync(IRetryPolicyOptions options, int millisecondsTimeout, CancellationToken ct, Action action);
        Task ExecuteAsync(IRetryPolicyOptions options, AsyncOptions asyncOptions, Action action);

        TResult Execute<TResult>(Func<TResult> func);
        TResult Execute<TResult>(CancellationToken ct, Func<TResult> func);
        TResult Execute<TResult>(TimeSpan timeout, Func<TResult> func);
        TResult Execute<TResult>(TimeSpan timeout, CancellationToken ct, Func<TResult> func);
        TResult Execute<TResult>(int millisecondsTimeout, Func<TRes ult> func);
        TResult Execute<TResult>(int millisecondsTimeout, CancellationToken ct, Func<TResult> func);
        TResult Execute<TResult>(SyncOptions syncOptions, Func<TResult> func);
        TResult Execute<TResult>(IRetryPolicyOptions options, Func<TResult> func);
        TResult Execute<TResult>(IRetryPolicyOptions options, CancellationToken ct, Func<TResult> func);
        TResult Execute<TResult>(IRetryPolicyOptions options, TimeSpan timeout, Func<TResult> func);
        TResult Execute<TResult>(IRetryPolicyOptions options, TimeSpan timeout, CancellationToken ct, Func<TResult> func);
        TResult Execute<TResult>(IRetryPolicyOptions options, int millisecondsTimeout, Func<TResult> func);
        TResult Execute<TResult>(IRetryPolicyOptions options, int millisecondsTimeout, CancellationToken ct, Func<TResult> func);
        TResult Execute<TResult>(IRetryPolicyOptions options, SyncOptions syncOptions, Func<TResult> func);
                    
        Task<TResult> ExecuteAsync<TResult>(Func<Task<TResult>> func);
                    
        Task<TResult> ExecuteAsync<TResult>(CancellationToken ct, Func<TResult> func);
        Task<TResult> ExecuteAsync<TResult>(TimeSpan timeout, Func<TResult> func);
        Task<TResult> ExecuteAsync<TResult>(TimeSpan timeout, CancellationToken ct, Func<TResult> func);
        Task<TResult> ExecuteAsync<TResult>(int millisecondsTimeout, Func<TResult> func);
        Task<TResult> ExecuteAsync<TResult>(int millisecondsTimeout, CancellationToken ct, Func<TResult> func);
        Task<TResult> ExecuteAsync<TResult>(AsyncOptions asyncOptions, Func<TResult> func);
        Task<TResult> ExecuteAsync<TResult>(IRetryPolicyOptions options, Func<TResult> func);
        Task<TResult> ExecuteAsync<TResult>(IRetryPolicyOptions options, CancellationToken ct, Func<TResult> func);
        Task<TResult> ExecuteAsync<TResult>(IRetryPolicyOptions options, TimeSpan timeout, Func<TResult> func);
        Task<TResult> ExecuteAsync<TResult>(IRetryPolicyOptions options, TimeSpan timeout, CancellationToken ct, Func<TResult> func);
        Task<TResult> ExecuteAsync<TResult>(IRetryPolicyOptions options, int millisecondsTimeout, Func<TResult> func);
        Task<TResult> ExecuteAsync<TResult>(IRetryPolicyOptions options, int millisecondsTimeout, CancellationToken ct, Func<TResult> func);
        Task<TResult> ExecuteAsync<TResult>(IRetryPolicyOptions options, AsyncOptions asyncOptions, Func<TResult> func);
    }
}
