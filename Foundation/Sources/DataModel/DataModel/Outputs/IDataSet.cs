using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.DataModel.Outputs
{
    public interface IDataSet : IEnumerable<IDataItem>
    {
        UInt32 Count { get; }

        IDataItem GetDataItem(UInt32 index);
    }

    public abstract class DataSet : IDataSet
    {
        #region Count

        public UInt32 Count { get { return GetCount(); } }
        protected abstract UInt32 GetCount();

        #endregion

        #region Get Data Item

        public IDataItem GetDataItem(UInt32 index)
        {
            return DoGetDataItem(index);
        }

        protected abstract IDataItem DoGetDataItem(UInt32 index);

        #endregion

        #region Enumerators

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }

        public IEnumerator<IDataItem> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
