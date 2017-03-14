using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SquaredInfinity.Types.Description
{
    public interface ITypeDescriptor
    {
        ITypeDescription DescribeType(Type type);
    }
}
