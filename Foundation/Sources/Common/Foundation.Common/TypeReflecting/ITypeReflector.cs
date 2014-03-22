using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Foundation.TypeReflecting
{
    public interface ITypeReflector
    {
        IList<ReflectedPropertyInfo> GetProperties();

        object GetValue(object obj, string memberName);
        void SetValue(object obj, string memberName, object newValue);
    }
}
