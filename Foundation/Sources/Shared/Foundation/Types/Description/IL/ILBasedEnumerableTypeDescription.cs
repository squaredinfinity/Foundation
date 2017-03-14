using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using SquaredInfinity.Collections.Concurrent;
using System.Collections.Concurrent;
using SquaredInfinity.Extensions;
using SquaredInfinity.Types.Description.Reflection;

namespace SquaredInfinity.Types.Description.IL
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
