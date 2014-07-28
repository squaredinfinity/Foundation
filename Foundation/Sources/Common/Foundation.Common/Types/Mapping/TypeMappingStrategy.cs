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

            var member = (from m in base.TargetTypeDescription.Members
                          where m.Name == memberName
                          select m).Single();

            TargetMembersMappings.AddOrUpdate(member, resolver);

            return this;
        }

        public ITypeMappingStrategy<TFrom, TTo> IgnoreAllMembers()
        {
            TargetMembersMappings.Clear();
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
        public bool AreFromAndToTypesSame { get; set; }
        public bool AreFromAndToImmutable { get; set; }
        public bool AreFromAndToValueType { get; set; }
        public bool CanCopyValueWithoutMapping { get; set; }


        public Type SourceType { get; set; }
        public Type TargetType { get; set; }
        public ITypeDescription SourceTypeDescription { get; private set; }
        public ITypeDescription TargetTypeDescription { get; private set; }

        protected Func<object, CreateInstanceContext, object> CreateTargetInstance { get; set; }

        MemberMatchingRuleCollection MemberMatchingRules { get; set; }

        public ITypeDescriptor SourceTypeDescriptor { get; set; }
        public ITypeDescriptor TargetTypeDescriptor { get; set; }

        public ValueResolving.ValueResolverCollection ValueResolvers { get; set; }

        Dictionary<ITypeMemberDescription, IValueResolver> _targetMembersMappings
         = new Dictionary<ITypeMemberDescription, IValueResolver>();
        public Dictionary<ITypeMemberDescription, IValueResolver> TargetMembersMappings
        {
            get { return _targetMembersMappings; }
        }
        
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

                    TargetMembersMappings
                        .AddOrUpdate(
                        m.To,
                        new MatchedMemberValueResolver(m));
                }
            }

            AreFromAndToTypesSame = SourceType == TargetType;
            AreFromAndToImmutable = SourceTypeDescription.AreAllMembersImmutable && TargetTypeDescription.AreAllMembersImmutable;
            AreFromAndToValueType = SourceType.IsValueType && TargetType.IsValueType;

            CanCopyValueWithoutMapping =
                (SourceType == typeof(string) && TargetType == typeof(string))
                ||
                (AreFromAndToTypesSame && AreFromAndToValueType && AreFromAndToImmutable)
                ||
                (AreFromAndToTypesSame && SourceType.IsEnum);
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

        public bool TryCreateInstace(object source, ITypeDescription targetTypeDescription, CreateInstanceContext create_cx, out object newInstance)
        {
            newInstance = null;

            try
            {
                if (CreateTargetInstance != null)
                {
                    newInstance = CreateTargetInstance(source, create_cx);
                    return true;
                }

                // get type descriptor

                newInstance = targetTypeDescription.CreateInstance();
                
                return true;
            }
            catch (Exception ex)
            {
                // todo: internal logging
            }

            return false;
        }
    }
}
