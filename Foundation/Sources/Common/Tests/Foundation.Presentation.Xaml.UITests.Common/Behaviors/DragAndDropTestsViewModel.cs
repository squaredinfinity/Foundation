using SquaredInfinity.Foundation.Collections;
using SquaredInfinity.Foundation.Presentation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Foundation.Presentation.Xaml.UITests.Common.Behaviors
{
    public class DragAndDropTestsViewModel : ViewModel
    {
        public ObservableCollectionEx<int> OddNumbers { get; set; }
        public ObservableCollectionEx<int> EvenNumbers { get; set; }

        public DragAndDropTestsViewModel()
        {
            OddNumbers = new ObservableCollectionEx<int>();
            EvenNumbers = new ObservableCollectionEx<int>();

            for (int i = 0; i < 100; i += 2)
            {
                EvenNumbers.Add(i);
                OddNumbers.Add(i + 1);
            }
        }
    }
}
