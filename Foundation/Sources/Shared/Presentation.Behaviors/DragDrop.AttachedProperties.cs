using System;
using System.Collections;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using SquaredInfinity.Foundation.Presentation.DragDrop.Utilities;
using System.Windows.Media.Imaging;
using SquaredInfinity.Foundation.Presentation.DragDrop;

namespace SquaredInfinity.Foundation.Presentation.Behaviors
{
    public static partial class DragDrop
    {
        #region Is Drag Source


        /// <summary>
        /// True if UIElement can act as Drag Source
        /// </summary>
        public static readonly DependencyProperty IsDragSourceProperty =
          DependencyProperty.RegisterAttached("IsDragSource", typeof(bool), typeof(DragDrop), new UIPropertyMetadata(false, OnIsDragSourceChanged));

        public static bool GetIsDragSource(UIElement target)
        {
            return (bool)target.GetValue(IsDragSourceProperty);
        }

        public static void SetIsDragSource(UIElement target, bool value)
        {
            target.SetValue(IsDragSourceProperty, value);
        }

        static void OnIsDragSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var uie = (UIElement)d;

            if ((bool)e.NewValue == true)
            {
                uie.PreviewMouseLeftButtonDown += DragSource_PreviewMouseLeftButtonDown;
                uie.PreviewMouseLeftButtonUp += DragSource_PreviewMouseLeftButtonUp;
                uie.PreviewMouseMove += DragSource_PreviewMouseMove;
                uie.QueryContinueDrag += DragSource_QueryContinueDrag;
            }
            else
            {
                uie.PreviewMouseLeftButtonDown -= DragSource_PreviewMouseLeftButtonDown;
                uie.PreviewMouseLeftButtonUp -= DragSource_PreviewMouseLeftButtonUp;
                uie.PreviewMouseMove -= DragSource_PreviewMouseMove;
                uie.QueryContinueDrag -= DragSource_QueryContinueDrag;
            }
        }

        #endregion

        #region Is Drop Target

        /// <summary>
        /// True if UI element can be a drop target
        /// </summary>
        public static readonly DependencyProperty IsDropTargetProperty =
          DependencyProperty.RegisterAttached("IsDropTarget", typeof(bool), typeof(DragDrop), new UIPropertyMetadata(false, OnIsDropTargetChanged));

        public static bool GetIsDropTarget(UIElement target)
        {
            return (bool)target.GetValue(IsDropTargetProperty);
        }

        public static void SetIsDropTarget(UIElement target, bool value)
        {
            target.SetValue(IsDropTargetProperty, value);
        }

        static void OnIsDropTargetChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var uie = (UIElement)d;

            if ((bool)e.NewValue == true)
            {
                uie.AllowDrop = true;

                if (uie is ItemsControl)
                {
                    uie.DragEnter += DropTarget_PreviewDragEnter;
                    uie.DragLeave += DropTarget_PreviewDragLeave;
                    uie.DragOver += DropTarget_PreviewDragOver;
                    uie.Drop += DropTarget_PreviewDrop;
                    uie.GiveFeedback += DropTarget_GiveFeedback;
                }
                else
                {
                    uie.PreviewDragEnter += DropTarget_PreviewDragEnter;
                    uie.PreviewDragLeave += DropTarget_PreviewDragLeave;
                    uie.PreviewDragOver += DropTarget_PreviewDragOver;
                    uie.PreviewDrop += DropTarget_PreviewDrop;
                    uie.PreviewGiveFeedback += DropTarget_GiveFeedback;
                }
            }
            else
            {
                uie.AllowDrop = false;

                if (uie is ItemsControl)
                {
                    uie.DragEnter -= DropTarget_PreviewDragEnter;
                    uie.DragLeave -= DropTarget_PreviewDragLeave;
                    uie.DragOver -= DropTarget_PreviewDragOver;
                    uie.Drop -= DropTarget_PreviewDrop;
                    uie.GiveFeedback -= DropTarget_GiveFeedback;
                }
                else
                {
                    uie.PreviewDragEnter -= DropTarget_PreviewDragEnter;
                    uie.PreviewDragLeave -= DropTarget_PreviewDragLeave;
                    uie.PreviewDragOver -= DropTarget_PreviewDragOver;
                    uie.PreviewDrop -= DropTarget_PreviewDrop;
                    uie.PreviewGiveFeedback -= DropTarget_GiveFeedback;
                }

                Mouse.OverrideCursor = null;
            }
        }

        #endregion




        #region Drag Adorner Template

        public static readonly DependencyProperty DragAdornerTemplateProperty =
            DependencyProperty.RegisterAttached(
            "DragAdornerTemplate",
            typeof(DataTemplate),
            typeof(DragDrop));

        public static DataTemplate GetDragAdornerTemplate(UIElement target)
        {
            return (DataTemplate)target.GetValue(DragAdornerTemplateProperty);
        }
        public static void SetDragAdornerTemplate(UIElement target, DataTemplate value)
        {
            target.SetValue(DragAdornerTemplateProperty, value);
        }

        #endregion

        #region Drag Adorner TemplateSelector

        public static readonly DependencyProperty DragAdornerTemplateSelectorProperty =
            DependencyProperty.RegisterAttached(
            "DragAdornerTemplateSelector",
            typeof(DataTemplateSelector),
            typeof(DragDrop),
            new PropertyMetadata(default(DataTemplateSelector)));

        public static void SetDragAdornerTemplateSelector(DependencyObject element, DataTemplateSelector value)
        {
            element.SetValue(DragAdornerTemplateSelectorProperty, value);
        }
        public static DataTemplateSelector GetDragAdornerTemplateSelector(DependencyObject element)
        {
            return (DataTemplateSelector)element.GetValue(DragAdornerTemplateSelectorProperty);
        }

        #endregion

        #region Use Default Drag Adorner

        public static readonly DependencyProperty UseDefaultDragAdornerProperty =
          DependencyProperty.RegisterAttached("UseDefaultDragAdorner", typeof(bool), typeof(DragDrop), new PropertyMetadata(false));

        public static bool GetUseDefaultDragAdorner(UIElement target)
        {
            return (bool)target.GetValue(UseDefaultDragAdornerProperty);
        }

        public static void SetUseDefaultDragAdorner(UIElement target, bool value)
        {
            target.SetValue(UseDefaultDragAdornerProperty, value);
        }

        #endregion

        #region Default Drag Adorner Opacity

        public static readonly DependencyProperty DefaultDragAdornerOpacityProperty =
          DependencyProperty.RegisterAttached("DefaultDragAdornerOpacity", typeof(double), typeof(DragDrop), new PropertyMetadata(0.8));

        public static double GetDefaultDragAdornerOpacity(UIElement target)
        {
            return (double)target.GetValue(DefaultDragAdornerOpacityProperty);
        }

        public static void SetDefaultDragAdornerOpacity(UIElement target, double value)
        {
            target.SetValue(DefaultDragAdornerOpacityProperty, value);
        }

        #endregion

        #region Use Default Effect Data Template

        public static readonly DependencyProperty UseDefaultEffectDataTemplateProperty =
         DependencyProperty.RegisterAttached("UseDefaultEffectDataTemplate", typeof(bool), typeof(DragDrop), new PropertyMetadata(false));

        public static bool GetUseDefaultEffectDataTemplate(UIElement target)
        {
            return (bool)target.GetValue(UseDefaultEffectDataTemplateProperty);
        }

        public static void SetUseDefaultEffectDataTemplate(UIElement target, bool value)
        {
            target.SetValue(UseDefaultEffectDataTemplateProperty, value);
        }

        #endregion

        #region Effect None Adorner Template

        public static readonly DependencyProperty EffectNoneAdornerTemplateProperty =
          DependencyProperty.RegisterAttached("EffectNoneAdornerTemplate", typeof(DataTemplate), typeof(DragDrop));

        public static DataTemplate GetEffectNoneAdornerTemplate(UIElement target)
        {
            var template = (DataTemplate)target.GetValue(EffectNoneAdornerTemplateProperty);

            if (template == null)
            {
                if (!GetUseDefaultEffectDataTemplate(target))
                {
                    return null;
                }

                // TODO: remove this
                //var imageSourceFactory = new FrameworkElementFactory(typeof(Image));
                //imageSourceFactory.SetValue(Image.SourceProperty, IconFactory.EffectNone);
                //imageSourceFactory.SetValue(FrameworkElement.HeightProperty, 12.0);
                //imageSourceFactory.SetValue(FrameworkElement.WidthProperty, 12.0);

                template = new DataTemplate();
                //template.VisualTree = imageSourceFactory;
            }

            return template;
        }

        public static void SetEffectNoneAdornerTemplate(UIElement target, DataTemplate value)
        {
            target.SetValue(EffectNoneAdornerTemplateProperty, value);
        }

        #endregion

        #region Effect Copy Adorner Template

        public static readonly DependencyProperty EffectCopyAdornerTemplateProperty =
         DependencyProperty.RegisterAttached("EffectCopyAdornerTemplate", typeof(DataTemplate), typeof(DragDrop));

        public static DataTemplate GetEffectCopyAdornerTemplate(UIElement target, string destinationText)
        {
            var template = (DataTemplate)target.GetValue(EffectCopyAdornerTemplateProperty);

            if (template == null)
            {
                template = CreateDefaultEffectDataTemplate(target, null, "Copy to", destinationText);
            }

            return template;
        }

        public static void SetEffectCopyAdornerTemplate(UIElement target, DataTemplate value)
        {
            target.SetValue(EffectCopyAdornerTemplateProperty, value);
        }

        #endregion

        #region Effect Move Adorner Template

        public static readonly DependencyProperty EffectMoveAdornerTemplateProperty =
          DependencyProperty.RegisterAttached("EffectMoveAdornerTemplate", typeof(DataTemplate), typeof(DragDrop));

        public static DataTemplate GetEffectMoveAdornerTemplate(UIElement target, string destinationText)
        {
            var template = (DataTemplate)target.GetValue(EffectMoveAdornerTemplateProperty);

            if (template == null)
            {
                template = CreateDefaultEffectDataTemplate(target, null, "Move to", destinationText);
            }

            return template;
        }

        public static void SetEffectMoveAdornerTemplate(UIElement target, DataTemplate value)
        {
            target.SetValue(EffectMoveAdornerTemplateProperty, value);
        }

        #endregion

        #region Drag Source

        public static readonly DependencyProperty DragSourceProperty =
          DependencyProperty.RegisterAttached("DragSource", typeof(IDragSource), typeof(DragDrop));

        public static IDragSource GetDragSource(UIElement target)
        {
            return (IDragSource)target.GetValue(DragSourceProperty);
        }

        public static void SetDragSource(UIElement target, IDragSource value)
        {
            target.SetValue(DragSourceProperty, value);
        }

        #endregion

        #region Drop Target

        public static readonly DependencyProperty DropHandlerProperty =
          DependencyProperty.RegisterAttached("DropHandler", typeof(IDropTarget), typeof(DragDrop));

        public static IDropTarget GetDropHandler(UIElement target)
        {
            return (IDropTarget)target.GetValue(DropHandlerProperty);
        }

        public static void SetDropHandler(UIElement target, IDropTarget value)
        {
            target.SetValue(DropHandlerProperty, value);
        }

        #endregion

        #region Drag Source Ignore

        public static readonly DependencyProperty DragSourceIgnoreProperty =
          DependencyProperty.RegisterAttached("DragSourceIgnore", typeof(bool), typeof(DragDrop), new PropertyMetadata(false));

        public static bool GetDragSourceIgnore(UIElement target)
        {
            return (bool)target.GetValue(DragSourceIgnoreProperty);
        }

        public static void SetDragSourceIgnore(UIElement target, bool value)
        {
            target.SetValue(DragSourceIgnoreProperty, value);
        }

        #endregion

        #region Drag Mouse Anchor Point

        /// <summary>
        /// DragMouseAnchorPoint defines the horizontal and vertical proportion at which the pointer will anchor on the DragAdorner.
        /// </summary>
        public static readonly DependencyProperty DragMouseAnchorPointProperty =
            DependencyProperty.RegisterAttached(
            "DragMouseAnchorPoint", 
            typeof(Point), 
            typeof(DragDrop),
            new PropertyMetadata(new Point(0, 1)));

        public static Point GetDragMouseAnchorPoint(UIElement target)
        {
            return (Point)target.GetValue(DragMouseAnchorPointProperty);
        }
        public static void SetDragMouseAnchorPoint(UIElement target, Point value)
        {
            target.SetValue(DragMouseAnchorPointProperty, value);
        }

        #endregion
    }
}