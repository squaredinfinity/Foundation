﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Foundation.Threading
{
    public enum LockTypes
    {
        Read,
        Write,
        UpgradeableRead
    }
}