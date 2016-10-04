using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SquaredInfinity.Foundation
{
    public partial class InvocationThrottle
    {
        class ActionInfo
        {
            readonly object Sync = new object();

            Action<CancellationToken> CancellableAction = null;
            Action Action = null;

            DoActionPayload LastAction = null;

            public TimeSpan Timeout { get; internal set; }

            public DateTime LastCompleteTimeUtc { get; internal set; } = DateTime.MinValue;

            public ActionInfo()
            { }

            class DoActionPayload
            {
                public Action Action;
                public Action<CancellationToken> CancellableAction;
                public CancellationTokenSource CancellationTokenSource;

                public bool ShouldContinueWithNextAction { get; internal set; } = true;
                public bool ShouldRunToCompletion { get; internal set; }
                public Task Task { get; internal set; }
            }

            public bool TryInvokeAsync(bool should_run_to_completion)
            {
                lock (Sync)
                {
                    // there's no pending action or cancellable action
                    // just return
                    if (Action == null && CancellableAction == null)
                    {
                        return false;
                    }

                    // last task is set, but cannot be cancelled
                    // when it's finished it should continue with next action (if one has already been set)
                    if (LastAction != null && LastAction.CancellableAction == null)
                    {
                        LastAction.ShouldContinueWithNextAction = true;
                        return false;
                    }

                    // if last task should run to completion, don't cancel it
                    // new action will be processed after last completes
                    if(LastAction != null && LastAction.ShouldRunToCompletion)
                    {
                        LastAction.ShouldContinueWithNextAction = true;
                        return false;
                    }
                    
                    // last task doesn't have to finish, and can be cancelled
                    // cancel it now
                    if (LastAction != null && LastAction.CancellationTokenSource != null)
                    {
                        LastAction.CancellationTokenSource.Cancel();
                    }

                    var payload = new DoActionPayload
                    {
                        Action = Action,
                        CancellableAction = CancellableAction,
                        CancellationTokenSource = new CancellationTokenSource(Timeout),
                        ShouldRunToCompletion = should_run_to_completion
                    };

                    var new_task = new Task(DoAction, payload, payload.CancellationTokenSource.Token);

                    if (LastAction != null)
                    {
                        payload.Task = LastAction.Task.ContinueWith(_prev => new_task, payload.CancellationTokenSource.Token, TaskContinuationOptions.ExecuteSynchronously, TaskScheduler.Default);
                    }
                    else
                    {
                        payload.Task = new_task;
                    }

                    // call self after finished to invoke any pending actions
                    payload.Task =
                        payload.Task
                        .ContinueWith(_prev => OnLastTaskFinished(payload), TaskContinuationOptions.ExecuteSynchronously);

                    // start task if there's no previous action
                    // othwerwise task will be a continueation of previous
                    if(LastAction == null)
                        new_task.Start();

                    LastAction = payload;

                    Action = null;
                    CancellableAction = null;

                    return true;
                }
            }

            static void DoAction(object payloadAsObject)
            {
                var payload = (DoActionPayload)payloadAsObject;

                if (payload.Action != null)
                {
                    payload.Action();
                    
                    return;
                }

                if (payload.CancellableAction != null)
                {
                    payload.CancellableAction(payload.CancellationTokenSource.Token);
                    return;
                }

            }

            void OnLastTaskFinished(DoActionPayload payload)
            {
                lock (Sync)
                {
                    var should_continue_with_next_action = false;

                    if (LastAction != null)
                    {
                        should_continue_with_next_action = LastAction.ShouldContinueWithNextAction;
                    }

                    LastAction = null;

                    if(!payload.CancellationTokenSource.IsCancellationRequested)
                        LastCompleteTimeUtc = DateTime.UtcNow;

                    if(should_continue_with_next_action)
                        TryInvokeAsync(should_run_to_completion: false);
                }
            }

            public void SetAction(Action action)
            {
                lock (Sync)
                {
                    Action = action;
                    CancellableAction = null;
                }
            }

            public void SetAction(Action<CancellationToken> cancellableAction)
            {
                lock (Sync)
                {
                    Action = null;
                    CancellableAction = cancellableAction;
                }
            }
        }
    }
}
