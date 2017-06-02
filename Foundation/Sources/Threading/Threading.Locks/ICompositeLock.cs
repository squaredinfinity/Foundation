using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Threading.Locks
{
    public interface ICompositeLock
    {
        int ChildrenCount { get; }
        void AddChild(ILock childLock);
        void RemoveChild(ILock childLock);
    }
}
