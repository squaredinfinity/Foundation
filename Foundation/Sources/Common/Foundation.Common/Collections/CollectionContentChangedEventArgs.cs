using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Foundation.Collections
{
    public class CollectionContentChangedEventArgs : EventArgs
    {
        public int Version { get; private set; }

        public CollectionContentChangedEventArgs(int version)
        {
            this.Version = version;
        }
    }
}
