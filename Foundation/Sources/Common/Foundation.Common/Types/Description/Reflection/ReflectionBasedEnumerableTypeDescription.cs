using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Foundation.Types.Description.Reflection
{
    public class ReflectionBasedEnumerableTypeDescription : ReflectionBasedTypeDescription, IEnumerableTypeDescription
    {

        IReadOnlyList<Type> _compatibleItemTypes;
        public IReadOnlyList<Type> CompatibleItemTypes
        {
            get { return _compatibleItemTypes; }
            set { _compatibleItemTypes = value; }
        }

        Type _defaultConcreteItemType;
        public Type DefaultConcreteItemType
        {
            get { return _defaultConcreteItemType; }
            set { _defaultConcreteItemType = value; }
        }


        public bool CanSetCapacity
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public void SetCapacity(object obj, int capacity)
        {
            throw new NotImplementedException();
        }
    }
}
