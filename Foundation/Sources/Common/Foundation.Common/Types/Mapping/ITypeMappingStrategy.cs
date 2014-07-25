﻿using SquaredInfinity.Foundation.Types.Mapping.ValueResolving;
using SquaredInfinity.Foundation.Types.Mapping.MemberMatching;
using System;
using SquaredInfinity.Foundation.Types.Description;
using System.Linq.Expressions;
using System.Collections.Concurrent;
namespace SquaredInfinity.Foundation.Types.Mapping
{

    public interface ITypeMappingStrategy
    {
        Type SourceType { get; }
        Type TargetType { get; }
        ITypeDescription SourceTypeDescription { get; }
        ITypeDescription TargetTypeDescription { get; }

        ConcurrentDictionary<ITypeMemberDescription, IValueResolver> TargetMembersMappings { get; }

        //bool TryGetValueResolverForMember(string memberName, out IValueResolver valueResolver);
        
        bool TryCreateInstace(object source, Type targetType, CreateInstanceContext create_cx, out object newInstance);
    }

    public interface ITypeMappingStrategy<TFrom, TTo> : ITypeMappingStrategy
    {
        // todo: this should return interface (other than type mapping stragegy?
        // fluid-sort-of wrapper to hide members of ITypeMappingStrategy ?
        ITypeMappingStrategy<TFrom, TTo> IgnoreAllMembers();
        ITypeMappingStrategy<TFrom, TTo> MapMember<TMember>(
            Expression<
            Func<TTo, object>> targetMemberExpression,
            Func<TFrom, TMember> getValue);

        ITypeMappingStrategy<TFrom, TTo> CreateTargetInstance(Func<TFrom, CreateInstanceContext, TTo> createTargetInstance);
    }
}
