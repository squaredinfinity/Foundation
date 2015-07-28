using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Foundation.Serialization.FlexiXml.TestEntities
{
    public class InterfaceTypeProperty : IDisposable
    {
        public IDisposable MyDispsableProperty { get; set; }

        public void Dispose() { }
    }
}
