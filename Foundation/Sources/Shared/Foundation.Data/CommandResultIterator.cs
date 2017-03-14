using System;
using System.Collections.Generic;
using System.Text;

namespace SquaredInfinity.Data
{
    class CommandResultIterator<TEntity> : IEnumerable<TEntity>
    {
        readonly Func<IEnumerator<TEntity>> GetEnumeratorFunc;

        public CommandResultIterator(Func<IEnumerator<TEntity>> getEnumerator)
        {
            this.GetEnumeratorFunc = getEnumerator;
        }
        public IEnumerator<TEntity> GetEnumerator()
        {
            return GetEnumeratorFunc();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumeratorFunc();
        }
    }
}
