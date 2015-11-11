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

        public bool AreFromAndToTypesSame { get; set; }
        public bool AreFromAndToImmutable { get; set; }
        public bool AreFromAndToValueType { get; set; }
        public bool CanCopyValueWithoutMapping { get; set; }

        public ValueResolver()
        {
            this.FromType = typeof(TFrom);
            this.ToType = typeof(TTo);

            AreFromAndToTypesSame = FromType == ToType;
            AreFromAndToImmutable = true; // this could be calculated here, bot not needed for now, for now always map unless specified otherwise
            AreFromAndToValueType = FromType.IsValueType && ToType.IsValueType;

            CanCopyValueWithoutMapping =
                AreFromAndToTypesSame
                &&
                AreFromAndToValueType
                &&
                AreFromAndToImmutable;
        }

        public abstract bool TryResolveValue(TFrom source, out TTo val);

        public bool TryResolveValue(object source, out object val)
        {
            var t_val = default(TTo);

            var result = TryResolveValue((TFrom)source, out t_val);

            val = t_val;

            return result;
        }
    }
}
