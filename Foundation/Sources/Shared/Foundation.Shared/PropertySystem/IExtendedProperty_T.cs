using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Foundation.PropertySystem
{
    public interface IExtendedProperty<T> : IExtendedProperty
    {
        new T Value { get; set; }
        new T ActualValue { get; }
    }
    

}
