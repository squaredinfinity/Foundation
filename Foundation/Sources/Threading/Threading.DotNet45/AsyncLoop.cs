using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SquaredInfinity.Foundation.Threading
{
    public class AsyncLoop : IAsyncLoop
    {
        WeakDelegate<Action> LoopBodyReference { get; set; }
        CancellationToken CancellationToken { get; set; }
        TimeSpan LoopIterationDelay { get; set; }

        public string Name { get; private set; }

        public AsyncLoop(Action loopBody)
        {
            this.Name = Guid.NewGuid().ToString();
            LoopBodyReference = new WeakDelegate<Action>(this, loopBody);
        }

        public AsyncLoop(string name, Action loopBody)
        {
            this.Name = name;
            LoopBodyReference = new WeakDelegate<Action>(this, loopBody);
        }

        Timer Timer;

        public void Start(TimeSpan loopIterationDelay, CancellationToken cancellationToken)
        {
            if (Timer != null)
            {
                Timer.Change(Timeout.Infinite, Timeout.Infinite);
                Timer.Dispose();
            }

            LoopIterationDelay = loopIterationDelay;
            CancellationToken = cancellationToken;

            // start now
            Timer = new Timer(OnTick, (object)null, 0, Timeout.Infinite);
        }

        public void Start(TimeSpan startDelay, TimeSpan loopIterationDelay, CancellationToken cancellationToken)
        {
            if (Timer != null)
            {
                Timer.Change(Timeout.Infinite, Timeout.Infinite);
                Timer.Dispose();
            }

            LoopIterationDelay = loopIterationDelay;
            CancellationToken = cancellationToken;
            Timer = new Timer(OnTick, (object)null, startDelay, Timeout.InfiniteTimeSpan);
        }

        void OnTick(object state)
        {
            if (CancellationToken.IsCancellationRequested)
                return;

            if(LoopBodyReference.TryExecute())
            {
                if (!CancellationToken.IsCancellationRequested)
                    Timer.Change(LoopIterationDelay, Timeout.InfiniteTimeSpan);
            }
        }

        public void Dispose()
        {
            Timer?.Dispose();
        }
    }
}
