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
        IList<ITypeMemberDescription> Members { get; }
    }
}
