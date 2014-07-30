using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Threading;
using SquaredInfinity.Foundation.Extensions;
using System.Diagnostics;
using SquaredInfinity.Foundation;
using SquaredInfinity.Foundation.Collections;
using System.Windows.Input;
using SquaredInfinity.Foundation.Presentation.Controls;

namespace SquaredInfinity.Foundation.Presentation
{
    public class AfterItemRenderedEventArgs : EventArgs
    {
        public FrameworkElement RenderedElement { get; private set; }

        public AfterItemRenderedEventArgs(FrameworkElement renderedElement)
        {
            this.RenderedElement = renderedElement;
        }
    }

    public class BackgroundRenderingService
    {
        //  priority 0 -    items that were not previously rendered and are currently in the view (on screen)
        //  (STACK)
        //                  high priority stack
        //                  will be processed before any other priorities

        //  priority 1 -    items that were not previousy rendered and are currently visible
        //  (STACK)
        //                  (but not in the view, e.g. outside of viewport of scrollviewer)


        //  priority 3 -    items that were not previously loaded and are currently *not* in the view
        //  (QUEUE)

        static readonly BackgroundRenderingServiceImplementation Instance = new BackgroundRenderingServiceImplementation();

        public static event EventHandler<AfterItemRenderedEventArgs> AfterItemRendered;

        static BackgroundRenderingService()
        {
            Instance.AfterItemRendered += (s, e) =>
                {
                    if (AfterItemRendered != null)
                        AfterItemRendered(null, e);
                };
        }
        
        public static void Start()
        {
            Instance.Start();
        }

        public static void RequestRender(RenderingPriority priority, FrameworkElement fe)
        {
            Instance.RequestRender(priority, fe);
        }

        internal static void RequestRender(RenderingPriority priority, List<FrameworkElement> itemsToRender)
        {
            Instance.RequestRender(priority, itemsToRender);
        }

        public static int RemainingQueueSize
        {
            get { return Instance.RemainingQueueSize; }
        }

        public static void Stop()
        {
            Instance.Stop();
        }
    }

    public enum RenderingPriority
    {
        BackgroundLow = 0,
        BackgroundHigh,
        ParentVisible,
        ImmediatelyVisible
        
    }

    class BackgroundRenderingServiceImplementation
    {
        static readonly Type ItemsControlType;
        static readonly PropertyInfo ItemsHostProperty;

        static readonly Type PanelType;
        static readonly MethodInfo EnsureGeneratorMethod;
        
        public event EventHandler<AfterItemRenderedEventArgs> AfterItemRendered;

        CancellationTokenSource WorkQueueProcessingCancellationTokenSource;
        
        BlockingCollection<BackgroundRenderingQueueItem> ImmediatelyVisibleWorkQueue;
        BlockingCollection<BackgroundRenderingQueueItem> ParentVisibleWorkQueue;
        BlockingCollection<BackgroundRenderingQueueItem> BackgroundHighWorkQueue;
        BlockingCollection<BackgroundRenderingQueueItem> BackgroundLowWorkQueue;

        Task ImmediatelyVisibleWorkQueueProcessingTask;
        Task ParentVisibleWorkQueueProcessingTask;
        Task BackgroundLowWorkQueueProcessingTask;
        Task BackgroundHighWorkQueueProcessingTask;

        public int RemainingQueueSize
        {
            get 
            {
                return
                    ImmediatelyVisibleWorkQueue.Count +
                    ParentVisibleWorkQueue.Count +
                    BackgroundLowWorkQueue.Count;
            }
        }

        IProgress<int> Progress;

        static BackgroundRenderingServiceImplementation()
        {
            ItemsControlType = typeof(ItemsControl);
            ItemsHostProperty = ItemsControlType.GetProperty("ItemsHost", BindingFlags.Instance | BindingFlags.NonPublic);

            PanelType = typeof(Panel);
            EnsureGeneratorMethod = PanelType.GetMethod("EnsureGenerator", BindingFlags.NonPublic | BindingFlags.Instance);
        }

        public void RequestRender(RenderingPriority priority, List<FrameworkElement> itemsToRender)
        {
            if (itemsToRender.IsNullOrEmpty())
                return;

            var notYetRendered =
                (from i in itemsToRender
                 let rs = i as ISupportsBackgroundRendering
                 where
                 rs == null || (!rs.BackgroundRenderingComplete && !rs.ScheduledForBackgroundRendering) || (rs.ScheduledForBackgroundRendering && priority > rs.HighestScheduledPriority)
                 select i).ToList();

            if (notYetRendered.Count == 0)
                return;

            for(int i = 0; i < notYetRendered.Count; i++)
            {
                var fe = notYetRendered[i];

                var queueItem = new BackgroundRenderingQueueItem();

                queueItem.ElementToRender = fe;
                //queueItem.PostRenderAction = postRenderAction;

                RequestRender(priority, queueItem);
            }

            var panel = notYetRendered[0].FindVisualParent<Panel>();
            if (panel != null && panel.IsItemsHost)
                RequestRender(RenderingPriority.BackgroundHigh, new BackgroundRenderingQueueItem { ElementToRender = panel });
        }

        public void RequestRender(RenderingPriority priority, FrameworkElement frameworkElement, Action postRenderAction = null)
        {
            var br = frameworkElement as ISupportsBackgroundRendering;

            if (br != null)
            {
                if (br.BackgroundRenderingComplete)
                    return;

                if (br.ScheduledForBackgroundRendering && priority <= br.HighestScheduledPriority)
                    return;

                br.ScheduledForBackgroundRendering = true;
            }

            var queueItem = new BackgroundRenderingQueueItem();

            queueItem.ElementToRender = frameworkElement;
            queueItem.PostRenderAction = postRenderAction;

            RequestRender(priority, queueItem);
        }

        void RequestRender(RenderingPriority priority, BackgroundRenderingQueueItem item)
        {
            switch (priority)
            {
                case RenderingPriority.ImmediatelyVisible:
                    ImmediatelyVisibleWorkQueue.Add(item);
                    break;
                case RenderingPriority.ParentVisible:
                    ParentVisibleWorkQueue.Add(item);
                    break;
                case RenderingPriority.BackgroundHigh:
                    BackgroundHighWorkQueue.Add(item);
                    break;
                case RenderingPriority.BackgroundLow:
                    BackgroundLowWorkQueue.Add(item);
                    break;
                default:
                    // log
                    break;
            }
        }

        public void Stop()
        {
            if (WorkQueueProcessingCancellationTokenSource != null)
                WorkQueueProcessingCancellationTokenSource.Cancel();

            WorkQueueProcessingCancellationTokenSource = null;
        }
       
         public void Restart()
        {
            Stop();
            Start();
        }

        public void Start()
        {
            Start(new Progress<int>());
        }

        public void Start(IProgress<int> progress)
        {
            Stop();

            this.Progress = progress;

            WorkQueueProcessingCancellationTokenSource = new CancellationTokenSource();

            ImmediatelyVisibleWorkQueue = 
                new BlockingCollection<BackgroundRenderingQueueItem>(
                    new ConcurrentStack<BackgroundRenderingQueueItem>());

            ParentVisibleWorkQueue =
                new BlockingCollection<BackgroundRenderingQueueItem>(
                    new ConcurrentStack<BackgroundRenderingQueueItem>());

            BackgroundHighWorkQueue =
                new BlockingCollection<BackgroundRenderingQueueItem>(
                    new ConcurrentQueue<BackgroundRenderingQueueItem>());

            BackgroundLowWorkQueue =
                new BlockingCollection<BackgroundRenderingQueueItem>(
                    new ConcurrentQueue<BackgroundRenderingQueueItem>());

            ImmediatelyVisibleWorkQueueProcessingTask =
                Task.Run(
                () => StartCore(
                    RenderingPriority.ImmediatelyVisible,
                    ImmediatelyVisibleWorkQueue,
                    DispatcherPriority.Input,
                    WorkQueueProcessingCancellationTokenSource.Token));

            ParentVisibleWorkQueueProcessingTask =
                Task.Run(
                () => StartCore(
                    RenderingPriority.ParentVisible,
                    ParentVisibleWorkQueue,
                    DispatcherPriority.Background,
                    WorkQueueProcessingCancellationTokenSource.Token));

            BackgroundHighWorkQueueProcessingTask =
                Task.Run(
                () => StartCore(
                    RenderingPriority.BackgroundHigh,
                    BackgroundHighWorkQueue,
                    DispatcherPriority.Background,
                    WorkQueueProcessingCancellationTokenSource.Token));

            BackgroundLowWorkQueueProcessingTask =
                Task.Run(
                () => StartCore(
                    RenderingPriority.BackgroundLow,
                    BackgroundLowWorkQueue,
                    DispatcherPriority.SystemIdle,
                    WorkQueueProcessingCancellationTokenSource.Token));
        }

        void StartCore(
            RenderingPriority priority,
            BlockingCollection<BackgroundRenderingQueueItem> collection,
            DispatcherPriority dispatcherPriority,
            CancellationToken cancellationToken)
        {
            try
            {
                while (true)
                {
                    Progress.Report(RemainingQueueSize);

                    var queueItem = collection.Take(cancellationToken);

                    if (queueItem == null)
                        continue;

                    if (queueItem.ElementToRender == null)
                        continue;

                    // background loading list view handles everything itself
                    if (queueItem.ElementToRender is BackgroundLoadingListView && queueItem.ElementToRender.IsVisible)
                        continue;

                    var supportsBackgroundRendering = queueItem.ElementToRender as ISupportsBackgroundRendering;
                    if (supportsBackgroundRendering != null && supportsBackgroundRendering.BackgroundRenderingComplete)
                        continue;

                    if (cancellationToken.IsCancellationRequested)
                        return;

                    if (priority == RenderingPriority.ImmediatelyVisible && !queueItem.ElementToRender.IsVisible)
                    {
                        RequestRender(RenderingPriority.BackgroundLow, queueItem);
                        continue;
                    }

                    if (priority == RenderingPriority.BackgroundHigh)
                    {

                    }

                    Trace.Write("rendering {0}, {1} left.".FormatWith(priority, RemainingQueueSize));

                    var ic = queueItem.ElementToRender as ItemsControl;

                    if (ic != null)
                    {
                        RenderItemsControl(ic, dispatcherPriority, cancellationToken);
                        continue;
                    }
                    else
                    {
                        RenderFrameworkElement(queueItem.ElementToRender, dispatcherPriority, cancellationToken);
                    }
                }
            }
            catch (OperationCanceledException)
            { 
                // operation was cancelled, nothing else to here.
            }
        }

        void RenderItemsHost(Panel panel, DispatcherPriority dispatcherPriority, CancellationToken cancellationToken)
        {
            try
            {
                panel.Dispatcher.Invoke(new Action(() =>
                {
                    try
                    {
                        RenderItemsHostCore(panel, cancellationToken);
                    }
                    catch (Exception ex)
                    {
                        //         Logger.DiagnosticOnlyLogException(ex);
                    }

                }), dispatcherPriority, cancellationToken);

                OnElementRendered(panel);
            }
            catch (OperationCanceledException)
            {
                // operation has been cancelled, nothing more needs to be done
            }
            catch (Exception ex)
            {
                //        Logger.DiagnosticOnlyLogException(ex);
            }
        }

        void RenderItemsControl(ItemsControl itemsControl, DispatcherPriority dispatcherPriority, CancellationToken cancellationToken)
        {
            try
            {
                itemsControl.Dispatcher.Invoke(new Action(() =>
                {
                    try
                    {
                        RenderItemsControlCore(itemsControl, cancellationToken);
                    }
                    catch (Exception ex)
                    {
                        //         Logger.DiagnosticOnlyLogException(ex);
                    }

                }), dispatcherPriority, cancellationToken);

                OnElementRendered(itemsControl);
            }
            catch (OperationCanceledException)
            {
                // operation has been cancelled, nothing more needs to be done
            }
            catch(Exception ex)
            {
                //        Logger.DiagnosticOnlyLogException(ex);
            }
        }

        void RenderItemsControlCore(ItemsControl itemsControl, CancellationToken cancellationToken)
        {
            if(itemsControl.GetVisualParent() == null)
            {
                var tabControl = itemsControl.FindLogicalParent<SquaredInfinity.Foundation.Presentation.Controls.TabControl>();
                var tabItem = itemsControl.FindLogicalParent<TabItem>();

                if (tabControl != null && tabItem != null && tabItem.IsDescendantOf(tabControl))
                {
                    tabControl.CreateChildContentPresenter(tabItem);
                    tabControl.InvalidateVisual();
                }
            }

            // NOTE: THIS WILL BE CALLED ON UI THREAD

            var itemsHost =
                (from c in itemsControl.VisualTreeTraversal(
                     includeChildItemsControls: false, 
                     traversalMode: TreeTraversalMode.BreadthFirst).OfType<Panel>()
                 where c.IsItemsHost
                 select c).FirstOrDefault();

            if (cancellationToken.IsCancellationRequested)
                return;

            if (itemsHost == null)
            {
                // todo: surely some of this is not needed

                itemsControl.ApplyTemplate();

                itemsHost =
                    (from c in itemsControl.VisualTreeTraversal(
                         includeChildItemsControls: false, 
                         traversalMode: TreeTraversalMode.BreadthFirst).OfType<Panel>()
                     where c.IsItemsHost
                     select c).FirstOrDefault();
            }

            if (itemsHost == null)
                return;

            itemsHost.InvalidateMeasure();
            itemsHost.InvalidateArrange();
            itemsHost.InvalidateVisual();
            itemsHost.UpdateLayout();

            if (cancellationToken.IsCancellationRequested)
                return;

            using (var wait = new ItemContainerGeneratorStatusMonitor(itemsControl.ItemContainerGenerator))
            {
                EnsureGeneratorMethod.Invoke(itemsHost, new object[] { });

                wait.WaitForContainersGeneratedStatus();
            }

            var priority = itemsControl.IsVisible ? RenderingPriority.ParentVisible : RenderingPriority.BackgroundLow;

            var parent = (FrameworkElement) null;
            var parentWindow = (Window) null;


            parent = itemsControl.FindVisualParent<Control>();

            if (parent != null)
            {
                if (parent.IsVisible)
                {
                    parentWindow = itemsControl.FindVisualParent<Window>();
                }
            }

//            using (itemsControl.ItemContainerGenerator.GenerateBatches())
            {
                for (int i = itemsControl.ItemContainerGenerator.Items.Count - 1; i >= 0; i--)
                {
                    if (cancellationToken.IsCancellationRequested)
                        return;

                    var item = itemsControl.ItemContainerGenerator.ContainerFromIndex(i) as FrameworkElement;

                    if (item != null)
                    {
                        var sbr = item as ISupportsBackgroundRendering;
                        if (sbr != null && sbr.BackgroundRenderingComplete)
                            continue;

                        if (itemsControl.IsVisible)
                        {
                            if (parentWindow != null && item.IsInViewport(parentWindow))
                            {
                                RequestRender(RenderingPriority.ImmediatelyVisible, item);
                            }
                            else
                            {
                                RequestRender(RenderingPriority.ParentVisible, item);
                            }
                        }
                        else if (parent == null || parentWindow == null || !item.IsInViewport(parentWindow))
                        {
                            RequestRender(RenderingPriority.BackgroundLow, item);
                        }
                        else
                        {
                            RequestRender(RenderingPriority.BackgroundHigh, item);
                        }
                    }
                }
            }
        }

        void RenderFrameworkElement(FrameworkElement fe, DispatcherPriority dispatcherPriority, CancellationToken cancellationToken)
        {
            var x = (fe as ISupportsBackgroundRendering);
            if (x != null && x.BackgroundRenderingComplete)
                    return;
            
            try
            {
                fe.Dispatcher.Invoke(new Action(() =>
                {
                    var ih = fe is Panel && ((Panel)fe).IsItemsHost;
                    if (ih)
                    {
                        RenderItemsHostCore((Panel)fe, cancellationToken);
                    }
                    else
                    {
                        RenderFrameworkElementCore(fe, cancellationToken);
                    }

                }), dispatcherPriority, cancellationToken);

                OnElementRendered(fe);
            }
            catch (OperationCanceledException)
            {
                // operation cancelled, nothing else to do here.
            }
            catch (Exception ex)
            {
                //        Logger.DiagnosticOnlyLogException(ex);
            }
        }

        void RenderItemsHostCore(Panel panel, CancellationToken cancellationToken)
        {
            panel.InvalidateVisual();
        }

        void RenderFrameworkElementCore(FrameworkElement fe, CancellationToken cancellationToken)
        {
            var itemsControl = fe.FindVisualParent<Panel>();

            // itemsControl may be null if this item has been disconnected
            if (itemsControl == null)
                return;

            var x = (fe as ISupportsBackgroundRendering);

            if (x != null)
            {
                if (x.BackgroundRenderingComplete)
                    return;

                x.BackgroundRenderingComplete = true;
                //  invalidating measure will cause parent container to recalculate needed area.
                //  for example, if this framework element is inside scrollviewer, the scrollviewer will update it's extent
                //  without this scrollviewer may not have up to date information and scrollbars instide it may not cover the whole child control
                fe.InvalidateMeasure();
                
                // invalide visual works in a simillar way to invalidate measure (in terms of outcome) but parent container will not be refreshed
                // todo: see if there's a visible performance benefit of doing one over another
                // fe.InvalidateVisual();
                return;

                // todo: arrange + invalidate parent panel seems to do the trick
                // try to arrange multiple children at the same time and invalidate panel in one go after
                //fe.Arrange(new Rect(fe.DesiredSize));
                //fe.FindVisualParent<Panel>().InvalidateVisual();

                return;
            }

            fe.InvalidateVisual();

            var contentcontrol = fe as ContentControl;

            if (contentcontrol != null)
            {
                contentcontrol.ApplyTemplate();

                var contentFe = contentcontrol.Content as FrameworkElement;

                if (contentFe == null)
                    return;

                if (cancellationToken.IsCancellationRequested)
                    return;

                contentFe.ApplyTemplate();

                var priority = contentFe.IsVisible ? RenderingPriority.ParentVisible : RenderingPriority.BackgroundLow;

                var parentWindow = contentFe.FindVisualParent<Window>();

                if (parentWindow != null && contentFe.IsInViewport(parentWindow))
                {
                    priority = RenderingPriority.BackgroundHigh;
                }

                RequestRender(priority, contentFe);
            }
        }

        void OnElementRendered(FrameworkElement fe)
        {
            if(AfterItemRendered != null)
                AfterItemRendered(this, new AfterItemRenderedEventArgs(fe));
        }

        class ItemContainerGeneratorStatusMonitor : IDisposable
        {
            readonly ItemContainerGenerator Generator;
            readonly TimeSpan Timeout;

            readonly EventWaitHandle WaitHandle = new ManualResetEvent(false);

            public ItemContainerGeneratorStatusMonitor(ItemContainerGenerator generator)
                : this(generator, TimeSpan.FromMilliseconds(500))
            { }

            public ItemContainerGeneratorStatusMonitor(ItemContainerGenerator generator, TimeSpan timeout)
            {
                this.Generator = generator;
                this.Timeout = timeout;

                Generator.StatusChanged += OnContainerGeneratorStatusChanged;
            }

            public void WaitForContainersGeneratedStatus()
            {
                if (Generator.Status == GeneratorStatus.ContainersGenerated)
                    WaitHandle.Set();

                if (!WaitHandle.WaitOne(this.Timeout))
                {
                    //Debug.WriteLine("WaitForContainerGeneratedStatus timedout.");
                }
            }

            void OnContainerGeneratorStatusChanged(object sender, EventArgs args)
            {
                if (Generator.Status == GeneratorStatus.ContainersGenerated)
                    WaitHandle.Set();
            }

            public void Dispose()
            {
                Generator.StatusChanged -= OnContainerGeneratorStatusChanged;
            }
        }

        class BackgroundRenderingQueueItem
        {
            public FrameworkElement ElementToRender { get; set; }

            public Action PostRenderAction = () => { };
        }
    }
}
