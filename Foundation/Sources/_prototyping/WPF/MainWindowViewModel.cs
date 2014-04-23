using SquaredInfinity.Foundation.Presentation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPF
{
    public class MainWindowViewModel : ViewModel
    {
        MainWindow _source;
        public MainWindow Source
        {
            get { return _source; }
            set
            {
                _source = value;
                RaiseThisPropertyChanged();
            }
        }

        public string Text { get; set; }

        public MainWindowViewModel()
        { }

        public MainWindowViewModel(MainWindow source)
        {
            this.Text = ":)";
            this.Source = source;
        }
    }
}
