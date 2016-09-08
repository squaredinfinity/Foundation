using SquaredInfinity.Foundation.IntraMessaging;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace SquaredInfinity.Foundation.Presentation
{
    public interface IViewModel : IIntraMessageNode, IDisposable
    {
        object DataContext { get; set; }

        ViewModelState State { get; set; }

        bool IsInitialized { get; }
        
        bool IsHostedInDialogWindow { get; }


        void Initialize(bool isHostedInDialogWindow);
    }
}
