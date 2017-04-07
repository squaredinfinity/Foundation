﻿using System.Collections;
using System.Windows;
using System.Windows.Input;

namespace SquaredInfinity.Presentation.DragDrop
{
    public interface IDragInfo
    {
        /// <summary>
        /// Gets or sets the drag data.
        /// </summary>
        /// 
        /// <remarks>
        /// This must be set by a drag handler in order for a drag to start.
        /// </remarks>
        object Data { get; set; }

        /// <summary>
        /// Gets the position of the click that initiated the drag, relative to <see cref="VisualSource"/>.
        /// </summary>
        Point DragStartPosition { get; }

        /// <summary>
        /// Gets the point where the cursor was relative to the item being dragged when the drag was started.
        /// </summary>
        Point PositionInDraggedItem { get; }

        DragDropEffects AllowedEffects { get; set; }

        /// <summary>
        /// Gets the mouse button that initiated the drag.
        /// </summary>
        MouseButton MouseButton { get; }

        /// <summary>
        /// Gets the collection that the source ItemsControl is bound to.
        /// </summary>
        /// 
        /// <remarks>
        /// If the control that initated the drag is unbound or not an ItemsControl, this will be null.
        /// </remarks>
        IEnumerable SourceCollection { get; }

        /// <summary>
        /// Gets the position from where the item was dragged.
        /// </summary>
        int SourceIndex { get; }

        /// <summary>
        /// Gets the object that a dragged item is bound to.
        /// </summary>
        /// 
        /// <remarks>
        /// If the control that initated the drag is unbound or not an ItemsControl, this will be null.
        /// </remarks>
        object SourceItem { get; }

        /// <summary>
        /// Gets a collection of objects that the selected items in an ItemsControl are bound to.
        /// </summary>
        /// 
        /// <remarks>
        /// If the control that initated the drag is unbound or not an ItemsControl, this will be empty.
        /// </remarks>
        IEnumerable SourceItems { get; }

        /// <summary>
        /// Gets the control that initiated the drag.
        /// </summary>
        UIElement VisualSource { get; }

        /// <summary>
        /// Gets the item in an ItemsControl that started the drag.
        /// </summary>
        /// 
        /// <remarks>
        /// If the control that initiated the drag is an ItemsControl, this property will hold the item
        /// container of the clicked item. For example, if <see cref="VisualSource"/> is a ListBox this
        /// will hold a ListBoxItem.
        /// </remarks>
        UIElement VisualSourceItem { get; }
        FlowDirection VisualSourceFlowDirection { get; }

        IDataObject DataObject { get; set; }
    }
}