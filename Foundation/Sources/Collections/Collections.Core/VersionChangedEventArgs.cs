﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity
{
    public class VersionChangedEventArgs : EventArgs
    {
        public long NewVersion { get; private set; }

        public VersionChangedEventArgs(long newVersion)
        {
            this.NewVersion = newVersion;
        }
    }
}
