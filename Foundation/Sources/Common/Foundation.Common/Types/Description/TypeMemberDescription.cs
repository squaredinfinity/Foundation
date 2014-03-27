using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SquaredInfinity.Foundation.Types.Description
{
    public class TypeMemberDescription : ITypeMemberDescription
    {
        string _rawName;
        public string RawName
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
    }
}
