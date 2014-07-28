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
        public bool IsMappingNeeded { get; set; }

        public ValueResolver()
        {
            this.FromType = typeof(TFrom);
            this.ToType = typeof(TTo);

            AreFromAndToTypesSame = FromType == ToType;
            AreFromAndToImmutable = true; // this could be calculated here, bot not needed for now, for now always map unless specified otherwise
            AreFromAndToValueType = FromType.IsValueType && ToType.IsValueType;

            IsMappingNeeded =
                AreFromAndToTypesSame
                &&
                AreFromAndToValueType
                &&
                AreFromAndToImmutable;
        }

        public abstract TTo ResolveValue(TFrom source);

        public object ResolveValue(object source)
        {
            return (TTo)ResolveValue((TFrom)source);
        }
    }
}
