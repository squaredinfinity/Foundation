﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Cache
{
    public interface ISupportsCaching
    {
        bool IsCacheEnabled { get; set; }
        void ClearCache();
    }
}
