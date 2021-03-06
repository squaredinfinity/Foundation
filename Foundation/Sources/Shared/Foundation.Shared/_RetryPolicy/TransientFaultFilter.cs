﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SquaredInfinity.Foundation
{
    public abstract class TransientFaultFilter<TException> : ITransientFaultFilter where TException : Exception
    {
        protected abstract bool IsTransientFault(TException exception);

        public bool IsTransientFault(Exception ex)
        {
            if (typeof(TException) != ex.GetType())
                return false;

            return IsTransientFault((TException)ex);
        }
    }
}
