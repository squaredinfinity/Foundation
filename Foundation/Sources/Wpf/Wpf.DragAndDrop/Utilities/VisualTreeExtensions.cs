using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Collections.Generic;
using System.Windows.Media.Media3D;
using SquaredInfinity.Foundation.Extensions;

namespace SquaredInfinity.Foundation.Presentation.DragDrop.Utilities
{
  public static class VisualTreeExtensions
  {
      public static IEnumerable<T> GetVisualDescendents<T>(this DependencyObject d) where T : DependencyObject
      {
          var childCount = VisualTreeHelper.GetChildrenCount(d);

          for (var n = 0; n < childCount; n++)
          {
              var child = VisualTreeHelper.GetChild(d, n);

              if (child is T)
              {
                  yield return (T)child;
              }

              foreach (var match in GetVisualDescendents<T>(child))
              {
                  yield return match;
              }
          }

          yield break;
      }
  }
}
