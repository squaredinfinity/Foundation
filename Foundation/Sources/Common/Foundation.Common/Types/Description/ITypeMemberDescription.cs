using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SquaredInfinity.Foundation.Types.Description
{
    public interface ITypeMemberDescription
    {
        /// <summary>
        /// e.g. XXX (property) or GetXXX() (method)
        /// </summary>
        string RawName { get; }
        string SanitizedName { get; }

        string AssemblyQualifiedMemberTypeName { get; }
        string FullMemberTypeName { get; }
        string MemberTypeName { get; }
    }
}
