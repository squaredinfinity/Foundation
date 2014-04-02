using SquaredInfinity.Foundation.Types.Mapping.ValueResolving;
using SquaredInfinity.Foundation.Types.Mapping.MemberMatching;
using System;
using SquaredInfinity.Foundation.Types.Description;
namespace SquaredInfinity.Foundation.Types.Mapping
{
    public interface ITypeMappingStrategy
    {
        ITypeDescription SourceTypeDescription { get; }
        ITypeDescription TargetTypeDescription { get; }

        bool CloneListElements { get; }

        bool TryGetValueResolverForMember(string memberName, out IValueResolver valueResolver);
    }
}
