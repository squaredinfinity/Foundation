using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SquaredInfinity.Types.Description
{
    public interface ITypeMemberDescription
    {
        string Name { get; }
        ITypeDescription MemberType { get; }
        MemberVisibility Visibility { get; }

        ITypeDescription DeclaringType { get; }

        bool CanSetValue { get; }
        bool CanGetValue { get; }

        bool IsExplicitInterfaceImplementation { get; }

        object GetValue(object obj);
        void SetValue(object obj, object value);
    }
}
