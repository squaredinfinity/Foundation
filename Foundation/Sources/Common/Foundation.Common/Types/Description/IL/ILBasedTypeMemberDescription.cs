using SquaredInfinity.Foundation.ILGeneration;
using SquaredInfinity.Foundation.Types.Description.Reflection;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Foundation.Types.Description.IL
{
    [DebuggerDisplay("{DebuggerDisplay}")]
    public partial class ILBasedTypeMemberDescription
    {
        PropertySetter MemberSetterValueDelegate;
        PropertyGetter MemberGetterDelegate;

        ReflectionBasedTypeMemberDescription Source;

        public ILBasedTypeMemberDescription(ReflectionBasedTypeMemberDescription source)
        {
            this.Source = source;

            if (Source.MemberInfo is PropertyInfo)
            {
                if(Source.PropertyInfo.CanWrite)
                    MemberSetterValueDelegate = Source.PropertyInfo.EmitSetPropertyValueDelegate();

                if(Source.PropertyInfo.CanRead)
                    MemberGetterDelegate = Source.PropertyInfo.EmitGetPropertyValueDelegate();
            }
            else if (Source.MemberInfo is FieldInfo)
            {
                throw new NotSupportedException();
            }
        }
    }
}
