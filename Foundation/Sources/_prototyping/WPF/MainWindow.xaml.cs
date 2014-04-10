using SquaredInfinity.Foundation.Collections;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using SquaredInfinity.Foundation.Extensions;
using System.Dynamic;

namespace WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public ObservableCollectionEx<MyItem> MyItems { get; set; }

        public MainWindow()
        {
            MyItems = new ObservableCollectionEx<MyItem>();

            for (int i = 0; i < 1000; i++)
                MyItems.Add(new MyItem { Id = i });

            DataContext = this;
            InitializeComponent();
        }
    }

    public class MyListView : ListView 
    {
        BackgroundRenderingService RS = new BackgroundRenderingService();

        public MyListView()
        {
            RS.AfterItemRendered += RS_AfterItemRendered;
            RS.Start();
        }

        protected override void OnItemsSourceChanged(System.Collections.IEnumerable oldValue, System.Collections.IEnumerable newValue)
        {
            base.OnItemsSourceChanged(oldValue, newValue);

            RS.RequestRender(this);
        }

        void RS_AfterItemRendered(object sender, BackgroundRenderingService.AfterItemRenderedEventArgs e)
        {
            Trace.WriteLine("rendered, {0} in queue.".FormatWith(RS.RemainingQueueSize));
        }
    }


    public class MyStackPanel : StackPanel
    {
        //static readonly PropertyInfo PROPERTY_IsScrolling 

        dynamic wrapper;

        public bool DoNothing = true;

        public MyStackPanel()
        {
            IsItemsHost = false;
            wrapper = PrivateReflectionDynamicObjectWrapper.WrapObjectIfNeeded(this);
        }

        protected override void OnVisualChildrenChanged(DependencyObject visualAdded, DependencyObject visualRemoved)
        {
            base.OnVisualChildrenChanged(visualAdded, visualRemoved);
        }

        protected override Size ArrangeOverride(Size arrangeSize)
        {
            var sw = Stopwatch.StartNew();
            if (DoNothing)
                return arrangeSize;

            UIElementCollection internalChildren = this.InternalChildren;
            bool flag = this.Orientation == Orientation.Horizontal;
            Rect finalRect = new Rect(arrangeSize);
            double num = 0.0;

            

            if (wrapper.IsScrolling)
            {
                if (flag)
                {
                    //finalRect.X = StackPanel.ComputePhysicalFromLogicalOffset(arrangeElement, scrollData.ComputedOffset.X, true);
                    //finalRect.Y = -1.0 * scrollData.ComputedOffset.Y;
                }
                else
                {
                    //finalRect.X = -1.0 * scrollData.ComputedOffset.X;
                    //finalRect.Y = StackPanel.ComputePhysicalFromLogicalOffset(arrangeElement, scrollData.ComputedOffset.Y, false);
                }
            }
            int index = 0;
            for (int count = internalChildren.Count; index < count; ++index)
            {
                UIElement uiElement = internalChildren[index];

                if (uiElement != null)
                {
                    if (uiElement.DesiredSize.Width == 0 && uiElement.DesiredSize.Height == 0)
                        continue;

                    if (flag)
                    {
                        finalRect.X += num;
                        num = uiElement.DesiredSize.Width;
                        finalRect.Width = num;
                        finalRect.Height = Math.Max(arrangeSize.Height, uiElement.DesiredSize.Height);
                    }
                    else
                    {
                        finalRect.Y += num;
                        num = uiElement.DesiredSize.Height;
                        finalRect.Height = num;
                        finalRect.Width = Math.Max(arrangeSize.Width, uiElement.DesiredSize.Width);
                    }
                    uiElement.Arrange(finalRect);
                }
            }

            Trace.WriteLine("AO : " + sw.ElapsedMilliseconds + " , " + arrangeSize.ToString());

            return arrangeSize;

        }

        protected override Size MeasureOverride(Size constraint)
        {
            var sw = Stopwatch.StartNew();
            if (DoNothing)
                return constraint;

            return new Size(5000, 5000);

            //var x =base.MeasureOverride(constraint);

            Size size = new Size();
            UIElementCollection internalChildren = this.InternalChildren;
            Size availableSize = constraint;
            bool flag = this.Orientation == Orientation.Horizontal;
            int num1 = -1;
            int num2;
            double num3;
            if (flag)
            {
                availableSize.Width = double.PositiveInfinity;
                //if (this.IsScrolling && this.CanVerticallyScroll)
                //    availableSize.Height = double.PositiveInfinity;
                //num2 = this.IsScrolling ? StackPanel.CoerceOffsetToInteger(scrollData.Offset.X, internalChildren.Count) : 0;
                num3 = constraint.Width;
            }
            else
            {
                availableSize.Height = double.PositiveInfinity;
                //if (measureElement.IsScrolling && measureElement.CanHorizontallyScroll)
                //    availableSize.Width = double.PositiveInfinity;
                //num2 = measureElement.IsScrolling ? StackPanel.CoerceOffsetToInteger(scrollData.Offset.Y, internalChildren.Count) : 0;
                num3 = constraint.Height;
            }
            int index = 0;
            for (int count = internalChildren.Count; index < count; ++index)
            {
                UIElement uiElement = internalChildren[index];
                if (uiElement != null)
                {
                    if (uiElement.DesiredSize.Width == 0 && uiElement.DesiredSize.Height == 0)
                        continue;
              //      uiElement.Measure(availableSize);
                    Size desiredSize = uiElement.DesiredSize;
                    double num4;
                    if (flag)
                    {
                        size.Width += desiredSize.Width;
                        size.Height = Math.Max(size.Height, desiredSize.Height);
                        num4 = desiredSize.Width;
                    }
                    else
                    {
                        size.Width = Math.Max(size.Width, desiredSize.Width);
                        size.Height += desiredSize.Height;
                        num4 = desiredSize.Height;
                    }
                    //if (measureElement.IsScrolling && num1 == -1 && index >= num2)
                    //{
                    //    num3 -= num4;
                    //    if (DoubleUtil.LessThanOrClose(num3, 0.0))
                    //        num1 = index;
                    //}
                }
            }
            //if (measureElement.IsScrolling)
            //{
            //    Size viewport = constraint;
            //    Size extent = size;
            //    Vector offset = scrollData.Offset;
            //    if (num1 == -1)
            //        num1 = internalChildren.Count - 1;
            //    while (num2 > 0)
            //    {
            //        double num4 = num3;
            //        double num5 = !flag ? num4 - internalChildren[num2 - 1].DesiredSize.Height : num4 - internalChildren[num2 - 1].DesiredSize.Width;
            //        if (!DoubleUtil.LessThan(num5, 0.0))
            //        {
            //            --num2;
            //            num3 = num5;
            //        }
            //        else
            //            break;
            //    }
            //    int count = internalChildren.Count;
            //    int num6 = num1 - num2;
            //    if (num6 == 0 || DoubleUtil.GreaterThanOrClose(num3, 0.0))
            //        ++num6;
            //    if (flag)
            //    {
            //        scrollData.SetPhysicalViewport(viewport.Width);
            //        viewport.Width = (double)num6;
            //        extent.Width = (double)count;
            //        offset.X = (double)num2;
            //        offset.Y = Math.Max(0.0, Math.Min(offset.Y, extent.Height - viewport.Height));
            //    }
            //    else
            //    {
            //        scrollData.SetPhysicalViewport(viewport.Height);
            //        viewport.Height = (double)num6;
            //        extent.Height = (double)count;
            //        offset.Y = (double)num2;
            //        offset.X = Math.Max(0.0, Math.Min(offset.X, extent.Width - viewport.Width));
            //    }
            //    size.Width = Math.Min(size.Width, constraint.Width);
            //    size.Height = Math.Min(size.Height, constraint.Height);
            //    StackPanel.VerifyScrollingData(measureElement, scrollData, viewport, extent, offset);
            //}
            return size;

             Trace.WriteLine("MO : " + sw.ElapsedMilliseconds);
            //return x;
        }
    }

    public class MyItem
    {
        int _id;
        public int Id 
        {
            get { return _id; }
            set { _id = value; }
        }
    }

    public class BackgroundRenderingService
    {
        static readonly Type ItemsControlType;
        static readonly PropertyInfo ItemsHostProperty;

        static readonly Type PanelType;
        static readonly MethodInfo EnsureGeneratorMethod;

        readonly IObservable<int> ItemsInLastWindowObservable;

        public event EventHandler<AfterItemRenderedEventArgs> AfterItemRendered;

        /// <summary>
        /// Default Rendering Priority may be set to high value (e.g. Input) to make it quicker, but it will stop UI from being updated
        /// Rendering queue will be observed and after specified period of time (e.g. 3s) priority will be lowered to ContextIdle to allow UI to catch up.
        /// After this happens Rendering Priority will revert back to Default.
        /// The process continues unitll there's no more items to render
        /// </summary>
        public DispatcherPriority DefaultRenderingPriority { get; set; }
        public DispatcherPriority CurrentRenderingPriority { get; set; }

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
            DefaultRenderingPriority = DispatcherPriority.Input;
            CurrentRenderingPriority = DefaultRenderingPriority;

            var afterItemRenderedObservable =
                Observable.FromEventPattern<AfterItemRenderedEventArgs>(this, "AfterItemRendered");

            ItemsInLastWindowObservable =
                (from x in afterItemRenderedObservable.Window(TimeSpan.FromSeconds(3))
                 select x.Count()).Switch();
        }

        void RequestRender(List<ListViewItem> items)
        {
            var queueItem = new BackgroundRenderingQueueItem_Multiple();

            queueItem.FrameworkElementsToRender = items;
            //queueItem.PostRenderAction = postRenderAction;

            WorkQueue.Add(queueItem);
        }

        public void RequestRender(FrameworkElement frameworkElement, Action postRenderAction = null)
        {
            var queueItem = new BackgroundRenderingQueueItem();

            queueItem.FrameworkElementToRender = frameworkElement;
            queueItem.DataContext = frameworkElement.DataContext;
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

            if (ItemsRenderedWindowEndedSubscription != null)
                ItemsRenderedWindowEndedSubscription.Dispose();

            ItemsRenderedWindowEndedSubscription = null;
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

        IDisposable ItemsRenderedWindowEndedSubscription;


        public void Start(IProgress<int> progress)
        {
            Stop();

            this.Progress = progress;

            WorkQueueProcessingCancellationTokenSource = new CancellationTokenSource();

            WorkQueueProcessingTask = Task.Run(
                () => StartCore(WorkQueueProcessingCancellationTokenSource.Token),
                WorkQueueProcessingCancellationTokenSource.Token);

            ItemsRenderedWindowEndedSubscription = ItemsInLastWindowObservable.Where(c => c > 0).Subscribe((i) =>
            {
                // there were no events
                if (i == 0)
                {
                    CurrentRenderingPriority = DefaultRenderingPriority;
                }
                else // there were events
                {
                    if (RemainingQueueSize < 50)
                        return;

                    CurrentRenderingPriority = DispatcherPriority.ContextIdle;
                }
            });
        }

        void StartCore(CancellationToken cancellationToken)
        {
            while (true)
            {
                var queueItem = (BackgroundRenderingQueueItem)null;

                WorkQueue.TryTake(out queueItem, TimeSpan.FromMilliseconds(250));

                if (cancellationToken.IsCancellationRequested)
                {
                    return;
                }

                if (queueItem == null)
                    continue;

                Progress.Report(WorkQueue.Count);

                //if (queueItem.FrameworkElementToRender == null)
                //    continue;

                var multiQueItem = queueItem as BackgroundRenderingQueueItem_Multiple;

                if (multiQueItem == null)
                {
                    if (queueItem.FrameworkElementToRender is ItemsControl)
                    {
                        //var sw = Stopwatch.StartNew();

                        bool rendered = TryRender(queueItem.FrameworkElementToRender as ItemsControl);

                        if (AfterItemRendered != null)
                            AfterItemRendered(this, new AfterItemRenderedEventArgs(queueItem.FrameworkElementToRender, queueItem.DataContext));

                        if (rendered)
                        {
                            //  Debug.WriteLine(string.Format("Rendered item in {0}ms. {1} items left in queue", sw.Elapsed.TotalMilliseconds, WorkQueue.Count));
                        }
                        else
                        {
                            //                        Debug.WriteLine(string.Format("Item doesn't need rendering or rendering failed.", sw.Elapsed.TotalMilliseconds, WorkQueue.Count));
                        }
                    }
                    else
                    {
                        TryRender(queueItem.FrameworkElementToRender);

                        if (AfterItemRendered != null)
                            AfterItemRendered(this, new AfterItemRenderedEventArgs(queueItem.FrameworkElementToRender, queueItem.DataContext));
                    }
                }
                else
                {
                    TryRender(multiQueItem.FrameworkElementsToRender);
                }

                if (cancellationToken.IsCancellationRequested)
                    return;

                // second part just added
                if (queueItem.PostRenderAction != null && queueItem.FrameworkElementToRender != null)
                {
                    queueItem.FrameworkElementToRender.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        queueItem.PostRenderAction();

                    }), DefaultRenderingPriority);
                }
            }
        }

        public IReadOnlyList<IReadOnlyList<FrameworkElement>> Chunks(IReadOnlyList<FrameworkElement> input)
        {
            List<List<FrameworkElement>> chunks = new List<List<FrameworkElement>>();
            int chunkSize = 1;

            for (int i = 0; i < input.Count / chunkSize; i++)
            {
                var c = new List<FrameworkElement>();

                for(int x = i * chunkSize; x < Math.Min((i + 1) * chunkSize, input.Count); x++)
                {
                    c.Add(input[x]);
                }

                chunks.Add(c);
            }

            return chunks;

        }

        private bool TryRender(IReadOnlyList<FrameworkElement> readOnlyList)
        {
            bool rendered = false;

            try
            {

             //   var itemsControl = (MyStackPanel)null;

                foreach (var c in Chunks(readOnlyList))
                {
                    foreach (var item in c)
                    {
                        var are = new AutoResetEvent(false);


                        item.Dispatcher.BeginInvoke(new Action(() =>
                        {
                            try
                            {

                                item.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                                //                        element.Arrange(new Rect(element.DesiredSize));

                                //element.UpdateLayout();

                                if (c.Last() == item)
                                {
                                    var itemsControl = VisualTreeHelper.GetParent(item) as MyStackPanel;

                                    //var r = new Random();

                                    //if (r.Next() % 50 == 0)
                                    {
                                        //itemsControl.IsItemsHost = true;
                                       itemsControl.DoNothing = false;
                                        //itemsControl.UpdateLayout();
                                        itemsControl.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                                        itemsControl.Arrange(new Rect(itemsControl.DesiredSize));
                                        //itemsControl.UpdateLayout();

                                        itemsControl.DoNothing = true;

                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                //         Logger.DiagnosticOnlyLogException(ex);
                            }

                            are.Set();

                        }), CurrentRenderingPriority);

                        are.WaitOne(TimeSpan.FromMilliseconds(250));
                    }
                }

                Trace.WriteLine("ALL DONE :)");

            }
            catch (Exception ex)
            {
                //        Logger.DiagnosticOnlyLogException(ex);
            }

            return rendered;
        }

        bool TryRender(ItemsControl itemsControl)
        {
            bool rendered = false;

            try
            {
                var are = new AutoResetEvent(false);

                itemsControl.Dispatcher.BeginInvoke(new Action(() =>
                {
                    try
                    {
                        rendered = RenderCore(itemsControl);
                    }
                    catch (Exception ex)
                    {
               //         Logger.DiagnosticOnlyLogException(ex);
                    }

                    are.Set();

                }), CurrentRenderingPriority);

                are.WaitOne(TimeSpan.FromMilliseconds(250));
            }
            catch (Exception ex)
            {
        //        Logger.DiagnosticOnlyLogException(ex);
            }

            return rendered;
        }

        bool TryRender(FrameworkElement element)
        {
            bool rendered = false;

            try
            {
                var are = new AutoResetEvent(false);

                element.Dispatcher.BeginInvoke(new Action(() =>
                {
                    try
                    {
                        element.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
//                        element.Arrange(new Rect(element.DesiredSize));

                        //element.UpdateLayout();

                        //var itemsControl = VisualTreeHelper.GetParent(element) as MyStackPanel;

                        //var r = new Random();

                        //if (r.Next() % 50 == 0)
                        //{
                        //    itemsControl.DoNothing = false;
                        //    itemsControl.UpdateLayout();
                        //    itemsControl.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                        //    itemsControl.Arrange(new Rect(itemsControl.DesiredSize));

                        //    itemsControl.DoNothing = true;

                        //}
                    }
                    catch (Exception ex)
                    {
                        //         Logger.DiagnosticOnlyLogException(ex);
                    }

                    are.Set();

                }), CurrentRenderingPriority);

                are.WaitOne(TimeSpan.FromMilliseconds(250));
            }
            catch (Exception ex)
            {
                //        Logger.DiagnosticOnlyLogException(ex);
            }

            return rendered;
        }

        bool RenderCore(ItemsControl itemsControl)
        {
            itemsControl.ApplyTemplate();

            var presenter = itemsControl.Template.FindName("ItemsHost", itemsControl) as ItemsPresenter;

            if(presenter == null)
            {
                presenter =
                    (from c in itemsControl.VisualTreeTraversal().OfType<ItemsPresenter>()
                     select c).FirstOrDefault();
            }

            if (presenter != null)
            {
                presenter.ApplyTemplate();
            }
            else
            {
                
                return false;
            }

            itemsControl.UpdateLayout();

          //  var itemsHost = ItemsHostProperty.GetValue(itemsControl);// as Panel;

            var itemsHost =
                (from c in itemsControl.VisualTreeTraversal().OfType<MyStackPanel>()
                 select c).FirstOrDefault();

          //  itemsHost.DoNothing = false;
            itemsHost.IsItemsHost = true;

            using (var wait = new ItemContainerGeneratorStatusMonitor(itemsControl.ItemContainerGenerator))
            {
                EnsureGeneratorMethod.Invoke(itemsHost, new object[] { });

                wait.WaitForContainersGeneratedStatus();
            }

            List<ListViewItem> items = new List<ListViewItem>();

            for(int i = 0 ; i < itemsControl.ItemContainerGenerator.Items.Count; i++)
            {
                var c = itemsControl.ItemContainerGenerator.ContainerFromIndex(i) as ListViewItem;
                c.ApplyTemplate();
                items.Add(c);
            }

            //itemsHost.IsItemsHost = false;

            RequestRender(items);

            Trace.WriteLine("DONE");

            //if (itemsControl.IsVisible)
            //    return false;

            

            //itemsControl.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
            //itemsControl.Arrange(new Rect(itemsControl.DesiredSize));

            return true;
        }

        class ItemContainerGeneratorStatusMonitor : IDisposable
        {
            readonly ItemContainerGenerator Generator;
            readonly EventWaitHandle WaitHandle = new ManualResetEvent(false);

            public ItemContainerGeneratorStatusMonitor(ItemContainerGenerator generator)
            {
                this.Generator = generator;
                Generator.StatusChanged += OnContainerGeneratorStatusChanged;
            }

            public void WaitForContainersGeneratedStatus()
            {
                if (Generator.Status == GeneratorStatus.ContainersGenerated)
                    WaitHandle.Set();

                if (!WaitHandle.WaitOne(TimeSpan.FromMilliseconds(250)))
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
            public FrameworkElement FrameworkElementToRender { get; set; }
            public object DataContext { get; set; }
            public Action PostRenderAction = () => { };
        }

        class BackgroundRenderingQueueItem_Multiple : BackgroundRenderingQueueItem
        {
            public IReadOnlyList<FrameworkElement> FrameworkElementsToRender { get; set; }
        }
    }

    public class PrivateReflectionDynamicObjectWrapper : DynamicObject
    {
        static IDictionary<Type, IDictionary<string, IProperty>> TypeToPropertyMappings = new ConcurrentDictionary<Type, IDictionary<string, IProperty>>();

        // abstraction to make field and property access consistent
        interface IProperty
        {
            string Name { get; }
            object GetValue(object obj, object[] index);
            void SetValue(object obj, object val, object[] index);
        }

        // IProperty implementation over a PropertyInfo
        class Property : IProperty
        {
            internal PropertyInfo PropertyInfo { get; set; }

            string IProperty.Name
            {
                get
                {
                    return PropertyInfo.Name;
                }
            }

            object IProperty.GetValue(object obj, object[] index)
            {
                return PropertyInfo.GetValue(obj, index);
            }

            void IProperty.SetValue(object obj, object val, object[] index)
            {
                PropertyInfo.SetValue(obj, val, index);
            }
        }

        // IProperty implementation over a FieldInfo
        class Field : IProperty
        {
            internal FieldInfo FieldInfo { get; set; }

            string IProperty.Name
            {
                get
                {
                    return FieldInfo.Name;
                }
            }

            object IProperty.GetValue(object obj, object[] index)
            {
                return FieldInfo.GetValue(obj);
            }

            void IProperty.SetValue(object obj, object val, object[] index)
            {
                FieldInfo.SetValue(obj, val);
            }
        }


        object WrappedObject { get; set; }

        const BindingFlags DefaultBindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

        internal static object WrapObjectIfNeeded(object o)
        {
            // Don't wrap primitive types, string, nulls
            if (o == null || o.GetType().IsPrimitive || o is string)
                return o;

            return new PrivateReflectionDynamicObjectWrapper() { WrappedObject = o };
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            IProperty prop = GetProperty(binder.Name);

            // Get the property value
            result = prop.GetValue(WrappedObject, index: null);

            // Wrap the sub object if necessary. This allows nested anonymous objects to work.
            result = WrapObjectIfNeeded(result);

            return true;
        }

        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            IProperty prop = GetProperty(binder.Name);

            // Set the property value
            prop.SetValue(WrappedObject, value, index: null);

            return true;
        }

        public override bool TryGetIndex(GetIndexBinder binder, object[] indexes, out object result)
        {
            // The indexed property is always named "Item" in C#
            IProperty prop = GetIndexProperty();
            result = prop.GetValue(WrappedObject, indexes);

            // Wrap the sub object if necessary. This allows nested anonymous objects to work.
            result = WrapObjectIfNeeded(result);

            return true;
        }

        public override bool TrySetIndex(SetIndexBinder binder, object[] indexes, object value)
        {
            // The indexed property is always named "Item" in C#
            IProperty prop = GetIndexProperty();
            prop.SetValue(WrappedObject, value, indexes);
            return true;
        }

        // Called when a method is called
        public override bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result)
        {
            result = InvokeMemberOnType(WrappedObject.GetType(), WrappedObject, binder.Name, args);

            // Wrap the sub object if necessary. This allows nested anonymous objects to work.
            result = WrapObjectIfNeeded(result);

            return true;
        }

        public override bool TryConvert(ConvertBinder binder, out object result)
        {
            result = Convert.ChangeType(WrappedObject, binder.Type);
            return true;
        }

        public override string ToString()
        {
            return WrappedObject.ToString();
        }

        private IProperty GetIndexProperty()
        {
            // The index property is always named "Item" in C#
            return GetProperty("Item");
        }

        private IProperty GetProperty(string propertyName)
        {

            // Get the list of properties and fields for this type
            IDictionary<string, IProperty> typeProperties = GetTypeProperties(WrappedObject.GetType());

            // Look for the one we want
            IProperty property;
            if (typeProperties.TryGetValue(propertyName, out property))
            {
                return property;
            }

            // The property doesn't exist

            // Get a list of supported properties and fields and show them as part of the exception message
            // For fields, skip the auto property backing fields (which name start with <)
            var propNames = typeProperties.Keys.Where(name => name[0] != '<').OrderBy(name => name);
            throw new ArgumentException(
                String.Format(
                "The property {0} doesn't exist on type {1}. Supported properties are: {2}",
                propertyName, WrappedObject.GetType(), String.Join(", ", propNames)));
        }

        private static IDictionary<string, IProperty> GetTypeProperties(Type type)
        {
            // First, check if we already have it cached
            IDictionary<string, IProperty> typeProperties;
            if (TypeToPropertyMappings.TryGetValue(type, out typeProperties))
            {
                return typeProperties;
            }

            // Not cache, so we need to build it

            typeProperties = new ConcurrentDictionary<string, IProperty>();

            // First, add all the properties
            foreach (PropertyInfo prop in type.GetProperties(DefaultBindingFlags).Where(p => p.DeclaringType == type))
            {
                typeProperties[prop.Name] = new Property() { PropertyInfo = prop };
            }

            // Now, add all the fields
            foreach (FieldInfo field in type.GetFields(DefaultBindingFlags).Where(p => p.DeclaringType == type))
            {
                typeProperties[field.Name] = new Field() { FieldInfo = field };
            }

            // Finally, recurse on the base class to add its fields
            if (type.BaseType != null)
            {
                foreach (IProperty prop in GetTypeProperties(type.BaseType).Values)
                {
                    typeProperties[prop.Name] = prop;
                }
            }

            // Cache it for next time
            TypeToPropertyMappings[type] = typeProperties;

            return typeProperties;
        }

        readonly ConcurrentDictionary<string, MethodInfo> MethodNameToMethodInfoCustomMappings = new ConcurrentDictionary<string, MethodInfo>();

        public void AddCustomMethodMapping(string methodName, MethodInfo methodInfo)
        {
            MethodNameToMethodInfoCustomMappings.AddOrUpdate(methodName, methodInfo, (key, oldValue) => methodInfo);
        }

        object InvokeMemberOnType(Type type, object target, string name, object[] args)
        {
            try
            {
                var customMethodInfo = (MethodInfo)null;
                if (MethodNameToMethodInfoCustomMappings.TryGetValue(name, out customMethodInfo))
                {
                    return customMethodInfo.Invoke(target, args);
                }

                // Try to invoke the method
                return type.InvokeMember(
                    name,
                    BindingFlags.InvokeMethod | DefaultBindingFlags,
                    null,
                    target,
                    args);
            }
            catch (MissingMethodException)
            {
                // If we couldn't find the method, try on the base class
                if (type.BaseType != null)
                {
                    return InvokeMemberOnType(type.BaseType, target, name, args);
                }

                throw;
            }
        }
    }
}
