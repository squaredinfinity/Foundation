using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SquaredInfinity.Foundation.Extensions;

namespace SquaredInfinity.Foundation.Types.Description
{
    public abstract class TypeMemberDescription : ITypeMemberDescription
    {
        string _rawName;
        public string Name
        {
            get { return _rawName; }
            set { _rawName = value; }
        }

        string _sanitizedName;
        public string SanitizedName
        {
            get { return _sanitizedName; }
            set { _sanitizedName = value; }
        }

        string _assemblyQualifiedMemberTypeName;
        public string AssemblyQualifiedMemberTypeName
        {
            get { return _assemblyQualifiedMemberTypeName; }
            set { _assemblyQualifiedMemberTypeName = value; }
        }

        string _fullMemberTypeName;
        public string FullMemberTypeName
        {
            get { return _fullMemberTypeName; }
            set { _fullMemberTypeName = value; }
        }

        string _memberTypeName;
        public string MemberTypeName
        {
            get { return _memberTypeName; }
            set { _memberTypeName = value; }
        }

        bool _canSetValue;
        public bool CanSetValue
        {
            get { return _canSetValue; }
            set { _canSetValue = value; }
        }

        bool _canGetValue;
        public bool CanGetValue
        {
            get { return _canGetValue; }
            set { _canGetValue = value; }
        }

        MemberVisibility _visibility;
        public MemberVisibility Visibility
        {
            get { return _visibility; }
            set { _visibility = value; }
        }

        ITypeDescription _declaringType;
        public ITypeDescription DeclaringType
        {
            get { return _declaringType; }
            set { _declaringType = value; }
        }

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
