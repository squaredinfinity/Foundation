using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SquaredInfinity.Presentation
{
    public abstract class ViewModelState { }

    public class BusyViewModelState : ViewModelState { }
    public class InitialisingViewModelState : BusyViewModelState { }
    public class ReadyViewModelState : ViewModelState { }
    public class ErroredViewModelState : ViewModelState { }

    public static class ViewModelStates
    {
        public static ViewModelState Busy = new BusyViewModelState();
        public static ViewModelState Initialising = new InitialisingViewModelState();
        public static ViewModelState Ready = new ReadyViewModelState();
        public static ViewModelState Errored = new ErroredViewModelState();
    }
}
