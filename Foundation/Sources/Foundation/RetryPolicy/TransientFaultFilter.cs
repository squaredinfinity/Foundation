﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SquaredInfinity
{
    public abstract class TransientFaultFilter<TException> : ITransientFaultFilter where TException : Exception
    {
        protected abstract bool IsTransientFault(TException exception);

        public bool IsTransientFault(Exception ex)
        {
            // TODO: this never worked, was it supposed to by !(ex is TException) ??
            //if (!ex.GetType().IsSubclassOf(typeof(Exception)))
            //    return false;

            return IsTransientFault((TException)ex);
        }
    }
}