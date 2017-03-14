using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SquaredInfinity.Presentation.DragDrop
{
  public class KnownDropTargetAdorners
  {
    public static Type Insert
    {
      get { return typeof(DropTargetInsertionAdorner); }
    }
  }
}