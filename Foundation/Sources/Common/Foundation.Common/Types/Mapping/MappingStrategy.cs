using SquaredInfinity.Foundation.Types.Description;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SquaredInfinity.Foundation.Types.Mapping
{
    public class MappingStrategy
    {
        public bool CopyListElements { get; set; }

        public IList<IMemberMappingStrategy> MemberMappingStrategies { get; private set; }

        public ITypeDescriptor TypeDescriptor { get; private set; }

        public MappingStrategy()
        {
            this.MemberMappingStrategies = new List<IMemberMappingStrategy>();

            this.TypeDescriptor = new Description.Reflection.ReflectionBasedTypeDescriptor();
        }

        public MemberMappingCollection GetMemberMappings(Type sourceType, Type targetType)
        {
            var result = new MemberMappingCollection();

            //# Describe both source and target types

            var sourceDescription = TypeDescriptor.DescribeType(sourceType);

            var targetDescription = (ITypeDescription)null;

            if (sourceType == targetType)
                targetDescription = sourceDescription;
            else
                targetDescription = TypeDescriptor.DescribeType(targetType);

            // list of source members
            var sourceMembers = 
                (from sourceDescription.Members.ToList();

            for (int i_member = 0; i_member < targetDescription.Members.Count; i_member++)
            {
                var targetMember = targetDescription.Members[i_member];

                if (!targetMember.CanSetValue)
                    continue;

                for (int i = 0; i < MemberMappingStrategies.Count; i++)
                {
                    var ms = MemberMappingStrategies[i];

                    var sourceMember = (ITypeMemberDescription) null;

                    if (ms.TryMapMembers(targetMember, sourceMembers, out sourceMember))
                    {
                        sourceMembers.Remove(sourceMember);

                        var mapping = new MemberMapping
                        {
                            From = sourceMember,
                            To = targetMember
                        };

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
