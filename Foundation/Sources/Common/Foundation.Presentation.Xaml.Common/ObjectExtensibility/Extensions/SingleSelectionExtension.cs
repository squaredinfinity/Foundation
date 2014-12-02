using SquaredInfinity.Foundation.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Foundation.Presentation.ObjectExtensibility.Extensions
{
    public class SingleSelectionExtension : NotifyPropertyChangedObjectExtension<PresentationWrapperCollection>
    {
        public event EventHandler AfterSelectedItemChanged;

        PresentationWrapper _selectedItem;
        /// <summary>
        /// Item of the list that has been selected.
        /// </summary>
        public PresentationWrapper SelectedItem
        {
            get { return _selectedItem; }
            set
            {
                if (TrySetThisPropertyValue(ref _selectedItem, value))
                {
                    if (AfterSelectedItemChanged != null)
                        AfterSelectedItemChanged(this, EventArgs.Empty);
                }
            }
        }

        protected override void OnAttached(PresentationWrapperCollection owner)
        {
            base.OnAttached(owner);

            owner.AfterItemAdded += owner_AfterItemAdded;
            owner.AfterItemRemoved += owner_AfterItemRemoved;
        }

        void owner_AfterItemAdded(object sender, AfterItemAddedEventArgs<PresentationWrapper> e)
        {
            var listItemExtension = e.AddedItem.Extensions.GetOrAdd<ListItemExtension>(() => new ListItemExtension());

            listItemExtension.AfterIsSelectedChanged += listItemExtension_AfterIsSelectedChanged;
        }

        protected override void OnDetached(PresentationWrapperCollection owner)
        {
            owner.AfterItemAdded -= owner_AfterItemAdded;
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
