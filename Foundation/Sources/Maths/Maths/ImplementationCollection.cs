using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Maths
{
#if DEBUG

    public class ImplementationCollection<TImplementation>
        : Collection<TImplementation>,
        IImplementationCollection<TImplementation>
    {

        public TImplementation Default
        {
            get { return GetDefault(); }
        }

        Func<TImplementation> GetDefault;

        public void SetDefault(Func<TImplementation> getDefaultImplementation)
        {
            GetDefault = getDefaultImplementation;
        }

        public ImplementationCollection()
        {
            GetDefault = () => this.FirstOrDefault();
        }
    }
#endif
}
