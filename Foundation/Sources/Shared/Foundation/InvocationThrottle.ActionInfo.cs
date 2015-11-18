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

            Task LastTask = null;

            CancellationTokenSource LastTaskCancellationTokenSource;


            bool ShouldContinueWinNextAction = false;

            public ActionInfo()
            { }

            class DoActionPayload
            {
                public Action Action;
                public Action<CancellationToken> CancellableAction;
                public CancellationTokenSource CancellationTokenSource;
            }

            public bool TryInvokeAsync()
            {
                lock (Sync)
                {
                    if (Action == null && CancellableAction == null)
                    {
                        return false;
                    }

                    if (LastTask != null && CancellableAction == null)
                    {
                        ShouldContinueWinNextAction = true;
                        return false;
                    }

                    if (CancellableAction != null)
                    {
                        if (LastTaskCancellationTokenSource != null)
                            LastTaskCancellationTokenSource.Cancel();
                    }


                    LastTaskCancellationTokenSource = new CancellationTokenSource();

                    var payload = new DoActionPayload
                    {
                        Action = Action,
                        CancellableAction = CancellableAction,
                        CancellationTokenSource = LastTaskCancellationTokenSource
                    };

                    LastTask = new Task(DoAction, payload, LastTaskCancellationTokenSource.Token);
                    LastTask.ContinueWith(_prev => OnLastTaskFinished(), TaskContinuationOptions.ExecuteSynchronously);
                    LastTask.Start();

                    Action = null;
                    CancellableAction = null;

                    ShouldContinueWinNextAction = false;

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

            void OnLastTaskFinished()
            {
                lock (Sync)
                {
                    LastTask = null;

                    if (!ShouldContinueWinNextAction)
                        return;

                    TryInvokeAsync();
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
