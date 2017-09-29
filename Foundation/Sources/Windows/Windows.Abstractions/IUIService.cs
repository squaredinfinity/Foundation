using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace SquaredInfinity.Windows.Abstractions
{
    public interface IUIService
    {
        /// <summary>
        /// Gets a value indicating whether this application is in design time.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this lication is in design mode, <c>false</c> otherwise, <c>false</c>.
        /// </value>
        bool IsDesignTime { get; }

        /// <summary>
        /// Runs true if current thread is a UI thread.
        /// </summary>
        /// <param name="action">The action.</param>
        bool IsUIThread { get; }

        void ChangeDispatcher(Dispatcher newDispatcher);
        void Run(Action action);
        Task RunAsync(Action action);
    }
}
