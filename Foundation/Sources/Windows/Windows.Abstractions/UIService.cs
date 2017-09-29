using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace SquaredInfinity.Windows.Abstractions
{
    public class UIService : IUIService
    {
        #region Default

        static IUIService _default;
        /// <summary>
        /// Returns a default UIService.
        /// </summary>
        public static IUIService Default => LazyInitializer.EnsureInitialized<IUIService>(ref _default, DefaultServiceFactory);
        
        static Func<IUIService> _defaultServiceFactory = new Func<IUIService>(() => new UIService());
        /// <summary>
        /// Factory to create a new instance of default UI Service.
        /// Factory will be called only once and result of first invocation will be cached.
        /// Setting new service factory will invalidate previously cached IUService.
        /// </summary>
        public static Func<IUIService> DefaultServiceFactory
        {
            get { return _defaultServiceFactory; }
            set
            {
                _defaultServiceFactory = value;
                _default = null;
            }
        }

        #endregion

        public bool IsDesignTime => throw new NotImplementedException();

        public bool IsUIThread => UIDispatcher.CheckAccess();

        Dispatcher UIDispatcher { get; set; }

        public UIService()
            : this(GetMainThreadDispatcher(Application.Current))
        { }

        public UIService(Application app)
            : this(GetMainThreadDispatcher(app))
        { }

        public UIService(Dispatcher uiDispatcher)
        {
            this.UIDispatcher = uiDispatcher;
        }

        static Dispatcher GetMainThreadDispatcher(Application app)
        {
            if (app.Dispatcher != null)
                return Application.Current.Dispatcher;

            return Dispatcher.CurrentDispatcher;
        }

        public void ChangeDispatcher(Dispatcher newDispatcher)
        {
            this.UIDispatcher = newDispatcher;
        }

        public void Run(Action action)
        {
            if (IsUIThread)
            {
                action();
                return;
            }

            UIDispatcher.Invoke(action);
        }

        public async Task RunAsync(Action action)
        {
            await UIDispatcher.InvokeAsync(action);
        }
    }
}
