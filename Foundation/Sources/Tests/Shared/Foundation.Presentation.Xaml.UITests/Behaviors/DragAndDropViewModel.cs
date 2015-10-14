using SquaredInfinity.Foundation.Collections;
using SquaredInfinity.Foundation.Presentation.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace SquaredInfinity.Foundation.Presentation.Xaml.UITests.Behaviors
{
    public class DragAndDropViewModel : ViewModel
    {
        public XamlObservableCollectionEx<MyObservableHierarchyLevelA> MyHierarchy { get; set; }

        public DragAndDropViewModel()
        {
            MyHierarchy = new XamlObservableCollectionEx<MyObservableHierarchyLevelA>();

            var a1 = new MyObservableHierarchyLevelA { Name = "A 1" };
            var b11 = new MyObservableHierarchyLevelB { Name = "B 11" };
            var b12 = new MyObservableHierarchyLevelB { Name = "B 12" };
            a1.Items.AddRange(new[] { b11, b12 });

            var a2 = new MyObservableHierarchyLevelA { Name = "A 2" };
            var b21 = new MyObservableHierarchyLevelB { Name = "B 21" };
            var b22 = new MyObservableHierarchyLevelB { Name = "B 22" };
            a2.Items.AddRange(new[] { b21, b22 });

            var a3 = new MyObservableHierarchyLevelA { Name = "A 3" };
            var b31 = new MyObservableHierarchyLevelB { Name = "B 31" };
            var b32 = new MyObservableHierarchyLevelB { Name = "B 32" };
            a3.Items.AddRange(new[] { b31, b32 });

            MyHierarchy.AddRange(new[] { a1, a2, a3 });
        }
    }

    public class MyObservableHierarchyLevelA : NotifyPropertyChangedObject
    {
        string _name;
        public string Name
        {
            get { return _name; }
            set { TrySetThisPropertyValue(ref _name, value); }
        }

        XamlObservableCollectionEx<MyObservableHierarchyLevelB> _items = new XamlObservableCollectionEx<MyObservableHierarchyLevelB>();
        public XamlObservableCollectionEx<MyObservableHierarchyLevelB> Items
        {
            get { return _items; }
        }
    }

    public class MyObservableHierarchyLevelB : NotifyPropertyChangedObject
    {
        string _name;
        public string Name
        {
            get { return _name; }
            set { TrySetThisPropertyValue(ref _name, value); }
        }
    }
}
