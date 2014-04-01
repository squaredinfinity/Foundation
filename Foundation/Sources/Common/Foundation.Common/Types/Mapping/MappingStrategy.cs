using SquaredInfinity.Foundation.Types.Description;
using SquaredInfinity.Foundation.Types.MemberMatching;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SquaredInfinity.Foundation.Types.Mapping
{
    public partial class MappingStrategy : IMappingStrategy
    {
        public bool CloneListElements { get; set; }

        IList<IMemberMatchingStrategy> MemberMatchingStrategies { get; set; }

        ITypeDescriptor TypeDescriptor { get; set; }

        public MappingStrategy()
            : this(new Description.Reflection.ReflectionBasedTypeDescriptor(), new List<IMemberMatchingStrategy>())
        { }

        public MappingStrategy(ITypeDescriptor typeDescriptor, IList<IMemberMatchingStrategy> memberMappingStrategies)
        {
            this.TypeDescriptor = typeDescriptor;
            this.MemberMatchingStrategies = memberMappingStrategies;
        }

        public MemberMatchCollection GetMemberMappings(Type sourceType, Type targetType)
        {
            var result = new MemberMatchCollection();

            //# Describe both source and target types

            var sourceDescription = TypeDescriptor.DescribeType(sourceType);

            var targetDescription = (ITypeDescription)null;

            if (sourceType == targetType)
                targetDescription = sourceDescription;
            else
                targetDescription = TypeDescriptor.DescribeType(targetType);

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

                for (int i = 0; i < MemberMatchingStrategies.Count; i++)
                {
                    var ms = MemberMatchingStrategies[i];

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
    }
}
