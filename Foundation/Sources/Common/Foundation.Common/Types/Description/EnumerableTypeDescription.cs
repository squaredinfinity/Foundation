using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Foundation.Types.Description
{
    public class EnumerableTypeDescription : TypeDescription, IEnumerableTypeDescription
    {
        IReadOnlyList<Type> _compatibleTypes;
        public IReadOnlyList<Type> CompatibleItemTypes
        {
            get { return _compatibleTypes; }
            set { _compatibleTypes = value; }
        }
    }
}
