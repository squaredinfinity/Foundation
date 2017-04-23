using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SquaredInfinity.Types.Mapping.ValueResolving
{
    public class DynamicValueResolver<TFrom, TTo> : ValueResolver<TFrom, TTo>, IDynamicValueResolver
    {
        Func<TFrom, TTo> Resolve { get; set; }

        public DynamicValueResolver(Func<TFrom, TTo> resolve)
        {
            this.Resolve = resolve;

            // mapping is not needed
            // mapped value will be supplied by user
            CanCopyValueWithoutMapping = true;
        }

        public override bool TryResolveValue(TFrom source, out TTo val)
        {
            try
            {
                val = Resolve(source);
                return true;
            }
            catch(Exception ex)
            {
                val = default(TTo);
                return false;
            }
        }
    }

    public interface IDynamicValueResolver
    { }
}
