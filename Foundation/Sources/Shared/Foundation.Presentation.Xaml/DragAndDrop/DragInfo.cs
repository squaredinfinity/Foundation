using System;
using System.Collections;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using SquaredInfinity.Foundation.Extensions;
using SquaredInfinity.Foundation.Presentation.DragDrop.Utilities;

namespace SquaredInfinity.Foundation.Presentation.DragDrop
{
    public class DragInfo : IDragInfo
    {

        public static IDragInfo CreateFromEvent(object sender, MouseButtonEventArgs e)
        {
            var dragInfo = new DragInfo();

            var itemsControl = sender as ItemsControl;

            if (itemsControl == null)
                return null;

            dragInfo.VisualSourceFlowDirection = itemsControl.GetItemsPanelFlowDirection();

            var draggedUIElement = e.OriginalSource as UIElement;

            if (draggedUIElement == null && e.OriginalSource is DependencyObject)
                draggedUIElement = (e.OriginalSource as DependencyObject).FindLogicalParent<UIElement>();

            if (draggedUIElement == null)
                return null;

            //# try to get dragged item container
            UIElement draggedItemContainer = itemsControl.GetItemContainer(draggedUIElement);

            if (draggedItemContainer == null)
                draggedItemContainer = itemsControl.GetItemContainerAt(e.GetPosition(itemsControl), itemsControl.GetItemsPanelOrientation());

            if (draggedItemContainer == null)
                return null;

            dragInfo.PositionInDraggedItem = e.GetPosition(draggedItemContainer);

            var itemParent = ItemsControl.ItemsControlFromItemContainer(draggedItemContainer);

            if (itemParent != null)
            {
                dragInfo.SourceCollection = itemParent.ItemsSource ?? itemParent.Items.SourceCollection;
                dragInfo.SourceIndex = itemParent.ItemContainerGenerator.IndexFromContainer(draggedItemContainer);
                dragInfo.SourceItem = itemParent.ItemContainerGenerator.ItemFromContainer(draggedItemContainer);
            }
            else
            {
                return null;
            }

            dragInfo.SourceItems = itemsControl.GetSelectedItems();

            // Some controls (e.g. TreeView) do not update their SelectedItem by this point. 
            // Check to see if there 1 or less item in the SourceItems collection, 
            // and if so, override the control's SelectedItems with the clicked item.

            // TreeView will not update source items at this point,
            // reuse previously found source item
            if (dragInfo.SourceItems.Cast<object>().Count() <= 1)
            {
                dragInfo.SourceItems = Enumerable.Repeat(dragInfo.SourceItem, 1);
            }

            dragInfo.VisualSourceItem = draggedItemContainer;

            dragInfo.DragStartPosition = e.GetPosition((IInputElement)sender);
            dragInfo.AllowedEffects = DragDropEffects.None;
            dragInfo.MouseButton = e.ChangedButton;
            dragInfo.VisualSource = sender as UIElement;

            if (dragInfo.SourceItems == null)
            {
                dragInfo.SourceItems = Enumerable.Empty<object>();
            }

            return dragInfo;
        }

        public static IDragInfo CreateFromEvent(object sender, DragEventArgs e)
        {
            var dragInfo = new DragInfo();

            var itemsControl = sender as ItemsControl;

            if (itemsControl != null)
            {
                dragInfo.VisualSourceFlowDirection = itemsControl.GetItemsPanelFlowDirection();

                if (e.OriginalSource != null)
                {
                    var draggedUIElement = e.OriginalSource as UIElement;

                    if (draggedUIElement == null && e.OriginalSource is DependencyObject)
                        draggedUIElement = (e.OriginalSource as DependencyObject).FindLogicalParent<UIElement>();

                    if (draggedUIElement != null)
                    {
                        //# try to get dragged item container
                        UIElement draggedItemContainer = itemsControl.GetItemContainer(draggedUIElement);

                        if (draggedItemContainer == null)
                            draggedItemContainer = itemsControl.GetItemContainerAt(e.GetPosition(itemsControl), itemsControl.GetItemsPanelOrientation());

                        if (draggedItemContainer == null)
                            return null;

                        dragInfo.PositionInDraggedItem = e.GetPosition(draggedItemContainer);

                        var itemParent = ItemsControl.ItemsControlFromItemContainer(draggedItemContainer);

                        if (itemParent != null)
                        {
                            dragInfo.SourceCollection = itemParent.ItemsSource ?? itemParent.Items.SourceCollection;
                            dragInfo.SourceIndex = itemParent.ItemContainerGenerator.IndexFromContainer(draggedItemContainer);
                            dragInfo.SourceItem = itemParent.ItemContainerGenerator.ItemFromContainer(draggedItemContainer);
                        }

                        if (itemsControl != null)
                        {
                            dragInfo.SourceItems = itemsControl.GetSelectedItems();

                            // Some controls (I'm looking at you TreeView!) haven't updated their
                            // SelectedItem by this point. Check to see if there 1 or less item in 
                            // the SourceItems collection, and if so, override the control's 
                            // SelectedItems with the clicked item.

                            // TreeView will not update source items at this point,
                            // reuse previously found source item
                            if (dragInfo.SourceItems.Cast<object>().Count() < 1)
                            {
                                dragInfo.SourceItems = Enumerable.Repeat(dragInfo.SourceItem, 1);
                            }
                        }

                        dragInfo.VisualSourceItem = draggedItemContainer;
                    }
                }
            }

            dragInfo.DragStartPosition = e.GetPosition((IInputElement)sender);
            dragInfo.AllowedEffects = DragDropEffects.None;
            dragInfo.VisualSource = sender as UIElement;

            if (dragInfo.SourceItems == null)
            {
                dragInfo.SourceItems = Enumerable.Empty<object>();
            }

            return dragInfo;
        }

        private DragInfo()
        {
            
        }

        /// <summary>
        /// Gets or sets the drag data.
        /// </summary>
        /// 
        /// <remarks>
        /// This must be set by a drag handler in order for a drag to start.
        /// </remarks>
        public object Data { get; set; }

        /// <summary>
        /// Gets the position of the click that initiated the drag, relative to <see cref="VisualSource"/>.
        /// </summary>
        public Point DragStartPosition { get; private set; }

        /// <summary>
        /// Gets the point where the cursor was relative to the item being dragged when the drag was started.
        /// </summary>
        public Point PositionInDraggedItem { get; private set; }

        /// <summary>
        /// Gets or sets the allowed effects for the drag.
        /// </summary>
        /// 
        /// <remarks>
        /// This must be set to a value other than <see cref="DragDropEffects.None"/> by a drag handler in order 
        /// for a drag to start.
        /// </remarks>
        public DragDropEffects AllowedEffects { get; set; }

        /// <summary>
        /// Gets the mouse button that initiated the drag.
        /// </summary>
        public MouseButton MouseButton { get; private set; }

        /// <summary>
        /// Gets the collection that the source ItemsControl is bound to.
        /// </summary>
        /// 
        /// <remarks>
        /// If the control that initated the drag is unbound or not an ItemsControl, this will be null.
        /// </remarks>
        public IEnumerable SourceCollection { get; private set; }

        /// <summary>
        /// Gets the position from where the item was dragged.
        /// </summary>
        /// <value>The index of the source.</value>
        public int SourceIndex { get; private set; }

        /// <summary>
        /// Gets the object that a dragged item is bound to.
        /// </summary>
        /// 
        /// <remarks>
        /// If the control that initated the drag is unbound or not an ItemsControl, this will be null.
        /// </remarks>
        public object SourceItem { get; private set; }

        /// <summary>
        /// Gets a collection of objects that the selected items in an ItemsControl are bound to.
        /// </summary>
        /// 
        /// <remarks>
        /// If the control that initated the drag is unbound or not an ItemsControl, this will be empty.
        /// </remarks>
        public IEnumerable SourceItems { get; private set; }

        /// <summary>
        /// Gets the control that initiated the drag.
        /// </summary>
        public UIElement VisualSource { get; private set; }

        /// <summary>
        /// Gets the item in an ItemsControl that started the drag.
        /// </summary>
        /// 
        /// <remarks>
        /// If the control that initiated the drag is an ItemsControl, this property will hold the item
        /// container of the clicked item. For example, if <see cref="VisualSource"/> is a ListBox this
        /// will hold a ListBoxItem.
        /// </remarks>
        public UIElement VisualSourceItem { get; private set; }

        /// <summary>
        /// Gets the FlowDirection of the current drag source.
        /// </summary>
        public FlowDirection VisualSourceFlowDirection { get; private set; }

        /// <summary>
        /// Gets the <see cref="IDataObject"/> which is used by the drag and drop operation. Set it to
        /// a custom instance if custom drag and drop behavior is needed.
        /// </summary>
        public IDataObject DataObject { get; set; }
    }
}