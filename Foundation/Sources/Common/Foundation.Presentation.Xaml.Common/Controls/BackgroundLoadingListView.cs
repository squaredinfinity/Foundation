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
using System.ComponentModel;
using System.Windows.Threading;

namespace SquaredInfinity.Foundation.Presentation.Controls
{
    public class BackgroundLoadingListView : ListView
    {
        internal ScrollViewer ScrollViewer { get; set; }

        IDisposable ContainerGeneratorStatusChangedSubscription;
        IDisposable SizeChangedSubscription;
        IDisposable IsVisibleChangedSubscription;

        public BackgroundLoadingListView()
        {
            IsVisibleChangedSubscription =
                Observable.FromEvent<DependencyPropertyChangedEventHandler, DependencyPropertyChangedEventArgs>(
                 h =>
                 {
                     DependencyPropertyChangedEventHandler x = (sender, e) => h(e);
                     return x;
                 },
                    h => this.IsVisibleChanged += h,
                    h => this.IsVisibleChanged -= h)
                    .Throttle(TimeSpan.FromMilliseconds(250))
                    .ObserveOnDispatcher()
                    .Subscribe(args =>
                    {
                        if ((bool)args.NewValue == false)
                            return;

                        if (VirtualizingPanel.GetIsVirtualizing(this))
                            return;

                        RenderItemsInView();
                        RenderAllItems();
                    });

            SizeChangedSubscription =
                Observable.FromEvent<SizeChangedEventHandler, SizeChangedEventArgs>(
                 h =>
                 {
                     SizeChangedEventHandler x = (sender, e) => h(e);
                     return x;
                 },
                    h => this.SizeChanged += h,
                    h => this.SizeChanged -= h)
                    .Throttle(TimeSpan.FromMilliseconds(250))
                    .ObserveOnDispatcher()
                    .Subscribe(args =>
                    {
                        if (VirtualizingPanel.GetIsVirtualizing(this))
                            return;

                        RenderAllItems(forceRender: true);
                    });

            ContainerGeneratorStatusChangedSubscription = 
                Observable.FromEvent<EventHandler, EventArgs>(
                 h =>
                    {
                        EventHandler x = (sender, e) => h(e);
                        return x;
                    },
                    h => ItemContainerGenerator.StatusChanged += h,
                    h => ItemContainerGenerator.StatusChanged -= h)
                    .Throttle(TimeSpan.FromMilliseconds(250))
                    .ObserveOnDispatcher()
                    .Subscribe(args =>
                    {
                        if (ItemContainerGenerator.Status == System.Windows.Controls.Primitives.GeneratorStatus.ContainersGenerated)
                        {
                            if (VirtualizingPanel.GetIsVirtualizing(this))
                                return;

                            RenderAllItems(forceRender: true);
                        }
                    }); 
        }

        IDisposable ScrollChangedSubscription;

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            ScrollViewer = this.FindVisualDescendant<ScrollViewer>();

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

        void RenderAllItems(bool forceRender = false)
        {
            if (!Dispatcher.CheckAccess())
            {
                Dispatcher.BeginInvoke(new Action(() => RenderAllItems(forceRender)), DispatcherPriority.DataBind);
                return;
            }

            if (VirtualizingPanel.GetIsVirtualizing(this))
                return;

            var itemsToRender = new List<FrameworkElement>();

            for (int i = 0; i < this.ItemContainerGenerator.Items.Count; i++)
            {
                var c = ItemContainerGenerator.ContainerFromIndex(i) as FrameworkElement;

                if (c == null)
                    continue;

                var sbr = c as ISupportsBackgroundRendering;

                if (sbr != null)
                {
                    if (forceRender)
                    {
                        sbr.BackgroundRenderingComplete = false;
                        sbr.HighestScheduledPriority = RenderingPriority.BackgroundLow;
                        sbr.ScheduledForBackgroundRendering = false;
                    }
                    else if(sbr.BackgroundRenderingComplete)
                        continue;
                }

                itemsToRender.Add(c);
            }

            itemsToRender.Reverse();

            BackgroundRenderingService.RequestRender(RenderingPriority.ParentVisible, itemsToRender);
        }

        void RenderItemsInView()
        {
            if (!Dispatcher.CheckAccess())
            {
                Dispatcher.BeginInvoke(new Action(() => RenderItemsInView()), DispatcherPriority.DataBind);
                return;
            }

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


            //var pc_old = oldValue as INotifyPropertyChanged;

            //if (pc_old != null)
            //{
            //    pc_old.PropertyChanged -= pc_PropertyChanged;
            //}

            //var pc_new = newValue as INotifyPropertyChanged;

            //if (pc_new != null)
            //{
            //    pc_new.PropertyChanged += pc_PropertyChanged;
            //}

            if (VirtualizingPanel.GetIsVirtualizing(this))
                return;

            var priority = IsVisible ? RenderingPriority.ParentVisible : RenderingPriority.BackgroundLow;

            BackgroundRenderingService.RequestRender(priority, this);
        }



        void cc_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            return;

            if (e.Action == NotifyCollectionChangedAction.Remove)
                return;

            if (e.Action == NotifyCollectionChangedAction.Reset)
            {
                RenderAllItems(forceRender: true);
                RenderItemsInView();
                return;
            }

            RenderAllItems();
            RenderItemsInView();
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
                var result = base.MeasureOverride(constraint);

                return result;
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
