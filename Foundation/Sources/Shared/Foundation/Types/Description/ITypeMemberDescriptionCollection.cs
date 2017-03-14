using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Types.Description
{
    public interface ITypeMemberDescriptionCollection : IReadOnlyList<ITypeMemberDescription>
    {
        ITypeMemberDescription FindMember(string memberName);
    }
}
