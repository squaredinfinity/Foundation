using SquaredInfinity.Foundation.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Foundation.Presentation.ObjectExtensibility.Extensions
{

    public class AfterSelectedItemChangedArgs : EventArgs
    {
        public object OldItem { get; private set; }
        public object NewItem { get; private set; }

        public AfterSelectedItemChangedArgs(object oldItem, object newItem)
        {
            this.OldItem = oldItem;
            this.NewItem = newItem;
        }
    }

    public class SingleSelectionExtension : NotifyPropertyChangedObjectExtension<PresentationWrapperCollection>
    {
        public event EventHandler<AfterSelectedItemChangedArgs> AfterSelectedItemChanged;

        PresentationWrapper _selectedItem;
        /// <summary>
        /// Item of the list that has been selected.
        /// </summary>
        public PresentationWrapper SelectedItem
        {
            get { return _selectedItem; }
            set
            {
                var oldValue = _selectedItem;

                if (TrySetThisPropertyValue(ref _selectedItem, value))
                {
                    if (AfterSelectedItemChanged != null)
                        AfterSelectedItemChanged(this, new AfterSelectedItemChangedArgs(oldValue, value));
                }
            }
        }

        protected override void OnAttached(PresentationWrapperCollection owner)
        {
            base.OnAttached(owner);

            // add handlers to existing items
            foreach(var item in owner)
            {
                var listItemExtension = item.Extensions.GetOrAdd<ListItemExtension>(() => new ListItemExtension());

                listItemExtension.AfterIsSelectedChanged -= listItemExtension_AfterIsSelectedChanged;
                listItemExtension.AfterIsSelectedChanged += listItemExtension_AfterIsSelectedChanged;
            }

            // make sure future changes to collection are handled
            owner.AfterItemInserted += owner_AfterItemAdded;
            owner.AfterItemRemoved += owner_AfterItemRemoved;
        }

        void owner_AfterItemAdded(object sender, AfterItemAddedEventArgs<PresentationWrapper> e)
        {
            var listItemExtension = e.AddedItem.Extensions.GetOrAdd<ListItemExtension>(() => new ListItemExtension());

            listItemExtension.AfterIsSelectedChanged -= listItemExtension_AfterIsSelectedChanged;
            listItemExtension.AfterIsSelectedChanged += listItemExtension_AfterIsSelectedChanged;
        }

        protected override void OnDetached(PresentationWrapperCollection owner)
        {
            owner.AfterItemInserted -= owner_AfterItemAdded;
            owner.AfterItemRemoved -= owner_AfterItemRemoved;

            base.OnDetached(owner);
        }

        void owner_AfterItemRemoved(object sender, AfterItemRemovedEventArgs<PresentationWrapper> e)
        {
            var listItemExtension = (ListItemExtension)e.RemovedItem.Extensions[typeof(ListItemExtension)];

            listItemExtension.AfterIsSelectedChanged -= listItemExtension_AfterIsSelectedChanged;
        }

        void listItemExtension_AfterIsSelectedChanged(object sender, EventArgs e)
        {
            var li = sender as ListItemExtension;

            if (object.Equals(li.Owner, SelectedItem))
            {
                if (li.IsSelected == false)
                    SelectedItem = null;
            }
            else
            {
                if (SelectedItem != null)
                {
                    ((ListItemExtension)SelectedItem.Extensions[typeof(ListItemExtension)]).IsSelected = false;
                }

                if (li.IsSelected)
                    SelectedItem = li.Owner;
                else
                    SelectedItem = null;
            }
        }
    }
}
