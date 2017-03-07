using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Foundation.Maths
{
    public interface IImplementationCollection<TImplementation> : ICollection<TImplementation>
    {
        TImplementation Default { get; }
        void SetDefault(Func<TImplementation> getDefaultImplementation);
    }
}
