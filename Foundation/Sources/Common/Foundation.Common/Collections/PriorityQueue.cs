using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace SquaredInfinity.Foundation.Collections
{
    public class PriorityQueueItem<TValue>
    {
        public int Priority;
        public TValue Value;
    }

    public class PriorityQueue<TValue> : IProducerConsumerCollection<PriorityQueueItem<TValue>>
    {
        // each priority holds its own queue with items

        class SpecificPriorityConcurrentQueue : ConcurrentQueue<PriorityQueueItem<TValue>>
        { }

        SpecificPriorityConcurrentQueue[] InternalQueues = null;
        
        int priorityCount = 0;
        int m_count = 0;

        public PriorityQueue(int priCount)
        {
            this.priorityCount = priCount;
            InternalQueues = new SpecificPriorityConcurrentQueue[priorityCount];

            for (int i = 0; i < priorityCount; i++)
                InternalQueues[i] = new SpecificPriorityConcurrentQueue();
        }

        public bool TryAdd(int priority, TValue item)
        {
            var pqItem = new PriorityQueueItem<TValue> { Priority = priority, Value = item };

            return TryAdd(pqItem);
        }

        public bool TryAdd(PriorityQueueItem<TValue> item)
        {
            InternalQueues[item.Priority].Enqueue(item);

            Interlocked.Increment(ref m_count);

            return true;
        }

        public bool TryTake(out TValue item)
        {
            PriorityQueueItem<TValue> result = default(PriorityQueueItem<TValue>);

            if(TryTake(out result))
            {
                item = result.Value;
                return true;
            }

            item = default(TValue);
            return false;
        }

        public bool TryTake(out PriorityQueueItem<TValue> item)
        {
            bool success = false;

            for (int i = 0; i < priorityCount; i++)
            {
                success = InternalQueues[i].TryDequeue(out item);

                if (success)
                {
                    Interlocked.Decrement(ref m_count);

                    return true;
                }
            }

            item = default(PriorityQueueItem<TValue>);

            return false;
        }

        public int Count
        {
            get { return m_count; }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IEnumerator<PriorityQueueItem<TValue>> GetEnumerator()
        {
            for (int i = 0; i < priorityCount; i++)
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
