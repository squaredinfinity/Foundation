using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Maths.Parsing.Terms
{
    public abstract class Term : ITerm
    {
        public object GetValue()
        {
            return DoGetValue();
        }

        protected abstract object DoGetValue();
    }
}
