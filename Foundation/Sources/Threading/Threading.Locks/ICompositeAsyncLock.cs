﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Threading.Locks
{
    public interface ICompositeAsyncLock : ICompositeLock
    {
        void AddChild(IAsyncLock childLock);
        void RemoveChild(IAsyncLock childLock);
    }
}