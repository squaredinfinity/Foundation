using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using SquaredInfinity.Foundation.Extensions;

namespace SquaredInfinity.Foundation.Types.Description.Reflection
{
    [DebuggerDisplay("{DebuggerDisplay}")]
    public class ReflectionBasedTypeMemberDescription : ITypeMemberDescription, IEquatable<ITypeMemberDescription>
    {
        string _name;
        public string Name 
        {
            get { return _name; }
            set 
            {
                _name = value;
                _hashCode = value.GetHashCode();
            }
        }

        public string SanitizedName { get; set; }

        public string AssemblyQualifiedMemberTypeName { get; set; }

        public ITypeDescription MemberType { get; set; }

        public bool CanSetValue { get; set; }

        public bool CanGetValue { get; set; }

        public MemberVisibility Visibility { get; set; }

        public ITypeDescription DeclaringType { get; set; }

        public bool IsExplicitInterfaceImplementation { get; set; }

        public string DebuggerDisplay
        {
            get
            {
                return "{0} ({1})".FormatWith(Name, MemberType.FullName);
            }
        }

        public MemberInfo MemberInfo { get; private set; }
        public PropertyInfo PropertyInfo { get; private set; }
        public FieldInfo FieldInfo { get; private set; }
        
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

        public virtual object GetValue(object obj)
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

        public virtual void SetValue(object obj, object value)
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

            return string.Equals(Name, other.Name);
        }
    }
}
