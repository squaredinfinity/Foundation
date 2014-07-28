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
    public partial class ILBasedTypeMemberDescription : ITypeMemberDescription
    {

        string ITypeMemberDescription.Name
        {
            get { return Source.Name; }
        }

        ITypeDescription ITypeMemberDescription.MemberType
        {
            get { return Source.MemberType; }
        }

        MemberVisibility ITypeMemberDescription.Visibility
        {
            get { return Source.Visibility; }
        }

        ITypeDescription ITypeMemberDescription.DeclaringType
        {
            get { return Source.DeclaringType; }
        }

        bool ITypeMemberDescription.CanSetValue
        {
            get { return Source.CanSetValue; }
        }

        bool ITypeMemberDescription.CanGetValue
        {
            get { return Source.CanGetValue; }
        }

        bool ITypeMemberDescription.IsExplicitInterfaceImplementation
        {
            get { return Source.IsExplicitInterfaceImplementation; }
        }

        object ITypeMemberDescription.GetValue(object obj)
        {
            return MemberGetterDelegate(obj);
        }

        void ITypeMemberDescription.SetValue(object obj, object value)
        {
            MemberSetterValueDelegate(obj, value);
        }
    }
}
