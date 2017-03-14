using SquaredInfinity.IntraMessaging;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;

namespace SquaredInfinity.Presentation
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
