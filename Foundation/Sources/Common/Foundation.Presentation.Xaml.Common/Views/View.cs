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
        #region View Model

        public new TViewModel ViewModel
        {
            get { return (TViewModel)base.ViewModel; }
            set { base.ViewModel = value; }
        }

        #endregion
    }

    public class View : Control
    {
        #region View Model

        /// <summary>
        /// View Model Placeholder is used in late-viewmodel-binding cases.
        /// For example, one view (A) can contain another view (B) inside.
        /// B view model may be bound to A view model (in xaml control template).
        /// This binding will be ready by the time OnApplyTemplate() is called, 
        /// but changing ViewModel property before that (e.g. in constructor) will effecitvely break this binding.
        /// For that reason, OnApplyTemplate() will check if ViewModel property has binding set, 
        /// if it has, ViewModelPlaceholder will be discarded,
        /// otherwise ViewModel DP will be set to Placeholder value.
        /// 
        /// </summary>
        IHostAwareViewModel ViewModelPlaceholder { get; set; }
        bool IsTemplateLoaded { get; set; }

        public IHostAwareViewModel ViewModel
        {
            get 
            {
                if (!IsTemplateLoaded)
                    return ViewModelPlaceholder;

                return (IHostAwareViewModel)GetValue(ViewModelProperty); 
            }
            set 
            {
                if(!IsTemplateLoaded)
                {
                    ViewModelPlaceholder = value;
                    return;
                }

                SetValue(ViewModelProperty, value); 
            }
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

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            IsTemplateLoaded = true;

            // check if view model is bound to something
            // if it is, use the binding value (discard placeholder
            // otherwise use placeholder
            if (IsViewModelBound())
            {
                ViewModelPlaceholder = null;
            }
            else
            {
                ViewModel = ViewModelPlaceholder;
            }
        }

        void RefreshViewModel(object oldDataContext, object newDataContext)
        {
            // if View Model Property has binding, then there's nothing to do here
            // only continue with automatic VM Discovery when VM property is not bound
            if (IsTemplateLoaded && IsViewModelBound())
            {
                return;
            }

            OnBeforeOldViewModelRemoved(oldDataContext, ViewModel);

            var newVM = ResolveViewModel(this.GetType(), newDataContext);

            newVM.DataContext = newDataContext;

            OnBeforeNewViewModelAdded(newDataContext, newVM);

            ViewModel = newVM;
        }

        protected virtual bool IsViewModelBound()
        {
            var vmBinding = BindingOperations.GetBinding(this, ViewModelProperty);

            if (vmBinding == null)
                return false;

            return true;
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
