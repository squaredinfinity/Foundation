using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SquaredInfinity.Foundation.Types.Mapping.ValueResolving
{
    public abstract class ValueResolver<TFrom, TTo> : IValueResolver<TFrom, TTo>
    {
        public Type ToType { get; set; }

        public Type FromType { get; set; }

        public ValueResolver()
        {
            this.FromType = typeof(TFrom);
            this.ToType = typeof(TTo);
        }

        public abstract TTo ResolveValue(TFrom source);

        public object ResolveValue(object source)
        {
            return (TTo)ResolveValue((TFrom)source);
        }
    }
}
