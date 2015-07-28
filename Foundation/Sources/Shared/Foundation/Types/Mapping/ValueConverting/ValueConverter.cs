using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SquaredInfinity.Foundation.Types.Mapping.ValueConverting
{
    public abstract class ValueConverter<TFrom, TTo> : IValueConverter<TFrom, TTo>
    {
        public Type From { get; private set; }

        public Type To { get; private set; }

        public ValueConverter()
        {
            From = typeof(TFrom);
            To = typeof(TTo);
        }

        public object Convert(object from)
        {
            return (TTo)Convert((TFrom)from);
        }
        public abstract TTo Convert(TFrom from);
    }
}
