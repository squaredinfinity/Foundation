using SquaredInfinity.Foundation.Types.Description;
using SquaredInfinity.Foundation.Types.Description.Reflection;
using SquaredInfinity.Foundation.Types.Mapping.MemberMatching;
using SquaredInfinity.Foundation.Types.Mapping.ValueResolving;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SquaredInfinity.Foundation.Types.Mapping
{
    public partial class TypeMappingStrategy : ITypeMappingStrategy
    {
        public Type SourceType { get; set; }
        public Type TargetType { get; set; }
        public ITypeDescription SourceTypeDescription { get; private set; }
        public ITypeDescription TargetTypeDescription { get; private set; }

        public bool CloneListElements { get; set; }

        MemberMatchingRuleCollection MemberMatchingRules { get; set; }

        public ITypeDescriptor SourceTypeDescriptor { get; set; }
        public ITypeDescriptor TargetTypeDescriptor { get; set; }

        public ValueResolving.ValueResolverCollection ValueResolvers { get; set; }

        readonly ConcurrentDictionary<string, IValueResolver> MemberNameToValueResolverMappings 
            = new ConcurrentDictionary<string, IValueResolver>();

        public TypeMappingStrategy(
            Type sourceType,
            Type targetType,
            ITypeDescriptor sourceTypeDescriptor,
            ITypeDescriptor targetTypeDescriptor,
            MemberMatchingRuleCollection memberMatchingRules, 
            ValueResolverCollection valueResolvers)
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

            for(int i = 0; i < memberMatches.Count; i++)
            {
                var m = memberMatches[i];

                MemberNameToValueResolverMappings
                    .AddOrUpdate(
                    m.To.Name,
                    _key => new MatchedMemberValueResolver(m),
                    (_key, _old) => new MatchedMemberValueResolver(m));
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
    }
}
