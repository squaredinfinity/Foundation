using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Foundation
{
    /// <summary>
    /// Provides an implementation of Chain Of Command pattern based on event handlers.
    /// Event handlers will be fired in a sequence.
    /// If event handler can handle given command it should set args.Handled = true when done.
    /// </summary>
    public static class ChainOfCommand
    {
        /// <summary>
        /// Provides an implementation of Chain Of Command pattern based on event handlers.
        /// Event handlers will be fired in a sequence.
        /// If event handler can handle given command it should set args.Handled = true when done.
        /// </summary>
        /// <typeparam name="TEventArgs"></typeparam>
        /// <param name="eventHandler"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static bool TryHandle<TEventArgs>(this EventHandler<TEventArgs> eventHandler, TEventArgs args)
            where TEventArgs : CommandHandlerEventArgs
        {
            if (eventHandler == null)
                return false;

            var invocationList =
                eventHandler
                .GetInvocationList();

            for (int i = 0; i < invocationList.Length; i++)
            {
                var del = invocationList[i] as EventHandler<TEventArgs>;

                del(null, args);

                if (args.IsHandled)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
