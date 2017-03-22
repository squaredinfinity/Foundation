using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace SquaredInfinity.Collections
{
    public class DynamicIterator<TSource> :
            IEnumerable<TSource>,
            IEnumerable
    {
        protected Func<IEnumerator<TSource>> CreateNewEnumerator { get; private set; }

        public DynamicIterator(Func<IEnumerator<TSource>> createNewEnumerator)
        {
            this.CreateNewEnumerator = createNewEnumerator;
        }

        public IEnumerator<TSource> GetEnumerator()
        {
            return CreateNewEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return (IEnumerator)this.GetEnumerator();
        }

        public void WalkAllItems()
        {
            var should_continue = true;
            var enumerator = GetEnumerator();

            do
            {
                should_continue = enumerator.MoveNext();
            }
            while (should_continue);
        }
    }
}
