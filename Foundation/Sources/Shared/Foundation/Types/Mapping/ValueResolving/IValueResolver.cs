using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SquaredInfinity.Foundation.Types.Mapping.ValueResolving
{
    public interface IValueResolver
    {
        Type FromType { get; }
        Type ToType { get; }

        bool AreFromAndToTypesSame { get; }
        //bool AreFromAndToImmutable { get; }
        bool AreFromAndToValueType { get; }

        bool CanCopyValueWithoutMapping { get; }

        bool TryResolveValue(object source, out object val);
    }

    public interface IValueResolver<TFrom, TTo> : IValueResolver
    {
        bool TryResolveValue(TFrom source, out TTo val);
    }
}
