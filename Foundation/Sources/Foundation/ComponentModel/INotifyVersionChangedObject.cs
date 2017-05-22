using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.ComponentModel
{
    public interface INotifyVersionChangedObject
    {
        event EventHandler<VersionChangedEventArgs> VersionChanged;

        long Version { get; }
    }
}
