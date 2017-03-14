using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SquaredInfinity.Foundation
{
    public class DynamicTransientFaultFilter<TException>
            : TransientFaultFilter<TException> where TException : Exception
    {
        readonly Func<TException, bool> IsTransientFaultInternal;

        public DynamicTransientFaultFilter(Func<TException, bool> isTransientFault)
        {
            this.IsTransientFaultInternal = isTransientFault;
        }

        protected override bool IsTransientFault(TException exception)
        {
            return IsTransientFaultInternal(exception);
        }
    }
}
