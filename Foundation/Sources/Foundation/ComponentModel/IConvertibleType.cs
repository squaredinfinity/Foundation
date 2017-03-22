using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.ComponentModel
{
    public interface IConvertibleType
    {
        bool CanConvertTo(Type destinationType);
        object ConvertTo(Type destinationType);
    }
}
