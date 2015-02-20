//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Runtime.Caching;
//using System.Text;
//using System.Threading.Tasks;


//{
//    // todo: allow to specify default cache item policy for group
//    public class CacheGroup : ICacheService
//    {
//        readonly ICacheService Owner;
        
//        public bool IsTransient { get; private set; }

//        readonly string GroupName;

//        public CacheGroup(ICacheService owner)
//            : this(owner, Guid.NewGuid().ToString())
//        {
//            IsTransient = true;
//        }

//        public CacheGroup(ICacheService owner, string groupName)
//        {
//            this.Owner = owner;
//            this.GroupName = groupName + ".";

//            _isCacheEnabled = true;
//        }

//        bool _isCacheEnabled;
//        public bool IsCacheEnabled
//        {
//            get
//            {
//                if (!Owner.IsCacheEnabled)
//                    return false;

//                return _isCacheEnabled;
//            }
//            set
//            {
//                _isCacheEnabled = value;
//            }
//        }

//        public void Remove(string key)
//        {
//            Owner.Remove(GroupName + key);
//        }

//        public T GetOrAdd<T>(string key, Func<T> valueFactory)
//        {
//            return Owner.GetOrAdd(GroupName + key, valueFactory);
//        }

//        public T GetOrAdd<T>(string key, Func<T> valueFactory, CacheItemPolicy cacheItemPolicy)
//        {
//            return Owner.GetOrAdd(GroupName + key, valueFactory, cacheItemPolicy);
//        }

//        public void AddOrUpdate<T>(string key, T value)
//        {
//            Owner.AddOrUpdate(GroupName + key, value);
//        }

//        public void AddOrUpdate<T>(string key, T value, Func<string, T, T> updateValueFactory = null)
//        {
//            Owner.AddOrUpdate(GroupName + key, value, updateValueFactory);
//        }

//        public void ClearAll()
//        {
//            Owner.ClearAll();
//        }

//        public ICacheService NewTransientCacheGroup()
//        {
//            return new CacheGroup(this);
//        }

//        public ICacheService NewCacheGroup(string groupName)
//        {
//            return new CacheGroup(this, groupName);
//        }
//    }
//}


defaultXamlREsources implement IXamlResourcesProvider + import order
modern window
Converters.ColumnHeaderRoleToBorder
    public class ColumnHeaderRoleToBorderConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var header = value as GridViewColumnHeader;

            if (header == null)
                return DependencyProperty.UnsetValue;

            if (header.Role == GridViewColumnHeaderRole.Padding)
                return Application.Current.Resources["GridView.ColumnHeader.Padding.Border"];

            return Application.Current.Resources["GridView.ColumnHeader.Normal.Border"];
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value;
        }
    }
     public static partial class ButtonBehaviors
    {
        #region Image

        public static readonly DependencyProperty ImageProperty =
            DependencyProperty.RegisterAttached(
            "Image",
            typeof(ImageSource),
            typeof(ButtonBehaviors),
            new PropertyMetadata(null));

        public static void SetImage(System.Windows.Controls.Button element, ImageSource value)
        {
            element.SetValue(ImageProperty, value);
        }

        public static ImageSource GetImage(System.Windows.Controls.Button element)
        {
            return (ImageSource)element.GetValue(ImageProperty);
        }

        #endregion

        #region Image Width

        public static readonly DependencyProperty ImageWidthProperty =
            DependencyProperty.RegisterAttached(
            "ImageWidth",
            typeof(double),
            typeof(ButtonBehaviors),
            new PropertyMetadata(null));

        public static void SetImageWidth(System.Windows.Controls.Button element, double value)
        {
            element.SetValue(ImageWidthProperty, value);
        }

        public static double GetImageWidth(System.Windows.Controls.Button element)
        {
            return (double)element.GetValue(ImageWidthProperty);
        }

        #endregion

        #region Image Height

        public static readonly DependencyProperty ImageHeightProperty =
            DependencyProperty.RegisterAttached(
            "ImageHeight",
            typeof(double),
            typeof(ButtonBehaviors),
            new PropertyMetadata(null));

        public static void SetImageHeight(System.Windows.Controls.Button element, double value)
        {
            element.SetValue(ImageHeightProperty, value);
        }

        public static double GetImageHeight(System.Windows.Controls.Button element)
        {
            return (double)element.GetValue(ImageHeightProperty);
        }

        #endregion



    }

        public class ModernWindow : ViewHostWindow
    {
        #region Title Horizontal Alignment

        public HorizontalAlignment TitleHorizontalAlignment
        {
            get { return (HorizontalAlignment)GetValue(TitleHorizontalAlignmentProperty); }
            set { SetValue(TitleHorizontalAlignmentProperty, value); }
        }

        public static readonly DependencyProperty TitleHorizontalAlignmentProperty =
            DependencyProperty.Register(
            "TitleHorizontalAlignment", 
            typeof(HorizontalAlignment),
            typeof(ModernWindow), 
            new PropertyMetadata(HorizontalAlignment.Left));

        #endregion

        #region Title Bar Additional Content

        public FrameworkElement TitleBarAdditionalContent
        {
            get { return (FrameworkElement)GetValue(TitleBarAdditionalContentProperty); }
            set { SetValue(TitleBarAdditionalContentProperty, value); }
        }

        public static readonly DependencyProperty TitleBarAdditionalContentProperty =
            DependencyProperty.Register(
            "TitleBarAdditionalContent", 
            typeof(FrameworkElement),
            typeof(ModernWindow), 
            new PropertyMetadata(null));

        #endregion

        static ModernWindow()
        {
            //DefaultStyleKeyProperty.OverrideMetadata(typeof(ModernWindow), new FrameworkPropertyMetadata("Styles.ModernWindow"));
        }

        public ModernWindow()
        {
            this.CommandBindings.Add(new CommandBinding(SystemCommands.CloseWindowCommand, this.OnCloseWindow));
            this.CommandBindings.Add(new CommandBinding(SystemCommands.MaximizeWindowCommand, this.OnMaximizeWindow, this.OnCanResizeWindow));
            this.CommandBindings.Add(new CommandBinding(SystemCommands.MinimizeWindowCommand, this.OnMinimizeWindow, this.OnCanMinimizeWindow));
            this.CommandBindings.Add(new CommandBinding(SystemCommands.RestoreWindowCommand, this.OnRestoreWindow, this.OnCanResizeWindow));
        }

        protected override void OnViewModelEvent(SquaredInfinity.Foundation.Presentation.ViewModels.ViewModelEventArgs args)
        {
            base.OnViewModelEvent(args);
        }

        private void OnCanResizeWindow(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = this.ResizeMode == ResizeMode.CanResize || this.ResizeMode == ResizeMode.CanResizeWithGrip;
        }

        private void OnCanMinimizeWindow(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = this.ResizeMode != ResizeMode.NoResize;
        }

        private void OnCloseWindow(object target, ExecutedRoutedEventArgs e)
        {
            SystemCommands.CloseWindow(this);
        }

        private void OnMaximizeWindow(object target, ExecutedRoutedEventArgs e)
        {
            SystemCommands.MaximizeWindow(this);
        }

        private void OnMinimizeWindow(object target, ExecutedRoutedEventArgs e)
        {
            SystemCommands.MinimizeWindow(this);
        }

        private void OnRestoreWindow(object target, ExecutedRoutedEventArgs e)
        {
            SystemCommands.RestoreWindow(this);
        }
    }

    //public static class GridView

    public static class Columns
    {
        public static readonly DependencyProperty MinWidthProperty =
            DependencyProperty.RegisterAttached(
            "MinWidth",
            typeof(double),
            typeof(Columns),
            new PropertyMetadata(OnMinWidthChanged));

        static void OnMinWidthChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var lv = d as ListView;

            if (lv == null)
                return;

            lv.AddHandler(Thumb.DragDeltaEvent, new DragDeltaEventHandler(OnThumDragDelta), handledEventsToo: true);
        }

        static void OnThumDragDelta(object sender, DragDeltaEventArgs e)
        {
            var thumb = e.OriginalSource as Thumb;

            if (thumb == null)
                return;

            var header = thumb.TemplatedParent as GridViewColumnHeader;

            // Header may be null if, for example, thumb is part of ScrollViewer (i.e. not a thumb we are monitoring)
            if (header == null)
                return;

            var minWidth = GetMinWidth(sender as ListView);

            if (header.Column.ActualWidth.IsLessThan(minWidth))
            {
                header.Column.Width = minWidth;
            }
        }

        public static void SetMinWidth(System.Windows.Controls.ListView element, double value)
        {
            element.SetValue(MinWidthProperty, value);
        }

        public static double GetMinWidth(System.Windows.Controls.ListView element)
        {
            return (double)element.GetValue(MinWidthProperty);
        }
    }

    selection.
    public partial class SelectionChanged
    {
        #region Command

        public static void SetCommand(System.Windows.Controls.TreeView element, ICommand value)
        {
            element.SetValue(CommandProperty, value);
        }

        public static ICommand GetCommand(System.Windows.Controls.TreeView element)
        {
            return (ICommand)element.GetValue(CommandProperty);
        }

        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.RegisterAttached(
            "Command",
            typeof(ICommand),
            typeof(SelectionChanged),
            new PropertyMetadata(null, OnCommandChanged));

        static void OnCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var c = d as UIElement;

            if (c == null)
                return;

            var tv = c as System.Windows.Controls.TreeView;
            if (tv != null)
            {
                if ((ICommand)e.NewValue != null)
                {
                    tv.SelectedItemChanged -= tv_SelectedItemChanged;
                    tv.SelectedItemChanged += tv_SelectedItemChanged;
                }
                else
                {
                    tv.SelectedItemChanged -= tv_SelectedItemChanged;
                }
            }
        }

        static void tv_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            var tv = sender as System.Windows.Controls.TreeView;

            if (tv == null)
                return;

            var command = GetCommand(tv);

            if(command != null && command.CanExecute(tv.SelectedItem))
            {
                command.Execute(tv.SelectedItem);
            }
        }


        #endregion
    }
    VSC.BorderGapMaskConverter
    combobox_popup_background	= "#FFFFFFFF";

    combobox <Setter Property="StaysOpenOnEdit"              Value="True" />


    public class ToListConverter : IMultiValueConverter
    {

        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return new List<object>(values);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class ToMapConverter : IMultiValueConverter
    {

        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var result = new Dictionary<string, object>(StringComparer.InvariantCultureIgnoreCase);

            for(int i = 0; i < values.Length; i+= 2)
            {
                result.Add(values[i].ToString(), values[i + 1]);
            }

            return result;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
