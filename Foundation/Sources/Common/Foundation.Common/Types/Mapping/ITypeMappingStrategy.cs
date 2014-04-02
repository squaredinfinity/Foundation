using SquaredInfinity.Foundation.Types.Mapping.ValueResolving;
using SquaredInfinity.Foundation.Types.Mapping.MemberMatching;
using System;
using SquaredInfinity.Foundation.Types.Description;
namespace SquaredInfinity.Foundation.Types.Mapping
{
    public interface ITypeMappingStrategy
    {
        Type SourceType { get; }
        Type TargetType { get; }

        ITypeDescription SourceTypeDescription { get; }
        ITypeDescription TargetTypeDescription { get; }

        bool CloneListElements { get; }
        ITypeDescriptor TypeDescriptor { get; }

        // todo: public interfaces should be IReadOnlyLists in 4.5, they really should not be modifed
        ValueResolverCollection ValueResolvers { get; }
        bool TryGetValueResolverForMember(string memberName, out IValueResolver valueResolver);
    }
}
