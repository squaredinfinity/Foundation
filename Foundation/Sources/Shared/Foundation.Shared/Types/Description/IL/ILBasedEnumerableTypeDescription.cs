using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using SquaredInfinity.Foundation.Collections.Concurrent;
using System.Collections.Concurrent;
using SquaredInfinity.Foundation.Extensions;
using SquaredInfinity.Foundation.Types.Description.Reflection;

namespace SquaredInfinity.Foundation.Types.Description.IL
{
    public partial class ILBasedEnumerableTypeDescription : ILBasedTypeDescription
    {
        ReflectionBasedEnumerableTypeDescription EnumerableSource;

        public ILBasedEnumerableTypeDescription(ReflectionBasedEnumerableTypeDescription source)
            : base (source)
        {
            this.EnumerableSource = source;
        }
    }
}
