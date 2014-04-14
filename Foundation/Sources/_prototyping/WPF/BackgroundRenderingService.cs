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
    public static class IEnumerableExtensions
    {
        public static IEnumerable<IEnumerable<T>> Chunkify<T>(
            this IEnumerable<T> enumerable,
            int chunkSize)
        {
            if (chunkSize < 1)
                throw new ArgumentException("chunkSize");

            using (var enumerator = enumerable.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    yield return enumerator.GetChunk(chunkSize);
                }
            }
        }

        private static IEnumerable<T> GetChunk<T>(
            this IEnumerator<T> enumerator,
            int chunkSize)
        {
            do 
            {
                yield return enumerator.Current;
            }
            while (--chunkSize > 0 && enumerator.MoveNext());
        }
    }

    public class PriorityQueueWithHighPriorityStack<TValue> : PriorityQueue<TValue>
    {
        //  priority level 0 -  high priority stack
        //                      will be processed before any other priorities

        readonly ConcurrentStack<PriorityQueueItem<TValue>>[] InternalHighPriorityStacks;

        public int HighPriorityStacksCount;

        public PriorityQueueWithHighPriorityStack(int highPriorityStacks, int lowPriorityQueues)
            : base(lowPriorityQueues - highPriorityStacks)
        {
            HighPriorityStacksCount = highPriorityStacks;

            InternalHighPriorityStacks = new ConcurrentStack<PriorityQueueItem<TValue>>[highPriorityStacks];

            for(int i = 0; i < highPriorityStacks; i++)
                InternalHighPriorityStacks[i] = new ConcurrentStack<PriorityQueueItem<TValue>>();
        }

        public override bool TryAdd(PriorityQueueItem<TValue> item)
        {
            if (item.Priority < HighPriorityStacksCount)
            {
                var stack = InternalHighPriorityStacks[item.Priority];
                stack.Push(item);
                IncrementCount();
                return true;
            }
            else
                return base.TryAdd(item);
        }

        public override bool TryTake(out PriorityQueueItem<TValue> item)
        {
            for (int i = 0; i < HighPriorityStacksCount; i++)
            {
                var stack = InternalHighPriorityStacks[i];

                if (stack.TryPop(out item))
                {
                    DecrementCount();
                    return true;
                }
            }

            return base.TryTake(out item);
        }

        public override IEnumerator<PriorityQueueItem<TValue>> GetEnumerator()
        {
            for (int i = 0; i < HighPriorityStacksCount; i++)
            {
                foreach (var item in InternalHighPriorityStacks[i])
                    yield return item;
            }

            for (int i = 0; i < PriorityCount; i++)
            {
                foreach (var item in InternalQueues[i])
                    yield return item;
            }
        }
    }

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

        public static void RequestRender(int priority, FrameworkElement fe)
        {
            Instance.RequestRender(priority, fe);
        }

        internal static void RequestRender(int priority, List<FrameworkElement> itemsToRender)
        {
            Instance.RequestRender(priority, itemsToRender);
        }

        public static int RemainingQueueSize
        {
            get { return Instance.RemainingQueueSize; }
        }
    }

    class BackgroundRenderingServiceImplementation
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

        BlockingCollection<PriorityQueueItem<BackgroundRenderingQueueItem>> WorkQueue =
            new BlockingCollection<PriorityQueueItem<BackgroundRenderingQueueItem>>(new PriorityQueueWithHighPriorityStack<BackgroundRenderingQueueItem>(2, 8));

        public int RemainingQueueSize
        {
            get { return WorkQueue.Count; }
        }

        IProgress<int> Progress;

        static BackgroundRenderingServiceImplementation()
        {
            ItemsControlType = typeof(ItemsControl);
            ItemsHostProperty = ItemsControlType.GetProperty("ItemsHost", BindingFlags.Instance | BindingFlags.NonPublic);

            PanelType = typeof(Panel);
            EnsureGeneratorMethod = PanelType.GetMethod("EnsureGenerator", BindingFlags.NonPublic | BindingFlags.Instance);
        }

        public BackgroundRenderingServiceImplementation()
        {
            RenderingPriority_HighPriorityQueue = DispatcherPriority.Input;
            RenderingPriority_LowPriorityQueue = DispatcherPriority.SystemIdle;
        }


        public void RequestRender(int priority, List<FrameworkElement> itemsToRender)
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
                RequestRender(priority, fe);
            }
        }

        public void RequestRender(int priority, FrameworkElement frameworkElement, Action postRenderAction = null)
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

            var item = new PriorityQueueItem<BackgroundRenderingQueueItem> { Priority = priority, Value = queueItem };

            WorkQueue.Add(item);
        }

        CancellationTokenSource WorkQueueProcessingCancellationTokenSource;

        Task WorkQueueProcessingTask;

        public void Stop()
        {
            if (WorkQueueProcessingCancellationTokenSource != null)
                WorkQueueProcessingCancellationTokenSource.Cancel();

            WorkQueue = new BlockingCollection<PriorityQueueItem<BackgroundRenderingQueueItem>>(new PriorityQueueWithHighPriorityStack<BackgroundRenderingQueueItem>(2, 8));

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
                Progress.Report(WorkQueue.Count);

                var pqi = (PriorityQueueItem<BackgroundRenderingQueueItem>) null;

                if (!WorkQueue.TryTake(out pqi, TimeSpan.FromMilliseconds(250)))
                    continue;

                var queueItem = pqi.Value;

                if (cancellationToken.IsCancellationRequested)
                {
                    return;
                }

                if (queueItem == null)
                    continue;

                if (queueItem.ElementToRender == null)
                    continue;

                var br = queueItem.ElementToRender as ISupportsBackgroundRendering;
                if (br != null && br.BackgroundRenderingComplete)
                    continue;

                if (pqi.Priority < 2)
                {
                    if (!pqi.Value.ElementToRender.IsVisible)
                    {
                        RequestRender(2, pqi.Value.ElementToRender);
                        continue;
                    }
                }

                var dispatcherPriority = pqi.Priority == 0 ? RenderingPriority_HighPriorityQueue : RenderingPriority_LowPriorityQueue;

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

            if (itemsControl.Name == "two")
            {

            }

            var itemsHost =
                (from c in itemsControl.VisualTreeTraversal(includeChildItemsControls: false, traversalMode: TreeTraversalMode.BreadthFirst).OfType<Panel>()
                 where c.IsItemsHost
                 select c).FirstOrDefault();

            if (itemsHost == null)
            {
                // todo: surely some of this is not needed

                itemsControl.ApplyTemplate();
                itemsControl.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                itemsControl.Arrange(new Rect(itemsControl.DesiredSize));

                itemsHost =
                (from c in itemsControl.VisualTreeTraversal(includeChildItemsControls: false, traversalMode: TreeTraversalMode.BreadthFirst).OfType<Panel>()
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

            int priority = itemsControl.IsVisible ? 1 : 2;

            var sv = itemsControl.FindDescendant<ScrollViewer>();

            for (int i = 0; i < itemsControl.ItemContainerGenerator.Items.Count; i++)
            {
                var item = itemsControl.ItemContainerGenerator.ContainerFromIndex(i) as FrameworkElement;

                if (item != null)
                {
                    RequestRender(priority, item);
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

                    int priority = contentFe.IsVisible ? 1 : 2;

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
