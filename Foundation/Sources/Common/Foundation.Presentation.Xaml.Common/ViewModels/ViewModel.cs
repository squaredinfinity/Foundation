using SquaredInfinity.Foundation.Presentation.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace SquaredInfinity.Foundation.Presentation.ViewModels
{
    public abstract partial class ViewModel<TDataContext> : ViewModel
    {
        TDataContext _dataContext;
        public new TDataContext DataContext
        {
            get { return (TDataContext) base.DataContext; }
            set
            {
                base.DataContext = value;
                OnAfterDataContextChanged(_dataContext);
                RaiseThisPropertyChanged();
            }
        }

        protected virtual void OnAfterDataContextChanged(TDataContext newDataContext)
        {

        }
    }

    /// <summary>
    /// Base class for all viewmodels
    /// </summary>
    public abstract partial class ViewModel : NotifyPropertyChangedObject, IViewModel
    {
        object _dataContext;
        public object DataContext
        {
            get { return _dataContext; }
            set
            {
                _dataContext = value;
                OnAfterDataContextChanged(_dataContext);
                RaiseThisPropertyChanged();
            }
        }

        protected virtual void OnAfterDataContextChanged(object newDataContext)
        {

        }

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
