using SquaredInfinity.Foundation.Collections;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using SquaredInfinity.Foundation.Extensions;
using System.Dynamic;
using System.ComponentModel;

namespace WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        ObservableCollectionEx<MyItem> _MyItems;
        public ObservableCollectionEx<MyItem> MyItems 
        {
            get { return _MyItems; }
            set { _MyItems = value; if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("MyItems")); }
        }

        public MainWindow()
        {
            

            

            DataContext = this;
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var x = new ObservableCollectionEx<MyItem>();

            for (int i = 0; i < 100; i++)
                x.Add(new MyItem { Id = i });

            MyItems = x;
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }


   

    public class MyItem
    {
        int _id;
        public int Id 
        {
            get { return _id; }
            set { _id = value; }
        }

        public override string ToString()
        {
            return Id.ToString();
        }
    }

}
