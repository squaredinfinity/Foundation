using SquaredInfinity.Foundation.Collections;
using SquaredInfinity.Foundation.Presentation.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace SquaredInfinity.Foundation.Presentation.Xaml.UITests.Binding
{
    public class ObservableCollectionExViewModel : ViewModel
    {
        public class MyTestEntity : NotifyPropertyChangedObject
        {
            bool _isEnabled;
            public bool IsEnabled
            {
                get { return _isEnabled; }
                set
                {
                    // setting this property value will raise PropertyChanged event
                    // this event will be intercepted by MyCollection and MyCollection.Version will be incremented
                    TrySetThisPropertyValue(ref _isEnabled, value);
                }
            }
        }

        ObservableCollectionEx<MyTestEntity> _myCollection = new ObservableCollectionEx<MyTestEntity>(monitorElementsForChanges: true);
        public ObservableCollectionEx<MyTestEntity> MyCollection
        {
            get { return _myCollection; }
        }

        public ObservableCollectionExViewModel()
        {
            MyCollection.Add(new MyTestEntity { IsEnabled = true });
            MyCollection.Add(new MyTestEntity { IsEnabled = true });
            MyCollection.Add(new MyTestEntity { IsEnabled = true });
        }
    }
}
