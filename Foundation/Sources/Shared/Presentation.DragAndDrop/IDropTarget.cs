using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace SquaredInfinity.Presentation.DragDrop
{
  /// <summary>
  /// Interface implemented by Drop Handlers.
  /// </summary>
  public interface IDropTarget
  {
    void DragOver(IDropInfo dropInfo);

    void Drop(IDropInfo dropInfo);
  }
}