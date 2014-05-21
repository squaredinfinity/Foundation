using SquaredInfinity.Foundation.Presentation;
using SquaredInfinity.Foundation.Presentation.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using SquaredInfinity.Foundation.Extensions;
using System.Windows.Controls;

namespace Foundation.Presentation.Xaml.UITests.MVVM.ViewModelEvents
{
    public class TestView : View<TestViewModel>
    {
        public int MaxDepth
        {
            get { return (int)GetValue(MaxDepthProperty); }
            set { SetValue(MaxDepthProperty, value); }
        }

        public static readonly DependencyProperty MaxDepthProperty =
            DependencyProperty.Register(
            "MaxDepth", 
            typeof(int), 
            typeof(TestView), 
            new PropertyMetadata(1));

        protected override IHostAwareViewModel ResolveViewModel(Type viewType, object newDatacontext)
        {
            return new TestViewModel();
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if(MaxDepth > 0)
            {
                var child_content_presenter = this.FindVisualDescendant<ContentPresenter>("PART_ChildPresenter");


                var childView = new TestView();
                childView.MaxDepth = MaxDepth - 1;

                child_content_presenter.Content = childView;
            }
        }
    }
}
