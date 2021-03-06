﻿using System;
using System.Collections;
using System.Windows;
using System.Windows.Controls;
using SquaredInfinity.Foundation.Presentation.DragDrop.Utilities;
using System.Windows.Data;
using System.Linq;
using System.Windows.Input;
using SquaredInfinity.Foundation.Extensions;

namespace SquaredInfinity.Foundation.Presentation.DragDrop
{
    /// <summary>
    /// Holds information about a the target of a drag drop operation.
    /// </summary>
    /// 
    /// <remarks>
    /// The <see cref="DropInfo"/> class holds all of the framework's information about the current 
    /// target of a drag. It is used by <see cref="IDropTarget.DragOver"/> method to determine whether 
    /// the current drop target is valid, and by <see cref="IDropTarget.Drop"/> to perform the drop.
    /// </remarks>
    public class DropInfo : IDropInfo
    {
        /// <summary>
        /// Initializes a new instance of the DropInfo class.
        /// </summary>
        /// 
        /// <param name="sender">
        /// The sender of the drag event.
        /// </param>
        /// 
        /// <param name="e">
        /// The drag event.
        /// </param>
        /// 
        /// <param name="dragInfo">
        /// Information about the source of the drag, if the drag came from within the framework.
        /// </param>
        public DropInfo(object sender, DragEventArgs e, IDragInfo dragInfo)
        {
            var dataFormat = SquaredInfinity.Foundation.Presentation.Behaviors.DragDrop.DataFormat.Name;
            Data = (e.Data.GetDataPresent(dataFormat)) ? e.Data.GetData(dataFormat) : e.Data;
            DragInfo = dragInfo;
            KeyStates = e.KeyStates;
            AllowedEffects = e.Effects;

            if (AllowedEffects.HasFlag(DragDropEffects.Copy) && Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
                ActualDropEffect = DragDropEffects.Copy;
            else
                ActualDropEffect = DragDropEffects.Move;

            VisualTarget = sender as UIElement;
            // if drop target isn't a ItemsControl
            if (!(VisualTarget is ItemsControl))
            {
                // try to find next ItemsControl
                var itemsControl = VisualTarget.FindVisualParent<ItemsControl>();
                if (itemsControl != null)
                {
                    // now check if this ItemsControl is a drop target
                    if (SquaredInfinity.Foundation.Presentation.Behaviors.DragDrop.GetIsDropTarget(itemsControl))
                    {
                        VisualTarget = itemsControl;
                    }
                }
            }
            // visual target can be null, so give us a point...
            this.DropPosition = this.VisualTarget != null ? e.GetPosition(this.VisualTarget) : new Point();

            if (this.VisualTarget is ItemsControl)
            {
                var itemsControl = (ItemsControl)this.VisualTarget;
                var itemContainer = itemsControl.GetItemContainerAt(this.DropPosition);
                var directlyOverItem = itemContainer != null;

                this.TargetGroup = this.FindGroup(itemsControl, this.DropPosition);
                this.VisualTargetOrientation = itemsControl.GetItemsPanelOrientation();
                this.VisualTargetFlowDirection = itemsControl.GetItemsPanelFlowDirection();

                if (itemContainer == null)
                {
                    itemContainer = itemsControl.GetItemContainerAt(this.DropPosition, this.VisualTargetOrientation);
                    directlyOverItem = false;
                }

                if (itemContainer != null)
                {
                    var itemParent = ItemsControl.ItemsControlFromItemContainer(itemContainer);

                    InsertIndex = itemParent.ItemContainerGenerator.IndexFromContainer(itemContainer);
                    TargetCollection = itemParent.ItemsSource ?? itemParent.Items.SourceCollection;

                    var rawItem = itemParent.ItemContainerGenerator.ItemFromContainer(itemContainer);

                    RawTargetCollection = itemParent.GetRawUnderlyingCollection();

                    var targetList = RawTargetCollection as IList;

                    if(targetList != null)
                        RawInsertIndex = targetList.IndexOf(rawItem);

                    if (directlyOverItem || typeof(TreeViewItem).IsAssignableFrom(itemContainer.GetType()))
                    {
                        TargetItem = itemParent.ItemContainerGenerator.ItemFromContainer(itemContainer);
                        VisualTargetItem = itemContainer;
                    }

                    var itemRenderSize = itemContainer.RenderSize;

                    if (VisualTargetOrientation == Orientation.Vertical)
                    {
                        var currentYPos = e.GetPosition(itemContainer).Y;
                        var targetHeight = itemRenderSize.Height;

                        if (currentYPos > targetHeight / 2)
                        {
                            InsertIndex++;
                            RawInsertIndex++;
                            InsertPosition = RelativeInsertPosition.AfterTargetItem;
                        }
                        else
                        {
                            InsertPosition = RelativeInsertPosition.BeforeTargetItem;
                        }

                        if (currentYPos > targetHeight * 0.25 && currentYPos < targetHeight * 0.75)
                        {
                            InsertPosition |= RelativeInsertPosition.TargetItemCenter;
                        }
                    }
                    else
                    {
                        var currentXPos = e.GetPosition(itemContainer).X;
                        var targetWidth = itemRenderSize.Width;

                        if ((VisualTargetFlowDirection == FlowDirection.RightToLeft && currentXPos < targetWidth / 2)
                            || (VisualTargetFlowDirection == FlowDirection.LeftToRight && currentXPos > targetWidth / 2))
                        {
                            InsertIndex++;
                            RawInsertIndex++;
                            InsertPosition = RelativeInsertPosition.AfterTargetItem;
                        }
                        else
                        {
                            InsertPosition = RelativeInsertPosition.BeforeTargetItem;
                        }

                        if (currentXPos > targetWidth * 0.25 && currentXPos < targetWidth * 0.75)
                        {
                            InsertPosition |= RelativeInsertPosition.TargetItemCenter;
                        }
                    }
                }
                else
                {
                    TargetCollection = itemsControl.ItemsSource ?? itemsControl.Items.SourceCollection;
                    RawTargetCollection = TargetCollection;

                    bool insert_at_the_end = false;

                    // no item under mouse,
                    // test if we are at the begining or end of target item control
                    // and based on that position make insertion either as a first or last item

                    if (VisualTargetOrientation == Orientation.Vertical)
                    {
                        if (DropPosition.Y > 5)
                            insert_at_the_end = true;                            
                    }
                    else
                    {
                        if (DropPosition.X > 5)
                            insert_at_the_end = true;
                    }

                    if(insert_at_the_end)
                    {
                        InsertIndex = itemsControl.Items.Count;
                        RawInsertIndex = itemsControl.Items.Count;
                    }
                    else
                    {
                        InsertIndex = 0;
                        RawInsertIndex = 0;
                    }
                }
            }
        }

        private CollectionViewGroup FindGroup(ItemsControl itemsControl, Point position)
        {
            var element = itemsControl.InputHitTest(position) as DependencyObject;

            if (element != null)
            {
                var groupItem = element.FindVisualParent<GroupItem>();

                if (groupItem != null)
                {
                    return groupItem.Content as CollectionViewGroup;
                }
            }

            return null;
        }

        /// <summary>
        /// Gets the drag data.
        /// </summary>
        /// 
        /// <remarks>
        /// If the drag came from within the framework, this will hold:
        /// 
        /// - The dragged data if a single item was dragged.
        /// - A typed IEnumerable if multiple items were dragged.
        /// </remarks>
        public object Data { get; private set; }

        public IEnumerable DataAsEnumerable
        {
            get
            {
                if (Data is IEnumerable && !(Data is string))
                    return (IEnumerable)Data;

                return Enumerable.Repeat(Data, 1);
            }
        }

        /// <summary>
        /// Gets a <see cref="DragInfo"/> object holding information about the source of the drag, 
        /// if the drag came from within the framework.
        /// </summary>
        public IDragInfo DragInfo { get; private set; }

        /// <summary>
        /// Gets the mouse position relative to the VisualTarget
        /// </summary>
        public Point DropPosition { get; private set; }

        /// <summary>
        /// Gets or sets the class of drop target to display.
        /// </summary>
        /// 
        /// <remarks>
        /// The standard drop target adorner classes are held in the <see cref="KnownDropTargetAdorners"/>
        /// class.
        /// </remarks>
        public Type DropTargetAdorner { get; set; }

        /// <summary>
        /// Gets or sets the allowed effects for the drop.
        /// </summary>
        /// 
        /// <remarks>
        /// This must be set to a value other than <see cref="DragDropEffects.None"/> by a drop handler in order 
        /// for a drop to be possible.
        /// </remarks>
        ///       

        DragDropEffects _effects;
        public DragDropEffects AllowedEffects
        {
            get { return _effects; }
            set { _effects = value; }
        }

        /// <summary>
        /// Gets the current insert position within <see cref="TargetCollection"/>.
        /// </summary>
        public int InsertIndex { get; private set; }

        /// <summary>
        /// Gets the collection that the target ItemsControl is bound to.
        /// </summary>
        /// 
        /// <remarks>
        /// If the current drop target is unbound or not an ItemsControl, this will be null.
        /// </remarks>
        public IEnumerable TargetCollection { get; private set; }

        /// <summary>
        /// Gets the object that the current drop target is bound to.
        /// </summary>
        /// 
        /// <remarks>
        /// If the current drop target is unbound or not an ItemsControl, this will be null.
        /// </remarks>
        public object TargetItem { get; private set; }

        /// <summary>
        /// Gets the current group target.
        /// </summary>
        /// 
        /// <remarks>
        /// If the drag is currently over an ItemsControl with groups, describes the group that
        /// the drag is currently over.
        /// </remarks>
        public CollectionViewGroup TargetGroup { get; private set; }

        /// <summary>
        /// Gets the control that is the current drop target.
        /// </summary>
        public UIElement VisualTarget { get; private set; }

        /// <summary>
        /// Gets the item in an ItemsControl that is the current drop target.
        /// </summary>
        /// 
        /// <remarks>
        /// If the current drop target is unbound or not an ItemsControl, this will be null.
        /// </remarks>
        public UIElement VisualTargetItem { get; private set; }

        /// <summary>
        /// Gets the orientation of the current drop target.
        /// </summary>
        public Orientation VisualTargetOrientation { get; private set; }

        /// <summary>
        /// Gets the orientation of the current drop target.
        /// </summary>
        public FlowDirection VisualTargetFlowDirection { get; private set; }

        /// <summary>
        /// Gets and sets the text displayed in the DropDropEffects adorner.
        /// </summary>
        public string DestinationText { get; set; }

        /// <summary>
        /// Gets the relative position the item will be inserted to compared to the TargetItem
        /// </summary>
        public RelativeInsertPosition InsertPosition { get; private set; }

        /// <summary>
        /// Gets a flag enumeration indicating the current state of the SHIFT, CTRL, and ALT keys, as well as the state of the mouse buttons.
        /// </summary>
        public DragDropKeyStates KeyStates { get; private set; }
        
        public DragDropEffects ActualDropEffect { get; set; }

        /// <summary>
        /// Insert index in actual target collection (e.g. collection under collection view)
        /// </summary>
        public int RawInsertIndex { get; set; }

        /// <summary>
        /// Actual target collection (e.g. collection under collection view)
        /// </summary>
        public IEnumerable RawTargetCollection { get; set; }
    }

    [Flags]
    public enum RelativeInsertPosition
    {
        BeforeTargetItem = 0,
        AfterTargetItem = 1,
        TargetItemCenter = 2
    }
}