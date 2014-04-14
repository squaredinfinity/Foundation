using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using SquaredInfinity.Foundation.Extensions;
using System.Diagnostics;
using System.Windows.Media;
using System.Windows;
using System.Collections.Specialized;
using System.Windows.Input;
using System.Windows.Media.Media3D;
using System.Reactive.Linq;
using System.Reactive.Concurrency;

namespace WPF
{
    public class BackgroundLoadingListView : ListView
    {
        internal ScrollViewer ScrollViewer { get; set; }

        public BackgroundLoadingListView()
        {
            this.IsVisibleChanged += BackgroundLoadingListView_IsVisibleChanged;
        }

        void BackgroundLoadingListView_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            RenderItemsInView();
            RenderAllItems();
        }

        object ScrollChangedSubscription;

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            ScrollViewer = this.FindDescendant<ScrollViewer>();

            ScrollChangedSubscription = Observable.FromEvent<ScrollChangedEventHandler, ScrollChangedEventArgs>(
                h =>
                    {
                        ScrollChangedEventHandler x = (sender, e) => h(e);
                        return x;
                    },
                    h => ScrollViewer.ScrollChanged += h,
                    h => ScrollViewer.ScrollChanged -= h)
                    .Throttle(TimeSpan.FromMilliseconds(250))
                    .ObserveOnDispatcher()
                    .Subscribe(args =>
                    {
                        RenderItemsInView();
                    }); 
        }

        void RenderAllItems()
        {
            var itemsToRender = new List<FrameworkElement>();

            for (int i = 0; i < this.ItemContainerGenerator.Items.Count; i++)
            {
                var c = ItemContainerGenerator.ContainerFromIndex(i) as FrameworkElement;

                if (c == null)
                    continue;

                itemsToRender.Add(c);
            }
            
            BackgroundRenderingService.RequestRender(1, itemsToRender);
        }

        void RenderItemsInView()
        {
            if (ScrollViewer == null)
                return;

            if (!ScrollViewer.IsLoaded)
                return;

            var itemsToRender = new List<FrameworkElement>();

            for (int i = 0; i < this.ItemContainerGenerator.Items.Count; i++)
            {
                var c = ItemContainerGenerator.ContainerFromIndex(i) as FrameworkElement;

                if (c == null)
                    continue;

                if (!IsInViewport(ScrollViewer, c))
                {
                    continue;
                }

                itemsToRender.Add(c);
            }
             
      //      itemsToRender.Reverse();

            BackgroundRenderingService.RequestRender(0, itemsToRender);
        }

        static bool IsInViewport(FrameworkElement scrollViewer, FrameworkElement fe)
        {
            var p = VisualTreeHelper.GetParent(fe) as FrameworkElement;

            var transform = fe.TransformToVisual(scrollViewer);
            var rectangle = transform.TransformBounds(new Rect(new Point(0, 0), fe.RenderSize));

            var intersection = Rect.Intersect(new Rect(new Point(0, 0), scrollViewer.RenderSize), rectangle);

            if (intersection == Rect.Empty)
            {
                // framework element is not in view

                return false;

                // todo: make this configurable (by number of pixels, pages)
                // render elements which are just outside of view port

                //rectangle.Inflate(scrollViewer.RenderSize.Width * .5, scrollViewer.RenderSize.Height * .5);
                //intersection = Rect.Intersect(new Rect(new Point(0, 0), scrollViewer.RenderSize), rectangle);

                //return intersection != Rect.Empty;

                //return false;
            }
            else
            {
                return true;
            }
        }

        protected override void OnItemsSourceChanged(System.Collections.IEnumerable oldValue, System.Collections.IEnumerable newValue)
        {
            base.OnItemsSourceChanged(oldValue, newValue);

            var cc_old = oldValue as INotifyCollectionChanged;

            if (cc_old != null)
            {
                cc_old.CollectionChanged -= cc_CollectionChanged;
            }

            var cc_new = newValue as INotifyCollectionChanged;

            if (cc_new != null)
            {
                cc_new.CollectionChanged += cc_CollectionChanged;
            }

            int priority = IsVisible ? 1 : 2;

            BackgroundRenderingService.RequestRender(priority, this);
        }

        void cc_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            Trace.WriteLine("collection changed : " + e.Action.ToString());

            if (e.Action != NotifyCollectionChangedAction.Remove)
            {
                int priority = IsVisible ? 1 : 2;

                BackgroundRenderingService.RequestRender(priority, this);
            }
        }

        protected override Size MeasureOverride(Size constraint)
        {
            return base.MeasureOverride(constraint);
        }

        protected override Size ArrangeOverride(Size arrangeBounds)
        {
            return base.ArrangeOverride(arrangeBounds);
        }

        protected override System.Windows.DependencyObject GetContainerForItemOverride()
        {
            return new BackgroundLoadingListViewItem();
        }
    }

    public interface ISupportsBackgroundRendering
    {
        bool BackgroundRenderingComplete { get; set; }
        bool ScheduledForBackgroundRendering { get; set; }
    }

    public class BackgroundLoadingListViewItem : ListViewItem, ISupportsBackgroundRendering
    {
        public bool BackgroundRenderingComplete { get; set; }
        public bool ScheduledForBackgroundRendering { get; set; }





        public Size SizeWhileBackgroundLoading
        {
            get { return (Size)GetValue(SizeWhileBackgroundLoadingProperty); }
            set { SetValue(SizeWhileBackgroundLoadingProperty, value); }
        }

        public static readonly DependencyProperty SizeWhileBackgroundLoadingProperty =
            DependencyProperty.Register("SizeWhileBackgroundLoading", typeof(Size), typeof(BackgroundLoadingListViewItem), new PropertyMetadata(new Size(1, 20)));

        

        public BackgroundLoadingListViewItem()
        {
            this.IsVisibleChanged += BackgroundLoadingListViewItem_IsVisibleChanged;
        }

        void BackgroundLoadingListViewItem_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var parent = VisualParent.FindVisualParent<BackgroundLoadingListView>();

            if (IsVisible && IsUserVisible(this, parent))
            {
                //BackgroundRenderingService.RequestRender(0, this, parent, parent.ScrollViewer);
            }
        }

        private bool IsUserVisible(FrameworkElement element, FrameworkElement container)
        {
            if (!element.IsVisible)
                return false;

            Rect bounds = element.TransformToAncestor(container).TransformBounds(new Rect(0.0, 0.0, element.ActualWidth, element.ActualHeight));
            Rect rect = new Rect(0.0, 0.0, container.ActualWidth, container.ActualHeight);
            return rect.Contains(bounds.TopLeft) || rect.Contains(bounds.BottomRight);
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
        }

        protected override System.Windows.Size MeasureOverride(System.Windows.Size constraint)
        {
            if (!BackgroundRenderingComplete)
            {
                var parentListView = this.VisualParent.FindVisualParent<BackgroundLoadingListView>();

                if (VisualParent is VirtualizingPanel && VirtualizingPanel.GetIsVirtualizing(parentListView))
                {
                    return base.MeasureOverride(constraint);
                }

                if (!ScheduledForBackgroundRendering)
                {
                    int priority = parentListView.IsVisible ? 1 : 2;
                    BackgroundRenderingService.RequestRender(priority, this);
                }

                if (!constraint.IsInfinite())
                    return constraint;

                return SizeWhileBackgroundLoading;
            }
            else
            {
                return base.MeasureOverride(constraint);
            }
        }

        protected override System.Windows.Size ArrangeOverride(System.Windows.Size arrangeBounds)
        {
            if (!BackgroundRenderingComplete)
            {
                var parentListView = this.VisualParent.FindVisualParent<BackgroundLoadingListView>();

                if (VisualParent is VirtualizingPanel && VirtualizingPanel.GetIsVirtualizing(parentListView))
                {
                    return base.ArrangeOverride(arrangeBounds);
                }

                if (!ScheduledForBackgroundRendering)
                {
                    int priority = parentListView.IsVisible ? 1 : 2;
                    BackgroundRenderingService.RequestRender(priority, this);
                }

                return arrangeBounds;
            }
            else
            {
                return base.ArrangeOverride(arrangeBounds);
            }
        }
    }
}
