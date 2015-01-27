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

namespace Foundation.Presentation.Xaml.UnitTests.Common.Collections.__Resources
{
    /// <summary>
    /// Interaction logic for ObservableCollection__BackgroundUpdatesTest.xaml
    /// </summary>
    public partial class ObservableCollection__BackgroundUpdatesTest : Window
    {
        ObservableCollectionEx<int> _items = new XamlObservableCollectionEx<int>();
        public ObservableCollectionEx<int> Items
        {
            get { return _items; }
        }

        public ObservableCollection__BackgroundUpdatesTest()
        {
            DataContext = this;
            InitializeComponent();
        }
    }
}
