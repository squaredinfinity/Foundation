using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
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

namespace WPF
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
    }

    public enum RenderingPriority
    {
        ImmediatelyVisible = 0,
        ParentVisible,
        BackgroundHigh,
        BackgroundLow
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
                 rs == null || !rs.BackgroundRenderingComplete
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
        }

        public void RequestRender(RenderingPriority priority, FrameworkElement frameworkElement, Action postRenderAction = null)
        {
            var br = frameworkElement as ISupportsBackgroundRendering;

            if (br != null)
            {
                if (br.BackgroundRenderingComplete)
                    return;
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
                    ImmediatelyVisibleWorkQueue,
                    DispatcherPriority.Input,
                    WorkQueueProcessingCancellationTokenSource.Token));

            ParentVisibleWorkQueueProcessingTask =
                Task.Run(
                () => StartCore(
                    ParentVisibleWorkQueue,
                    DispatcherPriority.Background,
                    WorkQueueProcessingCancellationTokenSource.Token));

            BackgroundHighWorkQueueProcessingTask =
                Task.Run(
                () => StartCore(
                    BackgroundHighWorkQueue,
                    DispatcherPriority.Background,
                    WorkQueueProcessingCancellationTokenSource.Token));

            BackgroundLowWorkQueueProcessingTask =
                Task.Run(
                () => StartCore(
                    BackgroundLowWorkQueue,
                    DispatcherPriority.SystemIdle,
                    WorkQueueProcessingCancellationTokenSource.Token));
        }

        void StartCore(
            BlockingCollection<BackgroundRenderingQueueItem> collection,
            DispatcherPriority dispatcherPriority,
            CancellationToken cancellationToken)
        {
            while (true)
            {
                Progress.Report(RemainingQueueSize);

                var queueItem = (BackgroundRenderingQueueItem)null;

                if (!collection.TryTake(out queueItem, TimeSpan.FromMilliseconds(250)))
                    continue;

                if (cancellationToken.IsCancellationRequested)
                    return;

                if (queueItem == null)
                    continue;

                if (queueItem.ElementToRender == null)
                    continue;

                var supportsBackgroundRendering = queueItem.ElementToRender as ISupportsBackgroundRendering;
                if (supportsBackgroundRendering != null && supportsBackgroundRendering.BackgroundRenderingComplete)
                    continue;

                //if (pqi.Priority < 2)
                //{
                //    if (!pqi.Value.ElementToRender.IsVisible)
                //    {
                //        RequestRender(2, pqi.Value.ElementToRender);
                //        continue;
                //    }
                //}

                Trace.WriteLine("rendering: {0}, {1} left".FormatWith(dispatcherPriority.ToString(), RemainingQueueSize));

                var ic = queueItem.ElementToRender as ItemsControl;

                if (ic != null)
                {
                    RenderItemsControl(ic, dispatcherPriority);
                }
                else
                {
                    RenderFrameworkElement(queueItem.ElementToRender, dispatcherPriority);
                }

                if (cancellationToken.IsCancellationRequested)
                    return;
            }
        }

        void RenderItemsControl(ItemsControl itemsControl, DispatcherPriority dispatcherPriority)
        {
            try
            {
                itemsControl.Dispatcher.BeginInvoke(new Action(() =>
                {
                    try
                    {
                        RenderItemsControlCore(itemsControl);
                    }
                    catch (Exception ex)
                    {
                        //         Logger.DiagnosticOnlyLogException(ex);
                    }

                }), dispatcherPriority).Wait();

                OnElementRendered(itemsControl);
            }
            catch (Exception ex)
            {
                //        Logger.DiagnosticOnlyLogException(ex);
            }
        }

        void RenderItemsControlCore(ItemsControl itemsControl)
        {
            if(itemsControl.GetVisualParent() == null)
            {
                var tabControl = itemsControl.FindLogicalParent<WPF.TabControl>();

                tabControl.CreateChildContentPresenter(itemsControl);
            }

            // NOTE: THIS WILL BE CALLED ON UI THREAD

            var itemsHost =
                (from c in itemsControl.VisualTreeTraversal(
                     includeChildItemsControls: false, 
                     traversalMode: TreeTraversalMode.BreadthFirst).OfType<Panel>()
                 where c.IsItemsHost
                 select c).FirstOrDefault();

            if (itemsHost == null)
            {
                // todo: surely some of this is not needed

                itemsControl.ApplyTemplate();
                itemsControl.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                itemsControl.Arrange(new Rect(itemsControl.DesiredSize));

                itemsHost =
                    (from c in itemsControl.VisualTreeTraversal(
                         includeChildItemsControls: false, 
                         traversalMode: TreeTraversalMode.BreadthFirst).OfType<Panel>()
                     where c.IsItemsHost
                     select c).FirstOrDefault();
            }

            if (itemsHost == null)
                return;

            using (var wait = new ItemContainerGeneratorStatusMonitor(itemsControl.ItemContainerGenerator))
            {
                EnsureGeneratorMethod.Invoke(itemsHost, new object[] { });

                wait.WaitForContainersGeneratedStatus();
            }

            var priority = itemsControl.IsVisible ? RenderingPriority.ParentVisible : RenderingPriority.BackgroundLow;

            var isParentVisible = false;
            var parentViewPort = (FrameworkElement)null;

            if (!itemsControl.IsVisible)
            {
                var parent = (LogicalTreeHelper.GetParent(itemsControl) as FrameworkElement);

                if (parent != null)
                {
                    if(parent.IsVisible)
                    {
                        isParentVisible = true;
                        parentViewPort = parent.FindDescendant<ScrollViewer>();

                        if (parentViewPort == null)
                            parentViewPort = parent;
                    }
                }
            }

            for (int i = 0; i < itemsControl.ItemContainerGenerator.Items.Count; i++)
            {
                var item = itemsControl.ItemContainerGenerator.ContainerFromIndex(i) as FrameworkElement;

                if (item != null)
                {
                    // TODO: should it adjust priority based on other criteria?
                    // e.g. find if item control above is visible, then check if item is visible
                    // in scroll viewer (if any), or within screen bounds

                    if(itemsControl.IsVisible || !isParentVisible || !item.IsInViewport(parentViewPort))
                        RequestRender(priority, item);
                    else
                    {
                        RequestRender(RenderingPriority.BackgroundHigh, item);
                    }
                }
            }
        }

        void RenderFrameworkElement(FrameworkElement fe, DispatcherPriority dispatcherPriority)
        {
            try
            {
                fe.Dispatcher.BeginInvoke(new Action(() =>
                {
                    RenderFrameworkElementCore(fe);

                }), dispatcherPriority).Wait();

                OnElementRendered(fe);
            }
            catch (Exception ex)
            {
                //        Logger.DiagnosticOnlyLogException(ex);
            }
        }

        void RenderFrameworkElementCore(FrameworkElement fe, Panel itemsControl = null, bool shouldInvalidateVisual = true)
        {
            try
            {
                if(itemsControl == null)
                    itemsControl = VisualTreeHelper.GetParent(fe) as Panel;

                // itemsControl may be null if this item has been disconnected
                if (itemsControl == null)
                    return;

                var x = (fe as ISupportsBackgroundRendering);

                if (x != null)
                {
                    x.BackgroundRenderingComplete = true;

                }

                fe.InvalidateArrange();

                if (shouldInvalidateVisual)
                {
                    fe.InvalidateVisual();
                }

                var contentcontrol = fe as ContentControl;

                if (contentcontrol != null)
                {
                    contentcontrol.ApplyTemplate();

                    var contentFe = contentcontrol.Content as FrameworkElement;

                    if (contentFe == null)
                        return;

                    contentFe.ApplyTemplate();

                    var priority = contentFe.IsVisible ? RenderingPriority.ParentVisible : RenderingPriority.BackgroundLow;

                    if(contentcontrol.IsVisible)
                    {
                        var isInViewPort = contentFe.IsInViewport(contentcontrol);

                        if (isInViewPort)
                            priority = RenderingPriority.BackgroundHigh;
                    }

                    if (contentFe is ItemsControl && VisualTreeHelper.GetParent(contentFe) == null)
                    {

                    }

                    RequestRender(priority, contentFe);
                }
            }
            catch (Exception ex)
            {
                //         Logger.DiagnosticOnlyLogException(ex);
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
                : this(generator, TimeSpan.FromMilliseconds(250))
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
