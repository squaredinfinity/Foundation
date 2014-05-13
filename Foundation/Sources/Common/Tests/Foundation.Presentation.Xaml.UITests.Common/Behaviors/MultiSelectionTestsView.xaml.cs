using SquaredInfinity.Foundation.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Foundation.Presentation.Xaml.UITests.Common.Behaviors
{
    /// <summary>
    /// Interaction logic for MultiSelectionTestsView.xaml
    /// </summary>
    public partial class MultiSelectionTestsView : UserControl
    {
        #region Selected Items DP



        public ObservableCollectionEx<int> SelectedItemsDP
        {
            get { return (ObservableCollectionEx<int>)GetValue(SelectedItemsDPProperty); }
            set { SetValue(SelectedItemsDPProperty, value); }
        }

        public static readonly DependencyProperty SelectedItemsDPProperty =
            DependencyProperty.Register(
            "SelectedItemsDP",
            typeof(ObservableCollectionEx<int>),
            typeof(MultiSelectionTestsView), 
            // NOTE: Default value must be set to an actual instance (i.e. not null)
            new FrameworkPropertyMetadata(new ObservableCollectionEx<int>()));



        #endregion

        public MultiSelectionTestsView()
        {
            DataContext = new MultiSelectionTestsViewModel();

            InitializeComponent();
        }
    }
}
