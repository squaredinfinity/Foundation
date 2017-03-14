using SquaredInfinity.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Types.Description
{
    public class TypeMemberDescriptionCollection : CollectionEx<ITypeMemberDescription>, ITypeMemberDescriptionCollection
    {
        public ITypeMemberDescription FindMember(string memberName)
        {
            foreach(var m in this)
            {
                if (string.Equals(m.Name, memberName))
                    return m;
            }

            return null;
        }
    }
}
