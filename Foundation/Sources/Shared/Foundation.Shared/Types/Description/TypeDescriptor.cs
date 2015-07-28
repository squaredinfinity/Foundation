using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Foundation.Types.Description
{
    public static class TypeDescriptor
    {
        public static ITypeDescriptor Default { get; private set; }

        static TypeDescriptor()
        {
            Default = new IL.ILBasedTypeDescriptor();
        }
    }
}
