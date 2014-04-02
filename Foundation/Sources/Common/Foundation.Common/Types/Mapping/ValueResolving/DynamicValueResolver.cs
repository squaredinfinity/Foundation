using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SquaredInfinity.Foundation.Types.Mapping.ValueResolving
{
    public class DynamicValueResolver<TSource, TTarget> : IValueResolver
    {
        Func<TSource, TTarget> Resolve { get; set; }

        public DynamicValueResolver(Func<TSource, TTarget> resolve)
        {
            this.Resolve = resolve;
        }

        public object ResolveValue(object source)
        {
            return Resolve((TSource)source);
        }
    }
}
