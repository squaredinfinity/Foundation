using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace SquaredInfinity.Foundation.Presentation.Views
{
    public class View<TViewModel> : View
        where TViewModel: IHostAwareViewModel
    {
        public new TViewModel ViewModel
        {
            get { return (TViewModel) base.ViewModel; }
            set { base.ViewModel = value; }
        }
    }

    public class View : Control
    {
        #region View Model

        public IHostAwareViewModel ViewModel
        {
            get { return (IHostAwareViewModel)GetValue(ViewModelProperty); }
            set { SetValue(ViewModelProperty, value); }
        }

        public static readonly DependencyProperty ViewModelProperty =
            DependencyProperty.Register(
            "ViewModel",
            typeof(IHostAwareViewModel),
            typeof(View),
            new FrameworkPropertyMetadata(null));

        #endregion

        public View()
        {
            RefreshViewModel(oldDataContext: null, newDataContext: null);

            DataContextChanged += (s, e) => RefreshViewModel(e.OldValue, e.NewValue);
        }

        void RefreshViewModel(object oldDataContext, object newDataContext)
        {
            // if View Model Property has binding, then there's nothing to do here
            // only continue with automatic VM Discovery when VM property is not bound
            var vmBinding = BindingOperations.GetBinding(this, ViewModelProperty);

            if (vmBinding != null)
                return;

            OnBeforeOldViewModelRemoved(oldDataContext, ViewModel);

            var newVM = ResolveViewModel(this.GetType(), newDataContext);

            OnBeforeNewViewModelAdded(newDataContext, newVM);

            ViewModel = newVM;
        }

        protected virtual void OnBeforeOldViewModelRemoved(object oldDataContext, IHostAwareViewModel oldViewModel)
        { }

        protected virtual void OnBeforeNewViewModelAdded(object newDataContext, IHostAwareViewModel newViewModel)
        { }

        protected virtual IHostAwareViewModel ResolveViewModel(Type viewType, object newDatacontext)
        {
            return null;
        }
    }
}
