using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Extensions
{
    public static class MemberInfoExtensions
    {
        public static bool IsStatic(this MemberInfo member)
        {
            var field = member as FieldInfo;
            if (field != null)
                return field.IsStatic;

            var property = member as PropertyInfo;
            if (property != null)
            {
                if(property.CanRead)
                {
                    return property.GetGetMethod(nonPublic: true).IsStatic;
                }
                else
                {   
                    return property.GetSetMethod(nonPublic: true).IsStatic;
                }
            }

            var method = member as MethodInfo;
            if (method != null)
                return method.IsStatic;

            throw new NotSupportedException();
        }
    }
}
