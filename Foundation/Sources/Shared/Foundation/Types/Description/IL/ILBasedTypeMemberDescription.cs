using SquaredInfinity.ILGeneration;
using SquaredInfinity.Types.Description.Reflection;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Types.Description.IL
{
    [DebuggerDisplay("{DebuggerDisplay}")]
    public partial class ILBasedTypeMemberDescription : IEquatable<ITypeMemberDescription>
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

            _hashCode = Source.Name.GetHashCode();
        }

        int _hashCode;

        public override int GetHashCode()
        {
            return _hashCode;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as ITypeMemberDescription);
        }

        public bool Equals(ITypeMemberDescription other)
        {
            if (other == null)
                return false;

            return string.Equals(Source.Name, other.Name);
        }
    }
}
