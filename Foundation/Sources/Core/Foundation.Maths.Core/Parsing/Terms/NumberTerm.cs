using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Foundation.Maths.Parsing.Terms
{
    public class NumberTerm : Term
    {
        public string Input { get; set; }

        protected override object DoGetValue()
        {
            throw new NotImplementedException();
        }
    }
}
