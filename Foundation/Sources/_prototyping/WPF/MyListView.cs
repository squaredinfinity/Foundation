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

            ItemContainerGenerator.StatusChanged += ItemContainerGenerator_StatusChanged;
        }

        void ItemContainerGenerator_StatusChanged(object sender, EventArgs e)
        {
            if (ItemContainerGenerator.Status == System.Windows.Controls.Primitives.GeneratorStatus.ContainersGenerated)
            {
                if (VirtualizingPanel.GetIsVirtualizing(this))
                    return;

                var priority = IsVisible ? RenderingPriority.ParentVisible : RenderingPriority.BackgroundLow;

                BackgroundRenderingService.RequestRender(priority, this);
            }
        }

        void BackgroundLoadingListView_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue == false)
                return;

            if (VirtualizingPanel.GetIsVirtualizing(this))
                return;

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
            if (VirtualizingPanel.GetIsVirtualizing(this))
                return;

            var itemsToRender = new List<FrameworkElement>();

            for (int i = 0; i < this.ItemContainerGenerator.Items.Count; i++)
            {
                var c = ItemContainerGenerator.ContainerFromIndex(i) as FrameworkElement;

                if (c == null)
                    continue;

                var sbr = c as ISupportsBackgroundRendering;

                if (sbr != null && sbr.BackgroundRenderingComplete)
                    continue;

                itemsToRender.Add(c);
            }

            itemsToRender.Reverse();

            BackgroundRenderingService.RequestRender(RenderingPriority.ParentVisible, itemsToRender);
        }

        void RenderItemsInView()
        {
            if (VirtualizingPanel.GetIsVirtualizing(this))
                return;

            if (ScrollViewer == null)
                return;

            if (!ScrollViewer.IsLoaded)
                return;

            var parentWindow = this.FindVisualParent<Window>();

            var itemsToRender = new List<FrameworkElement>();

            for (int i = 0; i < this.ItemContainerGenerator.Items.Count; i++)
            {
                var c = ItemContainerGenerator.ContainerFromIndex(i) as FrameworkElement;

                if (c == null)
                    continue;

                var sbr = c as ISupportsBackgroundRendering;

                if (sbr != null && sbr.BackgroundRenderingComplete)
                    continue;

                if (!c.IsInViewport(parentWindow))
                    continue;

                itemsToRender.Add(c);
            }

            itemsToRender.Reverse();

            BackgroundRenderingService.RequestRender(RenderingPriority.ImmediatelyVisible, itemsToRender);
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

            if (VirtualizingPanel.GetIsVirtualizing(this))
                return;

            var priority = IsVisible ? RenderingPriority.ParentVisible : RenderingPriority.BackgroundLow;

            BackgroundRenderingService.RequestRender(priority, this);
        }

        void cc_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action != NotifyCollectionChangedAction.Remove)
            {
                var priority = IsVisible ? RenderingPriority.ParentVisible : RenderingPriority.BackgroundLow;

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
        RenderingPriority HighestScheduledPriority { get; set; }
    }

    public class BackgroundLoadingListViewItem : ListViewItem, ISupportsBackgroundRendering
    {
        public bool BackgroundRenderingComplete { get; set; }
        public bool ScheduledForBackgroundRendering { get; set; }
        public RenderingPriority HighestScheduledPriority { get; set; }


        public Size SizeWhileBackgroundLoading
        {
            get { return (Size)GetValue(SizeWhileBackgroundLoadingProperty); }
            set { SetValue(SizeWhileBackgroundLoadingProperty, value); }
        }

        public static readonly DependencyProperty SizeWhileBackgroundLoadingProperty =
            DependencyProperty.Register("SizeWhileBackgroundLoading", typeof(Size), typeof(BackgroundLoadingListViewItem), new PropertyMetadata(new Size(1, 20)));

        

        public BackgroundLoadingListViewItem()
        { }
        
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

                return arrangeBounds;
            }
            else
            {
                return base.ArrangeOverride(arrangeBounds);
            }
        }
    }
}
