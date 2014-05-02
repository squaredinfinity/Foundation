using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;

namespace SquaredInfinity.Foundation.Collections
{
    public partial class ObservableCollectionEx<TItem> : INotifyCollectionContentChanged
    {
        public event Action<INotifyCollectionContentChanged> VersionChanged;

        int _version;
        public int Version
        {
            get { return _version; }
        }

        public void IncrementVersion()
        {
            Interlocked.Increment(ref _version);

            if (VersionChanged != null)
                VersionChanged(this);
        }
    }
}
