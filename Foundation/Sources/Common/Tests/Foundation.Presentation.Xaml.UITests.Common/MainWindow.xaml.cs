using SquaredInfinity.Foundation.Presentation.DragDrop;
using SquaredInfinity.Foundation.Presentation.ViewModels;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
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
using SquaredInfinity.Foundation.Extensions;
using SquaredInfinity.Foundation.Collections.Trees;
using SquaredInfinity.Foundation.Presentation.Windows;
using SquaredInfinity.Foundation.Collections;

namespace Foundation.Presentation.Xaml.UITests.Common
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            DataContext = this;

            //var tree = new BooleanExpressionTree();

            //var or_1 = new PredicateConnectiveNode { Mode = PredicateConnectiveMode.OR };
            
            //var and_2 = new PredicateConnectiveNode { Mode = PredicateConnectiveMode.AND };
            //and_2.InsertLeft(new PredicateNode { Attribute = "5", Operator = "==", Comparand = "5" });
            //and_2.InsertRight(new PredicateNode { Attribute = "6", Operator = "==", Comparand = "6" });
            //or_1.InsertRight(and_2);

            //var or_3 = new PredicateConnectiveNode { Mode = PredicateConnectiveMode.OR };
            //or_3.InsertRight(new PredicateNode { Attribute = "4", Operator = "==", Comparand = "4" });

            //var or_4 = new PredicateConnectiveNode { Mode = PredicateConnectiveMode.OR };
            //or_4.InsertRight(new PredicateNode { Attribute = "3", Operator = "==", Comparand = "3" });

            //var and_5 = new PredicateConnectiveNode { Mode = PredicateConnectiveMode.AND };
            //and_5.InsertLeft(new PredicateNode { Attribute = "1", Operator = "==", Comparand = "1" });
            //and_5.InsertRight(new PredicateNode { Attribute = "2", Operator = "==", Comparand = "2" });

            //or_4.InsertLeft(and_5);
            //or_3.InsertLeft(or_4);
            //or_1.InsertLeft(or_3);

            //tree.Root = or_1;

            //var x = tree.Root.Evaluate();

            //Tree = tree;

            //Root = new BooleanExpressionTreeTraversingCollection(Tree.Root);

            InitializeComponent();
        }
    }
}
