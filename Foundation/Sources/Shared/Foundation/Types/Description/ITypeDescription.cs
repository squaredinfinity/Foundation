using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SquaredInfinity.Foundation.Types.Description
{
    public interface ITypeDescription
    {
        string AssemblyQualifiedName { get; }
        string FullName { get; }
        string Name { get; }
        string Namespace { get; }
        ITypeMemberDescriptionCollection Members { get; }
        Type Type { get; }

        bool IsValueType { get; }

        object CreateInstance();

        bool AreAllMembersImmutable { get; }
    }
}
