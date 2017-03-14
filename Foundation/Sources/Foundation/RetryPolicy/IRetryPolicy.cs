using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity
{
    public interface IRetryPolicy
    {
        void Execute(Action action);
        void Execute(IRetryPolicyOptions options, Action action);

        Task ExecuteAsync(Action action);
        Task ExecuteAsync(IRetryPolicyOptions options, Action action);

        TResult Execute<TResult>(Func<TResult> func);
        TResult Execute<TResult>(IRetryPolicyOptions options, Func<TResult> func);

        Task<TResult> ExecuteAsync<TResult>(Func<Task<TResult>> func);
        Task<TResult> ExecuteAsync<TResult>(IRetryPolicyOptions options, Func<Task<TResult>> func);
    }
}
