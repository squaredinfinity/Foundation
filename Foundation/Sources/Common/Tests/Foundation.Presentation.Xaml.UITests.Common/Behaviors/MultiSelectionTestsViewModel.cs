using SquaredInfinity.Foundation.Collections;
using SquaredInfinity.Foundation.Presentation;
using SquaredInfinity.Foundation.Presentation.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Foundation.Presentation.Xaml.UITests.Common.Behaviors
{
    public class MultiSelectionTestsViewModel : ViewModel
    {
        public ObservableCollectionEx<int> AllItems { get; set; }

        ObservableCollectionEx<int> _selectedItems;
        public ObservableCollectionEx<int> SelectedItems 
        {
            get { return _selectedItems; }
            set 
            {
                _selectedItems = value;
                RaiseThisPropertyChanged();
            }
        }

        public MultiSelectionTestsViewModel()
        {
            AllItems = new ObservableCollectionEx<int>();
            SelectedItems = new ObservableCollectionEx<int>();

            for (int i = 0; i < 100; i += 2)
            {
                AllItems.Add(i);
            }
        }
    }
}
