using SquaredInfinity.Foundation.Presentation.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace SquaredInfinity.Foundation.Presentation.ViewModels
{
    /// <summary>
    /// Base class for all viewmodels
    /// </summary>
    public abstract partial class ViewModel : NotifyPropertyChangedObject, IViewModel
    {
        ViewModelState _state = ViewModelStates.Initialising;
        /// <summary>
        /// Gets or sets the state of a ViewModel
        /// </summary>
        /// <value>The state.</value>
        public ViewModelState State
        {
            get { return _state; }
            set
            {
                _state = value;
                RaisePropertyChanged(() => State);

            }
        }
    }
}
