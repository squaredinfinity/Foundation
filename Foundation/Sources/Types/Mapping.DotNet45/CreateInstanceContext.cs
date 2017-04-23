using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SquaredInfinity.Types.Mapping
{
    public class CreateInstanceContext
    {
        public bool IsFullyConstructed { get; private set; }
        public void MarkAsFullyConstructed()
        {
            IsFullyConstructed = true;
        }
    }
}
