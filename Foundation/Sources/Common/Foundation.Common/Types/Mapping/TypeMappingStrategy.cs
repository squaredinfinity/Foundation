using SquaredInfinity.Foundation.Types.Description;
using SquaredInfinity.Foundation.Types.Description.Reflection;
using SquaredInfinity.Foundation.Types.Mapping.MemberMatching;
using SquaredInfinity.Foundation.Types.Mapping.ValueResolving;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using SquaredInfinity.Foundation.Extensions;

namespace SquaredInfinity.Foundation.Types.Mapping
{
    public class TypeMappingStrategy<TFrom, TTo> : TypeMappingStrategy, ITypeMappingStrategy<TFrom, TTo>
    {
        public TypeMappingStrategy(
            ITypeDescriptor sourceTypeDescriptor,
            ITypeDescriptor targetTypeDescriptor,
            MemberMatchingRuleCollection memberMatchingRules,
            ValueResolverCollection valueResolvers)
            : base(
            typeof(TFrom), 
            typeof(TTo), 
            sourceTypeDescriptor,
            targetTypeDescriptor,
            memberMatchingRules,
            valueResolvers)
        { }


        public ITypeMappingStrategy<TFrom, TTo> MapMember<TMember>(Expression<Func<TTo, object>> sourceMemberExpression, Func<TFrom, TMember> getValue)
        {
            var resolver = new DynamicValueResolver<TFrom, TMember>(getValue);

            var memberName = sourceMemberExpression.GetAccessedMemberName();

            MemberNameToValueResolverMappings.AddOrUpdate(memberName, (key) => resolver, (key, oldValue) => resolver);

            return this;
        }

        public ITypeMappingStrategy<TFrom, TTo> IgnoreAllMembers()
        {
            MemberNameToValueResolverMappings.Clear();
            return this;
        }


        public ITypeMappingStrategy<TFrom, TTo> CreateTargetInstance(Func<TFrom, CreateInstanceContext, TTo> createTargetInstance)
        {
            base.CreateTargetInstance = (_s, _cx) => createTargetInstance((TFrom)_s, _cx);

            return this;
        }
    }

    public partial class TypeMappingStrategy : ITypeMappingStrategy
    {

        public Type SourceType { get; set; }
        public Type TargetType { get; set; }
        public ITypeDescription SourceTypeDescription { get; private set; }
        public ITypeDescription TargetTypeDescription { get; private set; }

        protected Func<object, CreateInstanceContext, object> CreateTargetInstance { get; set; }

        MemberMatchingRuleCollection MemberMatchingRules { get; set; }

        public ITypeDescriptor SourceTypeDescriptor { get; set; }
        public ITypeDescriptor TargetTypeDescriptor { get; set; }

        public ValueResolving.ValueResolverCollection ValueResolvers { get; set; }

        // todo, add cia method instead of maing int protected
        protected readonly ConcurrentDictionary<string, IValueResolver> MemberNameToValueResolverMappings 
            = new ConcurrentDictionary<string, IValueResolver>();
        
        public TypeMappingStrategy(
            Type sourceType,
            Type targetType,
            ITypeDescriptor sourceTypeDescriptor,
            ITypeDescriptor targetTypeDescriptor,
            MemberMatchingRuleCollection memberMatchingRules, 
            ValueResolverCollection valueResolvers, bool useautomatchedresolvers = true)
        {
            this.SourceType = sourceType;
            this.TargetType = targetType;

            this.SourceTypeDescriptor = sourceTypeDescriptor;
            this.TargetTypeDescriptor = targetTypeDescriptor;

            this.MemberMatchingRules = memberMatchingRules;
            this.ValueResolvers = valueResolvers;

            this.SourceTypeDescription = SourceTypeDescriptor.DescribeType(sourceType);
            this.TargetTypeDescription = TargetTypeDescriptor.DescribeType(targetType);

            var memberMatches = GetMemberMatches(SourceTypeDescription, TargetTypeDescription, MemberMatchingRules);

            if (useautomatchedresolvers)
            {
                for (int i = 0; i < memberMatches.Count; i++)
                {
                    var m = memberMatches[i];

                    MemberNameToValueResolverMappings
                        .AddOrUpdate(
                        m.To.Name,
                        _key => new MatchedMemberValueResolver(m),
                        (_key, _old) => new MatchedMemberValueResolver(m));
                }
            }
        }

        static MemberMatchCollection GetMemberMatches(
            ITypeDescription sourceDescription, 
            ITypeDescription targetDescription,
            MemberMatchingRuleCollection matchingRules)
        {
            var result = new MemberMatchCollection();

            // list of source members
            var sourceMembers =
                (from m in sourceDescription.Members
                 where m.CanGetValue
                 select m).ToList();

            for (int i_member = 0; i_member < targetDescription.Members.Count; i_member++)
            {
                var targetMember = targetDescription.Members[i_member];

                if (!targetMember.CanSetValue)
                    continue;

                for (int i = 0; i < matchingRules.Count; i++)
                {
                    var ms = matchingRules[i];

                    var sourceMember = (ITypeMemberDescription) null;

                    if (ms.TryMapMembers(targetMember, sourceMembers, out sourceMember))
                    {
                        sourceMembers.Remove(sourceMember);

                        var mapping = new MemberMatch
                        {
                            From = sourceMember,
                            To = targetMember
                        };

                        result.Add(mapping);

                        break;
                    }

                    if (sourceMembers.Count == 0)
                        return result;
                }
            }

            return result;
        }


        public bool TryGetValueResolverForMember(string memberName, out IValueResolver valueResolver)
        {
            valueResolver = null;

            return MemberNameToValueResolverMappings.TryGetValue(memberName, out valueResolver);                
        }

        public bool TryCreateInstace(object source, Type targetType, CreateInstanceContext create_cx, out object newInstance)
        {
            newInstance = null;

            try
            {
                if (CreateTargetInstance != null)
                {
                    newInstance = CreateTargetInstance(source, create_cx);
                    return true;
                }

                var constructor = targetType
                    .GetConstructor(
                    BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic,
                    binder: null,
                    types: Type.EmptyTypes,
                    modifiers: null);

                if (constructor != null)
                {
                    newInstance = constructor.Invoke(null);

                    return true;
                }
            }
            catch (Exception ex)
            {
                // todo: internal logging
            }

            return false;
        }
    }
}
