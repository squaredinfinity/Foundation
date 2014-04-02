using SquaredInfinity.Foundation.Types.Description;
using SquaredInfinity.Foundation.Types.Description.Reflection;
using SquaredInfinity.Foundation.Types.Mapping.MemberMatching;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SquaredInfinity.Foundation.Types.Mapping
{
    public partial class TypeMappingStrategy
    {
        public static ITypeMappingStrategy CreateTypeMappingStrategy(Type sourceType, Type targetType)
        {
            var result = 
                new TypeMappingStrategy(
                    sourceType, 
                    targetType,
                    new ReflectionBasedTypeDescriptor(),                    
                    new MemberMatchingRuleCollection() { new ExactNameMatchMemberMatchingRule() },
                    valueResolvers: null);

            result.CloneListElements = true;

            return result;
        }
    }
}
