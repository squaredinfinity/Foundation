using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SquaredInfinity.Foundation.Types.Mapping.ValueResolving
{
    public class DynamicValueResolver<TFrom, TTo> : ValueResolver<TFrom, TTo>, IDynamicValueResolver
    {
        Func<TFrom, TTo> Resolve { get; set; }

        public DynamicValueResolver(Func<TFrom, TTo> resolve)
        {
            this.Resolve = resolve;
        }

        public override TTo ResolveValue(TFrom source)
        {
            return Resolve(source);
        }
    }

    public interface IDynamicValueResolver
    { }
}
