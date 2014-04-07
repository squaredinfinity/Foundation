using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using SquaredInfinity.Foundation;
using System.Threading;
using System.Windows.Documents;
using System.Windows.Interop;
using System.Diagnostics;

namespace SquaredInfinity.Foundation.Extensions
{
    public static class DependencyObjectExtensions
    {
        public static void Refresh(this DependencyObject depobj)
        {
            if (depobj == null)
                return;

            // we are already on a dispatcher thread, dependency object should already be refreshed
            if (depobj.Dispatcher.Thread.ManagedThreadId == Thread.CurrentThread.ManagedThreadId)
                return;
            
            var are = new AutoResetEvent(false);

            depobj.Dispatcher.Invoke(() => are.Set(), System.Windows.Threading.DispatcherPriority.ApplicationIdle);

            are.WaitOne();
        }

        public static IEnumerable<DependencyObject> TreeTraversal(this DependencyObject me, TreeTraversalMode traversalMode = TreeTraversalMode.DepthFirst)
        {
            return me.TreeTraversal(GetChildrenFuncIncludeChildItemsControls);
        }

        public static IEnumerable<DependencyObject> VisualTreeTraversal(this DependencyObject me, bool includeChildItemsControls = true, TreeTraversalMode traversalMode = TreeTraversalMode.DepthFirst)
        {
            if (includeChildItemsControls)
                return me.TreeTraversal(GetChildrenFuncIncludeChildItemsControls);
            else
                return me.TreeTraversal(GetChildrenFuncExcludeChildItemsControls);
        }

        static IEnumerable<DependencyObject> GetChildrenFuncIncludeChildItemsControls(DependencyObject parent)
        {
            return GetChildrenFunc(parent, includeChildItemsControls: true);
        }

        static IEnumerable<DependencyObject> GetChildrenFuncExcludeChildItemsControls(DependencyObject parent)
        {
            return GetChildrenFunc(parent, includeChildItemsControls: false);
        }

        static IEnumerable<DependencyObject> GetChildrenFunc(DependencyObject parent, bool includeChildItemsControls = true)
        {
            int childrenCount = VisualTreeHelper.GetChildrenCount(parent);

            // STEP 1: If no childrent is IS Content Control -> return content
            if (childrenCount == 0 && parent is ContentControl)
            {
                DependencyObject content = (parent as ContentControl).Content as DependencyObject;

                if (content != null)
                {
                    yield return content;
                }
            }

            // STEP 2: Return children
            for (int i = 0; i < childrenCount; i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);

                if (child is ItemsControl && !includeChildItemsControls)
                    continue;

                yield return child;
            }

            yield break;
        }

        public static DependencyObject FindDescendant(this DependencyObject depObj, string descendantTypeFullName)
        {
            for (int childIndex = 0; childIndex < VisualTreeHelper.GetChildrenCount(depObj); childIndex++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(depObj, childIndex);

                if (child != null)
                {
                    if (child.GetType().FullName == descendantTypeFullName)
                    {
                        return child;
                    }

                    var child2 = child.FindDescendant(descendantTypeFullName);

                    if (child2 != null)
                        return child2;
                }
            }

            return null;
        }

        public static TDescendant FindDescendant<TDescendant>(this DependencyObject depObj, string descendantName = null) where TDescendant : class
        {
            for (int childIndex = 0; childIndex < VisualTreeHelper.GetChildrenCount(depObj); childIndex++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(depObj, childIndex);

                if (child != null)
                {
                    if (child is TDescendant)
                    {
                        if (descendantName.IsNullOrEmpty()) // if name wasn't provided then we found what we need
                        {
                            return child as TDescendant;
                        }
                        else if (child is FrameworkElement) // if name was provided then check if element has the right name
                        {
                            if ((child as FrameworkElement).Name == descendantName)
                                return child as TDescendant;
                        }
                    }

                    var child2 = child.FindDescendant<TDescendant>(descendantName);

                    if (child2 != null)
                        return child2 as TDescendant;
                }
            }

            return null;
        }

        public static TParent FindVisualParent<TParent>(this DependencyObject me)
            where TParent : DependencyObject
        {
            //# Inline is not a visual or visual3d, and therefore not supported by VisualTreeHelper.GetParent() - try to use its parent instead
            // NOTE: this may need to be done recursively until valid parent is found
            if (me is Inline)
            {
                me = (me as Inline).Parent;
            }

            DependencyObject parent = VisualTreeHelper.GetParent(me);

            while (parent != null)
            {
                TParent typedParent = parent as TParent;

                if (typedParent != null)
                {
                    return typedParent;
                }

                parent = VisualTreeHelper.GetParent(parent);
            }

            return null;
        }

        public static TInterface FindVisualParentByInterface<TInterface>(this DependencyObject me)
            where TInterface : class
        {
            DependencyObject parent = VisualTreeHelper.GetParent(me);

            while (parent != null)
            {
                var parentType = parent.GetType();

                if (parentType.ImplementsInterface<TInterface>())
                    return parent as TInterface;

                parent = VisualTreeHelper.GetParent(parent);
            }

            return null;
        }

        public static DependencyObject GetVisualOrLogicalParent(this DependencyObject sourceElement)
        {
            if (sourceElement is Visual)
            {
                return (VisualTreeHelper.GetParent(sourceElement) ?? LogicalTreeHelper.GetParent(sourceElement));
            }

            return LogicalTreeHelper.GetParent(sourceElement);
        }
      
    }
}
