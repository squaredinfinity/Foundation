using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Foundation.Collections
{
    public class DisposableCollection : CollectionEx<IDisposable>
    {
        public DisposableCollection()
        { }

        protected override void ClearItems()
        {
            for(int i = 0; i < Count; i++)
            {
                this[i].Dispose();
            }

            base.ClearItems();
        }
    }
}
