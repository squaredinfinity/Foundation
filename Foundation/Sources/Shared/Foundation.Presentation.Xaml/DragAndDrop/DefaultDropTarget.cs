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
using SquaredInfinity.Foundation.Extensions;
using SquaredInfinity.Foundation.Collections;

namespace SquaredInfinity.Foundation.Presentation.DragDrop
{
    /// <summary>
    /// Default Drop Target.
    /// Handles Most basic cases.
    /// </summary>
    public partial class DefaultDropTarget : IDropTarget
    {
        public static readonly IDropTarget Instance = new DefaultDropTarget();

        public virtual void DragOver(IDropInfo dropInfo)
        {
            if (dropInfo.ActualDropEffect == DragDropEffects.None)
                return;

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
            else
            {
                dropInfo.AllowedEffects = DragDropEffects.None;
            }
        }

        public virtual void Drop(IDropInfo dropInfo)
        {
            var rawInsertIndex = dropInfo.RawInsertIndex;

            var targetList = dropInfo.RawTargetCollection.AsList();
            var sourceList = dropInfo.DragInfo.SourceCollection.AsList();

            var data = new List<object>(dropInfo.DataAsEnumerable.Cast<object>());

            // remove items from source and add to target

            if (sourceList == targetList)
            {
                // operation within the same list
                if (dropInfo.ActualDropEffect == DragDropEffects.Move)
                {   
                    foreach(var item in data)
                    {
                        var ix = sourceList.IndexOf(item);

                        // where possible use .Move() to drag/drop within same collection
                        // for it to be an atomic operation
                        // where not possible, use remove + insert

                        var collEx = sourceList as ICollectionEx;

                        if(collEx != null)
                        {
                            if(ix < rawInsertIndex)
                                collEx.Move(ix, rawInsertIndex - 1);
                            else
                                collEx.Move(ix, rawInsertIndex);
                        }
                        else
                        {
                            sourceList.Remove(item);

                            if (ix < rawInsertIndex)
                                sourceList.Insert(rawInsertIndex - 1, item);
                            else
                                sourceList.Insert(rawInsertIndex, item);
                        }
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
                    {
                        try
                        {
                            sourceList.Remove(item);
                        }
                        catch(NotImplementedException ex)
                        {
                            // this is a valid case when source collection does not implement Remove.

                            // todo: add context data to exception ?
                            // log internal warning
                        }
                        catch(NotSupportedException ex)
                        {
                            // this is a valid case when source collection does not support Remove.

                            // todo: add context data to exception ?
                            // log internal warning
                        }
                    }

                    targetList.Insert(rawInsertIndex, item);
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