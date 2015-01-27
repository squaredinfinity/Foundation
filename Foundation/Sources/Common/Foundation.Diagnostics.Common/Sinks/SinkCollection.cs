using SquaredInfinity.Foundation.Collections;
using SquaredInfinity.Foundation.Threading;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Foundation.Diagnostics.Sinks
{
    public class SinkCollection : CollectionEx<ISink>, ISinkCollection
    {
        public IReadOnlyList<ISink> MustWaitForWriteSinks { get; private set; }
        public IReadOnlyList<ISink> FireAndForgetSinks { get; private set; }
        public IReadOnlyList<IRawMessageSink> MustWaitForWriteRawMessageSinks { get; private set; }
        public IReadOnlyList<IRawMessageSink> FireAndForgetRawMessageSinks { get; private set; }

        readonly ILock CacheLock = new ReaderWriterLockSlimEx();

        public SinkCollection() 
        {

            MustWaitForWriteSinks = new ReadOnlyCollection<ISink>(new List<ISink>());
            FireAndForgetSinks = new ReadOnlyCollection<ISink>(new List<ISink>());
            MustWaitForWriteRawMessageSinks = new ReadOnlyCollection<IRawMessageSink>(new List<IRawMessageSink>());
            FireAndForgetRawMessageSinks = new ReadOnlyCollection<IRawMessageSink>(new List<IRawMessageSink>());
        }

        public SinkCollection(ISink[] items)
            : this()
        {
            AddRange(items);
        }

        public bool TryFindByName(string name, out ISink result)
        {
            result =
                (from x in this
                 where string.Equals(x.Name, name, StringComparison.InvariantCultureIgnoreCase)
                 select x).FirstOrDefault();

            return result != null;
        }

        public ISink FindByName(string name)
        {
            var sink = (ISink)null;

            TryFindByName(name, out sink);

            return sink;
        }

        public void RefreshCache()
        {
            using (CacheLock.AcquireWriteLock())
            {
                try
                {

                    MustWaitForWriteSinks =
                        new ReadOnlyCollection<ISink>(
                            (from s in this
                             where s.MustWaitForWrite
                             && (!(s is IRawMessageSink) || !((IRawMessageSink)s).HandlesRawMessages)
                             select s)
                             .ToList());

                    FireAndForgetSinks =
                        new ReadOnlyCollection<ISink>(
                            (from s in this
                             where !s.MustWaitForWrite
                             && (!(s is IRawMessageSink) || !((IRawMessageSink)s).HandlesRawMessages)
                             select s)
                            .ToList());

                    MustWaitForWriteRawMessageSinks =
                        new ReadOnlyCollection<IRawMessageSink>(
                            (from s in this
                             where s.MustWaitForWrite
                             && (s is IRawMessageSink && ((IRawMessageSink)s).HandlesRawMessages)
                             select (IRawMessageSink)s)
                            .ToList());

                    FireAndForgetRawMessageSinks =
                        new ReadOnlyCollection<IRawMessageSink>(
                            (from s in this
                             where !s.MustWaitForWrite
                             && (s is IRawMessageSink && ((IRawMessageSink)s).HandlesRawMessages)
                             select (IRawMessageSink)s)
                            .ToList());
                }
                catch(Exception ex)
                {
                    // todo: logging
                }
            }
        }

        protected override void OnAfterVersionChanged(long newVersion)
        {
            base.OnAfterVersionChanged(newVersion);

            RefreshCache();
        }

    }
}
