using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace SquaredInfinity.Foundation.Presentation.DragDrop
{
  /// <summary>
  /// Interface implemented by Drag Handlers.
  /// </summary>
  public interface IDragSource
  {
    /// </remarks>
    void StartDrag(IDragInfo dragInfo);

    bool CanStartDrag(IDragInfo dragInfo);

    void Dropped(IDropInfo dropInfo);

    void DragCancelled();
  }
}