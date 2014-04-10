using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;

namespace SquaredInfinity.Foundation.Types.Description.Reflection
{
    [DebuggerDisplay("{DebuggerDisplay}")]
    public class ReflectionBasedTypeMemberDescription : TypeMemberDescription
    {
        MemberInfo MemberInfo { get; set; }
        PropertyInfo PropertyInfo { get; set; }
        FieldInfo FieldInfo { get; set; }

        public ReflectionBasedTypeMemberDescription(MemberInfo memberInfo)
        {
            this.MemberInfo = memberInfo;

            if(memberInfo is PropertyInfo)
            {
                PropertyInfo = memberInfo as PropertyInfo;
            }
            else if(memberInfo is FieldInfo)
            {
                FieldInfo = memberInfo as FieldInfo;
            }
        }

        public override object GetValue(object obj)
        {
            if(PropertyInfo != null)
            {
                return PropertyInfo.GetValue(obj, null);
            }
            else if (FieldInfo != null)
            {
                return FieldInfo.GetValue(obj);
            }
            else
            {
                throw new NotSupportedException();
            }
        }

        public override void SetValue(object obj, object value)
        {
            if (PropertyInfo != null)
            {
                PropertyInfo.SetValue(obj, value, null);
            }
            else if (FieldInfo != null)
            {
                FieldInfo.SetValue(obj, value);
            }
            else
            {
                throw new NotSupportedException();
            }
        }
    }
}
