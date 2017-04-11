using SquaredInfinity.Tagging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Presentation.Logic
{
    public class UserActionParameters : TagCollection
    {
        public UserActionParameters()
        { }

        public UserActionParameters(IDictionary<string, object> source)
            : base(source)
        { }
    }
}
