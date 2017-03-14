using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using SquaredInfinity.Extensions;
using System.Collections;
using SquaredInfinity.Types.Description.Reflection;

namespace SquaredInfinity.Types.Description.IL
{
    public class ILBasedTypeDescriptor : ReflectionBasedTypeDescriptor
    {
        protected override ITypeDescription DescribeTypeInternal(Type type, ReflectionBasedTypeDescription typeDescription)
        {
            var description = base.DescribeTypeInternal(type, typeDescription) as ReflectionBasedTypeDescription;

            var oldMembers = description.Members;

            var newMembers = new TypeMemberDescriptionCollection();

            for (int i = 0; i < oldMembers.Count; i++)
            {
                var m = description.Members[i] as ReflectionBasedTypeMemberDescription;

                newMembers.Add(new ILBasedTypeMemberDescription(m));
            }

            description.Members = newMembers;

            if (description is ReflectionBasedEnumerableTypeDescription)
                return new ILBasedEnumerableTypeDescription(description as ReflectionBasedEnumerableTypeDescription);
            else
                return new ILBasedTypeDescription(description as ReflectionBasedTypeDescription);
        }
    }
}
