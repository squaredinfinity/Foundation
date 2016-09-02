using SquaredInfinity.Foundation.Collections;
using SquaredInfinity.Foundation.Presentation.Windows;
using SquaredInfinity.Foundation.Presentation.Xaml.Styles.Modern.Windows;
using SquaredInfinity.Foundation.Tagging;
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

namespace Foundation.Presentation.Xaml.UITests
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : ModernWindow
    {
        public MainWindow()
        {
            InitializeComponent();

            var tc = new TagCollection();
            tc.Add("one", 1);
            tc.Add("one", 2);

            var tc2 = new TagCollection();
            tc2.AddOrUpdateFrom(tc);
            tc2.AddOrUpdateFrom(tc);

            int a = 1;
        }
    }
}
