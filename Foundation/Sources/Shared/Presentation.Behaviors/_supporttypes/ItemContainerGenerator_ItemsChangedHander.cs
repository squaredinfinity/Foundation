using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;

namespace SquaredInfinity.Presentation.Behaviors
{
    internal class ItemContainerGenerator_ItemsChangedHander : IDisposable
    {
        readonly ListView ListViewReference;
        readonly Action<ListView> Action;

        public ItemContainerGenerator_ItemsChangedHander(ListView listView, Action<ListView> action)
        {
            this.ListViewReference = listView;
            this.Action = action;

            listView.ItemContainerGenerator.ItemsChanged += OnItemsChanged;
        }

        void OnItemsChanged(object sender, System.Windows.Controls.Primitives.ItemsChangedEventArgs e)
        {
            Action(ListViewReference);
        }

        public void Dispose()
        {
            ListViewReference.ItemContainerGenerator.ItemsChanged -= OnItemsChanged;
        }
    }
}
