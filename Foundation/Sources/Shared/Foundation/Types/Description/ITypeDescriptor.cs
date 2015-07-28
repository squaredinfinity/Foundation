using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SquaredInfinity.Foundation.Types.Description
{
    public interface ITypeDescriptor
    {
        ITypeDescription DescribeType(Type type);
    }
}
