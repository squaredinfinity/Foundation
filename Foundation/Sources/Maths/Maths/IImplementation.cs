﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Maths
{
    #if DEBUG
    public interface IImplementation
    {
        string Name { get; }
    }
#endif
}