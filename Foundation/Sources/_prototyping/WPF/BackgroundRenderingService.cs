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

namespace WPF
{
    public class BackgroundRenderingService
    {
        static readonly Type ItemsControlType;
        static readonly PropertyInfo ItemsHostProperty;

        static readonly Type PanelType;
        static readonly MethodInfo EnsureGeneratorMethod;
        
        public event EventHandler<AfterItemRenderedEventArgs> AfterItemRendered;

        /// <summary>
        /// Rendering Priority may be set to high value (e.g. Input) to make it quicker, 
        /// but this in turn may stop other parts of UI from being updated quickly.
        /// </summary>
        public DispatcherPriority RenderingPriority_HighPriorityQueue { get; set; }

        public DispatcherPriority RenderingPriority_LowPriorityQueue { get; set; }

        public class AfterItemRenderedEventArgs : EventArgs
        {
            public FrameworkElement RenderedElement { get; private set; }
            public object RenderedElementDataContext { get; private set; }

            public AfterItemRenderedEventArgs(FrameworkElement renderedElement, object dataContext)
            {
                this.RenderedElement = renderedElement;
                this.RenderedElementDataContext = dataContext;
            }
        }

        BlockingCollection<BackgroundRenderingQueueItem> WorkQueue = new BlockingCollection<BackgroundRenderingQueueItem>();

        public int RemainingQueueSize
        {
            get { return WorkQueue.Count; }
        }

        IProgress<int> Progress;

        static BackgroundRenderingService()
        {
            ItemsControlType = typeof(ItemsControl);
            ItemsHostProperty = ItemsControlType.GetProperty("ItemsHost", BindingFlags.Instance | BindingFlags.NonPublic);

            PanelType = typeof(Panel);
            EnsureGeneratorMethod = PanelType.GetMethod("EnsureGenerator", BindingFlags.NonPublic | BindingFlags.Instance);
        }

        public BackgroundRenderingService()
        {
            RenderingPriority_HighPriorityQueue = DispatcherPriority.Input;
            RenderingPriority_LowPriorityQueue = DispatcherPriority.SystemIdle;
        }

        void RequestRender(IReadOnlyList<FrameworkElement> frameworkElements, Action postRenderAction = null)
        {
            for (int i = 0; i < frameworkElements.Count; i++)
            {
                var fe = frameworkElements[i];

                var queueItem = new BackgroundRenderingQueueItem();

                queueItem.ElementToRender = fe;

                queueItem.PostRenderAction = postRenderAction;

                WorkQueue.Add(queueItem);
            }
        }

        public void RequestRender(FrameworkElement frameworkElement, Action postRenderAction = null)
        {
            var queueItem = new BackgroundRenderingQueueItem();

            queueItem.ElementToRender = frameworkElement;
            queueItem.PostRenderAction = postRenderAction;

            WorkQueue.Add(queueItem);
        }

        CancellationTokenSource WorkQueueProcessingCancellationTokenSource;

        Task WorkQueueProcessingTask;

        public void Stop()
        {
            if (WorkQueueProcessingCancellationTokenSource != null)
                WorkQueueProcessingCancellationTokenSource.Cancel();

            WorkQueue = new BlockingCollection<BackgroundRenderingQueueItem>();

            WorkQueueProcessingCancellationTokenSource = null;
        }

        public void Start()
        {
            Start(new Progress<int>());
        }

        public void Restart()
        {
            Stop();
            Start();
        }
        

        public void Start(IProgress<int> progress)
        {
            Stop();

            this.Progress = progress;

            WorkQueueProcessingCancellationTokenSource = new CancellationTokenSource();

            WorkQueueProcessingTask = Task.Run(
                () => StartCore(WorkQueueProcessingCancellationTokenSource.Token),
                WorkQueueProcessingCancellationTokenSource.Token);
        }

        void StartCore(CancellationToken cancellationToken)
        {
            while (true)
            {
                var queueItem = (BackgroundRenderingQueueItem)null;

                if (!WorkQueue.TryTake(out queueItem, TimeSpan.FromMilliseconds(250)))
                    continue;

                if (cancellationToken.IsCancellationRequested)
                {
                    return;
                }

                if (queueItem == null)
                    continue;

                Progress.Report(WorkQueue.Count);

                if (queueItem.ElementToRender == null)
                        continue;

                var ic = queueItem.ElementToRender as ItemsControl;

                if (ic != null)
                {
                    RenderItemsControl(ic);
                }
                else
                {
                    RenderFrameworkElement(queueItem.ElementToRender);
                }

                if (cancellationToken.IsCancellationRequested)
                    return;

                //// second part just added
                //if (queueItem.PostRenderAction != null && queueItem.FrameworkElementToRender != null)
                //{
                //    queueItem.FrameworkElementToRender.Dispatcher.BeginInvoke(new Action(() =>
                //    {
                //        queueItem.PostRenderAction();

                //    }), DefaultRenderingPriority);
                //}
            }
        }

        void RenderItemsControl(ItemsControl itemsControl)
        {
            try
            {
                itemsControl.Dispatcher.Invoke(new Action(() =>
                {
                    try
                    {
                        RenderItemsControlCore(itemsControl);
                    }
                    catch (Exception ex)
                    {
                        //         Logger.DiagnosticOnlyLogException(ex);
                    }

                }), RenderingPriority_HighPriorityQueue);
            }
            catch (Exception ex)
            {
                //        Logger.DiagnosticOnlyLogException(ex);
            }
        }

        void RenderItemsControlCore(ItemsControl itemsControl)
        {
            var itemsHost =
                (from c in itemsControl.VisualTreeTraversal(includeChildItemsControls: false, traversalMode: TreeTraversalMode.BreadthFirst).OfType<Panel>()
                 where c.IsItemsHost
                 select c).FirstOrDefault();

            if (itemsHost == null)
                return;

            using (var wait = new ItemContainerGeneratorStatusMonitor(itemsControl.ItemContainerGenerator))
            {
                EnsureGeneratorMethod.Invoke(itemsHost, new object[] { });

                wait.WaitForContainersGeneratedStatus();
            }

            for (int i = 0; i < itemsControl.ItemContainerGenerator.Items.Count; i++)
            {
                var item = itemsControl.ItemContainerGenerator.ContainerFromIndex(i) as FrameworkElement;

                if (item != null)
                {
                    RequestRender(item);
                }
            }
        }

        void RenderFrameworkElement(FrameworkElement fe)
        {
            try
            {
                fe.Dispatcher.BeginInvoke(new Action(() =>
                {
                    RenderFrameworkElementCore(fe);

                }), RenderingPriority_HighPriorityQueue).Wait();
            }
            catch (Exception ex)
            {
                //        Logger.DiagnosticOnlyLogException(ex);
            }
        }

        void RenderFrameworkElementCore(FrameworkElement fe)
        {
            try
            {
                var itemsControl = VisualTreeHelper.GetParent(fe) as Panel;

                // itemsControl may be null if this item has been disconnected
                if (itemsControl == null)
                    return;

                var x = (fe as BackgroundLoadingListViewItem);

                x.BackgroundLoadComplete = true;

                fe.InvalidateMeasure();
                fe.InvalidateVisual();
            }
            catch (Exception ex)
            {
                //         Logger.DiagnosticOnlyLogException(ex);
            }
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
