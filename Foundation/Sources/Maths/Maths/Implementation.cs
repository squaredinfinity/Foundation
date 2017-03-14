using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Maths
{
    public abstract class Implementation : IImplementation
    {
        public string Name { get; protected set; }

        public Implementation()
        {
            this.Name = GetType().Name;
        }
    }
}
