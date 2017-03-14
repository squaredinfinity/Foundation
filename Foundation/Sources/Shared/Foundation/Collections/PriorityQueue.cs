using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace SquaredInfinity.Collections
{
    public class PriorityQueueItem<TValue>
    {
        public int Priority;
        public TValue Value;
    }

    public class PriorityQueue<TValue> : IProducerConsumerCollection<PriorityQueueItem<TValue>>
    {
        // each priority holds its own queue with items

        protected class SpecificPriorityConcurrentQueue : ConcurrentQueue<PriorityQueueItem<TValue>>
        { }

        readonly protected SpecificPriorityConcurrentQueue[] InternalQueues;

        protected int PriorityCount { get; private set; }

        public PriorityQueue(int priCount)
        {
            this.PriorityCount = priCount;
            InternalQueues = new SpecificPriorityConcurrentQueue[PriorityCount];

            for (int i = 0; i < PriorityCount; i++)
                InternalQueues[i] = new SpecificPriorityConcurrentQueue();
        }

        protected void IncrementCount()
        {
            Interlocked.Increment(ref _count);
        }

        protected void DecrementCount()
        {
            Interlocked.Decrement(ref _count);
        }

        public virtual bool TryAdd(PriorityQueueItem<TValue> item)
        {
            InternalQueues[item.Priority].Enqueue(item);

            IncrementCount();

            return true;
        }

        public virtual bool TryTake(out PriorityQueueItem<TValue> item)
        {
            bool success = false;

            for (int i = 0; i < PriorityCount; i++)
            {
                success = InternalQueues[i].TryDequeue(out item);

                if (success)
                {
                    DecrementCount();

                    return true;
                }
            }

            item = default(PriorityQueueItem<TValue>);

            return false;
        }

        int _count = 0;

        public int Count
        {
            get { return _count; }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public virtual IEnumerator<PriorityQueueItem<TValue>> GetEnumerator()
        {
            for (int i = 0; i < PriorityCount; i++)
            {
                foreach (var item in InternalQueues[i])
                    yield return item;
            }
        }

        public bool IsSynchronized
        {
            get
            {
                throw new NotSupportedException();
            }
        }

        public object SyncRoot
        {
            get { throw new NotSupportedException(); }
        }

        public void CopyTo(PriorityQueueItem<TValue>[] array, int index)
        {
            throw new NotImplementedException();
        }

        PriorityQueueItem<TValue>[] IProducerConsumerCollection<PriorityQueueItem<TValue>>.ToArray()
        {
            throw new NotImplementedException();
        }

        public void CopyTo(Array array, int index)
        {
            throw new NotImplementedException();
        }
    }
}
