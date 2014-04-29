using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SquaredInfinity.Foundation.Extensions;

namespace SquaredInfinity.Foundation.Types.Description
{
    public abstract class TypeMemberDescription : ITypeMemberDescription
    {
        public string Name { get; set; }

        public string SanitizedName { get; set; }

        public string AssemblyQualifiedMemberTypeName { get; set; }

        public string FullMemberTypeName { get; set; }

        public string MemberTypeName { get; set; }

        public bool CanSetValue { get; set; }

        public bool CanGetValue { get; set; }

        public MemberVisibility Visibility { get; set; }

        public ITypeDescription DeclaringType { get; set; }

        public bool IsExplicitInterfaceImplementation { get; set; }

        public abstract object GetValue(object obj);
        public abstract void SetValue(object obj, object value);

        public string DebuggerDisplay
        {
            get
            {
                return "{0} ({1})".FormatWith(Name, FullMemberTypeName);
            }
        }
    }
}
