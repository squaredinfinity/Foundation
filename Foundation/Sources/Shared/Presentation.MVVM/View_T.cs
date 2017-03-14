using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using SquaredInfinity.Extensions;
using SquaredInfinity.Presentation.ViewModels;

namespace SquaredInfinity.Presentation.Views
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

        protected override IHostAwareViewModel ResolveViewModel(Type viewType, object newDatacontext)
        {
            return Activator.CreateInstance<TViewModel>();
        }
    }
}
