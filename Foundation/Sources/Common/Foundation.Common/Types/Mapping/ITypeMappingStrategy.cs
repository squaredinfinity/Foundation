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
        
        bool TryGetValueResolverForMember(string memberName, out IValueResolver valueResolver);
        
        bool TryCreateInstace(object source, Type targetType, CreateInstanceContext create_cx, out object newInstance);
    }

    public interface ITypeMappingStrategy<TFrom, TTo> : ITypeMappingStrategy
    {
        // todo: this should return interface
        TypeMappingStrategy<TFrom, TTo> IgnoreAllMembers();
    }
}
