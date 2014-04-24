using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows;
using SquaredInfinity.Foundation.Presentation.DragDrop.Utilities;
using System.Windows.Controls;
using System.ComponentModel;
using System.Windows.Input;
using System.Windows.Data;

namespace SquaredInfinity.Foundation.Presentation.DragDrop
{
    /// <summary>
    /// Default Drop Target.
    /// Handles Most basic cases.
    /// </summary>
    public partial class DefaultDropTarget : IDropTarget
    {
        public virtual void DragOver(IDropInfo dropInfo)
        {
            if (CanAcceptData(dropInfo))
            {
                bool isSameCollection = dropInfo.TargetCollection == dropInfo.DragInfo.SourceCollection;

                // copy within the same collection not supported at the moment,
                // but it could be added with help of TypeMapper or som customization on user side
                if (!isSameCollection && (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl)))
                    dropInfo.AllowedEffects = DragDropEffects.Copy;
                else
                    dropInfo.AllowedEffects = DragDropEffects.Move;

                dropInfo.DropTargetAdorner = KnownDropTargetAdorners.Insert;
            }
        }

        public virtual void Drop(IDropInfo dropInfo)
        {
            var insertIndex = dropInfo.InsertIndex;

            var targetList = dropInfo.TargetCollection.AsList();
            var sourceList = dropInfo.DragInfo.SourceCollection.AsList();

            var data = dropInfo.DataAsEnumerable;

            // remove items from source and add to target

            if (sourceList == targetList)
            {
                // operation within the same list
                if (dropInfo.ActualDropEffect == DragDropEffects.Move)
                {
                    var dataCopy = new List<object>(data.Cast<object>());
                   
                    foreach(var item in dataCopy)
                    {
                        var ix = sourceList.IndexOf(item);

                        sourceList.Remove(item);

                        if (ix < insertIndex)
                            sourceList.Insert(insertIndex - 1, item);
                        else
                            sourceList.Insert(insertIndex, item);
                    }
                }
                else
                {
                    throw new NotSupportedException("Copy within the same list is not supported.");
                }
            }
            else
            {
                foreach (var item in data)
                {
                    if (dropInfo.ActualDropEffect == DragDropEffects.Move)
                        sourceList.Remove(item);

                    targetList.Insert(insertIndex, item);
                }
            }
        }

        protected virtual bool CanAcceptData(IDropInfo dropInfo)
        {
            if (dropInfo == null || dropInfo.DragInfo == null || dropInfo.TargetCollection == null)
                return false;

            var targetList = dropInfo.TargetCollection.AsList();

            // Drop collection must implement IList, otherwise we cannot insert items
            if (targetList == null)
                return false;

            // composite collections are not supported at the moment
            if (dropInfo.DragInfo.SourceCollection is CompositeCollection)
                return false;
            
            if (dropInfo.DragInfo.SourceCollection == dropInfo.TargetCollection)
                return true;

            // in case of control such as TreeView
            // we don't want to allow parent node to be dragged into its own child
            // as it would implode the universe
            if (dropInfo.DragInfo.VisualSourceItem.IsAncestorOf(dropInfo.VisualTarget))
                return false;

            return targetList.CanAcceptItem(dropInfo.Data);
        }
    }
}