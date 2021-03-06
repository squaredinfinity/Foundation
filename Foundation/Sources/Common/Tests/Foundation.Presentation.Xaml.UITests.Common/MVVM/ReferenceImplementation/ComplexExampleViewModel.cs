﻿using SquaredInfinity.Foundation.Presentation.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Foundation.Presentation.Xaml.UITests.MVVM.ReferenceImplementation
{
    public class ComplexExampleViewModel : ViewModel<ICollection<int>>
    {
        object _selectedValue;
        public object SelectedValue
        {
            get { return _selectedValue; }
            set { TrySetThisPropertyValue(ref _selectedValue, value); }
        }

        protected override void OnAfterDataContextChanged(ICollection<int> newDataContext)
        {
            base.OnAfterDataContextChanged(newDataContext);
        }

        protected override void OnAfterDataContextChanged(object newDataContext)
        {
            base.OnAfterDataContextChanged(newDataContext);
        }
    }
}
