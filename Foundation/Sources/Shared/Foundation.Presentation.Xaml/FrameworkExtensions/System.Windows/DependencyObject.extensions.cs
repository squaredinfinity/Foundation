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
using System.Windows.Media.Media3D;
using SquaredInfinity.Foundation.Maths.Graphs.Trees;

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

        public static IEnumerable<DependencyObject> VisualTreeTraversal(
            this DependencyObject me,
            bool includeChildItemsControls = true, 
            TreeTraversalMode traversalMode = TreeTraversalMode.BreadthFirst)
        {
            if (includeChildItemsControls)
                return me.TreeTraversal(traversalMode, GetChildrenFuncIncludeChildItemsControls).Skip(1);
            else
                return me.TreeTraversal(traversalMode, GetChildrenFuncExcludeChildItemsControls).Skip(1);
        }

        static IEnumerable<DependencyObject> GetChildrenFuncIncludeChildItemsControls(DependencyObject parent)
        {
            return GetChildrenFunc(parent, includeChildItemsControls: true);
        }

        static IEnumerable<DependencyObject> GetChildrenFuncExcludeChildItemsControls(DependencyObject parent)
        {
            return GetChildrenFunc(parent, includeChildItemsControls: false);
        }

        static IEnumerable<DependencyObject> GetChildrenFunc(
            DependencyObject parent, 
            bool includeChildItemsControls = true)
        {
            int childrenCount = VisualTreeHelper.GetChildrenCount(parent);

            // STEP 1: If no children and parent is a Content Control -> return content
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

        public static DependencyObject FindVisualDescendantByType(this DependencyObject depObj, string descendantTypeFullName)
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

                    var child2 = child.FindVisualDescendantByType(descendantTypeFullName);

                    if (child2 != null)
                        return child2;
                }
            }

            return null;
        }

        public static TDescendant FindVisualDescendant<TDescendant>(
            this DependencyObject depObj, 
            string descendantName = "",
            bool includeContentOfContentControls = true) where TDescendant : class
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

                    if (includeContentOfContentControls)
                    {
                        var cc = child as ContentControl;
                        if (cc != null)
                        {
                            var contentFe = cc.Content as FrameworkElement;
                            if (contentFe != null)
                            {
                                if (contentFe is TDescendant && contentFe.Name == descendantName)
                                    return contentFe as TDescendant;

                                var contentChild = contentFe.FindVisualDescendant<TDescendant>(descendantName, includeContentOfContentControls);
                                if (contentChild != null)
                                    return (TDescendant)contentChild;
                            }
                        }
                    }

                    var child2 = child.FindVisualDescendant<TDescendant>(descendantName, includeContentOfContentControls);

                    if (child2 != null)
                        return child2 as TDescendant;
                }
            }

            return null;
        }

        public static DependencyObject FindVisualDescendant(this DependencyObject depObj, Func<DependencyObject, bool> isMatchFunc)
        {
            for (int childIndex = 0; childIndex < VisualTreeHelper.GetChildrenCount(depObj); childIndex++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(depObj, childIndex);

                if (child != null)
                {
                    if (isMatchFunc(child))
                        return child;

                    var child2 = child.FindVisualDescendant(isMatchFunc);

                    if (child2 != null)
                        return child2;
                }
            }

            return null;
        }

        public static TDescendant FindVisualDescendantByDataContext<TDescendant>(this DependencyObject depObj, object dataContext)
            where TDescendant : class
        {
            return depObj.FindVisualDescendant(
                _do =>
                {
                    var fe = _do as FrameworkElement;

                    if (fe == null)
                        return false;

                    if (!(fe is TDescendant))
                        return false;

                    return object.Equals(fe.DataContext, dataContext);
                }) as TDescendant;
        }

        public static DependencyObject FindVisualDescendantByDataContext(this DependencyObject depObj, object dataContext)
        {
            return depObj.FindVisualDescendant(
                _do =>
                {
                    var fe = _do as FrameworkElement;

                    if (fe == null)
                        return false;

                    return object.Equals(fe.DataContext, dataContext);
                });
        }

        public static IEnumerable<TDescendant> FindVisualDescendants<TDescendant>(
            this DependencyObject depObj) where TDescendant : class
        {
            return
                (from c in depObj.VisualTreeTraversal().OfType<TDescendant>()
                 select c);
        }

        /// <summary>
        /// If specified Dependency Object is not a Visual or Visual3D, then try to walk up logical tree until you find a Visual
        /// </summary>
        /// <param name="me"></param>
        /// <returns></returns>
        public static DependencyObject FindNearestVisual(this DependencyObject dobj)
        {
            if (dobj is Visual || dobj is Visual3D)
                return dobj;

            var parent = dobj.GetLogicalParent();

            while(parent != null)
            {
                if (parent is Visual || parent is Visual3D)
                    return parent;

                parent = dobj.GetLogicalParent();
            }

            return null;
        }

        public static bool IsVisualChildOf(this DependencyObject child, DependencyObject potentialParent, DependencyObject stopSearchAt = null)
        {
            child = child.FindNearestVisual();

            DependencyObject parent = child.GetVisualParent();

            while (parent != null && parent != stopSearchAt)
            {
                if (parent == potentialParent)
                    return true;

                parent = parent.GetVisualParent();
            }

            return false;
        }
        
        public static TParent FindVisualParent<TParent>(this DependencyObject me, DependencyObject stopSearchAt = null)
            where TParent : DependencyObject
        {
            return (TParent) me.FindVisualParent(typeof(TParent), stopSearchAt);
        }

        public static DependencyObject FindVisualParent(this DependencyObject me, Type parentType, DependencyObject stopSearchAt = null)
        {
            me = me.FindNearestVisual();

            DependencyObject parent = me.GetVisualParent();

            while (parent != null && parent != stopSearchAt)
            {
                if (parent.GetType() == parentType || parent.GetType().IsSubclassOf(parentType))
                    return parent;

                parent = parent.GetVisualParent();
            }

            return null;
        }

        /// <summary>
        /// Find top most parent visual for this element.
        /// </summary>
        /// <param name="me"></param>
        /// <returns></returns>
        public static DependencyObject FindVisualRoot(this DependencyObject me)
        {
            me = me.FindNearestVisual();

            DependencyObject result = me.GetVisualParent();

            if (result == null)
                return null;

            var nextParent = result.GetVisualParent();

            while (nextParent != null)
            {
                result = nextParent;
                nextParent = nextParent.GetVisualParent();
            }

            return result;
        }


           public static TParent FindLogicalParent<TParent>(this DependencyObject me)
            where TParent : DependencyObject
        {
            DependencyObject parent = LogicalTreeHelper.GetParent(me);

            while (parent != null)
            {
                TParent typedParent = parent as TParent;

                if (typedParent != null)
                {
                    return typedParent;
                }

                parent = LogicalTreeHelper.GetParent(parent);
            }

            return null;
        }

        public static DependencyObject GetVisualParent(this DependencyObject obj)
        {
            return VisualTreeHelper.GetParent(obj);
        }

        public static DependencyObject GetLogicalParent(this DependencyObject obj)
        {
            return LogicalTreeHelper.GetParent(obj);
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
