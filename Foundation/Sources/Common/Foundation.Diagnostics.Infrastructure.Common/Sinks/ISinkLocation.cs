﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Foundation.Diagnostics.Sinks
{
    public interface ISinkLocation
    {
        string Schema { get; }

        string Location { get; }
    }
}