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

namespace Windows.Behaviors.Examples.DotNet
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public ICommand ExecuteClick { get; set; }
        public ICommand ExecuteDoubleClick { get; set; }

        public MainWindow()
        {
            DataContext = this;

            ExecuteClick = new cmd(() => MessageBox.Show("Click"));
            ExecuteDoubleClick = new cmd(() => MessageBox.Show("Double Click"));

            InitializeComponent();
        }
    }

    class cmd : ICommand
    {
        public event EventHandler CanExecuteChanged;

        Action Action;

        public cmd(Action action)
        {
            this.Action = action;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            Action();
        }
    }

}
